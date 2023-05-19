using Prism.Mvvm;
using System;

namespace StockMarket.Client.ViewModels;

internal class QuoteViewModel : BindableBase, IEquatable<QuoteViewModel>
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

    public bool Equals(QuoteViewModel? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _dateTime.Equals(other._dateTime) && _price == other._price;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((QuoteViewModel)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_dateTime, _price);
    }
}