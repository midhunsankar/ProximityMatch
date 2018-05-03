/*
 * Developer    : Midhun Sankar
 * Date         : 02/05/2018
 * Description  : This project is a demo to plot entities in a three dimentional vector space,
 *                and find vector points laying nearest to a point of reference. 
 * License      : GNU General Public License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProximityMatchApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<Car> carList = new List<Car>();
            /* Adding some data to the list. */
            carList.Add(new Car() { refno = "car#1", xaxis = 0, yaxis = 2018, zaxis = 30 });
            carList.Add(new Car() { refno = "car#2", xaxis = 0, yaxis = 2018, zaxis = 25 });
            carList.Add(new Car() { refno = "car#3", xaxis = 0, yaxis = 2018, zaxis = 50 });
            carList.Add(new Car() { refno = "car#4", xaxis = 1, yaxis = 2018, zaxis = 40 });
            carList.Add(new Car() { refno = "car#5", xaxis = 10, yaxis = 2015, zaxis = 30 });
            carList.Add(new Car() { refno = "car#6", xaxis = 30, yaxis = 2016, zaxis = 25 });
            carList.Add(new Car() { refno = "car#7", xaxis = 0, yaxis = 2017, zaxis = 50 });
            carList.Add(new Car() { refno = "car#8", xaxis = 70, yaxis = 2010, zaxis = 10 });
            carList.Add(new Car() { refno = "car#9", xaxis = 0, yaxis = 2018, zaxis = 22 });
            carList.Add(new Car() { refno = "car#10", xaxis = 5, yaxis = 2017, zaxis = 45 });
            carList.Add(new Car() { refno = "car#11", xaxis = 0, yaxis = 2018, zaxis = 35 });
            carList.Add(new Car() { refno = "car#12", xaxis = 2, yaxis = 2017, zaxis = 40 });
            carList.Add(new Car() { refno = "car#13", xaxis = 10, yaxis = 2015, zaxis = 21 });
            carList.Add(new Car() { refno = "car#14", xaxis = 8, yaxis = 2016, zaxis = 25 });
            carList.Add(new Car() { refno = "car#15", xaxis = 3, yaxis = 2017, zaxis = 35 });
            carList.Add(new Car() { refno = "car#16", xaxis = 60, yaxis = 2011, zaxis = 11 });
            carList.Add(new Car() { refno = "car#17", xaxis = 60, yaxis = 2014, zaxis = 11 });
            carList.Add(new Car() { refno = "car#18", xaxis = 35, yaxis = 2016, zaxis = 22 });
            carList.Add(new Car() { refno = "car#19", xaxis = 0, yaxis = 2017, zaxis = 45 });
            carList.Add(new Car() { refno = "car#20", xaxis = 20, yaxis = 2013, zaxis = 20 });

            Graph gp = new Graph(In: carList);
            
            gp.DataList();

            //// Un comment the below line of code to view all proximities from a perticular point of reference to all other vector points.
            // gp.ListAllProximities("car#4");

            gp.ListProximities("car#4", 10);
            gp.ListProximities("car#8", 10);
            gp.ListProximities("car#13", 10);
            
            Console.Read();
        }
    }

    /// <summary>
    /// Entity class for cars.
    /// </summary>
    public class Car
    {
        public string refno { get; set; }
        public double xaxis { get; set; } // x - coordinate
        public double yaxis { get; set; } // y - coordinate
        public double zaxis { get; set; } // z - coordinate
        private Tuple<double, double, double> coordinate;

        /// <summary>
        /// set vector coordinates.
        /// </summary>
        public void SetCoordinate()
        {
            coordinate = new Tuple<double, double, double>(xaxis, yaxis, zaxis);
        }

        /// <summary>
        /// return stored vector coordinate.
        /// </summary>
        /// <returns></returns>
        public Tuple<double, double, double> GetCoordinate()
        {
            return coordinate;
        }

        /// <summary>
        /// Function to compute the distance between two vector coordinates in three dimention.
        /// </summary>
        /// <param name="Incoordinate">The coordinate that to be checked against.</param>
        /// <returns></returns>
        public double CheckProximity(Tuple<double, double, double> Incoordinate)
        {
            /*
               Distance = sqrt( (x2−x1)^2 + (y2−y1)^2 + (z2-z1)^2 )
             * For better explanation follow url : https://betterexplained.com/articles/measure-any-distance-with-the-pythagorean-theorem
             */
            double proximity = Math.Sqrt(
                Math.Pow((Incoordinate.Item1 - coordinate.Item1), 2)
                +
                Math.Pow((Incoordinate.Item2 - coordinate.Item2), 2)
                +
                Math.Pow((Incoordinate.Item3 - coordinate.Item3), 2)
                );

            return proximity;
        }


    }

    public class Graph
    {
        private IList<Car> carList;
        
        public Graph(IList<Car> In)
        {
            carList = In;
        }

        /// <summary>
        /// Just show all car data.
        /// </summary>
        public void DataList()
        {
            Console.WriteLine("Data List :");
            Console.WriteLine("-----------------------------------------------------------------------------------");
            foreach (var dat in carList)
            {
                dat.SetCoordinate(); // set the coordinate vector.
                Console.WriteLine("refno = {0} , odometer = {1}K km , year = {2} , price = ${3}K , coordinate = ({1}, {2}, {3})",
                                    dat.refno, dat.xaxis, dat.yaxis, dat.zaxis);
            }
            Console.WriteLine("***----------------------------------------END-----------------------------------------------***");
        }

        /// <summary>
        /// Calculate the distance from a single coordinate to every other coordinates.
        /// </summary>
        /// <param name="refno">Point of reference, The cordinate that need to checked against.</param>
        public void ListAllProximities(string refno)
        {
            var car = carList.Where(x => x.refno.Equals(refno)).FirstOrDefault();
            if (car != null)
            {
                foreach (var x in carList)
                {
                    if (!x.refno.Equals(refno))
                    {
                        Console.WriteLine("\n {0} => {1} , proximity = {2} ", car.refno, x.refno, x.CheckProximity(car.GetCoordinate()));
                    }
                }
            }
        }

        /// <summary>
        /// Find the nearest coordinates to the point of reference.
        /// </summary>
        /// <param name="refno">Point of reference, The cordinate that need to checked against.</param>
        /// <param name="distance">The maximum threshold that to considered.</param>
        public void ListProximities(string refno, double distance)
        {
            var car = carList.Where(x => x.refno.Equals(refno)).FirstOrDefault();
            if (car != null)
            {
                int count = 0;   
                Console.WriteLine("\nrefno = {0} , odometer = {1}K km , year = {2} , price = ${3}K , coordinate = ({1}, {2}, {3})",
                                    car.refno, car.xaxis, car.yaxis, car.zaxis);
                Console.WriteLine("------------------------------------------------------------------------------------------------");
                foreach (var x in carList)
                {
                    if (!x.refno.Equals(refno))
                    {
                        var Proximity = x.CheckProximity(car.GetCoordinate());
                        if (Proximity < distance)
                        {
                            count++;
                            Console.WriteLine("refno = {0} , odometer = {1}K km , year = {2} , price = ${3}K , coordinate = ({1}, {2}, {3}) \n",
                                                x.refno, x.xaxis, x.yaxis, x.zaxis);
                        }
                    }
                }
                Console.WriteLine("{0} Matches found", count);
                Console.WriteLine("***----------------------------------------END-----------------------------------------------***");
            }
        }
    }

}
