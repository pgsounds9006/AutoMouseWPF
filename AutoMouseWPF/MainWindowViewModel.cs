using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Gma.System.MouseKeyHook;
using System.Windows.Forms;

namespace AutoMouseWPF
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private IKeyboardMouseEvents _globalHook;

        [ObservableProperty]
        private string? _startShortcutText = "";

        [ObservableProperty]
        private string? _stopShortcutText = "";

        [ObservableProperty]
        private int? _delayValue;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RunningStatus))]
        private bool _isRunning = false;

        [ObservableProperty]
        private bool _leftMouseButton = true;

        [ObservableProperty]
        private bool _rightMouseButton = false;

        private List<Key> _pressedKeys = new();

        public string RunningStatus => IsRunning ? "동작 중" : "정지";
        public ICommand StartShorcutChangedCommand { get; }
        public ICommand StopShorcutChangedCommand { get; }
        public ICommand DelayChangedCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }


        public List<Key> StartShortcut { get; set; } = new();
        public List<Key> StopShortcut { get; set; } = new();

        public MainWindowViewModel()
        {
            _globalHook = Hook.GlobalEvents();
            _globalHook.KeyDown += GlobalHook_KeyDown;
            _globalHook.KeyUp += GlobalHook_KeyUp;

            DelayChangedCommand = new RelayCommand<int>(DelayValueChanged);
            StartShorcutChangedCommand = new RelayCommand<List<Key>>(StartShortcutChanged);
            StopShorcutChangedCommand = new RelayCommand<List<Key>>(StopShortcutChanged);

            StartCommand = new RelayCommand(Start);
            StopCommand = new RelayCommand(Stop);

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

        void Start()
        {
            if (DelayValue is null) return;
            if (IsRunning) return;

            IsRunning = true;
            Task.Run(Work);
        }

        void Stop()
        {
            IsRunning = false;
        }

        private void Work()
        {
            while (IsRunning)
            {
                ClickSender.SendClick(LeftMouseButton ? MouseButton.Left : MouseButton.Right);
                System.Threading.Thread.Sleep((int)DelayValue!);
            }
        }

        private void DelayValueChanged(int value)
        {
            DelayValue = value;
        }

        private void StartShortcutChanged(List<Key>? values)
        {
            if (values == null) return;
            StartShortcut = values;
            StartShortcutText = string.Join("+", values);
        }
        private void StopShortcutChanged(List<Key>? values)
        {
            if (values == null) return;
            StopShortcut = values;
            StopShortcutText = string.Join("+", values);
        }

    }
}
