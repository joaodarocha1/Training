using System.Windows;
using AutoMapper;
using Prism.Ioc;
using Prism.Unity;
using StockMarket.Client.Control;
using StockMarket.Client.ViewModels;
using StockMarket.Client.Views;
using StockMarket.Domain;
using StockMarket.Service;
using StockMarket.Service.Publisher;

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
            containerRegistry.RegisterScoped<IMarketDataService, BloombergDataService>();
            containerRegistry.Register<IRandomPublisher, RandomPublisher>();

            containerRegistry.RegisterDialog<PriceHistoryDialog, PriceHistoryViewModel>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            IMapper mapper = configuration.CreateMapper();
            containerRegistry.RegisterInstance(mapper);
        }
        
    }
}
