// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.UIExtensions;

/// <summary>Provides the HslColorSlider member.</summary>
internal sealed class HslColorSlider : PreviewColorSlider
{
    /// <summary>Provides the SliderHslTypeProperty member.</summary>
    public static readonly DependencyProperty SliderHslTypeProperty = DependencyProperty.Register(
        nameof(SliderHslType),
        typeof(string),
        typeof(HslColorSlider),
        new PropertyMetadata(string.Empty));

    /// <summary>Gets or sets SliderHslType.</summary>
    public string SliderHslType
    {
        get => (string)GetValue(SliderHslTypeProperty);
        set => SetValue(SliderHslTypeProperty, value);
    }

    protected override void GenerateBackground()
    {
        if (SliderHslType == "H")
        {
            var colorStart = GetColorForSelectedArgb(MinimumHueDegrees);
            var colorEnd = GetColorForSelectedArgb(FullHueDegrees);
            LeftCapColor.Color = colorStart;
            RightCapColor.Color = colorEnd;
            SetBackgroundGradient([
                new GradientStop(colorStart, 0),
                new GradientStop(GetColorForSelectedArgb(YellowHueDegrees), YellowGradientOffset),
                new GradientStop(GetColorForSelectedArgb(GreenHueDegrees), GreenGradientOffset),
                new GradientStop(GetColorForSelectedArgb(CyanHueDegrees), CyanGradientOffset),
                new GradientStop(GetColorForSelectedArgb(BlueHueDegrees), BlueGradientOffset),
                new GradientStop(GetColorForSelectedArgb(MagentaHueDegrees), MagentaGradientOffset),
                new GradientStop(colorEnd, 1),]);
            return;
        }

        if (SliderHslType == "L")
        {
            var colorStart = GetColorForSelectedArgb(MinimumColorChannelValue);
            var colorEnd = GetColorForSelectedArgb(MaximumColorChannelValue);
            LeftCapColor.Color = colorStart;
            RightCapColor.Color = colorEnd;
            SetBackgroundGradient([
                new GradientStop(colorStart, 0),
                new GradientStop(GetColorForSelectedArgb(MidpointColorChannelValue), CyanGradientOffset),
                new GradientStop(colorEnd, 1),]);
            return;
        }

        var fallbackColorStart = GetColorForSelectedArgb(MinimumColorChannelValue);
        var fallbackColorEnd = GetColorForSelectedArgb(MaximumColorChannelValue);
        LeftCapColor.Color = fallbackColorStart;
        RightCapColor.Color = fallbackColorEnd;
        SetBackgroundGradient([new GradientStop(fallbackColorStart, 0.0), new GradientStop(fallbackColorEnd, 1)]);
    }

    /// <summary>Provides the GetColorForSelectedArgb member.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    private Color GetColorForSelectedArgb(int value)
    {
        switch (SliderHslType)
        {
            case "H":
            {
                var rgbtuple = ColorSpaceHelper.HslToRgb(value, CurrentColorState.HSL_S, CurrentColorState.HSL_L);
                double r = rgbtuple.Item1;
                double g = rgbtuple.Item2;
                double b = rgbtuple.Item3;
                return Color.FromArgb(
                    (byte)(CurrentColorState.A * ColorChannelScale),
                    (byte)(r * ColorChannelScale),
                    (byte)(g * ColorChannelScale),
                    (byte)(b * ColorChannelScale));
            }

            case "S":
            {
                var rgbtuple = ColorSpaceHelper.HslToRgb(
                    CurrentColorState.HSL_H,
                    value / ColorChannelScale,
                    CurrentColorState.HSL_L);
                double r = rgbtuple.Item1;
                double g = rgbtuple.Item2;
                double b = rgbtuple.Item3;
                return Color.FromArgb(
                    (byte)(CurrentColorState.A * ColorChannelScale),
                    (byte)(r * ColorChannelScale),
                    (byte)(g * ColorChannelScale),
                    (byte)(b * ColorChannelScale));
            }

            case "L":
            {
                var rgbtuple = ColorSpaceHelper.HslToRgb(
                    CurrentColorState.HSL_H,
                    CurrentColorState.HSL_S,
                    value / ColorChannelScale);
                double r = rgbtuple.Item1;
                double g = rgbtuple.Item2;
                double b = rgbtuple.Item3;
                return Color.FromArgb(
                    (byte)(CurrentColorState.A * ColorChannelScale),
                    (byte)(r * ColorChannelScale),
                    (byte)(g * ColorChannelScale),
                    (byte)(b * ColorChannelScale));
            }

            default:
                return Color.FromArgb(
                    (byte)(CurrentColorState.A * ColorChannelScale),
                    (byte)(CurrentColorState.RGB_R * ColorChannelScale),
                    (byte)(CurrentColorState.RGB_G * ColorChannelScale),
                    (byte)(CurrentColorState.RGB_B * ColorChannelScale));
        }
    }
}
