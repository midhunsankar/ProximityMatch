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
using ProximityMatch;

namespace ProximityMatchApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Vector carList = new Vector(dimension: 5);
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //IList<Car> carList = new List<Car>();
            /* Adding some data to the list. */
        
            Random rnd = new Random();
            double x, y, z, p = 0, q = 0;
            int maxlimit = 1000000;
            sw.Start();

            for (int i = 0; i < maxlimit; i++)
            {
                x = rnd.Next(0, 100000); // odometer
                y = rnd.Next(1965, 2018); // year
                z = rnd.Next(1000, 100000);// price
             //   p = rnd.Next(800, 6000);// engine cc
            //    q = rnd.Next(0, 5); // safety rating 

                x = x - x % 1000;
                z = z - z % 500;
             //   p = p - p % 1000;

                carList.Plot(new Car() { refno = "car#" + i, xaxis = x, yaxis = y, zaxis = z });
            }
            sw.Stop();
            Console.WriteLine("\nPloting finished!! Time: {0} ms Cycles: {1} ticks", sw.Elapsed.TotalSeconds, sw.Elapsed.Ticks);
            //foreach (Car dat in carList.getAll())
            //{
            //    Console.WriteLine("refno = {0} , odometer = {1}K km , year = {2} , price = ${3}K , coordinate = ({1}, {2}, {3})",
            //                       dat.refno, dat.xaxis, dat.yaxis, dat.zaxis);
            //}
            do
            {
                x = rnd.Next(0, 100000); // odometer
                y = rnd.Next(1965, 2018); // year
                z = rnd.Next(1000, 100000);// price
              
             //   p = rnd.Next(800, 6000);// engine cc
             //   q = rnd.Next(0, 5); // safety rating 

                x = x - x % 1000;
                z = z - z % 500;
            //    p = p - p % 500; 
                
                var car = new Car() { refno = "car#" + rnd.Next(0, 100000), xaxis = x, yaxis = y, zaxis = z, paxis= p, qaxis= q };
                Console.WriteLine("\nrefno = {0} , odometer = {1} km , year = {2} , price = ${3} , engine = {4} cc, rating = {5} coordinate = ({1}, {2}, {3}, {4}, {5})",
                                       car.refno, car.xaxis, car.yaxis, car.zaxis, car.paxis, car.qaxis);
                Console.WriteLine("\n*******************************************************************************************\n");                
                
                sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                var nodes = carList.Nearest(car, 5);
                foreach (Car dat in nodes)
                {
                    Console.WriteLine("\nrefno = {0} , odometer = {1} km , year = {2} , price = ${3} , engine = {4} cc, rating = {5} coordinate = ({1}, {2}, {3}, {4}, {5})",
                                           dat.refno, dat.xaxis, dat.yaxis, dat.zaxis, dat.paxis, dat.qaxis);
                }
                sw.Stop();
                Console.WriteLine("\nSearching finished!! Time: {0} ms Cycles: {1} ticks\n", sw.Elapsed.Milliseconds, sw.Elapsed.Ticks);

                sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                nodes = carList.NearestFullScan(car, 5);
                foreach (Car dat in nodes)
                {
                    Console.WriteLine("\nrefno = {0} , odometer = {1} km , year = {2} , price = ${3} , engine = {4} cc, rating = {5} coordinate = ({1}, {2}, {3}, {4}, {5})",
                           dat.refno, dat.xaxis, dat.yaxis, dat.zaxis, dat.paxis, dat.qaxis);
                }
                sw.Stop();
                Console.WriteLine("\nSearching finished (Full)!! Time: {0} ms Cycles: {1} ticks", sw.Elapsed.Milliseconds, sw.Elapsed.Ticks);

                Console.WriteLine("\nDo you want to continue (y/n)?");
            } while (Console.ReadKey(true).Key == ConsoleKey.Y);
            
        }
        
    }

    /// <summary>
    /// Entity class for cars.
    /// </summary>
    public class Car : IVector 
    {
        public string refno { get; set; }

        private double xaxisP;
        private double yaxisP;
        private double zaxisP;
        private double paxisP;
        private double qaxisP;

        public double xaxis
        {
            get { return xaxisP; }
            set { 
                xaxisP = value;
                coordinateP[0] = value;
            }
        }
        public double yaxis
        {
            get { return yaxisP; }
            set
            {
                yaxisP = value;
                coordinateP[1] = value;
            }
        }
        public double zaxis
        {
            get { return zaxisP; }
            set
            {
                zaxisP = value;
                coordinateP[2] = value;
            }
        }
        public double paxis
        {
            get { return paxisP; }
            set
            {
                paxisP = value;
                coordinateP[3] = value;
            }
        }
        public double qaxis
        {
            get { return qaxisP; }
            set
            {
                qaxisP = value;
                coordinateP[4] = value;
            }
        }

        public double _distance { get; set; }

        private double[] coordinateP = new double[5];
        public double[] coordinate { get { return coordinateP; } set { coordinateP = value; } }
    }

}
