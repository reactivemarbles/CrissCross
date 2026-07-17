// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;
using ReactiveUI;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Numeric Push Button.</summary>
public partial class NumericPushButton : System.Windows.Controls.Button, INumberPadButton, IDisposable
{
    /// <summary>Defaults decimal points.</summary>
    public static readonly DependencyProperty DecimalPlacesProperty = DependencyProperty.Register(
        nameof(DecimalPlaces),
        typeof(int),
        typeof(NumericPushButton),
        new UIPropertyMetadata(0, UpdateDecimalPlaces));

    /// <summary>The error text property.</summary>
    public static readonly DependencyProperty ErrorTextProperty = DependencyProperty.Register(
        nameof(ErrorText),
        typeof(string),
        typeof(NumericPushButton),
        new PropertyMetadata("Action disallowed - Please press and hold safety button first"));

    /// <summary>The error visible property.</summary>
    public static readonly DependencyProperty ErrorVisibleProperty = DependencyProperty.Register(
        nameof(ErrorVisible),
        typeof(Visibility),
        typeof(NumericPushButton),
        new PropertyMetadata(Visibility.Hidden));

    /// <summary>The mask color property.</summary>
    public static readonly DependencyProperty MaskColorProperty = DependencyProperty.Register(
        nameof(MaskColor),
        typeof(Brush),
        typeof(NumericPushButton),
        new PropertyMetadata(Brushes.Black, UpdateMask));

