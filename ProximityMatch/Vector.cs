using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProximityMatch
{
    public class Vector
    {
        private readonly int _dimension;
        protected readonly IDictionary<double, LinkedList<VectorNode>> _index;


        protected class VectorNode
        {
            public IVector _vector;
            public double[] _anglePlain;
        }

        public Vector(int dimension = 3)
        {
            _dimension = dimension;
            _index = new SortedDictionary<double, LinkedList<VectorNode>>();
        }

        public void Plot(IVector vector)
        {
            if (_dimension != vector.coordinate.Length)
            {
                throw new Exception("dimension invalid!!");
            }
            var v = new VectorNode()
                {
                    _vector = vector,
                    _anglePlain = generateAngles(vector)
                };

            if(_index.ContainsKey(v._anglePlain[0]))
            {
                _index[v._anglePlain[0]].AddLast(v);
            }
            else
            {
                var LinkedL = new LinkedList<VectorNode>();                
                LinkedL.AddFirst(v);
                _index.Add(v._anglePlain[0], LinkedL);
            }         
        }

        private double[] generateAngles(IVector vector)
        {
            /* combination formula. n!/(n-r)!.r! */

            var n = vector.coordinate.Length;
            var r = 2;
            var len = factorial(n) / (factorial(n - r) * factorial(r));
            var angleList = new double[len];
            var z = 0;
            
            for(int i = 0; i < vector.coordinate.Length - 1; i++)
                for(int j = i + 1; j < vector.coordinate.Length; j++)
                {
                    var slope = Math.Sqrt( Math.Pow(vector.coordinate[i], 2) + Math.Pow(vector.coordinate[j], 2) );
                    var costheta = vector.coordinate[i] / slope;
                    var angle = Math.Acos(costheta) * (180 / Math.PI);
                    angleList[z] = angle;
                    z++;
                }
            return angleList;
        }

        private int factorial(int number)
        {
            if (number > 1)                
                number = number * factorial(number - 1);
            return number;
        }

        //public double[] coordinate { get; set; }

        ///// <summary>
        ///// set vector coordinates.
        ///// </summary>
        //public void SetCoordinate()
        //{
        //    coordinateT = new Tuple<double, double, double>(xaxis, yaxis, zaxis);
        //}

        ///// <summary>
        ///// return stored vector coordinate.
        ///// </summary>
        ///// <returns></returns>
        //public Tuple<double, double, double> GetCoordinate()
        //{
        //    return coordinateT;
        //}

        ///// <summary>
        ///// Function to compute the distance between two vector coordinates in three dimention.
        ///// </summary>
        ///// <param name="Incoordinate">The coordinate that to be checked against.</param>
        ///// <returns></returns>
        //public double CheckProximity(Tuple<double, double, double> Incoordinate)
        //{
        //    /*
        //       Distance = sqrt( (x2−x1)^2 + (y2−y1)^2 + (z2-z1)^2 )
        //     * For better explanation follow url : https://betterexplained.com/articles/measure-any-distance-with-the-pythagorean-theorem
        //     */
        //    double proximity = Math.Sqrt(
        //        Math.Pow((Incoordinate.Item1 - coordinateT.Item1), 2)
        //        +
        //        Math.Pow((Incoordinate.Item2 - coordinateT.Item2), 2)
        //        +
        //        Math.Pow((Incoordinate.Item3 - coordinateT.Item3), 2)
        //        );

        //    return proximity;
        //}



        ///// <summary>
        ///// Just show all car data.
        ///// </summary>
        //public void DataList()
        //{
        //    Console.WriteLine("Data List :");
        //    Console.WriteLine("-----------------------------------------------------------------------------------");
        //    foreach (var dat in carList)
        //    {
        //        dat.SetCoordinate(); // set the coordinate vector.
        //        Console.WriteLine("refno = {0} , odometer = {1}K km , year = {2} , price = ${3}K , coordinate = ({1}, {2}, {3})",
        //                            dat.refno, dat.xaxis, dat.yaxis, dat.zaxis);
        //    }
        //    Console.WriteLine("***----------------------------------------END-----------------------------------------------***");
        //}

        ///// <summary>
        ///// Calculate the distance from a single coordinate to every other coordinates.
        ///// </summary>
        ///// <param name="refno">Point of reference, The cordinate that need to checked against.</param>
        //public void ListAllProximities(string refno)
        //{
        //    var car = carList.Where(x => x.refno.Equals(refno)).FirstOrDefault();
        //    if (car != null)
        //    {
        //        foreach (var x in carList)
        //        {
        //            if (!x.refno.Equals(refno))
        //            {
        //                Console.WriteLine("\n {0} => {1} , proximity = {2} ", car.refno, x.refno, x.CheckProximity(car.GetCoordinate()));
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Find the nearest coordinates to the point of reference.
        ///// </summary>
        ///// <param name="refno">Point of reference, The cordinate that need to checked against.</param>
        ///// <param name="distance">The maximum threshold that to considered.</param>
        //public void ListProximities(string refno, double distance)
        //{
        //    var car = carList.Where(x => x.refno.Equals(refno)).FirstOrDefault();
        //    if (car != null)
        //    {
        //        int count = 0;   
        //        Console.WriteLine("\nrefno = {0} , odometer = {1}K km , year = {2} , price = ${3}K , coordinate = ({1}, {2}, {3})",
        //                            car.refno, car.xaxis, car.yaxis, car.zaxis);
        //        Console.WriteLine("------------------------------------------------------------------------------------------------");
        //        foreach (var x in carList)
        //        {
        //            if (!x.refno.Equals(refno))
        //            {
        //                var Proximity = x.CheckProximity(car.GetCoordinate());
        //                if (Proximity < distance)
        //                {
        //                    count++;
        //                    Console.WriteLine("refno = {0} , odometer = {1}K km , year = {2} , price = ${3}K , coordinate = ({1}, {2}, {3}) \n",
        //                                        x.refno, x.xaxis, x.yaxis, x.zaxis);
        //                }
        //            }
        //        }
        //        Console.WriteLine("{0} Matches found", count);
        //        Console.WriteLine("***----------------------------------------END-----------------------------------------------***");
        //    }
        //}
    }


}
