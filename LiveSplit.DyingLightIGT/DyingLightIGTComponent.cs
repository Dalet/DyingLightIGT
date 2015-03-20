extern alias LiveSplitServer;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using ServerComponent = LiveSplitServer::LiveSplit.UI.Components.Component;

namespace LiveSplit.DyingLightIGT
{
    public class DyingLightIGTComponent : LogicComponent
    {
        public override string ComponentName
        {
            get { return "Dying Light IGT"; }
        }

        public DyingLightIGTSettings Settings { get; set; }

        private LiveSplitState _state;
        private IComponent _server;
        private Process _dyingLightIGT;

        public DyingLightIGTComponent(LiveSplitState state)
        {
            bool debug = false;
#if DEBUG
            debug = true;
#endif
            Trace.WriteLine("[NoLoads] Using LiveSplit.DyingLightIGT component version " + Assembly.GetExecutingAssembly().GetName().Version + " " + ((debug) ? "Debug" : "Release") + " build");
            _state = state;

            try
            {
                CreateServerComponent();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("LiveSplit.Server.dll is missing.\nDownload it at http://livesplit.org/components/", "LiveSplit.DyingLightIGT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Settings = new DyingLightIGTSettings(state, _server);

            if (_server != null)
            {
                Task.Factory.StartNew(LaunchDyingLightIGT);
                this.ContextMenuControls = _server.ContextMenuControls;
            }
        }

        private void CreateServerComponent()
        {
            IComponentFactory serverFactory;
            ComponentManager.ComponentFactories.TryGetValue("LiveSplit.Server.dll", out serverFactory);
            if (serverFactory != null)
                _server = serverFactory.Create(_state);
        }

        private void LaunchDyingLightIGT()
        {
            if (!Environment.Is64BitOperatingSystem)
            {
                MessageBox.Show("DyingLightIGT requires a 64-bit OS.", "LiveSplit.DyingLightIGT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (File.Exists(@"Components\DyingLightIGT\DyingLightIGT.exe"))
            {
                Thread.Sleep(250);
                var server = (ServerComponent)_server;
                if (Settings.ServerAutoStart)
                    server.Start();

                string args = "-port " + server.Settings.Port;
                _dyingLightIGT = Process.Start(@"Components\DyingLightIGT\DyingLightIGT.exe", args);
            }
            else
                MessageBox.Show("DyingLightIGT.exe is missing.\nPlease reinstall Dying Light IGT.", "LiveSplit.DyingLightIGT", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public override void Dispose()
        {
            if (_server != null)
                ServerDispose();

            if (_dyingLightIGT != null && !_dyingLightIGT.HasExited)
                _dyingLightIGT.CloseMainWindow();
        }

        public void ServerDispose()
        {
            var server = (ServerComponent)_server;

            server.Dispose();
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            if (Settings != null)
                return this.Settings.GetSettings(document);
            else
                return null;
        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return this.Settings;
        }

        public override void SetSettings(XmlNode settings)
        {
            if (Settings != null)
                this.Settings.SetSettings(settings);
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (invalidator != null)
            {
                if (_server != null)
                    _server.Update(invalidator, state, width, height, mode);
            }
        }
    }
}
