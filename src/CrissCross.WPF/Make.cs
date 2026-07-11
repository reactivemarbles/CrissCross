// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Threading;

namespace System.Windows;

/// <summary>Creates WPF application services.</summary>
public static class Make
{
    /// <summary>Stores whether the primary instance is running.</summary>
    private static bool _mainInstanceRunning;

    /// <summary>Makes the specified application Single Instance.</summary>
    /// <param name="appName">Name of the application, use a Guid in the Name to ensure unique.</param>
    /// <param name="uniquePerUser">if set to <c>true</c> [unique per user].</param>
    public static void SingleInstance(string appName, bool uniquePerUser = true)
    {
        if (_mainInstanceRunning)
        {
            return;
        }

        _mainInstanceRunning = true;

        var eventName = uniquePerUser
            ? $"{appName}-{Environment.MachineName}-{Environment.UserDomainName}-{Environment.UserName}"
            : $"{appName}-{Environment.MachineName}";

        var isSecondaryInstance = true;

        EventWaitHandle? eventWaitHandle = null;
        try
        {
            eventWaitHandle = EventWaitHandle.OpenExisting(eventName);
        }
        catch
        {
            // This code only runs on the first instance.
            isSecondaryInstance = false;
        }

        if (isSecondaryInstance)
        {
            ActivateFirstInstanceWindow(eventWaitHandle);
            Environment.Exit(0);
        }

        RegisterFirstInstanceWindowActivation(Application.Current, eventName);
    }

    /// <summary>Signals the first application instance to activate its window.</summary>
    /// <param name="eventWaitHandle">The first-instance event handle.</param>
    private static void ActivateFirstInstanceWindow(EventWaitHandle? eventWaitHandle) =>
        _ = eventWaitHandle?.Set(); // Let's notify the first instance to activate its main window.

    /// <summary>Registers activation for the first application instance.</summary>
    /// <param name="app">The WPF application.</param>
    /// <param name="eventName">The event name.</param>
    private static void RegisterFirstInstanceWindowActivation(Application app, string eventName)
    {
        var eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, eventName);
        _ = ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, WaitOrTimerCallback, app, Timeout.Infinite, false);
        eventWaitHandle.Close();
    }

    /// <summary>Runs the first-instance activation callback.</summary>
    /// <param name="state">The callback state.</param>
    /// <param name="timedOut">A value indicating whether the wait timed out.</param>
    private static void WaitOrTimerCallback(object? state, bool timedOut) =>
        _ = ((Application?)state)?.Dispatcher.BeginInvoke(new Action(() => _ = Application.Current.MainWindow.Activate()));
}
