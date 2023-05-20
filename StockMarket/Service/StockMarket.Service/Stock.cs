namespace StockMarket.Service;

public class Stock : IStock
{
    public string Ticker { get; set; }
    public string Name { get; set; }
}