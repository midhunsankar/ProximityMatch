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
using ProximityMatch.Exceptions;

namespace ProximityMatch
{
    public class Vector
    {
        public int? take { get; set; }

        protected readonly int _dimension;
        protected int _distance;

        internal readonly IDictionary<double, List<VectorNode>> _hashedCollection;

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

        public virtual IList<IVector> Nearest(IVector In)
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

        public virtual IList<IVector> Exact(IVector In)
        {
            List<IVector> candidates = new List<IVector>();
            var angle = GenerateAngles(In);
            if (_hashedCollection.ContainsKey(angle[0]))
            {
                var hashed = _hashedCollection[angle[0]];
                foreach (var element in hashed)
                {
                    element._vector._distance = Distance(In.coordinate, element._vector.coordinate);
                    if (element._vector._distance == 0)
                        candidates.Add(element._vector);
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
            foreach (var hashkey in _hashedCollection)
            {
                all.AddRange(hashkey.Value.Select<VectorNode, IVector>(x => x._vector).ToList());
            }
            return all;
        }

        public virtual bool Remove(IVector In)
        {
            if (In.uniqueId == 0)
            {
                throw new UniqueIdExceptions("UniqueId not set!!");
            }

            foreach(var hashed in _hashedCollection.Values){
                foreach(var vector in hashed)
                {
                if (In.uniqueId.Equals(vector._vector.uniqueId))
                {
                    hashed.Remove(vector);
                    return true;
                }
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
                var len = Factorial(n) / (Factorial(n - r) * Factorial(r));
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

            private bool InRange(double[] angle1, double[] angle2, double deviation)
            {

                for (int i = 0; i < angle1.Length; i++)
                {
                    if (!(angle2[i] >= (angle1[i] - deviation) && angle2[i] <= (angle1[i] + deviation)))
                    {
                        return false;
                    }
                }
                return true;
            }
 
            private bool InRange(double angle1, double angle2, double deviation)
            {
                    if (!(angle2 >= (angle1 - deviation) && angle2 <= (angle1 + deviation)))
                    {
                        return false;
                    }           
                return true;
            }

            private double StandardDeviation()
            {
               var mean = _mean;
               double sum = 0;
                sum = _hashedCollection.Sum(x => Math.Pow(x.Key - mean,2));

                if (sum == 0) /* standard deviation = 0 is not ideal so atleast 1 degree of check is required. */
                {
                    return 1;
                }
                return Math.Sqrt(sum / (_hashedCollection.Count > 10 ? N : N-1));
            }

            private long Unique()
            {
                return DateTime.Now.Ticks;
            }

    #endregion

    }


}
