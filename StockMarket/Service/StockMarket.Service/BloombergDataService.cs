using System.Collections.Concurrent;
using System.Timers;
using StockMarket.Domain;
using StockMarket.Service.Publisher;

namespace StockMarket.Service;

public class BloombergDataService : IMarketDataService
{
    private readonly IRandomPublisher _randomPublisher;
    private readonly System.Timers.Timer _timer;
    private readonly ConcurrentBag<Quote> _priceHistory;
    private List<Quote> _subscribedTo;
    public event EventHandler<Domain.TickEventArgs> Tick;

    public BloombergDataService(IRandomPublisher randomPublisher)
    {
        _randomPublisher = randomPublisher;
        _randomPublisher.Publish += OnPublish;

        _priceHistory = new ConcurrentBag<Quote>();
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
            var last = _priceHistory.OrderBy(o=>o.DateTime).LastOrDefault(w => w.Ticker == ticker.Ticker);

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

        Tick?.Invoke(sender, new Domain.TickEventArgs()
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

    public void Unsubscribe()
    {
        _timer.Stop();
        _randomPublisher.Publish -= OnPublish;
        _timer.Elapsed -= TimerElapsed;
        _randomPublisher.UnSubscribe();
    }

    public IEnumerable<IQuote> GetPriceHistory(string ticker, DateTime startDateTime, DateTime endDateTime)
    {
        return _priceHistory.ToList();
    }
}