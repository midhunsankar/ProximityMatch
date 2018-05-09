using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProximityMatch;
using System.Data;
using System.IO;


namespace ProximityMatchApp
{
    public class SamplePatient
    {
        public void Run()
        {           
           var Patients = LoadDataSet(2);
           Vector patientList = new Vector(dimension: 2);

           patientList.Plot(vectorList: Patients);

           ConsoleKey Key;
            do
            {
                Random rnd = new Random();
                Patient patient = Patients[rnd.Next(0, Patients.Count)] as Patient;

                Console.WriteLine("\nstate = {0} , professional = {1}% , communicated = {2}% coordinate = ({1}, {2})",
                                       patient.state, patient.professional, patient.communicated);
                
                Console.WriteLine("\nTrying to find the closest states to above sample where, patients who reported that their home health team communicated well with them and gave care in a professional way."); 

                Console.WriteLine("\n*******************************************************************************************\n");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                patientList.take = 5;
                var nodes = patientList.Nearest(patient);
                if (nodes.Count > 0)
                {
                    foreach (Patient dat in nodes)
                    {
                        Console.WriteLine("\nstate = {0} , professional = {1}% , communicated = {2}% coordinate = ({1}, {2}) distance = {3}",
                              dat.state, dat.professional, dat.communicated, dat._distance);
                    }
                }
                else
                {
                    Console.WriteLine("\nNo Match Found!!");
                }
                sw.Stop();

                Console.WriteLine("\nSearching finished!! Time: {0:000000} ms Cycles: {1} ticks\n", sw.Elapsed.TotalMilliseconds, sw.Elapsed.Ticks);
                Console.WriteLine("\nDo you want to continue (y/n)?");
                Key = Console.ReadKey(true).Key;

            } while (Key == ConsoleKey.Y);


        }

        public IList<IVector> LoadDataSet(int dimention = 2)
        {
            IList<IVector> dataSet = new List<IVector>();
            var table = ReadCsvFile("Home_Health_Care_-_Patient_survey__HHCAHPS__State_Data.csv");
            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    var patient = new Patient();
                        patient.state = !string.IsNullOrEmpty(Convert.ToString(row[0])) ? Convert.ToString(row[0]) : null;
                        patient.professional = !string.IsNullOrEmpty(Convert.ToString(row[1])) ? int.Parse(Convert.ToString(row[1]).Trim('%')) : 0;
                        patient.communicated = !string.IsNullOrEmpty(Convert.ToString(row[2])) ? int.Parse(Convert.ToString(row[2]).Trim('%')) : 0;
                        patient.discussed = !string.IsNullOrEmpty(Convert.ToString(row[3])) ? int.Parse(Convert.ToString(row[3]).Trim('%')) : 0;
                        patient.rating = !string.IsNullOrEmpty(Convert.ToString(row[4])) ? int.Parse(Convert.ToString(row[4]).Trim('%')) : 0;
                        patient.recommend = !string.IsNullOrEmpty(Convert.ToString(row[5])) ? int.Parse(Convert.ToString(row[5]).Trim('%')) : 0;
                        patient.surveys = !string.IsNullOrEmpty(Convert.ToString(row[6])) ? int.Parse(Convert.ToString(row[6]).Trim('%')) : 0;
                        patient.rate = !string.IsNullOrEmpty(Convert.ToString(row[7])) ? int.Parse(Convert.ToString(row[7]).Trim('%')) : 0;

                        if (dimention == 2)
                        {
                            patient.setCoordinates(new double[2] { patient.professional, patient.communicated });
                        }
                        else if (dimention == 3)
                        {
                            patient.setCoordinates(new double[3] { patient.professional, patient.communicated, patient.discussed });
                        }

                    dataSet.Add(patient);
                }
            }
            return dataSet;
        }

        public DataTable ReadCsvFile(string fileName)
        {

            DataTable dtCsv = new DataTable();
            string Fulltext;
            if (!string.IsNullOrEmpty(fileName))
            {
                string FileSaveWithPath = string.Format("{0}\\Data\\{1}",Directory.GetCurrentDirectory(), fileName);
                using (StreamReader sr = new StreamReader(FileSaveWithPath))
                {
                    while (!sr.EndOfStream)
                    {
                        Fulltext = sr.ReadToEnd().ToString(); //read full file text  
                        string[] rows = Fulltext.Split('\n'); //split full file text into rows  
                        for (int i = 0; i < rows.Count() - 1; i++)
                        {
                            string[] rowValues = rows[i].Split(','); //split each row with comma to get individual values  
                            {
                                if (i == 0)
                                {
                                    for (int j = 0; j < rowValues.Count(); j++)
                                    {
                                        dtCsv.Columns.Add(rowValues[j]); //add headers  
                                    }
                                }
                                else
                                {
                                    DataRow dr = dtCsv.NewRow();
                                    for (int k = 0; k < rowValues.Count(); k++)
                                    {
                                        dr[k] = rowValues[k].ToString();
                                    }
                                    dtCsv.Rows.Add(dr); //add other rows  
                                }
                            }
                        }
                    }
                }
            }
            return dtCsv;
        } 
    }

    public class Patient : IVector
    {
        public long uniqueId { get; set; }

        private double[] coordinateP = new double[2];

        public double[] coordinate
        {
            get { return coordinateP; }
            set { coordinateP = value; }
        }
                
        public double _distance { get; set; }

        public string state { get; set; }

        //Percent of patients who reported that their home health team gave care in a professional way
        public int professional { get; set; }

        //Percent of patients who reported that their home health team communicated well with them
        public int communicated { get; set; }

        //Percent of patients who reported that their home health team discussed medicines, pain, and home safety with them
        public int discussed { get; set; }

        //Percent of patients who gave their home health agency a rating of 9 or 10 on a scale from 0 (lowest) to 10 (highest),
        public int rating { get; set; }

        //Percent of patients who reported YES, they would definitely recommend the home health agency to friends and family
        public int recommend { get; set; }

        //Number of completed Surveys,
        public int surveys { get; set; }
        
        //Response rate
        public int rate { get; set; }

        public void setCoordinates(double[] co)
        {
            coordinateP = co;
        }

    }

}
