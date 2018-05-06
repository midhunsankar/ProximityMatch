using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProximityMatch
{
    public class Vector
    {
        protected readonly int _dimension;
        protected int _distance;

        internal readonly Dictionary<double, List<VectorNode>> _hashedCollection;

        private double Mu = 0, N = 0;
        private double _mean { get { return Mu / N; } }

        public Vector(int dimension = 3, int distance = 5)
        {
            _dimension = dimension;
            _distance = distance;
            _hashedCollection = new Dictionary<double, List<VectorNode>>();          
        }

        public virtual void Plot(IVector vector)
        {
            if (_dimension != vector.coordinate.Length)
            {
                throw new Exception("dimension invalid!!");
            }
            var v = new VectorNode()
                {
                    _vector = vector,
                    _anglePlain = GenerateAngles(vector),                  
                };

            Mu += v._anglePlain[0];
            N++;

            if(_hashedCollection.ContainsKey(v._anglePlain[0]))
            {
                _hashedCollection[v._anglePlain[0]].Add(v);
            }
            else
            {
                var LinkedL = new List<VectorNode>();                
                LinkedL.Add(v);
                _hashedCollection.Add(v._anglePlain[0], LinkedL);
            }
        }

        public virtual IList<IVector> Nearest(IVector In, int take = 10)
        {
            var angle = GenerateAngles(In);
            List<IVector> candidates = new List<IVector>();
            var sd = StandardDeviation();
            foreach (var hashed in _hashedCollection)
            {
                if (InRange(angle[0], hashed.Key, sd))
                {
                    foreach (var element in hashed.Value)
                    {
                            element._vector._distance = Distance(In.coordinate, element._vector.coordinate);
                            if(element._vector._distance < _distance)
                                candidates.Add(element._vector);
                    }
                }
            }
            return candidates.OrderBy(x => x._distance).Take(take).ToList();
        }

        public virtual IList<IVector> NearestFullScan(IVector In, int take = 10)
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
            return candidates.OrderBy(x => x._distance).Take(take).ToList();
        }

        public virtual IList<IVector> GetAll()
        {
            List<IVector> all = new List<IVector>();
            foreach (var hashkey in _hashedCollection)
            {
                all.AddRange(hashkey.Value.Select<VectorNode, IVector>(x => x._vector).ToList());
            }
            return all;
        }

    #region private_functions

            private double[] GenerateAngles(IVector vector)
            {
                /* combination formula. n!/(n-r)!.r! */

                var n = vector.coordinate.Length;
                var r = 2;
                var len = Factorial(n) / (Factorial(n - r) * Factorial(r));
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
                return Math.Sqrt(d);
            }

            private bool InRange(double[] co1, double[] co2, double deviation)
            {

                for (int i = 0; i < co1.Length; i++)
                {
                    if (co2[i] < co1[i] - deviation || co2[i] > co1[i] + deviation)
                    {
                        return false;
                    }
                }
                return true;
            }
 
            private bool InRange(double co1, double co2, double deviation)
            {
                    if (co2 < co1 - deviation || co2 > co1 + deviation)
                    {
                        return false;
                    }           
                return true;
            }

            private double StandardDeviation()
            {
               var mean = _mean;
               var sum = _hashedCollection.Sum(x => Math.Pow(x.Key - mean,2));
               return Math.Sqrt(sum / (_hashedCollection.Count > 10 ? N : N-1));
            }

    #endregion

    }


}
