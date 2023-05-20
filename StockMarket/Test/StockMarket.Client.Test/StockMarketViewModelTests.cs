using FluentAssertions;
using FluentAssertions.Execution;
using Prism.Services.Dialogs;
using Prism.Unity;
using StockMarket.Client.ViewModels;
using StockMarket.Service.Bloomberg.Publisher;
using System.Collections.ObjectModel;
using StockMarket.Service;
using StockMarket.Service.Enums;
using StockMarket.Service.Event;

namespace StockMarket.Client.Test;

public class StockMarketViewModelTests : BaseViewModelTests
{
    [Fact]
    private void InitializationTest()
    {
        var viewModel = new StockMarketViewModel(DataService,Mapper,new DialogService(new UnityContainerExtension()), Logger.Object);

        using (new AssertionScope())
        {
            viewModel.IsLoading.Should().BeFalse();
        }
    }

    [Fact]
    private async Task Load_Stocks_Should_Be_Successfully()
    {
        var viewModel = new StockMarketViewModel(DataService, Mapper, new DialogService(new UnityContainerExtension()), Logger.Object);
        
        viewModel.LoadCommand.Execute();

        await Task.Delay(3000);

        using (new AssertionScope())
        {
            viewModel.IsLoading.Should().BeFalse();
            viewModel.Stocks.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    private async Task DataService_Tick_Should_Update_Price()
    {
        var viewModelTicker = "STK1";
        var viewModel = new StockMarketViewModel(DataService, Mapper, new DialogService(new UnityContainerExtension()), Logger.Object);

        viewModel.LoadCommand.Execute();

        await Task.Delay(3000);
        
        var quote = new Quote { DateTime = DateTime.Now, Price = 100, Ticker = viewModelTicker, Movement = MovementType.None };
        Publisher.Raise(p => p.Publish += null, this, new PublishEventArgs() { Quote = quote });

        await Task.Delay(1500);

        viewModel.Stocks.Should().NotBeNullOrEmpty();
        var viewModelQuote = viewModel.Stocks.Single(w => w.Ticker == viewModelTicker);

        using (new AssertionScope())
        {
            viewModelQuote.DateTime.Should().Be(quote.DateTime);
            viewModelQuote.Movement.Should().Be(quote.Movement);
            viewModelQuote.Price.Should().Be(quote.Price);
        }
    }
}