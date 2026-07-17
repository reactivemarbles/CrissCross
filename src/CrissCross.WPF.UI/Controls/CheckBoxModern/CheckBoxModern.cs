// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Check Box Modern.</summary>
/// <seealso cref="Control"/>
public partial class CheckBoxModern : Control, ICommandSource, IDisposable
{
    /// <summary>The automatic text font size property.</summary>
    public static readonly DependencyProperty AutoTextFontSizeProperty = DependencyProperty.Register(
        nameof(AutoTextFontSize),
        typeof(bool),
        typeof(CheckBoxModern),
        new PropertyMetadata(false, UpdateFontSize));

    /// <summary>The box size property.</summary>
    public static readonly DependencyProperty BoxSizeProperty = DependencyProperty.Register(
        nameof(BoxSize),
        typeof(double),
        typeof(CheckBoxModern),
        new PropertyMetadata(40D, UpdateBoxSize));

    /// <summary>The check background property.</summary>
    public static readonly DependencyProperty CheckBackgroundProperty = DependencyProperty.Register(
        nameof(CheckBackground),
        typeof(Brush),
        typeof(CheckBoxModern),
        new PropertyMetadata(Brushes.White));

    /// <summary>The is checked background property.</summary>
    public static readonly DependencyProperty IsCheckedBackgroundProperty = DependencyProperty.Register(
        nameof(IsCheckedBackground),
        typeof(Brush),
        typeof(CheckBoxModern),
        new PropertyMetadata(Brushes.LightGray));

    /// <summary>Value Change.</summary>
    public static readonly DependencyProperty CheckBoxTickFontSizeProperty = DependencyProperty.Register(
        nameof(CheckBoxTickFontSize),
        typeof(double),
        typeof(CheckBoxModern),
        new UIPropertyMetadata(40D));

    /// <summary>Value Change.</summary>
    public static readonly DependencyProperty CheckedProperty = DependencyProperty.Register(
        nameof(Checked),
        typeof(bool),
        typeof(CheckBoxModern),
        new FrameworkPropertyMetadata(
            false,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            CheckedPropertyChanged));

    /// <summary>The tick symbol property.</summary>
    public static readonly DependencyProperty CheckedSymbolProperty = DependencyProperty.Register(
        nameof(CheckedSymbol),
        typeof(Icons),
        typeof(CheckBoxModern),
        new PropertyMetadata(Icons.Tick, UpdateTickSymbol));

