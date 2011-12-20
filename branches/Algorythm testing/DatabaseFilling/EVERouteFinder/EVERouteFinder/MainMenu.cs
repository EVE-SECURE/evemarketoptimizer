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
using System.Globalization;

namespace EVERouteFinder
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
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
            List<Node> myList = getSolarSystems();
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = Environment.ProcessorCount;
            //workload is around 55% program 35% database. probably could be highly optimized by saving all queried nodes to a single structure and working from there
            Parallel.ForEach(myList, po, n =>
            {
                List<Node> mylist2 = getSolarSystems();
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
            if (n.ID != n1.ID)
            {
                int tid = Thread.CurrentThread.ManagedThreadId;
                n.nofactor = n1.nofactor = false;
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
                    systems += "System: " + " " + n2.Name + " " + n2.Security.ToString() + " " + n2.Region.ToString() + " " + "\r\n"; //n2.f_score.ToString() +
                    i++;
                }
                n.resetNeighborNodes();
                n1.resetNeighborNodes();
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
                if (systems != systems1)
                {
                    SetText(n.Name + " " + n1.Name + " " + "Not qualified " + a.ToString() + ", " + (i - a).ToString() + "\r\n", 1);
                    if (i - a > 0)
                    {
                        if (i - a < 5)
                        {
                            Settings.SEVEDBSettings.factor -= (i - a) * Settings.SEVEDBSettings.factor / 100;
                        }
                        else if (i - a < 10)
                        {
                            Settings.SEVEDBSettings.factor -= 1.2 * (i - a) * Settings.SEVEDBSettings.factor / 100;
                        }
                        else
                        {
                            Settings.SEVEDBSettings.factor -= 2 * (i - a) * Settings.SEVEDBSettings.factor / 100;
                        }
                        Settings.SEVEDBSettings.addAvg();
                        SetText(Settings.SEVEDBSettings.factor.ToString() + "// Avg:" + Settings.SEVEDBSettings.avgFactor.ToString() + " /// Deviation: " + (i - a).ToString() + " // on " + DateTime.Now.ToLongTimeString() + "\r\n", 2);
                    }
                }
                else
                {
                    SetText(n.Name + " " + n1.Name + " " + "Qualified " + a.ToString() + "\r\n", 0);
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
                switch (thread)
                {
                    case 0:
                        this.textBoxResult.AppendText(text);
                        this.textBoxResult.Focus();
                        if ((this.textBoxResult.Lines.Count() + this.textBox2.Lines.Count()) % 10 == 0)
                        {
                            Settings.SEVEDBSettings.factor += Settings.SEVEDBSettings.factor / 1000;
                            Settings.SEVEDBSettings.addAvg();
                            this.textBox1.AppendText(Settings.SEVEDBSettings.factor.ToString() + "// Avg:" + Settings.SEVEDBSettings.avgFactor.ToString() + "\r\n");
                        }
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
            DirectoryInfo di = new DirectoryInfo(@"C:\Users\Greitone\EVEMarketDumps");
            foreach(FileInfo fi in di.GetFiles())
            {
                IEnumerable<string> stringList = File.ReadLines(fi.FullName);
                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = Environment.ProcessorCount;
                Parallel.ForEach(stringList, po, n =>
                {
                    string[] s = formatOrderString(n);
                    EVEOrder o = new EVEOrder(s);
                    o.InsertToDB();
                }
            );
            }
        }

        private string[] formatOrderString(string n)
        {
            string[] s = n.Split(new string[] { "\",\"", "\t"}, StringSplitOptions.RemoveEmptyEntries);
            if(s[0].Contains(','))
            {
                s = n.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            if (s[0].Contains('\"'))
            {
                for (int i = 0; i < s.Count(); i++)
                {
                    s[i] = s[i].Replace("\",\"", string.Empty);
                    s[i] = s[i].Replace('\"', ' ');
                }
            }
            return s;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //inputMarketDatabaseDump();
            searchFactor();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }


    }
}
