// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.UIExtensions;
#else
namespace CrissCross.WPF.UI.UIExtensions;
#endif

/// <summary>Provides the HsvColorSlider member.</summary>
public sealed class HsvColorSlider : PreviewColorSlider
{
    /// <summary>Provides the SliderHsvTypeProperty member.</summary>
    public static readonly DependencyProperty SliderHsvTypeProperty = DependencyProperty.Register(
        nameof(SliderHsvType),
        typeof(string),
        typeof(HsvColorSlider),
        new PropertyMetadata(string.Empty));

    /// <summary>Gets or sets SliderHsvType.</summary>
    public string SliderHsvType
    {
        get => (string)GetValue(SliderHsvTypeProperty);
        set => SetValue(SliderHsvTypeProperty, value);
    }

    protected override void GenerateBackground()
    {
        if (SliderHsvType == "H")
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
        switch (SliderHsvType)
        {
            case "H":
            {
                var rgbtuple = ColorSpaceHelper.HsvToRgb(value, CurrentColorState.HSV_S, CurrentColorState.HSV_V);
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
                var rgbtuple = ColorSpaceHelper.HsvToRgb(
                    CurrentColorState.HSV_H,
                    value / ColorChannelScale,
                    CurrentColorState.HSV_V);
                double r = rgbtuple.Item1;
                double g = rgbtuple.Item2;
                double b = rgbtuple.Item3;
                return Color.FromArgb(
                    (byte)(CurrentColorState.A * ColorChannelScale),
                    (byte)(r * ColorChannelScale),
                    (byte)(g * ColorChannelScale),
                    (byte)(b * ColorChannelScale));
            }

            case "V":
            {
                var rgbtuple = ColorSpaceHelper.HsvToRgb(
                    CurrentColorState.HSV_H,
                    CurrentColorState.HSV_S,
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
