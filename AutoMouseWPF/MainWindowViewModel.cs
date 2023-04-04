using System;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AutoMouseWPF.Services;
using System.Linq;

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
        private MouseButton _selectedButton = MouseButton.Left;


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
            StartShortcutChangedCommand = new RelayCommand<List<Key>>(OnStartShortcutChanged);
            StopShortcutChangedCommand = new RelayCommand<List<Key>>(OnStopShortcutChanged);

            StartCommand = new RelayCommand(Start);
            StopCommand = new RelayCommand(Stop);

            _macroManager.RunningStateChanged += OnRunningStateChanged;
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

        private void OnStartShortcutChanged(List<Key>? values)
        {
            if (values == null) return;
            if (values.All(StopShortcut.Contains)) return;

            _macroManager.StartShortcut = values;
            StartShortcut = values;
            StartShortcutText = string.Join("+", values);
        }
        private void OnStopShortcutChanged(List<Key>? values)
        {
            if (values == null) return;
            if (values.All(StartShortcut.Contains)) return;

            _macroManager.StopShortcut = values;
            StopShortcut = values;
            StopShortcutText = string.Join("+", values);
        }
        partial void OnSelectedButtonChanged(MouseButton value)
        {
            _macroManager.SelectedButton = value;
        }

        private void OnRunningStateChanged(object? sender, EventArgs e)
        {
            IsRunning = _macroManager.IsRunning;
        }
    }
}
