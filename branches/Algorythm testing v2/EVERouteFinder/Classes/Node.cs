using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using EVERouteFinder.Settings;

namespace EVERouteFinder.Classes
{
    [Serializable]
    public class Node
    {
        //solarSystemName, x, y, z, solarSystemID, constellationID, regionID, security, id
        private double if_score;
        [XmlArray] public List<Node> neighborNodes;
        public bool nofactor;
        public bool notnull;

        public Node()
        {
        }

        public Node(Node n, bool isNeighbor)
        {
            ID = n.ID;
            Name = n.Name;
            X = n.X;
            Y = n.Y;
            Z = n.Z;
            System = n.System;
            Constellation = n.Constellation;
            Region = n.Region;
            Security = n.Security;
            g_score = n.g_score;
            //if (!isNeighbor)
            //{
            //    fillNeighborList(n);
            //}
        }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public int ID { get; set; }

        [XmlElement]
        public double X { get; set; }

        [XmlElement]
        public double Y { get; set; }

        [XmlElement]
        public double Z { get; set; }

        [XmlElement]
        public int System { get; set; }

        [XmlElement]
        public int Constellation { get; set; }

        [XmlElement]
        public int Region { get; set; }

        [XmlElement]
        public double Security { get; set; }

        [XmlElement]
        public double g_score { get; set; }

        //public double ih_score;
        //public double h_score { get { return h_scoreF(); } set { this.ih_score = value; } }

        public double f_score
        {
            get { return f_scoreF(); }
            set { if_score = value; }
        }

        public Node camefrom { get; set; }
        public Node goal { get; set; }

        //private void fillNeighborList(Node n)
        //{
        //    this.neighborNodes = new List<Node>();
        //    foreach (Node n1 in n.neighborNodes)
        //    {
        //        this.neighborNodes.Add(new Node(n1, true));
        //    }
        //}

        //public List<Node> getNeighborNodes(Node n)
        //{
        //    if (this.neighborNodes == null)
        //    {
        //        fillNeighborList(n);
        //    }
        //    else if (this.neighborNodes.Count() == 0)
        //    {
        //        fillNeighborList(n);
        //    }
        //    return this.neighborNodes;
        //}

        private double hFunction(Node node, Node goal)
        {
            //1.0637 E+15 / 324.09 = Max distance between nodes
            double minDist;
            double maxDist;
            minDist = double.Parse("1063700000000000");
            maxDist = minDist*324.09;
            double distance;
            distance =
                Math.Sqrt(Math.Pow(node.X - goal.X, 2) + Math.Pow(node.Y - goal.Y, 2) + Math.Pow(node.Z - goal.Z, 2));
            return SEVEDBSettings.factor*distance/maxDist;
            //last stable factor found at 7 (more than 95% accuracy) 
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
            //double minDist;
            //double maxDist;
            //double.TryParse("1063700000000000", out minDist);
            //maxDist = minDist * 324.09;
            //double distance = double.MaxValue;
            //distance = Math.Sqrt(Math.Pow(node.X - goal.X, 2) + Math.Pow(node.Y - goal.Y, 2) + Math.Pow(node.Z - goal.Z, 2));
            //return 1 * distance / maxDist;
            return 0;
        }

        private double f_scoreF()
        {
            if_score = g_score + h_scoreF();
            return if_score;
        }


        private double h_scoreF()
        {
            double result = 0;
            //if (goal != null && this.nofactor == false)
            //{
            result = hFunction(this, goal);
            //}
            //else if (goal != null && this.nofactor == true)
            //{
            //    result = hFunctionNoFactor(this, goal);
            //}
            return result;
        }
    }
}