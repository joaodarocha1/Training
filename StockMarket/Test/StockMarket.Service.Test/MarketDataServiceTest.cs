using System.Diagnostics;
using Moq;
using StockMarket.Service.Bloomberg.Publisher;
using StockMarket.Service.Common;
using FluentAssertions;
using FluentAssertions.Execution;

namespace StockMarket.Service.Bloomberg.Test
{
    public class MarketDataServiceTest
    {
        private readonly MarketDataService _marketDataService;
        private readonly Mock<IRandomPublisher> _randomPublisher;
        private IEnumerable<IQuote>? _quotes;

        public MarketDataServiceTest()
        {
            _randomPublisher = new Mock<IRandomPublisher>();
            _marketDataService = new MarketDataService(_randomPublisher.Object);
        }

        [Fact]
        public void Subscribe_Test()
        {
            _marketDataService.Subscribe(new []{"STK1","STK2"});
            _randomPublisher.Verify(v => v.Subscribe(It.IsAny<IEnumerable<string>>()), Times.Once);
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
            _marketDataService.Subscribe(new[] { ticker });
            
            var stk1Quote = new Quote()
            {
                Price = 1,
                Ticker = ticker
            };

            _randomPublisher.Raise(p => p.Publish += null, this, new RandomPublishEventArgs() { Quote = stk1Quote });

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
            _marketDataService.Subscribe(new[] { ticker });
            _marketDataService.Tick += MarketDataServiceTick;

            var lastPrice = 100;

            var stk1Quote = new Quote()
            {
                DateTime = DateTime.Now,
                Price = lastPrice,
                Ticker = ticker
            };

            _randomPublisher.Raise(p => p.Publish += null, this, new RandomPublishEventArgs() { Quote = stk1Quote });

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

                _randomPublisher.Raise(p => p.Publish += null, this, new RandomPublishEventArgs() { Quote = stk1Quote2 });

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
            var marketDataService = new MarketDataService(new RandomPublisher());

            marketDataService.Subscribe(new[] { ticker });

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