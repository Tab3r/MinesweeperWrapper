using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace MinesweeperWrapper.Configuration
{
    [Serializable]
    public class Configuration
    {
        string _pathToLnk;
        string _name;
        bool _changedLnk;
        bool _overwrited;

        public Configuration()
        {
            this._pathToLnk = string.Empty;
            this._name = string.Empty;
            this._changedLnk = false;
            this._overwrited = false;
        }

        public string GetPathToLnk
        {
            get { return this._pathToLnk; }
            set { this._pathToLnk = value; }
        }

        public string GetNameApp
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public bool GetChangedLnk
        {
            get { return this._changedLnk; }
            set { this._changedLnk = value; }
        }

        public bool GetOverwrited
        {
            get { return this._overwrited; }
            set { this._overwrited = value; }
        }

        public static void Serialize(string file, Configuration c)
        {
            XmlSerializer xmlSer = new XmlSerializer(typeof(Configuration));
            StreamWriter sw = File.CreateText(file);
            xmlSer.Serialize(sw, c);
            sw.Flush();
            sw.Close();
            sw = null;
        }

        public static Configuration Deserialize(string file)
        {
            XmlSerializer xmlSer = new XmlSerializer(typeof(Configuration));
            StreamReader sr = File.OpenText(file);
            Configuration c = (Configuration)xmlSer.Deserialize(sr);
            sr.Close();
            return c;
        }
    }
}
