using System;
using System.Windows;
using AutoMapper;
using Prism.Ioc;
using Prism.Unity;
using Serilog;
using Serilog.Core;
using StockMarket.Client.Control;
using StockMarket.Client.Utils;
using StockMarket.Client.ViewModels;
using StockMarket.Client.Views;
using StockMarket.Service;
using StockMarket.Service.Bloomberg;
using StockMarket.Service.Bloomberg.Publisher;
using StockMarket.Service.Common;
using Unity;

namespace StockMarket.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private static ILogger _logger;

        protected override Window CreateShell()
        {
            return Container.Resolve<StockMarketView>();

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterScoped<IMarketDataService, MarketDataService>();
            containerRegistry.Register<IRandomPublisher, RandomPublisher>();
            containerRegistry.RegisterDialog<PriceHistoryDialog, PriceHistoryViewModel>();
            containerRegistry.Register<IDispatcherService, DispatcherService>();

            AddMapper(containerRegistry);
            AddLogger(containerRegistry);
        }

        private static void AddLogger(IContainerRegistry containerRegistry)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() 
                .WriteTo
                .File($"Logs/StockMarket.log",
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            containerRegistry.RegisterInstance<ILogger>(Log.Logger);
            _logger = containerRegistry.GetContainer().Resolve<ILogger>();
        }

        private static void AddMapper(IContainerRegistry containerRegistry)
        {
            var configuration = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });

            var mapper = configuration.CreateMapper();
            containerRegistry.RegisterInstance(mapper);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _logger.Information("Application exited");
            Log.CloseAndFlush();
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _logger.Information("Application started");
        }
    }
}
