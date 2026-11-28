// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a straight polar axis line extending outward from the origin.</summary>
/// <param name="angle">The direction of the spoke.</param>
/// <param name="length">The length of the spoke.</param>
public class PolarAxisSpoke(Angle angle, double length) : IHasLine
{
    /// <summary>Gets or sets the direction of the spoke.</summary>
    public Angle Angle { get; set; } = angle;

    /// <summary>Gets or sets the length of the spoke.</summary>
    public double Length { get; set; } = length;

    /// <summary>Gets the label style for the spoke.</summary>
    public LabelStyle LabelStyle { get; } = new();

    /// <summary>Gets or sets the spoke's label text.</summary>
    public string? LabelText { get; set; }

    /// <summary>Gets or sets the fractional padding beyond the spoke length used for its label.</summary>
    public double LabelPaddingFraction { get; set; } = Defaults.LabelPaddingFraction;

    /// <summary>Gets the spoke length including label padding.</summary>
    public double LabelLength => Length * (1 + LabelPaddingFraction);

    /// <summary>Gets or sets the line style for the spoke.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1, Color = Colors.Black.WithAlpha(Defaults.LineAlpha), };

    /// <summary>Gets or sets the width of the spoke line.</summary>
    public float LineWidth
    {
        get => LineStyle.Width;
        set => LineStyle.Width = value;
    }

    /// <summary>Gets or sets the pattern of the spoke line.</summary>
    public LinePattern LinePattern
    {
        get => LineStyle.Pattern;
        set => LineStyle.Pattern = value;
    }

    /// <summary>Gets or sets the color of the spoke line.</summary>
    public Color LineColor
    {
        get => LineStyle.Color;
        set => LineStyle.Color = value;
    }

    /// <summary>Stores named defaults used when constructing polar spoke styles.</summary>
    private static class Defaults
    {
        /// <summary>The default fractional padding for spoke labels.</summary>
        public const double LabelPaddingFraction = 0.2;

        /// <summary>The default alpha value for the spoke line.</summary>
        public const float LineAlpha = 0.5F;
    }
}
