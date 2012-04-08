using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EVERouteFinder
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Application.Run(new MainMenu());
        }
    }

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        //EVEDBoperations testOperations = new EVEDBoperations();

    //        //testOperations.startEVEDBConnection();
    //        //testOperations.openEVEDBConnection();
    //        //testOperations.setEVEDBQuery("select fromSolarSystemID, toSolarSystemID from dbo.mapSolarSystemJumps");


    //        //double mindistance = 0;
    //        //double maxdistance = 0;
    //        //List<int> fromSID = new List<int>();
    //        //List<int> toSID = new List<int>();
    //        //while (testOperations.eveDBQueryRead())
    //        //{
    //        //    fromSID.Add((int)testOperations.eveDBReader[0]);
    //        //    toSID.Add((int)testOperations.eveDBReader[1]);
    //        //}
    //        //testOperations.eveDBQueryClose();
    //        //for (int i = 0; i < fromSID.Count; i++)
    //        //{
    //        //    testOperations.setEVEDBQuery(testOperations.premadeQuery_getDistanceBetweenSolarSystems(fromSID[i], toSID[i]));
    //        //    testOperations.eveDBQueryRead();
    //        //    if (mindistance > (double)testOperations.eveDBReader[0] || mindistance == 0)
    //        //    {
    //        //        mindistance = (double)testOperations.eveDBReader[0];
    //        //    }
    //        //    if (maxdistance < (double)testOperations.eveDBReader[0])
    //        //    {
    //        //        maxdistance = (double)testOperations.eveDBReader[0];
    //        //    }
    //        //    testOperations.eveDBQueryClose();
    //        //}

    //        //Console.WriteLine(mindistance);
    //        //Console.WriteLine(maxdistance);
    //        //Console.WriteLine(maxdistance/mindistance);
    //        //testOperations.closeEVEDBConnection();

    //        Console.ReadKey();

    //    }
}

//Code samples


//EVEDBSettings programSettings = new EVEDBSettings();
//XmlOperations xmlSettingsSerializer = new XmlOperations();
////Type.GetType("EVERouteFinder.Settings.EVEDBSettings");
//xmlSettingsSerializer.Save(programSettings, @"C:\Users\Greitone\Documents\visual studio 2010\Projects\EVERouteFinder\EVERouteFinder\Settings.xml", "EVERouteFinder.Settings.EVEDBSettings");
//programSettings = null;
//programSettings = (EVEDBSettings)xmlSettingsSerializer.Load(@"C:\Users\Greitone\Documents\visual studio 2010\Projects\EVERouteFinder\EVERouteFinder\Settings.xml", "EVERouteFinder.Settings.EVEDBSettings");
//Console.WriteLine(programSettings.DBConnectionString);


//for (int i = 0; i < fromSID.Count; i++)
//{
//    testOperations.setEVEDBQuery(testOperations.premadeQuery_getDistanceBetweenSolarSystems(fromSID[i], toSID[i]));
//    testOperations.eveDBQueryRead();
//    Console.WriteLine("TIMES: " + (double)testOperations.eveDBReader[0] / mindistance);
//    testOperations.eveDBQueryClose();
//    if (i % 15 == 0)
//    {
//        Console.ReadKey();
//    }
//}