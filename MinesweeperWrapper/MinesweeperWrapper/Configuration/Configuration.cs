/* ********************************************************
 * The MIT License (MIT)
 * Copyright (c) <year> <copyright holders>
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
 * Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 */
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
        string _PathToWrapper;
        string _PathToExe;
        bool _changedLnk;
        bool _overwritedFile;

        public Configuration()
        {
            this._PathToWrapper = string.Empty;
            this._PathToExe = string.Empty;
            this._changedLnk = false;
            this._overwritedFile = false;
        }

        public string PathToWrapper
        {
            get { return this._PathToWrapper; }
            set { this._PathToWrapper = value; }
        }

        public string PathToExe
        {
            get { return this._PathToExe; }
            set { this._PathToExe = value; }
        }

        public bool IsChangedLnk
        {
            get { return this._changedLnk; }
            set { this._changedLnk = value; }
        }

        public bool IsGetOverwritedExe
        {
            get { return this._overwritedFile; }
            set { this._overwritedFile = value; }
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
