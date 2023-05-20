using StockMarket.Service.Event;

namespace StockMarket.Service.Publisher;

public interface IPublisher
{
    Task SubscribeAsync(IEnumerable<string> enumerable);
    event EventHandler<PublishEventArgs>? Publish;
    void UnSubscribe();
}