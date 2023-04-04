using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMouseWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;
        public MainWindow()
        {
            DataContext = App.Current.Services.GetService<MainWindowViewModel>();
            InitializeComponent();
        }
    }
}
