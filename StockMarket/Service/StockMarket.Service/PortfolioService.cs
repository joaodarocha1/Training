using StockMarket.Domain;

namespace StockMarket.Service;


public interface IPortfolioService
{
    public IEnumerable<Stock> GetPortfolio(long userId);

}

public class PortfolioService : IPortfolioService
{

    public IEnumerable<Stock> GetPortfolio(long userId)
    {
        var stockData = new List<Stock>
        {
            new Stock { Ticker = "STK1", Name = "Stock 1" },
            new Stock { Ticker = "STK2", Name = "Stock 2" },
        };

        return stockData;
    }
}