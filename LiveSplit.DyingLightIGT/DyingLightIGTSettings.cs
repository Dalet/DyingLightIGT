﻿extern alias LiveSplitServer;

using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using ServerComponent = LiveSplitServer::LiveSplit.UI.Components.Component;

namespace LiveSplit.DyingLightIGT
{
    public partial class DyingLightIGTSettings : UserControl
    {

        LiveSplitState _state;
        IComponent _server;

        public bool ServerAutoStart { get; set; }
        public bool NoGUI { get; set; }
        public bool AutoStart { get; set; }
        public bool AutoReset { get; set; }

        public const bool DEFAULT_SERVERAUTOSTART = true;
        public const bool DEFAULT_NOGUI = true;
        public const bool DEFAULT_AUTOSTART = true;
        public const bool DEFAULT_AUTORESET = true;

        public DyingLightIGTSettings(LiveSplitState state, IComponent server)
        {
            InitializeComponent();

            _state = state;
            _server = server;

            this.ServerAutoStart = DEFAULT_SERVERAUTOSTART;

            this.chkServerAutoStart.DataBindings.Add("Checked", this, "ServerAutoStart", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkNoGUI.DataBindings.Add("Checked", this, "NoGUI", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoStart.DataBindings.Add("Checked", this, "AutoStart", false, DataSourceUpdateMode.OnPropertyChanged);
            this.chkAutoReset.DataBindings.Add("Checked", this, "AutoReset", false, DataSourceUpdateMode.OnPropertyChanged);

            this.Load += DyingLightIGTSettings_Load;
        }

        void DyingLightIGTSettings_Load(object sender, EventArgs e)
        {
            tabs.Controls.Clear();
            tabs.Controls.Add(tabGeneral);
            if (_server != null)
                AddNewTab("LiveSplit Server", _server.GetSettingsControl(_state.Layout.Mode));
        }

        private void AddNewTab(string name, Control control)
        {
            var page = new TabPage(name);
            control.Location = new Point(0, 0);
            page.Controls.Add(control);
            page.AutoScroll = true;
            page.Name = name;
            tabs.TabPages.Add(page);
        }

        public XmlNode GetSettings(XmlDocument doc)
        {
            XmlElement settingsNode = doc.CreateElement("Settings");

            settingsNode.AppendChild(ToElement(doc, "Version", Assembly.GetExecutingAssembly().GetName().Version.ToString(3)));
            var generalNode = settingsNode.AppendChild(doc.CreateElement("General"));

            generalNode.AppendChild(ToElement(doc, "ServerAutoStart", ServerAutoStart));
            generalNode.AppendChild(ToElement(doc, "NoGUI", NoGUI));
            generalNode.AppendChild(ToElement(doc, "AutoStart", AutoStart));
            generalNode.AppendChild(ToElement(doc, "AutoReset", AutoReset));

            if (_server != null)
            {
                XmlElement serverSettings = (XmlElement)_server.GetSettings(doc);
                settingsNode.AppendChild(serverSettings);
                RenameElement(serverSettings, "LiveSplit.Server");
            }

            return settingsNode;
        }

        public void SetSettings(XmlNode settings)
        {
            var element = (XmlElement)settings;
            var generalNode = settings["General"];
            var serverSettingsNode = settings["LiveSplit.Server"];

            this.ServerAutoStart = ParseBool(generalNode, "ServerAutoStart", DEFAULT_SERVERAUTOSTART);
            this.NoGUI = ParseBool(generalNode, "NoGUI", DEFAULT_NOGUI);
            this.AutoStart = ParseBool(generalNode, "AutoStart", DEFAULT_AUTOSTART);
            this.AutoReset = ParseBool(generalNode, "AutoReset", DEFAULT_AUTORESET);

            if (_server != null && serverSettingsNode != null)
            {
                _server.SetSettings(serverSettingsNode);
            }
        }

        static bool ParseBool(XmlNode settings, string setting, bool default_ = false)
        {
            bool val;
            return settings[setting] != null ?
                (Boolean.TryParse(settings[setting].InnerText, out val) ? val : default_)
                : default_;
        }

        static XmlElement ToElement<T>(XmlDocument document, string name, T value)
        {
            XmlElement str = document.CreateElement(name);
            str.InnerText = value.ToString();
            return str;
        }

        static XmlElement RenameElement(XmlElement e, string newName)
        {
            XmlDocument doc = e.OwnerDocument;
            XmlElement newElement = doc.CreateElement(newName);
            while (e.HasChildNodes)
            {
                newElement.AppendChild(e.FirstChild);
            }
            XmlAttributeCollection ac = e.Attributes;
            while (ac.Count > 0)
            {
                newElement.Attributes.Append(ac[0]);
            }
            XmlNode parent = e.ParentNode;
            parent.ReplaceChild(newElement, e);
            return newElement;
        }

        private void chkNoGUI_CheckedChanged(object sender, EventArgs e)
        {
            this.chkAutoStart.Visible = chkNoGUI.Checked;
            this.chkAutoReset.Visible = chkNoGUI.Checked;
        }
    }
}
