using System.Diagnostics;
using Moq;
using StockMarket.Service.Bloomberg.Publisher;
using FluentAssertions;
using FluentAssertions.Execution;
using Serilog;
using StockMarket.Service.Enums;
using StockMarket.Service.Event;
using StockMarket.Service.Publisher;

namespace StockMarket.Service.Bloomberg.Test
{
    public class MarketDataServiceTests
    {
        private readonly MarketDataService _marketDataService;
        private readonly Mock<IPublisher> _randomPublisher;
        private IEnumerable<IQuote>? _quotes;
        private readonly Mock<ILogger> _logger;
        public MarketDataServiceTests()
        {
            _randomPublisher = new Mock<IPublisher>();
            _logger = new Mock<ILogger>();
            _marketDataService = new MarketDataService(_randomPublisher.Object, _logger.Object);
        }

        [Fact]
        public void Subscribe_Test()
        {
            _marketDataService.SubscribeAsync(new []{"STK1","STK2"});
            _randomPublisher.Verify(v => v.SubscribeAsync(It.IsAny<IEnumerable<string>>()), Times.Once);
        }

        [Fact]
        public void UnSubscribe_Test()
        {
            _marketDataService.Unsubscribe();
            _randomPublisher.Verify(v => v.UnSubscribe(), Times.Once);
        }

        [Theory]
        [InlineData("STK1")]
        [InlineData("STK2")]
        public void GetPriceHistory_Should_Return_Ticker_On_Price_History(string ticker)
        {
            _marketDataService.SubscribeAsync(new[] { ticker });
            
            var stk1Quote = new Quote()
            {
                Price = 1,
                Ticker = ticker
            };

            _randomPublisher.Raise(p => p.Publish += null, this, new PublishEventArgs() { Quote = stk1Quote });

            var result = _marketDataService.GetPriceHistory(ticker);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().Contain(c => c.Ticker == stk1Quote.Ticker);
            }

        }

        [Theory]
        [InlineData("STK1", MovementType.Down)]
        [InlineData("STK1", MovementType.None)]
        [InlineData("STK1", MovementType.Up)]
        public async Task Tick_Should_Move_In_Direction(string ticker, MovementType movementType)
        {
            _marketDataService.SubscribeAsync(new[] { ticker });
            _marketDataService.Tick += MarketDataServiceTick;

            var lastPrice = 100;

            var stk1Quote = new Quote()
            {
                DateTime = DateTime.Now,
                Price = lastPrice,
                Ticker = ticker
            };

            _randomPublisher.Raise(p => p.Publish += null, this, new PublishEventArgs() { Quote = stk1Quote });

            var delay = 200;

            while (delay < 2100)
            {
                if (movementType == MovementType.Down) lastPrice = lastPrice -= 1;
                if (movementType == MovementType.Up) lastPrice = lastPrice += 1;
                if (movementType == MovementType.None) lastPrice = lastPrice = lastPrice;

                Debug.WriteLine(lastPrice);
                var stk1Quote2 = new Quote()
                {
                    DateTime = DateTime.Now,
                    Price = lastPrice,
                    Ticker = ticker
                };

                _randomPublisher.Raise(p => p.Publish += null, this, new PublishEventArgs() { Quote = stk1Quote2 });

                await Task.Delay(500);
                delay += 500;
            }

            _quotes?.First().Movement.Should().Be(movementType);

        }

        private void MarketDataServiceTick(object? sender, TickEventArgs e)
        {
            _quotes = e.Quotes;
            Debug.WriteLine($"quote: {e.Quotes.First().Price}, {e.Quotes.First().Movement}");
        }

        [Theory]
        [InlineData("STK1", 240, 270)]
        [InlineData("STK2", 180, 210)]
        public async Task GetPriceHistory_Price_Should_Be_On_Range(string ticker, decimal minPrice, decimal maxPrice)
        {
            var marketDataService = new MarketDataService(new RandomPublisher(), _logger.Object);

            marketDataService.SubscribeAsync(new[] { ticker });

            await Task.Delay(1000);

            var result = marketDataService.GetPriceHistory(ticker);

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().AllSatisfy(a => a.Price.Should().BeInRange(minPrice, maxPrice));
            }
            
        }
    }
}