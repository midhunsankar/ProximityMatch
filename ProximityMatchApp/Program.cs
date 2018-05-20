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


namespace ProximityMatchApp
{
    class Program
    {
        static void Main(string[] args)
        {

            //SampleCars _carsObj = new SampleCars();
            //_carsObj.Run();

            //SamplePatient _patientObj = new SamplePatient();
            //_patientObj.Run();

            SampleFoodStatistics _foodObj = new SampleFoodStatistics();
            _foodObj.Run();
            
        }        
    }



}
