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
        StreamWriter sw = null;

        public Form1()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;

            

            try
            {
                sw = new StreamWriter(@"c:\akst.log", true);
                sw.WriteLine("------------------------------------------");
                sw.WriteLine("User: " + WindowsIdentity.GetCurrent().Name);
                sw.WriteLine("INIT: " + DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString());
                sw.Flush();
            }
            catch
            {
                sw = null;
            }
        }

        private void LaunchMineweeper(string path)
        {
            //string dirSystem = Environment.GetEnvironmentVariable("SystemRoot");

            ProcessStartInfo psi = new ProcessStartInfo(path); //(dirSystem + @"\system32\winmine.exe");
            Process pr = Process.Start(psi);
            pr.WaitForExit(); // Waiting...

            if (sw != null)
            {
                sw.WriteLine("END: " + DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString());
                sw.WriteLine("------------------------------------------");
                sw.Flush();
                sw.Close();
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

        private void ChangeLinkToMe()
        {
            string slnk = Environment.GetEnvironmentVariable("ALLUSERSPROFILE") + @"\Menú Inicio\Programas\Juegos\Buscaminas.lnk";

            try
            {

                IWshShell shell = new WshShell();
                var lnk = shell.CreateShortcut(slnk) as IWshShortcut;

                if (sw != null)
                {
                    sw.Write("LINK: Change target path, ");
                    sw.Flush();
                }

                if (lnk != null)
                {
                    if (lnk.TargetPath.ToLower() == Application.ExecutablePath.ToLower())
                    {
                        if (sw != null)
                        {
                            sw.Write("target is already changed.\n");
                            sw.Flush();
                        }
                    }
                    else
                    {
                        lnk.TargetPath = Application.ExecutablePath;
                        lnk.Save();
                        if (sw != null)
                        {
                            sw.Write("OK.\n");
                            sw.Flush();
                        }
                    }
                }
                else
                {
                    lnk = null;
                    if (sw != null)
                    {
                        sw.Write("ERROR: lnk is null.\n");
                        sw.Flush();
                    }
                }
            }
            catch(Exception e)
            {
                if (sw != null)
                {
                    sw.WriteLine("LINK: ERROR " + e.Message);
                    sw.Flush();
                }
            }
            
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            // Get configuration
            string PathXmlConfig = Environment.GetEnvironmentVariable("SystemRoot") + @"\winmine.xml";
            string PathExe = Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\winmine.exe";
            string PathBackupExe = Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\minewin.exe";
            Configuration.Configuration c;
            if (System.IO.File.Exists(PathXmlConfig))
            {
                // Load config
                c = Configuration.Configuration.Deserialize(PathXmlConfig);

            }
            else
            {
                // Matar al proceso
                KillAProcess("winmine");

                // IMPORTANT...
                // it don't work... The "DLLCache protection" is screwing us
                // Look the script folder for a possibly good solution
                // --------------------------------------------------------

                // Copy exe
                System.IO.File.Delete(PathBackupExe);
                System.IO.File.Move(PathExe, PathBackupExe);
                System.IO.File.Delete(PathExe);
                string myExe = Application.ExecutablePath;
                System.IO.File.Copy(myExe, PathExe, true);

                // ---------------------------------------------------------

                // Create config xml
                c = new Configuration.Configuration();
                c.GetOverwrited = true;
                c.GetNameApp = PathBackupExe;
                c.GetChangedLnk = true;
                c.GetPathToLnk = "";
                Configuration.Configuration.Serialize(PathXmlConfig, c);

                // and exit
                this.Close();
            }

            
            //ChangeLinkToMe();
            LaunchMineweeper(c.GetNameApp);
            this.Close();
        }
    }
}
