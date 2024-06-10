using HtmlAgilityPack;

namespace Monitor_2.Services.CurrencyParsers
{
    public class IFS_Parser
    {
        public string[,] ParseIFS(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            int currencyPairsCount = 4; // Кількість валютних пар
            int tableColsCount = 3; // Кількість стовпців таблиці
            string[,] currencyTable = new string[currencyPairsCount, tableColsCount];

            for (int i = 1; i <= currencyPairsCount; i++)
            {
                // Отримання назви валютної пари
                HtmlNode currencyNameNode = htmlDocument.DocumentNode.SelectSingleNode($"/html/body/main/section[1]/div/div[3]/div[2]/ul/li[{i}]/div[1]/p");
                string currencyName = currencyNameNode?.InnerText.Trim() ?? "";
                if (!string.IsNullOrEmpty(currencyName) && currencyName.Length > 5)
                {
                    currencyName = currencyName.Substring(6); // Обрізаємо назву валютної пари до трьох букв
                }

                // Отримання курсу купівлі
                HtmlNode buyRateNode = htmlDocument.DocumentNode.SelectSingleNode($"/html/body/main/section[1]/div/div[3]/div[2]/ul/li[{i}]/div[2]/p");
                string buyRate = buyRateNode?.InnerText.Trim() ?? "";

                // Отримання курсу продажу
                HtmlNode sellRateNode = htmlDocument.DocumentNode.SelectSingleNode($"/html/body/main/section[1]/div/div[3]/div[2]/ul/li[{i}]/div[3]/p");
                string sellRate = sellRateNode?.InnerText.Trim() ?? "";

                // Запис отриманих даних у масив
                currencyTable[i - 1, 0] = currencyName;
                currencyTable[i - 1, 1] = buyRate;
                currencyTable[i - 1, 2] = sellRate;
            }

            return currencyTable;
        }
    }
}
