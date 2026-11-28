// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;
using CrissCross.Plot.TickGenerators;

namespace CrissCross.Plot.AxisPanels;

/// <summary>Provides shared behavior and settings for plot axes.</summary>
public abstract class AxisBase : LabelStyleProperties
{
    /// <summary>Horizontal padding used for labels when the axis has no data.</summary>
    private const float EmptyLabelHorizontalPadding = 10;

    /// <summary>Vertical padding used for labels when the axis has no data.</summary>
    private const float EmptyLabelVerticalPadding = 5;

    /// <summary>Horizontal padding between tick marks and axis labels.</summary>
    private const float TickLabelHorizontalPadding = 5;

    /// <summary>Vertical padding between tick marks and axis labels.</summary>
    private const float TickLabelVerticalPadding = 3;

    /// <summary>Padding outside the axis labels.</summary>
    private const float OutsideLabelPadding = 2;

    /// <summary>Pixel adjustment used to align an antialias-free top frame.</summary>
    private const float TopFramePixelAdjustment = 0.1F;

    /// <summary>Default font size used for axis labels.</summary>
    private const float DefaultLabelFontSize = 16;

    /// <summary>Default rotation used for axis labels.</summary>
    private const float DefaultLabelRotation = -90;

    /// <summary>Default length of major tick marks.</summary>
    private const float MajorTickLength = 4;

    /// <summary>Default length of minor tick marks.</summary>
    private const float MinorTickLength = 2;

    /// <summary>Distance between horizontal tick marks and their labels.</summary>
    private const float HorizontalTickLabelDistance = 2;

    /// <summary>Distance between vertical tick marks and their labels.</summary>
    private const float VerticalTickLabelDistance = 5;

    /// <summary>Gets or sets whether this axis is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets the edge of the data area on which this axis is positioned.</summary>
    public abstract Edge Edge { get; }

    /// <summary>Gets the coordinate range displayed by this axis.</summary>
    public virtual CoordinateRangeMutable Range { get; } = CoordinateRangeMutable.NotSet;

    /// <summary>Gets or sets the minimum space reserved for this axis, in pixels.</summary>
    public float MinimumSize { get; set; }

    /// <summary>Gets or sets the maximum space reserved for this axis, in pixels.</summary>
    public float MaximumSize { get; set; } = float.MaxValue;

    /// <summary>Gets or sets the space reserved for this axis when there is no data, in pixels.</summary>
    public float SizeWhenNoData { get; set; } = 15;

    /// <summary>Gets or sets padding around labels when no data is available.</summary>
    public PixelPadding EmptyLabelPadding { get; set; } = new(EmptyLabelHorizontalPadding, EmptyLabelVerticalPadding);

    /// <summary>Gets or sets the padding between tick marks and axis labels.</summary>
    public PixelPadding PaddingBetweenTickAndAxisLabels { get; set; } = new(TickLabelHorizontalPadding, TickLabelVerticalPadding);

    /// <summary>Gets or sets padding outside the axis labels.</summary>
    public PixelPadding PaddingOutsideAxisLabels { get; set; } = new(OutsideLabelPadding, OutsideLabelPadding);

    /// <summary>Gets or sets whether labels are clipped to the boundaries of the data area.</summary>
    public bool ClipLabel { get; set; }

    /// <summary>Gets or sets the minimum coordinate shown by this axis.</summary>
    public double Min
    {
        get => Range.Min;
        set => Range.Min = value;
    }

    /// <summary>Gets or sets the maximum coordinate shown by this axis.</summary>
    public double Max
    {
        get => Range.Max;
        set => Range.Max = value;
    }

    /// <summary>Gets or sets the generator that supplies this axis's ticks.</summary>
    public virtual ITickGenerator TickGenerator { get; set; } = null!;

    /// <summary>Gets or sets the label style used for the axis title.</summary>
    public override LabelStyle LabelStyle { get; set; } = new() { Text = string.Empty, FontSize = DefaultLabelFontSize, Bold = true, Rotation = DefaultLabelRotation, };

