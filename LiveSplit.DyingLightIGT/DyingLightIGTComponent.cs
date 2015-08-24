using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.DyingLightIGT
{
    public class DyingLightIGTComponent : LogicComponent
    {
        public override string ComponentName => "Dying Light";

        public DyingLightIGTSettings Settings { get; set; }

        private LiveSplitState _state;
        private GameMemory _gameMemory;
        private TimerModel _timer;

        public DyingLightIGTComponent(LiveSplitState state)
        {
            bool debug = false;
#if DEBUG
            debug = true;
#endif
            Trace.WriteLine("[NoLoads] Using LiveSplit.DyingLightIGT component version " + Assembly.GetExecutingAssembly().GetName().Version + " " + ((debug) ? "Debug" : "Release") + " build");

            this.Settings = new DyingLightIGTSettings();

            _timer = new TimerModel { CurrentState = state };
            _state = state;

            _state.OnStart += State_OnStart;

            _gameMemory = new GameMemory();
            _gameMemory.OnTick += gameMemory_OnTick;
            _gameMemory.OnStart += gameMemory_OnStart;
            _gameMemory.OnReset += gameMemory_OnReset;
            _gameMemory.StartMonitoring();
        }

        void State_OnStart(object sender, EventArgs e)
        {
            _timer.InitializeGameTime();
            _state.IsGameTimePaused = true;
        }

        void gameMemory_OnStart(object sender, EventArgs e)
        {
            if (Settings.AutoStart)
                _timer.Start();
        }

        void gameMemory_OnReset(object sender, EventArgs e)
        {
            if (Settings.AutoReset)
                _timer.Reset();
        }

        void gameMemory_OnTick(object sender, float time)
        {
            TimeSpan ts = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond * time));
            _state.SetGameTime(ts);
        }

        public override void Dispose()
        {
            _state.OnStart -= State_OnStart;
            _gameMemory?.Stop();
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public override Control GetSettingsControl(LayoutMode mode)
        {
            return this.Settings;
        }

        public override void SetSettings(XmlNode settings)
        {
            this.Settings.SetSettings(settings);
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { }
    }
}
