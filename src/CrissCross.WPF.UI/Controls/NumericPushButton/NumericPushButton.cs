// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Subjects;
using System.Windows.Threading;
using ReactiveUI;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Numeric Push Button.
/// </summary>
public class NumericPushButton : System.Windows.Controls.Button, INumberPadButton, IDisposable
{
    /// <summary>
    /// Defaults decimal points.
    /// </summary>
    public static readonly DependencyProperty DecimalPlacesProperty =
        DependencyProperty.Register(
            nameof(DecimalPlaces),
            typeof(int),
            typeof(NumericPushButton),
            new UIPropertyMetadata(0, UpdateDecimalPlaces));

    /// <summary>
    /// The error text property.
    /// </summary>
    public static readonly DependencyProperty ErrorTextProperty =
        DependencyProperty.Register(
            nameof(ErrorText),
            typeof(string),
            typeof(NumericPushButton),
            new PropertyMetadata("Action disallowed - Please press and hold safety button first"));

    /// <summary>
    /// The error visible property.
    /// </summary>
    public static readonly DependencyProperty ErrorVisibleProperty =
        DependencyProperty.Register(
            nameof(ErrorVisible),
            typeof(Visibility),
            typeof(NumericPushButton),
            new PropertyMetadata(Visibility.Hidden));

    /// <summary>
    /// The mask color property.
    /// </summary>
    public static readonly DependencyProperty MaskColorProperty =
        DependencyProperty.Register(
            nameof(MaskColor),
            typeof(Brush),
            typeof(NumericPushButton),
            new PropertyMetadata(Brushes.Black, UpdateMask));

    /// <summary>
    /// Maximum of the size of value.
    /// </summary>
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(
            nameof(Maximum),
            typeof(double?),
            typeof(NumericPushButton),
            new UIPropertyMetadata(double.MaxValue, MaximumChanged));

    /// <summary>
    /// Minimum size of value.
    /// </summary>
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(
            nameof(Minimum),
            typeof(double?),
            typeof(NumericPushButton),
            new UIPropertyMetadata(double.MinValue, MinimumChanged));

    /// <summary>
    /// The show keypad property.
    /// </summary>
    public static readonly DependencyProperty ShowKeypadProperty =
        DependencyProperty.Register(
            nameof(ShowKeypad),
            typeof(ReactiveCommand<Unit, Unit>),
            typeof(NumericPushButton),
            new PropertyMetadata(null));

    /// <summary>
    /// The units on new line property.
    /// </summary>
    public static readonly DependencyProperty UnitsOnNewLineProperty =
        DependencyProperty.Register(
            nameof(UnitsOnNewLine),
            typeof(bool),
            typeof(NumericPushButton),
            new PropertyMetadata(false, UpdateNewLine));

    /// <summary>
    /// Units Dependency Property.
    /// </summary>
    public static readonly DependencyProperty UnitsProperty =
        DependencyProperty.Register(
            nameof(Units),
            typeof(string),
            typeof(NumericPushButton),
            new PropertyMetadata("Units", UnitsChanged));

