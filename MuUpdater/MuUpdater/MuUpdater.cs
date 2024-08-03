using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MuUpdater
{
    public partial class MuUdaterForm : Form
    {
        private string ExecutablePath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        private void SuspendForClosingForm(int Time)
        {
            Thread.Sleep(Time);
        }

        private void ExecutableReplacement(string OldLauncherPath, string NewLauncherPath)
        {
            try
            {
                if (!File.Exists(NewLauncherPath))
                {
                    return;
                }
                if (File.Exists(OldLauncherPath))
                {
                    File.Delete(OldLauncherPath);
                    //log: MessageBox.Show("OldLauncher.exe has deleted.");
                }

                File.Copy(NewLauncherPath, OldLauncherPath);

                //log: MessageBox.Show("NewLauncher.exe has copied to root directory.", "MuUpdater");

                if (File.Exists(NewLauncherPath))
                {
                    File.Delete(NewLauncherPath);
                    //log: MessageBox.Show("New Launcher from old directory has deleted.", "MuUpdater");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(string.Format("ExecutableReplacement() {0}", Ex.Message));
                Environment.Exit(0);
            }
        }

        private void StartExec(string FileName)
        {
            try
            {
                var Process = new Process();
                Process.StartInfo.Verb = "runas";
                Process.StartInfo.FileName = ExecutablePath + @"\" + FileName;
                Process.Start();
                Environment.Exit(0);
            }
            catch (System.Exception Ex)
            {
                MessageBox.Show(string.Format("StartExec() {0}", Ex.Message));
                Environment.Exit(0);
            }
        }     

        public MuUdaterForm()
        {
            InitializeComponent();
            SuspendForClosingForm(3000);
            ExecutableReplacement(ExecutablePath + @"\Launcher.exe", ExecutablePath + @"\NewLauncher.exe");     
            StartExec("Launcher.exe");
            Environment.Exit(0);
        }

        private void MuUdaterForm_Load(object sender, EventArgs e)
        {

        }
    }
}
