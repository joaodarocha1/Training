using AutoMapper;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Serilog;
using StockMarket.Client.Utils;
using StockMarket.Service.Common.Event;
using StockMarket.Service.Common.Services;

namespace StockMarket.Client.ViewModels
{
    public class PriceHistoryViewModel : BindableBase, IDialogAware
    {
        #region Fields

        private readonly IMarketDataService _marketDataService;
        private readonly IMapper _mapper;
        private readonly IDispatcherService _dispatcherService;
        private readonly ILogger _logger;
        private bool _isLoading;
        private string _name;
        private string _ticker;

        #endregion

        #region Properties

        public string Title => "Price History";

        public string Ticker
        {
            get => _ticker;
            set => SetProperty(ref _ticker, value);
        }


        public string Name  
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        public ObservableCollection<QuoteViewModel> PriceHistory { get; set; } = new();
        public ICollectionView PriceHistoryView { get; set; }

        #endregion

        #region Events

        public event Action<IDialogResult>? RequestClose;

        #endregion

        #region Methods
        public PriceHistoryViewModel(IMarketDataService marketDataService, IMapper mapper, IDispatcherService dispatcherService, ILogger logger)
        {
            logger.Debug("PriceHistoryViewModel Initialization started");

            IsLoading = true;

            _marketDataService = marketDataService;
            _marketDataService.Tick += OnMarketDataTick;
            _mapper = mapper;
            _dispatcherService = dispatcherService;
            _logger = logger;

            PriceHistoryView = CollectionViewSource.GetDefaultView(PriceHistory);
            PriceHistoryView.SortDescriptions.Add(new SortDescription("DateTime", ListSortDirection.Descending));

            logger.Debug("PriceHistoryViewModel Initialization completed");
        }
        
        private void OnMarketDataTick(object? sender, TickEventArgs e)
        {
            IsLoading = true;

            try
            {
                var history = _marketDataService.GetPriceHistory(Ticker);

                foreach (var quote in history.OrderByDescending(o => o.DateTime))
                {
                    var quoteViewModel = _mapper.Map<QuoteViewModel>(quote);

                    if (PriceHistory.Contains(quoteViewModel)) break;

                    _dispatcherService.Invoke(() => { PriceHistory.Add(quoteViewModel); });
                }

            }
            catch (Exception exception)
            {
                _logger.Error("PriceHistoryViewModel.OnMarketDataTick", exception);
                IsLoading = false;
                throw;
            }

            IsLoading = false;
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            _marketDataService.Tick -= OnMarketDataTick;
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Ticker = parameters.GetValue<string>("ticker");
            Name = parameters.GetValue<string>("name");
        }
        #endregion
    }
}
