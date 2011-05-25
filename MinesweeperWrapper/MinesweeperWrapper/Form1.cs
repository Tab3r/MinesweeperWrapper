using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        private void LaunchMineweeper()
        {
            string dirSystem = Environment.GetEnvironmentVariable("SystemRoot");

            ProcessStartInfo psi = new ProcessStartInfo(dirSystem + @"\system32\winmine.exe");
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
            ChangeLinkToMe();
            LaunchMineweeper();
            this.Close();
        }
    }
}
