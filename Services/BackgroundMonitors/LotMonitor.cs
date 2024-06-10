using Monitor_2.Data;
using Monitor_2.Models.Shopping;
using Monitor_2.Services.LotParsers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Monitor_2.Services.BackgroundMonitors
{
    public class LotMonitor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LotMonitor> _logger;
        private static System.Timers.Timer _timer;

        private static WebClient _webClient = new WebClient();
        private DateTime lastUpdateTime = DateTime.Now;

        public LotMonitor(IServiceProvider serviceProvider, ILogger<LotMonitor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int seconds = 15;//180;
            int interval = seconds * 1000;

            _timer = new System.Timers.Timer
            {
                Interval = interval,
                AutoReset = true,
                Enabled = true
            };

            _timer.Elapsed += OnTimedEvent;

            await Task.CompletedTask;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            lastUpdateTime = DateTime.Now;

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<Monitor_2Context>();

                var parserInfoList = new List<(Func<string, decimal> parser, string marketplaceName)>()
                {
                    (Prom_Parser.ParseProductPagePrice, "prom")
                };

                foreach (var parserInfo in parserInfoList)
                {
                    ProcessMarketplace(dbContext, parserInfo.parser, parserInfo.marketplaceName, lastUpdateTime);
                }
            }
        }

        private void ProcessMarketplace(Monitor_2Context dbContext, Func<string, decimal> priceParser, string marketplaceName, DateTime recordingDate)
        {
            var marketplace = dbContext.Marketplaces.SingleOrDefault(m => m.Name == marketplaceName);

            if (marketplace == null)
            {
                _logger.LogWarning($"Marketplace {marketplaceName} not found in the database.");
                return;
            }

            var lots = dbContext.Lots.Where(l => l.MarketplaceId == marketplace.Id).ToList();

            foreach (var lot in lots)
            {
                try
                {
                    decimal currentPrice = priceParser(lot.Url);
                    var priceRecord = new CurrentPriceValue
                    {
                        LotId = lot.Id,
                        RecordingDate = recordingDate,
                        Price = currentPrice
                    };
                    dbContext.CurrentPriceValues.Add(priceRecord);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing lot {lot.Id} from marketplace {marketplaceName}: {ex.Message}");
                }
            }

            dbContext.SaveChanges();
        }

        private static string GetHtmlFromUrl(string url)
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
                    throw new Exception($"Error downloading html from {url}: {(int)response.StatusCode} {response.StatusDescription}");
                }
                else
                {
                    throw new Exception($"Error downloading html from {url}: {webEx.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error downloading html from {url}: {ex.Message}");
            }

            return html;
        }
    }
}
