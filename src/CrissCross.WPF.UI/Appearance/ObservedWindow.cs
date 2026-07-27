// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Window = System.Windows.Window;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Appearance;
#else
namespace CrissCross.WPF.UI.Appearance;
#endif

/// <summary>Provides the ObservedWindow member.</summary>
internal sealed class ObservedWindow
{
    /// <summary>Stores the _source value.</summary>
    private readonly HwndSource _source;

    /// <summary>Initializes a new instance of the <see cref="ObservedWindow"/> class.</summary>
    /// <param name="handle">The handle value.</param>
    /// <param name="backdrop">The backdrop value.</param>
    /// <param name="forceBackgroundReplace">The forceBackgroundReplace value.</param>
    /// <param name="updateAccents">The updateAccents value.</param>
    internal ObservedWindow(IntPtr handle, WindowBackdropType backdrop, bool forceBackgroundReplace, bool updateAccents)
    {
        Handle = handle;
        Backdrop = backdrop;
        ForceBackgroundReplace = forceBackgroundReplace;
        UpdateAccents = updateAccents;
        HasHook = false;

        var windowSource = HwndSource.FromHwnd(handle);
        _source = windowSource ?? throw new InvalidOperationException("Unable to determine the window source.");
    }

    /// <summary>Gets the RootVisual value.</summary>
    internal Window? RootVisual => (Window?)_source.RootVisual;

    /// <summary>Gets the Handle value.</summary>
    internal IntPtr Handle { get; }

    /// <summary>Gets the Backdrop value.</summary>
    internal WindowBackdropType Backdrop { get; }

    /// <summary>Gets the ForceBackgroundReplace value.</summary>
    internal bool ForceBackgroundReplace { get; }

    /// <summary>Gets the UpdateAccents value.</summary>
    internal bool UpdateAccents { get; }

    /// <summary>Gets the HasHook value.</summary>
    internal bool HasHook { get; private set; }

    /// <summary>Provides the AddHook member.</summary>
    /// <param name="hook">The hook value.</param>
    internal void AddHook(HwndSourceHook hook)
    {
        _source.AddHook(hook);

        HasHook = true;
    }

    /// <summary>Provides the RemoveHook member.</summary>
    /// <param name="hook">The hook value.</param>
    internal void RemoveHook(HwndSourceHook hook)
    {
        _source.RemoveHook(hook);

        HasHook = false;
    }
}
