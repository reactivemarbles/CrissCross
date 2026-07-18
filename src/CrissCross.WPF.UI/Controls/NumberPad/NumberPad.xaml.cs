// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Interaction logic for NumberPad.</summary>
public partial class NumberPad : IDisposable
{
    /// <summary>The hide mask property.</summary>
    public static readonly DependencyProperty HideMaskProperty = DependencyProperty.Register(
        nameof(HideMask),
        typeof(bool),
        typeof(NumberPad),
        new PropertyMetadata(false));

    /// <summary>The mask color property.</summary>
    public static readonly DependencyProperty MaskColorProperty = DependencyProperty.Register(
        nameof(MaskColor),
        typeof(Brush),
        typeof(NumberPad),
        new PropertyMetadata(Brushes.Black, UpdateMask));

    /// <summary>The use criss cross theme manager property.</summary>
    public static readonly DependencyProperty UseCrissCrossThemeManagerProperty = DependencyProperty.Register(
        nameof(UseCrissCrossThemeManager),
        typeof(bool?),
        typeof(NumberPad),
        new PropertyMetadata(null, UpdateTheme));

    /// <summary>Delay used before and after keypad fade operations.</summary>
    private const int CloseAnimationDelayMilliseconds = 20;

    /// <summary>Number of stored keypad margin values.</summary>
    private const int KeypadMarginCount = 4;

    /// <summary>Fallback scale when the owner is not scaled above its desired size.</summary>
    private const double ViewboxScaleFallback = 1.0;

    /// <summary>Horizontal spacing between the owner button and keypad.</summary>
    private const int KeypadHorizontalMargin = 10;

    /// <summary>Vertical offset used when positioning the keypad above the owner.</summary>
    private const int KeypadVerticalOffset = 100;

    /// <summary>Index of the top margin in the stored margin array.</summary>
    private const int MarginTopIndex = 1;

    /// <summary>Index of the left margin in the stored margin array.</summary>
    private const int MarginLeftIndex = 2;

    /// <summary>Stores the _disposables value.</summary>
    private readonly CompositeDisposable _disposables = [];

    /// <summary>Stores the _margin value.</summary>
    private readonly double[] _margin = new double[KeypadMarginCount];

    /// <summary>Stores the _owner value.</summary>
    private readonly INumberPadButton _owner;

    /// <summary>Stores the _limitsTimer value.</summary>
    private DispatcherTimer _limitsTimer = null!;

    /// <summary>Stores the _currentvalue.</summary>
    private string? _currentValue;

    /// <summary>Stores the _disposedvalue.</summary>
    private bool _disposedValue;

    /// <summary>Stores the _hasFocus value.</summary>
    private bool _hasFocus;

    /// <summary>Initializes a new instance of the <see cref="NumberPad"/> class.</summary>
    /// <exception cref="ArgumentNullException">newOwner.</exception>
    /// <exception cref="ArgumentNullException">A ArgumentNullException.</exception>
    /// <param name="newOwner">The new owner.</param>
    public NumberPad(INumberPadButton newOwner)
    {
        _owner = newOwner ?? throw new ArgumentNullException(nameof(newOwner));

        DataContext = this;

        InitializeComponent();
    }

    /// <summary>Gets or sets a value indicating whether [hide mask].</summary>
    /// <value><c>true</c> if [hide mask]; otherwise, <c>false</c>.</value>
    public bool HideMask
    {
        get => (bool)GetValue(HideMaskProperty);
        set => SetValue(HideMaskProperty, value);
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

    /// <summary>Gets or sets a value indicating whether [use criss cross theme manager].</summary>
    /// <value>
    ///   <c>true</c> if [use criss cross theme manager]; otherwise, <c>false</c>.
    /// </value>
    [Description("Gets or sets a value indicating whether to use CrissCross Theme Manager or not")]
    [Category("Common")]
    public bool? UseCrissCrossThemeManager
    {
        get => (bool?)GetValue(UseCrissCrossThemeManagerProperty);
        set => SetValue(UseCrissCrossThemeManagerProperty, value);
    }

    /// <summary>Provides the Dispose member.</summary>
    public void Dispose()
    {
        Dispose(true);
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
            _limitsTimer?.Stop();
            _disposables?.Dispose();
            Close();
        }

        _disposedValue = true;
    }

    /// <summary>Digit Pressed on key pad.</summary>
    /// <param name="sender">Digit button pressed.</param>
    /// <param name="e">Routed Event Arguments.</param>
    protected void DigitPress(object sender, RoutedEventArgs e)
    {
        if (sender is not System.Windows.Controls.Button button)
        {
            return;
        }

        var key = button.Tag.ToString();
        switch (key)
        {
            case ".":
            {
                var value = Value.Value;
                if (value is not null)
                {
                    _currentValue = $"{(int)value}{key}";
                }

                break;
            }

            case "-":
            {
                InvertValue(key);
                break;
            }

            default:
            {
                AddDigit(key);
                break;
            }
        }
    }

