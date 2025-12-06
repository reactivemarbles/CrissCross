// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a numeric keypad control for number input.
/// </summary>
public class NumberPad : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="Value"/>.
    /// </summary>
    public static readonly StyledProperty<string> ValueProperty =
        AvaloniaProperty.Register<NumberPad, string>(nameof(Value), string.Empty);

    /// <summary>
    /// Property for <see cref="ButtonSize"/>.
    /// </summary>
    public static readonly StyledProperty<double> ButtonSizeProperty =
        AvaloniaProperty.Register<NumberPad, double>(nameof(ButtonSize), 48.0);

    /// <summary>
    /// Property for <see cref="Spacing"/>.
    /// </summary>
    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<NumberPad, double>(nameof(Spacing), 4.0);

    /// <summary>
    /// Property for <see cref="ShowDecimal"/>.
    /// </summary>
    public static readonly StyledProperty<bool> ShowDecimalProperty =
        AvaloniaProperty.Register<NumberPad, bool>(nameof(ShowDecimal), true);

    /// <summary>
    /// Property for <see cref="NumberPressedCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> NumberPressedCommandProperty =
        AvaloniaProperty.Register<NumberPad, ICommand?>(nameof(NumberPressedCommand));

    /// <summary>
    /// Defines the <see cref="NumberPressed"/> event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> NumberPressedEvent =
        RoutedEvent.Register<NumberPad, RoutedEventArgs>(nameof(NumberPressed), RoutingStrategies.Bubble);

    private global::Avalonia.Controls.Button? _button0;
    private global::Avalonia.Controls.Button? _button1;
    private global::Avalonia.Controls.Button? _button2;
    private global::Avalonia.Controls.Button? _button3;
    private global::Avalonia.Controls.Button? _button4;
    private global::Avalonia.Controls.Button? _button5;
    private global::Avalonia.Controls.Button? _button6;
    private global::Avalonia.Controls.Button? _button7;
    private global::Avalonia.Controls.Button? _button8;
    private global::Avalonia.Controls.Button? _button9;
    private global::Avalonia.Controls.Button? _buttonDecimal;
    private global::Avalonia.Controls.Button? _buttonBackspace;

    /// <summary>
    /// Occurs when a number button is pressed.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? NumberPressed
    {
        add => AddHandler(NumberPressedEvent, value);
        remove => RemoveHandler(NumberPressedEvent, value);
    }

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the button size.
    /// </summary>
    public double ButtonSize
    {
        get => GetValue(ButtonSizeProperty);
        set => SetValue(ButtonSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between buttons.
    /// </summary>
    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the decimal button.
    /// </summary>
    public bool ShowDecimal
    {
        get => GetValue(ShowDecimalProperty);
        set => SetValue(ShowDecimalProperty, value);
    }

    /// <summary>
    /// Gets or sets the command executed when a number is pressed.
    /// </summary>
    public ICommand? NumberPressedCommand
    {
        get => GetValue(NumberPressedCommandProperty);
        set => SetValue(NumberPressedCommandProperty, value);
    }

    /// <summary>
    /// Appends a digit to the value.
    /// </summary>
    /// <param name="digit">The digit to append.</param>
    public void AppendDigit(string digit)
    {
        if (digit == "." && Value.Contains('.'))
        {
            return;
        }

        Value += digit;
        RaiseEvent(new RoutedEventArgs(NumberPressedEvent));
        NumberPressedCommand?.Execute(digit);
    }

    /// <summary>
    /// Clears the value.
    /// </summary>
    public void Clear()
    {
        Value = string.Empty;
    }

    /// <summary>
    /// Removes the last digit from the value.
    /// </summary>
    public void Backspace()
    {
        if (Value.Length > 0)
        {
            Value = Value[..^1];
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnApplyTemplate(e);

        // Unsubscribe from old buttons
        UnsubscribeButtons();

        // Find and subscribe to new buttons
        _button0 = e.NameScope.Find<global::Avalonia.Controls.Button>("PART_Button0");
        _button1 = e.NameScope.Find<global::Avalonia.Controls.Button>("PART_Button1");
        _button2 = e.NameScope.Find<global::Avalonia.Controls.Button>("PART_Button2");
        _button3 = e.NameScope.Find<global::Avalonia.Controls.Button>("PART_Button3");
        _button4 = e.NameScope.Find<global::Avalonia.Controls.Button>("PART_Button4");
        _button5 = e.NameScope.Find<global::Avalonia.Controls.Button>("PART_Button5");
        _button6 = e.NameScope.Find<global::Avalonia.Controls.Button>("PART_Button6");
        _button7 = e.NameScope.Find<global::Avalonia.Controls.Button>("PART_Button7");
        _button8 = e.NameScope.Find<global::Avalonia.Controls.Button>("PART_Button8");
        _button9 = e.NameScope.Find<global::Avalonia.Controls.Button>("PART_Button9");
        _buttonDecimal = e.NameScope.Find<global::Avalonia.Controls.Button>("PART_ButtonDecimal");
        _buttonBackspace = e.NameScope.Find<global::Avalonia.Controls.Button>("PART_ButtonBackspace");

        SubscribeButtons();
    }

    private void SubscribeButtons()
    {
        if (_button0 != null)
        {
            _button0.Click += (_, _) => AppendDigit("0");
        }

        if (_button1 != null)
        {
            _button1.Click += (_, _) => AppendDigit("1");
        }

        if (_button2 != null)
        {
            _button2.Click += (_, _) => AppendDigit("2");
        }

        if (_button3 != null)
        {
            _button3.Click += (_, _) => AppendDigit("3");
        }

        if (_button4 != null)
        {
            _button4.Click += (_, _) => AppendDigit("4");
        }

        if (_button5 != null)
        {
            _button5.Click += (_, _) => AppendDigit("5");
        }

        if (_button6 != null)
        {
            _button6.Click += (_, _) => AppendDigit("6");
        }

        if (_button7 != null)
        {
            _button7.Click += (_, _) => AppendDigit("7");
        }

        if (_button8 != null)
        {
            _button8.Click += (_, _) => AppendDigit("8");
        }

        if (_button9 != null)
        {
            _button9.Click += (_, _) => AppendDigit("9");
        }

        if (_buttonDecimal != null)
        {
            _buttonDecimal.Click += (_, _) => AppendDigit(".");
        }

        if (_buttonBackspace != null)
        {
            _buttonBackspace.Click += (_, _) => Backspace();
        }
    }

    private void UnsubscribeButtons()
    {
        // Clear references - event handlers will be garbage collected with the buttons
        _button0 = null;
        _button1 = null;
        _button2 = null;
        _button3 = null;
        _button4 = null;
        _button5 = null;
        _button6 = null;
        _button7 = null;
        _button8 = null;
        _button9 = null;
        _buttonDecimal = null;
        _buttonBackspace = null;
    }
}
