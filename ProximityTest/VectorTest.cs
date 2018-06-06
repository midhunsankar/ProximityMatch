using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OctagonSquare;
using OctagonSquare.ProximityMatch;
using OctagonSquare.ProximityMatch.Exceptions;
using System.Collections.Generic;

namespace ProximityTest
{
    [TestClass]
    public class VectorTest
    {
        private Vector _cars;
        private IList<IVector> _carList;
        private long uniqueOut;

        public VectorTest()
        {
            _cars = new Vector(3);
            _carList = new List<IVector>();
        }

        [TestMethod]
        public void plotInvalidNumberOfDimension()
        {
            car _car = new car();
            _car.Coordinate = new double?[2];
            try
            {
                _carList.Add(_car);
                _cars.Plot(_carList);
                Assert.Fail();
            }
            catch (DimensionException)
            {
                // Expected result.
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void plotInvalidAtleast2Dimension()
        {
            car _car = new car();
            _car.Coordinate = new double?[1];
            try
            {
                _carList.Add(_car);
                _cars.Plot(_carList);
                Assert.Fail();
            }
            catch (DimensionException)
            {
                // Expected result.
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void nearestAllResult()
        {
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 5000 });
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 5000 });
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(_carList);

            //Should return 3 result. 
            var _carL = _cars.Nearest<car>(new car() { odometer = 1000, year = 2000, price = 5000 });
            Assert.IsTrue(_carL.Count == 3);
        }