    /// <summary>
    /// Value Change.
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(NumericPushButton),
            new PropertyMetadata(0d, UpdateValue));

    private readonly DispatcherTimer _errrorTimer;
    private readonly DispatcherTimer _isEnabledFalseTimer;
    private readonly ReplaySubject<(bool UserChanged, double Value)> _valueD = new(1);
    private readonly ReplaySubject<(bool UserChanged, float Value)> _valueF = new(1);
    private readonly IDisposable _keypadDisposable;
    private NumberPad? _keypad;
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericPushButton"/> class.
    /// </summary>
    public NumericPushButton()
    {
        DefaultStyleKey = typeof(NumericPushButton);
        ShowKeypad = ReactiveCommand.Create(() => { });
        _keypadDisposable = ShowKeypad.Subscribe(_ => _keypad = new NumberPad(this) { MaskColor = MaskColor });
        _isEnabledFalseTimer = new DispatcherTimer(
            TimeSpan.FromMilliseconds(100),
            DispatcherPriority.Normal,
            (s, e) =>
            {
                _isEnabledFalseTimer?.Stop();
                if (_keypad != null)
                {
                    _keypad.Visibility = Visibility.Collapsed;
                    _keypad?.Dispose();
                }
            },
            Dispatcher);

        _errrorTimer = new DispatcherTimer(
            TimeSpan.FromSeconds(2),
            DispatcherPriority.Normal,
            (s, e) =>
            {
                _errrorTimer?.Stop();
                ErrorVisible = Visibility.Collapsed;
            },
            Dispatcher);

        Loaded += (ss, ee) =>
        {
            this.UpdateSpinButtonContent();
            if (_valueD.HasObservers)
            {
                _valueD.OnNext((UserChanged, Value));
            }

            if (_valueF.HasObservers)
            {
                _valueF.OnNext((UserChanged, (float)Value));
            }

            var command = Command;
            if (command != null)
            {
                IsEnabledChanged += (_, e) =>
                {
                    if (!(bool)e.NewValue)
                    {
                        _isEnabledFalseTimer.Start();
                    }
                    else
                    {
                        _isEnabledFalseTimer.Stop();
                    }
                };
            }
        };

        IsEnabledChanged += (_, e) =>
        {
            var enabled = (bool)e.NewValue;
        };
    }

    /// <summary>
    /// Value changed and if user changed it
    /// </summary>
    public event EventHandler<(bool, double)>? ValueChanged;

    /// <summary>
    /// Gets or sets how many decimal places to visualize.
    /// </summary>
    [Description("Gets or sets how many decimal places to visualize")]
    [Category("Common")]
    public int DecimalPlaces
    {
        get => (int)GetValue(DecimalPlacesProperty);

        set => SetValue(DecimalPlacesProperty, value);
    }

    /// <summary>
    /// Gets or sets the error text.
    /// </summary>
    /// <value>The error text.</value>
    [Description("Gets or sets the Error Text")]
    [Category("Common")]
    public string ErrorText
    {
        get => (string)GetValue(ErrorTextProperty);

        set => SetValue(ErrorTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the error visible.
    /// </summary>
    /// <value>The error visible.</value>
    [Description("Gets or sets the Error Visibility.")]
    [Category("Common")]
    public Visibility ErrorVisible
    {
        get => (Visibility)GetValue(ErrorVisibleProperty);

        set => SetValue(ErrorVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the mask.
    /// </summary>
    /// <value>The color of the mask.</value>
    [Description("Sets MaskColor of the Keypad")]
    [Category("Brush")]
    public Brush MaskColor
    {
        get => (Brush)GetValue(MaskColorProperty);

        set => SetValue(MaskColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the Maximum value allowed.
    /// </summary>
    [Description("Gets or sets the Maximum value allowed")]
    [Category("Common")]
    public double? Maximum
    {
        get => (double?)GetValue(MaximumProperty);

        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// Gets or sets the Minimum value.
    /// </summary>
    [Description("Gets or sets the Minimum value allowed")]
    [Category("Common")]
    public double? Minimum
    {
        get => (double?)GetValue(MinimumProperty);

        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// Gets or sets the show keypad.
    /// </summary>
    /// <value>The show keypad.</value>
    public ReactiveCommand<Unit, Unit> ShowKeypad
    {
        get => (ReactiveCommand<Unit, Unit>)GetValue(ShowKeypadProperty);

        set => SetValue(ShowKeypadProperty, value);
    }

    /// <summary>
    /// Gets or sets the Units to display.
    /// </summary>
    [Description("Gets or sets the Units to display")]
    [Category("Common")]
    public string Units
    {
        get => (string)GetValue(UnitsProperty);

        set => SetValue(UnitsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether [units on new line].
    /// </summary>
    /// <value><c>true</c> if [units on new line]; otherwise, <c>false</c>.</value>
    [Description("Gets or sets a value indicating whether units is on a new line or not")]
    [Category("Common")]
    public bool UnitsOnNewLine
    {
        get => (bool)GetValue(UnitsOnNewLineProperty);

        set => SetValue(UnitsOnNewLineProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the user changed.
    /// </summary>
    /// <value><c>true</c> if [user changed]; otherwise, <c>false</c>.</value>
    public bool UserChanged { get; set; }

    /// <summary>
    /// Gets or sets the Value of Button.
    /// </summary>
    [Description("Gets or sets the Value of Button")]
    [Category("Common")]
    public double Value
    {
        get => (double)GetValue(ValueProperty);

        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets the value double observable and if the user changed it.
    /// </summary>
    /// <value>The value double observable.</value>
    public IObservable<(bool UserChanged, double Value)> ValueDoubleObservable => _valueD;

    /// <summary>
    /// Gets the value single.
    /// </summary>
    /// <value>The value single.</value>
    public float ValueSingle => (float)(double)GetValue(ValueProperty);

    /// <summary>
    /// Gets the value single observable and if the user changed it.
    /// </summary>
    /// <value>The value single observable.</value>
    public IObservable<(bool UserChanged, float Value)> ValueSingleObservable => _valueF;

    /// <summary>
    /// Gets a value indicating whether this instance is in design mode.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is in design mode; otherwise, <c>false</c>.
    /// </value>
    protected bool InDesignMode => DesignerProperties.GetIsInDesignMode(this);

    /// <summary>
    /// Disposes the keypad.
    /// </summary>
    public void DisposeKeypad()
    {
        _keypad?.Dispose();
        _keypad = null;
        GC.Collect();
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the specified disposing.
    /// </summary>
    /// <param name="disposing">if set to <c>true</c> [disposing].</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _valueD.Dispose();
                _valueF.Dispose();
                _keypad?.Dispose();
                _keypadDisposable.Dispose();
            }

            _disposedValue = true;
        }
    }

    /// <summary>
    /// Maximum Changed Dependency.
    /// </summary>
    /// <param name="d">Dependency Object.</param>
    /// <param name="e">Dependency Property Changed Event Arguments.</param>
    private static void MaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sb = d as NumericPushButton;
        var max = (double)e.NewValue;
        if (sb?._keypad != null)
        {
            sb._keypad.Value.Maximum = max;
            if (sb.Value > max)
            {
                sb.Value = max;
                sb._keypad.Value.Value = sb.Value;
            }
        }
    }

    /// <summary>
    /// The minimum Changed.
    /// </summary>
    /// <param name="d">Dependency Object.</param>
    /// <param name="e">Dependency Property Changed Event Arguments.</param>
    private static void MinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sb = d as NumericPushButton;
        var minVal = (double)e.NewValue;
        if (sb?._keypad != null)
        {
            sb._keypad.Value.Minimum = minVal;
            if (sb.Value < minVal)
            {
                sb.Value = minVal;
                sb._keypad.Value.Value = minVal;
            }
        }
    }

    /// <summary>
    /// Change the units displayed.
    /// </summary>
    /// <param name="d">Dependency Object.</param>
    /// <param name="e">Dependency Property Changed Event Arguments.</param>
    private static void UnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NumericPushButton sb && e.OldValue != e.NewValue)
        {
            if (sb._keypad != null)
            {
                sb._keypad.Unit.Content = e.NewValue.ToString();
            }

            sb.UpdateSpinButtonContent();
        }
    }

    /// <summary>
    /// Update the decimals.
    /// </summary>
    /// <param name="d">the dependency object.</param>
    /// <param name="e">Dependency Property Changed Event Arguments.</param>
    private static void UpdateDecimalPlaces(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NumericPushButton sb && e.OldValue != e.NewValue && sb._keypad != null)
        {
            sb._keypad.Value.MaxDecimalPlaces = (int)e.NewValue;
            sb.UpdateSpinButtonContent();
        }
    }

    /// <summary>
    /// Updates the mask.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="e">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
    /// </param>
    private static void UpdateMask(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NumericPushButton c && c._keypad != null)
        {
            c.MaskColor = (Brush)e.NewValue;
            c._keypad.MaskColor = c.MaskColor;
        }
    }

    /// <summary>
    /// Updates the new line.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="e">
    /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
    /// </param>
    private static void UpdateNewLine(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NumericPushButton sb && e.OldValue != e.NewValue)
        {
            sb.UpdateSpinButtonContent();
        }
    }

    /// <summary>
    /// Updates the Value that was changed.
    /// </summary>
    /// <param name="d">the dependency object.</param>
    /// <param name="e">Dependency Property Changed Event Arguments.</param>
    private static void UpdateValue(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is double newdbl && e.OldValue is double olddbl && newdbl != olddbl && d is NumericPushButton sb)
        {
            if (newdbl <= sb.Maximum && newdbl >= sb.Minimum)
            {
                sb.UpdateText();
            }
            else
            {
                sb.Value = olddbl;
            }
        }
    }

    /// <summary>
    /// Update the text of button.
    /// </summary>
    private void UpdateText()
    {
        this.UpdateSpinButtonContent();
        var v = (UserChanged, Value);
        if (_valueD.HasObservers)
        {
            _valueD.OnNext(v);
        }

        if (_valueF.HasObservers)
        {
            _valueF.OnNext((UserChanged, (float)Value));
        }

        ValueChanged?.Invoke(this, v);
        UserChanged = false;
    }
}
