using StockMarket.Service.Common.Event;

namespace StockMarket.Service.Common.Services;

public interface IMarketDataService
{
    event EventHandler<TickEventArgs> Tick;
    Task SubscribeAsync(IEnumerable<string> tickers);
    void Unsubscribe();
    IEnumerable<IQuote> GetPriceHistory(string ticker);
}