using System;
using Prism.Mvvm;
using StockMarket.Service;
using StockMarket.Service.Common;

namespace StockMarket.Client.ViewModels;

internal class StockViewModel : BindableBase
{
    private string _ticker;
    private string _name;
    private DateTime _dateTime;
    private decimal _price;
    private MovementType _movement;

    public string Ticker
    {
        get => _ticker;
        set => SetProperty(ref _ticker, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

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


    public MovementType Movement   
    {
        get => _movement;
        set => SetProperty(ref _movement, value);
    }
}