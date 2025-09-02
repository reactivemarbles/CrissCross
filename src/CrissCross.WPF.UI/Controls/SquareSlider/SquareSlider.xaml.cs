﻿// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CrissCross.WPF.UI;

internal partial class SquareSlider : UserControl, INotifyPropertyChanged
{
    public static readonly DependencyProperty HueProperty
        = DependencyProperty.Register(
            nameof(Hue),
            typeof(double),
            typeof(SquareSlider),
            new PropertyMetadata(0.0, OnHueChanged));

    public static readonly DependencyProperty HeadXProperty
        = DependencyProperty.Register(
            nameof(HeadX),
            typeof(double),
            typeof(SquareSlider),
            new PropertyMetadata(0.0));

    public static readonly DependencyProperty HeadYProperty
        = DependencyProperty.Register(
            nameof(HeadY),
            typeof(double),
            typeof(SquareSlider),
            new PropertyMetadata(0.0));

    public static readonly DependencyProperty PickerTypeProperty
        = DependencyProperty.Register(
            nameof(PickerType),
            typeof(PickerType),
            typeof(SquareSlider),
            new PropertyMetadata(PickerType.HSV, OnColorSpaceChanged));

    private double _rangeX;

    private double _rangeY;

    private WriteableBitmap? _gradientBitmap;

    private Func<double, double, double, Tuple<double, double, double>> _colorSpaceConversionMethod = ColorSpaceHelper.HsvToRgb;

    public SquareSlider()
    {
        GradientBitmap = new WriteableBitmap(32, 32, 96, 96, PixelFormats.Rgb24, null);
        InitializeComponent();
        RecalculateGradient();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public double Hue
    {
        get => (double)GetValue(HueProperty);
        set => SetValue(HueProperty, value);
    }

    public double HeadX
    {
        get => (double)GetValue(HeadXProperty);
        set => SetValue(HeadXProperty, value);
    }

    public double HeadY
    {
        get => (double)GetValue(HeadYProperty);
        set => SetValue(HeadYProperty, value);
    }

    public PickerType PickerType
    {
        get => (PickerType)GetValue(PickerTypeProperty);
        set => SetValue(PickerTypeProperty, value);
    }

    public double RangeX
    {
        get => _rangeX;
        set
        {
            _rangeX = value;
            RaisePropertyChanged(nameof(RangeX));
        }
    }

    public double RangeY
    {
        get => _rangeY;
        set
        {
            _rangeY = value;
            RaisePropertyChanged(nameof(RangeY));
        }
    }

    public WriteableBitmap? GradientBitmap
    {
        get => _gradientBitmap;
        set
        {
            _gradientBitmap = value;
            RaisePropertyChanged(nameof(GradientBitmap));
        }
    }

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

    private static void OnHueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args) => ((SquareSlider)d).RecalculateGradient();

    private void RecalculateGradient()
    {
        var w = GradientBitmap!.PixelWidth;
        var h = GradientBitmap.PixelHeight;
        var hue = Hue;
        var pixels = new byte[w * h * 3];
        for (var j = 0; j < h; j++)
        {
            for (var i = 0; i < w; i++)
            {
                var rgbtuple = _colorSpaceConversionMethod(hue, i / (double)(w - 1), (h - 1 - j) / (double)(h - 1));
                double r = rgbtuple.Item1, g = rgbtuple.Item2, b = rgbtuple.Item3;
                var pos = ((j * h) + i) * 3;
                pixels[pos] = (byte)(r * 255);
                pixels[pos + 1] = (byte)(g * 255);
                pixels[pos + 2] = (byte)(b * 255);
            }
        }

        GradientBitmap.WritePixels(new Int32Rect(0, 0, w, h), pixels, w * 3, 0);
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        ((UIElement)sender).CaptureMouse();
        UpdatePos(e.GetPosition(this));
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var grid = (System.Windows.Controls.Grid)sender;
        if (grid.IsMouseCaptured)
        {
            UpdatePos(e.GetPosition(this));
        }
    }

    private void UpdatePos(Point pos)
    {
        HeadX = MathHelper.Clamp(pos.X / ActualWidth, 0, 1) * RangeX;
        HeadY = (1 - MathHelper.Clamp(pos.Y / ActualHeight, 0, 1)) * RangeY;
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e) => ((UIElement)sender).ReleaseMouseCapture();

    private void RaisePropertyChanged(string property)
    {
        if (property != null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
