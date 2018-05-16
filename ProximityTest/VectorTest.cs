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
        public void RemoveNotFound()
        {
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 2000, year = 2000, price = 4000 });
            _cars.Plot(new car() { odometer = 3000, year = 2000, price = 3000 });

            var _car = new car() { odometer = 1000, year = 2000, price = 6000 };
            Assert.IsFalse(_cars.Remove(_car));
        }

        [TestMethod]
        public void RemoveFound()
        {
            _cars.Plot(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 2000, year = 2000, price = 4000 });
            _cars.Plot(new car() { odometer = 3000, year = 2000, price = 3000 });
            _cars.Plot(new car() { odometer = 2000, year = 2000, price = -4000 });

            var _car = new car() { odometer = 2000, year = 2000, price = 4000 };
            Assert.IsTrue(_cars.Remove(_car));
        }

         [TestMethod]
        public void RemoveWithId()
        {
            _cars.Plot(new car() { uniqueId = 10, odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { uniqueId = 11, odometer = 2000, year = 2000, price = 4000 });
            _cars.Plot(new car() { uniqueId = 12, odometer = 3000, year = 2000, price = 3000 });
            _cars.Plot(new car() { uniqueId = 13, odometer = 2000, year = 2000, price = -4000 });
            
             Assert.IsTrue(_cars.Remove(13));
        }

        [TestMethod]
        public void RemoveWithWrongId()
        {
            _cars.Plot(new car() { uniqueId = 10, odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(new car() { uniqueId = 11, odometer = 2000, year = 2000, price = 4000 });
            _cars.Plot(new car() { uniqueId = 12, odometer = 3000, year = 2000, price = 3000 });
         
            Assert.IsFalse(_cars.Remove(15));
        }

        [TestMethod]
        public void UpdateSuccess()
        {
            var Old = new car() { uniqueId = 10, odometer = 1000, year = 2000, price = 5000 };
            var New = new car() { uniqueId = 10, odometer = 2000, year = 2000, price = 4500 };
            
            _cars.Plot(Old);
            Assert.IsTrue(_cars.Update(Old: Old, New: New));

            Assert.IsTrue(_cars.Exact(New).Count > 0);
            Assert.IsTrue(_cars.Exact(Old).Count == 0);
        }

        [TestMethod]
        public void UpdateFailInvalidId()
        {
            var Old = new car() { uniqueId = 10, odometer = 1000, year = 2000, price = 5000 };
            var New = new car() { uniqueId = 11, odometer = 2000, year = 2000, price = 4500 };

            _cars.Plot(Old);
            try
            {
                _cars.Update(Old: Old, New: New);
                Assert.Fail();
            }
            catch(ProximityMatch.Exceptions.UniqueIdExceptions ex)
            {
                // Expected.
            }
            catch(Exception ex)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void UpdateFailNoId()
        {
            var Old = new car() { uniqueId = 10, odometer = 1000, year = 2000, price = 5000 };
            var New = new car() { odometer = 2000, year = 2000, price = 4500 };

            _cars.Plot(Old);
            try
            {
                _cars.Update(Old.uniqueId, New);
                Assert.Fail();
            }
            catch (ProximityMatch.Exceptions.UniqueIdExceptions ex)
            {
                // Expected.
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
            
        }

        [TestMethod]
        public void FindSuccess()
        {
            _cars.Plot(new car() { odometer = 1000, year = 2001, price = 5500 });
            _cars.Plot(new car() { odometer = 2000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 3000, year = 1999, price = 4500 });

            //Should return only 1 result.
            var _carL = _cars.Find(new car() { year = 2000 });
            Assert.IsTrue(_carL.Count == 1 && ((car)_carL[0]).year == 2000);
            //Should return only 1 result.
            _carL = _cars.Find(new car() { odometer = 2000, year = 2000 });
            Assert.IsTrue(_carL.Count == 1 && ((car)_carL[0]).year == 2000);
        }

        [TestMethod]
        public void FindFail()
        {
            _cars.Plot(new car() { odometer = 1000, year = 2001, price = 5500 });
            _cars.Plot(new car() { odometer = 2000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 2000, year = 1999, price = 4500 });

            //Should return only 1 result.
            var _carL = _cars.Find(new car() { year = 2002 });
            Assert.IsTrue(_carL.Count == 0);
            //Should return only 1 result.
            _carL = _cars.Find(new car() { odometer = 2000, year = 1998 });
            Assert.IsTrue(_carL.Count == 0);
        }

        [TestMethod]
        public void FindbyConditionSuccess()
        {
            _cars.Plot(new car() { odometer = 1000, year = 2001, price = 5500 });
            _cars.Plot(new car() { odometer = 2000, year = 2000, price = 5000 });
            _cars.Plot(new car() { odometer = 3000, year = 1999, price = 4500 });

            //Should return only 1 result.
            var _carL = _cars.Find(new car() { year = 2000 }, x => ((car)x).price == 5000);
            Assert.IsTrue(_carL.Count == 1 && ((car)_carL[0]).year == 2000);    
         }

        [TestMethod]
        public void FindById()
        {
            _cars.Plot(new car() { uniqueId = 10025, odometer = 1000, year = 2001, price = 5500 });

            //Should return only 1 result.
            var _carL = _cars.Find(10025);
            Assert.IsNotNull(_carL);   
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
