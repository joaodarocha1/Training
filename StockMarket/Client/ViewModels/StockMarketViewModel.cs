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
using StockMarket.Service.Common.Event;
using StockMarket.Service.Common.Services;

namespace StockMarket.Client.ViewModels
{
    public class StockMarketViewModel : BindableBase, IDisposable
    {
        #region Fields

        private readonly IMarketDataService _marketDataServices;
        private readonly IMapper _mapper;
        private readonly IDialogService _dialogService;

        private DelegateCommand? _loadCommand;
        private DelegateCommand? _showPriceHistoryCommand;
        private bool _isLoading;
        private StockViewModel _selectedStock;

        #endregion

        #region Constructors

        public StockMarketViewModel(IMarketDataService marketDataServices, IMapper mapper, IDialogService dialogService)
        {
            _marketDataServices = marketDataServices;
            _mapper = mapper;
            _dialogService = dialogService;

            _marketDataServices.Tick += OnTick;
        }

        #endregion

        #region Properties

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

        public DelegateCommand LoadCommand =>
            _loadCommand ??= new DelegateCommand(CommandLoadExecute);

        public DelegateCommand ShowPriceHistoryCommand =>
            _showPriceHistoryCommand ??= new DelegateCommand(ShowPriceHistoryExecute);

        #endregion

        #region Commands

        private void ShowPriceHistoryExecute()
        {
            if (SelectedStock == null || string.IsNullOrEmpty(SelectedStock.Ticker)) return;

            var parameters = new DialogParameters
            {
                { "ticker", SelectedStock.Ticker },
                { "name", SelectedStock.Name }
            };

            _dialogService.ShowDialog("PriceHistoryDialog", parameters, _ => { });
        }

        private void CommandLoadExecute()
        {
            Stocks.Clear();
            LoadStocksAsync();
        }

        #endregion

        #region Methods

        private async void LoadStocksAsync()
        {
            IsLoading = true;

            var portFolio = await GetPortfolioAsync();

            await _marketDataServices.SubscribeAsync(portFolio.Select(s => s.Ticker));

            foreach (var stockItem in portFolio)
            {
                Stocks.Add(_mapper.Map<StockViewModel>(stockItem));
            }

            IsLoading = false;
        }

        private async Task<IEnumerable<Stock>> GetPortfolioAsync()
        {
            await Task.Delay(2000);
            
            return new List<Stock>
            {
                new() { Ticker = "STK1", Name = "Stock 1" },
                new() { Ticker = "STK2", Name = "Stock 2" },
            }; ;
        }

        private void OnTick(object? sender, TickEventArgs e)
        {
            foreach (var eQuote in e.Quotes.Where(w => w != null))
            {
                var stock = Stocks.SingleOrDefault(s => s.Ticker == eQuote.Ticker);

                if (stock == null) continue;
                _mapper.Map(eQuote, stock);
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            _marketDataServices.Tick -= OnTick;
            _marketDataServices.Unsubscribe();
        }

        #endregion
    }
}

