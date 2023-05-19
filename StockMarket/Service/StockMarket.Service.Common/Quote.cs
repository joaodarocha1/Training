using StockMarket.Service.Common.Enums;

namespace StockMarket.Service.Common;

public class Quote : IQuote
{
    public string Ticker { get; set; }
    public DateTime DateTime { get; set; }
    public decimal Price { get; set; }
    public MovementType Movement { get; set; }
}