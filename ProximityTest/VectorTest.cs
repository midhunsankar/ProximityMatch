using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProximityMatch;

namespace ProximityTest
{
    [TestClass]
    public class VectorTest
    {
        private Vector _cars;

        public VectorTest()
        {
            _cars = new Vector(3);
        }

        [TestMethod]
        public void plotInvalidNumberOfDimension()
        {
            car _car = new car();
            _car.coordinate = new double[2];
            try
            {
                _cars.Plot(_car);
                Assert.Fail();
            }
            catch (ProximityMatch.Exceptions.DimensionExceptions ex)
            {
                // Expected result.
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void plotInvalidAtleast2Dimension()
        {
            car _car = new car();
            _car.coordinate = new double[1];
            try
            {
                _cars.Plot(_car);
                Assert.Fail();
            }
            catch (ProximityMatch.Exceptions.DimensionExceptions ex)
            {
                // Expected result.
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void nearestAllResult()
        {
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });
            
            //Should return 3 result. 
            var _carL = _cars.Nearest(new car() { odometer = 1000, year = 2000, price = 5000 });
            Assert.IsTrue(_carL.Count == 3);
        }

        [TestMethod]
        public void nearestLimitedResult()
        {
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 6000 });
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 20000 });

            //Should return only 1 result.
            var _carL = _cars.Nearest(new car() { odometer = 1000, year = 2000, price = 5000 });
            Assert.IsTrue(_carL.Count == 1);
        }

        [TestMethod]
        public void nearestNoResult()
        {
            _cars.Plot(new car() { odometer = 1000, year = -2000, price = 5000 });
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = -6000 });
            _cars.Plot(new car() { odometer = -1000, year = 2000, price = 20000 });

            //Should return only 1 result.
            var _carL = _cars.Nearest(new car() { odometer = 1000, year = 2000, price = 5000 });
            Assert.IsTrue(_carL.Count == 0);
        }

        [TestMethod]
        public void fullAndIndexScanAreEqual()
        {
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });

            var _car = new car() { odometer = 1000, year = 1999, price = 5010 };
            var n1 = _cars.Nearest(_car).Count;
            var n2 = _cars.NearestFullScan(_car).Count;
            Assert.AreEqual(n1,n2);
        }

        [TestMethod]
        public void fullAndIndexScanAreNotEqual()
        {
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });

            var _car = new car() { odometer = 1000, year = 2000, price = 5004 };
            var _car2 = new car() { odometer = 1000, year = 2000, price = 10000 };
            Assert.AreNotEqual(_cars.Nearest(_car2).Count, _cars.NearestFullScan(_car).Count);
        }

        [TestMethod]
        public void exactMatchResult()
        {
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 1000, year = -2000, price = 5000 });
            _cars.Plot(new car() { odometer = -1000, year = 2000, price = -5000 });

            //Should return only 1 result.
            var _carL = _cars.Exact(new car() { odometer = 1000, year = 2000, price = 5000 });
            Assert.IsTrue(_carL.Count == 1);
        }
        
        [TestMethod]
        public void exactMatchNoResult()
        {
            _cars.Plot(new car() { odometer = -1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 1000, year = -2000, price = 5000 });
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = -5000 });

            //Should return only 1 result.
            var _carL = _cars.Exact(new car() { odometer = 1000, year = 2000, price = 5000 });
            Assert.IsTrue(_carL.Count == 0);
        }

        [TestMethod]
        public void RemoveWithOutId()
        {
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });

            var _car = new car() { odometer = 1000, year = 2000, price = 5000 };
            try
            {
                _cars.Remove(_car);
                Assert.Fail();
            }
            catch (ProximityMatch.Exceptions.UniqueIdExceptions ex)
            {
                //Expected result.
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RemoveWithWrongId()
        {
            _cars.Plot(new car() { uniqueId = 10, odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { uniqueId = 11, odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { uniqueId = 12, odometer = 1000, year = 2000, price = 5000 });

            var _car = new car() { uniqueId = 15, odometer = 1000, year = 2000, price = 5000 };
            Assert.IsFalse(_cars.Remove(_car));
        }

        [TestMethod]
        public void RemoveWithCorrectId()
        {
            _cars.Plot(new car() { uniqueId = 10, odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { uniqueId = 11, odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { uniqueId = 12, odometer = 1000, year = 2000, price = 5000 });

            var _car = new car() { uniqueId = 11, odometer = 1000, year = 2000, price = 5000 };
            Assert.IsTrue(_cars.Remove(_car));
        }

    }

    [TestClass]
    public class car : IVector
    {
        private double[] _coordinateP = new double[3];
        public double[] coordinate
        {
            get { return _coordinateP; }
            set { _coordinateP = value; }
        }
        public long uniqueId { get; set; }
        public double _distance { get; set; }

        private double xaxisP;
        private double yaxisP;
        private double zaxisP;

        public double odometer { get { return xaxisP; } set { xaxisP = value; _coordinateP[0] = value; } }
        public double price { get { return yaxisP; } set { yaxisP = value; _coordinateP[1] = value; } }
        public double year { get { return zaxisP; } set { zaxisP = value; _coordinateP[2] = value; } }
    }
}
