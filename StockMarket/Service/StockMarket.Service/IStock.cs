namespace StockMarket.Service;

public interface IStock
{
    string Ticker { get; set; }
    string Name { get; set; }
}