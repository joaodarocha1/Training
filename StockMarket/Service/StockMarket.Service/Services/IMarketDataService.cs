using StockMarket.Service.Event;

namespace StockMarket.Service.Services;

public interface IMarketDataService
{
    event EventHandler<TickEventArgs> Tick;
    Task SubscribeAsync(IEnumerable<string> tickers);
    void Unsubscribe();
    IEnumerable<IQuote> GetPriceHistory(string ticker);
}