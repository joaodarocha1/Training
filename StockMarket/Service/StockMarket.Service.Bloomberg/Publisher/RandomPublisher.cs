using StockMarket.Service;
using StockMarket.Service.Event;
using StockMarket.Service.Publisher;
using Timer = System.Timers.Timer;

namespace StockMarket.Service.Bloomberg.Publisher;

public class RandomPublisher : IPublisher, IDisposable
{
    private readonly Random _random = new();
    private readonly Timer _timer1;
    private readonly Timer _timer2;

    private readonly (string Ticker, decimal MinPrice, decimal MaxPrice, DateTime LastChange) _stk1 = (Ticker: "STK1", MinPrice: 240, MaxPrice: 270, LastChange: DateTime.Now);
    private readonly (string Ticker, decimal MinPrice, decimal MaxPrice, DateTime LastChange) _stk2 = (Ticker: "STK2", MinPrice: 180, MaxPrice: 210, LastChange: DateTime.Now);


    public RandomPublisher()
    {
        _timer1 = new Timer(300);
        _timer2 = new Timer(500);
        _timer1.Elapsed += Timer1Elapsed;
        _timer2.Elapsed += Timer2Elapsed;

    }

    private void Timer1Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        PublishRandomEvent(_timer1, sender, _stk1);
    }

    private void PublishRandomEvent(Timer timer, object? sender, (string Ticker, decimal MinPrice, decimal MaxPrice, DateTime LastChange) stk)
    {

        SetRandomInterval(timer);
        
        stk.LastChange = DateTime.Now;
        Publish?.Invoke(sender, new PublishEventArgs()
        {
            Quote = new Quote()
            {
                DateTime = DateTime.Now,
                Price = NextDecimal(stk.MinPrice, stk.MaxPrice),
                Ticker = stk.Ticker
            }
        });
    }

    private void Timer2Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        PublishRandomEvent(_timer2, sender, _stk2);
    }

    private void SetRandomInterval(Timer timer)
    {
        var seconds = _random.Next(1, 4);
        timer.Interval = seconds * 1000;
    }

    private decimal NextDecimal(decimal minValue, decimal maxValue)
    {
        double doubleMinValue = Convert.ToDouble(minValue);
        double doubleMaxValue = Convert.ToDouble(maxValue);

        return (decimal)(_random.NextDouble() * (doubleMaxValue - doubleMinValue) + doubleMinValue);
    }

    public Task SubscribeAsync(IEnumerable<string> enumerable)
    {
        _timer1.Start();
        _timer2.Start();

        return Task.CompletedTask;
    }

    public event EventHandler<PublishEventArgs>? Publish;
    public void UnSubscribe()
    {
        //TODO: Unsubscribe to the publisher here
    }

    public void Dispose()
    {
        _timer1.Stop();
        _timer2.Stop();

        _timer1.Elapsed -= Timer1Elapsed;
        _timer2.Elapsed -= Timer2Elapsed;
    }
}