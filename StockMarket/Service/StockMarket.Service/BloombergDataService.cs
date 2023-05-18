using System.Timers;
using StockMarket.Domain;

namespace StockMarket.Service;

public class BloombergDataService : IMarketDataService
{
    private readonly IRandomPublisher _randomPublisher;
    private readonly System.Timers.Timer _timer;
    private readonly List<Quote> _priceHistory;
    private List<Quote> _subscribedTo;
    public event EventHandler<TickEventArgs>? Tick;

    public BloombergDataService(IRandomPublisher randomPublisher)
    {
        _randomPublisher = randomPublisher;
        _randomPublisher.Publish += OnPublish;

        _priceHistory = new List<Quote>();
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += TimerElapsed;
    }

    private void OnPublish(object? sender, RamdomPublishEventArgs e)
    {
        _priceHistory.Add(e.Quote);
    }

    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        var random = new Random();

        foreach (var ticker in _subscribedTo)
        {
            var last = _priceHistory.LastOrDefault(w => w.Ticker == ticker.Ticker);
            if (last == null) continue;

            var currentPrice = last.Price;

            var oldPrice = ticker.Price;
            ticker.Price = currentPrice;
            ticker.DateTime = last.DateTime;

            if (oldPrice == currentPrice)
            {
                ticker.Movement = MovementType.None;
                continue;
            }

            if (oldPrice > currentPrice)
            {
                ticker.Movement = MovementType.Up;
                continue;
            }

            if (oldPrice < currentPrice)
            {
                ticker.Movement = MovementType.Down;
                continue;
            }
        }

        Tick?.Invoke(sender, new TickEventArgs()
        {
            Quotes = _subscribedTo
        });
    }

    public void Subscribe(IEnumerable<string> tickers)
    {
        _subscribedTo = tickers.Select(s => new Quote() { Ticker = s }).ToList();

        _randomPublisher.Subscribe(tickers);

        _timer.Start();
    }


}