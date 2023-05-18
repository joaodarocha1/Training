using System;
using System.ComponentModel;
using System.Windows;

namespace StockMarket.Client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            var viewModel = (IDisposable)this.DataContext;
            viewModel.Dispose();
        }
    }
}