    /// <summary>Maximum of the size of value.</summary>
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum),
        typeof(double?),
        typeof(NumericPushButton),
        new UIPropertyMetadata(double.MaxValue, MaximumChanged));

    /// <summary>Minimum size of value.</summary>
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum),
        typeof(double?),
        typeof(NumericPushButton),
        new UIPropertyMetadata(double.MinValue, MinimumChanged));

    /// <summary>The show keypad property.</summary>
    public static readonly DependencyProperty ShowKeypadProperty = DependencyProperty.Register(
        nameof(ShowKeypad),
        typeof(ReactiveCommand<Unit, Unit>),
        typeof(NumericPushButton),
        new PropertyMetadata(null));

    /// <summary>The units on new line property.</summary>
    public static readonly DependencyProperty UnitsOnNewLineProperty = DependencyProperty.Register(
        nameof(UnitsOnNewLine),
        typeof(bool),
        typeof(NumericPushButton),
        new PropertyMetadata(false, UpdateNewLine));

    /// <summary>The units on new line property.</summary>
    public static readonly DependencyProperty UseSeperateEditValueProperty = DependencyProperty.Register(
        nameof(UseSeperateEditValue),
        typeof(bool),
        typeof(NumericPushButton),
        new PropertyMetadata(false, UpdateValue));

    /// <summary>Units Dependency Property.</summary>
    public static readonly DependencyProperty UnitsProperty = DependencyProperty.Register(
        nameof(Units),
        typeof(string),
        typeof(NumericPushButton),
        new PropertyMetadata("Units", UnitsChanged));

    /// <summary>Value Change.</summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(double),
        typeof(NumericPushButton),
        new PropertyMetadata(0D, UpdateValue));

    /// <summary>Edited Value Change.</summary>
    public static readonly DependencyProperty EditedValueProperty = DependencyProperty.Register(
        nameof(EditedValue),
        typeof(double),
        typeof(NumericPushButton),
        new PropertyMetadata(0D, UpdateValue));

    /// <summary>The use criss cross theme manager property.</summary>
    public static readonly DependencyProperty UseCrissCrossThemeManagerProperty = DependencyProperty.Register(
        nameof(UseCrissCrossThemeManager),
        typeof(bool),
        typeof(NumericPushButton),
        new PropertyMetadata(true, UpdateUseCrissCross));

    /// <summary>Delay before collapsing the keypad after the owner is disabled.</summary>
    private const int IsEnabledFalseDelayMilliseconds = 100;

    /// <summary>Delay before hiding the error state.</summary>
    private const int ErrorVisibilityDelaySeconds = 2;

    /// <summary>Stores the _valueD value.</summary>
    private readonly ReplaySignal<(bool UserChanged, double Value)> _valueD = new(1);

    /// <summary>Stores the _valueF value.</summary>
    private readonly ReplaySignal<(bool UserChanged, float Value)> _valueF = new(1);

    /// <summary>Stores the _keypadDisposable value.</summary>
    private readonly CompositeDisposable _keypadDisposable = [];

    /// <summary>Stores the _errrorTimer value.</summary>
    private DispatcherTimer _errrorTimer = null!;

    /// <summary>Stores the _isEnabledFalseTimer value.</summary>
    private DispatcherTimer _isEnabledFalseTimer = null!;

    /// <summary>Stores the _keypad value.</summary>
    private NumberPad? _keypad;

    /// <summary>Stores the _disposedvalue.</summary>
    private bool _disposedValue;

    /// <summary>Initializes a new instance of the <see cref="NumericPushButton"/> class.</summary>
    public NumericPushButton()
    {
        DefaultStyleKey = typeof(NumericPushButton);
    }

    /// <summary>Value changed and if user changed it</summary>
    public event EventHandler<NumberPadValueChangedEventArgs>? ValueChanged;

    /// <summary>Gets or sets a value indicating whether [use criss cross theme manager].</summary>
    /// <value>
    ///   <c>true</c> if [use criss cross theme manager]; otherwise, <c>false</c>.
    /// </value>
    [Description("Gets or sets a value indicating whether to use CrissCross Theme Manager or not")]
    [Category("Common")]
    public bool UseCrissCrossThemeManager
    {
        get => (bool)GetValue(UseCrissCrossThemeManagerProperty);
        set => SetValue(UseCrissCrossThemeManagerProperty, value);
    }

    /// <summary>Gets or sets how many decimal places to visualize.</summary>
    [Description("Gets or sets how many decimal places to visualize")]
    [Category("Common")]
    public int DecimalPlaces
    {
        get => (int)GetValue(DecimalPlacesProperty);
        set => SetValue(DecimalPlacesProperty, value);
    }

    /// <summary>Gets or sets the error text.</summary>
    /// <value>The error text.</value>
    [Description("Gets or sets the Error Text")]
    [Category("Common")]
    public string ErrorText
    {
        get => (string)GetValue(ErrorTextProperty);
        set => SetValue(ErrorTextProperty, value);
    }

    /// <summary>Gets or sets the error visible.</summary>
    /// <value>The error visible.</value>
    [Description("Gets or sets the Error Visibility.")]
    [Category("Common")]
    public Visibility ErrorVisible
    {
        get => (Visibility)GetValue(ErrorVisibleProperty);
        set => SetValue(ErrorVisibleProperty, value);
    }

    /// <summary>Gets or sets the color of the mask.</summary>
    /// <value>The color of the mask.</value>
    [Description("Sets MaskColor of the Keypad")]
    [Category("Brush")]
    public Brush MaskColor
    {
        get => (Brush)GetValue(MaskColorProperty);
        set => SetValue(MaskColorProperty, value);
    }

    /// <summary>Gets or sets the Maximum value allowed.</summary>
    [Description("Gets or sets the Maximum value allowed")]
    [Category("Common")]
    public double? Maximum
    {
        get => (double?)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>Gets or sets the Minimum value.</summary>
    [Description("Gets or sets the Minimum value allowed")]
    [Category("Common")]
    public double? Minimum
    {
        get => (double?)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>Gets or sets the show keypad.</summary>
    /// <value>The show keypad.</value>
    public ReactiveCommand<Unit, Unit> ShowKeypad
    {
        get => (ReactiveCommand<Unit, Unit>)GetValue(ShowKeypadProperty);
        set => SetValue(ShowKeypadProperty, value);
    }

    /// <summary>Gets or sets the Units to display.</summary>
    [Description("Gets or sets the Units to display")]
    [Category("Common")]
    public string Units
    {
        get => (string)GetValue(UnitsProperty);
        set => SetValue(UnitsProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether [units on new line].</summary>
    /// <value><c>true</c> if [units on new line]; otherwise, <c>false</c>.</value>
    [Description("Gets or sets a value indicating whether units is on a new line or not")]
    [Category("Common")]
    public bool UnitsOnNewLine
    {
        get => (bool)GetValue(UnitsOnNewLineProperty);
        set => SetValue(UnitsOnNewLineProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether [units on new line].</summary>
    /// <value><c>true</c> if [units on new line]; otherwise, <c>false</c>.</value>
    [Description("Gets or sets a value indicating whether edited value and value are stored seperately or not")]
    [Category("Common")]
    public bool UseSeperateEditValue
    {
        get => (bool)GetValue(UseSeperateEditValueProperty);
        set => SetValue(UseSeperateEditValueProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the user changed.</summary>
    /// <value><c>true</c> if [user changed]; otherwise, <c>false</c>.</value>
    public bool UserChanged { get; set; }

    /// <summary>Gets or sets the Value of Button.</summary>
    [Description("Gets or sets the Value of Button")]
    [Category("Common")]
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>Gets or sets the Value of Button.</summary>
    [Description("Gets or sets the Edited Value of Button")]
    [Category("Common")]
    public double EditedValue
    {
        get => (double)GetValue(EditedValueProperty);
        set => SetValue(EditedValueProperty, value);
    }

    /// <summary>Gets the value double observable and if the user changed it.</summary>
    /// <value>The value double observable.</value>
    public IObservable<(bool UserChanged, double Value)> ValueDoubleObservable => _valueD;

    /// <summary>Gets the value single.</summary>
    /// <value>The value single.</value>
    public float ValueSingle => (float)(double)GetValue(ValueProperty);

    /// <summary>Gets the value single observable and if the user changed it.</summary>
    /// <value>The value single observable.</value>
    public IObservable<(bool UserChanged, float Value)> ValueSingleObservable => _valueF;

    /// <summary>Gets a value indicating whether this instance is in design mode.</summary>
    /// <value>
    ///   <c>true</c> if this instance is in design mode; otherwise, <c>false</c>.
    /// </value>
    protected bool InDesignMode => DesignerProperties.GetIsInDesignMode(this);

    /// <summary>Disposes the keypad.</summary>
    public void DisposeKeypad()
    {
        _keypad?.Dispose();
        _keypad = null;
    }

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Disposes the specified disposing.</summary>
    /// <param name="disposing">if set to <c>true</c> [disposing].</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _errrorTimer?.Stop();
            _isEnabledFalseTimer?.Stop();
            _valueD.Dispose();
            _valueF.Dispose();
            _keypad?.Dispose();
            _keypadDisposable.Dispose();
        }

        _disposedValue = true;
    }

    /// <summary>Maximum Changed Dependency.</summary>
    /// <param name="d">Dependency Object.</param>
    /// <param name="e">Dependency Property Changed Event Arguments.</param>
    private static void MaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NumericPushButton sb || sb._keypad is not NumberPad keypad || e.NewValue is not double max)
        {
            return;
        }

        keypad.Value.Maximum = max;
        if (sb.Value <= max)
        {
            return;
        }

        sb.Value = max;
        keypad.Value.Value = sb.Value;
    }

    /// <summary>The minimum Changed.</summary>
    /// <param name="d">Dependency Object.</param>
    /// <param name="e">Dependency Property Changed Event Arguments.</param>
    private static void MinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NumericPushButton sb || sb._keypad is not NumberPad keypad || e.NewValue is not double minVal)
        {
            return;
        }

        keypad.Value.Minimum = minVal;
        if (sb.Value >= minVal)
        {
            return;
        }

        sb.Value = minVal;
        keypad.Value.Value = minVal;
    }

    /// <summary>Change the units displayed.</summary>
    /// <param name="d">Dependency Object.</param>
    /// <param name="e">Dependency Property Changed Event Arguments.</param>
    private static void UnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NumericPushButton sb || e.OldValue == e.NewValue)
        {
            return;
        }

        if (sb._keypad is not null)
        {
            sb._keypad.Unit.Content = e.NewValue.ToString();
        }

        _ = sb.UpdateSpinButtonContent();
    }

    /// <summary>Update the decimals.</summary>
    /// <param name="d">the dependency object.</param>
    /// <param name="e">Dependency Property Changed Event Arguments.</param>
    private static void UpdateDecimalPlaces(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NumericPushButton sb || sb._keypad is not NumberPad keypad || e.OldValue == e.NewValue)
        {
            return;
        }

        keypad.Value.MaxDecimalPlaces = (int)e.NewValue;
        _ = sb.UpdateSpinButtonContent();
    }

    /// <summary>Updates the mask.</summary>
    /// <param name="d">The dependency object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void UpdateMask(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NumericPushButton c || c._keypad is not NumberPad keypad)
        {
            return;
        }

        c.MaskColor = (Brush)e.NewValue;
        keypad.MaskColor = c.MaskColor;
    }

    /// <summary>Updates the new line.</summary>
    /// <param name="d">The dependency object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void UpdateNewLine(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NumericPushButton sb || e.OldValue == e.NewValue)
        {
            return;
        }

        _ = sb.UpdateSpinButtonContent();
    }

    /// <summary>Updates the use criss cross.</summary>
    /// <param name="d">The d.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void UpdateUseCrissCross(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NumericPushButton c || c._keypad is not NumberPad keypad)
        {
            return;
        }

        c.UseCrissCrossThemeManager = (bool)e.NewValue;
        keypad.UseCrissCrossThemeManager = c.UseCrissCrossThemeManager;
    }

    /// <summary>Updates the Value that was changed.</summary>
    /// <param name="d">the dependency object.</param>
    /// <param name="e">Dependency Property Changed Event Arguments.</param>
    private static void UpdateValue(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (
            e.NewValue is not double newdbl
            || e.OldValue is not double olddbl
            || DoubleComparison.AreClose(newdbl, olddbl)
            || d is not NumericPushButton sb)
        {
            return;
        }

        if (newdbl <= sb.Maximum && newdbl >= sb.Minimum)
        {
            sb.UpdateText();
        }
        else
        {
            sb.Value = olddbl;
        }
    }

    /// <summary>Update the text of button.</summary>
    private void UpdateText()
    {
        _ = this.UpdateSpinButtonContent();
        var v = (UserChanged, Value);
        if (_valueD.HasObservers)
        {
            _valueD.OnNext(v);
        }

        if (_valueF.HasObservers)
        {
            _valueF.OnNext((UserChanged, (float)Value));
        }

        ValueChanged?.Invoke(this, new NumberPadValueChangedEventArgs(UserChanged, Value));
        UserChanged = false;
    }
}
