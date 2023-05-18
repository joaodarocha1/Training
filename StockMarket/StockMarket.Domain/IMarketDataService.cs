namespace StockMarket.Domain;

public interface IMarketDataService
{
    event EventHandler<TickEventArgs> Tick;
    void Subscribe(IEnumerable<string> tickers);
    void Unsubscribe();
    IEnumerable<IQuote> GetPriceHistory(string ticker, DateTime startDateTime, DateTime endDateTime);
}