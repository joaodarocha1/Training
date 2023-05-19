using System;
using System.Windows;

namespace StockMarket.Client.Utils;

public class DispatcherService : IDispatcherService
{
    public void Invoke(Action action)
    {
        Application.Current.Dispatcher.Invoke(action);
    }
}