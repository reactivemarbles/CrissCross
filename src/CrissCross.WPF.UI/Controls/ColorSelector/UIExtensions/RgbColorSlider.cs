// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.UIExtensions;
#else
namespace CrissCross.WPF.UI.UIExtensions;
#endif

/// <summary>Provides the RgbColorSlider member.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class RgbColorSlider : PreviewColorSlider
{
    /// <summary>Provides the SliderArgbTypeProperty member.</summary>
    public static readonly DependencyProperty SliderArgbTypeProperty = DependencyProperty.Register(
        nameof(SliderArgbType),
        typeof(string),
        typeof(RgbColorSlider),
        new(string.Empty));

    /// <summary>Gets or sets SliderArgbType.</summary>
    public string SliderArgbType
    {
        get => (string)GetValue(SliderArgbTypeProperty);
        set => SetValue(SliderArgbTypeProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    protected override void GenerateBackground()
    {
        var colorStart = GetColorForSelectedArgb(MinimumColorChannelValue);
        var colorEnd = GetColorForSelectedArgb(MaximumColorChannelValue);
        LeftCapColor.Color = colorStart;
        RightCapColor.Color = colorEnd;
        SetBackgroundGradient([new GradientStop(colorStart, 0.0), new GradientStop(colorEnd, 1)]);
    }

    /// <summary>Provides the GetColorForSelectedArgb member.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The result.</returns>
    private Color GetColorForSelectedArgb(int value)
    {
        var a = (byte)(CurrentColorState.A * ColorChannelScale);
        var r = (byte)(CurrentColorState.RGB_R * ColorChannelScale);
        var g = (byte)(CurrentColorState.RGB_G * ColorChannelScale);
        var b = (byte)(CurrentColorState.RGB_B * ColorChannelScale);
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
