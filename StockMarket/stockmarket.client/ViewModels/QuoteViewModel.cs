using Prism.Mvvm;
using System;

namespace StockMarket.Client.ViewModels;

internal class QuoteViewModel : BindableBase
{
    private DateTime _dateTime;
    private decimal _price;

    public DateTime DateTime
    {
        get => _dateTime;
        set => SetProperty(ref _dateTime, value);
    }

    public decimal Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }
}