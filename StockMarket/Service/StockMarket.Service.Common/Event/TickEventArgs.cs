namespace StockMarket.Service.Event
{
    public class TickEventArgs : EventArgs
    {
        public IEnumerable<IQuote?> Quotes { get; set; }
    }
}