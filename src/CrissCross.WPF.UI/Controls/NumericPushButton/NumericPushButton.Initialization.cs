// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;
using ReactiveUI;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Contains numeric push-button initialization behavior.</summary>
public partial class NumericPushButton
{
    /// <inheritdoc />
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        ConfigureShowKeypadCommand();
        ConfigureTimers();
        ObserveLoaded();
    }

    /// <summary>Configures the keypad command.</summary>
    private void ConfigureShowKeypadCommand()
    {
        ShowKeypad = ReactiveCommand.Create(() => { });
        _keypadDisposable.Add(
            ShowKeypad.Subscribe(_ =>
            {
                var maskColor = MaskColor;
                var useThemeManager = UseCrissCrossThemeManager;
                _keypad = new(this) { MaskColor = maskColor, UseCrissCrossThemeManager = useThemeManager };
            }));
    }

    /// <summary>Configures keypad state timers.</summary>
    private void ConfigureTimers()
    {
        _isEnabledFalseTimer = new(
            TimeSpan.FromMilliseconds(IsEnabledFalseDelayMilliseconds),
            DispatcherPriority.Normal,
            (_, _) =>
            {
                _isEnabledFalseTimer.Stop();
                if (_keypad is null)
                {
                    return;
                }

                _keypad.Visibility = Visibility.Collapsed;
                DisposeKeypad();
            },
            Dispatcher);
        _errrorTimer = new(
            TimeSpan.FromSeconds(ErrorVisibilityDelaySeconds),
            DispatcherPriority.Normal,
            (_, _) =>
            {
                _errrorTimer.Stop();
                ErrorVisible = Visibility.Collapsed;
            },
            Dispatcher);
    }

    /// <summary>Observes control loading.</summary>
    private void ObserveLoaded() =>
        _keypadDisposable.Add(
            EventSignal
                .From<RoutedEventHandler, RoutedEventArgs>(
                    handler => handler.Invoke,
                    handler => Loaded += handler,
                    handler => Loaded -= handler)
                .Subscribe(_ => HandleLoaded()));

    /// <summary>Handles control loading.</summary>
    private void HandleLoaded()
    {
        _ = this.UpdateSpinButtonContent();
        if (_valueD.HasObservers)
        {
            _valueD.OnNext((UserChanged, Value));
        }

        if (_valueF.HasObservers)
        {
            _valueF.OnNext((UserChanged, (float)Value));
        }

        if (Command is null)
        {
            return;
        }

        DependencyPropertyChangedEventHandler enabledChanged = (_, args) =>
        {
            if ((bool)args.NewValue)
            {
                _isEnabledFalseTimer.Stop();
            }
            else
            {
                _isEnabledFalseTimer.Start();
            }
        };
        IsEnabledChanged += enabledChanged;
        _keypadDisposable.Add(new ActionDisposable(() => IsEnabledChanged -= enabledChanged));
    }
}
