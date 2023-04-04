using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Gma.System.MouseKeyHook;

namespace AutoMouseWPF.Services
{
    public interface IMacroManager
    {
        List<Key> StartShortcut { get; set; }
        List<Key> StopShortcut { get; set; }
        int? Interval { get; set; }
        bool IsRunning { get; }
        MouseButton SelectedButton { get; set; }
        void Start();
        void Stop();
        EventHandler? RunningStateChanged { get; set; }
    }
    
    public class MacroManager : IMacroManager
    {
        private IKeyboardMouseEvents _globalHook;
        private List<Key> _pressedKeys = new();

        public List<Key> StartShortcut { get; set; } = new();
        public List<Key> StopShortcut { get; set; } = new();
        public int? Interval { get; set; }
        public bool IsRunning { get; private set; } = false;
        public MouseButton SelectedButton { get; set; } = MouseButton.Left;
        public Action? Do { get; }
        public EventHandler? RunningStateChanged { get; set; }

        public MacroManager()
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += GlobalHook_KeyDown;
            _globalHook.KeyUp += GlobalHook_KeyUp;

            Do = () =>
            {
                ClickSender.SendClick(SelectedButton);
            };

        }

        public void Start()
        {
            if (IsRunning) return;
            if (Interval is null or < 1) return;

            IsRunning = true;
            RunningStateChanged?.Invoke(this, EventArgs.Empty);

            Task.Run(async () =>
            {
                while (IsRunning)
                {
                    Do?.Invoke();
                    await Task.Delay(Interval.Value);
                }
            });
        }
            
        public void Stop()
        {
            IsRunning = false;
            RunningStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void GlobalHook_KeyUp(object? sender, System.Windows.Forms.KeyEventArgs e)
        {
            _pressedKeys.Clear();
        }

        private void GlobalHook_KeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (StartShortcut.Count > 0 && StopShortcut.Count > 0)
            {
                _pressedKeys.Add(KeyInterop.KeyFromVirtualKey(e.KeyValue));
                if (StartShortcut.All(_pressedKeys.Contains))
                    Start();

                if (StopShortcut.All(_pressedKeys.Contains))
                    Stop();
            }
        }
    }
}
