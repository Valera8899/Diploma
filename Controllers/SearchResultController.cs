using Microsoft.AspNetCore.Mvc;
using Monitor_2.Models.Shopping;
using Monitor_2.Data;
using Monitor_2.Services.OneTimeFinders;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Monitor_2.Controllers
{
    public class SearchResultController : Controller
    {
        private readonly Monitor_2Context _context;
        private readonly int recentSearchesCount = 3;
        private readonly UserManager<User> _userManager;
        private IMemoryCache _cache;

        public SearchResultController(Monitor_2Context context, UserManager<User> userManager, IMemoryCache memoryCache)
        {
            _context = context;
            _userManager = userManager;
            _cache = memoryCache;
        }

        [HttpGet]
        public IActionResult Index(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return RedirectToAction("Index", "Home");
            }

            var search = new Search
            {
                SearchQuery = searchQuery,
                MinPrice = null,
                MaxPrice = null
            };

            _context.Searches.Add(search);
            _context.SaveChanges();

            var firstFoundLots = _cache.Get<List<Lot>>("FirstFoundLots");

            if (firstFoundLots == null)
            {
                firstFoundLots = LotFinder.MakeSearch(searchQuery, _context);

                foreach (var lot in firstFoundLots)
                {
                    lot.Marketplace = _context.Marketplaces.FirstOrDefault(m => m.Id == lot.MarketplaceId);
                }

                _cache.Set("FirstFoundLots", firstFoundLots);
            }

            ViewBag.SearchQuery = searchQuery;
            ViewBag.SearchId = search.Id;
            return View(firstFoundLots);
        }

        [HttpPost]
        public IActionResult AddKeywords(int searchId, string keywords)
        {
            var firstFoundLots = _cache.Get<List<Lot>>("FirstFoundLots");

            if (firstFoundLots == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var search = _context.Searches.FirstOrDefault(s => s.Id == searchId);
            if (search == null)
            {
                return NotFound();
            }

            var keywordList = keywords.Split('\n').Where(k => !string.IsNullOrWhiteSpace(k)).Select(k => new KeyWord { Word = k.Trim(), SearchId = searchId }).ToList();

            _context.KeyWords.AddRange(keywordList);
            _context.SaveChanges();

            var lots = LotFinder.MakeParameterizedSearch(firstFoundLots, _context, keywordList.Select(k => k.Word).ToList());

            ViewBag.SearchQuery = search.SearchQuery;
            ViewBag.Keywords = keywordList.Select(k => k.Word).ToList();
            return View("Index", lots);
        }

        [HttpPost]
        public async Task<IActionResult> SaveLot(string lotUrl)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                // Пошук лоту за URL у базі даних
                var existingLot = await _context.Lots.Include(l => l.Marketplace)
                                                      .FirstOrDefaultAsync(l => l.Url == lotUrl);
                if (existingLot != null)
                {
                    // Перевірка, чи лот вже збережений у користувача
                    if (_context.User_Lots.Any(ul => ul.UserId == user.Id && ul.LotId == existingLot.Id))
                    {
                        return Ok(new { message = "Лот вже є у збережених" });
                    }

                    // Зв'язування лоту з користувачем
                    var userLot = new User_Lot
                    {
                        UserId = user.Id,
                        LotId = existingLot.Id
                    };
                    _context.User_Lots.Add(userLot);
                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Лот збережено" });
                }
                else
                {
                    // Лот не знайдено у базі даних, спробувати знайти його у кеші
                    var cachedLots = _cache.Get<List<Lot>>("FirstFoundLots");
                    if (cachedLots != null)
                    {
                        var newLot = cachedLots.FirstOrDefault(l => l.Url == lotUrl);
                        if (newLot != null)
                        {
                            // Перевірка, чи існує відповідний Marketplace у базі даних
                            var existingMarketplace = await _context.Marketplaces
                                                                    .FirstOrDefaultAsync(m => m.SiteUrl == newLot.Marketplace.SiteUrl);
                            if (existingMarketplace != null)
                            {
                                newLot.MarketplaceId = existingMarketplace.Id;
                                newLot.Marketplace = existingMarketplace;
                            }

                            // Збереження нового лоту
                            _context.Lots.Add(newLot);
                            await _context.SaveChangesAsync();

                            // Зв'язування нового лоту з користувачем
                            var userLot = new User_Lot
                            {
                                UserId = user.Id,
                                LotId = newLot.Id
                            };
                            _context.User_Lots.Add(userLot);
                            await _context.SaveChangesAsync();

                            // Видалення лоту з кешу
                            _cache.Remove("FirstFoundLots");

                            return Ok(new { message = "Лот збережено" });
                        }
                    }

                    return NotFound(); // Лот не знайдено в кеші
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error saving lot: " + ex.Message);
                return StatusCode(500, "Помилка збереження лоту: " + ex.Message);
            }
        }



        [HttpGet]
        public async Task<IActionResult> SavedLots()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var savedLots = await _context.User_Lots
                                           .Where(ul => ul.UserId == user.Id)
                                           .Include(ul => ul.Lot)
                                           .ThenInclude(l => l.Marketplace)
                                           .Select(ul => ul.Lot)
                                           .ToListAsync();

            return View(savedLots);
        }


        [HttpGet]
        public async Task<IActionResult> RecentSearches()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var recentSearches = await _context.User_Searches
                .Where(us => us.UserId == user.Id)
                .OrderByDescending(us => us.SearchId)
                .Take(recentSearchesCount)
                .Select(us => us.Search)
                .ToListAsync();

            return View(recentSearches);
        }
    }
}
