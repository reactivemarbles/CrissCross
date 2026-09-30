// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Threading;
using CrissCross.WPF.UI.Gallery;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Owns the shared STA dispatcher and gallery application for WPF runtime tests.</summary>
internal static class WpfTestDispatcher
{
    /// <summary>Signals that the dedicated dispatcher is available.</summary>
    private static readonly TaskCompletionSource<Dispatcher> Started = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Signals termination of the dispatcher loop.</summary>
    private static readonly TaskCompletionSource Stopped = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Starts the dispatcher once for the test assembly.</summary>
    private static readonly Lazy<Task<Dispatcher>> Startup = new(StartThread);

    /// <summary>The dispatcher owned by the dedicated STA thread.</summary>
    private static Dispatcher? _dispatcher;

    /// <summary>The gallery application, accessed only on the shared dispatcher.</summary>
    private static App? _application;

    /// <summary>Runs WPF work on the dispatcher that owns shared application resources.</summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="action">The WPF action.</param>
    /// <returns>A task that completes with the action result.</returns>
    internal static async Task<TResult> RunAsync<TResult>(Func<TResult> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        var dispatcher = await Startup.Value;
        var execution = dispatcher.InvokeAsync(action).Task;
        var completed = await Task.WhenAny(execution, Stopped.Task);
        if (ReferenceEquals(completed, Stopped.Task))
        {
            await Stopped.Task;
            throw new InvalidOperationException("The WPF dispatcher stopped before the test action completed.");
        }

        return await execution;
    }

    /// <summary>Initializes the gallery application once on the shared dispatcher.</summary>
    /// <returns>The application that owns the gallery resources.</returns>
    internal static App GetApplication()
    {
        if (_dispatcher is null)
        {
            throw new InvalidOperationException("The WPF test dispatcher has not started.");
        }

        _dispatcher.VerifyAccess();
        if (_application is null)
        {
            _application = new App { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            _application.InitializeComponent();
        }

        return _application;
    }

    /// <summary>Shuts down the application and dispatcher after all tests have completed.</summary>
    /// <returns>The dispatcher shutdown task.</returns>
    internal static async Task ShutdownAsync()
    {
        if (!Startup.IsValueCreated)
        {
            return;
        }

        var dispatcher = await Startup.Value;
        await dispatcher.InvokeAsync(static () => _application?.Shutdown()).Task;
        dispatcher.BeginInvokeShutdown(DispatcherPriority.ApplicationIdle);
        await Stopped.Task;
    }

    /// <summary>Creates and starts the dedicated STA dispatcher thread.</summary>
    /// <returns>The dispatcher startup task.</returns>
    private static Task<Dispatcher> StartThread()
    {
        var thread = new Thread(RunMessageLoop) { IsBackground = true, Name = "CrissCross WPF test UI thread" };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return Started.Task;
    }

    /// <summary>Runs the shared WPF dispatcher loop and reports startup or shutdown failures.</summary>
    private static void RunMessageLoop()
    {
        try
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Started.SetResult(_dispatcher);
            Dispatcher.Run();
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
