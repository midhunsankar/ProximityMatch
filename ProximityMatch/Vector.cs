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
using OctagonSquare.ProximityMatch.Exceptions;


namespace OctagonSquare.ProximityMatch
{
    public delegate void FinishedEventHandler(object sender, ProximityMatchEventArgs e);

    public class Vector
    {
        public int? Take { get; set; }
        public ProximityMatch.Enums.OrderBy OrderBy { get; set; }
        
        protected readonly int Dimension;
        protected int Distance;
        protected int Spread;

        protected readonly IDictionary<double, IDictionary<double, List<VectorNode>>> VectorDictionary;
        protected readonly IDictionary<long, IVector> ItemDictionary;
        protected IDictionary<double, List<long>>[] HashDictionary;

        public event FinishedEventHandler PlotingFinished;
        public event FinishedEventHandler RemoveFinished;

        private double mu = 0; // The summation of data points.
        private double n = 0; // Number of data points. 
        private double mean { get { return mu / n; } } // The mean of data points.
        private Random random;
        private long uniqueidOut;

        public Vector(
            int dimension = 3,
            int distance = 5,
            int spread = 0
            )
        {
            this.Dimension = dimension;
            this.VectorDictionary = new Dictionary<double, IDictionary<double, List<VectorNode>>>();
            this.ItemDictionary = new Dictionary<long, IVector>();
            this.HashDictionary = new Dictionary<double, List<long>>[dimension];
            this.OrderBy = ProximityMatch.Enums.OrderBy.Asc;

            random = new Random();

            SetProximityDistance(distance);
            SetProximitySpread(spread);
        }

        /// <summary>
        /// Plot vector nodes to the Eucledian plain.
        /// </summary>
        /// <param name="vector">A vector Node/Coordinate.</param>
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.DimensionException">Thrown when invalid number of dimensions.</exception>  
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.CoordinateException">Thrown when coordinates are missing.</exception>
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.UniqueIdException">Thrown when uniqueId is exists.</exception>
        public void Plot(IVector vector, out long uniqueid)
        {
            try
            {
                if (this.Dimension != vector.Coordinate.Length)
                {
                    throw new DimensionException("Invalid dimension!!.");
                }

                if (this.Dimension < 2)
                {
                    throw new DimensionException("Atleast two dimensions are required!!.");
                }

                if (!vector.Coordinate.All(x => x.HasValue))
                {
                    throw new CoordinateException("Missing coordinate.");
                }

                if (vector.UniqueId <= 0)
                {
                    vector.UniqueId = GenerateUniqueId();
                }

                //All good, try create a vector node/coordinate now.

                if (this.ItemDictionary.ContainsKey(vector.UniqueId))
                {
                    throw new UniqueIdException("UniqueId already exist!!.");
                }

                this.ItemDictionary.Add(vector.UniqueId, vector.Copy<IVector>());
                uniqueid = vector.UniqueId;

                var vectorNode = new VectorNode(
                        uniqueid: vector.UniqueId,
                        angles: GenerateAngles(vector),
                        distance: EuclideanDistance(vector.Coordinate, CreateDouble(vector.Coordinate.Length))
                    );

                //Indexing vectors based on cos(beta) angle and Euclidean distance from orgin. 
                var angleKey = Math.Truncate(vectorNode.Angles[0]);
                var distanceFromOrginKey = Math.Truncate(vectorNode.DistanceFromOrgin / 10);

                if (this.VectorDictionary.ContainsKey(angleKey))
                {
                    if (this.VectorDictionary[angleKey].ContainsKey(distanceFromOrginKey))
                    {
                        this.VectorDictionary[angleKey][distanceFromOrginKey].Add(vectorNode);
                    }
                    else
                    {
                        this.VectorDictionary[angleKey].Add(distanceFromOrginKey, new List<VectorNode> { vectorNode });
                    }
                }
                else
                {
                    var distanceFromOrginDictionary = new Dictionary<double, List<VectorNode>>();
                    distanceFromOrginDictionary.Add(distanceFromOrginKey, new List<VectorNode>() { vectorNode });
                    this.VectorDictionary.Add(angleKey, distanceFromOrginDictionary);
                }

                //Build indexes.
                AddIndex(vector);

                mu += angleKey;
                n++;

                //Ploting done!! now fire the finished event. 
                PlotingFinishedEvent(new PlotEventArgs(vectorNode));
            }
            catch (Exception exception)
            {                
                throw exception;
            }
        }

