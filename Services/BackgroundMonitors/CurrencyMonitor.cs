using Monitor_2.Data;
using Monitor_2.Models.Currency;
using Monitor_2.Services.CurrencyParsers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Timers;

namespace Monitor_2.Services.BackgroundMonitors
{
    public class CurrencyMonitor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CurrencyMonitor> _logger;
        private static System.Timers.Timer _timer;

        private static WebClient _webClient = new WebClient();
        private DateTime lastUpdateTime = DateTime.Now; //просто ініціалізація поля

        public CurrencyMonitor(IServiceProvider serviceProvider, ILogger<CurrencyMonitor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int seconds = 180; //в чистовій версії має буть >120 сек. В ідеалі реалізувати, аби працювало із 60
            int interval = seconds * 1000;

            _timer = new System.Timers.Timer
            {
                Interval = interval,
                AutoReset = true,
                Enabled = true
            };

            //_timer.Elapsed += OnTimedEvent; //РОЗКОМЕНТУВАТИ

            await Task.CompletedTask;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            lastUpdateTime = DateTime.Now; // Оновлення часу останнього оновлення

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<Monitor_2Context>();

                var parserInfoList = new List<(object parser, string companyName, string url)>()
                {
                    (new Change_Parser(), "Change", "https://change.kiev.ua/"),
                    (new GarantParser(), "Garant", "https://garant.money/"),
                    (new IFS_Parser(), "IFS", "https://obmenvalut.com.ua/"),                    
                };

                foreach (var parserInfo in parserInfoList)
                {
                    ProcessCompany(dbContext, parserInfo.parser, parserInfo.companyName, parserInfo.url, lastUpdateTime);
                }
            }
        }

        private void ProcessCompany(Monitor_2Context dbContext, object parser, string companyName, string url, DateTime updateTime)
        {
            string html = GetHtmlFromUrl(url);

            string[,] currencyTable;
            if (parser is GarantParser)
            {
                currencyTable = ((GarantParser)parser).ParseGarant(html);
            }
            else if (parser is IFS_Parser)
            {
                currencyTable = ((IFS_Parser)parser).ParseIFS(html);
            }
            else if (parser is Change_Parser)
            {
                currencyTable = ((Change_Parser)parser).ParseChange(html);
            }
            else
            {
                throw new InvalidOperationException("Unknown parser type.");
            }

            var currencyPairs = dbContext.CurrencyPair.ToList();
            var exchangeCompany = dbContext.ExchangeCompany.FirstOrDefault(ec => ec.Name == companyName);

            if (exchangeCompany != null)
            {
                for (int i = 0; i < currencyTable.GetLength(0); i++)
                {
                    string currencyName = currencyTable[i, 0];

                    if (decimal.TryParse(currencyTable[i, 1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal buyRate) &&
                        decimal.TryParse(currencyTable[i, 2], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal sellRate))
                    {
                        var currencyPair = currencyPairs.FirstOrDefault(cp => cp.Name == currencyName);

                        if (currencyPair == null)
                        {
                            currencyPair = currencyPairs.FirstOrDefault(cp => cp.Name.Contains(currencyName));
                        }

                        if (currencyPair != null)
                        {
                            var currentCpValue = new CurrentCpValue
                            {
                                BuyRate = buyRate,
                                SellRate = sellRate,
                                CurrencyPairId = currencyPair.Id,
                                ExchangeCompanyId = exchangeCompany.Id,
                                ReleaseDate = updateTime
                            };

                            dbContext.CurrentCpValue.Add(currentCpValue);
                        }
                        else
                        {
                            _logger.LogWarning($"Currency pair for {currencyName} not found in the database.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to parse rates for {currencyName}: BuyRate = {currencyTable[i, 1]}, SellRate = {currencyTable[i, 2]}.");
                    }
                }

                dbContext.SaveChanges();
            }
            else
            {
                _logger.LogError($"Exchange company '{companyName}' not found in the database.");
            }
        }

        static string GetHtmlFromUrl(string url)
        {
            string html;

            try
            {
                _webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
                html = _webClient.DownloadString(url);
            }
            catch (WebException webEx)
            {
                if (webEx.Response is HttpWebResponse response)
                {
                    throw new Exception($"Помилка при скачуванні html {url} : {(int)response.StatusCode} {response.StatusDescription}");
                }
                else
                {
                    throw new Exception($"Помилка при скачуванні html {url} : {webEx.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка при скачуванні html {url} :  {ex.Message}");
            }

            return html;
        }

    }
}
