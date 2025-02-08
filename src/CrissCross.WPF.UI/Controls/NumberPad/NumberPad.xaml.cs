// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ReactiveMarbles.ObservableEvents;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Interaction logic for NumberPad.
/// </summary>
public partial class NumberPad : IDisposable
{
    /// <summary>
    /// The hide mask property.
    /// </summary>
    public static readonly DependencyProperty HideMaskProperty =
        DependencyProperty.Register(
            nameof(HideMask),
            typeof(bool),
            typeof(NumberPad),
            new PropertyMetadata(false));

    /// <summary>
    /// The mask color property.
    /// </summary>
    public static readonly DependencyProperty MaskColorProperty =
        DependencyProperty.Register(
            nameof(MaskColor),
            typeof(Brush),
            typeof(NumberPad),
            new PropertyMetadata(Brushes.Black, UpdateMask));

    private readonly CompositeDisposable _disposables = [];
    private readonly DispatcherTimer _limitsTimer;
    private readonly double[] _margin = new double[4];
    private readonly INumberPadButton _owner;
    private string? _currentValue;
    private bool _disposedValue;
    private bool _hasFocus;

    /// <summary>
    /// Initializes a new instance of the <see cref="NumberPad"/> class.
    /// </summary>
    /// <param name="newOwner">The new owner.</param>
    /// <exception cref="ArgumentNullException">newOwner.</exception>
    /// <exception cref="ArgumentNullException">A ArgumentNullException.</exception>
    public NumberPad(INumberPadButton newOwner)
    {
        _owner = newOwner;
        if (_owner == null)
        {
            throw new ArgumentNullException(nameof(newOwner));
        }

        DataContext = this;
        SystemThemeWatcher.Watch(this);
        InitializeComponent();
        Unit.Content = _owner.Units;
        _owner.IsEnabled = false;
        this.Events().MouseLeftButtonDown
            .Merge(Mask.Events().MouseLeftButtonDown)
            .Subscribe(e =>
            {
                var mouse = e.GetPosition(this);
                var gridposition = WGrid.Margin;

                if (mouse.X < gridposition.Left || mouse.X > gridposition.Left + WGrid.Width || mouse.Y < gridposition.Top || mouse.Y > gridposition.Top + WGrid.Height)
                {
                    CloseKeypad();
                }
            }).DisposeWith(_disposables);
        this.Events().PreviewKeyDown
            .Subscribe(Window_PreviewKeyDown)
            .DisposeWith(_disposables);
        Value.Events().GotFocus.Select(_ => true)
            .Merge(Value.Events().LostFocus.Select(_ => false))
            .Subscribe(x => _hasFocus = x)
            .DisposeWith(_disposables);
        Accept.Events().Click
            .Subscribe(AcceptResult)
            .DisposeWith(_disposables);
        CancelBtn.Events().Click
            .Subscribe(_ => CloseKeypad())
            .DisposeWith(_disposables);
        ClearBtn.Events().Click
            .Subscribe(ClearValues)
            .DisposeWith(_disposables);
        _limitsTimer = new DispatcherTimer(
            TimeSpan.FromSeconds(1),
            DispatcherPriority.Normal,
            (s, e) =>
            {
                if (Value.Value.HasValue && Value.Value!.Value <= _owner.Maximum && Value.Value.Value >= _owner.Minimum)
                {
                    _limitsTimer?.Stop();
                }
            },
            Dispatcher);
        Showkeypad();
    }

    /// <summary>
    /// Gets or sets a value indicating whether [hide mask].
    /// </summary>
    /// <value><c>true</c> if [hide mask]; otherwise, <c>false</c>.</value>
    public bool HideMask
    {
        get => (bool)GetValue(HideMaskProperty); set => SetValue(HideMaskProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the mask.
    /// </summary>
    /// <value>The color of the mask.</value>
    public Brush MaskColor
    {
        get => (Brush)GetValue(MaskColorProperty); set => SetValue(MaskColorProperty, value);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting
    /// unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
    /// unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _limitsTimer?.Stop();
                _disposables?.Dispose();
                Close();
            }

            _disposedValue = true;
        }
    }

