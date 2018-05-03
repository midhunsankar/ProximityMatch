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
using ProximityMatch;

namespace ProximityMatchApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Vector carList = new Vector();

            //IList<Car> carList = new List<Car>();
            /* Adding some data to the list. */
            carList.Plot(new Car() { refno = "car#1", xaxis = 0, yaxis = 2.018, zaxis = 30 });

            carList.Plot(new Car() { refno = "car#2", xaxis = 0, yaxis = 2.018, zaxis = 25 });
            carList.Plot(new Car() { refno = "car#3", xaxis = 0, yaxis = 2.018, zaxis = 50 });
            carList.Plot(new Car() { refno = "car#4", xaxis = 1, yaxis = 2.018, zaxis = 40 });
            carList.Plot(new Car() { refno = "car#5", xaxis = 10, yaxis = 2.015, zaxis = 30 });
            carList.Plot(new Car() { refno = "car#6", xaxis = 30, yaxis = 2.016, zaxis = 25 });
            carList.Plot(new Car() { refno = "car#7", xaxis = 0, yaxis = 2.017, zaxis = 50 });
            carList.Plot(new Car() { refno = "car#8", xaxis = 70, yaxis = 2.010, zaxis = 10 });
            carList.Plot(new Car() { refno = "car#9", xaxis = 0, yaxis = 2.018, zaxis = 22 });
            carList.Plot(new Car() { refno = "car#10", xaxis = 5, yaxis = 2.017, zaxis = 45 });
            carList.Plot(new Car() { refno = "car#11", xaxis = 0, yaxis = 2.018, zaxis = 35 });
            carList.Plot(new Car() { refno = "car#12", xaxis = 2, yaxis = 2.017, zaxis = 40 });
            carList.Plot(new Car() { refno = "car#13", xaxis = 10, yaxis = 2.015, zaxis = 21 });
            carList.Plot(new Car() { refno = "car#14", xaxis = 8, yaxis = 2.016, zaxis = 25 });
            carList.Plot(new Car() { refno = "car#15", xaxis = 3, yaxis = 2.017, zaxis = 35 });
            carList.Plot(new Car() { refno = "car#16", xaxis = 60, yaxis = 2.011, zaxis = 11 });
            carList.Plot(new Car() { refno = "car#17", xaxis = 60, yaxis = 2.014, zaxis = 11 });
            carList.Plot(new Car() { refno = "car#18", xaxis = 35, yaxis = 2.016, zaxis = 22 });
            carList.Plot(new Car() { refno = "car#19", xaxis = 0, yaxis = 2.017, zaxis = 45 });
            carList.Plot(new Car() { refno = "car#20", xaxis = 20, yaxis = 2.013, zaxis = 20 });

            //Graph gp = new Graph(In: carList);
            
            //gp.DataList();

            ////// Un comment the below line of code to view all proximities from a perticular point of reference to all other vector points.
            //// gp.ListAllProximities("car#4");

            //gp.ListProximities("car#4", 10);
            //gp.ListProximities("car#8", 10);
            //gp.ListProximities("car#13", 10);
            
            Console.Read();
        }
    }

    /// <summary>
    /// Entity class for cars.
    /// </summary>
    public class Car : IVector 
    {
        public string refno { get; set; }

        private double xaxisP;
        private double yaxisP;
        private double zaxisP;

        public double xaxis
        {
            get { return xaxisP; }
            set { 
                xaxisP = value;
                coordinateP[0] = value;
            }
        }
        public double yaxis
        {
            get { return yaxisP; }
            set
            {
                yaxisP = value;
                coordinateP[1] = value;
            }
        }
        public double zaxis
        {
            get { return zaxisP; }
            set
            {
                zaxisP = value;
                coordinateP[2] = value;
            }
        }
        
        private double[] coordinateP = new double[3];
        public double[] coordinate { get { return coordinateP; } set { coordinateP = value; } }
    }

}
