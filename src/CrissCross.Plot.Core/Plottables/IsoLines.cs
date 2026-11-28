// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;

namespace CrissCross.Plot.Plottables;

/// <summary>Renders diagonal isolines and labels their intersections with the axes.</summary>
public class IsoLines : IPlottable, IHasLine
{
    /// <summary>Gets the mutable list of manually specified isoline positions and labels.</summary>
    [SuppressMessage("Design", "SST1401", Justification = "Changing this public field to a property would break the existing API.")]
    [SuppressMessage("Design", "SST1414", Justification = "Naming the tuple elements changes the existing public tuple type.")]
    [SuppressMessage("Design", "SST2322", Justification = "The public mutable list is the existing API for configuring manual isolines.")]
    public readonly List<(double, string)> ManualPositions = [];

    /// <summary>Gets or sets the style used to render tick labels.</summary>
    [SuppressMessage("Design", "SST1401", Justification = "Changing this public field to a property would break the existing API.")]
    public LabelStyle TickLabelStyle = new();

    /// <summary>Gets or sets the formatter used to convert automatic tick positions to labels.</summary>
    [SuppressMessage("Design", "SST1401", Justification = "Changing this public field to a property would break the existing API.")]
    public Func<double, string> TickLabelFormatter = DefaultTickFormatter;

    /// <summary>The default alpha used for isoline rendering.</summary>
    private const float DefaultLineAlpha = 0.2F;

    /// <summary>The padding for fixed-position tick labels.</summary>
    private const float FixedLabelPadding = 5;

    /// <summary>The offset for rotated exterior tick labels.</summary>
    private const float RotatedExteriorLabelOffset = 15;

    /// <summary>The offset for rotated interior tick labels.</summary>
    private const float RotatedInteriorLabelOffset = 10;

    /// <summary>The left margin for isoline labels.</summary>
    private const float LeftLabelMargin = 50;

    /// <summary>The bottom margin for isoline labels.</summary>
    private const float BottomLabelMargin = 40;

    /// <summary>The offset used to generate automatic isolines.</summary>
    private const int AutomaticIsoLineOffset = 5;

    /// <summary>The number of automatically generated isolines.</summary>
    private const int AutomaticIsoLineCount = 50;

    /// <summary>The decimal places used for automatic tick labels.</summary>
    private const int DefaultTickLabelDecimalPlaces = 3;

    /// <summary>The slope used to calculate diagonal grid lines.</summary>
    private const double GridLineSlope = 1.0;

    /// <summary>Gets or sets whether this plottable is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position isolines.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this plottable. Isolines do not produce legend items.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets whether tick labels are rotated to follow the isolines.</summary>
    public bool RotateLabels { get; set; } = true;

    /// <summary>Gets or sets whether tick labels are placed outside the plot area.</summary>
    public bool ExteriorTickLabels { get; set; }

    /// <summary>Gets or sets the line style used to render isolines.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1, Color = Colors.Black.WithAlpha(DefaultLineAlpha), Pattern = LinePattern.DenselyDashed, };

    /// <summary>Gets or sets the isoline width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the isoline pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the isoline color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Formats an automatic tick label by rounding the intercept to three decimal places.</summary>
    /// <param name="interceptY">The Y-intercept to format.</param>
    /// <returns>The rounded intercept using the current culture.</returns>
    public static string DefaultTickFormatter(double interceptY) => Math.Round(interceptY, DefaultTickLabelDecimalPlaces).ToString();

    /// <summary>Gets the axis limits required by this plottable. Isolines do not impose limits.</summary>
    /// <returns>No axis limits.</returns>
    public AxisLimits GetAxisLimits() => AxisLimits.NoLimits;

    /// <summary>Renders the isolines and their labels.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        List<PixelLine> lines = ManualPositions.Count > 0
            ? GetLinesManual(rp)
            : GetLinesAutomatic(rp);

        RenderLines(rp, rp.Paint, lines);