        /// <summary>
        /// Pass an array of vector nodes for plotting.
        /// </summary>
        /// <param name="vectorList">A list of vector Node/Coordinate.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when list is null or empty.</exception>
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.DimensionException">Thrown when invalid number of dimensions.</exception>  
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.CoordinateException">Thrown when coordinates are missing.</exception>
        public void Plot(IList<IVector> vectorList)
        {
            if (vectorList == null || vectorList.Count == 0)
            {
                throw new ArgumentNullException();
            }
            foreach (var vector in vectorList)
            {
                try
                {
                    Plot(vector: vector, uniqueid: out uniqueidOut);
                }
                catch (Exception exception)
                {
                    throw exception;
                }                
            }
        }

        /// <summary>
        /// Find the nearest vector coordinates.
        /// </summary>
        /// <typeparam name="T">Classes that implements IVector interface.</typeparam>
        /// <param name="In">Input vector.</param>
        /// <returns>A collection of nearest vectors.</returns>
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.CoordinateException">Thrown when coordinates are missing.</exception>
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.UniqueIdException">Thrown when uniqueId exists.</exception>
        public IList<T> Nearest<T>(IVector In) where T : IVector 
        {           
            if (!In.Coordinate.All(x => x.HasValue))
            {
                throw new CoordinateException("Missing coordinate.");
            }

            var candidates = new List<T>();
            foreach (T vector in FindOutNearestMatches(In: In))
            {
                candidates.Add(vector);
            }
            if (Take.HasValue)
                return candidates.OrderBy(x => x.Distance).Take(Take.Value).ToList();
            else
                return candidates.OrderBy(x => x.Distance).ToList();
        }

        /// <summary>
        /// Find the nearest vector coordinates.
        /// </summary>
        /// <typeparam name="T">Classes that implements IVector interface.</typeparam>
        /// <param name="In">Input vector.</param>
        /// <param name="where">Pass a boolean expression.</param>
        /// <param name="orderby">Pass an orderby expression.</param>
        /// <returns>A collection of nearest vectors.</returns>
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.CoordinateException">Thrown when coordinates are missing.</exception>
        public IList<T> Nearest<T>(IVector In, Func<T, bool> where, Func<T, double> orderby = null) where T : IVector
        {
            if (!In.Coordinate.All(x => x.HasValue))
            {
                throw new CoordinateException("Missing coordinate.");
            }

            var candidates = new List<T>();
            foreach (T vector in FindOutNearestMatches(In: In))
            {
                if(where(vector))
                    candidates.Add(vector);
            }
            if (orderby == null)
            {
                candidates = candidates.OrderBy(x => x.Distance).ToList();
            }
            else
            {
                if (this.OrderBy == ProximityMatch.Enums.OrderBy.Asc)
                {
                    candidates = candidates.OrderBy(orderby).ToList();
                }
                else if (this.OrderBy == ProximityMatch.Enums.OrderBy.Desc)
                {
                    candidates = candidates.OrderByDescending(orderby).ToList();
                }
                else
                {
                    candidates = candidates.OrderBy(x => x.Distance).ToList();
                }
            }

            if (Take.HasValue)
                return candidates.Take(Take.Value).ToList();
            else
                return candidates;
        }

        /// <summary>
        /// Find the nearest vector coordinates.
        /// </summary>
        /// <remarks>This method is slow on big dataset, since it has to perform a full scan.</remarks>
        /// <param name="In">Input vector.</param>
        /// <returns>A collection of nearest vectors.</returns>
        public IList<IVector> NearestFullScan(IVector In)
        {
            List<IVector> candidates = new List<IVector>();
            foreach (var vector in GetAll())
            {
                vector.Distance = EuclideanDistance(In.Coordinate, vector.Coordinate);
                if (vector.Distance < GetProximityDistance())
                {
                    if (vector.UniqueId != In.UniqueId)
                        candidates.Add(vector);
                }
            }
            if (Take.HasValue)
                return candidates.OrderBy(x => x.Distance).Take(Take.Value).ToList();
            else
                return candidates.OrderBy(x => x.Distance).ToList();
        }       

