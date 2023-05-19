using System;

namespace StockMarket.Client.Utils;

public interface IDispatcherService
{
    void Invoke(Action action);
}