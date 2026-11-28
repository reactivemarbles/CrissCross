// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.Plottables;

namespace CrissCross.Plot;

/// <summary>Provides methods to create and add annotations and geometry plottables.</summary>
public partial class PlottableAdder
{
    /// <summary>Creates and adds an Annotation to this plot.</summary>
    /// <param name="text">The text content to display.</param>
    /// <param name="alignment">The alignment of the annotation.</param>
    /// <returns>The created Annotation.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Annotation Annotation(string text, Alignment alignment = Alignment.UpperLeft)
    {
        Annotation an = new()
        {
            Alignment = alignment,
            Text = text,
            LabelBackgroundColor = Colors.Yellow.WithAlpha(AnnotationBackgroundAlpha),
            LabelBorderColor = Colors.Black,
            LabelPadding = AnnotationLabelPadding,
        };

        Plot.PlottableList.Add(an);

        return an;
    }

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="center">The center coordinate of the shape.</param>
    /// <param name="outerRadiusX">The outer radius along the x-axis.</param>
    /// <param name="outerRadiusY">The outer radius along the y-axis.</param>
    /// <param name="innerRadiusX">The inner radius along the x-axis.</param>
    /// <param name="innerRadiusY">The inner radius along the y-axis.</param>
    /// <param name="startAngle">The starting angle of the arc or sector.</param>
    /// <param name="sweepAngle">The angular distance covered by the arc or sector.</param>
    /// <param name="rotation">An optional rotation applied to the shape.</param>
    /// <returns>The created Ellipse.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1472", Justification = "The established overloads are retained for source compatibility.")]
    public Ellipse AnnularEllipticalSector(
        Coordinates center,
        double outerRadiusX,
        double outerRadiusY,
        double innerRadiusX,
        double innerRadiusY,
        Angle startAngle,
        Angle sweepAngle,
        Angle? rotation = null)
    {
        Color color = GetNextColor();

        Ellipse ellipticalSector = new()
        {
            Center = center,
            RadiusX = outerRadiusX,
            RadiusY = outerRadiusY,
            InnerRadiusX = innerRadiusX,
            InnerRadiusY = innerRadiusY,
            StartAngle = startAngle,
            SweepAngle = sweepAngle,
            Rotation = rotation ?? Angle.FromDegrees(0),
            LineColor = color,
            IsSector = true,
        };

        Plot.PlottableList.Add(ellipticalSector);
        return ellipticalSector;
    }

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="xCenter">The x-coordinate of the center.</param>
    /// <param name="yCenter">The y-coordinate of the center.</param>
    /// <param name="outerRadiusX">The outer radius along the x-axis.</param>
    /// <param name="outerRadiusY">The outer radius along the y-axis.</param>
    /// <param name="innerRadiusX">The inner radius along the x-axis.</param>
    /// <param name="innerRadiusY">The inner radius along the y-axis.</param>
    /// <param name="startAngle">The starting angle of the arc or sector.</param>
    /// <param name="sweepAngle">The angular distance covered by the arc or sector.</param>
    /// <param name="rotation">An optional rotation applied to the shape.</param>
    /// <returns>The created Ellipse.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Existing public parameter names support named-argument callers.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1472", Justification = "The established overloads are retained for source compatibility.")]
    public Ellipse AnnularEllipticalSector(
        int xCenter,
        int yCenter,
        double outerRadiusX,
        double outerRadiusY,
        double innerRadiusX,
        double innerRadiusY,
        Angle startAngle,
        Angle sweepAngle,
        Angle? rotation = null) => AnnularEllipticalSector(new(xCenter, yCenter), outerRadiusX, outerRadiusY, innerRadiusX, innerRadiusY, startAngle, sweepAngle, rotation);

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="center">The center coordinate of the shape.</param>
    /// <param name="outerRadius">The outer radius.</param>
    /// <param name="innerRadius">The inner radius.</param>
    /// <param name="startAngle">The starting angle of the arc or sector.</param>
    /// <param name="sweepAngle">The angular distance covered by the arc or sector.</param>
    /// <param name="rotation">An optional rotation applied to the shape.</param>
    /// <returns>The created Ellipse.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Ellipse AnnularSector(
        Coordinates center,
        double outerRadius,
        double innerRadius,
        Angle startAngle,
        Angle sweepAngle,
        Angle? rotation = null) => AnnularEllipticalSector(center, outerRadius, outerRadius, innerRadius, innerRadius, startAngle, sweepAngle, rotation);

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="xCenter">The x-coordinate of the center.</param>
    /// <param name="yCenter">The y-coordinate of the center.</param>
    /// <param name="outerRadius">The outer radius.</param>
    /// <param name="innerRadius">The inner radius.</param>
    /// <param name="startAngle">The starting angle of the arc or sector.</param>
    /// <param name="sweepAngle">The angular distance covered by the arc or sector.</param>
    /// <param name="rotation">An optional rotation applied to the shape.</param>
    /// <returns>The created Ellipse.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Existing public parameter names support named-argument callers.")]
    public Ellipse AnnularSector(
        int xCenter,
        int yCenter,
        double outerRadius,
        double innerRadius,
        Angle startAngle,
        Angle sweepAngle,
        Angle? rotation = null) => AnnularSector(new(xCenter, yCenter), outerRadius, innerRadius, startAngle, sweepAngle, rotation);

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="center">The center coordinate of the shape.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="startAngle">The starting angle of the arc or sector.</param>
    /// <param name="sweepAngle">The angular distance covered by the arc or sector.</param>
    /// <returns>The created Ellipse.</returns>
    public Ellipse Arc(Coordinates center, double radius, Angle startAngle, Angle sweepAngle) => EllipticalArc(center, radius, radius, startAngle, sweepAngle);

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="xCenter">The x-coordinate of the center.</param>
    /// <param name="yCenter">The y-coordinate of the center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="startAngle">The starting angle of the arc or sector.</param>
    /// <param name="sweepAngle">The angular distance covered by the arc or sector.</param>
    /// <returns>The created Ellipse.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Existing public parameter names support named-argument callers.")]
    public Ellipse Arc(double xCenter, double yCenter, double radius, Angle startAngle, Angle sweepAngle) => Arc(new(xCenter, yCenter), radius, startAngle, sweepAngle);

    /// <summary>Creates and adds an Arrow to this plot.</summary>
    /// <param name="arrowBase">The arrow base to use.</param>
    /// <param name="arrowTip">The arrow tip to use.</param>
    /// <returns>The created Arrow.</returns>
    public Arrow Arrow(Coordinates arrowBase, Coordinates arrowTip)
    {
        Color color = GetNextColor();

        Arrow arrow = new() { Base = arrowBase, Tip = arrowTip, ArrowLineColor = color, ArrowFillColor = color.WithAlpha(ArrowFillAlpha), };

        Plot.PlottableList.Add(arrow);

        return arrow;
    }

    /// <summary>Creates and adds an Arrow to this plot.</summary>
    /// <param name="xBase">The x-coordinate of the arrow base.</param>
    /// <param name="yBase">The y-coordinate of the arrow base.</param>
    /// <param name="xTip">The x-coordinate of the arrow tip.</param>
    /// <param name="yTip">The y-coordinate of the arrow tip.</param>
    /// <returns>The created Arrow.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Existing public parameter names support named-argument callers.")]
    public Arrow Arrow(double xBase, double yBase, double xTip, double yTip)
    {
        Coordinates arrowBase = new(xBase, yBase);
        Coordinates arrowTip = new(xTip, yTip);
        return Arrow(arrowBase, arrowTip);
    }

    /// <summary>Creates and adds an Arrow to this plot.</summary>
    /// <param name="line">The line defining the geometry.</param>
    /// <returns>The created Arrow.</returns>
    public Arrow Arrow(CoordinateLine line) => Arrow(line.Start, line.End);

    /// <summary>Creates and adds text behind the other plottables.</summary>
    /// <param name="text">The text content to display.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <param name="size">The size of the marker or text.</param>
    /// <returns>The created Annotation.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Annotation BackgroundText(string text, Color? color = null, double size = 48)
    {
        Annotation an = new()
        {
            Text = text,
            LabelFontColor = color ?? Colors.Gray.WithAlpha(BackgroundTextAlpha),
            LabelFontSize = (float)size,
            LabelBackgroundColor = Colors.Transparent,
            LabelShadowColor = Colors.Transparent,
            LabelBorderColor = Colors.Transparent,
            Alignment = Alignment.MiddleCenter,
            OffsetX = 0,
            OffsetY = 0,
        };

        Plot.PlottableList.Insert(0, an);

        return an;
    }

    /// <summary>Creates and adds text behind the other plottables.</summary>
    /// <param name="line1">The line1 to use.</param>
    /// <param name="line2">The line2 to use.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <param name="size1">The font size for the first line.</param>
    /// <param name="size2">The font size for the second line.</param>
    /// <returns>The annotations added to the plot.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public (Annotation line1, Annotation line2) BackgroundText(string line1, string line2, Color? color = null, double size1 = 48, double size2 = 24)
    {
        Annotation an1 = BackgroundText(line1, color, size1);
        an1.Alignment = Alignment.LowerCenter;
        an1.FractionRect = FractionRect.Row(0, BackgroundTextRowCount);

        Annotation an2 = BackgroundText(line2, color, size2);
        an2.Alignment = Alignment.UpperCenter;
        an2.FractionRect = FractionRect.Row(1, BackgroundTextRowCount);

        return (an1, an2);
    }

    /// <summary>Creates and adds a BarPlot to this plot.</summary>
    /// <param name="bar">The bar to use.</param>
    /// <returns>The created BarPlot.</returns>
    public BarPlot Bar(Bar bar)
    {
        List<Bar> bars = [bar];
        BarPlot bp = new(bars);
        _ = Plottable(bp);
        return bp;
    }

    /// <summary>Creates and adds a BarPlot to this plot.</summary>
    /// <param name="position">The position of the bar.</param>
    /// <param name="value">The value represented by the bar.</param>
    /// <param name="error">The error to use.</param>
    /// <returns>The created BarPlot.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public BarPlot Bar(double position, double value, double error = 0)
    {
        Bar bar = new() { Position = position, Value = value, Error = error, FillColor = GetNextColor(), };
        return Bar(bar);
    }

    /// <summary>Creates and adds a BarPlot to this plot.</summary>
    /// <param name="bars">The bars to add.</param>
    /// <returns>The created BarPlot.</returns>
    public BarPlot Bars(List<Bar> bars)
    {
        BarPlot bp = new(bars);
        _ = Plottable(bp);
        return bp;
    }

    /// <summary>Creates and adds a BarPlot to this plot.</summary>
    /// <param name="bars">The bars to add.</param>
    /// <returns>The created BarPlot.</returns>
    public BarPlot Bars(Bar[] bars)
    {
        BarPlot bp = new([.. bars]);
        _ = Plottable(bp);
        return bp;
    }

    /// <summary>Creates and adds a BarPlot to this plot.</summary>
    /// <param name="values">The values represented by the plot.</param>
    /// <returns>The created BarPlot.</returns>
    public BarPlot Bars(double[] values)
    {
        double[] positions = new double[values.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = i;
        }

        return Bars(positions, values);
    }

    /// <summary>Creates and adds a BarPlot to this plot.</summary>
    /// <typeparam name="T">The type of values accepted by this overload.</typeparam>
    /// <param name="positions">The positions to use.</param>
    /// <param name="values">The values represented by the plot.</param>
    /// <returns>The created BarPlot.</returns>
    public BarPlot Bars<T>(IEnumerable<double> positions, IEnumerable<T> values)
    {
        double[] values2 = NumericConversion.GenericToDoubleArray(values);
        return Bars(positions, values2);
    }

    /// <summary>Creates and adds a BarPlot to this plot.</summary>
    /// <param name="positions">The positions to use.</param>
    /// <param name="values">The values represented by the plot.</param>
    /// <returns>The created BarPlot.</returns>
    public BarPlot Bars(IEnumerable<double> positions, IEnumerable<double> values)
    {
        List<double> positionList = [.. positions];
        List<double> valueList = [.. values];

        if (positionList.Count != valueList.Count)
        {
            throw new ArgumentException($"{nameof(positions)} and {nameof(values)} have different lengths");
        }

        Color color = GetNextColor();
        List<Bar> bars = [with(capacity: positionList.Count)];
        for (int i = 0; i < positionList.Count; i++)
        {
            bars.Add(new() { Position = positionList[i], Value = valueList[i], FillColor = color });
        }

        return Bars(bars);
    }

    /// <summary>Creates and adds a BoxPlot to this plot.</summary>
    /// <param name="box">The box statistics to add.</param>
    /// <returns>The created BoxPlot.</returns>
    public BoxPlot Box(Box box)
    {
        BoxPlot bp = new();
        bp.Boxes.Add(box);
        bp.FillColor = GetNextColor();
        Plot.PlottableList.Add(bp);
        return bp;
    }

    /// <summary>Creates and adds a BoxPlot to this plot.</summary>
    /// <param name="boxes">The box statistics to add.</param>
    /// <returns>The created BoxPlot.</returns>
    public BoxPlot Boxes(IEnumerable<Box> boxes)
    {
        BoxPlot bp = new();
        bp.Boxes.AddRange(boxes);
        bp.FillColor = GetNextColor();
        Plot.PlottableList.Add(bp);
        return bp;
    }

    /// <summary>Creates and adds a Bracket to this plot.</summary>
    /// <param name="point1">The point1 to use.</param>
    /// <param name="point2">The point2 to use.</param>
    /// <param name="label">The optional label text.</param>
    /// <returns>The created Bracket.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Bracket Bracket(Coordinates point1, Coordinates point2, string? label = null)
    {
        Bracket bracket = new() { Point1 = point1, Point2 = point2, Text = label ?? string.Empty, };

        Plot.PlottableList.Add(bracket);

        return bracket;
    }

    /// <summary>Creates and adds a Bracket to this plot.</summary>
    /// <param name="x1">The first x-coordinate.</param>
    /// <param name="y1">The first y-coordinate.</param>
    /// <param name="x2">The second x-coordinate.</param>
    /// <param name="y2">The second y-coordinate.</param>
    /// <param name="label">The optional label text.</param>
    /// <returns>The created Bracket.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Bracket Bracket(double x1, double y1, double x2, double y2, string? label = null)
    {
        Coordinates point1 = new(x1, y1);
        Coordinates point2 = new(x2, y2);
        return Bracket(point1, point2, label);
    }

    /// <summary>Creates and adds a Bracket to this plot.</summary>
    /// <param name="line">The line defining the geometry.</param>
    /// <param name="label">The optional label text.</param>
    /// <returns>The created Bracket.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Bracket Bracket(CoordinateLine line, string? label = null) => Bracket(line.Start, line.End, label);

    /// <summary>Creates and adds a Callout to this plot.</summary>
    /// <param name="text">The text content to display.</param>
    /// <param name="textX">The text x to use.</param>
    /// <param name="textY">The text y to use.</param>
    /// <param name="tipX">The x-coordinate of the tooltip tip.</param>
    /// <param name="tipY">The y-coordinate of the tooltip tip.</param>
    /// <returns>The created Callout.</returns>
    public Callout Callout(string text, double textX, double textY, double tipX, double tipY)
    {
        Coordinates labelCoordinates = new(textX, textY);
        Coordinates lineCoordinates = new(tipX, tipY);
        return Callout(text, labelCoordinates, lineCoordinates);
    }

    /// <summary>Creates and adds a Callout to this plot.</summary>
    /// <param name="text">The text content to display.</param>
    /// <param name="textLocation">The text location to use.</param>
    /// <param name="tipLocation">The location of the tooltip tip.</param>
    /// <returns>The created Callout.</returns>
    public Callout Callout(string text, Coordinates textLocation, Coordinates tipLocation)
    {
        Color color = GetNextColor();

        Callout callout = new()
        {
            Text = text,
            TextCoordinates = textLocation,
            TipCoordinates = tipLocation,
            ArrowLineColor = Colors.Transparent,
            ArrowFillColor = color,
            TextBackgroundColor = color.Lighten(),
            TextBorderColor = color,
            TextBorderWidth = CalloutBorderWidth,
            TextColor = Colors.Black,
            FontSize = CalloutFontSize,
        };

        Plot.PlottableList.Add(callout);

        return callout;
    }
}
