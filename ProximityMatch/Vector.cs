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
using ProximityMatch.Exceptions;

namespace ProximityMatch
{
    public class Vector
    {
        public int? take { get; set; }

        protected readonly int _dimension;
        protected int _distance;

        internal readonly IDictionary<double, IDictionary<double, List<VectorNode>>> _hashedCollection;

        private double Mu = 0, N = 0;
        private double _mean { get { return Mu / N; } }

        public Vector(int dimension = 3, int distance = 5)
        {
            _dimension = dimension;
            _distance = distance;
            _hashedCollection = new Dictionary<double, IDictionary<double, List<VectorNode>>>();
        }

        public virtual void Plot(IVector vector)
        {
            if (_dimension != vector.coordinate.Length)
            {
                throw new DimensionExceptions("dimension invalid!!");
            }
            if (_dimension < 2)
            {
                throw new DimensionExceptions("Atleast two dimensions are required!!");
            }

            if (vector.uniqueId == 0)
            {
                vector.uniqueId = Unique();
            }

            var v = new VectorNode()
                {
                    _vector = vector,
                    _anglePlain = GenerateAngles(vector),
                    _distanceOrgin = Distance(new double[2]{ 0, 0 }, new double[2]{ vector.coordinate[0], vector.coordinate[1] })  
                };

            Mu += v._anglePlain[0];
            N++;

            if(_hashedCollection.ContainsKey(v._anglePlain[0]))
            {
                if (_hashedCollection[v._anglePlain[0]].ContainsKey(v._distanceOrgin))
                {
                    _hashedCollection[v._anglePlain[0]][v._distanceOrgin].Add(v);
                }
                else
                {
                    _hashedCollection[v._anglePlain[0]].Add(v._distanceOrgin, new List<VectorNode> { v });
                }
            }
            else
            {
                var LinkedL = new Dictionary<double, List<VectorNode>>();   
                LinkedL.Add(v._distanceOrgin, new List<VectorNode>(){ v });
                _hashedCollection.Add(v._anglePlain[0], LinkedL);
            }

        }

        public virtual void Plot(IList<IVector> vectorList)
        {
            if (vectorList == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var vector in vectorList)
            {
                Plot(vector: vector);
            }
        }

        public virtual IList<IVector> Nearest(IVector In)
        {
            var angle = GenerateAngles(In);
            var distanceOrgin = Distance(new double[2]{ 0, 0 }, new double[2]{ In.coordinate[0], In.coordinate[1] });
            List<IVector> candidates = new List<IVector>();
            var sdAngle = StdDev_Angle();
            foreach (var hashedAngles in _hashedCollection)
            {
                if (InRange(angle[0], hashedAngles.Key, sdAngle))
                {
                    foreach (var hashedDistance in hashedAngles.Value)
                    {
                        if (InRange(distanceOrgin, hashedDistance.Key, _distance))
                        {
                            foreach (var vnode in hashedDistance.Value)
                            {
                                vnode._vector._distance = Distance(In.coordinate, vnode._vector.coordinate);
                                if (vnode._vector._distance < _distance)
                                    candidates.Add(vnode._vector);
                            }
                        }
                    }
                }
            }
            if(take.HasValue)
                return candidates.OrderBy(x => x._distance).Take(take.Value).ToList();
            else
                return candidates.OrderBy(x => x._distance).ToList();
        }

        public virtual IList<IVector> NearestFullScan(IVector In)
        {
            List<IVector> candidates = new List<IVector>();
            foreach (var vector in GetAll())
                {
                    vector._distance = Distance(In.coordinate, vector.coordinate);
                    if (vector._distance < _distance)
                    {
                        candidates.Add(vector);
                    }
                }            
            if(take.HasValue)
                return candidates.OrderBy(x => x._distance).Take(take.Value).ToList();
            else
                return candidates.OrderBy(x => x._distance).ToList();
        }

        public virtual IList<IVector> NearestRangeScan(IVector In, double[] range)
        {
            List<IVector> candidates = new List<IVector>();
            bool add;
            foreach (var vector in GetAll())
            {
                add = true;
                for(int i = 0; i < range.Count(); i++)
                {
                    if (!InRange(In.coordinate[i], vector.coordinate[i], range[i]))
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    candidates.Add(vector);
                }
            }
            if (take.HasValue)
                return candidates.OrderBy(x => x._distance).Take(take.Value).ToList();
            else
                return candidates.OrderBy(x => x._distance).ToList();
        }