        /// <summary>
        /// Find the vectors which share same coordinate with the input vector.
        /// </summary>
        /// <typeparam name="T">Classes that implements IVector interface.</typeparam>
        /// <param name="In">Input vector.</param>
        /// <returns>A collection of vectors.</returns>
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.CoordinateException">Thrown when coordinates are missing.</exception>
        public IList<T> Exact<T>(IVector In) where T : IVector
        {
            if (!In.Coordinate.All(x => x.HasValue))
            {
                throw new CoordinateException("Missing coordinate.");
            }

            var candidates = new List<T>();
            foreach (T vector in FindOutExactMatches(In: In))
            {
                candidates.Add(vector);
            }
            if (Take.HasValue)
                return candidates.OrderBy(x => x.Distance).Take(Take.Value).ToList();
            else
                return candidates.OrderBy(x => x.Distance).ToList();
        }
        
        /// <summary>
        /// Find the vectors which share same coordinate with the input vector.
        /// </summary>
        /// <typeparam name="T">Classes that implements IVector interface.</typeparam>
        /// <param name="In">Input vector.</param>
        /// <param name="where">Pass a boolean expression.</param>
        /// <param name="orderby">Pass an orderby expression.</param>
        /// <returns>A collection of vectors.</returns>
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.CoordinateException">Thrown when coordinates are missing.</exception>
        public IList<T> Exact<T>(IVector In, Func<T, bool> where, Func<T, double> orderby = null) where T : IVector
        {
            if (!In.Coordinate.All(x => x.HasValue))
            {
                throw new CoordinateException("Missing coordinate.");
            }

            var candidates = new List<T>();
            foreach (T vector in FindOutExactMatches(In: In))
            {
                if(where(vector))
                    candidates.Add(vector);
            }
            if (orderby == null)
            {
                candidates = candidates.OrderBy(x => x.Distance).ToList();
            }
            else
            {
                if (this.OrderBy == ProximityMatch.Enums.OrderBy.Asc)
                {
                    candidates = candidates.OrderBy(orderby).ToList();
                }
                else if (this.OrderBy == ProximityMatch.Enums.OrderBy.Desc)
                {
                    candidates = candidates.OrderByDescending(orderby).ToList();
                }
                else
                {
                    candidates = candidates.OrderBy(x => x.Distance).ToList();
                }
            }

            if (Take.HasValue)
                return candidates.Take(Take.Value).ToList();
            else
                return candidates;
        }

        /// <summary>
        /// Find the vectors based on the input vector's coordinates.
        /// </summary>
        /// <remarks>A Index based search is performed, atleast one coordinate value is required.</remarks>
        /// <typeparam name="T">Classes that implements IVector interface.</typeparam>
        /// <param name="In">Input vector.</param>
        /// <returns>A collection of vectors.</returns>
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.CoordinateException">Thrown when coordinates are missing.</exception>
        public IList<T> Find<T>(IVector In) where T : IVector
        {
            if (!CheckAtleastOneCoordinateExist(In.Coordinate))
            {
                throw new CoordinateException("Set atleast one coordinate.");
            }

            IList<T> candidates = new List<T>();
            IList<long> processedKeys = new List<long>();            
            foreach (var uniqueIdList in IndexedSearch(In: In))
            {
                foreach (var uniqueId in uniqueIdList)
                {
                    if(!processedKeys.Contains(uniqueId))
                    {
                        T vector = (T)this.ItemDictionary[uniqueId];
                        processedKeys.Add(uniqueId);

                        if(CheckWithInRange(In.Coordinate, vector.Coordinate))
                        {                            
                            candidates.Add(vector);
                        }                            
                    }
                 }
             }
            return candidates;
        }

