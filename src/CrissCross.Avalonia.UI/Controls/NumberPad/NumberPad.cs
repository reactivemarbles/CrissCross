// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents a numeric keypad control for number input.</summary>
public class NumberPad : TemplatedControl
{
    /// <summary>Property for <see cref="Value"/>.</summary>
    public static readonly StyledProperty<string> ValueProperty =
        AvaloniaProperty.Register<NumberPad, string>(nameof(Value), string.Empty);

    /// <summary>Property for <see cref="ButtonSize"/>.</summary>
    public static readonly StyledProperty<double> ButtonSizeProperty =
        AvaloniaProperty.Register<NumberPad, double>(nameof(ButtonSize), 48.0);

    /// <summary>Property for <see cref="Spacing"/>.</summary>
    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<NumberPad, double>(nameof(Spacing), 4.0);

    /// <summary>Property for <see cref="ShowDecimal"/>.</summary>
    public static readonly StyledProperty<bool> ShowDecimalProperty =
        AvaloniaProperty.Register<NumberPad, bool>(nameof(ShowDecimal), true);

    /// <summary>Property for <see cref="NumberPressedCommand"/>.</summary>
    public static readonly StyledProperty<ICommand?> NumberPressedCommandProperty =
        AvaloniaProperty.Register<NumberPad, ICommand?>(nameof(NumberPressedCommand));

    /// <summary>Defines the <see cref="NumberPressed"/> event.</summary>
    public static readonly RoutedEvent<RoutedEventArgs> NumberPressedEvent =
        RoutedEvent.Register<NumberPad, RoutedEventArgs>(nameof(NumberPressed), RoutingStrategies.Bubble);

    /// <summary>Provides the _button0 member.</summary>
    private global::Avalonia.Controls.Button? _button0;

    /// <summary>Provides the _button1 member.</summary>
    private global::Avalonia.Controls.Button? _button1;

    /// <summary>Provides the _button2 member.</summary>
    private global::Avalonia.Controls.Button? _button2;

    /// <summary>Provides the _button3 member.</summary>
    private global::Avalonia.Controls.Button? _button3;

    /// <summary>Provides the _button4 member.</summary>
    private global::Avalonia.Controls.Button? _button4;

    /// <summary>Provides the _button5 member.</summary>
    private global::Avalonia.Controls.Button? _button5;

    /// <summary>Provides the _button6 member.</summary>
    private global::Avalonia.Controls.Button? _button6;

    /// <summary>Provides the _button7 member.</summary>
    private global::Avalonia.Controls.Button? _button7;

    /// <summary>Provides the _button8 member.</summary>
    private global::Avalonia.Controls.Button? _button8;

    /// <summary>Provides the _button9 member.</summary>
    private global::Avalonia.Controls.Button? _button9;

    /// <summary>Provides the _buttonDecimal member.</summary>
    private global::Avalonia.Controls.Button? _buttonDecimal;

    /// <summary>Provides the _buttonBackspace member.</summary>
    private global::Avalonia.Controls.Button? _buttonBackspace;

    /// <summary>Occurs when a number button is pressed.</summary>
    public event EventHandler<RoutedEventArgs>? NumberPressed
    {
        add => AddHandler(NumberPressedEvent, value);
        remove => RemoveHandler(NumberPressedEvent, value);
    }

    /// <summary>Gets or sets the current value.</summary>
    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>Gets or sets the button size.</summary>
    public double ButtonSize
    {
        get => GetValue(ButtonSizeProperty);
        set => SetValue(ButtonSizeProperty, value);
    }

    /// <summary>Gets or sets the spacing between buttons.</summary>
    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether to show the decimal button.</summary>
    public bool ShowDecimal
    {
        get => GetValue(ShowDecimalProperty);
        set => SetValue(ShowDecimalProperty, value);
    }

    /// <summary>Gets or sets the command executed when a number is pressed.</summary>
    public ICommand? NumberPressedCommand
    {
        get => GetValue(NumberPressedCommandProperty);
        set => SetValue(NumberPressedCommandProperty, value);
    }

    /// <summary>Appends a digit to the value.</summary>
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

    /// <summary>Clears the value.</summary>
    public void Clear()
    {
        Value = string.Empty;
    }

    /// <summary>Removes the last digit from the value.</summary>
    public void Backspace()
    {
        if (Value.Length == 0)
        {
            return;
        }

        Value = Value[..^1];
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

    /// <summary>Provides the SubscribeButtons member.</summary>
    private void SubscribeButtons()
    {
        SubscribeDigitButton(_button0, "0");
        SubscribeDigitButton(_button1, "1");
        SubscribeDigitButton(_button2, "2");
        SubscribeDigitButton(_button3, "3");
        SubscribeDigitButton(_button4, "4");
        SubscribeDigitButton(_button5, "5");
        SubscribeDigitButton(_button6, "6");
        SubscribeDigitButton(_button7, "7");
        SubscribeDigitButton(_button8, "8");
        SubscribeDigitButton(_button9, "9");
        SubscribeDigitButton(_buttonDecimal, ".");
        SubscribeBackspaceButton();
    }

    /// <summary>Subscribes a digit button when present.</summary>
    /// <param name="button">The button to subscribe.</param>
    /// <param name="digit">The digit appended by the button.</param>
    private void SubscribeDigitButton(global::Avalonia.Controls.Button? button, string digit)
    {
        if (button is null)
        {
            return;
        }

        button.Click += (_, _) => AppendDigit(digit);
    }

    /// <summary>Subscribes the backspace button when present.</summary>
    private void SubscribeBackspaceButton()
    {
        if (_buttonBackspace is null)
        {
            return;
        }

        _buttonBackspace.Click += (_, _) => Backspace();
    }

    /// <summary>Provides the UnsubscribeButtons member.</summary>
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
