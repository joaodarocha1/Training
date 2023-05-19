using System;
using System.ComponentModel;
using System.Windows;

namespace StockMarket.Client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class StockMarketView : Window
    {
        public StockMarketView()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            var viewModel = (IDisposable)DataContext;
            viewModel.Dispose();
        }
    }
}
