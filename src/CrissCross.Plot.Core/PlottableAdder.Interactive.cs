// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.DataSources;
using CrissCross.Plot.Plottables;
using CrissCross.Plot.Plottables.Interactive;

namespace CrissCross.Plot;

/// <summary>Provides methods to create and add interactive plottables to a plot.</summary>
public partial class PlottableAdder
{
    /// <summary>Creates and adds an InteractiveHorizontalLine to this plot.</summary>
    /// <param name="y">The y to use.</param>
    /// <returns>The created InteractiveHorizontalLine.</returns>
    public InteractiveHorizontalLine InteractiveHorizontalLine(double y)
    {
        InteractiveHorizontalLine plottable = new() { Y = y };
        plottable.LineStyle.Color = GetNextColor();
        Plot.PlottableList.Add(plottable);
        return plottable;
    }

    /// <summary>Creates and adds an InteractiveHorizontalLineSegment to this plot.</summary>
    /// <param name="x1">The first x-coordinate.</param>
    /// <param name="x2">The second x-coordinate.</param>
    /// <param name="y">The y to use.</param>
    /// <returns>The created InteractiveHorizontalLineSegment.</returns>
    public InteractiveHorizontalLineSegment InteractiveHorizontalLineSegment(double x1, double x2, double y)
    {
        InteractiveHorizontalLineSegment plottable = new() { X1 = x1, X2 = x2, Y = y, };
        plottable.LineStyle.Color = GetNextColor();
        Plot.PlottableList.Add(plottable);
        return plottable;
    }

    /// <summary>Creates and adds an InteractiveHorizontalSpan to this plot.</summary>
    /// <param name="x1">The first x-coordinate.</param>
    /// <param name="x2">The second x-coordinate.</param>
    /// <returns>The created InteractiveHorizontalSpan.</returns>
    public InteractiveHorizontalSpan InteractiveHorizontalSpan(double x1, double x2)
    {
        InteractiveHorizontalSpan plottable = new() { X1 = x1, X2 = x2 };
        plottable.LineStyle.Color = GetNextColor();
        plottable.FillStyle.Color = plottable.LineStyle.Color.WithOpacity(InteractiveFillOpacity);
        Plot.PlottableList.Add(plottable);
        return plottable;
    }

    /// <summary>Creates and adds an InteractiveLineSegment to this plot.</summary>
    /// <param name="line">The line defining the geometry.</param>
    /// <returns>The created InteractiveLineSegment.</returns>
    public InteractiveLineSegment InteractiveLineSegment(CoordinateLine line)
    {
        InteractiveLineSegment plottable = new() { Line = line, Color = GetNextColor(), };
        Plot.PlottableList.Add(plottable);
        return plottable;
    }

    /// <summary>Creates and adds an InteractiveMarker to this plot.</summary>
    /// <param name="point">The coordinate to plot.</param>
    /// <returns>The created InteractiveMarker.</returns>
    public InteractiveMarker InteractiveMarker(Coordinates point)
    {
        InteractiveMarker plottable = new() { Point = point };
        plottable.MarkerStyle.MarkerColor = GetNextColor();
        Plot.PlottableList.Add(plottable);
        return plottable;
    }

    /// <summary>Creates and adds an InteractiveRectangle to this plot.</summary>
    /// <param name="rect">The rect to use.</param>
    /// <returns>The created InteractiveRectangle.</returns>
    public InteractiveRectangle InteractiveRectangle(CoordinateRect rect)
    {
        InteractiveRectangle plottable = new() { Rect = rect };
        plottable.LineStyle.Color = GetNextColor();
        plottable.FillStyle.Color = plottable.LineStyle.Color.WithAlpha(InteractiveFillOpacity);
        Plot.PlottableList.Add(plottable);
        return plottable;
    }

    /// <summary>Creates and adds an InteractiveTrendLine to this plot.</summary>
    /// <param name="line">The line defining the geometry.</param>
    /// <returns>The created InteractiveTrendLine.</returns>
    public InteractiveTrendLine InteractiveTrendLine(CoordinateLine line)
    {
        InteractiveTrendLine plottable = new() { Line = line, Color = GetNextColor(), };
        Plot.PlottableList.Add(plottable);
        return plottable;
    }

    /// <summary>Creates and adds an InteractiveVerticalLine to this plot.</summary>
    /// <param name="x">The x to use.</param>
    /// <returns>The created InteractiveVerticalLine.</returns>
    public InteractiveVerticalLine InteractiveVerticalLine(double x)
    {
        InteractiveVerticalLine plottable = new() { X = x };
        plottable.LineStyle.Color = GetNextColor();
        plottable.LabelBackgroundColor = plottable.LineStyle.Color;
        plottable.LabelFontColor = Colors.White;
        Plot.PlottableList.Add(plottable);
        return plottable;
    }

