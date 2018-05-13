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
    public delegate void FinishedEventHandler(object sender, ProximityMatchEventArgs e);

    public class Vector
    {       
        public int? take { get; set; }        
        protected readonly int _dimension;
        protected int _distance;
        protected int _spread;

        protected readonly IDictionary<double, IDictionary<double, List<VectorNode>>> _hashedCollection;
        protected readonly IDictionary<long, IVector> _hashedDictionary;


        public event FinishedEventHandler FinishedPloting;
        public event FinishedEventHandler FinishedRemove;

        private double Mu = 0, N = 0;
        private double _mean { get { return Mu / N; } }
        private Random _rand;

        public Vector(
            int dimension = 3, 
            int distance = 5,
            int spread = 0
            )
        {
            _dimension = dimension;
            _hashedCollection = new Dictionary<double, IDictionary<double, List<VectorNode>>>();
            _hashedDictionary = new Dictionary<long, IVector>();
            _rand = new Random();
            SetDistance(distance);
            SetSpread(spread);
        }

        public void Plot(IVector vector)
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

            var vnode = new VectorNode(
                    uniqueid: vector.uniqueId,
                    angle: GenerateAngles(vector),
                    distance : Distance(vector.coordinate, CreateDefault(vector.coordinate.Length))  
                );

            var angle = Math.Truncate(vnode._anglePlain[0]);
            var _distanceOrgin = Math.Truncate(vnode._distanceOrgin / 10);
            Mu += angle;
            N++;
            
            if (_hashedCollection.ContainsKey(angle))
            {
                if (_hashedCollection[angle].ContainsKey(_distanceOrgin))
                {
                    _hashedCollection[angle][_distanceOrgin].Add(vnode);
                    _hashedDictionary.Add(vnode._uniqueID, vector);
                }
                else
                {
                    _hashedCollection[angle].Add(_distanceOrgin, new List<VectorNode> { vnode });
                    _hashedDictionary.Add(vnode._uniqueID, vector);
                }
            }
            else
            {
                var LinkedL = new Dictionary<double, List<VectorNode>>();   

                LinkedL.Add(_distanceOrgin, new List<VectorNode>(){ vnode });
                _hashedCollection.Add(angle, LinkedL);
                _hashedDictionary.Add(vnode._uniqueID, vector);
            }
            EventFinishedPloting(new PlotEventArgs(vnode));
        }

        public void Plot(IList<IVector> vectorList)
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

        public IList<IVector> Nearest(IVector In)
        {
            var angles = GenerateAngles(In);
            var distanceOrgin = Distance(In.coordinate, CreateDefault(In.coordinate.Length));
            distanceOrgin = Math.Truncate(distanceOrgin / 10);
            List<IVector> candidates = new List<IVector>();
            var sdAngle = StdDev_Angle();
            double _distance;
            var angle = Math.Truncate(angles[0]);
            foreach (var hashedAngles in _hashedCollection)
            {
                if (InRange(angle, hashedAngles.Key, sdAngle))
                {
                    foreach (var hashedDistance in hashedAngles.Value)
                    {
                        if (InRange(distanceOrgin, hashedDistance.Key, GetDistance()))
                        {
                            foreach (var vnode in hashedDistance.Value)
                            {
                                var _vnodeObj = _hashedDictionary[vnode._uniqueID];
                                _distance = Distance(In.coordinate, _vnodeObj.coordinate);
                                if (_distance < GetDistance())
                                    if (_vnodeObj.uniqueId != In.uniqueId)
                                    {
                                        _vnodeObj._distance = _distance;
                                        candidates.Add(_vnodeObj);
                                    }
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

        public IList<IVector> Nearest(IVector In, Func<bool> condition)
        {
            var angles = GenerateAngles(In);
            var distanceOrgin = Distance(In.coordinate, CreateDefault(In.coordinate.Length));
            distanceOrgin = Math.Truncate(distanceOrgin / 10);
            List<IVector> candidates = new List<IVector>();
            var sdAngle = StdDev_Angle();
            double _distance;
            var angle = Math.Truncate(angles[0]);
            foreach (var hashedAngles in _hashedCollection)
            {
                if (InRange(angle, hashedAngles.Key, sdAngle))
                {
                    foreach (var hashedDistance in hashedAngles.Value)
                    {
                        if (InRange(distanceOrgin, hashedDistance.Key, GetDistance()))
                        {
                            foreach (var vnode in hashedDistance.Value)
                            {
                                var _vnodeObj = _hashedDictionary[vnode._uniqueID];
                                _distance = Distance(In.coordinate, _vnodeObj.coordinate);
                                if (_distance < GetDistance())
                                    if (_vnodeObj.uniqueId != In.uniqueId)
                                    {
                                        if (condition())
                                        {
                                            _vnodeObj._distance = _distance;
                                            candidates.Add(_vnodeObj);
                                        }
                                    }
                            }
                        }
                    }
                }
            }
            if (take.HasValue)
                return candidates.OrderBy(x => x._distance).Take(take.Value).ToList();
            else
                return candidates.OrderBy(x => x._distance).ToList();
        }

        public IList<IVector> NearestFullScan(IVector In)
        {
            List<IVector> candidates = new List<IVector>();
            foreach (var vector in GetAll())
                {
                    vector._distance = Distance(In.coordinate, vector.coordinate);
                    if (vector._distance < GetDistance())
                    {
                        if (vector.uniqueId != In.uniqueId)
                            candidates.Add(vector);
                    }
                }            
            if(take.HasValue)
                return candidates.OrderBy(x => x._distance).Take(take.Value).ToList();
            else
                return candidates.OrderBy(x => x._distance).ToList();
        }

        public IList<IVector> NearestRangeScan(IVector In, double[] range)
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
                    if (vector.uniqueId != In.uniqueId)
                        candidates.Add(vector);
                }
            }
            if (take.HasValue)
                return candidates.OrderBy(x => x._distance).Take(take.Value).ToList();
            else
                return candidates.OrderBy(x => x._distance).ToList();
        }

        public IList<IVector> Exact(IVector In)
        {
            List<IVector> candidates = new List<IVector>();
            var angles = GenerateAngles(In);
            var distanceOrgin = Distance(In.coordinate, CreateDefault(In.coordinate.Length));
            distanceOrgin = Math.Truncate(distanceOrgin / 10);
            double _distance;
            var angle = Math.Truncate(angles[0]);
            if (_hashedCollection.ContainsKey(angle))
            {
                var hashedAngles = _hashedCollection[angle];
                foreach (var hashedDistance in hashedAngles.Keys)
                {
                    if(hashedDistance == distanceOrgin)
                    {
                        foreach(var vnode in hashedAngles[hashedDistance])
                        {
                            var _vnodeObj = _hashedDictionary[vnode._uniqueID];
                            _distance = Distance(In.coordinate, _vnodeObj.coordinate);
                            if (_distance == 0)
                                candidates.Add(_vnodeObj);
                        }
                    }
                }
            }
            if (take.HasValue)
                return candidates.OrderBy(x => x._distance).Take(take.Value).ToList();
            else
                return candidates.OrderBy(x => x._distance).ToList();
        }

        public IList<IVector> Exact(IVector In, Func<bool> condition)
        {
            List<IVector> candidates = new List<IVector>();
            var angles = GenerateAngles(In);
            var distanceOrgin = Distance(In.coordinate, CreateDefault(In.coordinate.Length));
            distanceOrgin = Math.Truncate(distanceOrgin / 10);
            double _distance;
            var angle = Math.Truncate(angles[0]);
            if (_hashedCollection.ContainsKey(angle))
            {
                var hashedAngles = _hashedCollection[angle];
                foreach (var hashedDistance in hashedAngles.Keys)
                {
                    if (hashedDistance == distanceOrgin)
                    {
                        foreach (var vnode in hashedAngles[hashedDistance])
                        {
                            var _vnodeObj = _hashedDictionary[vnode._uniqueID];
                            _distance = Distance(In.coordinate, _vnodeObj.coordinate);
                            if (_distance == 0)
                                if(condition())
                                    candidates.Add(_vnodeObj);
                        }
                    }
                }
            }
            if (take.HasValue)
                return candidates.OrderBy(x => x._distance).Take(take.Value).ToList();
            else
                return candidates.OrderBy(x => x._distance).ToList();
        }

        public IList<IVector> GetAll()
        {
            return _hashedDictionary.Values.ToList();
        }
       
        public bool Remove(IVector In)
        {
            bool ret = false;
            var angles = GenerateAngles(In);
            var distanceOrgin = Distance(In.coordinate, CreateDefault(In.coordinate.Length));
            distanceOrgin = Math.Truncate(distanceOrgin / 10);
            var angle = Math.Truncate(angles[0]);
            if (_hashedCollection.ContainsKey(angle))
            {
                var hashedDistances = _hashedCollection[angle];
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
                            var vnodeObj = _hashedDictionary[vnode._uniqueID];
                            if (Distance(In.coordinate, vnodeObj.coordinate) == 0)
                            {
                                Mu -= vnode._anglePlain[0];
                                N--;
                                //remove node.
                                _vectorNodeList.RemoveAt(j);
                                _hashedDictionary.Remove(vnodeObj.uniqueId);
                                ret = true;
                                EventFinishedRemove(new RemoveEventArgs(vnode._uniqueID, true));
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
                if (_hashedCollection[angle] == null || _hashedCollection[angle].Count == 0)
                {
                    //No nodes empty dictionary so remove the key from parent.
                    _hashedCollection.Remove(angle);
                }
            }           
            return ret;
        }

        public bool Remove(long uniqueId)
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
                        if(uniqueId == _vectorNodeList[k]._uniqueID)
                        {
                            Mu -= _vectorNodeList[k]._anglePlain[0];
                            N--;
                            _vectorNodeList.RemoveAt(k);
                            _hashedDictionary.Remove(uniqueId);
                            ret = true;
                            EventFinishedRemove(new RemoveEventArgs(uniqueId, true));
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

        public bool Update(IVector Old, IVector New)
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

        public bool Update(long uniqueId, IVector New)
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

 
        public virtual void SetDistance(int distance)
        {
            _distance = distance;
        }

        public virtual void SetSpread(int spread)
        {
            _spread = spread;
        }


    #region private_functions

            private double[] GenerateAngles(IVector vector)
            {
                var n = vector.coordinate.Length;
                var angleList = new double[n];
                var orgin = CreateDefault(n);
                
                var slope = Distance(vector.coordinate, orgin);
                for (int i = 0; i < n; i++)
                {
                    var costheta = vector.coordinate[i] / slope;
                    var angle = Math.Acos(costheta) * (180 / Math.PI);
                    angleList[i] = Math.Round(angle, 4);
                }
                return angleList;
            }

            private double[] CreateDefault(int size)
            {
                var _double = new double[size];
                for (int i = 0; i < size; i++)
                {
                    _double[i] = 0;
                }
                return _double;
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
                if (GetSpread() > 0)
                {
                    return GetSpread();
                }
                else
                {
                    var mean = _mean;
                    double sum = 0;
                    sum = _hashedCollection.Sum(x => Math.Pow(x.Key - mean, 2));

                    if (sum == 0) /* standard deviation = 0 is not ideal so atleast 1 degree of check is required. */
                    {
                        return 1;
                    }
                    return Math.Round(Math.Sqrt(sum / (_hashedCollection.Count > 10 ? N : N - 1)), 4);
                }
            }

            private double StdDev_Distance(ICollection<double> Keys)
            {
                var count = Keys.Count();
                var mean = Keys.Sum() / count;
                double sum = 0;
                sum = Keys.Sum(x => Math.Pow(x - mean, 2));

                if (sum == 0) /* standard deviation = 0 is not ideal so atleast 1 degree of check is required. */
                {
                    return GetDistance() / 2;
                }
                return Math.Round(Math.Sqrt(sum / (count > 10 ? count : count - 1)), 4);
            }

    #endregion

    #region protected_functions

            protected virtual long Unique()
            {
                return LongRandom(0, long.MaxValue);
            }

            long LongRandom(long min, long max)
            {
                byte[] buf = new byte[8];
                _rand.NextBytes(buf);
                long longRand = BitConverter.ToInt64(buf, 0);

                return (Math.Abs(longRand % (max - min)) + min);
            }

            protected virtual int GetDistance()
            {
                return _distance;
            }

            protected virtual int GetSpread()
            {
                return _spread;
            }

            protected virtual void EventFinishedPloting(ProximityMatchEventArgs e)
            {
                if (FinishedPloting != null)
                    FinishedPloting(sender: this, e: e);
            }

            protected virtual void EventFinishedRemove(ProximityMatchEventArgs e)
            {
                if (FinishedRemove != null)
                    FinishedRemove(sender: this, e: e);
            }
            
        #endregion

    }
}
