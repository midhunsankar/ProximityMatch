using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OctagonSquare.ProximityMatch;

namespace ProximityMatchApp
{
    public class SampleCars
    {

        public void Run()
        {
            Vector carList = new Vector(dimension: 5);
            carList.Take = 5;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            long uniqueOut;

            Random rnd = new Random();
            double x, y, z, p = 0, q = 0;
            int maxlimit = 1000000;
            sw.Start();

            for (int i = 0; i < maxlimit; i++)
            {
                x = rnd.Next(0, 20000); // odometer
                y = rnd.Next(1965, 2018); // year
                z = rnd.Next(1000, 10000);// price

                p = rnd.Next(1000, 6000);// engine cc
                //  q = rnd.Next(0, 5); // safety rating 

                x = x - x % 1000;
                z = z - z % 500;
                p = p - p % 1000;

                /* Adding some data to the list. */
                carList.Plot(new Car() { refno = "car#" + i, xaxis = x, yaxis = y, zaxis = z, paxis = p, qaxis = q }, out uniqueOut);
            }
            sw.Stop();
            Console.WriteLine("\nPloting finished!! Time: {0} sec Cycles: {1} ticks", sw.Elapsed.TotalSeconds, sw.Elapsed.Ticks);

            ConsoleKey Key;
            do
            {
                x = rnd.Next(0, 20000); // odometer
                y = rnd.Next(1965, 2018); // year
                z = rnd.Next(1000, 10000);// price

                p = rnd.Next(1000, 6000);// engine cc
                //   q = rnd.Next(0, 5); // safety rating 

                x = x - x % 1000;
                z = z - z % 500;
                p = p - p % 1000;

                var car = new Car() { refno = "car#" + rnd.Next(0, 100000), xaxis = x, yaxis = y, zaxis = z, paxis = p, qaxis = q };
                Console.WriteLine("\nrefno = {0} , odometer = {1} km , year = {2} , price = ${3} , engine = {4} cc, rating = {5} coordinate = ({1}, {2}, {3}, {4}, {5})",
                                       car.refno, car.xaxis, car.yaxis, car.zaxis, car.paxis, car.qaxis);
                Console.WriteLine("\n*******************************************************************************************\n");

                sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                var nodes = carList.Nearest<Car>(car);
                foreach (var dat in nodes)
                {
                    Console.WriteLine("\nrefno = {0} , odometer = {1} km , year = {2} , price = ${3} , engine = {4} cc, rating = {5} coordinate = ({1}, {2}, {3}, {4}, {5}) distance = {6}",
                                           dat.refno, dat.xaxis, dat.yaxis, dat.zaxis, dat.paxis, dat.qaxis, dat.Distance);
                }
                sw.Stop();

                Console.WriteLine("\nSearching finished!! Time: {0:000000} ms Cycles: {1} ticks\n", sw.Elapsed.TotalMilliseconds, sw.Elapsed.Ticks);

                sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                var nodes2 = carList.NearestFullScan(car);
                // var nodes2 = carList.NearestRangeScan(car, new double[4] { 1000, 2, 1000, 1000 });
                foreach (Car dat in nodes2)
                {
                    Console.WriteLine("\nrefno = {0} , odometer = {1} km , year = {2} , price = ${3} , engine = {4} cc, rating = {5} coordinate = ({1}, {2}, {3}, {4}, {5}) distance = {6}",
                           dat.refno, dat.xaxis, dat.yaxis, dat.zaxis, dat.paxis, dat.qaxis, dat.Distance);
                }
                sw.Stop();

                Console.WriteLine("\nSearching finished (Full)!! Time: {0:000000} ms Cycles: {1} ticks", sw.Elapsed.TotalMilliseconds, sw.Elapsed.Ticks);
                Console.WriteLine("\nDo you want to continue (y/n)?");

                //if (nodes.Count > 0 || nodes2.Count > 0)
                //{
                //    Key = Console.ReadKey(true).Key;
                //}
                //else
                //{
                //    Key = ConsoleKey.Y;
                //}

                Key = Console.ReadKey(true).Key;

            } while (Key == ConsoleKey.Y);
        }

    }

    /// <summary>
    /// Entity class for cars.
    /// </summary>
    public class Car : IVector
    {
        public string refno { get; set; }
        public long UniqueId { get; set; }
        private double xaxisP;
        private double yaxisP;
        private double zaxisP;
        private double paxisP;
        private double qaxisP;

        public double xaxis
        {
            get { return xaxisP; }
            set
            {
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

        /* IVector implimentation. */
        public double Distance { get; set; }
        private double?[] coordinateP = new double?[5];
        public double?[] Coordinate { get { return coordinateP; } set { coordinateP = value; } }

    }
}
