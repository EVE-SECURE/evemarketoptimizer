using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using EVERouteFinder.Classes;
using System.IO;

namespace EVERouteFinder
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        string openset = string.Empty;
        string closedset = string.Empty;

        private void button1_Click(object sender, EventArgs e)
        {
        }

        //        PathOperations pop = new PathOperations(new Node(30004659), new Node(30000647));
        //        List<Node> route = new List<Node>();

        //        route = pop.Evaluate();
        //        string systems = "";
        //        int i = 0;
        //        foreach (Node n in route)
        //        {
        //            systems += n.Name + ",";
        //            Console.WriteLine(n.Name);
        //            i++;
        //        }
        //        Console.WriteLine(i);
        private void openListEvent(object sender, nodeInOpenSetEventArgs e)
        {
            this.openset += "Openlist: " + e.eventTime.Second + "." + e.eventTime.Millisecond + " " + e.isAdded.ToString() + " " + e.opensetNode.Name + "; ";
            //this.Refresh();
        }
        private void closedListEvent(object sender, nodeInClosedSetEventArgs e)
        {
            this.closedset += "ClosedList: " + e.eventTime.ToLongTimeString() + e.isAdded.ToString() + e.closedsetNode.Name + "\r\n";
            //this.Refresh();
        }

        private List<Node> getSolarSystems()
        {
            EVEDBoperations nodeOperations = new EVEDBoperations();
            nodeOperations.startEVEDBConnection();
            nodeOperations.openEVEDBConnection();
            nodeOperations.setEVEDBQuery(nodeOperations.premadeQuery_getSolarSystemsList());
            List<Node> solarSystems = new List<Node>();
            while (nodeOperations.eveDBQueryRead())
            {
                solarSystems.Add(new Node((int)nodeOperations.eveDBReader[0]));
            }
            nodeOperations.eveDBQueryClose();
            nodeOperations.closeEVEDBConnection();
            return solarSystems;
        }

        private void loopNodes()
        {
            //int routes = 0;
            //foreach(Node n in getSolarSystems())
            //{
            //    Parallel.ForEach(getSolarSystems(), n1 =>
            //        {
            //            loop(n, n1);
            //        }
            //    );
            //}
            List<Node> myList = getSolarSystems();
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = Environment.ProcessorCount;
            Parallel.ForEach(myList, po, n =>
            {
                List<Node> mylist2 = getSolarSystems();
                //Parallel.ForEach(mylist2.AsParallel(), po2, n1 =>
                //{
                //    loop(n, n1);
                //}
                //);
                foreach (Node n1 in mylist2)
                {
                    loop(n, n1);
                }
            }
            );

            this.textBoxResult.Text = Settings.SEVEDBSettings.factor.ToString();
        }

        private void loop(Node n, Node n1)
        {
                    if(n.ID != n1.ID)
                    {
                        int tid = Thread.CurrentThread.ManagedThreadId;
                        PathOperations pop = new PathOperations(n, n1);
                        pop.nofactor = false;
                        pop.start.nofactor = false;
                        pop.goal.nofactor = false;
                        List<Node> route = new List<Node>();

                        route = pop.Evaluate();
                        string systems = "";
                        int i = 0;
                        foreach (Node n2 in route)
                        {
                            systems += "System: " + " " + n2.Name + " " + n2.Security.ToString() + " " + n2.Region.ToString() + " " +  "\r\n"; //n2.f_score.ToString() +
                            i++;
                        }
                        //routes++;
                        pop = new PathOperations(n, n1);
                        pop.nofactor = true;
                        pop.start.nofactor = true;
                        pop.goal.nofactor = true;
                        route = new List<Node>();

                        route = pop.Evaluate();
                        string systems1 = "";
                        int a = 0;
                        foreach (Node n2 in route)
                        {
                            systems1 += "System: " + " " + n2.Name + " " + n2.Security.ToString() + " " + n2.Region.ToString() + " " + "\r\n"; //n2.f_score.ToString() +
                            a++;
                        }
                        //routes++;
                        //this.Text = routes.ToString();
                        if (systems != systems1)
                        {
                            SetText(n.Name + " " + n1.Name + " " + "Not qualified " + a.ToString() + ", " + (i - a).ToString() + "\r\n", 2);
                            if (i - a > 0)
                            {
                                Settings.SEVEDBSettings.factor -= 0.01;
                                SetText(Settings.SEVEDBSettings.factor.ToString() + " /// " + DateTime.Now.ToLongTimeString() + "\r\n", 3);
                            }
                        }
                        else
                        {
                            SetText(n.Name + " " + n1.Name + " " + "Qualified " + a.ToString() + "\r\n", 1);
                        }
                    }
        }

        private void SetText(string text, int thread)
        {
            if (this.textBox2.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text, thread });
            }
            else
            {
                switch (thread % 3)
                {
                    case 0:
                        this.textBoxResult.AppendText(text);
                        this.textBoxResult.Focus();
                        break;
                    case 1:
                        this.textBox2.AppendText(text);
                        this.textBox2.Focus();
                        break;
                    case 2:
                        this.textBox1.AppendText(text);
                        this.textBox1.Focus();
                        break;
                }
            }
        }

        delegate void SetTextCallback(string text, int thread);

        delegate void StartDoingWork();

        private void searchFactor()
        {
            StartDoingWork s = new StartDoingWork(loopNodes);
            Thread myNewThread = new Thread(s.Invoke);
            myNewThread.Start();
            this.button1.Enabled = false;
        }

        private void inputMarketDatabaseDump()
        {
            StartDoingWork s = new StartDoingWork(loopMarketDatabase);
            Thread myNewThread = new Thread(s.Invoke);
            myNewThread.Start();
            this.button1.Enabled = false;
        }

        private void loopMarketDatabase()
        {
            IEnumerable<string> stringList = File.ReadLines(@"C:\Users\Greitone\Downloads\2011-09-05.dump\2011-09-05.dump");
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = Environment.ProcessorCount;
            Parallel.ForEach(stringList, po, n =>
            {
                //n.Replace("\",\"", ",");
                //n.Replace("\"", "");
                //n.Replace('	'.ToString(), ",");
                
                string[] s = n.Split(new string[] {"\",\"", "\t"}, StringSplitOptions.RemoveEmptyEntries);
                for(int i=0; i < s.Count(); i++)
                {
                    s[i] = s[i].Replace("\",\"", string.Empty);
                    s[i] = s[i].Replace('\"', ' ');
                    //s2.Replace("\"", string.Empty);
                }
                EVEOrder o = new EVEOrder(s);
                o.InsertToDB();
            }
            );

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            inputMarketDatabaseDump();
            //IEnumerable<string> ss = 
            //foreach (string s in ss)
            //{
            //    foreach (string s1 in s.Split(new Char[] { '	' }))
            //    {
            //        Console.WriteLine(s1);
            //    }

            //}

        }


    }
}
