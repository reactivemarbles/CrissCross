// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;

namespace CrissCross.WPF.UI;

internal partial class HueSlider : UserControl
{
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(HueSlider),
            new PropertyMetadata(0.0));

    public static readonly DependencyProperty SmallChangeProperty =
        DependencyProperty.Register(
            nameof(SmallChange),
            typeof(double),
            typeof(HueSlider),
            new PropertyMetadata(1.0));

    public HueSlider() => InitializeComponent();

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public double SmallChange
    {
        get => (double)GetValue(SmallChangeProperty);
        set => SetValue(SmallChangeProperty, value);
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        ((UIElement)sender).CaptureMouse();
        var circle = (System.Windows.Shapes.Path)sender;
        var mousePos = e.GetPosition(circle);
        UpdateValue(mousePos, circle.ActualWidth, circle.ActualHeight);
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e) => ((UIElement)sender).ReleaseMouseCapture();

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

    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
    {
        Value = MathHelper.Mod(Value + (SmallChange * args.Delta / 120), 360);
        args.Handled = true;
    }
}
