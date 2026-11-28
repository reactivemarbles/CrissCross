// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>A polar axis tick that describes a circle centered at the origin and includes rendering styles.</summary>
/// <param name="radius">The radius of the circle.</param>
public class PolarAxisCircle(double radius) : IHasLine
{
    /// <summary>Gets or sets the circle radius.</summary>
    public double Radius { get; set; } = radius;

    /// <summary>Gets or sets the center of the circle.</summary>
    public Coordinates Origin { get; set; } = new();

    /// <summary>Gets or sets the starting angle of the circle arc.</summary>
    public Angle StartAngle { get; set; } = Angle.FromDegrees(0);

    /// <summary>Gets or sets the angular sweep of the circle arc.</summary>
    public Angle SweepAngle { get; set; } = Angle.FromDegrees(Defaults.FullSweepDegrees);

    /// <summary>Gets or sets the label style.</summary>
    public LabelStyle LabelStyle { get; set; } = new() { Alignment = Alignment.LowerLeft, OffsetX = Defaults.LabelOffsetX, };

    /// <summary>Gets or sets the label text.</summary>
    public string LabelText { get; set; } = string.Empty;

    /// <summary>Gets or sets the label angle.</summary>
    public Angle LabelAngle { get; set; } = Angle.FromDegrees(0);

    /// <summary>Gets or sets the circle line style.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1, Color = Colors.Black.WithAlpha(Defaults.LineAlpha), };

    /// <summary>Gets or sets the width of the circle line.</summary>
    public float LineWidth
    {
        get => LineStyle.Width;
        set => LineStyle.Width = value;
    }

    /// <summary>Gets or sets the pattern of the circle line.</summary>
    public LinePattern LinePattern
    {
        get => LineStyle.Pattern;
        set => LineStyle.Pattern = value;
    }

    /// <summary>Gets or sets the color of the circle line.</summary>
    public Color LineColor
    {
        get => LineStyle.Color;
        set => LineStyle.Color = value;
    }

    /// <summary>Stores named defaults used when constructing polar circle styles.</summary>
    private static class Defaults
    {
        /// <summary>The default full-circle angular sweep in degrees.</summary>
        public const int FullSweepDegrees = 360;

        /// <summary>The default horizontal offset for the circle label.</summary>
        public const float LabelOffsetX = 3;

        /// <summary>The default alpha value for the circle line.</summary>
        public const float LineAlpha = 0.5F;
    }
}
