// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;

namespace CrissCross.WPF.UI.UIExtensions;

/// <summary>Provides the PreviewColorSlider member.</summary>
internal abstract class PreviewColorSlider : Slider, INotifyPropertyChanged
{
    /// <summary>Provides the CurrentColorStateProperty member.</summary>
    public static readonly DependencyProperty CurrentColorStateProperty =
        DependencyProperty.Register(
            nameof(CurrentColorState),
            typeof(ColorState),
            typeof(PreviewColorSlider),
            new PropertyMetadata(ColorStateChangedCallback));

    /// <summary>Provides the SmallChangeBindableProperty member.</summary>
    public static readonly DependencyProperty SmallChangeBindableProperty =
        DependencyProperty.Register(
            nameof(SmallChangeBindable),
            typeof(double),
            typeof(PreviewColorSlider),
            new PropertyMetadata(1.0, SmallChangeBindableChangedCallback));

    /// <summary>Stores the _backgroundBrush value.</summary>
    private readonly LinearGradientBrush _backgroundBrush = new();

    /// <summary>Initializes a new instance of the <see cref="PreviewColorSlider"/> class.</summary>
    protected PreviewColorSlider()
    {
        Minimum = 0;
        Maximum = 255;
        SmallChange = 1;
        LargeChange = 10;
        MinHeight = 12;
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

    /// <summary>Gets or sets BackgroundGradient.</summary>
    public GradientStopCollection BackgroundGradient
    {
        get => _backgroundBrush.GradientStops;
        set => _backgroundBrush.GradientStops = value;
    }

    /// <summary>Gets or sets LeftCapColor.</summary>
    public SolidColorBrush LeftCapColor
    {
        get => field;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LeftCapColor)));
        }
    }
= new();

    /// <summary>Gets or sets RightCapColor.</summary>
    public SolidColorBrush RightCapColor
    {
        get => field;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RightCapColor)));
        }
    }
    = new();

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
        Value = MathExtensions.Clamp(Value + (SmallChange * args.Delta / 120), Minimum, Maximum);
        args.Handled = true;
    }
}
