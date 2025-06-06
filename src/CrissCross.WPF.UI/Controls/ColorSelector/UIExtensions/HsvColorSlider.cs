﻿// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.UIExtensions;

internal class HsvColorSlider : PreviewColorSlider
{
    public static readonly DependencyProperty SliderHsvTypeProperty =
        DependencyProperty.Register(
            nameof(SliderHsvType),
            typeof(string),
            typeof(HsvColorSlider),
            new PropertyMetadata(string.Empty));

    public string SliderHsvType
    {
        get => (string)GetValue(SliderHsvTypeProperty);
        set => SetValue(SliderHsvTypeProperty, value);
    }

    protected override void GenerateBackground()
    {
        if (SliderHsvType == "H")
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
        switch (SliderHsvType)
        {
            case "H":
                {
                    var rgbtuple = ColorSpaceHelper.HsvToRgb(value, CurrentColorState.HSV_S, CurrentColorState.HSV_V);
                    double r = rgbtuple.Item1, g = rgbtuple.Item2, b = rgbtuple.Item3;
                    return Color.FromArgb((byte)(CurrentColorState.A * 255), (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                }

            case "S":
                {
                    var rgbtuple = ColorSpaceHelper.HsvToRgb(CurrentColorState.HSV_H, value / 255.0, CurrentColorState.HSV_V);
                    double r = rgbtuple.Item1, g = rgbtuple.Item2, b = rgbtuple.Item3;
                    return Color.FromArgb((byte)(CurrentColorState.A * 255), (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                }

            case "V":
                {
                    var rgbtuple = ColorSpaceHelper.HsvToRgb(CurrentColorState.HSV_H, CurrentColorState.HSV_S, value / 255.0);
                    double r = rgbtuple.Item1, g = rgbtuple.Item2, b = rgbtuple.Item3;
                    return Color.FromArgb((byte)(CurrentColorState.A * 255), (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                }

            default:
                return Color.FromArgb((byte)(CurrentColorState.A * 255), (byte)(CurrentColorState.RGB_R * 255), (byte)(CurrentColorState.RGB_G * 255), (byte)(CurrentColorState.RGB_B * 255));
        }
    }
}
