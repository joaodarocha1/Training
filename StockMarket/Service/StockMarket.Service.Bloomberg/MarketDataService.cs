using System.Collections.Concurrent;
using System.Timers;
using Serilog;
using StockMarket.Service.Enums;
using StockMarket.Service.Event;
using StockMarket.Service.Publisher;
using StockMarket.Service.Services;

namespace StockMarket.Service.Bloomberg;

public class MarketDataService : IMarketDataService, IDisposable
{
    private readonly IPublisher _randomPublisher;
    private readonly ILogger _logger;
    private readonly System.Timers.Timer _timer;
    private readonly ConcurrentBag<IQuote> _priceHistory;
    private readonly ConcurrentDictionary<string, Quote?> _subscribedTo = new();
    public event EventHandler<TickEventArgs>? Tick;

    public MarketDataService(IPublisher randomPublisher, ILogger logger)
    {
        _logger = logger;

        _logger.Information("Initializing MarkedDataService");

        _randomPublisher = randomPublisher;
        _randomPublisher.Publish += OnPublish;

        _priceHistory = new ConcurrentBag<IQuote>();
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += TimerElapsed;

        _timer.Start();

        _logger.Information("MarkedDataService Initialization completed");
    }

    private void OnPublish(object? sender, PublishEventArgs e)
    {
        _logger.Debug($"MarketDataService.OnPublish: Ticker: {e.Quote.Ticker}, Price: {e.Quote.Price}");
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
            }
            else
            {
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

        }

        Tick?.Invoke(sender, new TickEventArgs()
        {
            Quotes = _subscribedTo.Values
        });
    }

    public async Task SubscribeAsync(IEnumerable<string> tickers)
    {
        _logger.Information($"Subscribing to: {string.Join(",", tickers)}");

        try
        {
            foreach (var ticker in tickers)
            {
                _subscribedTo[ticker] = null;
            }

            await _randomPublisher.SubscribeAsync(tickers);
        }
        catch (Exception ex)
        {
            _logger.Error($"Error while subscribing to: {string.Join(",", tickers)}", ex);
            throw;
        }
        
        _logger.Information($"Successfully subscribed to: {string.Join(",", tickers)}");
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