using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using EVERouteFinder.Classes;

namespace EVERouteFinder.Settings
{
    public static class SEVEDBSettings
    {
        public static string DBConnectionString = string.Empty;
        private static string SettingsPath = @"C:\Users\Greitone\Documents\visual studio 2010\Projects\EVERouteFinder\EVERouteFinder\Settings.xml";

        //public static void SettingsLoad()
        //{
        //    if (DBConnectionString == string.Empty)
        //    {
        //        XmlOperations xmlSettingsSerializer = new XmlOperations();
        //        DBConnectionString = ((EVEDBSettings)xmlSettingsSerializer.Load(SettingsPath, "EVERouteFinder.Settings.EVEDBSettings")).DBConnectionString;
        //    }
        //}
        public static double factor = 35; // 9.5;
        public static double avgFactor = 35;
        private static double avgFactorsum = 35;
        public static void addAvg()
        {
            times++;
            avgFactorsum += factor;
            avgFactor = avgFactorsum / times;
        }
        private static int times = 1;
    }

    //[XmlRoot("EVEDBSettings")]
    //public class EVEDBSettings
    //{
    //    [XmlAttribute("DBConnectionString")]
    //    public string DBConnectionString = string.Empty;// = "user id=greitone;" + "password=leochase23; server=Greitone-PC;" + "Trusted_Connection=yes;" + "database=EVEDB; " + "connection timeout=30";

    //    private string SettingsPath = @"C:\Users\Greitone\Documents\visual studio 2010\Projects\EVERouteFinder\EVERouteFinder\Settings.xml";

    //    public EVEDBSettings SettingsLoad()
    //    {
    //        if (DBConnectionString == string.Empty)
    //        {
    //            XmlOperations xmlSettingsSerializer = new XmlOperations();
    //            return (EVEDBSettings)xmlSettingsSerializer.Load(SettingsPath, "EVERouteFinder.Settings.EVEDBSettings");
    //        }
    //        else
    //        {
    //            return this;
    //        }
    //    }

    //    public void SettingsSave()
    //    {
    //        XmlOperations xmlSettingsSerializer = new XmlOperations();
    //        xmlSettingsSerializer.Save(this, SettingsPath, "EVERouteFinder.Settings.EVEDBSettings");
    //    }

    //}
}