        [TestMethod]
        public void nearestLimitedResult()
        {
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 5000 });
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 6000 });
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 20000 });
            _cars.Plot(_carList);

            //Should return only 1 result.
            var _carL = _cars.Nearest<car>(new car() { odometer = 1000, year = 2000, price = 5000 });
            Assert.IsTrue(_carL.Count == 1);
        }

        [TestMethod]
        public void nearestNoResult()
        {
            _carList.Add(new car() { odometer = 1000, year = -2000, price = 5000 });
            _carList.Add(new car() { odometer = 1000, year = 2000, price = -6000 });
            _carList.Add(new car() { odometer = -1000, year = 2000, price = 20000 });
            _cars.Plot(_carList);

            //Should return only 1 result.
            var _carL = _cars.Nearest<car>(new car() { odometer = 1000, year = 2000, price = 5000 });
            Assert.IsTrue(_carL.Count == 0);
        }

        [TestMethod]
        public void fullAndIndexScanAreEqual()
        {
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 5000 });
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 5000 });
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(_carList);

            var _car = new car() { odometer = 1000, year = 1999, price = 5010 };
            var n1 = _cars.Nearest<car>(_car).Count;
            var n2 = _cars.NearestFullScan(_car).Count;
            Assert.AreEqual(n1,n2);
        }

        [TestMethod]
        public void fullAndIndexScanAreNotEqual()
        {
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 5000 });
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 5000 });
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 5000 });
            _cars.Plot(_carList);

            var _car = new car() { odometer = 1000, year = 2000, price = 5004 };
            var _car2 = new car() { odometer = 1000, year = 2000, price = 10000 };
            Assert.AreNotEqual(_cars.Nearest<car>(_car2).Count, _cars.NearestFullScan(_car).Count);
        }

        [TestMethod]
        public void exactMatchResult()
        {
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 5000 });
            _carList.Add(new car() { odometer = 1000, year = -2000, price = 5000 });
            _carList.Add(new car() { odometer = -1000, year = 2000, price = -5000 });
            _cars.Plot(_carList);

            //Should return only 1 result.
            var _carL = _cars.Exact<car>(new car() { odometer = 1000, year = 2000, price = 5000 });
            Assert.IsTrue(_carL.Count == 1);
        }
        
        [TestMethod]
        public void exactMatchNoResult()
        {
            _carList.Add(new car() { odometer = -1000, year = 2000, price = 5000 });
            _carList.Add(new car() { odometer = 1000, year = -2000, price = 5000 });
            _carList.Add(new car() { odometer = 1000, year = 2000, price = -5000 });
            _cars.Plot(_carList);

            //Should return only 1 result.
            var _carL = _cars.Exact<car>(new car() { odometer = 1000, year = 2000, price = 5000 });
            Assert.IsTrue(_carL.Count == 0);
        }

        [TestMethod]
        public void RemoveNotFound()
        {
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 5000 });
            _carList.Add(new car() { odometer = 2000, year = 2000, price = 4000 });
            _carList.Add(new car() { odometer = 3000, year = 2000, price = 3000 });
            _cars.Plot(_carList);

            var _car = new car() { odometer = 1000, year = 2000, price = 6000 };
            Assert.IsFalse(_cars.RemoveAll(_car));
        }

        [TestMethod]
        public void RemoveFound()
        {
            _carList.Add(new car() { odometer = 1000, year = 2000, price = 5000 });
            _carList.Add(new car() { odometer = 2000, year = 2000, price = 4000 });
            _carList.Add(new car() { odometer = 3000, year = 2000, price = 3000 });
            _carList.Add(new car() { odometer = 2000, year = 2000, price = -4000 });
            _carList.Add(new car() { odometer = 2000, year = 2000, price = 4000 });
            _cars.Plot(_carList);

            var _car = new car() { odometer = 2000, year = 2000, price = 4000 };
            Assert.IsTrue(_cars.RemoveAll(_car));
        }

         [TestMethod]
        public void RemoveWithId()
        {
            _carList.Add(new car() { UniqueId = 10, odometer = 1000, year = 2000, price = 5000 });
            _carList.Add(new car() { UniqueId = 11, odometer = 2000, year = 2000, price = 4000 });
            _carList.Add(new car() { UniqueId = 12, odometer = 3000, year = 2000, price = 3000 });
            _carList.Add(new car() { UniqueId = 13, odometer = 2000, year = 2000, price = -4000 });
            _carList.Add(new car() { UniqueId = 14, odometer = 2000, year = 2000, price = 8000 });
            _carList.Add(new car() { UniqueId = 15, odometer = 3000, year = 2000, price = 9000 });
            _cars.Plot(_carList);

             Assert.IsTrue(_cars.Remove(13));
        }

        [TestMethod]
        public void RemoveWithWrongId()
        {
            _carList.Add(new car() { UniqueId = 10, odometer = 1000, year = 2000, price = 5000 });
            _carList.Add(new car() { UniqueId = 11, odometer = 2000, year = 2000, price = 4000 });
            _carList.Add(new car() { UniqueId = 12, odometer = 3000, year = 2000, price = 3000 });
            _cars.Plot(_carList);
            try
            {
                Assert.IsFalse(_cars.Remove(15));
            }
            catch (UniqueIdException)
            {
                // Expected result.
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void UpdateSuccess()
        {
            var Old = new car() { UniqueId = 10, odometer = 1000, year = 2000, price = 5000 };

            _cars.Plot(Old, out uniqueOut);

            car New = _cars.Get(uniqueOut) as car;
            New.price = 4500;

            Assert.IsTrue(_cars.Update(In : New));
            Assert.AreEqual(uniqueOut,_cars.Get(uniqueOut).UniqueId);            
        }

        [TestMethod]
        public void UpdateFailInvalidId()
        {
            var Old = new car() { UniqueId = 10, odometer = 1000, year = 2000, price = 5000 };
            _carList.Add(Old);
            _cars.Plot(Old, out uniqueOut);
            try
            {
                Old.UniqueId = 11;
                Old.price = 4500;
                _cars.Update(In: Old);
                Assert.Fail();
            }
            catch(UniqueIdException)
            {
                // Expected.
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void UpdateFailNoId()
        {
            var Old = new car() { odometer = 1000, year = 2000, price = 5000 };
            _carList.Add(Old);
            _cars.Plot(Old, out uniqueOut);
            try
            {               
                Old.price = 4500;
                Old.UniqueId = 0;
                _cars.Update(In: Old);
                Assert.Fail();
            }
            catch (UniqueIdException)
            {
                // Expected.
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void FindSuccess()
        {
            _carList.Add(new car() { odometer = 1000, year = 2001, price = 5500 });
            _carList.Add(new car() { odometer = 2000, year = 2000, price = 5000 });
            _carList.Add(new car() { odometer = 3000, year = 1999, price = 4500 });
            _cars.Plot(_carList);

            //Should return only 1 result.
            var _carL = _cars.Find<car>(new car() { year = 2000 });
            Assert.IsTrue(_carL.Count == 1 && _carL[0].year == 2000);
            //Should return only 1 result.
            _carL = _cars.Find<car>(new car() { odometer = 2000, year = 2000 });
            Assert.IsTrue(_carL.Count == 1 && _carL[0].year == 2000);
        }

        [TestMethod]
        public void FindFailed()
        {
            _carList.Add(new car() { odometer = 1000, year = 2001, price = 5500 });
            _carList.Add(new car() { odometer = 2000, year = 2000, price = 5000 });
            _carList.Add(new car() { odometer = 2000, year = 1999, price = 4500 });
            _cars.Plot(_carList);

            //Should return only 1 result.
            var _carL = _cars.Find<car>(new car() { year = 2002 });
            Assert.IsTrue(_carL.Count == 0);
            //Should return only 1 result.
            _carL = _cars.Find<car>(new car() { odometer = 2000, year = 1998 });
            Assert.IsTrue(_carL.Count == 0);
        }

        [TestMethod]
        public void FindbyConditionSuccess()
        {
            _carList.Add(new car() { odometer = 1000, year = 2001, price = 5500 });
            _carList.Add(new car() { odometer = 2000, year = 2000, price = 5000 });
            _carList.Add(new car() { odometer = 3000, year = 1999, price = 4500 });
            _cars.Plot(_carList);

            //Should return only 1 result.
            var _carL = _cars.Find<car>(new car() { year = 2000 }, x => x.price == 5000);
            Assert.IsTrue(_carL.Count == 1 && _carL[0].year == 2000);    
         }

        [TestMethod]
        public void FindById()
        {
            _carList.Add(new car() { UniqueId = 10025, odometer = 1000, year = 2001, price = 5500 });
            _cars.Plot(_carList);

            //Should return only 1 result.
            var _carL = _cars.Get(10025);
            Assert.IsNotNull(_carL);   
        }
    }

    [TestClass]
    public class car : IVector
    {
        private double?[] _coordinateP = new double?[3];
        public double?[] Coordinate
        {
            get { return _coordinateP; }
            set { _coordinateP = value; }
        }
        public long UniqueId { get; set; }
        public double Distance { get; set; }

        private double xaxisP;
        private double yaxisP;
        private double zaxisP;

        public double odometer { get { return xaxisP; } set { xaxisP = value; _coordinateP[0] = value; } }
        public double price { get { return yaxisP; } set { yaxisP = value; _coordinateP[1] = value; } }
        public double year { get { return zaxisP; } set { zaxisP = value; _coordinateP[2] = value; } }

    }
}