        /// <summary>
        /// Find the vectors based on the input vector's coordinates.
        /// </summary>
        /// <remarks>A Index based search is performed, atleast one coordinate value is required.</remarks>
        /// <typeparam name="T">Classes that implements IVector interface.</typeparam>
        /// <param name="In">Input vector.</param>
        /// <param name="where">Pass a boolean expression.</param>
        /// <param name="orderby">Pass an orderby expression.</param>
        /// <returns>A collection of vectors.</returns>
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.CoordinateException">Thrown when coordinates are missing.</exception>
        public IList<T> Find<T>(IVector In, Func<T, bool> where, Func<T, double> orderby = null) where T : IVector
        {
            if (!CheckAtleastOneCoordinateExist(In.Coordinate))
            {
                throw new CoordinateException("Set atleast one coordinate.");
            }

            IList<T> candidates = new List<T>();
            IList<long> processedKeys = new List<long>();
            bool fullscan = true;

            foreach(var c in In.Coordinate)
            {
                if(c != default(double?))
                {
                    fullscan = false;
                    break;
                }
            }

            if (fullscan)
            {
                foreach (T _vObj in GetAll())
                {
                    if (where(_vObj))
                        candidates.Add(_vObj);
                }
            }
            else
            {
                foreach (var uniqueIdList in IndexedSearch(In: In))
                {
                    foreach (var uniqueId in uniqueIdList)
                    {
                        if (!processedKeys.Contains(uniqueId))
                        {
                            T vector = (T)this.ItemDictionary[uniqueId];
                            processedKeys.Add(uniqueId);

                            if (CheckWithInRange(In.Coordinate, vector.Coordinate))
                            {
                                if (where(vector))
                                    candidates.Add(vector);
                            }
                        }
                    }
                }
            }

            if (orderby == null)
            {
                candidates = candidates.OrderBy(x => x.Distance).ToList();
            }
            else
            {
                if (this.OrderBy == ProximityMatch.Enums.OrderBy.Asc)
                {
                    candidates = candidates.OrderBy(orderby).ToList();
                }
                else if (this.OrderBy == ProximityMatch.Enums.OrderBy.Desc)
                {
                    candidates = candidates.OrderByDescending(orderby).ToList();
                }
                else
                {
                    candidates = candidates.OrderBy(x => x.Distance).ToList();
                }
            }

            if (Take.HasValue)
                return candidates.Take(Take.Value).ToList();
            else
                return candidates;
        }

        /// <summary>
        /// Get the vector by it's uniqueid.
        /// </summary>
        /// <remarks>Performs a hashed index search, Instant!!.</remarks>
        /// <param name="uniqueId">Input uniqueid.</param>
        /// <returns>A vector.</returns>
        public IVector Get(long uniqueId)
        {            
            if (this.ItemDictionary.ContainsKey(uniqueId))
                return (IVector)(this.ItemDictionary[uniqueId]).Copy();
            return null;
        }

        /// <summary>
        /// Get all vectors ploted.
        /// </summary>
        /// <returns>All vectors.</returns>
        public IList<IVector> GetAll()
        {
            return this.ItemDictionary.Values.ToList();
        }

        /// <summary>
        /// Remove vectors based on input vector.
        /// </summary>
        /// <remarks>if uniqueid is available remove specific vector, else remove all vectors that share same coordinate.</remarks>
        /// <param name="In"></param>
        /// <returns>Return true if success.</returns>
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.CoordinateException">Thrown when coordinates are missing.</exception>
        public bool RemoveAll(IVector In)
        {
            if (!In.Coordinate.All(x => x.HasValue))
            {
                throw new CoordinateException("Missing coordinate.");
            }

            List<long> uniqueIds;
            if (RemoveVector(In: In, uniqueIds: out uniqueIds))
            {
                foreach (var uniq in uniqueIds)
                {
                    RemoveIndex(this.ItemDictionary[uniq]);
                    this.ItemDictionary.Remove(uniq);
                    RemoveFinishedEvent(new RemoveEventArgs(uniq, true));
                }
                return true;
            }            
            return false;
        }

