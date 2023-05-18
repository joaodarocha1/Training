namespace StockMarket.Service.Common;

public interface IQuote
{
    string Ticker { get; set; }
    DateTime DateTime { get; set; }
    decimal Price { get; set; }
    MovementType Movement { get; set; }
}