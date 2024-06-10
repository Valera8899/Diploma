using HtmlAgilityPack;

namespace Monitor_2.Services.CurrencyParsers
{
    public class Change_Parser
    {
        private static readonly char[] separator = [' ', '\t', '\n', '\r'];

        public string[,] ArrangeCurrencyPairs(string[,] currencyTableRaw, int[] indices)
        {
            int rowCount = indices.Length;
            int colCount = currencyTableRaw.GetLength(1);
            string[,] arrangedTable = new string[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
            {
                int rowIndex = indices[i] - 1; // Перетворюємо індекс, який починається з 1, в індекс, який починається з 0
                if (rowIndex >= 0 && rowIndex < currencyTableRaw.GetLength(0))
                {
                    for (int j = 0; j < colCount; j++)
                    {
                        arrangedTable[i, j] = currencyTableRaw[rowIndex, j];
                    }
                }
                else
                {
                    // Обробка помилки, якщо індекс виходить за межі розмірів початкового масиву
                    Console.WriteLine($"Помилка: індекс {indices[i]} виходить за межі розмірів початкового масиву.");
                }
            }

            return arrangedTable;
        }

        public string[,] ParseChange(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            int currencyPairsCount = 8; // Кількість валютних пар
            int standardPairsCount = 4;// 7; // Кількість валютних пар, які треба виокремити з усіх наявних
            int tableColsCount = 3; // Кількість стовпців таблиці
            string[,] currencyTableRaw = new string[currencyPairsCount, tableColsCount];
            string[,] currencyTable = new string[standardPairsCount, tableColsCount];

            string ex = "Значення вузла не має бути null";

            for (int i = 1; i <= currencyPairsCount; i++)
            {
                // Отримання курсу купівлі 
                HtmlNode buyRateNode = htmlDocument.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div[{i}]/div/div[1]/div");
                string buyRate = "";
                if (buyRateNode != null)
                {
                    string[] buyRateValues = buyRateNode.InnerText.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    if (buyRateValues.Length > 0)
                    {
                        buyRate = buyRateValues[0];
                    }
                }
                else
                {
                    throw new ArgumentException(ex);
                }

                // Отримання назви валютної пари
                HtmlNode currencyNameNode = htmlDocument.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div[{i}]/div/div[2]");
                string currencyName = currencyNameNode?.InnerText.Trim().ToUpper() ?? "";

                // Отримання курсу продажу 
                HtmlNode sellRateNode = htmlDocument.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div[{i}]/div/div[3]/div");
                string sellRate = "";
                if (sellRateNode != null)
                {
                    string[] sellRateValues = sellRateNode.InnerText.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    if (sellRateValues.Length > 0)
                    {
                        sellRate = sellRateValues[0];
                    }
                }
                else
                {
                    throw new ArgumentException(ex);
                }

                // Запис отриманих даних у масив
                currencyTableRaw[i - 1, 0] = currencyName;
                currencyTableRaw[i - 1, 1] = buyRate;
                currencyTableRaw[i - 1, 2] = sellRate;
            }

            currencyTable = ArrangeCurrencyPairs(currencyTableRaw, [1, 2, 4, 5, 3, 7, 8]);
            currencyTable = ArrangeCurrencyPairs(currencyTableRaw, [1, 2, 4, 5]);

            return currencyTable;
        }
    }
}