        /// <summary>
        /// Remove vectors based on input vector.
        /// </summary>
        /// <param name="uniqueId">Vector uniqueid.</param>
        /// <returns>Return true if success.</returns>
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.UniqueIdException">Thrown when uniqueId is missing.</exception>
        public bool Remove(long uniqueId)
        {
            if (!this.ItemDictionary.ContainsKey(uniqueId))
            {
                throw new UniqueIdException("UniqueId not found!!");
            }
            List<long> uniqueIds;
            IVector In = this.ItemDictionary[uniqueId];
            if (RemoveVector(In: In, uniqueIds: out uniqueIds))
            {
                foreach (var uniq in uniqueIds)
                {
                    RemoveIndex(this.ItemDictionary[uniq]);
                    this.ItemDictionary.Remove(uniq);
                    RemoveFinishedEvent(new RemoveEventArgs(uniq, true));
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Update the vector.
        /// </summary>
        /// <param name="In">Input vector.</param>
        /// <returns>Return true if success.</returns>
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.UniqueIdException">Thrown when uniqueId is missing.</exception>
        public bool Update(IVector In)
        {
            if (In.UniqueId <= 0)
            {
                throw new UniqueIdException("UniqueId not set!!");
            }
            else if (!ItemDictionary.ContainsKey(In.UniqueId))
            {
                throw new UniqueIdException("UniqueId not found!!");
            }
            else
            {
                var vectorOld = this.Get(In.UniqueId);
                if (Remove(vectorOld.UniqueId))
                {
                    Plot(vector: In, uniqueid: out uniqueidOut);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Replace a vector with new one.
        /// </summary>
        /// <param name="uniqueId">Old vector uniqueid.</param>
        /// <param name="In">Updated vector.</param>
        /// <returns>Return true if success.</returns>
        /// <exception cref="OctagonSquare.ProximityMatch.Exceptions.UniqueIdException">Thrown when uniqueId is missing.</exception>
        public bool Replace(long uniqueId, IVector In)
        {
            if (In.UniqueId <= 0)
            {
                throw new UniqueIdException("Input uniqueId not set!!");
            }
            else if (In.UniqueId <= 0)
            {
                throw new UniqueIdException("Input vector uniqueId not set!!");
            }
            else
            {
                if (Remove(uniqueId))
                {
                    if (ItemDictionary.ContainsKey(In.UniqueId))
                    {
                        Remove(In.UniqueId);
                    }
                    Plot(vector: In, uniqueid: out uniqueidOut);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Default value is 5 units, change it to expand or narrow the area. 
        /// </summary>
        /// <remarks>A positive value is expected.</remarks>
        /// <param name="distance"></param>
        public virtual void SetProximityDistance(int distance)
        {
            if(distance > 0)
                this.Distance = distance;
        }

        /// <summary>
        /// By default standard deviation is applied, change it to wide or narrow the spread.
        /// </summary>
        /// <param name="spread"></param>
        public virtual void SetProximitySpread(int spread)
        {
            if(spread > 0)
                this.Spread = spread;
        }


        #region private_functions

        private IEnumerable<IVector> FindOutNearestMatches(IVector In)
        {
            var angles = GenerateAngles(In);
            var distanceOrgin = EuclideanDistance(In.Coordinate, CreateDouble(In.Coordinate.Length));
            distanceOrgin = Math.Truncate(distanceOrgin / 10);
            //List<IVector> candidates = new List<IVector>();
            var sdAngle = CalculateAngleSpread();
            double _distance;
            var angle = Math.Truncate(angles[0]);
            foreach (var hashedAngles in this.VectorDictionary)
            {
                if (CheckWithInRange(angle, hashedAngles.Key, sdAngle))
                {
                    foreach (var hashedDistance in hashedAngles.Value)
                    {
                        if (CheckWithInRange(distanceOrgin, hashedDistance.Key, GetProximityDistance()))
                        {
                            foreach (var vnode in hashedDistance.Value)
                            {
                                var _vnodeObj = this.ItemDictionary[vnode.UniqueId];
                                _distance = EuclideanDistance(In.Coordinate, _vnodeObj.Coordinate);
                                if (_distance < GetProximityDistance())
                                    if (_vnodeObj.UniqueId != In.UniqueId)
                                    {
                                        _vnodeObj.Distance = _distance;
                                        yield return _vnodeObj;
                                    }
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<IVector> FindOutExactMatches(IVector In)
        {
            var angles = GenerateAngles(In);
            var distanceOrgin = EuclideanDistance(In.Coordinate, CreateDouble(In.Coordinate.Length));
            distanceOrgin = Math.Truncate(distanceOrgin / 10);
            double _distance;
            var angle = Math.Truncate(angles[0]);
            if (this.VectorDictionary.ContainsKey(angle))
            {
                var hashedAngles = this.VectorDictionary[angle];
                foreach (var hashedDistance in hashedAngles.Keys)
                {
                    if (hashedDistance == distanceOrgin)
                    {
                        foreach (var vnode in hashedAngles[hashedDistance])
                        {
                            var _vnodeObj = this.ItemDictionary[vnode.UniqueId];
                            _distance = EuclideanDistance(In.Coordinate, _vnodeObj.Coordinate);
                            if (_distance == 0)
                              yield return _vnodeObj;
                        }
                    }
                }
            }           
        }

        private IEnumerable<IList<long>> IndexedSearch(IVector In)
        {
            for (int i = 0; i < this.Dimension; i++)
            {
                if (In.Coordinate[i] != default(double?))
                {
                    var hashKey = (In.Coordinate[i] == 0) ? 0 : In.Coordinate[i] > 0 ? 10 * Math.Log10(In.Coordinate[i].Value) : -10 * Math.Log10(-1 * In.Coordinate[i].Value);
                    if (this.HashDictionary[i].ContainsKey(hashKey))
                    {
                        yield return this.HashDictionary[i][hashKey]; ;
                    }
                }
            }
        }

        private void AddIndex(IVector In)
        {
            for (int i = 0; i < this.Dimension; i++)
            {
                var hashKey = (In.Coordinate[i] == 0) ? 0 : In.Coordinate[i] > 0 ? 10 * Math.Log10(In.Coordinate[i].Value) : -10 * Math.Log10(-1 * In.Coordinate[i].Value);
                if (this.HashDictionary[i] == null)
                    this.HashDictionary[i] = new Dictionary<double, List<long>>();

                if (this.HashDictionary[i].ContainsKey(hashKey))
                {
                    this.HashDictionary[i][hashKey].Add(In.UniqueId);
                }
                else
                {
                    this.HashDictionary[i].Add(hashKey, new List<long>());
                    this.HashDictionary[i][hashKey].Add(In.UniqueId);
                }
            }
        }

        private void RemoveIndex(IVector In)
        {
            for (int i = 0; i < this.Dimension; i++)
            {
                var hashKey = (In.Coordinate[i] == 0) ? 0 : In.Coordinate[i] > 0 ? 10 * Math.Log10(In.Coordinate[i].Value) : -10 * Math.Log10(-1 * In.Coordinate[i].Value);

                if (this.HashDictionary[i].ContainsKey(hashKey))
                {
                    if (this.HashDictionary[i][hashKey].Contains(In.UniqueId))
                    {
                        this.HashDictionary[i][hashKey].Remove(In.UniqueId);
                        if (this.HashDictionary[i][hashKey].Count == 0)
                        {
                            this.HashDictionary[i].Remove(hashKey);
                        }
                    }
                }
            }
        }

        private bool RemoveVector(IVector In, out List<long> uniqueIds)
        {
            bool ret = false;
            var angles = GenerateAngles(In);
            var distanceOrgin = EuclideanDistance(In.Coordinate, CreateDouble(In.Coordinate.Length));
            distanceOrgin = Math.Truncate(distanceOrgin / 10);
            var angle = Math.Truncate(angles[0]);
            bool haveUniqueId = In.UniqueId != default(long);
            bool foundVector = false;

            uniqueIds = new List<long>();
            if (this.VectorDictionary.ContainsKey(angle))
            {
                var hashedDistances = this.VectorDictionary[angle];
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
                            if (haveUniqueId)
                            {
                                foundVector = In.UniqueId == vnode.UniqueId;
                            }
                            else
                            {
                                var vnodeObj = this.ItemDictionary[vnode.UniqueId];
                                foundVector = EuclideanDistance(In.Coordinate, vnodeObj.Coordinate) == 0;
                            }
                                                        
                            if (foundVector)
                            {
                                mu -= vnode.Angles[0];
                                n--;
                                //remove node.
                                _vectorNodeList.RemoveAt(j);
                                uniqueIds.Add(vnode.UniqueId);
                                ret = true;
                                if (haveUniqueId)
                                    break;
                                j--;
                            }
                            j++;
                        }

                        if (hashedDistances[_hashedDistanceOrgin] == null || hashedDistances[_hashedDistanceOrgin].Count == 0)
                        {
                            //No nodes empty dictionary so remove the key from parent.
                            hashedDistances.Remove(_hashedDistanceOrgin);
                            i--;
                        }

                        if (haveUniqueId && foundVector)
                            break;
                    }
                    i++;
                }
                if (this.VectorDictionary[angle] == null || this.VectorDictionary[angle].Count == 0)
                {
                    //No nodes empty dictionary so remove the key from parent.
                    this.VectorDictionary.Remove(angle);
                }
            }
            return ret;
        }

        private double[] GenerateAngles(IVector vector)
        {
            var n = vector.Coordinate.Length;
            var angleList = new double[n];
            var orgin = CreateDouble(n);

            var slope = EuclideanDistance(vector.Coordinate, orgin);
            for (int i = 0; i < n; i++)
            {
                var costheta = vector.Coordinate[i] / slope;
                var angle = Math.Acos(costheta.Value) * (180 / Math.PI);
                angleList[i] = Math.Round(angle, 4);
            }
            return angleList;
        }

        private double[] CreateDouble(int size)
        {
            return new double[size];
        }

        private int Factorial(int number)
        {
            if (number > 1)
                number = number * Factorial(number - 1);
            return number;
        }

        private double EuclideanDistance(double?[] co1, double?[] co2)
        {
            /*
               Distance = sqrt( (x2−x1)^2 + (y2−y1)^2 + (z2-z1)^2 )
             * For better explanation follow url : https://betterexplained.com/articles/measure-any-distance-with-the-pythagorean-theorem
             */
            double d = 0;
            for (int i = 0; i < co1.Length; i++)
            {
                if(co1[i].HasValue && co2[i].HasValue)
                    d += Math.Pow((co1[i].Value - co2[i].Value), 2);
            }
            return Math.Round(Math.Sqrt(d), 4);
        }

        private double EuclideanDistance(double?[] co1, double[] co2)
        {
            /*
               Distance = sqrt( (x2−x1)^2 + (y2−y1)^2 + (z2-z1)^2 )
             * For better explanation follow url : https://betterexplained.com/articles/measure-any-distance-with-the-pythagorean-theorem
             */
            double d = 0;
            for (int i = 0; i < co1.Length; i++)
            {
                if(co1[i].HasValue)
                    d += Math.Pow((co1[i].Value - co2[i]), 2);
            }
            return Math.Round(Math.Sqrt(d), 4);
        }

        private bool CheckWithInRange(double?[] coordinate1, double?[] coordinate2)
        {
            for (int i = 0; i < coordinate1.Length; i++)
            {
                if (coordinate1[i] != default(double?) && coordinate2[i] != coordinate1[i])
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckWithInRange(double coordinate1, double coordinate2, double deviation)
        {
            if (!(coordinate2 >= (coordinate1 - deviation) && coordinate2 <= (coordinate1 + deviation)))
            {
                return false;
            }
            return true;
        }

        private bool CheckWithInRange(double?[] coordinate1, double?[] coordinate2, double deviation)
        {
            for (int i = 0; i < coordinate1.Length; i++)
            {
                if (!(coordinate2[i] >= (coordinate1[i] - deviation) && coordinate2[i] <= (coordinate1[i] + deviation)))
                {
                    return false;
                }
            }
            return true;
        }

        private double CalculateAngleSpread()
        {
            if (GetProximitySpread() > 0)
            {
                return GetProximitySpread();
            }
            else
            {
                double sum = 0;
                sum = this.VectorDictionary.Sum(x => Math.Pow(x.Key - mean, 2));

                if (sum == 0) /* standard deviation = 0 is not ideal so atleast 1 degree of check is required. */
                {
                    return 1;
                }
                return Math.Round(Math.Sqrt(sum / (this.VectorDictionary.Count > 10 ? n : n - 1)), 4);
            }
        }

        private long GenerateRandom(long min, long max)
        {
            byte[] buf = new byte[8];
            random.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }

        private bool CheckAtleastOneCoordinateExist(double?[] coordinate)
        {
            for (int i = 0; i < coordinate.Length; i++)
            {
                if(coordinate[i].HasValue)
                    return true;
            }
            return false;
        }

        #endregion

        #region protected_functions

        /// <summary>
        /// Override this function if you want your own implementation.
        /// </summary>
        /// <returns></returns>
        protected virtual long GenerateUniqueId()
        {
            return GenerateRandom(0, long.MaxValue);
        }
  
        protected virtual int GetProximityDistance()
        {
            return this.Distance;
        }

        protected virtual int GetProximitySpread()
        {
            return this.Spread;
        }

        /// <summary>
        /// Event fired after ploting.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void PlotingFinishedEvent(ProximityMatchEventArgs e)
        {
            if (this.PlotingFinished != null)
                this.PlotingFinished(sender: this, e: e);
        }

        /// <summary>
        /// Event fired after remove vector.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void RemoveFinishedEvent(ProximityMatchEventArgs e)
        {
            if (this.RemoveFinished != null)
                this.RemoveFinished(sender: this, e: e);
        }

        #endregion

    }
}
