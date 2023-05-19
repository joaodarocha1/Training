using System.Collections.Concurrent;
using System.Diagnostics;
using System.Timers;
using StockMarket.Service.Bloomberg.Publisher;
using StockMarket.Service.Common;

namespace StockMarket.Service.Bloomberg;

public class MarketDataService : IMarketDataService, IDisposable
{
    private readonly IRandomPublisher _randomPublisher;
    private readonly System.Timers.Timer _timer;
    private readonly ConcurrentBag<Quote> _priceHistory;
    private readonly ConcurrentDictionary<string, Quote?> _subscribedTo = new();
    public event EventHandler<TickEventArgs>? Tick;

    public MarketDataService(IRandomPublisher randomPublisher)
    {
        _randomPublisher = randomPublisher;
        _randomPublisher.Publish += OnPublish;

        _priceHistory = new ConcurrentBag<Quote>();
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += TimerElapsed;

        _timer.Start();
    }

    private void OnPublish(object? sender, RandomPublishEventArgs e)
    {
        _priceHistory.Add(e.Quote);
    }

    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        foreach (var subscription in _subscribedTo)
        {
            var last = _priceHistory.OrderBy(o=>o.DateTime).LastOrDefault(w => w.Ticker == subscription.Key);

            if (last == null) continue;

            if (subscription.Value == null)
            {
                _subscribedTo[subscription.Key] = new Quote()
                {
                    Price = last.Price,
                    Ticker = subscription.Key,
                    DateTime = last.DateTime,
                    Movement = MovementType.None
                };

                return;
            }
            
            var oldPrice = subscription.Value.Price;
            
            subscription.Value.Price = last.Price;
            subscription.Value.DateTime = last.DateTime;

            subscription.Value.Movement = oldPrice switch
            {
                _ when oldPrice == last.Price => MovementType.None,
                _ when oldPrice < last.Price => MovementType.Up,
                _ when oldPrice > last.Price => MovementType.Down,
                _ => throw new InvalidOperationException("Unexpected condition") 
            };

        }

        Tick?.Invoke(sender, new TickEventArgs()
        {
            Quotes = _subscribedTo.Values
        });
    }

    public void Subscribe(IEnumerable<string> tickers)
    {
        foreach (var ticker in tickers)
        {
            _subscribedTo[ticker] = null;
        }
        
        _randomPublisher.Subscribe(tickers);
    }

    public void Unsubscribe()
    {
        _randomPublisher.Publish -= OnPublish;
        _timer.Elapsed -= TimerElapsed;
        _randomPublisher.UnSubscribe();
    }

    public IEnumerable<IQuote> GetPriceHistory(string ticker)
    {
        return _priceHistory.Where(w => w.Ticker == ticker).ToList();
    }

    public void Dispose()
    {
        _timer.Dispose();
        _randomPublisher.Publish -= OnPublish;
        _timer.Elapsed -= TimerElapsed;
        _randomPublisher.UnSubscribe();
    }
}