using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using StockMarket.Domain;
using StockMarket.Service;

namespace StockMarket.Client.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
        private readonly IMarketDataService _marketDataServices;
        private readonly IPortfolioService _portfolioService;

        public MainWindowViewModel(IMarketDataService marketDataServices, IPortfolioService portfolioService)
        {
            _marketDataServices = marketDataServices;
            _portfolioService = portfolioService;
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ObservableCollection<StockViewModel> Stocks { get; set; } = new();

        private DelegateCommand? _loadCommand;
        private bool _isLoading;

        public DelegateCommand LoadCommand =>
            _loadCommand ??= new DelegateCommand(CommandLoadExecute);

        private void CommandLoadExecute()
        {

            Stocks.Clear();
            LoadStocksAsync();
        }


        private async void LoadStocksAsync()
        {
            IsLoading = true;

            await Task.Delay(2000);

            var portFolio = _portfolioService.GetPortfolio(1);

            _marketDataServices.Subscribe(portFolio.Select(s => s.Ticker));
            _marketDataServices.Tick += OnTick;

            // Populate the Stocks collection with the retrieved data
            foreach (var stockItem in portFolio)
            {
                Stocks.Add(new StockViewModel()
                {
                    Name = stockItem.Name,
                    Ticker = stockItem.Ticker
                });
            }

            IsLoading = false;
        }

        private void OnTick(object? sender, TickEventArgs e)
        {
            foreach (var eQuote in e.Quotes)
            {
                var stock = Stocks.SingleOrDefault(s => s.Ticker == eQuote.Ticker);

                if (stock == null) continue;

                stock.DateTime = eQuote.DateTime;
                stock.Price = eQuote.Price;
                stock.Movement = eQuote.Movement;

            }
        }
    }
}