        public virtual IList<IVector> Exact(IVector In)
        {
            List<IVector> candidates = new List<IVector>();
            var angle = GenerateAngles(In);
            var distanceOrgin = Distance(new double[2]{ 0, 0 }, new double[2]{ In.coordinate[0], In.coordinate[1] });
            if (_hashedCollection.ContainsKey(angle[0]))
            {
                var hashedAngles = _hashedCollection[angle[0]];
                foreach (var hashedDistance in hashedAngles.Keys)
                {
                    if(hashedDistance == distanceOrgin)
                    {
                        foreach(var vnode in hashedAngles[hashedDistance])
                        {
                            vnode._vector._distance = Distance(In.coordinate, vnode._vector.coordinate);
                            if (vnode._vector._distance == 0)
                                candidates.Add(vnode._vector);
                        }
                    }
                }
            }
            if (take.HasValue)
                return candidates.OrderBy(x => x._distance).Take(take.Value).ToList();
            else
                return candidates.OrderBy(x => x._distance).ToList();
        }

        public virtual IList<IVector> GetAll()
        {
            List<IVector> all = new List<IVector>();
            foreach (var hashedAngles in _hashedCollection.Values)
            {
                foreach (var hashedDistance in hashedAngles)
                {
                    all.AddRange(hashedDistance.Value.Select<VectorNode, IVector>(x => x._vector).ToList());
                }
            }
            return all;
        }
       
        public virtual bool Remove(IVector In)
        {
            bool ret = false;
            var angle = GenerateAngles(In);
            var distanceOrgin = Distance(new double[2] { 0, 0 }, new double[2] { In.coordinate[0], In.coordinate[1] });
            if (_hashedCollection.ContainsKey(angle[0]))
            {
                var hashedDistances = _hashedCollection[angle[0]];
                int i = 0;
               
                while (i < hashedDistances.Count)
                {
                    var _hashedDistanceOrgin = hashedDistances.ElementAt(i).Key;
                    if (_hashedDistanceOrgin == distanceOrgin)
                    {
                        int j = 0;
                        var _vectorNodeList = hashedDistances.ElementAt(i).Value;
                        while (j < _vectorNodeList.Count)
                        {
                            var vnode = _vectorNodeList[j];                            
                            if (Distance(In.coordinate, vnode._vector.coordinate) == 0)
                            {
                                Mu -= vnode._anglePlain[0];
                                N--;
                                //remove node.
                                _vectorNodeList.RemoveAt(j);
                                ret = true;
                                j--;
                            }
                            j++;
                        }

                        if(hashedDistances[_hashedDistanceOrgin] == null || hashedDistances[_hashedDistanceOrgin].Count == 0)
                        {
                            //No nodes empty dictionary so remove the key from parent.
                            hashedDistances.Remove(_hashedDistanceOrgin);
                            i--;
                        }
                    }
                    i++;
                 }
                if (_hashedCollection[angle[0]] == null || _hashedCollection[angle[0]].Count == 0)
                {
                    //No nodes empty dictionary so remove the key from parent.
                    _hashedCollection.Remove(angle[0]);
                }
            }
            return ret;
        }

        public virtual bool Remove(long uniqueId)
        {
            var ret = false;
            if (uniqueId == 0)
            {
                throw new UniqueIdExceptions("UniqueId not set!!");
            }

            int i = 0;
            while(i < _hashedCollection.Count)
            {
                var angleKey = _hashedCollection.ElementAt(i).Key;
                var hashedDistance = _hashedCollection[angleKey];
                int j = 0;

                while(j < hashedDistance.Count)
                {
                    var distanceKey = hashedDistance.ElementAt(j).Key;
                    var _vectorNodeList = hashedDistance[distanceKey];
                    int k = 0;

                    while(k < _vectorNodeList.Count)
                    {
                        if(uniqueId == _vectorNodeList[k]._vector.uniqueId)
                        {
                            Mu -= _vectorNodeList[k]._anglePlain[0];
                            N--;
                            _vectorNodeList.RemoveAt(k);
                            ret = true;
                            break;
                        }
                        k++;
                    }

                    if (ret && (_vectorNodeList == null || _vectorNodeList.Count == 0))
                    {
                        hashedDistance.Remove(distanceKey);
                        break;
                    }

                    j++;
                }

                if (ret && (hashedDistance == null || hashedDistance.Count == 0))
                {
                    _hashedCollection.Remove(angleKey);
                    break;
                }
                i++;
            }
            return ret;
        }

