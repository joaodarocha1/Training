using StockMarket.Service.Common;

namespace StockMarket.Service.Bloomberg.Publisher;

public interface IRandomPublisher
{
    void Subscribe(IEnumerable<string> enumerable);
    event EventHandler<RamdomPublishEventArgs> Publish;
    void UnSubscribe();
}

public class RamdomPublishEventArgs
{
    public Quote Quote { get; set; }
}