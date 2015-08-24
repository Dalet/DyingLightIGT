using Fetze.WinFormsColor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace DyingLightIGT
{
    public partial class Settings : Form
    {
        public bool CheckUpdates { get; set; }
        public string SERVER_IP { get; set; }
        public int Port { get; set; }
        public bool AutoStart { get; set; }
        public bool AutoReset { get; set; }
        public bool AutoSplit { get; set; }
        public BindingList<int> AutoSplits { get; set; }
        public Color TimeColor { get; set; }
        public Color BackgroundColor { get; set; }

        public const string CONFIG_FILE_NAME = "DyingLightIGT.cfg";

        Color DefaultBackgroundColor;
        Color DefaultTimeColor;

        Form1 _mainWindow;
        Autosplits _autosplitsWindow;

        public Settings(Form1 parent)
        {
            InitializeComponent();
            this.FormClosing += Form_FormClosing;
            this.Icon = Properties.Resources.DyingLightGame_161;

            _mainWindow = parent;

            CheckUpdates = true;
            SERVER_IP = "127.0.0.1";
            Port = 16834;
            AutoStart = true;
            AutoReset = true;
            AutoSplit = true;
            AutoSplits = new BindingList<int>();
            BackgroundColor = DefaultBackgroundColor = _mainWindow.BackColor;
            TimeColor = DefaultTimeColor = _mainWindow.labelTimer.ForeColor;

            this.btnBackgroundColorReset.Click += (s, e) => btnBackgroundColor.BackColor = DefaultBackgroundColor;
            this.btnTimeColorReset.Click += (s, e) => btnTimeColor.BackColor = DefaultTimeColor;

            LoadSettings();
            CheckArguments();

            this.chkCheckUpdates.DataBindings.Add("Checked", this, "CheckUpdates", false, DataSourceUpdateMode.OnPropertyChanged);
            this.btnBackgroundColor.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            this.btnTimeColor.DataBindings.Add("BackColor", this, "TimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoStart.DataBindings.Add("Checked", this, "AutoStart", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoReset.DataBindings.Add("Checked", this, "AutoReset", false, DataSourceUpdateMode.OnPropertyChanged);
            this.numPort.DataBindings.Add("Value", this, "Port", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutosplits.DataBindings.Add("Checked", this, "AutoSplit", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        void CheckArguments()
        {
            for (int i = 0; i < Program.args.Length; i++)
            {
                string arg = Program.args[i];

                if (arg == "-port" && i + 1 < Program.args.Length)
                {
                    int port;
                    if (int.TryParse(Program.args[i + 1], out port))
                        Port = port;
                }
                else if (arg == "-livesplit") // arg used when launching via the LiveSplit component
                {
                    CheckUpdates = false;
                    this.chkCheckUpdates.Visible = false;
                }
                else if (arg.StartsWith("-autostart="))
                {
                    bool ret;
                    if (Boolean.TryParse(arg.Remove(0, "-autostart=".Length), out ret))
                        AutoStart = ret;
                }
                else if (arg.StartsWith("-autoreset="))
                {
                    bool ret;
                    if (Boolean.TryParse(arg.Remove(0, "-autoreset=".Length), out ret))
                        AutoReset = ret;
                }
                else if (arg.StartsWith("-autosplit="))
                {
                    bool ret;
                    if (Boolean.TryParse(arg.Remove(0, "-autosplit=".Length), out ret))
                        AutoSplit = ret;
                }
            }
            SaveSettings();
        }

        void SaveSettings()
        {
            var doc = new XmlDocument();

            doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));

            XmlElement settingsElem = doc.CreateElement("Settings");
            settingsElem.SetAttribute("version", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));
            doc.AppendChild(settingsElem);

            XmlNode generalNode = doc.CreateElement("General");
            settingsElem.AppendChild(generalNode);

            generalNode.AppendChild(ToElement(doc, "CheckUpdates", CheckUpdates));
            generalNode.AppendChild(ToElement(doc, "BackgroundColor", BackgroundColor));
            generalNode.AppendChild(ToElement(doc, "TimeColor", TimeColor));


            XmlNode liveSplitServerNode = doc.CreateElement("LiveSplitServer");
            settingsElem.AppendChild(liveSplitServerNode);

            liveSplitServerNode.AppendChild(ToElement(doc, "Port", Port));
            liveSplitServerNode.AppendChild(ToElement(doc, "AutoStart", AutoStart));
            liveSplitServerNode.AppendChild(ToElement(doc, "AutoReset", AutoReset));
            liveSplitServerNode.AppendChild(ToElement(doc, "AutoSplit", AutoSplit));
            liveSplitServerNode.AppendChild(doc.CreateElement("AutoSplits"));

            XmlNode autosplitsNode = liveSplitServerNode["AutoSplits"];
            foreach (uint percent in AutoSplits)
            {
                autosplitsNode.AppendChild(ToElement(doc, "StoryPercentage", percent));
            }

            try
            {
                doc.Save(Application.StartupPath + "\\" + CONFIG_FILE_NAME);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(_mainWindow, "Access to the config file was denied when trying to save settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void LoadSettings()
        {
            var doc = new XmlDocument();

            if (File.Exists(Application.StartupPath + "\\" + CONFIG_FILE_NAME))
            {
                doc.Load(Application.StartupPath + "\\" + CONFIG_FILE_NAME);

                Version settingsVersion = new Version(doc["Settings"].GetAttribute("version"));

                XmlNode generalNode = doc["Settings"]["General"];

                CheckUpdates = ParseBool(generalNode, "CheckUpdates", true);
                BackgroundColor = ParseColor(generalNode["BackgroundColor"]);
                TimeColor = ParseColor(generalNode["TimeColor"]);

                XmlNode liveSplitServerNode = doc["Settings"]["LiveSplitServer"];

                foreach (XmlNode child in liveSplitServerNode["AutoSplits"].ChildNodes)
                {
                    if (child.Name == "StoryPercentage")
                    {
                        var p = int.Parse(child.InnerText);
                        if (!AutoSplits.Contains(p))
                            AutoSplits.Add(p);
                    }
                }

                AutoStart = ParseBool(liveSplitServerNode, "AutoStart", true);
                AutoReset = ParseBool(liveSplitServerNode, "AutoReset", true);
                AutoSplit = ParseBool(liveSplitServerNode, "AutoSplit", true);
                Port = int.Parse(liveSplitServerNode["Port"].InnerText);
            }
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.FormOwnerClosing)
                e.Cancel = true;
            SaveSettings();
            this.Hide();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SaveSettings();
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            LoadSettings();
            this.Hide();
        }
        static XmlElement ToElement(XmlDocument document, string name, Color color)
        {
            var element = document.CreateElement(name);
            element.InnerText = color.ToArgb().ToString("X8");
            return element;
        }

        static XmlElement ToElement<T>(XmlDocument document, string name, T value)
        {
            XmlElement str = document.CreateElement(name);
            str.InnerText = value.ToString();
            return str;
        }
        static Color ParseColor(XmlElement colorElement)
        {
            return Color.FromArgb(Int32.Parse(colorElement.InnerText, NumberStyles.HexNumber));
        }

        static bool ParseBool(XmlNode settings, string setting, bool default_ = false)
        {
            bool val;
            return settings[setting] != null ?
                (Boolean.TryParse(settings[setting].InnerText, out val) ? val : default_)
                : default_;
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var picker = new ColorPickerDialog();
            picker.SelectedColorChanged += (s, x) => button.BackColor = Color.FromArgb(picker.SelectedColor.R, picker.SelectedColor.G, picker.SelectedColor.B);
            picker.SelectedColor = picker.OldColor = button.BackColor;
            picker.ShowDialog(this);
            button.BackColor = Color.FromArgb(picker.SelectedColor.R, picker.SelectedColor.G, picker.SelectedColor.B);
        }

        private void btnAutosplits_Click(object sender, EventArgs e)
        {
            _autosplitsWindow = new Autosplits(this);
            _autosplitsWindow.ShowDialog(this);
            _autosplitsWindow = null;
        }
    }
}
