using System;
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

        public Node(Node n, bool isNeighbor)
        {
            this.ID = n.ID;
            this.Name = n.Name;
            this.X = n.X;
            this.Y = n.Y;
            this.Z = n.Z;
            this.System = n.System;
            this.Constellation = n.Constellation;
            this.Region = n.Region;
            this.Security = n.Security;
            this.g_score = n.g_score;
            if (!isNeighbor)
            {
                fillNeighborList(n);
            }
        }
        
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
        public bool nofactor;
        [XmlArray()]
        public List<Node> neighborNodes = null;

        private void fillNeighborList(Node n)
        {
            this.neighborNodes = new List<Node>();
            foreach (Node n1 in n.neighborNodes)
            {
                this.neighborNodes.Add(new Node(n1, true));
            }
        }

        public List<Node> getNeighborNodes(Node n)
        {
            if (this.neighborNodes == null)
            {
                fillNeighborList(n);
            }
            else if (this.neighborNodes.Count() == 0)
            {
                fillNeighborList(n);
            }
            return this.neighborNodes;
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
            return 1 * distance / maxDist;
        }
        
        private double f_scoreF()
        {
            this.if_score = g_score + h_scoreF();
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
