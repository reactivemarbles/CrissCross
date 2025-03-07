﻿// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.UIExtensions;

internal class HslColorSlider : PreviewColorSlider
{
    public static readonly DependencyProperty SliderHslTypeProperty =
        DependencyProperty.Register(
            nameof(SliderHslType),
            typeof(string),
            typeof(HslColorSlider),
            new PropertyMetadata(string.Empty));

    public string SliderHslType
    {
        get => (string)GetValue(SliderHslTypeProperty);
        set => SetValue(SliderHslTypeProperty, value);
    }

    protected override void GenerateBackground()
    {
        if (SliderHslType == "H")
        {
            var colorStart = GetColorForSelectedArgb(0);
            var colorEnd = GetColorForSelectedArgb(360);
            LeftCapColor.Color = colorStart;
            RightCapColor.Color = colorEnd;
            BackgroundGradient =
            [
                new GradientStop(colorStart, 0),
                new GradientStop(GetColorForSelectedArgb(60), 1 / 6.0),
                new GradientStop(GetColorForSelectedArgb(120), 2 / 6.0),
                new GradientStop(GetColorForSelectedArgb(180), 0.5),
                new GradientStop(GetColorForSelectedArgb(240), 4 / 6.0),
                new GradientStop(GetColorForSelectedArgb(300), 5 / 6.0),
                new GradientStop(colorEnd, 1)
            ];
            return;
        }

        if (SliderHslType == "L")
        {
            var colorStart = GetColorForSelectedArgb(0);
            var colorEnd = GetColorForSelectedArgb(255);
            LeftCapColor.Color = colorStart;
            RightCapColor.Color = colorEnd;
            BackgroundGradient =
            [
                new GradientStop(colorStart, 0),
                new GradientStop(GetColorForSelectedArgb(128), 0.5),
                new GradientStop(colorEnd, 1)
            ];
            return;
        }

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
    }

    private Color GetColorForSelectedArgb(int value)
    {
        switch (SliderHslType)
        {
            case "H":
                {
                    var rgbtuple = ColorSpaceHelper.HslToRgb(value, CurrentColorState.HSL_S, CurrentColorState.HSL_L);
                    double r = rgbtuple.Item1, g = rgbtuple.Item2, b = rgbtuple.Item3;
                    return Color.FromArgb((byte)(CurrentColorState.A * 255), (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                }

            case "S":
                {
                    var rgbtuple = ColorSpaceHelper.HslToRgb(CurrentColorState.HSL_H, value / 255.0, CurrentColorState.HSL_L);
                    double r = rgbtuple.Item1, g = rgbtuple.Item2, b = rgbtuple.Item3;
                    return Color.FromArgb((byte)(CurrentColorState.A * 255), (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                }

            case "L":
                {
                    var rgbtuple = ColorSpaceHelper.HslToRgb(CurrentColorState.HSL_H, CurrentColorState.HSL_S, value / 255.0);
                    double r = rgbtuple.Item1, g = rgbtuple.Item2, b = rgbtuple.Item3;
                    return Color.FromArgb((byte)(CurrentColorState.A * 255), (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                }

            default:
                return Color.FromArgb((byte)(CurrentColorState.A * 255), (byte)(CurrentColorState.RGB_R * 255), (byte)(CurrentColorState.RGB_G * 255), (byte)(CurrentColorState.RGB_B * 255));
        }
    }
}
