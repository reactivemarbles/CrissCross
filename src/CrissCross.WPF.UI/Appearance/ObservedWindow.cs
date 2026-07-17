// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
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
    public ObservedWindow(IntPtr handle, WindowBackdropType backdrop, bool forceBackgroundReplace, bool updateAccents)
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
    public Window? RootVisual => (Window?)_source.RootVisual;

    /// <summary>Gets the Handle value.</summary>
    public IntPtr Handle { get; }

    /// <summary>Gets the Backdrop value.</summary>
    public WindowBackdropType Backdrop { get; }

    /// <summary>Gets the ForceBackgroundReplace value.</summary>
    public bool ForceBackgroundReplace { get; }

    /// <summary>Gets the UpdateAccents value.</summary>
    public bool UpdateAccents { get; }

    /// <summary>Gets the HasHook value.</summary>
    public bool HasHook { get; private set; }

    /// <summary>Provides the AddHook member.</summary>
    /// <param name="hook">The hook value.</param>
    public void AddHook(HwndSourceHook hook)
    {
        _source.AddHook(hook);

        HasHook = true;
    }

    /// <summary>Provides the RemoveHook member.</summary>
    /// <param name="hook">The hook value.</param>
    public void RemoveHook(HwndSourceHook hook)
    {
        _source.RemoveHook(hook);

        HasHook = false;
    }
}