    private static void UpdateMask(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NumberPad c)
        {
            c.MaskColor = (Brush)e.NewValue;
            c.Mask.Background = (Brush)e.NewValue;
        }
    }

    /// <summary>
    /// Adds the digit.
    /// </summary>
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

    /// <summary>
    /// Clear Button Clicked.
    /// </summary>
    /// <param name="e">Routed Event Arguments.</param>
    private void ClearValues(RoutedEventArgs e)
    {
        Value.Value = double.NaN;
        Value.Text = string.Empty;
        _currentValue = string.Empty;
    }

    private async void CloseKeypad()
    {
        ClearValues(null!);
        await Task.Delay(20).ConfigureAwait(true);
        FadeOut();
        Value.Value = Value.Minimum > 0 ? Value.Minimum : Value.Maximum < 0 ? Value.Maximum : 0;
        _currentValue = Value.Value.Value.ToString(CultureInfo.InvariantCulture);
        _owner.DisposeKeypad();
        await Task.Delay(20).ConfigureAwait(true);
        _owner.IsEnabled = true;
    }

    /// <summary>
    /// Checks the limits.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Checked Limits.</returns>
    private double CheckTheLimits(double value)
    {
        Accept.IsEnabled = true;
        if (value > _owner.Maximum || value < _owner.Minimum)
        {
            _limitsTimer.Start();
            ////Value.Background = Brushes.Orange;
            Accept.IsEnabled = false;
        }

        return value;
    }

    /// <summary>
    /// Digit Pressed on key pad.
    /// </summary>
    /// <param name="sender">Digit button pressed.</param>
    /// <param name="e">Routed Event Arguments.</param>
    private void DigitPress(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Button button)
        {
            var key = button.Tag.ToString();
            switch (key)
            {
                case ".":
                    var value = Value.Value;
                    if (value != null)
                    {
                        _currentValue = $"{(int)value}{key}";
                    }

                    break;

                case "-":
                    InvertValue(key);
                    break;

                default:
                    AddDigit(key);
                    break;
            }
        }
    }

    /// <summary>
    /// Enter Button Clicked.
    /// </summary>
    /// <param name="e">Routed Event Arguments.</param>
    private async void AcceptResult(RoutedEventArgs e)
    {
        if (Value.Value.HasValue)
        {
            _owner.UserChanged = true;
            var check = CheckTheLimits(Value.Value.Value);
            if (!Accept.IsEnabled)
            {
                return;
            }

            _owner.Value = check;
            _owner.DisposeKeypad();
        }

        ClearValues(null!);
        await Task.Delay(20).ConfigureAwait(true);
        FadeOut();
        await Task.Delay(20).ConfigureAwait(true);
        _owner.IsEnabled = true;
    }

    private void FadeIn()
    {
        Visibility = Visibility.Visible;
        Focus();
    }

    private void FadeOut()
    {
        Visibility = Visibility.Collapsed;
        Dispose();
    }

    private void InvertValue(string key)
    {
        _currentValue = _currentValue?.Contains(key) == true ? _currentValue?.Remove(0, 1) : $"{key}{Value.Value}";

        if (double.TryParse(_currentValue, out var value))
        {
            Value.Value = value;
        }
    }

    private void Showkeypad()
    {
        Mask.Visibility = (Debugger.IsAttached || HideMask) ? Visibility.Collapsed : Visibility.Visible;
        Mask.Background = MaskColor;
        FadeIn();
        _currentValue = string.Empty;
        Value.Value = _owner.Minimum > 0 ? _owner.Minimum : _owner.Maximum < 0 ? _owner.Maximum : 0;

        var button = _owner as System.Windows.Controls.Button;
        var window = Window.GetWindow(button);

        WindowStartupLocation = WindowStartupLocation.Manual;

        // occupy owner screen
        if (window?.WindowState == WindowState.Maximized)
        {
            // Get the current screen
            WindowInteropHelper wih = new(window);
            if (User32.MonitorFromWindow(wih.Handle, User32.MONITOR_DEFAULTTONEAREST) is IntPtr monitor && monitor != IntPtr.Zero)
            {
                var monitorInfo = new User32.NativeMonitorInfo();
                User32.GetMonitorInfo(monitor, monitorInfo);

                Left = monitorInfo.Monitor.Left;
                Top = monitorInfo.Monitor.Top;
            }
        }
        else
        {
            Top = window!.Top;
            Left = window!.Left;
            Topmost = true;
        }

        Width = window!.ActualWidth;
        Height = window!.ActualHeight;

        var presentationSource = PresentationSource.FromVisual(button);
        if (window != null && button != null && presentationSource != null)
        {
            var ownerPosition = button.TransformToAncestor(presentationSource.RootVisual).Transform(new Point(0, 0));

            // Set the top position of the Keypad
            _margin[1] = ownerPosition.Y - 100;

            if ((_margin[1] + WGrid.Height) > window.ActualHeight)
            {
                _margin[1] = window.ActualHeight - WGrid.Height - 10;
            }

            // Set the left position of the Keypad
            var element = this.TryFindParent<Viewbox>();
            if (element != null)
            {
                var scaledWidth = element.ActualWidth / element.Child.DesiredSize.Width;
                _margin[2] = ownerPosition.X + (button.ActualWidth * (scaledWidth > 1 ? scaledWidth : 1)) + 10;
            }
            else
            {
                _margin[2] = ownerPosition.X + button.ActualWidth + 10;
            }

            if ((_margin[2] + WGrid.Width) > (window.ActualWidth - 10))
            {
                // Set location to left of button if the location + with of keypad will be off
                // the screen
                _margin[2] = ownerPosition.X - WGrid.Width;
            }

            if (_margin[2] > (window.ActualWidth - 10))
            {
                _margin[2] = window.ActualWidth - WGrid.Width - 10;
            }

            WGrid.Margin = new Thickness(_margin[2], _margin[1], 0, 0);
        }
    }

    private void Window_PreviewKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.NumPad0:
            case Key.D0:
                if (_hasFocus)
                {
                    return;
                }

                AddDigit("0");
                break;

            case Key.D1:
            case Key.NumPad1:
                if (_hasFocus)
                {
                    return;
                }

                AddDigit("1");
                break;

            case Key.D2:
            case Key.NumPad2:
                if (_hasFocus)
                {
                    return;
                }

                AddDigit("2");
                break;

            case Key.D3:
            case Key.NumPad3:
                if (_hasFocus)
                {
                    return;
                }

                AddDigit("3");
                break;

            case Key.D4:
            case Key.NumPad4:
                if (_hasFocus)
                {
                    return;
                }

                AddDigit("4");
                break;

            case Key.D5:
            case Key.NumPad5:
                if (_hasFocus)
                {
                    return;
                }

                AddDigit("5");
                break;

            case Key.D6:
            case Key.NumPad6:
                if (_hasFocus)
                {
                    return;
                }

                AddDigit("6");
                break;

            case Key.D7:
            case Key.NumPad7:
                if (_hasFocus)
                {
                    return;
                }

                AddDigit("7");
                break;

            case Key.D8:
            case Key.NumPad8:
                if (_hasFocus)
                {
                    return;
                }

                AddDigit("8");
                break;

            case Key.D9:
            case Key.NumPad9:
                if (_hasFocus)
                {
                    return;
                }

                AddDigit("9");
                break;

            case Key.Enter:

                AcceptResult(null!);
                break;

            case Key.Escape:
                CloseKeypad();
                break;

            case Key.OemMinus:
                InvertValue("-");
                break;

            case Key.OemPeriod:
            case Key.Decimal:
                var value = Value.Value;
                if (value != null)
                {
                    _currentValue = $"{(int)value}.";
                }

                break;

            default:
                if (e.Key == Key.Return)
                {
                    AcceptResult(null!);
                }

                break;
        }
    }
}
