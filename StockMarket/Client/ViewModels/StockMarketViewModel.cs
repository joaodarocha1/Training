using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using StockMarket.Service.Common;

namespace StockMarket.Client.ViewModels
{
    internal class StockMarketViewModel : BindableBase, IDisposable
    {
        private readonly IMarketDataService _marketDataServices;
        private readonly IMapper _mapper;
        private readonly IDialogService _dialogService;

        public StockMarketViewModel(IMarketDataService marketDataServices,  IMapper mapper, IDialogService dialogService)
        {
            _marketDataServices = marketDataServices;
            _mapper = mapper;
            _dialogService = dialogService;

            _marketDataServices.Tick += OnTick;
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public StockViewModel SelectedStock 
        {
            get => _selectedStock;
            set => SetProperty(ref _selectedStock, value);
        }

        public ObservableCollection<StockViewModel> Stocks { get; set; } = new();

        private DelegateCommand? _loadCommand;
        private DelegateCommand? _showPriceHistoryCommand;
        private bool _isLoading;
        private StockViewModel _selectedStock;

        public DelegateCommand LoadCommand =>
            _loadCommand ??= new DelegateCommand(CommandLoadExecute);
        public DelegateCommand ShowPriceHistoryCommand =>
            _showPriceHistoryCommand ??= new DelegateCommand(ShowPriceHistoryExecute);

        private void ShowPriceHistoryExecute()
        {
            if(SelectedStock == null || string.IsNullOrEmpty(SelectedStock.Ticker)) return;

            var parameters = new DialogParameters
            {
                { "ticker", SelectedStock.Ticker },
                { "name", SelectedStock.Name }
            };

            _dialogService.ShowDialog("PriceHistoryDialog", parameters, r => {});
         }

        private void CommandLoadExecute()
        {
            Stocks.Clear();
            LoadStocksAsync();
        }


        private async void LoadStocksAsync()
        {
            IsLoading = true;
            
            var portFolio = await GetPortfolioAsync();

            _marketDataServices.Subscribe(portFolio.Select(s => s.Ticker));

            foreach (var stockItem in portFolio)
            {
                Stocks.Add(_mapper.Map<StockViewModel>(stockItem));
            }

            IsLoading = false;
        }

        private async Task<IEnumerable<Stock>> GetPortfolioAsync()
        {
            await Task.Delay(2000);

            var stockData = new List<Stock>
            {
                new() { Ticker = "STK1", Name = "Stock 1" },
                new() { Ticker = "STK2", Name = "Stock 2" },
            };

            return stockData;
        }

        private void OnTick(object? sender, TickEventArgs e)
        {
            foreach (var eQuote in e.Quotes)
            {
                var stock = Stocks.SingleOrDefault(s => s.Ticker == eQuote.Ticker);

                if (stock == null) continue;
                _mapper.Map(eQuote, stock);

            }
        }

        public void Dispose()
        {
            _marketDataServices.Tick -= OnTick;
            _marketDataServices.Unsubscribe();
        }
    }
}