    /// <summary>Gets the initial value for the configured range.</summary>
    /// <param name="minimum">The minimum value.</param>
    /// <param name="maximum">The maximum value.</param>
    /// <returns>The initial value.</returns>
    private static double GetInitialValue(double? minimum, double? maximum)
    {
        if (minimum > 0)
        {
            return minimum.GetValueOrDefault();
        }

        return maximum < 0 ? maximum.GetValueOrDefault() : 0;
    }

    /// <summary>Provides the UpdateMask member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void UpdateMask(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NumberPad c)
        {
            return;
        }

        c.MaskColor = (Brush)e.NewValue;
        c.Mask.Background = (Brush)e.NewValue;
    }

    /// <summary>Provides the UpdateTheme member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void UpdateTheme(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NumberPad c || e.NewValue is not bool useTheme)
        {
            return;
        }

        if (useTheme)
        {
            SystemThemeWatcher.Watch(c);
        }
        else
        {
            _ = EventSignal
                .From<RoutedEventHandler, RoutedEventArgs>(
                    handler => handler.Invoke,
                    handler => c.Loaded += handler,
                    handler => c.Loaded -= handler)
                .Take(1)
                .Subscribe(_ => SystemThemeWatcher.UnWatch(c))
                .DisposeWith(c._disposables);
        }
    }

    /// <summary>Adds the digit.</summary>
    /// <param name="key">The key.</param>
    private void AddDigit(string? key)
    {
        _currentValue += key;
        if (double.TryParse(_currentValue, out var value))
        {
            Value.Value = CheckTheLimits(value);
        }

        _currentValue = Value.Text;
    }

    /// <summary>Clear Button Clicked.</summary>
    private void ClearValues()
    {
        Value.Value = double.NaN;
        Value.Text = string.Empty;
        _currentValue = string.Empty;
    }

    /// <summary>Starts closing the keypad.</summary>
    private void CloseKeypad() => _ = CloseKeypadAsync();

    /// <summary>Closes the keypad after its transition completes.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task CloseKeypadAsync()
    {
        ClearValues();
        await Task.Delay(CloseAnimationDelayMilliseconds).ConfigureAwait(true);
        FadeOut();
        Value.Value = GetInitialValue(Value.Minimum, Value.Maximum);
        _currentValue = Value.Value.Value.ToString(CultureInfo.InvariantCulture);
        _owner.DisposeKeypad();
        await Task.Delay(CloseAnimationDelayMilliseconds).ConfigureAwait(true);
        _owner.IsEnabled = true;
    }

    /// <summary>Checks the limits.</summary>
    /// <param name="value">The value.</param>
    /// <returns>Checked Limits.</returns>
    private double CheckTheLimits(double value)
    {
        Accept.IsEnabled = true;
        if (value > _owner.Maximum || value < _owner.Minimum)
        {
            _limitsTimer.Start();
            Accept.IsEnabled = false;
        }

        return value;
    }

    /// <summary>Starts accepting the current result.</summary>
    private void AcceptResult() => _ = AcceptResultAsync();

    /// <summary>Accepts the current result and completes the keypad transition.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AcceptResultAsync()
    {
        if (Value.Value.HasValue)
        {
            _owner.UserChanged = true;
            var check = CheckTheLimits(Value.Value.Value);
            if (!Accept.IsEnabled)
            {
                return;
            }

            if (_owner.UseSeperateEditValue)
            {
                _owner.EditedValue = check;
            }
            else
            {
                _owner.Value = check;
                _owner.EditedValue = check;
            }

            _owner.DisposeKeypad();
        }

        ClearValues();
        await Task.Delay(CloseAnimationDelayMilliseconds).ConfigureAwait(true);
        FadeOut();
        await Task.Delay(CloseAnimationDelayMilliseconds).ConfigureAwait(true);
        _owner.IsEnabled = true;
    }

    /// <summary>Provides the FadeIn member.</summary>
    private void FadeIn()
    {
        Visibility = Visibility.Visible;
        _ = Focus();
    }

    /// <summary>Provides the FadeOut member.</summary>
    private void FadeOut()
    {
        Visibility = Visibility.Collapsed;
        Dispose();
    }

    /// <summary>Provides the InvertValue member.</summary>
    /// <param name="key">The key value.</param>
    private void InvertValue(string key)
    {
        _currentValue = _currentValue?.Contains(key) == true ? _currentValue?.Remove(0, 1) : $"{key}{Value.Value}";

        if (!double.TryParse(_currentValue, out var value))
        {
            return;
        }

        Value.Value = value;
    }

    /// <summary>Provides the Showkeypad member.</summary>
    private void Showkeypad()
    {
        Mask.Visibility = (Debugger.IsAttached || HideMask) ? Visibility.Collapsed : Visibility.Visible;
        Mask.Background = MaskColor;
        FadeIn();
        _currentValue = string.Empty;
        Value.Value = GetInitialValue(_owner.Minimum, _owner.Maximum);

        if (
            _owner is not System.Windows.Controls.Button button
            || Window.GetWindow(button) is not { } window
            || PresentationSource.FromVisual(button) is not { } presentationSource)
        {
            return;
        }

        WindowStartupLocation = WindowStartupLocation.Manual;
        ApplyOwnerWindowBounds(window);

        Width = window.ActualWidth;
        Height = window.ActualHeight;

        var ownerPosition = button.TransformToAncestor(presentationSource.RootVisual).Transform(new Point(0, 0));
        SetTopMargin(window, ownerPosition);
        SetLeftMargin(button, window, ownerPosition);

        WGrid.Margin = new(_margin[MarginLeftIndex], _margin[MarginTopIndex], 0, 0);
    }

    /// <summary>Provides the Window_PreviewKeyDown member.</summary>
    /// <param name="e">The event arguments.</param>
    private void Window_PreviewKeyDown(KeyEventArgs e)
    {
        if (GetDigitFromKey(e.Key) is { } digit)
        {
            if (!_hasFocus)
            {
                AddDigit(digit);
            }

            return;
        }

        switch (e.Key)
        {
            case Key.Enter or Key.Return:
            {
                AcceptResult();
                break;
            }

            case Key.Escape:
            {
                CloseKeypad();
                break;
            }

            case Key.OemMinus:
            {
                InvertValue("-");
                break;
            }

            case Key.OemPeriod
            or Key.Decimal:
            {
                var value = Value.Value;
                if (value is not null)
                {
                    _currentValue = $"{(int)value}.";
                }

                break;
            }

            default:
            {
                break;
            }
        }

        static string? GetDigitFromKey(Key key)
        {
            if (key is >= Key.D0 and <= Key.D9)
            {
                return ((char)('0' + key - Key.D0)).ToString();
            }

            return key is >= Key.NumPad0 and <= Key.NumPad9 ? ((char)('0' + key - Key.NumPad0)).ToString() : null;
        }
    }

    /// <summary>Applies bounds from the owner window to the keypad.</summary>
    /// <param name="window">The owner window.</param>
    private void ApplyOwnerWindowBounds(System.Windows.Window window)
    {
        if (window.WindowState == WindowState.Maximized)
        {
            WindowInteropHelper wih = new(window);
            if (
                User32.MonitorFromWindow(wih.Handle, User32.MONITOR_DEFAULTTONEAREST) is IntPtr monitor
                && monitor != IntPtr.Zero)
            {
                var monitorInfo = new User32.NativeMonitorInfo();
                _ = User32.GetMonitorInfo(monitor, monitorInfo);

                Left = monitorInfo.Monitor.Left;
                Top = monitorInfo.Monitor.Top;
            }

            return;
        }

        Top = window.Top;
        Left = window.Left;
        Topmost = true;
    }

    /// <summary>Sets the keypad left margin beside the owner button.</summary>
    /// <param name="button">The owner button.</param>
    /// <param name="window">The owner window.</param>
    /// <param name="ownerPosition">The owner position.</param>
    private void SetLeftMargin(System.Windows.Controls.Button button, System.Windows.Window window, Point ownerPosition)
    {
        var element = this.TryFindParent<Viewbox>();
        if (element is not null)
        {
            var scaledWidth = element.ActualWidth / element.Child.DesiredSize.Width;
            _margin[MarginLeftIndex] =
                ownerPosition.X
                + (button.ActualWidth * (scaledWidth > ViewboxScaleFallback ? scaledWidth : ViewboxScaleFallback))
                + KeypadHorizontalMargin;
        }
        else
        {
            _margin[MarginLeftIndex] = ownerPosition.X + button.ActualWidth + KeypadHorizontalMargin;
        }

        _margin[MarginLeftIndex] =
            (_margin[MarginLeftIndex] + WGrid.Width) > (window.ActualWidth - KeypadHorizontalMargin)
                ? ownerPosition.X - WGrid.Width
                : _margin[MarginLeftIndex];
        _margin[MarginLeftIndex] =
            _margin[MarginLeftIndex] > (window.ActualWidth - KeypadHorizontalMargin)
                ? window.ActualWidth - WGrid.Width - KeypadHorizontalMargin
                : _margin[MarginLeftIndex];
    }

    /// <summary>Sets the keypad top margin near the owner button.</summary>
    /// <param name="window">The owner window.</param>
    /// <param name="ownerPosition">The owner position.</param>
    private void SetTopMargin(System.Windows.Window window, Point ownerPosition)
    {
        _margin[MarginTopIndex] = Math.Min(
            ownerPosition.Y - KeypadVerticalOffset,
            window.ActualHeight - WGrid.Height - KeypadHorizontalMargin);
    }
}
