// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>A Phasor plot marks a collection of points in polar space using an arrow with its base centered at the origin.</summary>
public class Phasor : IPlottable, IHasArrow, IHasLegendText
{
    /// <summary>Gets a collection of points in polar space.</summary>
    public List<PolarCoordinates> Points { get; } = [];

    /// <summary>Gets if populated, <see cref="Points"/> will be labeled with these strings.</summary>
    public List<string> Labels { get; } = [];

    /// <summary>Gets style for arrow labels defined in <see cref="Labels"/></summary>
    public LabelStyle LabelStyle { get; } = new();

    /// <summary>Gets or sets additional padding given to accommodate spoke labels.</summary>
    public double PaddingFraction { get; set; } = 0.01;

    /// <summary>Gets or sets additional padding given to accommodate labels.</summary>
    public double PaddingArc { get; set; } = 5;

    /// <summary>Gets or sets whether this plot is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this plot.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend item represented by this plot.</summary>
    public IEnumerable<LegendItem> LegendItems => [new LegendItem() { LineStyle = ArrowStyle.LineStyle, LabelText = LegendText }];

    /// <summary>Gets or sets the text shown for this plot in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets the style of the arrows.</summary>
    public ArrowStyle ArrowStyle { get; set; } = new() { LineWidth = Constants.DefaultArrowLineWidth, ArrowWidth = Constants.DefaultArrowWidth };

    /// <summary>Gets or sets the arrow line color.</summary>
    public Color ArrowLineColor { get => ArrowStyle.LineStyle.Color; set => ArrowStyle.LineStyle.Color = value; }

    /// <summary>Gets or sets the arrow line width.</summary>
    public float ArrowLineWidth { get => ArrowStyle.LineStyle.Width; set => ArrowStyle.LineStyle.Width = value; }

    /// <summary>Gets or sets the arrow fill color.</summary>
    public Color ArrowFillColor { get => ArrowStyle.FillStyle.Color; set => ArrowStyle.FillStyle.Color = value; }

    /// <summary>Gets or sets the minimum arrow length.</summary>
    public float ArrowMinimumLength { get => ArrowStyle.MinimumLength; set => ArrowStyle.MinimumLength = value; }

    /// <summary>Gets or sets the maximum arrow length.</summary>
    public float ArrowMaximumLength { get => ArrowStyle.MaximumLength; set => ArrowStyle.MaximumLength = value; }

    /// <summary>Gets or sets the arrow offset.</summary>
    public float ArrowOffset { get => ArrowStyle.Offset; set => ArrowStyle.Offset = value; }

    /// <summary>Gets or sets the arrow anchor.</summary>
    public ArrowAnchor ArrowAnchor { get => ArrowStyle.Anchor; set => ArrowStyle.Anchor = value; }

    /// <summary>Gets or sets the arrow width.</summary>
    public float ArrowWidth { get => ArrowStyle.ArrowWidth; set => ArrowStyle.ArrowWidth = value; }

    /// <summary>Gets or sets the arrowhead axis length.</summary>
    public float ArrowheadAxisLength { get => ArrowStyle.ArrowheadAxisLength; set => ArrowStyle.ArrowheadAxisLength = value; }

    /// <summary>Gets or sets the arrowhead length.</summary>
    public float ArrowheadLength { get => ArrowStyle.ArrowheadLength; set => ArrowStyle.ArrowheadLength = value; }

    /// <summary>Gets or sets the arrowhead width.</summary>
    public float ArrowheadWidth { get => ArrowStyle.ArrowheadWidth; set => ArrowStyle.ArrowheadWidth = value; }

    /// <summary>Gets or sets the shape used to render each arrow.</summary>
    public IArrowShape ArrowShape { get; set; } = new ArrowShapes.Single();

    /// <summary>Gets the axis limits occupied by the phasor points.</summary>
    /// <returns>The axis limits of the points.</returns>
    public AxisLimits GetAxisLimits()
    {
        if (Points.Count == 0)
        {
            return AxisLimits.NoLimits;
        }

        List<Coordinates> points = [with(capacity: Points.Count + 1)];
        foreach (PolarCoordinates point in Points)
        {
            points.Add(point.ToCartesian());
        }

        points.Add(Coordinates.Origin);
        return new(points);
    }

    /// <summary>Renders the phasor arrows and their labels.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (Points.Count == 0)
        {
            return;
        }

        Pixel basePixel = Axes.GetPixel(Coordinates.Origin);
        for (int i = 0; i < Points.Count; i++)
        {
            PolarCoordinates point = Points[i];
            Pixel tipPixel = Axes.GetPixel(point.ToCartesian());
            PixelLine pixelLine = new(basePixel, tipPixel);
            if (ArrowOffset != 0)
            {
                pixelLine = pixelLine.BackedUpBy(ArrowOffset);
            }

            ArrowShape.Render(rp, pixelLine, ArrowStyle);
            if (i >= Labels.Count)
            {
                continue;
            }

            LabelStyle.Text = Labels[i];
            Angle angle = point.Radius > 0
                ? Angle.FromRadians(point.Angle.Radians + (PaddingArc / point.Radius))
                : Angle.FromRadians(point.Angle.Radians);
            double padding = Math.Min(Axes.XAxis.Range.Span, Axes.YAxis.Range.Span) * PaddingFraction;
            PolarCoordinates labelPoint = new(point.Radius + padding, angle);
            Pixel labelPixel = Axes.GetPixel(labelPoint.ToCartesian());
            PixelRect labelRect = LabelStyle.Measure(rp.Paint).Rect(Alignment.MiddleCenter);
            Pixel labelOffset = labelRect.Center - labelRect.TopLeft;
            labelPixel -= labelOffset;
            LabelStyle.Render(rp.Canvas, labelPixel, rp.Paint);
        }
    }

    /// <summary>Contains default values used by phasor plots.</summary>
    private static class Constants
    {
        /// <summary>The default width of a phasor arrow line.</summary>
        public const float DefaultArrowLineWidth = 2;

        /// <summary>The default width of a phasor arrowhead.</summary>
        public const float DefaultArrowWidth = 3;
    }
}
