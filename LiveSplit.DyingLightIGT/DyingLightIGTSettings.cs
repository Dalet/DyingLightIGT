using LiveSplit.UI;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.DyingLightIGT
{
    public partial class DyingLightIGTSettings : UserControl
    {
        public bool AutoStart { get; set; }
        public bool AutoReset { get; set; }

        const bool DEFAULT_AUTOSTART = true;
        const bool DEFAULT_AUTORESET = true;

        public DyingLightIGTSettings()
        {
            InitializeComponent();

            this.AutoStart = chkAutoStart.Checked = DEFAULT_AUTOSTART;
            this.AutoReset = chkAutoReset.Checked = DEFAULT_AUTORESET;

            this.chkAutoStart.DataBindings.Add("Checked", this, "AutoStart", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoReset.DataBindings.Add("Checked", this, "AutoReset", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public XmlNode GetSettings(XmlDocument doc)
        {
            XmlElement settingsNode = doc.CreateElement("Settings");

            settingsNode.AppendChild(SettingsHelper.ToElement(doc, "Version", Assembly.GetExecutingAssembly().GetName().Version.ToString(3)));
            settingsNode.AppendChild(SettingsHelper.ToElement(doc, "AutoStart", AutoStart));
            settingsNode.AppendChild(SettingsHelper.ToElement(doc, "AutoReset", AutoReset));

            return settingsNode;
        }

        public void SetSettings(XmlNode settings)
        {
            var element = (XmlElement)settings;

            this.AutoStart = SettingsHelper.ParseBool(settings["AutoStart"], DEFAULT_AUTOSTART);
            this.AutoReset = SettingsHelper.ParseBool(settings["AutoReset"], DEFAULT_AUTORESET);
        }
    }
}
