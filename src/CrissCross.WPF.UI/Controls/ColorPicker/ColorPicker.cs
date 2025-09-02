// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a control that lets a user pick a color using a color spectrum, sliders, and text input.
/// </summary>
public class ColorPicker : Control
{
    /// <summary>
    /// Identifies the <see cref="SelectedColor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register(
            nameof(SelectedColor),
            typeof(Color),
            typeof(ColorPicker),
            new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorChanged));

    /// <summary>
    /// Identifies the <see cref="ShowAlpha"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowAlphaProperty =
        DependencyProperty.Register(
            nameof(ShowAlpha),
            typeof(bool),
            typeof(ColorPicker),
            new PropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="A"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AProperty =
        DependencyProperty.Register(
            nameof(A),
            typeof(double),
            typeof(ColorPicker),
            new FrameworkPropertyMetadata(255d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnChannelChanged, CoerceChannel));

    /// <summary>
    /// Identifies the <see cref="R"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RProperty =
        DependencyProperty.Register(
            nameof(R),
            typeof(double),
            typeof(ColorPicker),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnChannelChanged, CoerceChannel));

    /// <summary>
    /// Identifies the <see cref="G"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GProperty =
        DependencyProperty.Register(
            nameof(G),
            typeof(double),
            typeof(ColorPicker),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnChannelChanged, CoerceChannel));

    /// <summary>
    /// Identifies the <see cref="B"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BProperty =
        DependencyProperty.Register(
            nameof(B),
            typeof(double),
            typeof(ColorPicker),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnChannelChanged, CoerceChannel));

    private bool _updating;

    static ColorPicker()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
    }

    /// <summary>
    /// Gets or sets the selected color.
    /// </summary>
    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the alpha channel is shown and editable.
    /// </summary>
    public bool ShowAlpha
    {
        get => (bool)GetValue(ShowAlphaProperty);
        set => SetValue(ShowAlphaProperty, value);
    }

    /// <summary>
    /// Gets or sets the alpha channel value (0-255).
    /// </summary>
    public double A
    {
        get => (double)GetValue(AProperty);
        set => SetValue(AProperty, CoerceByteRange(value));
    }

    /// <summary>
    /// Gets or sets the red channel value (0-255).
    /// </summary>
    public double R
    {
        get => (double)GetValue(RProperty);
        set => SetValue(RProperty, CoerceByteRange(value));
    }

    /// <summary>
    /// Gets or sets the green channel value (0-255).
    /// </summary>
    public double G
    {
        get => (double)GetValue(GProperty);
        set => SetValue(GProperty, CoerceByteRange(value));
    }

    /// <summary>
    /// Gets or sets the blue channel value (0-255).
    /// </summary>
    public double B
    {
        get => (double)GetValue(BProperty);
        set => SetValue(BProperty, CoerceByteRange(value));
    }

    private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ColorPicker cp)
        {
            return;
        }

        if (cp._updating)
        {
            return;
        }

        var c = (Color)e.NewValue;
        try
        {
            cp._updating = true;
            cp.A = c.A;
            cp.R = c.R;
            cp.G = c.G;
            cp.B = c.B;
        }
        finally
        {
            cp._updating = false;
        }
    }

    private static void OnChannelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ColorPicker cp)
        {
            return;
        }

        if (cp._updating)
        {
            return;
        }

        var a = (byte)Math.Round(cp.A);
        var r = (byte)Math.Round(cp.R);
        var g = (byte)Math.Round(cp.G);
        var b = (byte)Math.Round(cp.B);

        try
        {
            cp._updating = true;
            cp.SelectedColor = Color.FromArgb(a, r, g, b);
        }
        finally
        {
            cp._updating = false;
        }
    }

    private static object CoerceChannel(DependencyObject d, object baseValue) => CoerceByteRange((double)baseValue);

    private static double CoerceByteRange(double value)
    {
        if (value < 0)
        {
            return 0;
        }

        if (value > 255)
        {
            return 255;
        }

        return value;
    }
}
