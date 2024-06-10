using Microsoft.AspNetCore.Mvc;
using Monitor_2.Data;
using Monitor_2.Models.Currency;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Monitor_2.Controllers
{
    public class SeedCurrencyDataController : Controller
    {
        private readonly Monitor_2Context _context;

        public SeedCurrencyDataController(Monitor_2Context context)
        {
            _context = context;
        }

        // GET: SeedCurrencyData
        public IActionResult Index()
        {
            // Вставка валютних пар (уникнення дублікатів)
            if (!_context.CurrencyPair.Any())
            {
                var currencyPairs = new List<CurrencyPair>
                {
                    new CurrencyPair { Name = "USD-UAH" },
                    new CurrencyPair { Name = "EUR-UAH" },
                    new CurrencyPair { Name = "GBP-UAH" },
                    new CurrencyPair { Name = "CHF-UAH" }
                };

                _context.CurrencyPair.AddRange(currencyPairs);
                _context.SaveChanges();
            }

            // Вставка компаній обміну
            if (!_context.ExchangeCompany.Any())
            {
                var exchangeCompanies = new List<ExchangeCompany>
                {
                    //new ExchangeCompany { Name = "Change", SiteUrl = "https://change.kiev.ua/", Addresses = new List<string>
                    //{
                    //    "вулиця Василя Липківського, 8, Київ",
                    //    "проспект Повітряних Сил, 34/1, Київ",
                    //    "вулиця Солом'янська, 8, Київ",
                    //    "вулиця Солом'янська, 14, Київ"
                    //}},
                    //new ExchangeCompany { Name = "Garant", SiteUrl = "https://garant.money/", Addresses = new List<string>
                    //{
                    //    "вулиця В'ячеслава Чорновола, 29а, Київ",
                    //    "вулиця Січових Стрільців, 77, Київ",
                    //    "Голосіївський проспект, 50, Київ",
                    //    "Голосіївський проспект, 27, Київ",
                    //    "вулиця Дорогожицька, 17, Київ",
                    //    "бульвар Лесі Українки, 34, Київ",
                    //    "проспект Леся Курбаса, 6Г, Київ",
                    //    "вулиця Данила Щербаківського, 64, Київ",
                    //    "проспект Свободи, 15/1, Київ",
                    //    "Оболонський проспект, 35, Київ"
                    //}},
                    //new ExchangeCompany { Name = "IFS", SiteUrl = "https://obmenvalut.com.ua/", Addresses = new List<string>
                    //{
                    //    "м. Київ, площ. Бессарабська, 2",
                    //    "м. Київ, вул. Софіївська 5",
                    //    "м. Київ, вул. Сагайдачного, 14в",
                    //    "м. Київ, вул. Сагайдачного, 41",
                    //    "м. Київ, вул. Глубочицька, 29-31",
                    //    "м. Київ, прт. Науки, 1",
                    //    "м. Київ, просп. Берестейський, 136Ж",
                    //    "Вул. Героїв полку «Азов» 12",
                    //    "м. Київ, прт.Бажана, 40"
                    //}}
                };

                _context.ExchangeCompany.AddRange(exchangeCompanies);
                _context.SaveChanges();
            }            

            return Ok("Database seeded successfully.");
        }

        // Вставка поточних значень валютних пар
        // GET: SeedCurrencyData/AddMoreData
        public IActionResult AddMoreData()
        {
            var currencyPairs = _context.CurrencyPair.ToList();
            var exchangeCompanies = _context.ExchangeCompany.ToList();

            var currentCpValues = new List<CurrentCpValue>
            {
                new CurrentCpValue { BuyRate = 39.78M, SellRate = 40.02M, CurrencyPairId = currencyPairs.First(cp => cp.Name == "USD-UAH").Id, ExchangeCompanyId = exchangeCompanies.First(ec => ec.Name == "Garant").Id, ReleaseDate = DateTime.Now },
                new CurrentCpValue { BuyRate = 39.80M, SellRate = 40.04M, CurrencyPairId = currencyPairs.First(cp => cp.Name == "USD-UAH").Id, ExchangeCompanyId = exchangeCompanies.First(ec => ec.Name == "Garant").Id, ReleaseDate = DateTime.Now },
                new CurrentCpValue { BuyRate = 39.80M, SellRate = 40.00M, CurrencyPairId = currencyPairs.First(cp => cp.Name == "USD-UAH").Id, ExchangeCompanyId = exchangeCompanies.First(ec => ec.Name == "Garant").Id, ReleaseDate = DateTime.Now },
                new CurrentCpValue { BuyRate = 39.80M, SellRate = 39.98M, CurrencyPairId = currencyPairs.First(cp => cp.Name == "USD-UAH").Id, ExchangeCompanyId = exchangeCompanies.First(ec => ec.Name == "Garant").Id, ReleaseDate = DateTime.Now },
                new CurrentCpValue { BuyRate = 39.74M, SellRate = 39.96M, CurrencyPairId = currencyPairs.First(cp => cp.Name == "USD-UAH").Id, ExchangeCompanyId = exchangeCompanies.First(ec => ec.Name == "Garant").Id, ReleaseDate = DateTime.Now },

                new CurrentCpValue { BuyRate = 39.87M, SellRate = 40.00M, CurrencyPairId = currencyPairs.First(cp => cp.Name == "USD-UAH").Id, ExchangeCompanyId = exchangeCompanies.First(ec => ec.Name == "IFS").Id, ReleaseDate = DateTime.Now },
                new CurrentCpValue { BuyRate = 39.90M, SellRate = 40.05M, CurrencyPairId = currencyPairs.First(cp => cp.Name == "USD-UAH").Id, ExchangeCompanyId = exchangeCompanies.First(ec => ec.Name == "IFS").Id, ReleaseDate = DateTime.Now },
                new CurrentCpValue { BuyRate = 39.80M, SellRate = 39.95M, CurrencyPairId = currencyPairs.First(cp => cp.Name == "USD-UAH").Id, ExchangeCompanyId = exchangeCompanies.First(ec => ec.Name == "IFS").Id, ReleaseDate = DateTime.Now },
                new CurrentCpValue { BuyRate = 39.80M, SellRate = 39.95M, CurrencyPairId = currencyPairs.First(cp => cp.Name == "USD-UAH").Id, ExchangeCompanyId = exchangeCompanies.First(ec => ec.Name == "IFS").Id, ReleaseDate = DateTime.Now },
                new CurrentCpValue { BuyRate = 39.80M, SellRate = 39.95M, CurrencyPairId = currencyPairs.First(cp => cp.Name == "USD-UAH").Id, ExchangeCompanyId = exchangeCompanies.First(ec => ec.Name == "IFS").Id, ReleaseDate = DateTime.Now },
            };

            _context.CurrentCpValue.AddRange(currentCpValues);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
