using System;

namespace StockMarket.Service;

public interface IRandomPublisher
{
    void Subscribe(IEnumerable<string> enumerable);
    event EventHandler<RamdomPublishEventArgs> Publish;
}

public class RamdomPublishEventArgs
{
    public Quote Quote { get; set; }
}