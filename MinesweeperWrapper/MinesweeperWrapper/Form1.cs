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
            
            try
            {
                sw = new StreamWriter(@"c:\ASkLog.txt", true);
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

            string dirSystem = Environment.GetEnvironmentVariable("SystemRoot");

            ProcessStartInfo psi = new ProcessStartInfo(dirSystem + @"\system32\winmine.exe");
            Process pr = Process.Start(psi);
            pr.WaitForExit();

            if (openlog)
            {
                sw.WriteLine("END: " + DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString());
                sw.WriteLine("------------------------------------------");
                sw.Flush();
                sw.Close();
            }

            
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            LaunchMineweeper();
            this.Close();
        }
    }
}
