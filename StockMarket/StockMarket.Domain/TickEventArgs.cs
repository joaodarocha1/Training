namespace StockMarket.Domain
{
    public class TickEventArgs : EventArgs
    {
        public IEnumerable<IQuote> Quotes { get; set; }
    }
}