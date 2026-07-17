// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Window = System.Windows.Window;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Appearance;
#else
namespace CrissCross.WPF.UI.Appearance;
#endif

/// <summary>
/// Automatically updates the application background if the system theme or color is changed.
/// <para><see cref="SystemThemeWatcher"/> settings work globally and cannot be changed for each <see
/// cref="Window"/>.</para>
/// </summary>
/// <example>
/// <code lang="csharp">
/// SystemThemeWatcher.Watch(this as System.Windows.Window);
/// SystemThemeWatcher.UnWatch(this as System.Windows.Window);
/// </code>
/// <code lang="csharp">
/// SystemThemeWatcher.Watch(
///     _serviceProvider.GetRequiredService&lt;MainWindow&gt;()
/// );
/// </code>
/// </example>
public static class SystemThemeWatcher
{
    /// <summary>The error reported when a native window handle is unavailable.</summary>
    private const string WindowHandleUnavailableMessage = "Could not get window handle.";

    /// <summary>Stores the _observedWindows value.</summary>
    private static readonly ICollection<ObservedWindow> _observedWindows = [];

    /// <summary>Watches the Window and applies the background effect and theme according to the system theme.</summary>
    /// <param name="window">The window that will be updated.</param>
    public static void Watch(Window? window) => Watch(window, WindowBackdropType.Mica, true, false);

    /// <summary>Watches the window for system theme changes.</summary>
    /// <param name="window">The window that will be updated.</param>
    /// <param name="backdrop">Background effect to apply when changing the theme.</param>
    public static void Watch(Window? window, WindowBackdropType backdrop) => Watch(window, backdrop, true, false);

    /// <summary>Watches the window for system theme changes.</summary>
    /// <param name="window">The window that will be updated.</param>
    /// <param name="backdrop">Background effect to apply when changing the theme.</param>
    /// <param name="updateAccents">Whether accents are updated.</param>
    public static void Watch(Window? window, WindowBackdropType backdrop, bool updateAccents) =>
        Watch(window, backdrop, updateAccents, false);

    /// <summary>Watches the window for system theme changes.</summary>
    /// <param name="window">The window that will be updated.</param>
    /// <param name="backdrop">Background effect to be applied when changing the theme.</param>
    /// <param name="updateAccents">Whether accents are updated.</param>
    /// <param name="forceBackgroundReplace">Whether to force replacement of the background effect.</param>
    public static void Watch(
        Window? window,
        WindowBackdropType backdrop,
        bool updateAccents,
        bool forceBackgroundReplace)
    {
        if (window is null)
        {
            return;
        }

        if (window.IsLoaded)
        {
            ObserveLoadedWindow(window, backdrop, updateAccents, forceBackgroundReplace);
        }
        else
        {
            ObserveWindowWhenLoaded(window, backdrop, updateAccents, forceBackgroundReplace);
        }

        if (_observedWindows.Count != 0 || ApplicationThemeManager.GetAppTheme() != ApplicationTheme.Unknown)
        {
            return;
        }

#if DEBUG
        System.Diagnostics.Debug.WriteLine(
            $"INFO | {typeof(SystemThemeWatcher)} changed the app theme on initialization.",
            nameof(SystemThemeWatcher));
#endif
        ApplicationThemeManager.ApplySystemTheme(updateAccents);
    }

    /// <summary>Unwatches the window and removes the hook to receive messages from the system.</summary>
    /// <exception cref="InvalidOperationException">
    /// You cannot unwatch a window that is not yet loaded.
    /// or
    /// Could not get window handle.
    /// </exception>
    /// <param name="window">The window.</param>
    public static void UnWatch(Window? window)
    {
        if (window is null)
        {
            return;
        }

        if (!window.IsLoaded)
        {
            throw new InvalidOperationException("You cannot unwatch a window that is not yet loaded.");
        }

        var windowHandle = new WindowInteropHelper(window).Handle;
        if (windowHandle == IntPtr.Zero)
        {
            throw new InvalidOperationException(WindowHandleUnavailableMessage);
        }

        var observedWindow = _observedWindows.FirstOrDefault(x => x.Handle == windowHandle);

        if (observedWindow is null)
        {
            return;
        }

        observedWindow.RemoveHook(WndProc);

        _ = _observedWindows.Remove(observedWindow);
    }

