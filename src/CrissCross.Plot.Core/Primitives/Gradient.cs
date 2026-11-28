// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using PlotColors = CrissCross.Plot.Colors;

namespace CrissCross.Plot;

/// <summary>Configures a color gradient used to fill a plot area.</summary>
/// <param name="gradientType">The initial gradient geometry.</param>
public class Gradient(GradientType gradientType = GradientType.Linear) : IHatch
{
    /// <summary>The divisor used to calculate the radial gradient radius.</summary>
    private const float RadiusDivisor = 2.0F;

    /// <summary>Gets or sets the geometry of the color gradient.</summary>
    public GradientType GradientType { get; set; } = gradientType;

    /// <summary>Gets or sets the starting angle in degrees for a sweep gradient.</summary>
    public float StartAngle { get; set; }

    /// <summary>Gets or sets the ending angle in degrees for a sweep gradient.</summary>
    public float EndAngle { get; set; } = 360F;

    /// <summary>Gets or sets how the shader handles drawing outside its original bounds.</summary>
    public SKShaderTileMode TileMode { get; set; } = SKShaderTileMode.Clamp;

    /// <summary>Gets or sets the starting alignment of a linear gradient.</summary>
    public Alignment AlignmentStart { get; set; } = Alignment.UpperLeft;

    /// <summary>Gets or sets the ending alignment of a linear gradient.</summary>
    public Alignment AlignmentEnd { get; set; } = Alignment.LowerRight;

