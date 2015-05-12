using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DyingLightIGT
{
    class GameMemory
    {
        public const int SLEEP_TIME = 15;

        public delegate void OnTickEventHandler(object sender, float time);
        public event OnTickEventHandler OnTick;
        public event EventHandler OnStart;
        public event EventHandler OnReset;

        private Task _thread;
        private CancellationTokenSource _cancelSource;
        private SynchronizationContext _uiThread;
        private List<int> _ignorePIDs;

        private DeepPointer _gameTimePtr;

        private enum ExpectedExeSizes
        {
            steam_1_5_x64 = 1454080,
        }

        public uint frameCounter = 0;

        public GameMemory()
        {
            _gameTimePtr = new DeepPointer("gamedll_x64_rwdi.dll", 0x18B6FE8, 0x518, 0x1f0, 0x8, 0x4b8, 0x3a0);

            _ignorePIDs = new List<int>();
        }

        public void StartMonitoring()
        {
            if (_thread != null && _thread.Status == TaskStatus.Running)
            {
                throw new InvalidOperationException();
            }
            if (!(SynchronizationContext.Current is WindowsFormsSynchronizationContext))
            {
                throw new InvalidOperationException("SynchronizationContext.Current is not a UI thread.");
            }

            _uiThread = SynchronizationContext.Current;
            _cancelSource = new CancellationTokenSource();
            _thread = Task.Factory.StartNew(MemoryReadThread);
        }

        public void Stop()
        {
            if (_cancelSource == null || _thread == null || _thread.Status != TaskStatus.Running)
            {
                return;
            }

            _cancelSource.Cancel();
            _thread.Wait();
        }
        void MemoryReadThread()
        {
            Trace.WriteLine("[NoLoads] MemoryReadThread");

            while (!_cancelSource.IsCancellationRequested)
            {
                try
                {
                    Trace.WriteLine("[NoLoads] Waiting for DyingLightGame.exe...");

                    Process game;
                    while ((game = GetGameProcess()) == null)
                    {
                        Thread.Sleep(250);
                        if (_cancelSource.IsCancellationRequested)
                        {
                            return;
                        }
                    }

                    Trace.WriteLine("[NoLoads] Got DyingLightGame.exe!");

                    frameCounter = 0;
                    float prevGameTime = -1;

                    while (!game.HasExited)
                    {
                        float gameTime;

                        _gameTimePtr.Deref(game, out gameTime);
                        
                        if (gameTime != prevGameTime)
                        {
                            if (gameTime < 1 && gameTime > 0 && prevGameTime != -1)
                            {
                                if (gameTime < prevGameTime)
                                {
                                    _uiThread.Post(d =>
                                    {
                                        if (this.OnReset != null)
                                        {
                                            this.OnReset(this, EventArgs.Empty);
                                        }
                                    }, null);
                                }
                                
                                _uiThread.Post(d =>
                                {
                                    if (this.OnStart != null)
                                    {
                                        this.OnStart(this, EventArgs.Empty);
                                    }
                                }, null);
                            }

                            _uiThread.Post(d =>
                            {
                                if (this.OnTick != null)
                                {
                                    this.OnTick(this, gameTime);
                                }
                            }, null);
                        }

                        frameCounter++;
                        prevGameTime = gameTime;

                        Thread.Sleep(SLEEP_TIME);

                        if (_cancelSource.IsCancellationRequested)
                        {
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                    Thread.Sleep(1000);
                }
            }
        }

        Process GetGameProcess()
        {
            Process game = Process.GetProcesses().FirstOrDefault(p => p.ProcessName == "DyingLightGame"
                && !p.HasExited && !_ignorePIDs.Contains(p.Id));
            if (game == null)
            {
                return null;
            }

            /*if (game.MainModule.ModuleMemorySize != (int)ExpectedExeSizes.steam_1_5_x64)
            {
                _ignorePIDs.Add(game.Id);
                _uiThread.Send(d => MessageBox.Show("Unexpected game version. Version 1.5 x64 is required.\r\n ModuleMemorySize: " + game.MainModule.ModuleMemorySize, "Dying Light IGT",
                    MessageBoxButtons.OK, MessageBoxIcon.Error), null);
                return null;
            }*/

            return game;
        }
    }
}
