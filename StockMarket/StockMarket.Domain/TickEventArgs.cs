namespace StockMarket.Domain
{
    public class TickEventArgs : EventArgs
    {
        public IEnumerable<Quote> Quotes { get; set; }
    }
}