// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Avalonia.Threading;
using AvaloniaWindow = Avalonia.Controls.Window;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Enables native Windows UI Automation TextPattern for rich editors in a window.</summary>
[SupportedOSPlatform("windows")]
public static class RichTextWindowsAutomation
{
    /// <summary>The registrations already associated with windows.</summary>
    private static readonly ConditionalWeakTable<AvaloniaWindow, Automation.Windows.WindowsAutomationContext> Registrations = new();

    /// <summary>Occurs on the UI thread after a registration falls back to Avalonia automation because a native control host was added.</summary>
    public static event EventHandler<RichTextWindowsAutomationUnavailableEventArgs>? RegistrationUnavailable;

    /// <summary>Registers a Windows UI Automation tree over the window's public Avalonia automation peers.</summary>
    /// <param name="window">The Windows window containing rich editors.</param>
    /// <returns>The registration, which removes native hooks and peer subscriptions when disposed.</returns>
    /// <exception cref="PlatformNotSupportedException">The operating system or window backend is not Windows.</exception>
    /// <exception cref="NotSupportedException">Native AOT is active or the window contains native control islands.</exception>
    /// <exception cref="InvalidOperationException">The window already has an active registration.</exception>
    [RequiresDynamicCode("Native UI Automation uses the runtime COM callable wrapper for VARIANT and SAFEARRAY contracts. Native AOT is not supported.")]
    [RequiresUnreferencedCode("Native UI Automation COM callable wrappers require the provider interfaces and members to be retained.")]
    public static IDisposable Enable(AvaloniaWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);
        Dispatcher.UIThread.VerifyAccess();
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Native rich text automation requires Windows.");
        }

        if (!RuntimeFeature.IsDynamicCodeSupported)
        {
            throw new NotSupportedException("Native rich text automation requires runtime COM callable wrappers and cannot run under Native AOT.");
        }

        if (window.TryGetPlatformHandle() is not { HandleDescriptor: "HWND" })
        {
            throw new PlatformNotSupportedException("Native rich text automation requires an Avalonia Win32 window.");
        }

        if (Registrations.TryGetValue(window, out var existing) && !existing.IsDisposed)
        {
            throw new InvalidOperationException("This window already has a rich text Windows automation registration.");
        }

        _ = Registrations.Remove(window);
        var registration = new Automation.Windows.WindowsAutomationContext(window);
        Registrations.Add(window, registration);
        return registration;
    }

    /// <summary>Reports a registration that can no longer preserve the window's automation tree.</summary>
    /// <param name="window">The affected window.</param>
    internal static void ReportUnavailable(AvaloniaWindow window) => Dispatcher.UIThread.Post(
        static state => RegistrationUnavailable?.Invoke(state, new((AvaloniaWindow)state!)),
        window);
}
