using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Monitor_2.Data;
using Monitor_2.Models.Currency;

namespace Monitor_2.Controllers
{
    public class CurrencyPairsController : Controller
    {
        private readonly Monitor_2Context _context;

        public CurrencyPairsController(Monitor_2Context context)
        {
            _context = context;
        }

        // GET: CurrencyPairs
        public async Task<IActionResult> Index()
        {
            return View(await _context.CurrencyPair.ToListAsync());
        }

        // GET: CurrencyPairs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currencyPair = await _context.CurrencyPair
                .FirstOrDefaultAsync(m => m.Id == id);
            if (currencyPair == null)
            {
                return NotFound();
            }

            return View(currencyPair);
        }

        // GET: CurrencyPairs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CurrencyPairs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] CurrencyPair currencyPair)
        {
            if (ModelState.IsValid)
            {
                _context.Add(currencyPair);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(currencyPair);
        }

        // GET: CurrencyPairs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currencyPair = await _context.CurrencyPair.FindAsync(id);
            if (currencyPair == null)
            {
                return NotFound();
            }
            return View(currencyPair);
        }

        // POST: CurrencyPairs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] CurrencyPair currencyPair)
        {
            if (id != currencyPair.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(currencyPair);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CurrencyPairExists(currencyPair.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(currencyPair);
        }

        // GET: CurrencyPairs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currencyPair = await _context.CurrencyPair
                .FirstOrDefaultAsync(m => m.Id == id);
            if (currencyPair == null)
            {
                return NotFound();
            }

            return View(currencyPair);
        }

        // POST: CurrencyPairs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currencyPair = await _context.CurrencyPair.FindAsync(id);
            if (currencyPair != null)
            {
                _context.CurrencyPair.Remove(currencyPair);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CurrencyPairExists(int id)
        {
            return _context.CurrencyPair.Any(e => e.Id == id);
        }
    }
}
