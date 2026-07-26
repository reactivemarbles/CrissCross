// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Window = System.Windows.Window;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Contains application-bar input observers.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public partial class AppBar
{
    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Observes whether the pointer is over the application bar.</summary>
    private void ObservePointerState() => _ = EventSignal
            .From<MouseEventHandler, MouseEventArgs>(
                static handler => handler.Invoke,
                handler => BottomAppBar.MouseEnter += handler,
                handler => BottomAppBar.MouseEnter -= handler)
            .Select(static _ => true)
            .Merge(
                EventSignal
                    .From<MouseEventHandler, MouseEventArgs>(
                        static handler => handler.Invoke,
                        handler => BottomAppBar.MouseLeave += handler,
                        handler => BottomAppBar.MouseLeave -= handler)
                    .Select(static _ => false))
            .Subscribe(isOver => _mouseIsOverAppBar = isOver)
            .DisposeWith(_disposables);

    /// <summary>Registers window-level input and message listeners.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _ = sender;
        _ = e;
        var parentWindow = Window.GetWindow(this);
        if (parentWindow is not null)
        {
            ObserveWindowPointer(parentWindow);
        }

        this.AppBarIsStickyListener(() => AppBarIsSticky, isSticky => AppBarIsSticky = isSticky);
        this.AppBarLeftListener(() => AppBarLeft);
        this.AppBarRightListener(() => AppBarRight);
        this.ShowAppBarListener(ShowAppBar);
        this.HideAppBarListener(HideAppBar);
    }

    /// <summary>Observes pointer input on the owning window.</summary>
    /// <param name="parentWindow">The owning window.</param>
    private void ObserveWindowPointer(Window parentWindow) => _ = EventSignal
            .From<MouseButtonEventHandler, MouseButtonEventArgs>(
                static handler => handler.Invoke,
                handler => parentWindow.PreviewMouseDown += handler,
                handler => parentWindow.PreviewMouseDown -= handler)
            .Subscribe(e =>
            {
                if (_mouseIsOverAppBar)
                {
                    return;
                }

                if (
                    !_appBarVisible
                    && e.ButtonState == MouseButtonState.Pressed
                    && e.ChangedButton == MouseButton.Right)
                {
                    ShowAppBar();
                }
                else if (_appBarVisible && e.ChangedButton != MouseButton.Right && !AppBarIsSticky)
                {
                    HideAppBar();
                }
            })
            .DisposeWith(_disposables);
}
