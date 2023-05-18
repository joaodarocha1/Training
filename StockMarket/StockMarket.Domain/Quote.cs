﻿using StockMarket.Service;

namespace StockMarket.Domain;

public class Quote : IQuote
{
    public string Ticker { get; set; }
    public DateTime DateTime { get; set; }
    public decimal Price { get; set; }
    public MovementType Movement { get; set; }
}