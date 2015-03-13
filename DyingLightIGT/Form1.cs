using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace DyingLightIGT
{
    public partial class Form1 : Form
    {
        GameMemory _gameMemory;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Dying Light IGT " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _gameMemory = new GameMemory();
            _gameMemory.OnTick += gameMemory_OnTick;
            _gameMemory.StartMonitoring();

            Version lastVer = new Version(0,0,0,0);
            try
            {
                lastVer = Updater.Check();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
            }

            if (lastVer > Assembly.GetExecutingAssembly().GetName().Version)
            {
                DialogResult result = MessageBox.Show("A new version is available.\nDo you want to update?", lastVer + " update", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/Dalet/DyingLightIGT/releases/latest");
                }
            }
        }

        void gameMemory_OnTick(object sender, float time)
        {
            TimeSpan ts = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond * time));

            string hours = string.Format("{0:00}:", ts.Hours);
            string minutes = string.Format("{0:00}:",  ts.Minutes);
            string seconds = string.Format("{0:00}", ts.Seconds); ;
            string milliseconds = string.Format(".{0:00}", ts.Milliseconds).Substring(0, 3);

            if (ts.Hours <= 0)
                hours = "";
            else if (ts.Hours < 10)
                hours = ts.Hours + ":";

            if (ts.Minutes <= 0 && ts.Hours <= 0)
                minutes = "";
            else if (ts.Minutes != 0 && ts.Minutes < 10)
                minutes = ts.Minutes + ":";

            if (ts.Seconds < 10 && ts.Minutes <= 0 && ts.Hours <= 0)
                seconds = ts.Seconds.ToString();

            labelTimer.Text = hours + minutes + seconds + milliseconds;
        }
    }
}
