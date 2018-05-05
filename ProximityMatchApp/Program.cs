﻿/*
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
            Vector carList = new Vector();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //IList<Car> carList = new List<Car>();
            /* Adding some data to the list. */
            //carList.Plot(new Car() { refno = "car#1", xaxis = 0, yaxis = 2018, zaxis = 30 });

            //carList.Plot(new Car() { refno = "car#2", xaxis = 0, yaxis = 2018, zaxis = 25 });
            //carList.Plot(new Car() { refno = "car#3", xaxis = 0, yaxis = 2018, zaxis = 50 });
            //carList.Plot(new Car() { refno = "car#4", xaxis = 1, yaxis = 2018, zaxis = 40 });
            //carList.Plot(new Car() { refno = "car#5", xaxis = 10, yaxis = 2015, zaxis = 30 });
            //carList.Plot(new Car() { refno = "car#6", xaxis = 30, yaxis = 2016, zaxis = 25 });
            //carList.Plot(new Car() { refno = "car#7", xaxis = 0, yaxis = 2017, zaxis = 50 });
            //carList.Plot(new Car() { refno = "car#8", xaxis = 70, yaxis = 2010, zaxis = 10 });
            //carList.Plot(new Car() { refno = "car#9", xaxis = 0, yaxis = 2018, zaxis = 22 });
            //carList.Plot(new Car() { refno = "car#10", xaxis = 5, yaxis = 2017, zaxis = 45 });
            //carList.Plot(new Car() { refno = "car#11", xaxis = 0, yaxis = 2018, zaxis = 35 });
            //carList.Plot(new Car() { refno = "car#12", xaxis = 2, yaxis = 2017, zaxis = 40 });
            //carList.Plot(new Car() { refno = "car#13", xaxis = 10, yaxis = 2015, zaxis = 21 });
            //carList.Plot(new Car() { refno = "car#14", xaxis = 8, yaxis = 2016, zaxis = 25 });
            //carList.Plot(new Car() { refno = "car#15", xaxis = 3, yaxis = 2017, zaxis = 35 });
            //carList.Plot(new Car() { refno = "car#16", xaxis = 60, yaxis = 2011, zaxis = 11 });
            //carList.Plot(new Car() { refno = "car#17", xaxis = 60, yaxis = 2014, zaxis = 11 });
            //carList.Plot(new Car() { refno = "car#18", xaxis = 35, yaxis = 2016, zaxis = 22 });
            //carList.Plot(new Car() { refno = "car#19", xaxis = 0, yaxis = 2017, zaxis = 45 });
            //carList.Plot(new Car() { refno = "car#20", xaxis = 20, yaxis = 2013, zaxis = 20 });

            Random rnd = new Random();
            double x, y, z;
            int maxlimit = 1000000;
            sw.Start();

            for (int i = 0; i < maxlimit; i++)
            {
                x = rnd.Next(0, 200);
                y = rnd.Next(1965, 2018);
                z = rnd.Next(1000, 100000);
                z = z - z % 500;
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
                x = rnd.Next(0, 200);
                y = rnd.Next(1965, 2018);
                z = rnd.Next(1000, 100000);
                z = z - z % 500;
                var car = new Car() { refno = "car#" + rnd.Next(0, 100000), xaxis = x, yaxis = y, zaxis = z };

                Console.WriteLine("\nrefno = {0} , odometer = {1}K km , year = {2} , price = ${3} , coordinate = ({1}, {2}, {3})",
                                       car.refno, car.xaxis, car.yaxis, car.zaxis);
                Console.WriteLine("\n*******************************************************************************************\n");                
                
                sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                var nodes = carList.nearest(car);
                foreach (Car dat in nodes)
                {
                    Console.WriteLine("refno = {0} , odometer = {1}K km , year = {2} , price = ${3} , coordinate = ({1}, {2}, {3})",
                                       dat.refno, dat.xaxis, dat.yaxis, dat.zaxis);
                }
                sw.Stop();
                Console.WriteLine("\nSearching finished!! Time: {0} ms Cycles: {1} ticks", sw.Elapsed.TotalSeconds, sw.Elapsed.Ticks);

                sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                nodes = carList.nearestFull(car);
                foreach (Car dat in nodes)
                {
                    Console.WriteLine("refno = {0} , odometer = {1}K km , year = {2} , price = ${3} , coordinate = ({1}, {2}, {3})",
                                       dat.refno, dat.xaxis, dat.yaxis, dat.zaxis);
                }
                sw.Stop();
                Console.WriteLine("\nSearching finished (Full)!! Time: {0} ms Cycles: {1} ticks", sw.Elapsed.TotalSeconds, sw.Elapsed.Ticks);

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
        
        private double[] coordinateP = new double[3];
        public double[] coordinate { get { return coordinateP; } set { coordinateP = value; } }
    }

}
