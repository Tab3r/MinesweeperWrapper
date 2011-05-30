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
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Security.Principal;
using IWshRuntimeLibrary;

namespace MinesweeperWrapper
{
    public partial class Form1 : Form
    {
        StreamWriter _sw = null;
        Configuration.Configuration _conf;
        string _PathXmlConfig = Environment.GetEnvironmentVariable("SystemRoot") + @"\winmine.xml";
        string _PathExe = Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\winmine.exe";
        string _PathBackupExe = Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\winmine2.exe";

        #region Form Events

        public Form1()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;

            // Get configuration
            if (System.IO.File.Exists(_PathXmlConfig))
            {
                // Load config
                this._conf = Configuration.Configuration.Deserialize(_PathXmlConfig);

            }
            else
            {
                // Create config xml
                this._conf = new Configuration.Configuration();
            }

            try
            {
                _sw = new StreamWriter(@"c:\akst.log", true);
                _sw.WriteLine("------------------------------------------");
                _sw.WriteLine("User: " + WindowsIdentity.GetCurrent().Name);
                _sw.WriteLine("INIT: " + DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString());
                _sw.Flush();
            }
            catch
            {
                _sw = null;
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {

            // Kill process
            KillAProcess("winmine");

            // Is the file copied?
            if (this._conf.IsGetOverwritedExe == false)
                this.CopyWrapper();
            // Is the link changed?
            if (this._conf.IsChangedLnk == false)
                this.ChangeLinkToWrapper();

            LaunchMineweeper(this._conf.PathToExe);
            this.Close();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Configuration.Configuration.Serialize(this._PathXmlConfig, this._conf);
        }

        #endregion

        private void LaunchMineweeper(string path)
        {
            //string dirSystem = Environment.GetEnvironmentVariable("SystemRoot");

            ProcessStartInfo psi = new ProcessStartInfo(path); //(dirSystem + @"\system32\winmine.exe");
            Process pr = Process.Start(psi);
            pr.WaitForExit(); // Waiting...

            if (_sw != null)
            {
                _sw.WriteLine("END: " + DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString());
                _sw.WriteLine("------------------------------------------");
                _sw.Flush();
                _sw.Close();
            }

            
        }

        private bool KillAProcess(string name)
        {
            bool ok = false;

            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName.StartsWith(name))
                {
                    try
                    {
                        p.Kill();
                        ok = true;
                    }
                    catch
                    {
                        ok = false;
                    }
                }
            }

            return ok;
        }

        private void ChangeLinkToWrapper()
        {
            string slnk = Environment.GetEnvironmentVariable("ALLUSERSPROFILE") + @"\Menú Inicio\Programas\Juegos\Buscaminas.lnk";

            try
            {

                IWshShell shell = new WshShell();
                var lnk = shell.CreateShortcut(slnk) as IWshShortcut;

                if (_sw != null)
                {
                    _sw.Write("LINK: Change target path, ");
                    _sw.Flush();
                }

                if (lnk != null)
                {
                    if (lnk.TargetPath.ToLower() == this._PathExe.ToLower())
                    {
                        if (_sw != null)
                        {
                            _sw.Write("target is already changed.\n");
                            _sw.Flush();
                            this._conf.IsChangedLnk = true;
                        }
                    }
                    else
                    {
                        lnk.TargetPath = this._PathExe;
                        lnk.Save();
                        if (_sw != null)
                        {
                            _sw.Write("OK.\n");
                            _sw.Flush();
                        }
                    }
                }
                else
                {
                    lnk = null;
                    if (_sw != null)
                    {
                        _sw.Write("ERROR: lnk is null.\n");
                        _sw.Flush();
                    }
                }
            }
            catch(Exception e)
            {
                if (_sw != null)
                {
                    _sw.WriteLine("LINK: ERROR " + e.Message);
                    _sw.Flush();
                }
            }
            
        }

        private bool CopyWrapper()
        {
            bool ok = false;
            

            // Copy exe
            try
            {
                System.IO.File.Delete(_PathBackupExe);
                System.IO.File.Copy(_PathExe, _PathBackupExe);

                // Remove Minesweeper component
                // ----------------------------
                // Is not necesary, but cleaner...
                //string t = ("[Components]\nminesweeper=off\nGames=off\n");
                //System.IO.File.WriteAllText(@"c:\tempmw.ini", t);

                //ProcessStartInfo psi = new ProcessStartInfo();
                //psi.FileName = "sysocmgr";
                //psi.Arguments = @"/i:c:\windows\inf\sysoc.inf /u:c:\tempmw.ini /q";
                //psi.CreateNoWindow = true;
                //psi.WindowStyle = ProcessWindowStyle.Hidden;
                //Process.Start(psi);
                //System.IO.File.Delete(@"c:\tempmw.ini");

                // Disable WFP during 1 minute to overwrite file
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.FileName = Application.StartupPath + @"\WfpDeprotect.exe";
                psi.Arguments = this._PathExe;
                Process.Start(psi);

                string myExe = Application.ExecutablePath;
                System.IO.File.Copy(myExe, _PathExe, true);

                this._conf.IsGetOverwritedExe = true;
                this._conf.PathToExe = _PathBackupExe;
                this._conf.PathToWrapper = _PathExe;

                ok = true;
            }
            catch (Exception ex)
            {
                ok = false;
                if (this._sw != null)
                {
                    this._sw.WriteLine("ERROR: coping new file.\n");
                    this._sw.WriteLine("ERROR: " + ex.Message);
                }

            }

            return ok;

        }

        
    }
}
