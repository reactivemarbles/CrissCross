// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Threading;
using CrissCross.Avalonia.UI.Gallery;
using ReactiveUI.Avalonia;

namespace CrissCross.NavigationView.Tests;

/// <summary>Runs Avalonia UI tests on the dispatcher thread that owns the test compositor.</summary>
internal static class AvaloniaTestUiThread
{
    /// <summary>Signals completion of Avalonia platform initialization.</summary>
    private static readonly TaskCompletionSource Started = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Signals termination of the dispatcher loop.</summary>
    private static readonly TaskCompletionSource Stopped = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Starts the dispatcher thread once for the test assembly.</summary>
    private static readonly Lazy<Task> Startup = new(StartThread);

    /// <summary>Initializes Avalonia on the shared test UI thread.</summary>
    /// <returns>The platform initialization task.</returns>
    internal static Task EnsureStartedAsync() => Startup.Value;

    /// <summary>Runs a test action on the shared Avalonia UI thread.</summary>
    /// <param name="action">The action to run.</param>
    /// <returns>The dispatched action task.</returns>
    internal static async Task RunAsync(Func<Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        await EnsureStartedAsync();
        var execution = Dispatcher.UIThread.InvokeAsync(action);
        var completed = await Task.WhenAny(execution, Stopped.Task);
        if (ReferenceEquals(completed, Stopped.Task))
        {
            await Stopped.Task;
            throw new InvalidOperationException("The Avalonia dispatcher stopped before the test action completed.");
        }

        await execution;
    }

    /// <summary>Stops the shared Avalonia UI thread after the test assembly completes.</summary>
    /// <returns>The dispatcher shutdown task.</returns>
    internal static async Task ShutdownAsync()
    {
        if (!Startup.IsValueCreated)
        {
            return;
        }

        await Startup.Value;
        Dispatcher.UIThread.BeginInvokeShutdown(DispatcherPriority.Send);
        await Stopped.Task;
    }

    /// <summary>Creates and starts the dedicated STA dispatcher thread.</summary>
    /// <returns>The platform initialization task.</returns>
    private static Task StartThread()
    {
        var thread = new Thread(RunMessageLoop) { IsBackground = true, Name = "CrissCross Avalonia test UI thread" };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return Started.Task;
    }

    /// <summary>Runs the Avalonia platform setup and native dispatcher loop.</summary>
    private static void RunMessageLoop()
    {
        try
        {
            _ = AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI(static _ => { })
                .SetupWithoutStarting();
            Started.SetResult();
            Dispatcher.UIThread.MainLoop(CancellationToken.None);
        }
        catch (Exception exception)
        {
            _ = Started.TrySetException(exception);
            _ = Stopped.TrySetException(exception);
        }
        finally
        {
            _ = Stopped.TrySetResult();
        }
    }
}