    /// <summary>Creates and adds an InteractiveVerticalLineSegment to this plot.</summary>
    /// <param name="x">The x to use.</param>
    /// <param name="y1">The first y-coordinate.</param>
    /// <param name="y2">The second y-coordinate.</param>
    /// <returns>The created InteractiveVerticalLineSegment.</returns>
    public InteractiveVerticalLineSegment InteractiveVerticalLineSegment(double x, double y1, double y2)
    {
        InteractiveVerticalLineSegment plottable = new() { X = x, Y1 = y1, Y2 = y2, };
        plottable.LineStyle.Color = GetNextColor();
        Plot.PlottableList.Add(plottable);
        return plottable;
    }

    /// <summary>Creates and adds an InteractiveVerticalSpan to this plot.</summary>
    /// <param name="y1">The first y-coordinate.</param>
    /// <param name="y2">The second y-coordinate.</param>
    /// <returns>The created InteractiveVerticalSpan.</returns>
    public InteractiveVerticalSpan InteractiveVerticalSpan(double y1, double y2)
    {
        InteractiveVerticalSpan plottable = new() { Y1 = y1, Y2 = y2 };
        plottable.LineStyle.Color = GetNextColor();
        plottable.FillStyle.Color = plottable.LineStyle.Color.WithOpacity(InteractiveFillOpacity);
        Plot.PlottableList.Add(plottable);
        return plottable;
    }

    /// <summary>Creates and adds a Legend to this plot.</summary>
    /// <returns>The created Legend.</returns>
    public Legend Legend()
    {
        Legend legend = new(Plot) { IsVisible = true };
        Plot.PlottableList.Add(legend);
        return legend;
    }

    /// <summary>Creates and adds a LinePlot to this plot.</summary>
    /// <param name="start">The start to use.</param>
    /// <param name="end">The end to use.</param>
    /// <returns>The created LinePlot.</returns>
    public LinePlot Line(Coordinates start, Coordinates end)
    {
        LinePlot lp = new() { Start = start, End = end, };

        lp.LineStyle.Color = GetNextColor();
        lp.MarkerStyle.FillColor = lp.LineStyle.Color;

        Plot.PlottableList.Add(lp);

        return lp;
    }

    /// <summary>Creates and adds a LinePlot to this plot.</summary>
    /// <param name="line">The line defining the geometry.</param>
    /// <returns>The created LinePlot.</returns>
    public LinePlot Line(CoordinateLine line) => Line(line.Start, line.End);

    /// <summary>Creates and adds a LinePlot to this plot.</summary>
    /// <param name="x1">The first x-coordinate.</param>
    /// <param name="y1">The first y-coordinate.</param>
    /// <param name="x2">The second x-coordinate.</param>
    /// <param name="y2">The second y-coordinate.</param>
    /// <returns>The created LinePlot.</returns>
    public LinePlot Line(double x1, double y1, double x2, double y2)
    {
        Coordinates start = new(x1, y1);
        Coordinates end = new(x2, y2);
        return Line(start, end);
    }

    /// <summary>Creates and adds a LollipopPlot to this plot.</summary>
    /// <param name="values">The values represented by the plot.</param>
    /// <returns>The created LollipopPlot.</returns>
    public LollipopPlot Lollipop(double[] values)
    {
        Coordinates[] coordinates = new Coordinates[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            coordinates[i] = new(i, values[i]);
        }

        return Lollipop(coordinates);
    }

    /// <summary>Creates and adds a LollipopPlot to this plot.</summary>
    /// <param name="values">The values represented by the plot.</param>
    /// <param name="positions">The positions to use.</param>
    /// <returns>The created LollipopPlot.</returns>
    public LollipopPlot Lollipop(double[] values, double[] positions)
    {
        Coordinates[] coordinates = Coordinates.Zip(positions, values);
        return Lollipop(coordinates);
    }

    /// <summary>Creates and adds a LollipopPlot to this plot.</summary>
    /// <param name="coordinates">The coordinates to plot.</param>
    /// <returns>The created LollipopPlot.</returns>
    public LollipopPlot Lollipop(IEnumerable<Coordinates> coordinates)
    {
        LollipopPlot plottable = new(coordinates) { Color = GetNextColor() };
        Plot.PlottableList.Add(plottable);
        return plottable;
    }

