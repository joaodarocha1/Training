namespace StockMarket.Service
{
    public class TickEventArgs : EventArgs
    {
        public IEnumerable<Quote> Quotes { get; set; }
    }
}