    /// <summary>Provides the ObserveLoadedWindow member.</summary>
    /// <param name="window">The window.</param>
    /// <param name="backdrop">The backdrop value.</param>
    /// <param name="updateAccents">The updateAccents value.</param>
    /// <param name="forceBackgroundReplace">The forceBackgroundReplace value.</param>
    private static void ObserveLoadedWindow(
        Window window,
        WindowBackdropType backdrop,
        bool updateAccents,
        bool forceBackgroundReplace)
    {
        var windowHandle = new WindowInteropHelper(window).Handle;
        if (windowHandle == IntPtr.Zero)
        {
            throw new InvalidOperationException(WindowHandleUnavailableMessage);
        }

        ObserveLoadedHandle(new ObservedWindow(windowHandle, backdrop, forceBackgroundReplace, updateAccents));
    }

    /// <summary>Provides the ObserveWindowWhenLoaded member.</summary>
    /// <param name="window">The window.</param>
    /// <param name="backdrop">The backdrop value.</param>
    /// <param name="updateAccents">The updateAccents value.</param>
    /// <param name="forceBackgroundReplace">The forceBackgroundReplace value.</param>
    private static void ObserveWindowWhenLoaded(
        Window window,
        WindowBackdropType backdrop,
        bool updateAccents,
        bool forceBackgroundReplace) =>
        window.Loaded += (_, _) =>
        {
            var windowHandle = new WindowInteropHelper(window).Handle;
            if (windowHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException(WindowHandleUnavailableMessage);
            }

            ObserveLoadedHandle(new ObservedWindow(windowHandle, backdrop, forceBackgroundReplace, updateAccents));
        };

    /// <summary>Provides the ObserveLoadedHandle member.</summary>
    /// <param name="observedWindow">The observedWindow value.</param>
    private static void ObserveLoadedHandle(ObservedWindow observedWindow)
    {
        if (observedWindow.HasHook)
        {
            return;
        }

#if DEBUG
        System.Diagnostics.Debug.WriteLine(
            $"INFO | {observedWindow.Handle} ({observedWindow.RootVisual?.Title}) registered as watched window.",
            nameof(SystemThemeWatcher));
#endif
        observedWindow.AddHook(WndProc);
        _observedWindows.Add(observedWindow);

        var currentApplicationTheme = ApplicationThemeManager.GetAppTheme();
        if (observedWindow.RootVisual is null || currentApplicationTheme == ApplicationTheme.Unknown)
        {
            return;
        }

        WindowBackgroundManager.UpdateBackground(
            observedWindow.RootVisual,
            currentApplicationTheme,
            observedWindow.Backdrop);
    }

    /// <summary>Listens to system messages on the application windows.</summary>
    /// <param name="windowHandle">The window handle.</param>
    /// <param name="msg">The msg value.</param>
    /// <param name="wordParameter">The word parameter value.</param>
    /// <param name="longParameter">The long parameter value.</param>
    /// <param name="handled">The handled value.</param>
    /// <returns>The result.</returns>
    private static IntPtr WndProc(
        IntPtr windowHandle,
        int msg,
        IntPtr wordParameter,
        IntPtr longParameter,
        ref bool handled)
    {
        _ = wordParameter;
        _ = longParameter;

        if (msg == (int)User32.WM.WININICHANGE)
        {
            UpdateObservedWindow(windowHandle);
        }

        return IntPtr.Zero;
    }

    /// <summary>Provides the UpdateObservedWindow member.</summary>
    /// <param name="windowHandle">The window handle.</param>
    private static void UpdateObservedWindow(nint windowHandle)
    {
        if (!UnsafeNativeMethods.IsValidWindow(windowHandle))
        {
            return;
        }

        var observedWindow = _observedWindows.FirstOrDefault(x => x.Handle == windowHandle);

        if (observedWindow is null)
        {
            return;
        }

        ApplicationThemeManager.ApplySystemTheme(observedWindow.UpdateAccents);
        var currentApplicationTheme = ApplicationThemeManager.GetAppTheme();

#if DEBUG
        System.Diagnostics.Debug.WriteLine(
            $"INFO | {observedWindow.Handle} ({observedWindow.RootVisual?.Title}) triggered "
                + $"the application theme change to {ApplicationThemeManager.GetSystemTheme()}.",
            nameof(SystemThemeWatcher));
#endif

        if (observedWindow.RootVisual is null)
        {
            return;
        }

        WindowBackgroundManager.UpdateBackground(
            observedWindow.RootVisual,
            currentApplicationTheme,
            observedWindow.Backdrop);
    }
}
