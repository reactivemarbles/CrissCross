// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Contains modern check-box input observers.</summary>
public partial class CheckBoxModern
{
    /// <summary>Observes click input.</summary>
    private void ObserveInput()
    {
        _ = EventSignal
            .From<MouseButtonEventHandler, MouseButtonEventArgs>(
                handler => handler.Invoke,
                handler => PreviewMouseLeftButtonDown += handler,
                handler => PreviewMouseLeftButtonDown -= handler)
            .Subscribe(_ =>
            {
                if (Command?.CanExecute(null) == false)
                {
                    return;
                }

                UpdateValue(true, !Checked);
            })
            .DisposeWith(_disposables);
    }

    /// <summary>Observes control loading and command state.</summary>
    private void ObserveLoaded()
    {
        _ = EventSignal
            .From<RoutedEventHandler, RoutedEventArgs>(
                handler => handler.Invoke,
                handler => Loaded += handler,
                handler => Loaded -= handler)
            .Subscribe(loadedArgs => _ = HandleLoadedAsync(loadedArgs))
            .DisposeWith(_disposables);
    }

    /// <summary>Initializes enabled-state observation after the control loads.</summary>
    /// <param name="loadedArgs">The loaded event arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task HandleLoadedAsync(RoutedEventArgs loadedArgs)
    {
        _ = loadedArgs;
        if (_isChecked.HasObservers)
        {
            _isChecked.OnNext(new CheckBoxResultEventArgs(false, Checked));
        }

        await EnableChange(IsEnabled);
        IsEnabledChanged += OnIsEnabledChanged;
        if (Command is null)
        {
            return;
        }

        if (!Command.CanExecute(null))
        {
            await EnableChange(false);
        }

        _ = EventSignal
            .From<EventHandler, EventArgs>(
                handler => handler.Invoke,
                handler => Command.CanExecuteChanged += handler,
                handler => Command.CanExecuteChanged -= handler)
            .Subscribe(ignored =>
            {
                _ = ignored;
                _ = EnableChange(IsEnabled && Command.CanExecute(null));
            })
            .DisposeWith(_disposables);
    }

    /// <summary>Updates the enabled animation after the WPF enabled state changes.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The property-change arguments.</param>
    private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        _ = sender;
        _ = EnableChange((bool)e.NewValue);
    }

    /// <summary>Observes pointer state for hover styling.</summary>
    private void ObservePointerState()
    {
        _ = EventSignal
            .From<MouseEventHandler, MouseEventArgs>(
                handler => handler.Invoke,
                handler => MouseEnter += handler,
                handler => MouseEnter -= handler)
            .Subscribe(_ => UpdateHoverState(true))
            .DisposeWith(_disposables);
        _ = EventSignal
            .From<MouseEventHandler, MouseEventArgs>(
                handler => handler.Invoke,
                handler => MouseLeave += handler,
                handler => MouseLeave -= handler)
            .Subscribe(_ => UpdateHoverState(false))
            .DisposeWith(_disposables);
    }

    /// <summary>Updates pointer hover styling.</summary>
    /// <param name="isHovered">Whether the pointer is hovering.</param>
    private void UpdateHoverState(bool isHovered)
    {
        if (Command?.CanExecute(null) == false || !IsEnabled)
        {
            return;
        }

        CheckBackground = isHovered
            ? new SolidColorBrush(Color.FromRgb(HoverRedChannel, HoverGreenChannel, HoverBlueChannel))
            : Brushes.White;
    }
}
