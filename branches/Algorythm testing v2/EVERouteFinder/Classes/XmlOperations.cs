using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Threading;

namespace EVERouteFinder.Classes
{


    class XmlOperations
    {
        private int retrysleeptime;

        public XmlOperations()
        {
            this.retrysleeptime = 1500;
        }

        public XmlOperations(int RetrySleepTime)
        {
            this.retrysleeptime = RetrySleepTime;
        }

        public int RetrySleepTime
        {
            get { return this.retrysleeptime; }
            set { this.retrysleeptime = value; }
        }

        /* Converts and saves an object to an xml file,
         * and locks the file during the process.*/
        public bool Save(Object objectToSerialize, string path, string type)
        {
            FileStream fs = null;
            XmlSerializer xs = null;
            try
            {
                if (CheckFile(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
                    xs = new XmlSerializer(Type.GetType(type));
                    xs.Serialize(fs, objectToSerialize);
                    fs.Close();
                    return true;
                }
                else
                {
                    throw new IOException("File " + path + " is locked by another user and can't be written.");
                }
            }
            catch (IOException ex1)
            {
                throw new IOException("IO Exception thrown at XmlOperations.Save, possibly due to the path to file being wrong. Exception message: " + ex1.Message);
            }
            catch (Exception ex2)
            {
                fs.Close();
                throw new Exception("Exception thrown at XmlOperations.Save, possibly due to a serialization issue, check the inputted data! Exception message: " + ex2.Message);
            }
        }

        /* Loads and returns an object from an xml file
         * and locks the file the file for reading during
         * the process.*/
        public Object Load(string path, string type)
        {
            FileStream fs = null;
            XmlSerializer xs = null;
            try
            {
                if (CheckFile(path, FileMode.Open, FileAccess.Read))
                {
                    fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    xs = new XmlSerializer(Type.GetType(type));
                    Object objectToReturn = xs.Deserialize(fs);
                    fs.Close();
                    return objectToReturn;
                }
                else
                {
                    throw new IOException("File " + path + " is locked by another user and can't be read.");
                }
            }
            catch (IOException ex1)
            {
                throw new IOException("IO Exception thrown at XmlOperations.Load, possibly due to the path to file being wrong or the file being locked. Exception message: " + ex1.Message);
            }
            catch (Exception ex2)
            {
                fs.Close();
                throw new Exception("Exception thrown at XmlOperations.Load, possibly due to a deserialization issue, check the Xml File. Exception message: " + ex2.Message);
            }
        }

        public Object LoadFromStream(MemoryStream mems, string type)
        {
            mems.Position = 0;
            XmlSerializer xs = new XmlSerializer(Type.GetType(type));
            Object objectToReturn = xs.Deserialize(mems);
            return objectToReturn;
        }

        public MemoryStream GetStreamFromFile(string path)
        {
            try
            {
                if (CheckFile(path, FileMode.Open, FileAccess.Read))
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    int lenght = Convert.ToInt32(fs.Length);
                    MemoryStream mems = new MemoryStream(lenght);
                    BinaryReader br = new BinaryReader(fs);
                    byte[] byteBuffer = br.ReadBytes(lenght);
                    mems.Write(byteBuffer, 0, lenght);
                    mems.Position = 0;
                    return mems;
                }
                else
                {
                    throw new IOException("File " + path + " is locked by another user and can't be read.");
                }
            }
            catch (IOException ex1)
            {
                throw new IOException("IO Exception thrown at XmlOperations.Load, possibly due to the path to file being wrong or the file being locked. Exception message: " + ex1.Message);
            }
            catch (Exception ex2)
            {
                throw new Exception("Exception thrown at XmlOperations.Load, possibly due to a deserialization issue, check the Xml File. Exception message: " + ex2.Message);
            }

        }

        /* Checks that the file is available for the required file
         * access, retries 10 times waiting "retrysleeptime" miliseconds
         * for each try, and returns the availability when determined.*/
        private bool CheckFile(string path, FileMode fm, FileAccess fa)
        {
            bool available = false;
            FileStream fs = null;
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    fs = new FileStream(path, fm);
                    fs.Close();
                    available = true;
                    return available;
                }
                catch (Exception ex)
                {
                    Random rn = new Random();
                    Thread.Sleep(this.retrysleeptime + rn.Next(300));
                }
            }
            return available;
        }
    }
}
