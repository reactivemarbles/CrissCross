// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;

namespace CrissCross.WPF.UI;

/// <summary>Interaction logic for HueSlider.</summary>
internal sealed partial class HueSlider : UserControl
{
    /// <summary>Provides the ValueProperty member.</summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(HueSlider),
            new PropertyMetadata(0.0));

    /// <summary>Provides the SmallChangeProperty member.</summary>
    public static readonly DependencyProperty SmallChangeProperty =
        DependencyProperty.Register(
            nameof(SmallChange),
            typeof(double),
            typeof(HueSlider),
            new PropertyMetadata(1.0));

    /// <summary>Initializes a new instance of the <see cref="HueSlider"/> class.</summary>
    public HueSlider() => InitializeComponent();

    /// <summary>Gets or sets Value.</summary>
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>Gets or sets SmallChange.</summary>
    public double SmallChange
    {
        get => (double)GetValue(SmallChangeProperty);
        set => SetValue(SmallChangeProperty, value);
    }

    /// <summary>Provides the OnMouseDown member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _ = ((UIElement)sender).CaptureMouse();
        var circle = (System.Windows.Shapes.Path)sender;
        var mousePos = e.GetPosition(circle);
        UpdateValue(mousePos, circle.ActualWidth, circle.ActualHeight);
    }

    /// <summary>Provides the OnMouseUp member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnMouseUp(object sender, MouseButtonEventArgs e) => ((UIElement)sender).ReleaseMouseCapture();

    /// <summary>Provides the OnMouseMove member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!((UIElement)sender).IsMouseCaptured)
        {
            return;
        }

        var circle = (System.Windows.Shapes.Path)sender;
        var mousePos = e.GetPosition(circle);
        UpdateValue(mousePos, circle.ActualWidth, circle.ActualHeight);
    }

    /// <summary>Provides the UpdateValue member.</summary>
    /// <param name="mousePos">The mousePos value.</param>
    /// <param name="width">The width value.</param>
    /// <param name="height">The height value.</param>
    private void UpdateValue(Point mousePos, double width, double height)
    {
        var x = mousePos.X / (width * 2);
        var y = mousePos.Y / (height * 2);

        var length = Math.Sqrt((x * x) + (y * y));
        if (length == 0)
        {
            return;
        }

        var angle = Math.Acos(x / length);
        if (y < 0)
        {
            angle = -angle;
        }

        angle = (angle * 360 / (Math.PI * 2)) + 180;
        Value = angle.Clamp(0, 360);
    }

    /// <summary>Provides the OnPreviewMouseWheel member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
    {
        Value = MathExtensions.Mod(Value + (SmallChange * args.Delta / 120), 360);
        args.Handled = true;
    }
}
