using Fetze.WinFormsColor;
using System;
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
        public Color TimeColor { get; set; }
        public Color BackgroundColor { get; set; }

        const string configFileName = "DyingLightIGT.cfg";
        XmlDocument doc;

        Color DefaultBackgroundColor;
        Color DefaultTimeColor;

        Form1 _mainWindow;

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
            BackgroundColor = DefaultBackgroundColor = _mainWindow.BackColor;
            TimeColor = DefaultTimeColor = _mainWindow.labelTimer.ForeColor;

            this.btnBackgroundColorReset.Click += (s, e) => btnBackgroundColor.BackColor = DefaultBackgroundColor;
            this.btnTimeColorReset.Click += (s, e) => btnTimeColor.BackColor = DefaultTimeColor;

            InitializeConfigFile();
            CheckArguments();
            
            this.chkCheckUpdates.DataBindings.Add("Checked", this, "CheckUpdates", false, DataSourceUpdateMode.OnPropertyChanged);
            this.btnBackgroundColor.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            this.btnTimeColor.DataBindings.Add("BackColor", this, "TimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoStart.DataBindings.Add("Checked", this, "AutoStart", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoReset.DataBindings.Add("Checked", this, "AutoReset", false, DataSourceUpdateMode.OnPropertyChanged);
            this.numPort.DataBindings.Add("Value", this, "Port", false, DataSourceUpdateMode.OnPropertyChanged);
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
            }
            SaveSettings();
        }

        void InitializeConfigFile()
        {
            doc = new XmlDocument();

            if (File.Exists(configFileName))
            {
                LoadSettings();
            }
            else
            {
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

                liveSplitServerNode.AppendChild(ToElement(doc, "AutoStart", AutoStart));
                liveSplitServerNode.AppendChild(ToElement(doc, "AutoReset", AutoReset));
                liveSplitServerNode.AppendChild(ToElement(doc, "Port", Port));
            }
        }

        void SaveSettings()
        {
            doc["Settings"].SetAttribute("version", Assembly.GetExecutingAssembly().GetName().Version.ToString(3));

            XmlNode generalNode = doc["Settings"]["General"];
            generalNode["CheckUpdates"].InnerText = CheckUpdates.ToString();
            generalNode["BackgroundColor"].InnerText = BackgroundColor.ToArgb().ToString("X8");
            generalNode["TimeColor"].InnerText = TimeColor.ToArgb().ToString("X8");

            XmlNode liveSplitServerNode = doc["Settings"]["LiveSplitServer"];
            liveSplitServerNode["AutoStart"].InnerText = AutoStart.ToString();
            liveSplitServerNode["AutoReset"].InnerText = AutoReset.ToString();
            liveSplitServerNode["Port"].InnerText = Port.ToString();

            try
            {
                doc.Save(configFileName);
            }
            catch (UnauthorizedAccessException) { }
        }

        void LoadSettings()
        {
            if (File.Exists(configFileName))
            {
                doc.Load(configFileName);
            }

            Version settingsVersion = new Version(doc["Settings"].GetAttribute("version"));

            XmlNode generalNode = doc["Settings"]["General"];
            if (generalNode == null)
            {
                generalNode = doc.CreateElement("General");
                doc["Settings"].AppendChild(generalNode);
            }

            if (generalNode["CheckUpdates"] == null)
                generalNode.AppendChild(ToElement(doc, "CheckUpdates", CheckUpdates));
            CheckUpdates = bool.Parse(generalNode["CheckUpdates"].InnerText);

            if (generalNode["BackgroundColor"] == null)
                generalNode.AppendChild(ToElement(doc, "BackgroundColor", BackgroundColor));
            BackgroundColor = ParseColor(generalNode["BackgroundColor"]);

            if (generalNode["TimeColor"] == null)
                generalNode.AppendChild(ToElement(doc, "TimeColor", TimeColor));
            TimeColor = ParseColor(generalNode["TimeColor"]);

            XmlNode liveSplitServerNode = doc["Settings"]["LiveSplitServer"];
            AutoStart = bool.Parse(liveSplitServerNode["AutoStart"].InnerText);
            AutoReset = bool.Parse(liveSplitServerNode["AutoReset"].InnerText);
            Port = int.Parse(liveSplitServerNode["Port"].InnerText);
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

        private void ColorButtonClick(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var picker = new ColorPickerDialog();
            picker.SelectedColorChanged += (s, x) => button.BackColor = Color.FromArgb(picker.SelectedColor.R, picker.SelectedColor.G, picker.SelectedColor.B);
            picker.SelectedColor = picker.OldColor = button.BackColor;
            picker.ShowDialog(this);
            button.BackColor = Color.FromArgb(picker.SelectedColor.R, picker.SelectedColor.G, picker.SelectedColor.B);
        }
    }
}
