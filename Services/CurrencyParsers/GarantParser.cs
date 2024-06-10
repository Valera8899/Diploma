using System;
using System.Globalization;
using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace Monitor_2.Services.CurrencyParsers
{
    public class GarantParser
    {
        public string[,] ParseGarant(string html)
        {
            var htmlDocument = new HtmlDocument(); // Створення об'єкта для парсингу HTML
            htmlDocument.LoadHtml(html);
            //Console.Write(html);

            //Записуємо в колекцію tableRows всі рядки (tr) таблиці, яка міститься у htmlDocument

            var tableRows = htmlDocument.DocumentNode.SelectNodes("//table//tr");
            //var tableRows = htmlDocument.DocumentNode.SelectNodes("//table[@class='rates__table__cus']/tbody/tr");

            int currencyPairsCount = 4; //к-ть валютних пар рівна кількості рядків таблиці 
            int tableColsCount = 3; //к-ть стовпців таблиці 3: ...
            string[,] currencyTable = new string[currencyPairsCount, tableColsCount];//

            //нумерація в мові XPath починається з 1
            for (int i = 1; i <= currencyPairsCount; i++)//таблиця по факту має лиш 5 заповнених рядків: usd,eur,gbp,chf,pln
            {
                for (int j = 1; j <= tableColsCount; j++) //в кожному рядку виявилося лише 3 елементи: <назва валюти> <купівля> <продаж> 
                {
                    var tempCell = htmlDocument.DocumentNode.SelectSingleNode($"//table//tr[{i}]/td[{j}]"); //отримання даних j-тої комірки i-го рядка
                    if (tempCell != null)
                    {
                        string cellText = tempCell.InnerText; //виокремлення тексту із вузла
                        string cleanStringCell = cellText.Trim().Replace("\n", "").Replace("\r", ""); //очищення від зайвих пробілів та ентерів

                        if (i > 0/*1*/) //оминаємо 1й рядок (шапка таблиці)
                        {
                            currencyTable[i - 1/*2*/, j - 1] = cleanStringCell;
                        }//переносимо дані в таблицю типу string
                    }
                    else
                    {
                        throw new ArgumentNullException($"Елемент таблиці [{i}, {j}] має значення null");
                    }
                }
            }

            return currencyTable;
        }
              
    }
}
