using System.ComponentModel;
using System.Configuration;
using System.Windows;
using Microsoft.Practices.Unity.Configuration;
using Prism.Ioc;
using Prism.Unity;
using StockMarket.Client.Views;
using StockMarket.Domain;
using StockMarket.Service;

namespace StockMarket.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            var w = Container.Resolve<MainWindow>();
            
            return w;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IMarketDataService, BloombergDataService>();
            containerRegistry.Register<IPortfolioService, PortfolioService>();
            containerRegistry.Register<IRandomPublisher, RandomPublisher>();
        }
    }
}
