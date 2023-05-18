using AutoMapper;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using StockMarket.Domain;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace StockMarket.Client.ViewModels
{
    class PriceHistoryViewModel : BindableBase, IDialogAware
    {
        private readonly IMarketDataService _marketDataService;
        private readonly IMapper _mapper;
        private bool _isLoading;

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
        public PriceHistoryViewModel(IMarketDataService marketDataService, IMapper mapper)
        {
            _marketDataService = marketDataService;
            _marketDataService.Tick += OnMarketDataTick;
            _mapper = mapper;
        }

        private void OnMarketDataTick(object? sender, TickEventArgs e)
        {
            IsLoading = true;

            Application.Current.Dispatcher.Invoke(() =>
            {
                PriceHistory.Clear();
            });

            var history = _marketDataService.GetPriceHistory(Ticker, DateTime.Now, DateTime.Now);

            foreach (var quote in history.OrderByDescending(o => o.DateTime))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PriceHistory.Add(_mapper.Map<QuoteViewModel>(quote));
                });
                
            }

            IsLoading = false;
        }

        public ObservableCollection<QuoteViewModel> PriceHistory { get; set; } = new();

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

        public string Title => "Price History";
        public event Action<IDialogResult>? RequestClose;
    }
}
