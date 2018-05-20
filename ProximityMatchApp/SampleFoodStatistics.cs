using ProximityMatch;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace ProximityMatchApp
{
    public class SampleFoodStatistics
    {
        private IDictionary<int, Period> _periodTable;
        private IDictionary<int, Food> _foodTable;
        

        public SampleFoodStatistics()
        {
            _periodTable = new Dictionary<int, Period>();
            _foodTable = new Dictionary<int, Food>();
        }

        public void Run()
        {
            var foodstats = LoadDataSet();
            Vector foodList = new Vector(dimension: 3);
            foodList.Plot(vectorList: foodstats);

            ConsoleKey Key;
            do
            {
                foodList.take = 10;

                /* Find the carrot price during the month of March.*/
                Console.WriteLine("\n\nFind all carrot (item = 110) price during the month of March.");
                var results = foodList.Find(new FoodStatistics() { item = 110 }, x => getPeriodItemData(((FoodStatistics)x).period).month == "03", y => ((FoodStatistics)y).period);
                foreach (FoodStatistics nodes in results)
                {
                    var itm = getFoodItemData(nodes.item);
                    var monthyear = getPeriodItemData(nodes.period);
                    Console.WriteLine("\n period = {1}, price = {2}, desc = {0}",
                             itm.description, string.Format(@"{0:00}/{1:0000}", monthyear.month, monthyear.year), nodes.price);
                }


                /* Find food items whose price atleast greater than 10 once and order them by price.*/
                Console.WriteLine("\n\nFind food items whose price atleast greater than 10 once and order them by price.");
                results = foodList.Find(new FoodStatistics(), x => ((FoodStatistics)x).price > 10, y => ((FoodStatistics)y).price);
                foreach (FoodStatistics nodes in results)
                {
                    var itm = getFoodItemData(nodes.item);
                    var monthyear = getPeriodItemData(nodes.period);
                    Console.WriteLine("\n period = {1}, price = {2}, desc = {0}",
                             itm.description, string.Format(@"{0:00}/{1:0000}", monthyear.month, monthyear.year), nodes.price);
                }

                Console.WriteLine("\nDo you want to continue (y/n)?");
                Key = Console.ReadKey(true).Key;

            } while (Key == ConsoleKey.Y);
        }

        private IList<IVector> LoadDataSet()
        {
            IList<IVector> dataSet = new List<IVector>();
            var table = ReadCsvFile("food-price-index-apr18-weighted-average-prices-csv-tables.csv");
            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    // Series_reference,Period,Data_value,STATUS,UNITS,Subject,Group,Series_title_1
                    var food = new FoodStatistics();

                    var series_reference = !string.IsNullOrEmpty(Convert.ToString(row["Series_reference"])) ? Convert.ToString(row["Series_reference"]) : null;
                    var period = !string.IsNullOrEmpty(Convert.ToString(row["Period"])) ? Convert.ToString(row["Period"]) : null;
                    var data_value = !string.IsNullOrEmpty(Convert.ToString(row["Data_value"])) ? Convert.ToDouble(row["Data_value"]) : default(double);
                    var series_title = !string.IsNullOrEmpty(Convert.ToString(row["Series_title_1"])) ? Convert.ToString(row["Series_title_1"]) : null;

                    food.item = setFoodItemData(series_reference, series_title);
                    food.period = setPeriodData(period);
                    food.price = data_value;
                    food.setCoordinates(new double?[3] { food.item, food.period, food.price });
                    
                    dataSet.Add(food);
                }
            }
            return dataSet;
        }

        private DataTable ReadCsvFile(string fileName)
        {

            DataTable dtCsv = new DataTable();
            string Fulltext;
            if (!string.IsNullOrEmpty(fileName))
            {
                string FileSaveWithPath = string.Format("{0}\\Data\\{1}", Directory.GetCurrentDirectory(), fileName);
                using (StreamReader sr = new StreamReader(FileSaveWithPath))
                {
                    while (!sr.EndOfStream)
                    {
                        Fulltext = sr.ReadToEnd().ToString(); //read full file text  
                        string[] rows = Fulltext.Split('\n'); //split full file text into rows  
                        for (int i = 0; i < rows.Count() - 1; i++)
                        {
                            string[] rowValues = rows[i].Split(','); //split each row with comma to get individual values  
                            {
                                if (i == 0)
                                {
                                    for (int j = 0; j < rowValues.Count(); j++)
                                    {
                                        dtCsv.Columns.Add(rowValues[j].Trim()); //add headers  
                                    }
                                }
                                else
                                {
                                    DataRow dr = dtCsv.NewRow();
                                    for (int k = 0; k < rowValues.Count(); k++)
                                    {
                                        dr[k] = rowValues[k].ToString().Trim();
                                    }
                                    dtCsv.Rows.Add(dr); //add other rows  
                                }
                            }
                        }
                    }
                }
            }
            return dtCsv;
        }

        private int setFoodItemData(string serialNo, string desc)
        {
            Food f = new Food();
            f.serialNo = serialNo;
            f.description = desc;

            int key = int.Parse(serialNo.Substring(serialNo.Length - 3, 3));

            if (!_foodTable.ContainsKey(key))
                _foodTable.Add(key, f);

            return key;
        }

        private Food getFoodItemData(int key)
        {
            if (_foodTable.ContainsKey(key))
                return _foodTable[key];
            return null;
        }

        private int setPeriodData(string period)
        {            
            var monthyear = period.Split('.');
            Period p = new Period();         
            p.year = monthyear[0];
            p.month = string.Format("{0:00}",monthyear[1]);
            
            int key = int.Parse(string.Format("{0}{1:00}",monthyear[0], monthyear[1]));

            if (!_periodTable.ContainsKey(key))
                _periodTable.Add(key, p);

            return key;
        }

        private Period getPeriodItemData(int key)
        {
            if (_periodTable.ContainsKey(key))
                return _periodTable[key];
            return null;
        }

    }

    public class FoodStatistics : IVector
    {
        public long uniqueId { get; set; }

        private double?[] coordinateP = new double?[3];

        public double?[] coordinate
        {
            get { return coordinateP; }
            set { coordinateP = value; }
        }

        public double _distance { get; set; }

        public int item { get { return coordinateP[0].HasValue ? (int)coordinateP[0].Value : default(int); } set { coordinateP[0] = value; } }
        public int period { get { return coordinateP[1].HasValue ? (int)coordinateP[1].Value : default(int); } set { coordinateP[1] = value; } }
        public double price { get { return coordinateP[2].HasValue ? coordinateP[2].Value : default(double); } set { coordinateP[2] = value; } }

        public void setCoordinates(double?[] co)
        {
            coordinateP = co;
        }
    }

    public class Period
    {
        public string month { get; set; }
        public string year { get; set; }
    }

    public class Food
    {
        public string serialNo { get; set; }
        public string description { get; set; }
    }
}
