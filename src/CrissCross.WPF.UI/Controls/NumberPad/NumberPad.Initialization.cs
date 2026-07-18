// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using System.Windows.Threading;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Contains number-pad initialization behavior.</summary>
public partial class NumberPad
{
    /// <inheritdoc />
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        Unit.Content = _owner.Units;
        _owner.IsEnabled = false;
        ObservePointerAndKeyboard();
        ObserveFocus();
        ObserveActions();
        ConfigureLimitsTimer();
        Showkeypad();
    }

    /// <summary>Observes pointer and keyboard input.</summary>
    private void ObservePointerAndKeyboard()
    {
        _ = EventSignal
            .From<MouseButtonEventHandler, MouseButtonEventArgs>(
                handler => handler.Invoke,
                handler => MouseLeftButtonDown += handler,
                handler => MouseLeftButtonDown -= handler)
            .Merge(
                EventSignal.From<MouseButtonEventHandler, MouseButtonEventArgs>(
                    handler => handler.Invoke,
                    handler => Mask.MouseLeftButtonDown += handler,
                    handler => Mask.MouseLeftButtonDown -= handler))
            .Subscribe(CloseWhenOutside)
            .DisposeWith(_disposables);
        _ = EventSignal
            .From<KeyEventHandler, KeyEventArgs>(
                handler => handler.Invoke,
                handler => PreviewKeyDown += handler,
                handler => PreviewKeyDown -= handler)
            .Subscribe(Window_PreviewKeyDown)
            .DisposeWith(_disposables);
    }

    /// <summary>Closes the keypad for pointer input outside its grid.</summary>
    /// <param name="args">The pointer arguments.</param>
    private void CloseWhenOutside(MouseButtonEventArgs args)
    {
        var mouse = args.GetPosition(this);
        var gridPosition = WGrid.Margin;
        if (
            mouse.X >= gridPosition.Left
            && mouse.X <= gridPosition.Left + WGrid.Width
            && mouse.Y >= gridPosition.Top
            && mouse.Y <= gridPosition.Top + WGrid.Height)
        {
            return;
        }

        CloseKeypad();
    }

    /// <summary>Observes value focus.</summary>
    private void ObserveFocus() =>
        EventSignal
            .From<RoutedEventHandler, RoutedEventArgs>(
                handler => handler.Invoke,
                handler => Value.GotFocus += handler,
                handler => Value.GotFocus -= handler)
            .Select(_ => true)
            .Merge(
                EventSignal
                    .From<RoutedEventHandler, RoutedEventArgs>(
                        handler => handler.Invoke,
                        handler => Value.LostFocus += handler,
                        handler => Value.LostFocus -= handler)
                    .Select(_ => false))
            .Subscribe(x => _hasFocus = x)
            .DisposeWith(_disposables);

    /// <summary>Observes keypad action buttons.</summary>
    private void ObserveActions()
    {
        ObserveAction(Accept, AcceptResult);
        ObserveAction(CancelBtn, CloseKeypad);
        ObserveAction(ClearBtn, ClearValues);
    }

    /// <summary>Observes a keypad action button.</summary>
    /// <param name="button">The button to observe.</param>
    /// <param name="action">The action to run.</param>
    private void ObserveAction(System.Windows.Controls.Button button, Action action) =>
        EventSignal
            .From<RoutedEventHandler, RoutedEventArgs>(
                handler => handler.Invoke,
                handler => button.Click += handler,
                handler => button.Click -= handler)
            .Subscribe(_ => action())
            .DisposeWith(_disposables);

    /// <summary>Configures the value limits timer.</summary>
    private void ConfigureLimitsTimer() =>
        _limitsTimer = new(
            TimeSpan.FromSeconds(1),
            DispatcherPriority.Normal,
            (_, _) =>
            {
                if (!Value.Value.HasValue || Value.Value.Value > _owner.Maximum || Value.Value.Value < _owner.Minimum)
                {
                    return;
                }

                _limitsTimer.Stop();
            },
            Dispatcher);
}
