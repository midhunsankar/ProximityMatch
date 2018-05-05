﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProximityMatch
{
    public class Vector
    {
        private readonly int _dimension;
        protected readonly SortedList<double, LinkedList<VectorNode>> _index;

        protected readonly IList<VectorNode> _indexfull;


        protected class VectorNode
        {
            public IVector _vector;
            public double[] _anglePlain;
            public double _distanceOrgin;
            public double _distance;
        }

        public Vector(int dimension = 3)
        {
            _dimension = dimension;
            _index = new SortedList<double, LinkedList<VectorNode>>();

            _indexfull = new List<VectorNode>();
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
                    _anglePlain = generateAngles(vector),
                    _distanceOrgin = distance(new double[3]{0, 0, 0} , vector.coordinate)
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

            _indexfull.Add(v);
        }

        public IList<IVector> nearest(IVector In, int take = 10)
        {
            var angle = generateAngles(In);
            var lower = angle[0] - 3;
            var upper = angle[0] + 3;
            bool forward = true, backward = true;
            List<VectorNode> candidates = new List<VectorNode>();
            var index = _index.IndexOfKey(angle[0]);
            int i = index;

            foreach (var E in _index.ElementAt(i).Value.ToList())
            {
                E._distance = distance(In.coordinate, E._vector.coordinate);
                if (InRange(angle, E._anglePlain, 3) && E._distance < 10)
                {
                    candidates.Add(E);
                }
            }

            while (forward)
            {
                i++;
                if (i < _index.Count)
                {
                    var L = _index.ElementAt(i).Key;
                    if (L <= upper)
                    {
                        foreach (var E in _index.ElementAt(i).Value.ToList())
                        {
                            E._distance = distance(In.coordinate, E._vector.coordinate);
                            if (InRange(angle, E._anglePlain, 3) && E._distance < 10)
                            {
                                candidates.Add(E);
                            }
                        }
                    }
                    else
                    {
                        forward = false;
                    }
                }
                else
                {
                    forward = false;
                }
            }
            i = index;
            while (backward)
            {
                i--;
                if (i >= 0)
                {
                    var L = _index.ElementAt(i).Key;
                    if (L >= lower)
                    {
                        foreach (var E in _index.ElementAt(i).Value.ToList())
                        {
                            E._distance = distance(In.coordinate, E._vector.coordinate);
                            if (InRange(angle, E._anglePlain, 3) && E._distance < 10)
                            {
                                candidates.Add(E);
                            }
                        }
                    }
                    else
                    {
                        backward = false;
                    }
                }
                else
                {
                    backward = false;
                }
            }
            return candidates.OrderBy(x =>x._distance).Take(take).Select<VectorNode, IVector>(x => x._vector).ToList();
        }

        public IList<IVector> nearestFull(IVector In, int take = 10)
        {
            List<VectorNode> candidates = new List<VectorNode>();

            foreach (var E in _indexfull)
            {
                E._distance = distance(In.coordinate, E._vector.coordinate);
                if (E._distance < 10)
                {
                    candidates.Add(E);
                }
            }
            return candidates.OrderBy(x => x._distance).Take(take).Select<VectorNode, IVector>(x => x._vector).ToList();
        }

        public IList<IVector> getAll()
        {
            List<IVector> all = new List<IVector>();
            foreach (var n in _index)
            {
                all.AddRange(n.Value.Select<VectorNode, IVector>(x => x._vector).ToList());
            }
            return all;
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

        private double distance(double[] co1, double[] co2)
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
        
    }


}
