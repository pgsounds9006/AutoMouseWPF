using System;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AutoMouseWPF.Services;

namespace AutoMouseWPF
{
    public partial class MainWindowViewModel : ObservableObject
    {

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


        public string RunningStatus => IsRunning ? "동작 중" : "정지";
        public ICommand StartShortcutChangedCommand { get; }
        public ICommand StopShortcutChangedCommand { get; }
        public ICommand DelayChangedCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }


        public List<Key> StartShortcut { get; set; } = new();
        public List<Key> StopShortcut { get; set; } = new();
        private readonly IMacroManager _macroManager;
        public MainWindowViewModel(IMacroManager macroManager)
        {
            _macroManager = macroManager;

            DelayChangedCommand = new RelayCommand<int>(DelayValueChanged);
            StartShortcutChangedCommand = new RelayCommand<List<Key>>(StartShortcutChanged);
            StopShortcutChangedCommand = new RelayCommand<List<Key>>(StopShortcutChanged);

            StartCommand = new RelayCommand(Start);
            StopCommand = new RelayCommand(Stop);

            _macroManager.RunningStateChanged += OnRunningStateChanged;
        }

        private void OnRunningStateChanged(object? sender, EventArgs e)
        {
            IsRunning = _macroManager.IsRunning;
        }

        void Start()
        {
            _macroManager.Start();
        }

        void Stop()
        {
            _macroManager.Stop();
        }

        private void DelayValueChanged(int value)
        {
            DelayValue = value;
            _macroManager.Interval = value;
        }


        private void StartShortcutChanged(List<Key>? values)
        {
            if (values == null) return;
            _macroManager.StartShortcut = values;
            StartShortcut = values;
            StartShortcutText = string.Join("+", values);
        }
        private void StopShortcutChanged(List<Key>? values)
        {
            if (values == null) return;
            _macroManager.StopShortcut = values;
            StopShortcut = values;
            StopShortcutText = string.Join("+", values);
        }

    }
}
