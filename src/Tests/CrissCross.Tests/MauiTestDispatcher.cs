// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Dispatching;

namespace CrissCross.Tests;

/// <summary>Provides deterministic dispatch for handler-free MAUI control tests.</summary>
public sealed class MauiTestDispatcher : IDispatcher, IDispatcherProvider
{
    /// <summary>Stores the dispatcher provider to restore after testing.</summary>
    private static IDispatcherProvider? _previousProvider;

    /// <inheritdoc/>
    public bool IsDispatchRequired => false;

    /// <summary>Installs dispatch support before MAUI bindings are constructed.</summary>
    [Before(HookType.Assembly)]
    public static void Initialize()
    {
        _previousProvider = DispatcherProvider.Current;
        _ = DispatcherProvider.SetCurrent(new MauiTestDispatcher());
    }

    /// <summary>Restores the original provider after the test assembly completes.</summary>
    [After(HookType.Assembly)]
    public static void Restore() => DispatcherProvider.SetCurrent(_previousProvider);

    /// <inheritdoc/>
    public IDispatcher GetForCurrentThread() => this;

    /// <inheritdoc/>
    public bool Dispatch(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        action();
        return true;
    }

    /// <inheritdoc/>
    public bool DispatchDelayed(TimeSpan delay, Action action) => throw new NotSupportedException("Delayed work requires a timer-specific test dispatcher.");

    /// <inheritdoc/>
    public IDispatcherTimer CreateTimer() => throw new NotSupportedException("Timers require a timer-specific test dispatcher.");
}
