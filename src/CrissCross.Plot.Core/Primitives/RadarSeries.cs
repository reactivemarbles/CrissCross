// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Defines values and styling information for displaying a single shape on a radar chart.</summary>
public class RadarSeries : LabelStyleProperties, IHasLabel, IHasLegendText, IHasFill, IHasLine
{
    /// <summary>Gets or sets the values that determine the radar shape.</summary>
    public IReadOnlyList<double> Values { get; set; } = [];

    /// <summary>Gets or sets the label style for the series.</summary>
    public sealed override LabelStyle LabelStyle { get; set; } = new() { Alignment = Alignment.MiddleCenter };

    /// <summary>Gets or sets the legend text through the label style.</summary>
    public string LegendText { get => LabelStyle.Text; set => LabelStyle.Text = value; }

    /// <summary>Gets or sets the fill style for the radar shape.</summary>
    public FillStyle FillStyle { get; set; } = new() { IsVisible = true, Color = Colors.Black.WithAlpha(Defaults.FillAlpha), };

    /// <summary>Gets or sets the fill color.</summary>
    public Color FillColor { get => FillStyle.Color; set => FillStyle.Color = value; }

    /// <summary>Gets or sets the hatch color.</summary>
    public Color FillHatchColor { get => FillStyle.HatchColor; set => FillStyle.HatchColor = value; }

    /// <summary>Gets or sets the fill hatch.</summary>
    public IHatch? FillHatch { get => FillStyle.Hatch; set => FillStyle.Hatch = value; }

    /// <summary>Gets or sets the line style for the radar shape.</summary>
    public LineStyle LineStyle { get; set; } = new() { IsVisible = true, Width = 1, Pattern = LinePattern.Solid, Color = Colors.Black, };

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Stores named defaults used when constructing radar series styles.</summary>
    private static class Defaults
    {
        /// <summary>The default fill alpha value.</summary>
        public const float FillAlpha = 0.2F;
    }
}