    /// <summary>The command parameter property.</summary>
    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
        nameof(CommandParameter),
        typeof(object),
        typeof(CheckBoxModern),
        new UIPropertyMetadata(null));

    /// <summary>The command property.</summary>
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command),
        typeof(ICommand),
        typeof(CheckBoxModern),
        new UIPropertyMetadata(null));

    /// <summary>The command target property.</summary>
    public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(
        nameof(CommandTarget),
        typeof(IInputElement),
        typeof(CheckBoxModern),
        new UIPropertyMetadata(null));

    /// <summary>The disabled state property.</summary>
    public static readonly DependencyProperty DisabledStateProperty = DependencyProperty.Register(
        nameof(DisabledState),
        typeof(DisabledState),
        typeof(CheckBoxModern),
        new PropertyMetadata(DisabledState.Ignore));

    /// <summary>The dock side property.</summary>
    public static readonly DependencyProperty DockSideProperty = DependencyProperty.Register(
        nameof(DockSide),
        typeof(Dock),
        typeof(CheckBoxModern),
        new PropertyMetadata(Dock.Left));

    /// <summary>The RadioButton style property.</summary>
    public static readonly DependencyProperty RadioButtonStyleProperty = DependencyProperty.Register(
        nameof(RadioButtonStyle),
        typeof(bool),
        typeof(CheckBoxModern),
        new PropertyMetadata(false));

    /// <summary>The stroke property.</summary>
    public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
        nameof(Stroke),
        typeof(Brush),
        typeof(CheckBoxModern),
        new UIPropertyMetadata(Brushes.Black));

    /// <summary>The stroke thickness property.</summary>
    public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
        nameof(StrokeThickness),
        typeof(double),
        typeof(CheckBoxModern),
        new UIPropertyMetadata(1D));

    /// <summary>Value Change.</summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(CheckBoxModern),
        new PropertyMetadata(string.Empty));

    /// <summary>Value Change.</summary>
    public static readonly DependencyProperty TickTextProperty = DependencyProperty.Register(
        nameof(TickText),
        typeof(string),
        typeof(CheckBoxModern),
        new PropertyMetadata(string.Empty));

    /// <summary>The unchecked symbol property.</summary>
    public static readonly DependencyProperty UncheckedSymbolProperty = DependencyProperty.Register(
        nameof(UncheckedSymbol),
        typeof(Icons),
        typeof(CheckBoxModern),
        new PropertyMetadata(Icons.None, UpdateTickSymbol));

    /// <summary>Value Change.</summary>
    public static readonly DependencyProperty WarningVisibleProperty = DependencyProperty.Register(
        nameof(WarningVisible),
        typeof(Visibility),
        typeof(CheckBoxModern),
        new PropertyMetadata(Visibility.Collapsed));

    /// <summary>The RGB channel used for the disabled check background.</summary>
    private const byte DisabledColorChannel = 137;

    /// <summary>The blue RGB channel used for the hover check background.</summary>
    private const byte HoverBlueChannel = 253;

    /// <summary>The green RGB channel used for the hover check background.</summary>
    private const byte HoverGreenChannel = 230;

    /// <summary>The red RGB channel used for the hover check background.</summary>
    private const byte HoverRedChannel = 190;

    /// <summary>The font-size scale used when automatic text sizing is enabled.</summary>
    private const double AutomaticFontSizeScale = 0.6D;

    /// <summary>The delay before applying disabled-state behavior.</summary>
    private const int DisabledStateDelayMilliseconds = 100;

    /// <summary>The duration that warning feedback remains visible.</summary>
    private const int WarningDisplayMilliseconds = 2000;

    /// <summary>Stores the _isChecked value.</summary>
    private readonly Signal<CheckBoxResultEventArgs> _isChecked = new();

    /// <summary>Stores the _disposables value.</summary>
    private readonly CompositeDisposable _disposables = [];

    /// <summary>Stores the _lastFontSize value.</summary>
    private double _lastFontSize;

    /// <summary>Stores the _startedTime value.</summary>
    private bool _startedTime;

    /// <summary>Stores the _disposedvalue.</summary>
    private bool _disposedValue;

    /// <summary>Provides the CheckBoxModern member.</summary>
    static CheckBoxModern() =>
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(CheckBoxModern),
            new FrameworkPropertyMetadata(typeof(CheckBoxModern)));

    /// <summary>Initializes a new instance of the <see cref="CheckBoxModern"/> class.</summary>
    public CheckBoxModern()
    {
        ObserveInput();
        ObserveLoaded();
        ObservePointerState();
    }

    /// <summary>Occurs when [value changed].</summary>
    public event EventHandler<CheckBoxResultEventArgs>? ValueChanged;

    /// <summary>Gets or sets a value indicating whether [automatic text font size].</summary>
    /// <value><c>true</c> if [automatic text font size]; otherwise, <c>false</c>.</value>
    [Description("Gets or sets a value indicating whether [automatic text font size].")]
    [Category("Common")]
    public bool AutoTextFontSize
    {
        get => (bool)GetValue(AutoTextFontSizeProperty);
        set => SetValue(AutoTextFontSizeProperty, value);
    }

    /// <summary>Gets or sets the size of the box.</summary>
    /// <value>The size of the box.</value>
    [Description("Gets or sets the size of the box.")]
    [Category("Common")]
    public double BoxSize
    {
        get => (double)GetValue(BoxSizeProperty);
        set => SetValue(BoxSizeProperty, value);
    }

    /// <summary>Gets or sets the check background.</summary>
    /// <value>The check background.</value>
    public Brush CheckBackground
    {
        get => (Brush)GetValue(CheckBackgroundProperty);
        set => SetValue(CheckBackgroundProperty, value);
    }

    /// <summary>Gets or sets the checked background.</summary>
    /// <value>The check background.</value>
    public Brush IsCheckedBackground
    {
        get => (Brush)GetValue(IsCheckedBackgroundProperty);
        set => SetValue(IsCheckedBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the CheckBox tick font. Reset this when changing the box size for
    /// automatic updating of this font size.
    /// </summary>
    /// <value>The size of the CheckBox tick font.</value>
    [Description(
        "Gets the size of the CheckBox tick font. Reset this when changing the box size "
            + "to update the font size automatically.")]
    [Category("Common")]
    public double CheckBoxTickFontSize
    {
        get => (double)GetValue(CheckBoxTickFontSizeProperty);
        set => SetValue(CheckBoxTickFontSizeProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the value of the box is checked.</summary>
    [Description("Gets or sets a value indicating whether the value of the box is checked")]
    [Category("Common")]
    public bool Checked
    {
        get => (bool)GetValue(CheckedProperty);
        set => SetValue(CheckedProperty, value);
    }

    /// <summary>Gets or sets the checked symbol.</summary>
    /// <value>The checked symbol.</value>
    [Description(" Gets or sets the checked symbol.")]
    [Category("Common")]
    public Icons CheckedSymbol
    {
        get => (Icons)GetValue(CheckedSymbolProperty);
        set => SetValue(CheckedSymbolProperty, value);
    }

    /// <summary>Gets or sets the command that will be executed when the command source is invoked.</summary>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>Gets or sets the GetValue value.</summary>
    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>Gets or sets the object that the command is being executed on.</summary>
    public IInputElement CommandTarget
    {
        get => (IInputElement)GetValue(CommandTargetProperty);
        set => SetValue(CommandTargetProperty, value);
    }

    /// <summary>Gets or sets the disabled the state.</summary>
    /// <value>The state of the disabled.</value>
    [Description("Gets or sets a value used when the box disabled of Ignored, Checked or UnChecked")]
    [Category("Common")]
    public DisabledState DisabledState
    {
        get => (DisabledState)GetValue(DisabledStateProperty);
        set => SetValue(DisabledStateProperty, value);
    }

    /// <summary>Gets or sets the dock side.</summary>
    /// <value>The dock side.</value>
    [Description("Gets or sets the dock side.")]
    [Category("Common")]
    public Dock DockSide
    {
        get => (Dock)GetValue(DockSideProperty);
        set => SetValue(DockSideProperty, value);
    }

    /// <summary>Gets the is checked.</summary>
    /// <value>The is checked.</value>
    [Description("Gets or sets the is checked.")]
    [Category("Common")]
    public IObservable<CheckBoxResultEventArgs> IsChecked => _isChecked;

    /// <summary>Gets or sets a value indicating whether [RadioButton style].</summary>
    /// <value><c>true</c> if [RadioButton style]; otherwise, <c>false</c>.</value>
    [Description("Gets or sets the is checked.")]
    [Category("Common")]
    public bool RadioButtonStyle
    {
        get => (bool)GetValue(RadioButtonStyleProperty);
        set => SetValue(RadioButtonStyleProperty, value);
    }

    /// <summary>Gets or sets the stroke.</summary>
    /// <value>The stroke.</value>
    [Description("Gets or sets the stroke of the border.")]
    public Brush Stroke
    {
        get => (Brush)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    /// <summary>Gets or sets the stroke thickness.</summary>
    /// <value>The stroke thickness.</value>
    [Description("Gets or sets the stroke thickness of the border. Reset this when using a radio button")]
    [Category("Common")]
    public double StrokeThickness
    {
        get => (double)GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    /// <summary>Gets or sets the Text of the Check box.</summary>
    [Description("Gets or sets the Text of the Check box")]
    [Category("Common")]
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>Gets the tick text.</summary>
    /// <value>The tick text.</value>
    [Description("Gets or sets the tick text.")]
    [Category("Common")]
    public string TickText
    {
        get => (string)GetValue(TickTextProperty);
        private set => SetValue(TickTextProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether [trigger disabled].</summary>
    /// <value><c>true</c> if [trigger disabled]; otherwise, <c>false</c>.</value>
    [Description("Gets or sets a value indicating whether [trigger disabled].")]
    [Category("Common")]
    public bool TriggerDisabled { get; set; }

    /// <summary>Gets or sets the unchecked symbol property.</summary>
    /// <value>The unchecked symbol property.</value>
    [Description(" Gets or sets the unchecked symbol.")]
    [Category("Common")]
    public Icons UncheckedSymbol
    {
        get => (Icons)GetValue(UncheckedSymbolProperty);
        set => SetValue(UncheckedSymbolProperty, value);
    }

    /// <summary>Gets the warning visible.</summary>
    /// <value>The warning visible.</value>
    [Description("Gets the warning visible.")]
    [Category("Common")]
    public Visibility WarningVisible
    {
        get => (Visibility)GetValue(WarningVisibleProperty);
        private set => SetValue(WarningVisibleProperty, value);
    }

    /// <summary>Gets the icon.</summary>
    /// <param name="icon">The icon.</param>
    /// <returns>string from list.</returns>
    public static string GetIcon(Icons icon) =>
        icon switch
        {
            Icons.Tick => "P",
            Icons.Cross => "O",
            Icons.StopSign => "X",
            Icons.Bin => "3",
            Icons.Hand => "N",
            Icons.ThumbsDown => "=",
            Icons.ZeroCircle => "i",
            Icons.OneCircle => "j",
            Icons.TwoCircle => "k",
            Icons.ThreeCircle => "l",
            Icons.FourCircle => "m",
            Icons.FiveCircle => "n",
            Icons.SixCircle => "o",
            Icons.SevenCircle => "p",
            Icons.EightCircle => "q",
            Icons.NineCircle => "r",
            Icons.TenCircle => "s",
            Icons.ZeroSolid => "t",
            Icons.OneSolid => "u",
            Icons.TwoSolid => "v",
            Icons.ThreeSolid => "w",
            Icons.FourSolid => "x",
            Icons.FiveSolid => "y",
            Icons.SixSolid => "z",
            Icons.SevenSolid => "{",
            Icons.EightSolid => "|",
            Icons.NineSolid => "}",
            Icons.TenSolid => "~",
            _ => string.Empty,
        };

    /// <summary>Shows the warning.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task ShowWarning() => ShowWarning(false);

    /// <summary>Shows the warning.</summary>
    /// <param name="checkedAfterwards">if set to <c>true</c> [checked afterwards].</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ShowWarning(bool checkedAfterwards)
    {
        WarningVisible = Visibility.Visible;
        await Task.Delay(WarningDisplayMilliseconds);
        Checked = checkedAfterwards;
        WarningVisible = Visibility.Collapsed;
    }

    /// <summary>Provides the Dispose member.</summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
    /// only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _isChecked.Dispose();
            _disposables.Dispose();
        }

        _disposedValue = true;
    }

    /// <summary>Checks the property changed.</summary>
    /// <param name="d">The dependency object.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void CheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CheckBoxModern chb || e.OldValue == e.NewValue)
        {
            return;
        }

        chb.TickText = chb.Checked ? GetIcon(chb.CheckedSymbol) : GetIcon(chb.UncheckedSymbol);
    }

    /// <summary>Provides the UpdateBoxSize member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void UpdateBoxSize(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CheckBoxModern cb)
        {
            return;
        }

        cb.CheckBoxTickFontSize = (double)e.NewValue;
        if (!cb.AutoTextFontSize)
        {
            return;
        }

        cb.FontSize = cb.BoxSize * AutomaticFontSizeScale;
    }

    /// <summary>Provides the UpdateFontSize member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void UpdateFontSize(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not CheckBoxModern cb)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            cb._lastFontSize = cb.FontSize;
            cb.FontSize = cb.BoxSize * AutomaticFontSizeScale;
        }
        else
        {
            cb.FontSize = cb._lastFontSize;
        }
    }

    /// <summary>Provides the UpdateTickSymbol member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="eventArgs">The event arguments.</param>
    private static void UpdateTickSymbol(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
    {
        _ = eventArgs;

        if (d is not CheckBoxModern cb)
        {
            return;
        }

        cb.TickText = cb.Checked ? GetIcon(cb.CheckedSymbol) : GetIcon(cb.UncheckedSymbol);
    }

    /// <summary>Provides the EnableChange member.</summary>
    /// <param name="result">The result value.</param>
    /// <returns>The result.</returns>
    private async Task EnableChange(bool result)
    {
        if (result)
        {
            CheckBackground = Brushes.White;
            _startedTime = false;
        }
        else
        {
            CheckBackground = new SolidColorBrush(
                Color.FromRgb(DisabledColorChannel, DisabledColorChannel, DisabledColorChannel));
            await StarTimer();
        }
    }

    /// <summary>Provides the StarTimer member.</summary>
    /// <returns>The result.</returns>
    private async Task StarTimer()
    {
        _startedTime = true;
        await Task.Delay(DisabledStateDelayMilliseconds);
        if (!_startedTime || DisabledState == DisabledState.Ignore)
        {
            return;
        }

        UpdateValue(false, DisabledState == DisabledState.Checked);
    }

    /// <summary>Provides the UpdateValue member.</summary>
    /// <param name="userClicked">The userClicked value.</param>
    /// <param name="result">The result value.</param>
    private void UpdateValue(bool userClicked, bool result)
    {
        var userStatus = new CheckBoxResultEventArgs(userClicked, result);

        if (Command is not null)
        {
            if (Command.CanExecute(null))
            {
                Command.Execute(CommandParameter ?? userStatus);
            }
            else if (DisabledState == DisabledState.Ignore)
            {
                return;
            }
        }

        Checked = !TriggerDisabled && result;
        if (_isChecked?.HasObservers == true)
        {
            _isChecked.OnNext(userStatus);
        }

        ValueChanged?.Invoke(this, userStatus);
    }
}
