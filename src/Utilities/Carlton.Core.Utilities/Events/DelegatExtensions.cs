﻿namespace Carlton.Core.Utilities.Events.Extensions;

public static class DelegatExtensions
{
    public static async Task RaiseAsyncDelegates<TArgs>(this Delegate[] delegates, TArgs args)
    {
        var tasks = delegates.Select(handlers =>
                        {
                            var castedHandler = handlers as Func<TArgs, Task>;
                            return castedHandler(args);
                        });

        await Task.WhenAll(tasks);
    }
}