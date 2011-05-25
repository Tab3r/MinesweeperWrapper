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
        public Form1()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
        }

        private void LaunchMineweeper()
        {
            bool openlog = false;

            StreamWriter sw = null;

            string dirSystem = Environment.GetEnvironmentVariable("SystemRoot");

            try
            {
                sw = new StreamWriter(@"c:\akst.log", true);
                sw.WriteLine("------------------------------------------");
                sw.WriteLine("User: " + WindowsIdentity.GetCurrent().Name);
                sw.WriteLine("INIT: " + DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString());
                sw.Flush();

                openlog = true;
            }
            catch
            {
                openlog = false;
                sw = null;
            }

            ProcessStartInfo psi = new ProcessStartInfo(dirSystem + @"\system32\winmine.exe");
            Process pr = Process.Start(psi);
            pr.WaitForExit(); // Waiting...

            if (openlog)
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
            
            IWshShell shell = new WshShell();
            var lnk = shell.CreateShortcut(slnk) as IWshShortcut;
            if (lnk != null)
            {
                Console.WriteLine("Link name: {0}", lnk.FullName);
                Console.WriteLine("link target: {0}", lnk.TargetPath);
                Console.WriteLine("link working: {0}", lnk.WorkingDirectory);
                Console.WriteLine("description: {0}", lnk.Description);

                lnk.TargetPath = Application.ExecutablePath;
                lnk.Save();
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
