// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Window = System.Windows.Window;

namespace CrissCross.WPF.UI;

/// <summary>Contains application-bar input observers.</summary>
public partial class AppBar
{
    /// <summary>Observes whether the pointer is over the application bar.</summary>
    private void ObservePointerState()
    {
        _ = EventSignal
            .From<MouseEventHandler, MouseEventArgs>(
                handler => handler.Invoke,
                handler => BottomAppBar.MouseEnter += handler,
                handler => BottomAppBar.MouseEnter -= handler)
            .Select(_ => true)
            .Merge(
                EventSignal
                    .From<MouseEventHandler, MouseEventArgs>(
                        handler => handler.Invoke,
                        handler => BottomAppBar.MouseLeave += handler,
                        handler => BottomAppBar.MouseLeave -= handler)
                    .Select(_ => false))
            .Subscribe(isOver => _mouseIsOverAppBar = isOver)
            .DisposeWith(_disposables);
    }

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
    private void ObserveWindowPointer(Window parentWindow)
    {
        _ = EventSignal
            .From<MouseButtonEventHandler, MouseButtonEventArgs>(
                handler => handler.Invoke,
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
}
