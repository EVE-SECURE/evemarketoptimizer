﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace EVERouteFinder.Classes
{
    public class Node
    {
        //solarSystemName, x, y, z, solarSystemID, constellationID, regionID, security, id
        public Node()
        {
        }

        public Node(string queryData)
        {
            string[] separateString = queryData.Split(',');
            this.Name = separateString[0];
            this.X = Convert.ToDouble(separateString[1]);
            this.Y = Convert.ToDouble(separateString[2]);
            this.Z = Convert.ToDouble(separateString[3]);
            this.System = Convert.ToInt32(separateString[4]);
            this.Constellation = Convert.ToInt32(separateString[5]);
            this.Region = Convert.ToInt32(separateString[6]);
            this.Security = Convert.ToDouble(separateString[7]);
            this.ID = Convert.ToInt32(separateString[8]);
            this.g_score = double.MaxValue;
        }
        public Node(string name, int id, double x, double y, double z, int system, int constellation, int region, double security)
        {
            this.Name = name;
            this.ID = id;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.System = system;
            this.Constellation = constellation;
            this.Region = region;
            this.Security = security;
            this.g_score = double.MaxValue;
            this.notnull = true;
        }
        public Node(int id)
        {
            //if (QueriesCache.systems.ContainsKey(id))
            //{
            //    Node n = new Node(QueriesCache.systems[id]);
            //    this.Name = n.Name;
            //    this.ID = n.ID;
            //    this.X = n.X;
            //    this.Y = n.Y;
            //    this.Z = n.Z;
            //    this.System = n.System;
            //    this.Constellation = n.Constellation;
            //    this.Region = n.Region;
            //    this.Security = n.Security;
            //    this.g_score = n.g_score;
            //}
            //else
            //{       
                //solarSystemName, x, y, z, solarSystemID, constellationID, regionID, security, id
                string nodeData = "";
                EVEDBoperations nodeOperations = new EVEDBoperations();
                nodeOperations.startEVEDBConnection();
                nodeOperations.openEVEDBConnection();
                nodeOperations.setEVEDBQuery(nodeOperations.premadeQuery_getSolarSystemNode(id));
                nodeOperations.eveDBQueryRead();
                this.Name = nodeOperations.eveDBReader[0].ToString();
                nodeData += this.Name + ",";
                this.X = (double)nodeOperations.eveDBReader[1];
                nodeData += this.X + ",";
                this.Y = (double)nodeOperations.eveDBReader[2];
                nodeData += this.Y + ",";
                this.Z = (double)nodeOperations.eveDBReader[3];
                nodeData += this.Z + ",";
                this.System = (int)nodeOperations.eveDBReader[4];
                nodeData += this.System + ",";
                this.Constellation = (int)nodeOperations.eveDBReader[5];
                nodeData += this.Constellation + ",";
                this.Region = (int)nodeOperations.eveDBReader[6];
                nodeData += this.Region + ",";
                this.Security = (double)nodeOperations.eveDBReader[7];
                nodeData += this.Security + ",";
                this.ID = id;
                nodeData += this.ID;
                //QueriesCache.systems.Add(id, nodeData);
                this.g_score = double.MaxValue;
                nodeOperations.eveDBQueryClose();
                nodeOperations.closeEVEDBConnection();
            //}
        }
        //            return "select solarSystemName, x, y, z, solarSystemID, constellationID, regionID, security from dbo.mapSolarSystems where solarSystemID = " + sID.ToString();
        
        [XmlAttribute()]
        public string Name { get; set; }
        [XmlAttribute()]
        public int ID { get; set; }
        [XmlElement()]
        public double X { get; set; }
        [XmlElement()]
        public double Y { get; set; }
        [XmlElement()]
        public double Z { get; set; }
        [XmlElement()]
        public int System { get; set; }
        [XmlElement()]
        public int Constellation { get; set; }
        [XmlElement()]
        public int Region { get; set; }
        [XmlElement()]
        public double Security { get; set; }
        [XmlElement()]
        public double g_score { get; set; }
        //public double ih_score;
        //public double h_score { get { return h_scoreF(); } set { this.ih_score = value; } }
        private double if_score;
        public double f_score { get { return this.f_scoreF(); } set { if_score = value;} }
        public Node camefrom { get; set; }
        public Node goal { get; set; }
        public bool notnull = false;
        public bool nofactor = false;
        [XmlArray()]
        public List<Node> neighborNodes = null;

        public List<Node> getNeighborNodes()
        {
            if (this.neighborNodes == null)
            {
                    EVEDBoperations nodeOperations = new EVEDBoperations();
                    nodeOperations.startEVEDBConnection();
                    nodeOperations.openEVEDBConnection();
                    nodeOperations.setEVEDBQuery(nodeOperations.premadeQuery_getAdjacentSolarSystems(this.ID));
                    this.neighborNodes = new List<Node>();
                    while (nodeOperations.eveDBQueryRead())
                    {
                        int id = (int)nodeOperations.eveDBReader[0];

                            this.neighborNodes.Add(new Node(id));
                    }
                    nodeOperations.eveDBQueryClose();
                    nodeOperations.closeEVEDBConnection();
                
            }
            return this.neighborNodes;
        }

        public void resetNeighborNodes()
        {
            this.neighborNodes = null;
        }

        private double hFunction(Node node, Node goal)
        {
            //1.0637 E+15 / 324.09 = Max distance between nodes
            double minDist;
            double maxDist;
            double.TryParse("1063700000000000", out minDist);
            maxDist = minDist * 324.09;
            double distance = double.MaxValue;
            distance = Math.Sqrt(Math.Pow(node.X - goal.X, 2) + Math.Pow(node.Y - goal.Y, 2) + Math.Pow(node.Z - goal.Z, 2));
            return Settings.SEVEDBSettings.factor * distance / maxDist; 
            //last stable factor found at 3,89000000000012 (more than 95% accuracy) 
            //but factors of up to 15-17 are quite accurate (maybe around 85-90% accuracy
            //and only miss by 1-2 jumps) but quite faster. If calculation times for the whole
            //algorythm are a problem accuracy can be lowered for the sake of execution times (since
            //the only 100% accurate is factor=1) route finding times for long routes (60+ nodes) are halved with factor=17 
            //from factor=0 usually long routes get higher accuracy from high factors while shorter routes
            //benefit from lower or 1 factor (and their execution time is pretty low anyway
        }

        private double hFunctionNoFactor(Node node, Node goal)
        {
            //1.0637 E+15 / 324.09 = Max distance between nodes
            double minDist;
            double maxDist;
            double.TryParse("1063700000000000", out minDist);
            maxDist = minDist * 324.09;
            double distance = double.MaxValue;
            distance = Math.Sqrt(Math.Pow(node.X - goal.X, 2) + Math.Pow(node.Y - goal.Y, 2) + Math.Pow(node.Z - goal.Z, 2));
            return 0 * distance / maxDist;
        }
        
        private double f_scoreF()
        {
            if (this.if_score == 0)
            {
                this.if_score = g_score + h_scoreF();
            }
            return this.if_score;
        }


        private double h_scoreF()
        {
            double result = 0;
            if (goal != null && this.nofactor == false)
            {
                result = hFunction(this, goal);
            }
            else if (goal != null && this.nofactor == true)
            {
                result = hFunctionNoFactor(this, goal);
            }
            return result;
        }

    }

}
