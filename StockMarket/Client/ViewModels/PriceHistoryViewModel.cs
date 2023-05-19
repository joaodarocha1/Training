using AutoMapper;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using StockMarket.Service.Common;

namespace StockMarket.Client.ViewModels
{
    class PriceHistoryViewModel : BindableBase, IDialogAware
    {
        private readonly IMarketDataService _marketDataService;
        private readonly IMapper _mapper;
        private bool _isLoading;

        #region Properties

        public string Title => "Price History";

        public string Ticker
        {
            get => _ticker;
            set => SetProperty(ref _ticker, value);
        }

        private string _name;
        private string _ticker;

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

        #endregion

        public ObservableCollection<QuoteViewModel> PriceHistory { get; set; } = new();

        public PriceHistoryViewModel(IMarketDataService marketDataService, IMapper mapper)
        {
            IsLoading = true;

            _marketDataService = marketDataService;
            _marketDataService.Tick += OnMarketDataTick;
            _mapper = mapper;

            PriceHistoryView = CollectionViewSource.GetDefaultView(PriceHistory);
            PriceHistoryView.SortDescriptions.Add(new SortDescription("DateTime", ListSortDirection.Descending));
        }

        public ICollectionView PriceHistoryView { get; set; }

        private void OnMarketDataTick(object? sender, TickEventArgs e)
        {
            IsLoading = true;
            
            var history = _marketDataService.GetPriceHistory(Ticker);

            foreach (var quote in history.OrderByDescending(o => o.DateTime))
            {
                var quoteViewModel = _mapper.Map<QuoteViewModel>(quote);

                if (PriceHistory.Contains(quoteViewModel)) break;

                Application.Current.Dispatcher.Invoke(() => { PriceHistory.Add(quoteViewModel); });
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

        public event Action<IDialogResult>? RequestClose;
    }
}
