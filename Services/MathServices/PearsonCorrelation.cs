using Monitor_2.DataTransferObjects;
using Monitor_2.Controllers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Monitor_2.Services.MathServices
{
    public class PearsonCorrelation
    {
        private Random rand = new Random(Guid.NewGuid().GetHashCode());

        public decimal GenRandomDecimalVal(decimal min, decimal max, int precision = 2)
        {
            return min + (decimal)(rand.NextDouble() * (double)(max - min));
        }

        public (List<DataPoint>, List<DataPoint>) Gen_1PeriodOf_two_SinsWithError(int pointsCount, double deviation)
        {//повертає 2 списки точок(DataPoint), які є приблизною траєкторією двох періодів синуса
            decimal[] sinData1 = new decimal[pointsCount];
            decimal[] sinData2 = new decimal[pointsCount];
            decimal[] xValues = new decimal[pointsCount];

            decimal step = (decimal)(2 * Math.PI / pointsCount); // Крок для періоду sin(x)
            decimal dev = (decimal)deviation;

            for (int i = 0; i < pointsCount; i++)
            {
                decimal sinValue = (decimal)Math.Sin(i * (double)step);

                // Перевіряємо парність ітерації
                if (i % 2 == 0)
                {
                    // Збільшуємо значення на dev
                    sinData1[i] = sinValue * (1 - dev);
                    sinData2[i] = sinValue * (1 + dev);
                }
                else
                {
                    // Зменшуємо значення на dev
                    sinData1[i] = sinValue * (1 + dev);
                    sinData2[i] = sinValue * (1 - dev);
                }

                // Якщо значення близьке до нуля, додаємо або віднімаємо 0.1 залежно від парності ітерації
                if (Math.Abs(sinValue) < 0.1m)
                {
                    if (i % 2 == 0)
                    {
                        sinData1[i] = sinValue - 0.1m;
                        sinData2[i] = sinValue + 0.1m;
                    }
                    else
                    {
                        sinData1[i] = sinValue + 0.1m;
                        sinData2[i] = sinValue - +0.1m;
                    }
                }

                xValues[i] = i * step;
            }


            List<DataPoint> data1 = new List<DataPoint>();
            List<DataPoint> data2 = new List<DataPoint>();
            try
            {
                for (int i = 1; i <= pointsCount; i++)//sinData1.Length
                {
                    //data1.Add(new DataPoint { X = i.ToString(), Y = sinData1[i-1].ToString("0.00", CultureInfo.InvariantCulture) });
                    //data2.Add(new DataPoint { X = i.ToString(), Y = sinData2[i - 1].ToString("0.00", CultureInfo.InvariantCulture) });
                    data1.Add(new DataPoint { X = xValues[i - 1].ToString("0.00", CultureInfo.InvariantCulture), Y = sinData1[i - 1].ToString("0.00", CultureInfo.InvariantCulture) });
                    data2.Add(new DataPoint { X = xValues[i - 1].ToString("0.00", CultureInfo.InvariantCulture), Y = sinData2[i - 1].ToString("0.00", CultureInfo.InvariantCulture) });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при спробі перезапису з sinData у DataPoint: {ex.Message}");
            }

            return (data1, data2);
        }

        // Метод обчислення середнього значення
        public decimal CalculateMean(decimal[] data)
        {
            decimal sum = 0;
            foreach (decimal value in data)
            {
                sum += value;
            }
            return sum / data.Length;
        }

        // Метод обчислення коваріації між двома рядами
        public decimal CalculateCovariance(decimal[] data1, decimal[] data2)
        {
            if (data1.Length != data2.Length)
                throw new ArgumentException("Довжини масивів повинні бути однаковими.");

            decimal mean1 = CalculateMean(data1);
            decimal mean2 = CalculateMean(data2);

            decimal sum = 0;
            for (int i = 0; i < data1.Length; i++)
            {
                sum += (data1[i] - mean1) * (data2[i] - mean2);
            }
            return sum / data1.Length;
        }

        // Метод обчислення стандартного відхилення
        public decimal CalculateStandardDeviation(decimal[] data)
        {
            decimal mean = CalculateMean(data);
            decimal sumSquaredDifferences = 0;
            foreach (decimal value in data)
            {
                sumSquaredDifferences += (value - mean) * (value - mean);
            }
            return (decimal)Math.Sqrt((double)(sumSquaredDifferences / data.Length));
        }

        // Метод обчислення кореляції між двома масивами за допомогою коефіцієнта кореляції Пірсона
        public decimal CalculateCorrelation(decimal[] data1, decimal[] data2)
        {
            // Обчислення коваріації
            decimal covariance = CalculateCovariance(data1, data2);

            // Обчислення стандартних відхилень
            decimal stdDeviation1 = CalculateStandardDeviation(data1);
            decimal stdDeviation2 = CalculateStandardDeviation(data2);

            // Обчислення коефіцієнта кореляції Пірсона
            decimal correlation = covariance / (stdDeviation1 * stdDeviation2);
            return correlation;
        }

        public decimal CalculateCorrelation(List<DataPoint> data1, List<DataPoint> data2) //__________треба ще продебажжити його окремо в консольному додатку
        {
            // Зрівняти довжини списків
            if (data1.Count > data2.Count)
            {
                data1 = data1.Skip(data1.Count - data2.Count).ToList();
            }
            else if (data2.Count > data1.Count)
            {
                data2 = data2.Skip(data2.Count - data1.Count).ToList();
            }

            // Переписуємо властивість об'єктів зі списку в масив
            decimal[] data1Arr = data1.Select(d => decimal.Parse(d.Y, CultureInfo.InvariantCulture)).ToArray();
            decimal[] data2Arr = data2.Select(d => decimal.Parse(d.Y, CultureInfo.InvariantCulture)).ToArray();

            // Обчислення коваріації
            decimal covariance = CalculateCovariance(data1Arr, data2Arr);

            // Обчислення стандартних відхилень
            decimal stdDeviation1 = CalculateStandardDeviation(data1Arr);
            decimal stdDeviation2 = CalculateStandardDeviation(data2Arr);

            decimal correlation = 0;

            if (stdDeviation1 == 0 || stdDeviation2 == 0)
            {
                // Стандартне відхилення одного з масивів дорівнює нулю, кореляція невизначена
                return correlation; // кореляція лишається нульовою, метод поверне 0
            }

            // Обчислення коефіцієнта кореляції Пірсона
            correlation = covariance / (stdDeviation1 * stdDeviation2);
            return correlation;
        }


        // Функція для збереження масиву у файл
        public void SaveInMathCadFormat(decimal[] array, string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.Write("(:= (@LABEL VARIABLE " + Path.GetFileNameWithoutExtension(filename) + ") (@MATRIX " + array.Length + " 1 ");

                // Записуємо кожен елемент масиву з відповідним форматом
                foreach (var number in array)
                {
                    if (number < 0)
                    {
                        writer.Write("(@NEG " + Math.Abs(number).ToString("0.00", CultureInfo.InvariantCulture) + ") ");
                    }
                    else
                    {
                        writer.Write(number.ToString("0.00", CultureInfo.InvariantCulture) + " ");
                    }
                }
                writer.WriteLine("))");
            }
        }

    }
}
