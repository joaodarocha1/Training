using System.Collections.ObjectModel;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Prism.Services.Dialogs;
using StockMarket.Client.Utils;
using StockMarket.Client.ViewModels;
using StockMarket.Service.Bloomberg;
using StockMarket.Service.Bloomberg.Publisher;
using StockMarket.Service.Common;
using StockMarket.Service.Common.Enums;

namespace StockMarket.Client.Test
{
    public class PriceHistoryViewModelTests: BaseViewModelTests
    {

        public PriceHistoryViewModelTests()
        {
        }

        [Fact]
        public void InitializationTest()
        {
            var viewModel = new PriceHistoryViewModel(DataService, Mapper, DispatcherServiceMock.Object, Logger.Object);

            using (new AssertionScope())
            {
                viewModel.IsLoading.Should().BeTrue();
                viewModel.PriceHistory.Should().NotBeNull();
                viewModel.PriceHistoryView.Should().NotBeNull();
            }
        }

        [Fact]
        public void Initial_Parameters_Must_Be_Set()
        {
            var viewModel = new PriceHistoryViewModel(DataService, Mapper, DispatcherServiceMock.Object, Logger.Object);
            var parameters = new DialogParameters
            {
                { "ticker", "STK1" },
                { "name", "Stock 1" }
            };

            viewModel.OnDialogOpened(parameters);

            viewModel.Ticker.Should().Be("STK1");
            viewModel.Name.Should().Be("Stock 1");
        }

        [Fact]
        public async Task On_Tick_Must_Update_History()
        {
            var viewModelTicker = "STK1";

            DataService.SubscribeAsync(new [] { viewModelTicker });

            var viewModel = new PriceHistoryViewModel(DataService, Mapper, DispatcherServiceMock.Object, Logger.Object)
                {
                    Ticker = viewModelTicker,
                    PriceHistory = new ObservableCollection<QuoteViewModel>()
                };

            var quote = new Quote { DateTime = DateTime.Now, Price = 100, Ticker = viewModelTicker, Movement = MovementType.None};

            Publisher.Raise(p => p.Publish += null, this, new RandomPublishEventArgs() { Quote = quote });

            await Task.Delay(1500);
            
            viewModel.PriceHistory.Should().NotBeNullOrEmpty();
            viewModel.IsLoading.Should().BeFalse();
        }
    }
}