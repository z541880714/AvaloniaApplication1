using System;
using System.Threading.Tasks;

namespace AvaloniaApplication1.copilot;

public static class ZAsync
{
    public static Task<T> RunLongTimeTask<T>(Func<T> action)
    {
        return Task.Factory.StartNew(action, TaskCreationOptions.LongRunning);
    }

    public static void PostToUi(Action action)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(action);
    }
}