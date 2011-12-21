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

        //private static List<Node> getSolarSystems()
        //{
        //    List<Node> nodelist = new List<Node>();
        //    XmlOperations xo = new XmlOperations();
        //    nodelist = (List<Node>) xo.LoadFromStream(initializeMemStream(), nodelist.GetType().ToString());
        //    //SetText(nodelist[0].neighborNodes[0].f_score + "\r\n", 1);
        //    return nodelist;
        //}

        private void loopNodes()
        {
            Thread[] threadpool = new Thread[Environment.ProcessorCount];
            //Thread[] threadpool = new Thread[1];
            for (int i = 0; i < threadpool.Count(); i++)
            {
                threadpool[i] = new Thread( () => Loop(i));
                threadpool[i].Start();
                Thread.Sleep(300);
            }
        }

        private void Loop(int i)
        {
            List<Node> nodelist = new List<Node>();
            XmlOperations xo = new XmlOperations();
            MemoryStream ms;
            ms = xo.GetStreamFromFile(Path.Combine(Directory.GetCurrentDirectory(), "nodeList.xml"));
            nodelist = (List<Node>)xo.LoadFromStream(ms, nodelist.GetType().ToString());
            for (int j = 1; j < nodelist.Count + 1; j++)
            {
                if (j % (i + 1) == 0)
                {
                    foreach (Node n in nodelist)
                    {
                        loop(nodelist[j - 1], n, ms);
                    }
                }
            }


        }
        //private void loopNodes()
        //{
        //    List<Node> myList = getSolarSystems();
        //    ParallelOptions po = new ParallelOptions();
        //    po.MaxDegreeOfParallelism = Environment.ProcessorCount;
        //    Parallel.ForEach(myList, po, n =>
        //    {
        //        List<Node> mylist2 = getSolarSystems();
        //        foreach (Node n1 in mylist2)
        //        {
        //            loop(n, n1);
        //        }
        //    }
        //    );

        //    this.textBoxResult.Text = Settings.SEVEDBSettings.factor.ToString();
        //}

        private void loop(Node n, Node n1, MemoryStream ms)
        {
            if (n.ID != n1.ID)
            {
                n.nofactor = n1.nofactor = false;
                PathOperations pop = new PathOperations(n, n1);
                pop.nofactor = false;
                pop.start.nofactor = false;
                pop.goal.nofactor = false;
                List<Node> route = new List<Node>();
                List<Node> nodelist = new List<Node>();
                XmlOperations xo = new XmlOperations();
                nodelist = (List<Node>)xo.LoadFromStream(ms, nodelist.GetType().ToString());

                pop.completeNodeList = nodelist;

                route = pop.Evaluate();
                string systems = "";
                int i = 0;
                foreach (Node n2 in route)
                {
                    systems += "System: " + " " + n2.Name + " " + n2.Security.ToString() + " " + n2.Region.ToString() + " " + "\r\n"; //n2.f_score.ToString() +
                    i++;
                }
                n.nofactor = n1.nofactor = true;
                pop = new PathOperations(n, n1);
                pop.nofactor = true;
                pop.start.nofactor = true;
                pop.goal.nofactor = true;
                route = new List<Node>();
                nodelist = new List<Node>();
                xo = new XmlOperations();
                nodelist = (List<Node>)xo.LoadFromStream(ms, nodelist.GetType().ToString());

                pop.completeNodeList = nodelist;
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
                        SetText(Settings.SEVEDBSettings.factor.ToString() + "// Avg:" + Settings.SEVEDBSettings.avgFactor.ToString() + " /// Deviation: " + (i - a).ToString() + " // on " + DateTime.Now.ToLongTimeString() + "\r\n", 2);
                        SetText((i - a).ToString(), 3);
                    }
                    if (a - i > 0)
                    {
                        int b = 0;
                        b += 1;
                    }
                }
                else
                {
                    SetText(n.Name + " " + n1.Name + " " + "Qualified " + i.ToString() + "\r\n", 0);
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
                        double fact = 5;
                        if (errorrate < 0.1)
                        {
                            Settings.SEVEDBSettings.factor += fact * Settings.SEVEDBSettings.factor / 5000;
                            Settings.SEVEDBSettings.addAvg();
                            this.textBox1.AppendText(Settings.SEVEDBSettings.factor.ToString() + "// Avg:" + Settings.SEVEDBSettings.avgFactor.ToString() + "\r\n");
                        }
                        else
                        {
                            Settings.SEVEDBSettings.factor += Settings.SEVEDBSettings.factor / 5000;
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
                    case 3:
                        int a = Convert.ToInt32(text);
                        double errRate;
                        errors++;
                        this.textBoxCentre2.Text = (errors).ToString();
                        Settings.SEVEDBSettings.factor -= a * Settings.SEVEDBSettings.factor / 100;
                        Settings.SEVEDBSettings.addAvg();
                        errRate = errors / ((double)(Convert.ToInt32(this.textBoxLeft.Text) + Convert.ToInt32(this.textBoxCentre.Text)));
                        this.errorrate = errRate;
                        this.textBoxRight2.Text = errRate.ToString();
                        break;
                }
            }
        }
        int errors = 0;
        double errorrate = 0;
        //private List<Node> getList(int thread)
        //{
            
        //}

        delegate void StartLoop(Node n, Node n1);

        delegate void SetTextCallback(string text, int thread);

        delegate void StartDoingWork();

        delegate void StartDoingWorkInt(int i);

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
            this.button1.Text = DateTime.Now.ToString();
            searchFactor();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        private void textBoxResult_TextChanged(object sender, EventArgs e)
        {
            this.textBoxLeft.Text = textBoxResult.Lines.Count().ToString();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.textBoxCentre.Text = textBox2.Lines.Count().ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.textBoxRight.Text = textBox1.Lines.Count().ToString();
        }


    }
}
