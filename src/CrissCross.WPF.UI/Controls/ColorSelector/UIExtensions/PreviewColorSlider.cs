// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.UIExtensions;
#else
namespace CrissCross.WPF.UI.UIExtensions;
#endif

/// <summary>Provides the PreviewColorSlider member.</summary>
public abstract class PreviewColorSlider : Slider, INotifyPropertyChanged
{
    /// <summary>Provides the CurrentColorStateProperty member.</summary>
    public static readonly DependencyProperty CurrentColorStateProperty = DependencyProperty.Register(
        nameof(CurrentColorState),
        typeof(ColorState),
        typeof(PreviewColorSlider),
        new PropertyMetadata(ColorStateChangedCallback));

    /// <summary>Provides the SmallChangeBindableProperty member.</summary>
    public static readonly DependencyProperty SmallChangeBindableProperty = DependencyProperty.Register(
        nameof(SmallChangeBindable),
        typeof(double),
        typeof(PreviewColorSlider),
        new PropertyMetadata(1.0, SmallChangeBindableChangedCallback));

    /// <summary>The scale used to convert normalized color channels to byte channel values.</summary>
    private protected const double ColorChannelScale = byte.MaxValue;

    /// <summary>The minimum value for an ARGB byte channel.</summary>
    private protected const int MinimumColorChannelValue = byte.MinValue;

    /// <summary>The maximum value for an ARGB byte channel.</summary>
    private protected const int MaximumColorChannelValue = byte.MaxValue;

    /// <summary>The midpoint value for an ARGB byte channel.</summary>
    private protected const int MidpointColorChannelValue = 128;

    /// <summary>The minimum hue angle in degrees.</summary>
    private protected const int MinimumHueDegrees = 0;

    /// <summary>The full hue-circle angle in degrees.</summary>
    private protected const int FullHueDegrees = 360;

    /// <summary>The yellow hue stop angle in degrees.</summary>
    private protected const int YellowHueDegrees = 60;

    /// <summary>The green hue stop angle in degrees.</summary>
    private protected const int GreenHueDegrees = 120;

    /// <summary>The cyan hue stop angle in degrees.</summary>
    private protected const int CyanHueDegrees = 180;

    /// <summary>The blue hue stop angle in degrees.</summary>
    private protected const int BlueHueDegrees = 240;

    /// <summary>The magenta hue stop angle in degrees.</summary>
    private protected const int MagentaHueDegrees = 300;

    /// <summary>The gradient offset for the yellow hue stop.</summary>
    private protected const double YellowGradientOffset = 1D / 6D;

    /// <summary>The gradient offset for the green hue stop.</summary>
    private protected const double GreenGradientOffset = 2D / 6D;

    /// <summary>The gradient offset for the cyan hue stop.</summary>
    private protected const double CyanGradientOffset = 0.5D;

    /// <summary>The gradient offset for the blue hue stop.</summary>
    private protected const double BlueGradientOffset = 4D / 6D;

    /// <summary>The gradient offset for the magenta hue stop.</summary>
    private protected const double MagentaGradientOffset = 5D / 6D;

    /// <summary>The default large-change step for preview sliders.</summary>
    private const double DefaultLargeChange = 10D;

    /// <summary>The default minimum height for preview sliders.</summary>
    private const double DefaultMinimumHeight = 12D;

    /// <summary>The WPF mouse wheel delta value for one wheel notch.</summary>
    private const int MouseWheelDelta = 120;

    /// <summary>Stores the _backgroundBrush value.</summary>
    private readonly LinearGradientBrush _backgroundBrush = new();

    /// <summary>Initializes a new instance of the <see cref="PreviewColorSlider"/> class.</summary>
    protected PreviewColorSlider()
    {
        Minimum = MinimumColorChannelValue;
        Maximum = MaximumColorChannelValue;
        SmallChange = 1;
        LargeChange = DefaultLargeChange;
        MinHeight = DefaultMinimumHeight;
        PreviewMouseWheel += OnPreviewMouseWheel;
    }

    /// <summary>Provides the PropertyChanged member.</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Gets or sets SmallChangeBindable.</summary>
    public double SmallChangeBindable
    {
        get => (double)GetValue(SmallChangeBindableProperty);
        set => SetValue(SmallChangeBindableProperty, value);
    }

    /// <summary>Gets or sets CurrentColorState.</summary>
    public ColorState CurrentColorState
    {
        get => (ColorState)GetValue(CurrentColorStateProperty);
        set => SetValue(CurrentColorStateProperty, value);
    }

    /// <summary>Gets BackgroundGradient.</summary>
    public GradientStopCollection BackgroundGradient => _backgroundBrush.GradientStops;

    /// <summary>Gets or sets LeftCapColor.</summary>
    public SolidColorBrush LeftCapColor
    {
        get => field;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LeftCapColor)));
        }
    } = new();

    /// <summary>Gets or sets RightCapColor.</summary>
    public SolidColorBrush RightCapColor
    {
        get => field;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RightCapColor)));
        }
    } = new();

    public override void EndInit()
    {
        base.EndInit();
        Background = _backgroundBrush;
        GenerateBackground();
    }

    /// <summary>Provides the ColorStateChangedCallback member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    protected static void ColorStateChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var slider = (PreviewColorSlider)d;
        slider.GenerateBackground();
    }

    /// <summary>Provides the GenerateBackground member.</summary>
    protected abstract void GenerateBackground();

    /// <summary>Replaces the gradient stops while retaining the exposed collection instance.</summary>
    /// <param name="gradientStops">The new gradient stops.</param>
    protected void SetBackgroundGradient(IEnumerable<GradientStop> gradientStops)
    {
        BackgroundGradient.Clear();
        foreach (var gradientStop in gradientStops)
        {
            BackgroundGradient.Add(gradientStop);
        }
    }

    /// <summary>Provides the SmallChangeBindableChangedCallback member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void SmallChangeBindableChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((PreviewColorSlider)d).SmallChange = (double)e.NewValue;

    /// <summary>Provides the OnPreviewMouseWheel member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
    {
        Value = MathExtensions.Clamp(Value + (SmallChange * args.Delta / MouseWheelDelta), Minimum, Maximum);
        args.Handled = true;
    }
}
