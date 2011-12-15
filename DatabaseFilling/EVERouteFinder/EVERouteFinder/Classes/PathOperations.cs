using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EVERouteFinder.Classes
{
    public class PathOperations
    {
        public PathOperations(Node start, Node goal)
        {
            this.start = start;
            this.goal = goal;
            this.avoidanceList= new List<Node>();
            this.closedset = new List<Node>();
            this.openset = new List<Node>();
        }

        public PathOperations(Node start, Node goal, List<Node> avoidancelist)
        {
            this.start = start;
            this.goal = goal;
            this.avoidanceList = avoidancelist;
            this.closedset = new List<Node>();
            this.openset = new List<Node>();
        }

        public PathOperations(Node start, Node goal, bool safeRoute)
        {
            //this.start = start;
            //this.goal = goal;
            //this.avoidanceList = avoidancelist;
            //this.closedset = new List<Node>();
            //this.openset = new List<Node>();
        }

        public Node start { get; set; }
        public Node goal { get; set; }
        public List<Node> avoidanceList { get; set; }
        List<Node> closedset;
        List<Node> openset;
        public bool nofactor = true;
        private DateTime timestamp;

        public List<Node> Evaluate()
        {
            timestamp = DateTime.Now;
            this.start.goal = this.goal;
            this.openset.Add(this.start);
            //nodeOset(this, new nodeInOpenSetEventArgs(this.start, true));
            //this.openset[this.openset.IndexOf(this.start)].g_score = 0;
            this.searchbyID(openset, this.start.ID).g_score = 0;
            //this.openset[this.openset.IndexOf(this.start)].h_score = this.heuristicFunction(this.start, this.goal);
            //this.searchbyID(openset, this.start.ID).h_score = 0;
            while (this.openset.Count > 0)
            {
                Node x = this.getLowestFscoreNode(openset);
                if (x.ID == this.goal.ID)
                {
                    return this.ReconstructPath(x.camefrom, x);
                }
                else
                {
                    this.closedset.Add(x);
                    //nodeCset(this, new nodeInClosedSetEventArgs(x, true));
                    this.openset.Remove(x);
                    //nodeOset(this, new nodeInOpenSetEventArgs(x, false));
                    //Parallel.ForEach(getSolarSystems(), po, n1 =>
                    //{
                    //    loop(n, n1);
                    //}
                    //);
                    foreach (Node y in x.getNeighborNodes())
                    {
                        if (this.searchbyID(avoidanceList, y.ID) == null)
                        {
                            y.goal = this.goal;
                            y.nofactor = this.nofactor;
                            double tentative_g_score = x.g_score + 1; //el 1 se sustituiria por dist(x,y) aplicable segun el caso
                            bool tentative_is_better = false;

                            if (this.searchbyID(this.closedset, y.ID) != null)
                            {
                                y.g_score = this.searchbyID(this.closedset, y.ID).g_score;
                            }
                            else
                            {
                                if (this.searchbyID(this.openset, y.ID) == null)
                                {
                                    this.openset.Add(y);
                                    //nodeOset(this, new nodeInOpenSetEventArgs(y, true));
                                    tentative_is_better = true;
                                }
                            }
                            if (tentative_g_score < y.g_score)
                            {
                                tentative_is_better = true;
                            }
                            else
                            {
                                tentative_is_better = false;
                            }
                            if (tentative_is_better)
                            {
                                y.camefrom = x;
                                y.g_score = tentative_g_score;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private Node searchbyID(List<Node> nodeList, int id)
        {
            return nodeList.Find
            (
            delegate(Node n)
            {
                return n.ID == id;
            }
            );
//          return nodeList.Find(delegate(Node n){return n.ID == id;});
        }

        double heuristicFunction(Node from, Node to)
        {
            double a = 0;
            return a;
        }

        Node getLowestFscoreNode(List<Node> nodelist)
        {
            int lowestindex = 0;
            double lowestFscore = double.MaxValue;
            Parallel.For(0, nodelist.Count, i =>
            {
                if (nodelist[i].f_score < lowestFscore)
                {
                    lowestindex = i;
                    lowestFscore = nodelist[i].f_score;
                }
            });
            return nodelist[lowestindex];
        }

        List<Node> ReconstructPath(Node cameFrom, Node currentNode)
        {
            TimeSpan t = timestamp.Subtract(DateTime.Now);
            List<Node> Path = new List<Node>();
            if (cameFrom.camefrom != null)
            {
                Path = ReconstructPath(cameFrom.camefrom, cameFrom);
                Path.Add(currentNode);
                return Path;
            }
            else
            {
                Path.Add(cameFrom);
                Path.Add(currentNode);
                return Path;
            }
        }

        public event nodeInOpenSet nodeOset;

        public event nodeInClosedSet nodeCset;
    }


    public delegate void nodeInOpenSet(object sender, nodeInOpenSetEventArgs e);
    public delegate void nodeInClosedSet(object sender, nodeInClosedSetEventArgs e);
    
    public class nodeInOpenSetEventArgs : EventArgs
    {
        public nodeInOpenSetEventArgs(Node node, bool isadded)
        {
            this.opensetNode = node;
            this.isAdded = isadded;
            this.eventTime = DateTime.Now;
        }
        public Node opensetNode { get; set; }
        public bool isAdded { get; set; }
        public DateTime eventTime { get; set; }
    }

    public class nodeInClosedSetEventArgs : EventArgs
    {
        public nodeInClosedSetEventArgs(Node node, bool isadded)
        {
            this.closedsetNode = node;
            this.isAdded = isadded;
            this.eventTime = DateTime.Now;
        }
        public Node closedsetNode { get; set; }
        public bool isAdded { get; set; }
        public DateTime eventTime { get; set; }
    }

}