    /// <summary>Gets or sets whether debug information is shown for this axis.</summary>
    public bool ShowDebugInformation { get; set; }

    /// <summary>Gets the line style used to draw the axis frame.</summary>
    public LineStyle FrameLineStyle { get; } = new() { Width = 1, Color = Colors.Black, AntiAlias = false, };

    /// <summary>Gets or sets the style used for major tick marks.</summary>
    public TickMarkStyle MajorTickStyle { get; set; } = new() { Length = MajorTickLength, Width = 1, Color = Colors.Black, AntiAlias = false, };

    /// <summary>Gets or sets the style used for minor tick marks.</summary>
    public TickMarkStyle MinorTickStyle { get; set; } = new() { Length = MinorTickLength, Width = 1, Color = Colors.Black, AntiAlias = false, };

    /// <summary>Gets or sets the style used for tick labels.</summary>
    public LabelStyle TickLabelStyle { get; set; } = new() { Alignment = Alignment.MiddleCenter };

    /// <summary>Gets or sets the legacy label-style property.</summary>
    /// <remarks>Use <see cref="LabelStyle"/> or its individual properties instead.</remarks>
    [Obsolete("use LabelText, LabelFontColor, LabelFontSize, LabelFontName, etc. or properties of LabelStyle", false)]
    [SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310",
        Justification = "This obsolete public property is retained for source and binary compatibility.")]
    public LabelStyle Label => LabelStyle;

    /// <summary>Draws a line along an axis edge on the side of the data area.</summary>
    /// <param name="rp">The render pack used for drawing.</param>
    /// <param name="panelRect">The pixel rectangle occupied by the panel.</param>
    /// <param name="edge">The edge on which to draw the frame.</param>
    /// <param name="lineStyle">The style of the frame line.</param>
    public static void DrawFrame(RenderPack rp, PixelRect panelRect, Edge edge, LineStyle lineStyle)
    {
        PixelLine line = edge switch
        {
            Edge.Left => new(panelRect.Right, panelRect.Bottom, panelRect.Right, panelRect.Top),
            Edge.Right => new(panelRect.Left, panelRect.Bottom, panelRect.Left, panelRect.Top),
            Edge.Bottom => new(panelRect.Left, panelRect.Top, panelRect.Right, panelRect.Top),
            Edge.Top => new(panelRect.Left, panelRect.Bottom, panelRect.Right, panelRect.Bottom),
            _ => throw new ArgumentOutOfRangeException(nameof(edge), edge, "Unsupported axis edge."),
        };

        if (edge == Edge.Top && !lineStyle.AntiAlias)
        {
            line = line.WithDelta(0, TopFramePixelAdjustment);
        }

        Drawing.DrawLine(rp.Canvas, rp.Paint, line, lineStyle);
    }

    /// <summary>Draws tick marks and labels for an axis.</summary>
    /// <param name="rp">The render pack used for drawing.</param>
    /// <param name="label">The style used for tick labels.</param>
    /// <param name="panelRect">The pixel rectangle occupied by the panel.</param>
    /// <param name="ticks">The ticks to draw.</param>
    /// <param name="axis">The axis that defines the coordinate-to-pixel mapping.</param>
    /// <param name="majorStyle">The style used for major tick marks.</param>
    /// <param name="minorStyle">The style used for minor tick marks.</param>
    public static void DrawTicks(
        RenderPack rp,
        LabelStyle label,
        PixelRect panelRect,
        IEnumerable<Tick> ticks,
        IAxis axis,
        TickMarkStyle majorStyle,
        TickMarkStyle minorStyle)
    {
        if (axis.Edge.IsVertical())
        {
            DrawTicksVerticalAxis(rp, label, panelRect, ticks, axis, majorStyle, minorStyle);
        }
        else
        {
            DrawTicksHorizontalAxis(rp, label, panelRect, ticks, axis, majorStyle, minorStyle);
        }
    }

    /// <summary>Returns a string representation of this axis and its range.</summary>
    /// <returns>A string representation of this axis and its range.</returns>
    public override string ToString()
    {
        string? baseText = base.ToString();
        return $"{baseText} {Range}";
    }

    /// <summary>Applies a single color to all axis components.</summary>
    /// <param name="color">The color applied to the axis label, tick labels, tick marks, and frame.</param>
    public void Color(Color color)
    {
        LabelStyle.ForeColor = color;
        TickLabelStyle.ForeColor = color;
        MajorTickStyle.Color = color;
        MinorTickStyle.Color = color;
        FrameLineStyle.Color = color;
    }

    /// <summary>Replaces this axis's tick generator with one pre-loaded with the specified ticks.</summary>
    /// <param name="xs">The coordinates of the major ticks.</param>
    /// <param name="labels">The labels associated with the major ticks.</param>
    /// <exception cref="ArgumentException">The coordinate and label arrays have different lengths.</exception>
    public void SetTicks(double[] xs, string[] labels)
    {
        if (xs.Length != labels.Length)
        {
            throw new ArgumentException($"{nameof(xs)} and {nameof(labels)} must have equal length");
        }

        NumericManual manualTickGen = new();

        for (int i = 0; i < xs.Length; i++)
        {
            manualTickGen.AddMajor(xs[i], labels[i]);
        }

        TickGenerator = manualTickGen;
    }

    /// <summary>Draws tick marks and labels for a horizontal axis.</summary>
    /// <param name="rp">The render pack used for drawing.</param>
    /// <param name="label">The style used for tick labels.</param>
    /// <param name="panelRect">The pixel rectangle occupied by the panel.</param>
    /// <param name="ticks">The ticks to draw.</param>
    /// <param name="axis">The axis whose ticks are drawn.</param>
    /// <param name="majorStyle">The style used for major tick marks.</param>
    /// <param name="minorStyle">The style used for minor tick marks.</param>
    private static void DrawTicksHorizontalAxis(
        RenderPack rp,
        LabelStyle label,
        PixelRect panelRect,
        IEnumerable<Tick> ticks,
        IAxis axis,
        TickMarkStyle majorStyle,
        TickMarkStyle minorStyle)
    {
        if (axis.Edge != Edge.Bottom && axis.Edge != Edge.Top)
        {
            throw new InvalidOperationException();
        }

        foreach (Tick tick in ticks)
        {
            DrawHorizontalTick(rp, label, panelRect, tick, axis, majorStyle, minorStyle);
        }
    }

    /// <summary>Draws one tick and its label on a horizontal axis.</summary>
    /// <param name="rp">The render pack used for drawing.</param>
    /// <param name="label">The style used for the tick label.</param>
    /// <param name="panelRect">The pixel rectangle occupied by the panel.</param>
    /// <param name="tick">The tick to draw.</param>
    /// <param name="axis">The axis that defines the coordinate-to-pixel mapping.</param>
    /// <param name="majorStyle">The style used for major tick marks.</param>
    /// <param name="minorStyle">The style used for minor tick marks.</param>
    private static void DrawHorizontalTick(
        RenderPack rp,
        LabelStyle label,
        PixelRect panelRect,
        Tick tick,
        IAxis axis,
        TickMarkStyle majorStyle,
        TickMarkStyle minorStyle)
    {
        bool isBottom = axis.Edge == Edge.Bottom;
        TickMarkStyle tickStyle = tick.IsMajor ? majorStyle : minorStyle;
        float tickLength = tickStyle.Length;
        float x = axis.GetPixel(tick.Position, panelRect);
        float axisY = isBottom ? panelRect.Top : panelRect.Bottom;
        float tickEndY = isBottom ? axisY + tickLength : axisY - tickLength;
        PixelLine line = new(x, axisY, x, tickEndY);
        DrawTickMark(rp, tickStyle, line);

        if (string.IsNullOrWhiteSpace(tick.Label) || !label.IsVisible)
        {
            return;
        }

        label.Text = tick.Label;
        float labelDistance = tickLength + HorizontalTickLabelDistance;
        float labelY = isBottom ? axisY + labelDistance : axisY - labelDistance;
        Pixel labelPixel = new(x, labelY);
        if (label.Rotation == 0)
        {
            label.Alignment = isBottom ? Alignment.UpperCenter : Alignment.LowerCenter;
        }

        label.Render(rp.Canvas, labelPixel, rp.Paint);
    }

    /// <summary>Draws tick marks and labels for a vertical axis.</summary>
    /// <param name="rp">The render pack used for drawing.</param>
    /// <param name="label">The style used for tick labels.</param>
    /// <param name="panelRect">The pixel rectangle occupied by the panel.</param>
    /// <param name="ticks">The ticks to draw.</param>
    /// <param name="axis">The axis whose ticks are drawn.</param>
    /// <param name="majorStyle">The style used for major tick marks.</param>
    /// <param name="minorStyle">The style used for minor tick marks.</param>
    private static void DrawTicksVerticalAxis(
        RenderPack rp,
        LabelStyle label,
        PixelRect panelRect,
        IEnumerable<Tick> ticks,
        IAxis axis,
        TickMarkStyle majorStyle,
        TickMarkStyle minorStyle)
    {
        if (axis.Edge != Edge.Left && axis.Edge != Edge.Right)
        {
            throw new InvalidOperationException();
        }

        foreach (Tick tick in ticks)
        {
            DrawVerticalTick(rp, label, panelRect, tick, axis, majorStyle, minorStyle);
        }
    }

    /// <summary>Draws one tick and its label on a vertical axis.</summary>
    /// <param name="rp">The render pack used for drawing.</param>
    /// <param name="label">The style used for the tick label.</param>
    /// <param name="panelRect">The pixel rectangle occupied by the panel.</param>
    /// <param name="tick">The tick to draw.</param>
    /// <param name="axis">The axis that defines the coordinate-to-pixel mapping.</param>
    /// <param name="majorStyle">The style used for major tick marks.</param>
    /// <param name="minorStyle">The style used for minor tick marks.</param>
    private static void DrawVerticalTick(
        RenderPack rp,
        LabelStyle label,
        PixelRect panelRect,
        Tick tick,
        IAxis axis,
        TickMarkStyle majorStyle,
        TickMarkStyle minorStyle)
    {
        bool isLeft = axis.Edge == Edge.Left;
        TickMarkStyle tickStyle = tick.IsMajor ? majorStyle : minorStyle;
        float tickLength = tickStyle.Length;
        float y = axis.GetPixel(tick.Position, panelRect);
        float axisX = isLeft ? panelRect.Right : panelRect.Left;
        float tickEndX = isLeft ? axisX - tickLength : axisX + tickLength;
        PixelLine line = new(axisX, y, tickEndX, y);
        DrawTickMark(rp, tickStyle, line);

        if (string.IsNullOrWhiteSpace(tick.Label) || !label.IsVisible)
        {
            return;
        }

        label.Text = tick.Label;
        float labelDistance = tickLength + VerticalTickLabelDistance;
        float labelX = isLeft ? axisX - labelDistance : axisX + labelDistance;
        Pixel labelPixel = new(labelX, y);
        if (label.Rotation == 0)
        {
            label.Alignment = isLeft ? Alignment.MiddleRight : Alignment.MiddleLeft;
        }

        label.Render(rp.Canvas, labelPixel, rp.Paint);
    }

    /// <summary>Draws a tick-mark line using its configured style.</summary>
    /// <param name="rp">The render pack used for drawing.</param>
    /// <param name="style">The style applied to the tick mark.</param>
    /// <param name="line">The pixel line representing the tick mark.</param>
    private static void DrawTickMark(RenderPack rp, TickMarkStyle style, PixelLine line)
    {
        Paint paint = rp.Paint;
        paint.Color = style.Color;
        paint.StrokeWidth = style.Width;
        paint.IsAntialias = style.AntiAlias;
        style.Render(rp.Canvas, paint, line);
    }
}
