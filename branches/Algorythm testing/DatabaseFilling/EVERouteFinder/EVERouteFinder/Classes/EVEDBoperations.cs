using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading;

namespace EVERouteFinder.Classes
{
    class EVEDBoperations
    {

#region privateVariables

        private SqlConnection myConnection;
        private SqlCommand myCommand;
        private SqlDataReader myReader;
        
#endregion
        
#region publicMethods

        bool marketDB=false;

        public void startEVEDBConnection()
        {
            SqlConnection myConnection = new SqlConnection("user id=greitone;password=leochase23; server=Greitone-PC;Trusted_Connection=yes;database=EVEDB; connection timeout=120");
            this.myConnection = myConnection;
        }

        public void startEVEDBConnection(bool MarketDB)
        {
            if (MarketDB)
            {
                SqlConnection myConnection = new SqlConnection("user id=greitone;password=leochase23; server=Greitone-PC;Trusted_Connection=yes;database=EVEMarketDB; connection timeout=120");
                this.myConnection = myConnection;
                this.marketDB = MarketDB;
            }
            else
            {
                startEVEDBConnection();
            }
        }

        public void openEVEDBConnection()
        {
            try
            {
                myConnection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while opening the EVEDB Connection");
            }
        }

        public void closeEVEDBConnection()
        {
            myConnection.Close();
        }

        public void setEVEDBQuery(string query)
        {
            SqlCommand myCommand = new SqlCommand(query, this.myConnection);
            this.myCommand = myCommand;
            if (!marketDB || query.ToUpper().StartsWith("SELECT"))
            {
                try
                {
                    SqlDataReader myReader = myCommand.ExecuteReader();
                    this.myReader = myReader;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while Executing the EVEDB command query");
                }
            }
        }

        public bool eveDBQueryRead()
        {
            try
            {
                return this.myReader.Read();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while reading the EVEDB reader");
            }
        }

        public int eveDBExecuteNonQuery()
        {
            try
            {
                return this.myCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //Thread.CurrentThread.Abort();
                //throw new Exception("Error upon executing query");
                return 0;
            }
        }

        public void eveDBQueryClose() 
        {
            this.myReader.Close();
        }

#endregion

#region publicProperties

        public SqlDataReader eveDBReader
        {
            get { return this.myReader; }
        }

        public int eveDBReaderFieldCount
        {
            get { return this.myReader.FieldCount;}
        }

#endregion

#region premadeQueries

        public string premadeQuery_getDistanceBetweenSolarSystems(int fromSID, int toSID)
        {
           return "declare @result float set @result = [dbo].[getDistanceBetweenSolarSystems](" + fromSID.ToString() + "," + toSID.ToString() + ") select @result";
        }

        public string premadeQuery_getAdjacentSolarSystems(int sID)
        {
            return "select * from [dbo].[getJumpsInSystem]("+ sID.ToString() +")";
        }

        public string premadeQuery_getSolarSystemNode(int sID)
        {
            return "select solarSystemName, x, y, z, solarSystemID, constellationID, regionID, security from dbo.mapSolarSystems where solarSystemID = " + sID.ToString();
        }
        //public string Name { get; set; }
        //public int ID { get; set; }
        //public double X { get; set; }
        //public double Y { get; set; }
        //public double Z { get; set; }
        //public int System { get; set; }
        //public int Constellation { get; set; }
        //public int Region { get; set; }
        //public double Security { get; set; };

        public string premadeQuery_getAdjacentStarGates(int gID)
        {
            return "";
        }

        public string premadeQuery_getSolarSystemsList()
        {
            return "select solarSystemID from mapSolarSystems WHERE regionID < 11000001 AND regionID != 10000004 AND regionID != 10000017 AND regionID != 10000019";
        }

        public string premadeQuery_insertToEveMarketData(EVEOrder order)
        {
            string s = "insert into dbo.EVEMarketData values(" + order.OrderID.ToString() + ", " + order.RegionID.ToString() + ", " + order.SystemID.ToString() + ", " + order.StationID.ToString() + ", " + order.TypeID.ToString() + ", " + order.Bid.ToString() + ", " + order.Price.ToString(new System.Globalization.CultureInfo("en-US")) + ", " + order.MinVolume.ToString() + ", " + order.VolRemain.ToString() + ", " + order.VolEnter.ToString() + ", " + "'" + order.Issued.ToString("yyyy/MM/dd hh:mm:ss") + "'" + ", " + order.Duration.TotalDays.ToString().Replace(',', '.') + ", " + "'" + order.Reported.ToString("yyyy/MM/dd hh:mm:ss") + "'" + ");";
            return s;
        }

        public string premadeQuery_getEveOrder(long orderID)
        {
            string s = "SELECT * FROM [EVEMarketDB].[dbo].[EVEMarketData] where orderID = " + orderID.ToString();
            return s;
        }

        public string premadeQuery_UpdateEveOrder(EVEOrder order)
        {
            string s = "update dbo.EVEMarketData set volremain = " + order.VolRemain.ToString() + ", reported = '" + order.Reported.ToString("yyyy/MM/dd hh:mm:ss") + "' where orderID = " + order.OrderID.ToString();
            return s;
        }

#endregion

    }
}
