﻿extern alias LiveSplitServer;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
        private Task _launchDLIGTTask;

        public DyingLightIGTComponent(LiveSplitState state)
        {
            bool debug = false;
#if DEBUG
            debug = true;
#endif
            Trace.WriteLine("[NoLoads] Using LiveSplit.DyingLightIGT component version " + Assembly.GetExecutingAssembly().GetName().Version + " " + ((debug) ? "Debug" : "Release") + " build");
            _state = state;
            _state.OnStart += State_OnStart;

            if((_server = CreateServerComponent()) == null)
                MessageBox.Show("LiveSplit.Server.dll is missing.\nDownload it at http://livesplit.org/components/", "LiveSplit.DyingLightIGT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            this.Settings = new DyingLightIGTSettings(state, _server);

            if (_server != null)
                this.ContextMenuControls = _server.ContextMenuControls;
        }

        private IComponent CreateServerComponent()
        {
            IComponentFactory serverFactory;

            if (ComponentManager.ComponentFactories.TryGetValue("LiveSplit.Server.dll", out serverFactory))
                return serverFactory.Create(_state);
            else
                return null;
        }

        private void LaunchDyingLightIGT()
        {
            if (!Environment.Is64BitOperatingSystem)
            {
                MessageBox.Show("Dying Light IGT requires a 64-bit OS.", "LiveSplit.DyingLightIGT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (File.Exists(@"Components\DyingLightIGT\DyingLightIGT.exe"))
            {
                string optionalArgs = String.Empty;
                var server = (ServerComponent)_server;

                if (server != null)
                {
                    if (Settings.ServerAutoStart)
                        StartServer();

                    optionalArgs += " -port " + server.Settings.Port;
                }
                                
                if (Settings.NoGUI)
                {
                    optionalArgs += " -nogui";
                }

                _dyingLightIGT = Process.Start(new ProcessStartInfo()
                {
                    FileName = @"Components\DyingLightIGT\DyingLightIGT.exe",
                    Arguments = "-livesplit -launcherid " + Process.GetCurrentProcess().Id + optionalArgs
                });
            }
            else
                MessageBox.Show("DyingLightIGT.exe is missing.\nPlease reinstall Dying Light IGT.", "LiveSplit.DyingLightIGT", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        void StartServer()
        {
            var server = (ServerComponent)_server;
            
            try
            {
                server.Start();
            }
            catch (System.Net.Sockets.SocketException e)
            {
                string message;
                switch (e.SocketErrorCode)
                {
                    case System.Net.Sockets.SocketError.AddressAlreadyInUse:
                        message = "Port " + server.Settings.Port + " is already in use.";
                        break;
                    default:
                        message = "An error occurred while trying to start the server.\nError code: " + e.SocketErrorCode;
                        break;
                }

                if (DialogResult.Retry == MessageBox.Show(message, "LiveSplit.Server", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning))
                    StartServer();
            }
        }

        void State_OnStart(object sender, EventArgs e)
        {
            if (_dyingLightIGT != null && !_dyingLightIGT.HasExited)
                _state.IsGameTimePaused = true;
        }

        public override void Dispose()
        {
            _state.OnStart -= State_OnStart;

            _launchDLIGTTask.Dispose();

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
                if (_launchDLIGTTask == null)
                    _launchDLIGTTask = Task.Factory.StartNew(LaunchDyingLightIGT);
            }
        }
    }
}
