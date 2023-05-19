namespace StockMarket.Service.Bloomberg.Publisher;

public interface IRandomPublisher
{
    Task SubscribeAsync(IEnumerable<string> enumerable);
    event EventHandler<RandomPublishEventArgs> Publish;
    void UnSubscribe();
}