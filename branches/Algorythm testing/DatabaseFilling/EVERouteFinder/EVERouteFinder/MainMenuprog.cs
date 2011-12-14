//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using EVERouteFinder.Classes;

//namespace EVERouteFinder
//{
//    public partial class MainMenu : Form
//    {
//        public MainMenu()
//        {
//            InitializeComponent();
//        }

//        string openset = string.Empty;
//        string closedset = string.Empty;

//        private void button1_Click(object sender, EventArgs e)
//        {
//            int from;
//            int to;
//            bool noSkip = false;
//            if (int.TryParse(this.textBoxFrom.Text, out from) && int.TryParse(this.textBoxTo.Text, out to))
//            {
//                noSkip = true;
//                textBoxResult.Text = "";
//            }
//            else 
//            {
//                from = 30004659;
//                to = 30000647;
//                textBoxResult.Text = "\r\n";
//            }
//            PathOperations pop = new PathOperations(new Node(from), new Node(to));
//            pop.nodeOset += new nodeInOpenSet(openListEvent);
//            pop.nodeCset += new nodeInClosedSet(closedListEvent);
//            List<Node> route = new List<Node>();

//            route = pop.Evaluate();
//            string systems = "";
//            int i = 0;
//            foreach (Node n in route)
//            {
//                systems += "System: " + " " + n.Name + " " + n.Security.ToString() +" " + n.Region.ToString() + " " + n.f_score.ToString() + "\r\n";
//                i++;
//            }
//            textBoxResult.Text = systems;
//            textBoxResult.Text += "\r\n" + "Total Jumps: " + (i - 1).ToString();
//            this.textBox1.Text = openset + "\r\n" + closedset;
//        }

//        //        PathOperations pop = new PathOperations(new Node(30004659), new Node(30000647));
//        //        List<Node> route = new List<Node>();

//        //        route = pop.Evaluate();
//        //        string systems = "";
//        //        int i = 0;
//        //        foreach (Node n in route)
//        //        {
//        //            systems += n.Name + ",";
//        //            Console.WriteLine(n.Name);
//        //            i++;
//        //        }
//        //        Console.WriteLine(i);
//        private void openListEvent(object sender, nodeInOpenSetEventArgs e)
//        {
//            this.openset += "Openlist: " + e.eventTime.Second + "." + e.eventTime.Millisecond + " " + e.isAdded.ToString() + " " + e.opensetNode.Name + "; ";
//            //this.Refresh();
//        }
//        private void closedListEvent(object sender, nodeInClosedSetEventArgs e)
//        {
//            this.closedset += "ClosedList: " + e.eventTime.ToLongTimeString() + e.isAdded.ToString() + e.closedsetNode.Name + "\r\n";
//            //this.Refresh();
//        }
//    }
//}