    /// <summary>Gets or sets the gradient colors, or null to use the hatch colors.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Style",
        "SST2305",
        Justification = "The public array setter is part of the existing API; replacing it with a read-only collection would break callers.")]
    public Color[] Colors { get; set; } = null!;

    /// <summary>Gets or sets normalized positions for gradient colors, or null to distribute them evenly.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Style",
        "SST2305",
        Justification = "The public array setter is part of the existing API; replacing it with a read-only collection would break callers.")]
    public float[] ColorPositions { get; set; } = null!;

    /// <summary>Creates a gradient for the specified axis limits and color stops.</summary>
    /// <param name="rp">The render pack containing the plot rectangle.</param>
    /// <param name="limits">The axis limits that bound the gradient.</param>
    /// <param name="direction">The axis direction of the gradient.</param>
    /// <param name="axes">The axes used to convert pixel positions to coordinates.</param>
    /// <param name="colorPositions">The gradient colors and their axis positions.</param>
    /// <returns>A gradient whose stops are normalized to the visible axis range.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="direction"/> is not a supported axis gradient direction.</exception>
    public static Gradient FromAxisLimits(RenderPack rp, AxisLimits limits, AxisGradientDirection direction, IAxes axes, List<AxisGradientColorPosition> colorPositions)
    {
        double min = direction == AxisGradientDirection.Horizontal
            ? axes.XAxis.GetCoordinate(rp.DataRect.Left, rp.DataRect)
            : limits.Bottom;

        double max = direction == AxisGradientDirection.Horizontal
            ? axes.XAxis.GetCoordinate(rp.DataRect.Right, rp.DataRect)
            : limits.Top;

        List<(AxisGradientColorPosition ColorPosition, int OriginalIndex)> sortedColorPositions = GetSortedColorPositions(colorPositions);
        List<double> stops = GetColorStops(sortedColorPositions, min, max);

        List<Color> colors = [];
        float[] normalizedPositions = new float[stops.Count];
        for (int i = 0; i < stops.Count; i++)
        {
            double position = stops[i];
            colors.Add(InterpolateColor(sortedColorPositions, position));
            normalizedPositions[i] = GetNormalizedPosition(position, min, max);
        }

        (Alignment alignmentStart, Alignment alignmentEnd) = direction switch
        {
            AxisGradientDirection.Horizontal => (Alignment.MiddleLeft, Alignment.MiddleRight),
            AxisGradientDirection.Vertical => (Alignment.LowerCenter, Alignment.UpperCenter),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unsupported axis gradient direction.")
        };

        return new() { AlignmentStart = alignmentStart, AlignmentEnd = alignmentEnd, ColorPositions = normalizedPositions, Colors = colors.ToArray() };
    }

    /// <summary>Creates a shader that renders this gradient in the specified rectangle.</summary>
    /// <param name="backgroundColor">The background color used when no custom colors are specified.</param>
    /// <param name="hatchColor">The hatch color used when no custom colors are specified.</param>
    /// <param name="rect">The rectangle filled by the gradient.</param>
    /// <returns>The shader for the configured gradient.</returns>
    public SKShader GetShader(Color backgroundColor, Color hatchColor, PixelRect rect)
    {
        SKColor[] shaderColors;
        if (Colors is { Length: > 1 })
        {
            shaderColors = new SKColor[Colors.Length];
            for (int i = 0; i < Colors.Length; i++)
            {
                shaderColors[i] = Colors[i].ToSKColor();
            }
        }
        else
        {
            shaderColors = [backgroundColor.ToSKColor(), hatchColor.ToSKColor()];
        }

        return GradientType switch
        {
            GradientType.Radial => SKShader.CreateRadialGradient(
                center: new(rect.HorizontalCenter, rect.VerticalCenter),
                radius: Math.Max(rect.Width, rect.Height) / RadiusDivisor,
                colors: shaderColors,
                colorPos: ColorPositions,
                mode: TileMode),
            GradientType.Sweep => SKShader.CreateSweepGradient(
                center: new(rect.HorizontalCenter, rect.VerticalCenter),
                colors: shaderColors,
                colorPos: ColorPositions,
                tileMode: TileMode,
                startAngle: StartAngle,
                endAngle: EndAngle),
            GradientType.TwoPointConical => SKShader.CreateTwoPointConicalGradient(
                start: rect.TopLeft.ToSKPoint(),
                startRadius: Math.Min(rect.Width, rect.Height),
                end: rect.BottomRight.ToSKPoint(),
                endRadius: Math.Min(rect.Width, rect.Height),
                colors: shaderColors,
                colorPos: ColorPositions,
                mode: TileMode),
            _ => SKShader.CreateLinearGradient(
                start: rect.GetAlignedPixel(AlignmentStart).ToSKPoint(),
                end: rect.GetAlignedPixel(AlignmentEnd).ToSKPoint(),
                colors: shaderColors,
                colorPos: ColorPositions,
                mode: TileMode),
        };
    }

    /// <summary>Sorts color stops by position while preserving the original order of ties.</summary>
    /// <param name="colorPositions">The color stops to sort.</param>
    /// <returns>The sorted color stops and their original indices.</returns>
    private static List<(AxisGradientColorPosition ColorPosition, int OriginalIndex)> GetSortedColorPositions(List<AxisGradientColorPosition> colorPositions)
    {
        List<(AxisGradientColorPosition ColorPosition, int OriginalIndex)> sortedColorPositions = [];
        for (int i = 0; i < colorPositions.Count; i++)
        {
            sortedColorPositions.Add((colorPositions[i], i));
        }

        sortedColorPositions.Sort(static (left, right) =>
        {
            int positionComparison = left.ColorPosition.Position.CompareTo(right.ColorPosition.Position);
            return positionComparison != 0 ? positionComparison : left.OriginalIndex.CompareTo(right.OriginalIndex);
        });
        return sortedColorPositions;
    }

    /// <summary>Creates distinct sorted axis color stops within the visible range and at its boundaries.</summary>
    /// <param name="colorPositions">The color stops sorted by axis position.</param>
    /// <param name="min">The minimum visible axis position.</param>
    /// <param name="max">The maximum visible axis position.</param>
    /// <returns>The sorted distinct axis positions used by the gradient.</returns>
    private static List<double> GetColorStops(List<(AxisGradientColorPosition ColorPosition, int OriginalIndex)> colorPositions, double min, double max)
    {
        List<double> stops = [];
        foreach (var colorPositionEntry in colorPositions)
        {
            double position = colorPositionEntry.ColorPosition.Position;
            if (position >= min && position <= max)
            {
                stops.Add(position);
            }
        }

        stops.Add(min);
        stops.Add(max);
        stops.Sort();
        for (int i = stops.Count - 1; i > 0; i--)
        {
            if (stops[i].CompareTo(stops[i - 1]) == 0)
            {
                stops.RemoveAt(i);
            }
        }

        return stops;
    }

    /// <summary>Interpolates a color at an axis position from the sorted color stops.</summary>
    /// <param name="colorPositions">Color stops sorted by axis position.</param>
    /// <param name="position">The axis position to interpolate.</param>
    /// <returns>The interpolated color, or black when no color stops are available.</returns>
    private static Color InterpolateColor(List<(AxisGradientColorPosition ColorPosition, int OriginalIndex)> colorPositions, double position)
    {
        if (colorPositions.Count == 0)
        {
            return PlotColors.Black;
        }

        AxisGradientColorPosition first = colorPositions[0].ColorPosition;
        AxisGradientColorPosition last = colorPositions[^1].ColorPosition;
        if (position <= first.Position)
        {
            return first.Color;
        }

        if (position >= last.Position)
        {
            return last.Color;
        }

        int upperIndex = 0;
        while (colorPositions[upperIndex].ColorPosition.Position < position)
        {
            upperIndex++;
        }

        AxisGradientColorPosition lower = colorPositions[upperIndex - 1].ColorPosition;
        AxisGradientColorPosition upper = colorPositions[upperIndex].ColorPosition;
        double fraction = (position - lower.Position) / (upper.Position - lower.Position);
        return lower.Color.InterpolateRgb(upper.Color, fraction);
    }

    /// <summary>Converts an axis position to a normalized gradient position.</summary>
    /// <param name="position">The axis position to normalize.</param>
    /// <param name="min">The minimum axis position.</param>
    /// <param name="max">The maximum axis position.</param>
    /// <returns>The position in the range from zero to one, or zero when the axis range is empty.</returns>
    private static float GetNormalizedPosition(double position, double min, double max) =>
        !double.IsNaN(max) && max.CompareTo(min) == 0
            ? 0
            : (float)((position - min) / (max - min));
}
