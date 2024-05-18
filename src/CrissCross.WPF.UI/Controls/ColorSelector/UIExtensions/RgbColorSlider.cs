// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.UIExtensions;

internal class RgbColorSlider : PreviewColorSlider
{
    public static readonly DependencyProperty SliderArgbTypeProperty =
        DependencyProperty.Register(
            nameof(SliderArgbType),
            typeof(string),
            typeof(RgbColorSlider),
            new PropertyMetadata(string.Empty));

    public string SliderArgbType
    {
        get => (string)GetValue(SliderArgbTypeProperty);
        set => SetValue(SliderArgbTypeProperty, value);
    }

    protected override void GenerateBackground()
    {
        var colorStart = GetColorForSelectedArgb(0);
        var colorEnd = GetColorForSelectedArgb(255);
        LeftCapColor.Color = colorStart;
        RightCapColor.Color = colorEnd;
        BackgroundGradient =
        [
            new GradientStop(colorStart, 0.0),
            new GradientStop(colorEnd, 1)
        ];
    }

    private Color GetColorForSelectedArgb(int value)
    {
        var a = (byte)(CurrentColorState.A * 255);
        var r = (byte)(CurrentColorState.RGB_R * 255);
        var g = (byte)(CurrentColorState.RGB_G * 255);
        var b = (byte)(CurrentColorState.RGB_B * 255);
        return SliderArgbType switch
        {
            "A" => Color.FromArgb((byte)value, r, g, b),
            "R" => Color.FromArgb(a, (byte)value, g, b),
            "G" => Color.FromArgb(a, r, (byte)value, b),
            "B" => Color.FromArgb(a, r, g, (byte)value),
            _ => Color.FromArgb(a, r, g, b),
        };
    }
}
