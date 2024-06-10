using Monitor_2.Data;
using Monitor_2.DataTransferObjects;
using Monitor_2.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Monitor_2.Models.Currency;

namespace Monitor_2.Services.DataServices
{
    public class CurrencyService //Діставання історій зміни різних валютних пар з різних компаній
    {                           //у вигляді CurrentCpValue або DataPoint
        private readonly Monitor_2Context _context;

        public CurrencyService(Monitor_2Context context)
        {
            _context = context;
        }

        //отримання історії у вигляді списку CurrentCpValue (які містять все)
        public List<CurrentCpValue> GetRateHistoryForCurrencyPairInCompany(string currencyPairName, string exchangeCompanyName)
        {
            // Знаходимо Id валютної пари за назвою
            int currencyPairId = _context.CurrencyPair
                                        .Where(cp => cp.Name == currencyPairName)
                                        .Select(cp => cp.Id)
                                        .FirstOrDefault();

            // Знаходимо Id компанії обміну за назвою
            int exchangeCompanyId = _context.ExchangeCompany
                                            .Where(ec => ec.Name == exchangeCompanyName)
                                            .Select(ec => ec.Id)
                                            .FirstOrDefault();

            if (currencyPairId == 0 || exchangeCompanyId == 0)
            {
                // Якщо валютна пара або компанія з такою назвою не знайдені, повертаємо порожній список
                return new List<CurrentCpValue>();
            }

            // Знаходимо всі об'єкти миттєвих значень для даної валютної пари та компанії обміну
            List<CurrentCpValue> currentCpValues = _context.CurrentCpValue
                                                           .Where(c => c.CurrencyPairId == currencyPairId && c.ExchangeCompanyId == exchangeCompanyId)
                                                           .OrderBy(c => c.ReleaseDate)
                                                           .ToList();

            return currentCpValues;
        }

        public List<DataPoint> GetRateHistoryForCurrencyPairInCompany(string exchangeCompanyName, string currencyPairName, string RateType)
        {
            var currentCpValues = GetRateHistoryForCurrencyPairInCompany(currencyPairName, exchangeCompanyName);

            switch (RateType)
            {
                case "Buy":
                    var dataPoints = currentCpValues.Select(c => new DataPoint
                    {
                        X = c.ReleaseDate.ToString("yyyy-MM-dd"),
                        Y = c.BuyRate.ToString("0.00", CultureInfo.InvariantCulture)
                    }).ToList();
                    return dataPoints;

                case "Sell":
                    dataPoints = currentCpValues.Select(c => new DataPoint
                    {
                        X = c.ReleaseDate.ToString("yyyy-MM-dd"),
                        Y = c.SellRate.ToString("0.00", CultureInfo.InvariantCulture)
                    }).ToList();
                    return dataPoints;

                case null:
                    throw new ArgumentException("RateType is null");
                case "":
                    throw new ArgumentException("RateType is empty string");

                default:
                    throw new ArgumentException($"Wrong RateType entered: '{RateType}'. It must be 'Buy' or 'Sell'");
            }

        }

        //переписування моментів часу зі значеннями курсу продажу з CurrentCpValue в DataPoint
        //public List<DataPoint> GetSellRateHistoryForCurrencyPairInCompany(string currencyPairName, string exchangeCompanyName)
        //{
        //    var currentCpValues = GetRateHistoryForCurrencyPairInCompany(currencyPairName, exchangeCompanyName);

        //    var dataPoints = currentCpValues.Select(c => new DataPoint
        //    {
        //        X = c.ReleaseDate.ToString("yyyy-MM-dd"),
        //        Y = c.SellRate.ToString("0.00", CultureInfo.InvariantCulture)
        //    }).ToList();

        //    return dataPoints;
        //}

        ////переписування моментів часу зі значеннями курсу купівлі з CurrentCpValue в DataPoint
        //public List<DataPoint> GetBuyRateHistoryForCurrencyPairInCompany(string currencyPairName, string exchangeCompanyName)
        //{
        //    var currentCpValues = GetRateHistoryForCurrencyPairInCompany(currencyPairName, exchangeCompanyName);

        //    var dataPoints = currentCpValues.Select(c => new DataPoint
        //    {
        //        X = c.ReleaseDate.ToString("yyyy-MM-dd"),
        //        Y = c.BuyRate.ToString("0.00", CultureInfo.InvariantCulture)
        //    }).ToList();

        //    return dataPoints;
        //}
    }
}
