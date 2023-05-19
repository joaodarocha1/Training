namespace StockMarket.Service.Common.Event
{
    public class TickEventArgs : EventArgs
    {
        public IEnumerable<IQuote?> Quotes { get; set; }
    }
}