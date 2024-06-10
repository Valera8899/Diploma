using Microsoft.AspNetCore.Mvc;
using Monitor_2.Services.DataServices;
using System.Linq;

namespace Monitor_2.Controllers
{
    public class TestExtractDataController : Controller
    {
        private readonly CurrencyService _currencyService;

        public TestExtractDataController(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public IActionResult Index()
        {
            // Отримуємо історію sellRate для пари USD-UAH
            //var sellRates = _currencyService.GetSellRateHistoryForCurrencyPair("USD-UAH", "Garant");//тепер воно вже не повертає List decimal
            var a = _currencyService.GetRateHistoryForCurrencyPairInCompany("Garant", "USD-UAH", "Sell");


            var sellRates = "there no arrays or other data yet";

            // Перетворюємо список на рядок з пробілами
            var sellRatesString = string.Join(" ", sellRates);

            // Повертаємо рядок
            return Ok(sellRatesString);
        }
    }
}
