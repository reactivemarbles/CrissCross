// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Threading;

namespace System.Windows;

/// <summary>
/// Make.
/// </summary>
public static class Make
{
    private static bool MainInstanceRunning;

    /// <summary>
    /// Makes the specified application Single Instance.
    /// </summary>
    /// <param name="appName">Name of the application, use a Guid in the Name to ensure unique.</param>
    /// <param name="uniquePerUser">if set to <c>true</c> [unique per user].</param>
    public static void SingleInstance(string appName, bool uniquePerUser = true)
    {
        if (MainInstanceRunning)
        {
            return;
        }

        MainInstanceRunning = true;

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

    private static void ActivateFirstInstanceWindow(EventWaitHandle? eventWaitHandle) =>
        _ = eventWaitHandle?.Set(); // Let's notify the first instance to activate its main window.

    private static void RegisterFirstInstanceWindowActivation(Application app, string eventName)
    {
        var eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, eventName);
        _ = ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, WaitOrTimerCallback, app, Timeout.Infinite, false);
        eventWaitHandle.Close();
    }

    private static void WaitOrTimerCallback(object? state, bool timedOut) =>
        _ = ((Application?)state)?.Dispatcher.BeginInvoke(new Action(() => _ = Application.Current.MainWindow.Activate()));
}
