// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfGrid = System.Windows.Controls.Grid;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Interaction logic for SquareSlider.</summary>
public sealed partial class SquareSlider : UserControl, INotifyPropertyChanged
{
    /// <summary>Provides the HueProperty member.</summary>
    public static readonly DependencyProperty HueProperty = DependencyProperty.Register(
        nameof(Hue),
        typeof(double),
        typeof(SquareSlider),
        new PropertyMetadata(0.0, OnHueChanged));

    /// <summary>Provides the HeadXProperty member.</summary>
    public static readonly DependencyProperty HeadXProperty = DependencyProperty.Register(
        nameof(HeadX),
        typeof(double),
        typeof(SquareSlider),
        new PropertyMetadata(0.0));

    /// <summary>Provides the HeadYProperty member.</summary>
    public static readonly DependencyProperty HeadYProperty = DependencyProperty.Register(
        nameof(HeadY),
        typeof(double),
        typeof(SquareSlider),
        new PropertyMetadata(0.0));

    /// <summary>Provides the PickerTypeProperty member.</summary>
    public static readonly DependencyProperty PickerTypeProperty = DependencyProperty.Register(
        nameof(PickerType),
        typeof(PickerType),
        typeof(SquareSlider),
        new PropertyMetadata(PickerType.HSV, OnColorSpaceChanged));

    /// <summary>Provides the width and height of the generated gradient bitmap.</summary>
    private const int GradientBitmapDimension = 32;

    /// <summary>Provides the default bitmap resolution in dots per inch.</summary>
    private const double DefaultBitmapDpi = 96D;

    /// <summary>Provides the number of channels in an RGB pixel.</summary>
    private const int RgbChannelCount = 3;

    /// <summary>Provides the offset of the green channel in an RGB pixel.</summary>
    private const int GreenChannelOffset = 1;

    /// <summary>Provides the offset of the blue channel in an RGB pixel.</summary>
    private const int BlueChannelOffset = 2;

    /// <summary>Stores the _rangeX value.</summary>
    private double _rangeX;

    /// <summary>Stores the _rangeY value.</summary>
    private double _rangeY;

    /// <summary>Stores the _gradientBitmap value.</summary>
    private WriteableBitmap? _gradientBitmap;

    /// <summary>Stores the _colorSpaceConversionMethod value.</summary>
    private Func<double, double, double, Tuple<double, double, double>> _colorSpaceConversionMethod =
        ColorSpaceHelper.HsvToRgb;

    /// <summary>Initializes a new instance of the <see cref="SquareSlider"/> class.</summary>
    public SquareSlider()
    {
        GradientBitmap = new(
            GradientBitmapDimension,
            GradientBitmapDimension,
            DefaultBitmapDpi,
            DefaultBitmapDpi,
            PixelFormats.Rgb24,
            null);
        InitializeComponent();
        RecalculateGradient();
    }

    /// <summary>Provides the PropertyChanged member.</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Gets or sets Hue.</summary>
    public double Hue
    {
        get => (double)GetValue(HueProperty);
        set => SetValue(HueProperty, value);
    }

    /// <summary>Gets or sets HeadX.</summary>
    public double HeadX
    {
        get => (double)GetValue(HeadXProperty);
        set => SetValue(HeadXProperty, value);
    }

    /// <summary>Gets or sets HeadY.</summary>
    public double HeadY
    {
        get => (double)GetValue(HeadYProperty);
        set => SetValue(HeadYProperty, value);
    }

    /// <summary>Gets or sets PickerType.</summary>
    public PickerType PickerType
    {
        get => (PickerType)GetValue(PickerTypeProperty);
        set => SetValue(PickerTypeProperty, value);
    }

    /// <summary>Gets or sets RangeX.</summary>
    public double RangeX
    {
        get => _rangeX;
        set
        {
            _rangeX = value;
            RaisePropertyChanged(nameof(RangeX));
        }
    }

    /// <summary>Gets or sets RangeY.</summary>
    public double RangeY
    {
        get => _rangeY;
        set
        {
            _rangeY = value;
            RaisePropertyChanged(nameof(RangeY));
        }
    }

    /// <summary>Gets or sets GradientBitmap.</summary>
    public WriteableBitmap? GradientBitmap
    {
        get => _gradientBitmap;
        set
        {
            _gradientBitmap = value;
            RaisePropertyChanged(nameof(GradientBitmap));
        }
    }

    /// <summary>Provides the OnColorSpaceChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="args">The event arguments.</param>
    private static void OnColorSpaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        var sender = (SquareSlider)d;
        if ((PickerType)args.NewValue == PickerType.HSV)
        {
            sender._colorSpaceConversionMethod = ColorSpaceHelper.HsvToRgb;
        }
        else
        {
            sender._colorSpaceConversionMethod = ColorSpaceHelper.HslToRgb;
        }

        sender.RecalculateGradient();
    }

    /// <summary>Provides the OnHueChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="_">Unused event arguments required by the dependency property callback.</param>
    private static void OnHueChanged(DependencyObject d, DependencyPropertyChangedEventArgs _) =>
        ((SquareSlider)d).RecalculateGradient();

    /// <summary>Provides the RecalculateGradient member.</summary>
    private void RecalculateGradient()
    {
        var w = GradientBitmap!.PixelWidth;
        var h = GradientBitmap.PixelHeight;
        var hue = Hue;
        var pixels = new byte[w * h * RgbChannelCount];
        for (var j = 0; j < h; j++)
        {
            for (var i = 0; i < w; i++)
            {
                var rgbtuple = _colorSpaceConversionMethod(hue, i / (double)(w - 1), (h - 1 - j) / (double)(h - 1));
                double r = rgbtuple.Item1;
                double g = rgbtuple.Item2;
                double b = rgbtuple.Item3;
                var pos = ((j * w) + i) * RgbChannelCount;
                pixels[pos] = (byte)(r * byte.MaxValue);
                pixels[pos + GreenChannelOffset] = (byte)(g * byte.MaxValue);
                pixels[pos + BlueChannelOffset] = (byte)(b * byte.MaxValue);
            }
        }

        GradientBitmap.WritePixels(new Int32Rect(0, 0, w, h), pixels, w * RgbChannelCount, 0);
    }

    /// <summary>Provides the OnMouseDown member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _ = ((UIElement)sender).CaptureMouse();
        UpdatePos(e.GetPosition(this));
    }

    /// <summary>Provides the OnMouseMove member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var grid = (WpfGrid)sender;
        if (!grid.IsMouseCaptured)
        {
            return;
        }

        UpdatePos(e.GetPosition(this));
    }

    /// <summary>Provides the UpdatePos member.</summary>
    /// <param name="pos">The pos value.</param>
    private void UpdatePos(Point pos)
    {
        HeadX = MathExtensions.Clamp(pos.X / ActualWidth, 0, 1) * RangeX;
        HeadY = (1 - MathExtensions.Clamp(pos.Y / ActualHeight, 0, 1)) * RangeY;
    }

    /// <summary>Provides the OnMouseUp member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="_">Unused event arguments required by the mouse event handler.</param>
    private void OnMouseUp(object sender, MouseButtonEventArgs _) => ((UIElement)sender).ReleaseMouseCapture();

    /// <summary>Provides the RaisePropertyChanged member.</summary>
    /// <param name="property">The property value.</param>
    private void RaisePropertyChanged(string property)
    {
        if (property is null)
        {
            return;
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}
