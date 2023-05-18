namespace StockMarket.Service;

public interface IMarketDataService
{
    event EventHandler<TickEventArgs> Tick;
    void Subscribe(IEnumerable<string> tickers);
}