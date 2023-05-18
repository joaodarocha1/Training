using AutoMapper;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using StockMarket.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StockMarket.Client.ViewModels
{
    class PriceHistoryViewModel : BindableBase, IDialogAware
    {
        private readonly IMarketDataService _marketDataService;
        private readonly IMapper _mapper;
        private bool _isLoading;
        private string _ticker;

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

            var history = _marketDataService.GetPriceHistory(_ticker, DateTime.Now, DateTime.Now);

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
           // throw new NotImplementedException();
        }

        public void OnDialogClosed()
        {
            _marketDataService.Tick -= OnMarketDataTick;
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _ticker = parameters.GetValue<string>("ticker");
        }

        public string Title { get => "Price History"; }
        public event Action<IDialogResult>? RequestClose;
    }
}
