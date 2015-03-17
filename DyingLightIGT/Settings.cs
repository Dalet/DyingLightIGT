using System;
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

        const string configFileName = "DyingLightIGT.cfg";
        XmlDocument doc;

        public Settings()
        {
            InitializeComponent();
            this.FormClosing += Form_FormClosing;
            this.Icon = Properties.Resources.DyingLightGame_161;

            CheckUpdates = true;
            SERVER_IP = "127.0.0.1";
            Port = 16834;
            AutoStart = true;
            AutoReset = true;

            InitializeConfigFile();
            
            this.chkCheckUpdates.DataBindings.Add("Checked", this, "CheckUpdates", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoStart.DataBindings.Add("Checked", this, "AutoStart", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoReset.DataBindings.Add("Checked", this, "AutoReset", false, DataSourceUpdateMode.OnPropertyChanged);
            this.numPort.DataBindings.Add("Value", this, "Port", false, DataSourceUpdateMode.OnPropertyChanged);
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

            XmlNode liveSplitServerNode = doc["Settings"]["LiveSplitServer"];
            AutoStart = bool.Parse(liveSplitServerNode["AutoStart"].InnerText);
            AutoReset = bool.Parse(liveSplitServerNode["AutoReset"].InnerText);
            Port = int.Parse(liveSplitServerNode["Port"].InnerText);
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            SaveSettings();
            this.Hide();
        }

        static XmlElement ToElement<T>(XmlDocument document, string name, T value)
        {
            XmlElement str = document.CreateElement(name);
            str.InnerText = value.ToString();
            return str;
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
    }
}