        public virtual bool Update(IVector Old, IVector New)
        {
            if (New.uniqueId == 0)
            {
                throw new UniqueIdExceptions("UniqueId not set!!");
            }
            else if (Old.uniqueId != New.uniqueId)
            {
                throw new UniqueIdExceptions("UniqueId should be same!!");
            }
            else
            {

                if (Remove(Old))
                {
                    Plot(New);
                    return true;
                }
            }
            return false;
        }

        public virtual bool Update(long uniqueId, IVector New)
        {
            if (New.uniqueId == 0)
            {
                throw new UniqueIdExceptions("UniqueId not set!!");
            }
            else if (uniqueId != New.uniqueId)
            {
                throw new UniqueIdExceptions("UniqueId should be same!!");
            }
            else
            {
                if (Remove(uniqueId))
                {
                    Plot(New);
                    return true;
                }
            }
            return false;
        }

    #region private_functions

            private double[] GenerateAngles(IVector vector)
            {
                /* combination formula. n!/(n-r)!.r! */

                var n = vector.coordinate.Length;
                var r = 2;
                var len = 2;
                
                if (n > 2)
                {
                   len = Factorial(n) / (Factorial(n - r) * Factorial(r));
                }

                var angleList = new double[len];
                var z = 0;
            
                for(int i = 0; i < vector.coordinate.Length - 1; i++)
                    for(int j = i + 1; j < vector.coordinate.Length; j++)
                    {
                        var slope = Math.Sqrt( Math.Pow(vector.coordinate[i], 2) + Math.Pow(vector.coordinate[j], 2) );
                        var costheta = vector.coordinate[i] / slope;
                        var angle = Math.Acos(costheta) * (180 / Math.PI);
                        angleList[z] = Math.Round(angle, 4);
                        z++;
                    }
                return angleList;
            }

            private double GenerateAngles(double xPlain, double yPlain)
            {
                var slope = Math.Sqrt(Math.Pow(xPlain, 2) + Math.Pow(yPlain, 2));
                var costheta = xPlain / slope;
                var angle = Math.Acos(costheta) * (180 / Math.PI);
                return Math.Round(angle, 4);
            }

            private int Factorial(int number)
            {
                if (number > 1)                
                    number = number * Factorial(number - 1);
                return number;
            }

            private double Distance(double[] co1, double[] co2)
            {
                /*
                   Distance = sqrt( (x2−x1)^2 + (y2−y1)^2 + (z2-z1)^2 )
                 * For better explanation follow url : https://betterexplained.com/articles/measure-any-distance-with-the-pythagorean-theorem
                 */
                double d = 0;
                for (int i = 0; i < co1.Length; i++)
                {
                    d += Math.Pow((co1[i] - co2[i]), 2);
                }
                return Math.Round(Math.Sqrt(d),4);
            }

            private bool InRange(double[] c1, double[] c2, double deviation)
            {

                for (int i = 0; i < c1.Length; i++)
                {
                    if (!(c2[i] >= (c1[i] - deviation) && c2[i] <= (c1[i] + deviation)))
                    {
                        return false;
                    }
                }
                return true;
            }
 
            private bool InRange(double c1, double c2, double deviation)
            {
                    if (!(c2 >= (c1 - deviation) && c2 <= (c1 + deviation)))
                    {
                        return false;
                    }           
                return true;
            }

            private double StdDev_Angle()
            {
               var mean = _mean;
               double sum = 0;
                sum = _hashedCollection.Sum(x => Math.Pow(x.Key - mean,2));

                if (sum == 0) /* standard deviation = 0 is not ideal so atleast 1 degree of check is required. */
                {
                    return 1;
                }
                return Math.Round(Math.Sqrt(sum / (_hashedCollection.Count > 10 ? N : N-1)),4);
            }

            private double StdDev_Distance(ICollection<double> Keys)
            {
                var count = Keys.Count();
                var mean = Keys.Sum() / count;
                double sum = 0;
                sum = Keys.Sum(x => Math.Pow(x - mean, 2));

                if (sum == 0) /* standard deviation = 0 is not ideal so atleast 1 degree of check is required. */
                {
                    return _distance / 2;
                }
                return Math.Round(Math.Sqrt(sum / (count > 10 ? count : count - 1)), 4);
            }

            private long Unique()
            {
                return DateTime.Now.Ticks;
            }

    #endregion

    }


}
