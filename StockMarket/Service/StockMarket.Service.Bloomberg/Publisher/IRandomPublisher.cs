namespace StockMarket.Service.Bloomberg.Publisher;

public interface IRandomPublisher
{
    void Subscribe(IEnumerable<string> enumerable);
    event EventHandler<RandomPublishEventArgs> Publish;
    void UnSubscribe();
}