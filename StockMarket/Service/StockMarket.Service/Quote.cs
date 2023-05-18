﻿namespace StockMarket.Service;

public class Quote
{
    public string Ticker { get; set; }
    public DateTime DateTime { get; set; }
    public decimal Price { get; set; }
    public MovementType Movement { get; set; }
}