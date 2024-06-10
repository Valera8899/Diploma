using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Net;

using Monitor_2.Models.Shopping;
using Monitor_2.Services.OneTimeFinders;

namespace Monitor_2.Services.LotParsers
{
    public class Prom_Parser
    {
        public static List<Lot> ParseProm(string searchQuery)
        {
            // Формування URL
            string baseUrl = "https://prom.ua/ua/search?search_term=";
            string url = LotFinder.CreateSearchUrl_P20(baseUrl, searchQuery);

            // Завантаження HTML за сформованим URL
            string pageHtml = LotFinder.GetHtmlFromUrl(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(pageHtml);

            List<Lot> lotList = new List<Lot>();

            var scriptNodes = htmlDocument.DocumentNode.SelectNodes("//script[contains(text(), 'Product')]");

            if (scriptNodes != null)
            {
                foreach (var scriptNode in scriptNodes)
                {
                    // Витягуємо текст з тега <script>
                    string scriptText = scriptNode.InnerText.Trim();

                    // Парсимо JSON
                    try
                    {
                        JObject json = JObject.Parse(scriptText);

                        // Отримуємо назву товару
                        string title = json["name"].ToString();

                        // Отримуємо посилання на сторінку товару
                        string productUrl = json["url"].ToString();

                        // Отримуємо ціну товару
                        string priceText = json["offers"]["price"].ToString();
                        decimal.TryParse(priceText.Replace("грн", "").Replace(" ", "").Replace(",", "."), out decimal price);

                        // Отримуємо перше фото товару
                        string imageUrl = json["image"].First.ToString();

                        lotList.Add(new Lot
                        {
                            Name = title,
                            Price = price,
                            Url = productUrl,
                            PhotoUrl = imageUrl,
                            //PublicationDate = new DateTime(9999, 1, 1) // Default date if not found
                            
                            MarketplaceId = 1
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Помилка при парсингу JSON: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Товари не знайдені.");
            }

            return lotList;
        }

        //повертає лише ті ключові слова, що знайшлись на сторінці лота
        public static List<string> ParseProductPage(string productUrl, List<string> keywords)
        {
            List<string> foundKeywords = new List<string>();

            try
            {
                var web = new HtmlWeb();
                var htmlDoc = web.Load(productUrl);

                // Find the div containing the description
                var descriptionDiv = htmlDoc.DocumentNode.SelectSingleNode("//div[@data-qaid='descriptions']");

                if (descriptionDiv != null)
                {
                    // Get the text under the description
                    var descriptionText = descriptionDiv.InnerText.Trim();

                    // Check for each keyword
                    foreach (var keyword in keywords)
                    {
                        if (descriptionText.Contains(keyword))
                        {
                            foundKeywords.Add(keyword);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Не вдалося знайти блок з описом товару.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при парсингу сторінки товару: {ex.Message}");
            }

            return foundKeywords;
        }

        public static decimal ParseProductPagePrice(string productUrl)//повертає лише ЦІНУ лота
        {
            try
            {
                var web = new HtmlWeb();
                var htmlDoc = web.Load(productUrl);

                // Find all nodes with data-qaid='product_price'
                var priceNodes = htmlDoc.DocumentNode.SelectNodes("//div[@data-qaid='product_price']");

                if (priceNodes != null && priceNodes.Count > 0)
                {
                    // Get the last node in the list
                    var lastPriceNode = priceNodes.Last();
                    string priceText = lastPriceNode.GetAttributeValue("data-qaprice", "0").Trim();
                    if (decimal.TryParse(priceText, out decimal price))
                    {
                        return price;
                    }
                    else
                    {
                        Console.WriteLine("Не вдалося розпарсити ціну.");
                    }
                }
                else
                {
                    Console.WriteLine("Не вдалося знайти жодного вузла з ціною.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при парсингу сторінки товару: {ex.Message}");
            }

            return 0.0m; // Return 0 if the price cannot be found or parsed
        }
    }
}