        rp.CanvasState.DisableClipping();
        if (RotateLabels)
        {
            RenderLabelsRotated(rp, rp.Paint, lines);
        }
        else
        {
            RenderLabelsFixed(rp, rp.Paint, lines);
        }
    }

    /// <summary>Renders each isoline using the configured line style.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="lines">The isolines to render.</param>
    private void RenderLines(RenderPack rp, Paint paint, List<PixelLine> lines)
    {
        LineStyle.ApplyToPaint(paint);
        foreach (PixelLine line in lines)
        {
            LineStyle.Render(rp.Canvas, line, paint);
        }
    }

    /// <summary>Renders labels without rotating them to follow the isolines.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="lines">The isolines to label.</param>
    private void RenderLabelsFixed(RenderPack rp, Paint paint, List<PixelLine> lines)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            PixelLine line = lines[i];
            bool topEdge = line.Pixel2.X < rp.DataRect.Right;
            Pixel position = GetFixedLabelPosition(rp, line, topEdge);
            SetFixedLabelAlignment(topEdge);
            RenderLabel(rp, paint, line, position, i, rotate: false);
        }
    }

    /// <summary>Calculates the position of a fixed-orientation label.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="line">The isoline to label.</param>
    /// <param name="topEdge">Whether the isoline intersects the top edge.</param>
    /// <returns>The label position.</returns>
    private Pixel GetFixedLabelPosition(RenderPack rp, PixelLine line, bool topEdge)
    {
        if (ExteriorTickLabels)
        {
            return topEdge
                ? new(line.Pixel2.X, line.Pixel2.Y - FixedLabelPadding)
                : new(rp.DataRect.Right + FixedLabelPadding, line.Y(rp.DataRect.Right));
        }

        return topEdge
            ? new(line.Pixel2.X, line.Pixel2.Y + FixedLabelPadding)
            : new(rp.DataRect.Right - FixedLabelPadding, line.Y(rp.DataRect.Right));
    }

    /// <summary>Sets label alignment based on its edge and exterior-label setting.</summary>
    /// <param name="topEdge">Whether the isoline intersects the top edge.</param>
    private void SetFixedLabelAlignment(bool topEdge)
    {
        if (ExteriorTickLabels)
        {
            TickLabelStyle.Alignment = topEdge ? Alignment.LowerCenter : Alignment.MiddleLeft;
            return;
        }

        TickLabelStyle.Alignment = topEdge ? Alignment.UpperCenter : Alignment.MiddleRight;
    }

    /// <summary>Renders labels rotated to follow their isolines.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="lines">The isolines to label.</param>
    private void RenderLabelsRotated(RenderPack rp, Paint paint, List<PixelLine> lines)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            PixelLine line = lines[i];
            bool topEdge = line.Pixel2.X < rp.DataRect.Right;
            if (!TryGetRotatedLabelPosition(rp, line, topEdge, out Pixel position))
            {
                continue;
            }

            RenderLabel(rp, paint, line, position, i, rotate: true);
        }
    }

    /// <summary>Calculates a rotated label position and determines whether it is visible.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="line">The isoline to label.</param>
    /// <param name="topEdge">Whether the isoline intersects the top edge.</param>
    /// <param name="position">The calculated label position.</param>
    /// <returns><see langword="true"/> when the label should be rendered.</returns>
    private bool TryGetRotatedLabelPosition(RenderPack rp, PixelLine line, bool topEdge, out Pixel position)
    {
        if (ExteriorTickLabels)
        {
            position = topEdge
                ? new(line.Pixel2.X + RotatedExteriorLabelOffset, line.Pixel2.Y - RotatedExteriorLabelOffset)
                : new(rp.DataRect.Right + RotatedInteriorLabelOffset, line.Y(rp.DataRect.Right) - RotatedInteriorLabelOffset);
            TickLabelStyle.Alignment = Alignment.MiddleLeft;
        }
        else
        {
            position = topEdge
                ? new(line.Pixel2.X, line.Pixel2.Y + FixedLabelPadding)
                : new(rp.DataRect.Right - RotatedInteriorLabelOffset, line.Y(rp.DataRect.Right) + RotatedInteriorLabelOffset);
            TickLabelStyle.Alignment = Alignment.UpperRight;

            if ((topEdge && position.X < rp.DataRect.Left + LeftLabelMargin)
                || (!topEdge && position.Y > rp.DataRect.Bottom - BottomLabelMargin))
            {
                return false;
            }
        }

        return position.Y <= rp.DataRect.Bottom && position.X >= rp.DataRect.Left;
    }

    /// <summary>Sets up and renders a label for one isoline.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="line">The isoline to label.</param>
    /// <param name="position">The label position.</param>
    /// <param name="index">The isoline index.</param>
    /// <param name="rotate">Whether the label should follow the isoline angle.</param>
    private void RenderLabel(RenderPack rp, Paint paint, PixelLine line, Pixel position, int index, bool rotate)
    {
        // TODO: pass a CoordinateLine instead of a PixelLine
        Coordinates firstCoordinate = Axes.GetCoordinates(line.Pixel1);
        Coordinates secondCoordinate = Axes.GetCoordinates(line.Pixel2);
        CoordinateLine coordinateLine = new(firstCoordinate, secondCoordinate);

        TickLabelStyle.Text = ManualPositions.Count == 0
            ? TickLabelFormatter(coordinateLine.YIntercept)
            : ManualPositions[index].Item2;
        if (rotate)
        {
            TickLabelStyle.Rotation = line.SlopeDegrees;
        }

        TickLabelStyle.Render(rp.Canvas, position, paint);
    }

    /// <summary>Selects the automatic isoline direction based on the plot aspect ratio.</summary>
    /// <param name="rp">The render context.</param>
    /// <returns>The automatic pixel-space isolines.</returns>
    private List<PixelLine> GetLinesAutomatic(RenderPack rp)
    {
        AxisLimits limits = rp.Plot.Axes.GetLimits();
        bool isWider = limits.HorizontalSpan > limits.VerticalSpan;
        return isWider ? GetLinesByX(rp) : GetLinesByY(rp);
    }

    /// <summary>Creates automatic isolines based on the horizontal-axis tick spacing.</summary>
    /// <param name="rp">The render context.</param>
    /// <returns>The pixel-space isolines to render.</returns>
    private List<PixelLine> GetLinesByX(RenderPack rp)
    {
        List<PixelLine> lines = [];
        List<double> tickPositions = [];

        foreach (var tick in Axes.XAxis.TickGenerator.Ticks)
        {
            if (tick.IsMajor)
            {
                tickPositions.Add(tick.Position);
            }
        }

        tickPositions.Reverse();
        if (tickPositions.Count < 2)
        {
            return lines;
        }

        double lastTickPosition = tickPositions[0];
        double tickSpacing = tickPositions[0] - tickPositions[1];
        lastTickPosition += tickSpacing * AutomaticIsoLineOffset;

        AxisLimits limits = rp.Plot.Axes.GetLimits();
        double[] isoLineXs = new double[AutomaticIsoLineCount];
        for (int i = 0; i < isoLineXs.Length; i++)
        {
            isoLineXs[i] = lastTickPosition - (i * tickSpacing);
        }

        foreach (double x in isoLineXs)
        {
            Coordinates firstPoint = new(x, 0);
            Coordinates secondPoint = new(x + 1, GridLineSlope);
            CoordinateLine line = new CoordinateLine(firstPoint, secondPoint).ExtendTo(limits.Rect);
            if (line.End.X < limits.Left)
            {
                return lines;
            }

            lines.Add(Axes.GetPixelLine(line));
        }

        return lines;
    }

    /// <summary>Creates automatic isolines based on the vertical-axis tick spacing.</summary>
    /// <param name="rp">The render context.</param>
    /// <returns>The pixel-space isolines to render.</returns>
    private List<PixelLine> GetLinesByY(RenderPack rp)
    {
        List<PixelLine> lines = [];
        List<double> tickPositions = [];

        foreach (var tick in Axes.YAxis.TickGenerator.Ticks)
        {
            if (tick.IsMajor)
            {
                tickPositions.Add(tick.Position);
            }
        }

        tickPositions.Reverse();
        if (tickPositions.Count < 2)
        {
            return lines;
        }

        double lastTickPosition = tickPositions[0];
        double tickSpacing = tickPositions[0] - tickPositions[1];
        lastTickPosition += lastTickPosition * AutomaticIsoLineOffset;

        AxisLimits limits = rp.Plot.Axes.GetLimits();
        double[] isoLineYs = new double[AutomaticIsoLineCount];
        for (int i = 0; i < isoLineYs.Length; i++)
        {
            isoLineYs[i] = lastTickPosition - (i * tickSpacing);
        }

        foreach (double y in isoLineYs)
        {
            Coordinates firstPoint = new(0, y);
            Coordinates secondPoint = new(1, y + GridLineSlope);
            CoordinateLine line = new CoordinateLine(firstPoint, secondPoint).ExtendTo(limits.Rect);
            if (line.Start.X > limits.Right)
            {
                return lines;
            }

            lines.Add(Axes.GetPixelLine(line));
        }

        return lines;
    }

    /// <summary>Creates isolines from the configured manual positions.</summary>
    /// <param name="rp">The render context.</param>
    /// <returns>The pixel-space isolines to render.</returns>
    private List<PixelLine> GetLinesManual(RenderPack rp)
    {
        List<PixelLine> lines = [];
        AxisLimits limits = rp.Plot.Axes.GetLimits();

        for (int i = 0; i < ManualPositions.Count; i++)
        {
            (double y, _) = ManualPositions[i];
            Coordinates firstPoint = new(0, y);
            Coordinates secondPoint = new(1, y + GridLineSlope);
            CoordinateLine line = new CoordinateLine(firstPoint, secondPoint).ExtendTo(limits.Rect);
            if (line.Start.X > limits.Right)
            {
                return lines;
            }

            lines.Add(Axes.GetPixelLine(line));
        }

        return lines;
    }
}
