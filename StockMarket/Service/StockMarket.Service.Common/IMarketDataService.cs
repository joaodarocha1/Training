namespace StockMarket.Service.Common;

public interface IMarketDataService
{
    event EventHandler<TickEventArgs> Tick;
    void Subscribe(IEnumerable<string> tickers);
    void Unsubscribe();
    IEnumerable<IQuote> GetPriceHistory(string ticker);
}