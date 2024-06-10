using Microsoft.AspNetCore.Mvc;
using Monitor_2.Services.DataServices;
using Monitor_2.Services.MathServices;
using Monitor_2.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Monitor_2.Controllers
{
    public class LineChartsController : Controller //LineCharts
    {
        private readonly CurrencyService _currencyService;
        private readonly PearsonCorrelation _pearsonCorrelation;

        public LineChartsController(CurrencyService currencyService, PearsonCorrelation pearsonCorrelation)
        {
            _currencyService = currencyService;
            _pearsonCorrelation = pearsonCorrelation;
        }

        // Додавання методу Index
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetRateHistory(string company, string pair, string rate)
        {
            if (string.IsNullOrEmpty(company) || string.IsNullOrEmpty(pair) || string.IsNullOrEmpty(rate))
            {
                return Json(new { error = "Invalid parameters" });
            }

            var rates = _currencyService.GetRateHistoryForCurrencyPairInCompany(company, pair, rate);

            if (rates == null || rates.Count == 0)
            {
                return Json(new { error = "Rates not found" });
            }

            var dates = rates.Select(ratePoint => ratePoint.X).ToList();
            var values = rates.Select(ratePoint => string.Format(CultureInfo.InvariantCulture, "{0:F2}", ratePoint.Y)).ToList();

            return Json(new { dates, rates = values });
        }

        [HttpGet]
        public JsonResult CalculateCorrelation(string company1, string pair1, string rate1, string company2, string pair2, string rate2)
        {
            if (string.IsNullOrEmpty(company1) || string.IsNullOrEmpty(pair1) || string.IsNullOrEmpty(rate1) || string.IsNullOrEmpty(company2) || string.IsNullOrEmpty(pair2) || string.IsNullOrEmpty(rate2))
            {
                return Json(new { error = "Invalid parameters" });
            }

            var rates1 = _currencyService.GetRateHistoryForCurrencyPairInCompany(company1, pair1, rate1);
            var rates2 = _currencyService.GetRateHistoryForCurrencyPairInCompany(company2, pair2, rate2);

            if (rates1 == null || rates2 == null || rates1.Count == 0 || rates2.Count == 0)
            {
                return Json(new { error = "Rates not found" });
            }

            var correlation = _pearsonCorrelation.CalculateCorrelation(rates1, rates2);
            //correlation = correlation * 100;

            //string result = string.Format(CultureInfo.InvariantCulture, "{0:F2}%", correlation * 100);

            return Json(new { correlation });
        }
    }
}