    /// <summary>Creates and adds a Marker to this plot.</summary>
    /// <param name="x">The x to use.</param>
    /// <param name="y">The y to use.</param>
    /// <param name="shape">The marker shape.</param>
    /// <param name="size">The size of the marker or text.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Marker.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Marker Marker(double x, double y, MarkerShape shape = MarkerShape.FilledCircle, float size = 10, Color? color = null)
    {
        Marker mp = new() { MarkerShape = shape, MarkerSize = size, Color = color ?? GetNextColor(), Location = new(x, y), };

        Plot.PlottableList.Add(mp);

        return mp;
    }

    /// <summary>Creates and adds a Marker to this plot.</summary>
    /// <param name="location">The location to use.</param>
    /// <param name="shape">The marker shape.</param>
    /// <param name="size">The size of the marker or text.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Marker.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Marker Marker(Coordinates location, MarkerShape shape = MarkerShape.FilledCircle, float size = 10, Color? color = null) => Marker(location.X, location.Y, shape, size, color);

    /// <summary>Creates and adds a Markers to this plot.</summary>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="shape">The marker shape.</param>
    /// <param name="size">The size of the marker or text.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Markers.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Plottables.Markers Markers(double[] xs, double[] ys, MarkerShape shape = MarkerShape.FilledCircle, float size = 10, Color? color = null)
    {
        ScatterSourceDoubleArray ss = new(xs, ys);

        Plottables.Markers mp = new(ss) { MarkerShape = shape, MarkerSize = size, Color = color ?? GetNextColor() };

        Plot.PlottableList.Add(mp);

        return mp;
    }

    /// <summary>Creates and adds a Markers to this plot.</summary>
    /// <param name="coordinates">The coordinates to plot.</param>
    /// <param name="shape">The marker shape.</param>
    /// <param name="size">The size of the marker or text.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Markers.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Plottables.Markers Markers(Coordinates[] coordinates, MarkerShape shape = MarkerShape.FilledCircle, float size = 10, Color? color = null)
    {
        ScatterSourceCoordinatesArray ss = new(coordinates);

        Plottables.Markers mp = new(ss) { MarkerShape = shape, MarkerSize = size, Color = color ?? GetNextColor() };

        Plot.PlottableList.Add(mp);

        return mp;
    }

    /// <summary>Creates and adds a Markers to this plot.</summary>
    /// <param name="coordinates">The coordinates to plot.</param>
    /// <param name="shape">The marker shape.</param>
    /// <param name="size">The size of the marker or text.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Markers.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Plottables.Markers Markers(List<Coordinates> coordinates, MarkerShape shape = MarkerShape.FilledCircle, float size = 10, Color? color = null)
    {
        ScatterSourceCoordinatesList ss = new(coordinates);

        Plottables.Markers mp = new(ss) { MarkerShape = shape, MarkerSize = size, Color = color ?? GetNextColor() };

        Plot.PlottableList.Add(mp);

        return mp;
    }

    /// <summary>Creates and adds a Markers to this plot.</summary>
    /// <typeparam name="TX">The type of the x-coordinate values.</typeparam>
    /// <typeparam name="TY">The type of the y-coordinate values.</typeparam>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="shape">The marker shape.</param>
    /// <param name="size">The size of the marker or text.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Markers.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Plottables.Markers Markers<TX, TY>(TX[] xs, TY[] ys, MarkerShape shape = MarkerShape.FilledCircle, float size = 10, Color? color = null)
    {
        ScatterSourceGenericArray<TX, TY> ss = new(xs, ys);

        Plottables.Markers mp = new(ss) { MarkerShape = shape, MarkerSize = size, Color = color ?? GetNextColor() };

        Plot.PlottableList.Add(mp);

        return mp;
    }

    /// <summary>Creates and adds a Markers to this plot.</summary>
    /// <typeparam name="TX">The type of the x-coordinate values.</typeparam>
    /// <typeparam name="TY">The type of the y-coordinate values.</typeparam>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="shape">The marker shape.</param>
    /// <param name="size">The size of the marker or text.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Markers.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Plottables.Markers Markers<TX, TY>(List<TX> xs, List<TY> ys, MarkerShape shape = MarkerShape.FilledCircle, float size = 10, Color? color = null)
    {
        ScatterSourceGenericList<TX, TY> ss = new(xs, ys);

        Plottables.Markers mp = new(ss) { MarkerShape = shape, MarkerSize = size, Color = color ?? GetNextColor() };

        Plot.PlottableList.Add(mp);

        return mp;
    }
}
