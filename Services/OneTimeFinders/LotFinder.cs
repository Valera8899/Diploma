using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Monitor_2.Models.Shopping;
using Monitor_2.Data;
using Monitor_2.Services.LotParsers;
using System.Net;

namespace Monitor_2.Services.OneTimeFinders
{
    public class LotFinder
    {
        // Виконує пошук на сторінках знайдених лотів, залазячи в описи
        public static List<Lot> MakeParameterizedSearch(List<Lot> lotList, Monitor_2Context context, List<string> keywords/*, decimal minPrice, decimal maxPrice, string[] marketPlaces*/)
        {
            //Parallel.ForEach(lotList, new ParallelOptions { MaxDegreeOfParallelism = 5 }, lot =>
            //{
            //    Thread.Sleep(new Random().Next(1000, 3000)); // Затримка для демонстраційних цілей

            //    // Парсинг опису товару на ключові слова
            //    var foundKeywords = Prom_Parser.ParseProductPage(lot.Url, keywords);

            //    // Вивід значення foundKeywords для відлагодження
            //    Console.WriteLine($"Keywords found for {lot.Url}: {string.Join(", ", foundKeywords)}");

            //    // Додавання знайдених ключових слів до лота
            //    lot.FoundKeywords = foundKeywords ?? new List<string>();
            //});
            for (int i = lotList.Count - 1; i >= 0; i--)
            {
                var lot = lotList[i];
                // Парсинг опису товару на ключові слова
                var foundKeywords = Prom_Parser.ParseProductPage(lot.Url, keywords);

                // Вивід значення foundKeywords для відлагодження
                Console.WriteLine($"Keywords found for {lot.Url}: {string.Join(", ", foundKeywords)}");

                if (foundKeywords == null || !foundKeywords.Any())
                {
                    // Видалення лота, якщо не знайдено жодного ключового слова
                    lotList.RemoveAt(i);
                }
                else
                {
                    // Додавання знайдених ключових слів до лота
                    lot.FoundKeywords = foundKeywords;
                }
            }

            return lotList;
        }

        // Виконує пошук, запускаючи парсери для всіх маркетплейсів із переліку та об'єднуючи результати з усіх парсерів у List<Lot>
        public static List<Lot> MakeSearch(string searchQuery, Monitor_2Context context)
        {
            // Парсинг лотів (зараз викликається лише парсер Prom)
            List<Lot> lotList = Prom_Parser.ParseProm(searchQuery);

            // Завантаження об'єктів Marketplace
            foreach (var lot in lotList)
            {
                lot.Marketplace = context.Marketplaces.FirstOrDefault(m => m.Id == lot.MarketplaceId);
            }

            return lotList;
        }

        public static string CreateSearchUrl_Plus(string baseQueryUrl, string searchQuery) // Creates URL with +
        {
            string formattedQuery = string.Join("+", searchQuery.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return $"{baseQueryUrl}{formattedQuery}";
        }

        public static string CreateSearchUrl_P20(string baseQueryUrl, string searchQuery) // Creates URL with %20
        {
            string formattedQuery = string.Join("%20", searchQuery.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return $"{baseQueryUrl}{formattedQuery}";
        }

        public static string GetHtmlFromUrl(string url)
        {
            string html;

            try
            {
                // Створення об'єкту для відправки запитів на веб-сервер
                WebClient client = new WebClient();

                // Отримання HTML-коду сторінки за заданим URL
                html = client.DownloadString(url);

                // Закриття з'єднання з веб-сервером
                client.Dispose();
            }
            catch (Exception ex)
            {
                // Вивід повідомлення про помилку у консоль
                Console.WriteLine($"Помилка при отриманні HTML: {ex.Message}");
                html = string.Empty; // Якщо сталася помилка, повертаємо пустий рядок
            }

            return html;
        }
    }
}
