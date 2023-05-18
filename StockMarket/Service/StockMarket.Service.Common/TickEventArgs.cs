namespace StockMarket.Service.Common
{
    public class TickEventArgs : EventArgs
    {
        public IEnumerable<IQuote> Quotes { get; set; }
    }
}