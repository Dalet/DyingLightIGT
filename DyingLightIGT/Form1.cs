using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DyingLightIGT
{
    public partial class Form1 : Form
    {
        string formTitle;
        string infoString;

        GameMemory _gameMemory;

        TcpClient _client;
        Process _launcher;

        Task _connectionTask;
        Task _connectionCheckTask;
        SynchronizationContext _uiThread;

        Settings _settings;

        public Form1()
        {
            InitializeComponent();
            Version currentVer = Assembly.GetExecutingAssembly().GetName().Version;
            formTitle = "DL IGT " + currentVer.Major + "." + currentVer.Minor + "." + currentVer.Build;
            labelTimer.Text = "0.00";
            this.Disposed += Dispose;
            this.Icon = Properties.Resources.DyingLightGame_161;

            CheckArguments();

            _settings = new Settings(this);

            _uiThread = SynchronizationContext.Current;
            if (_settings.CheckUpdates)
                Task.Factory.StartNew(CheckUpdate);
            _connectionTask = Task.Factory.StartNew(TryToConnect);
            _connectionCheckTask = Task.Factory.StartNew(CheckClientConnection);

            this.DataBindings.Add("BackColor", _settings, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            this.labelTimer.DataBindings.Add("ForeColor", _settings, "TimeColor", false, DataSourceUpdateMode.OnPropertyChanged);

            _gameMemory = new GameMemory();
            _gameMemory.OnTick += gameMemory_OnTick;
            _gameMemory.OnStart += gameMemory_OnStart;
            _gameMemory.OnReset += gameMemory_OnReset;
            _gameMemory.StartMonitoring();
        }

        void gameMemory_OnStart(object sender, EventArgs e)
        {
            SendCommand("pausegametime");
            if (_settings.AutoStart)
                SendCommand("starttimer");
        }

        void gameMemory_OnReset(object sender, EventArgs e)
        {
            if (_settings.AutoReset)
                SendCommand("reset");
        }

        void gameMemory_OnTick(object sender, float time)
        {
            TimeSpan ts = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond * time));

            int hoursTotal = ts.Days * 24 + ts.Hours;
            string hours = hoursTotal + ":";
            string minutes = string.Format("{0:00}:", ts.Minutes);
            string seconds = string.Format("{0:00}", ts.Seconds); ;
            string milliseconds = string.Format(".{0:00}", ts.Milliseconds).Substring(0, 3);

            if (hoursTotal == 0)
                hours = "";

            if (ts.Minutes == 0 && hoursTotal == 0)
                minutes = "";
            else if (ts.Minutes != 0 && ts.Minutes < 10 && hoursTotal == 0)
                minutes = ts.Minutes + ":";

            if (ts.Seconds < 10 && ts.Minutes == 0 && hoursTotal == 0)
                seconds = ts.Seconds.ToString();

            labelTimer.Text = hours + minutes + seconds + milliseconds;
            if (ts > new TimeSpan(0))
                SendCommand("setgametime " + labelTimer.Text);
        }

        void CheckArguments()
        {
            int i = 0;
            foreach (string args in Program.args)
            {
                if (args == "-launcherid" && i + 1 < Program.args.Length)
                {
                    int id;
                    if (int.TryParse(Program.args[i + 1], out id))
                    {
                        try
                        {
                            _launcher = Process.GetProcessById(id);
                        } catch (ArgumentException) { }
                    }
                }

                i++;
            }
        }

        void SendCommand(string str)
        {
            if (_client != null)
            {
                try
                {
                    byte[] bytes = ASCIIEncoding.ASCII.GetBytes(str + "\r\n");
                    Trace.WriteLine("Sending: " + str);
                    _client.GetStream().Write(bytes, 0, bytes.Length);
                } catch (Exception) { }
            }
        }

        void TryToConnect()
        {
            if (_launcher != null && _launcher.HasExited)
                Application.Exit();

            _uiThread.Send(d =>
            {
                this.SetInfoString("LS link = KO");
            }, null);

            while (_client == null)
            {
                try
                {
                    _client = new TcpClient(_settings.SERVER_IP, _settings.Port);
                }
                catch (SocketException) { }
                Thread.Sleep(100);
            }

            _uiThread.Send(d =>
            {
                this.SendCommand("pausegametime");
                this.SetInfoString("LS link = OK!");
            }, null);
        }

        void CheckClientConnection()
        {
            while (true)
            {
                if (_client != null && _connectionTask.IsCompleted)
                {
                    try
                    {
                        if (_client.GetStream().Read(new byte[] { 0 }, 0, 1) == 0)
                            throw new System.IO.IOException("Couldn't read from the stream.");
                    }
                    catch (Exception)
                    {
                        if (_client != null)
                        {
                            _client.Close();
                            _client = null;
                        }
                        _connectionTask = Task.Factory.StartNew(TryToConnect);
                    }
                }
                Thread.Sleep(500);
            }
        }

        public void SetInfoString(string str)
        {
            infoString = str;
            this.Text = formTitle + " | " + infoString;
        }

        void CheckUpdate()
        {
            Version lastVer = new Version(0, 0, 0, 0);
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
                DialogResult result = MessageBox.Show("A new version is available.\nDo you want to update?", lastVer + " update", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/Dalet/DyingLightIGT/releases/latest");
                }
            }
        }

        void Dispose(Object sender, EventArgs e)
        {
            if (_gameMemory != null)
            {
                _gameMemory.OnStart -= gameMemory_OnStart;
                _gameMemory.OnTick -= gameMemory_OnTick;
                _gameMemory.Stop();
            }

            if (_client != null)
            {
                SendCommand("unpausegametime");
                _client.Close();
            }
            
            if (_settings != null)
                _settings.Dispose();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settings.SetDesktopLocation(this.DesktopLocation.X + 10, this.DesktopLocation.Y + 10);
            _settings.ShowDialog(this);
        }

        private void gitHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Dalet/DyingLightIGT");
        }
    }
}
