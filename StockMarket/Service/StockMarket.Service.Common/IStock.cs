namespace StockMarket.Service.Common;

public interface IStock
{
    string Ticker { get; set; }
    string Name { get; set; }
}