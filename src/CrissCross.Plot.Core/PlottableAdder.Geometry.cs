// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.DataSources;
using CrissCross.Plot.Panels;
using CrissCross.Plot.Plottables;
using CrissCross.Plot.Statistics;

namespace CrissCross.Plot;

/// <summary>Provides geometry and annotation plottable creation methods.</summary>
public partial class PlottableAdder
{
    /// <summary>Creates and adds a CandlestickPlot to this plot.</summary>
    /// <param name="ohlcs">The ohlcs to use.</param>
    /// <returns>The created CandlestickPlot.</returns>
    public CandlestickPlot Candlestick(OHLC[] ohlcs)
    {
        OHLCSourceArray dataSource = new(ohlcs);
        CandlestickPlot candlestickPlot = new(dataSource);
        Plot.PlottableList.Add(candlestickPlot);
        return candlestickPlot;
    }

    /// <summary>Creates and adds a CandlestickPlot to this plot.</summary>
    /// <param name="ohlcs">The ohlcs to use.</param>
    /// <returns>The created CandlestickPlot.</returns>
    public CandlestickPlot Candlestick(List<OHLC> ohlcs)
    {
        OHLCSourceList dataSource = new(ohlcs);
        CandlestickPlot candlestickPlot = new(dataSource);
        Plot.PlottableList.Add(candlestickPlot);
        return candlestickPlot;
    }

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="center">The center coordinate of the shape.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <returns>The created Ellipse.</returns>
    public Ellipse Circle(Coordinates center, double radius) => Ellipse(center, radius, radius);

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="xCenter">The x-coordinate of the center.</param>
    /// <param name="yCenter">The y-coordinate of the center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <returns>The created Ellipse.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Existing public parameter names support named-argument callers.")]
    public Ellipse Circle(double xCenter, double yCenter, double radius) => Circle(new(xCenter, yCenter), radius);

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="center">The center coordinate of the shape.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="startAngle">The starting angle of the arc or sector.</param>
    /// <param name="sweepAngle">The angular distance covered by the arc or sector.</param>
    /// <returns>The created Ellipse.</returns>
    public Ellipse CircleSector(Coordinates center, double radius, Angle startAngle, Angle sweepAngle) => EllipticalSector(center, radius, radius, startAngle, sweepAngle);

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="xCenter">The x-coordinate of the center.</param>
    /// <param name="yCenter">The y-coordinate of the center.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="startAngle">The starting angle of the arc or sector.</param>
    /// <param name="sweepAngle">The angular distance covered by the arc or sector.</param>
    /// <returns>The created Ellipse.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Existing public parameter names support named-argument callers.")]
    public Ellipse CircleSector(int xCenter, int yCenter, int radius, Angle startAngle, Angle sweepAngle) => CircleSector(new(xCenter, yCenter), radius, startAngle, sweepAngle);

    /// <summary>Creates and adds a color-bar panel for the supplied color-axis source.</summary>
    /// <param name="source">The source that provides plot data.</param>
    /// <param name="edge">The plot edge where the color bar is placed.</param>
    /// <returns>The created ColorBar.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public ColorBar ColorBar(IHasColorAxis source, Edge edge = Edge.Right)
    {
        ColorBar colorBar = new(source, edge);

        Plot.Axes.AddPanel(colorBar);
        return colorBar;
    }

    /// <summary>Creates and adds a ContourLines to this plot.</summary>
    /// <param name="coordinates">The coordinates to plot.</param>
    /// <param name="count">The number of contours to draw.</param>
    /// <returns>The created ContourLines.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("PerformanceSharp", "PSH1020", Justification = "Existing rectangular-array overloads are part of the public API.")]
    public ContourLines ContourLines(Coordinates3d[, ] coordinates, int count = 10)
    {
        ContourLines contour = new() { ContourLineCount = count };
        contour.Update(coordinates);
        Plot.PlottableList.Add(contour);
        return contour;
    }

    /// <summary>Creates and adds a ContourLines to this plot.</summary>
    /// <param name="coordinates">The coordinates to plot.</param>
    /// <param name="count">The number of contours to draw.</param>
    /// <returns>The created ContourLines.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public ContourLines ContourLines(Coordinates3d[] coordinates, int count = 10)
    {
        ContourLines contour = new() { ContourLineCount = count };
        contour.Update(coordinates);
        Plot.PlottableList.Add(contour);
        return contour;
    }

    /// <summary>Creates and adds a Coxcomb to this plot.</summary>
    /// <param name="slices">The slices to use.</param>
    /// <returns>The created Coxcomb.</returns>
    public Coxcomb Coxcomb(IList<PieSlice> slices)
    {
        Coxcomb coxcomb = new(slices);
        Plot.PlottableList.Add(coxcomb);
        return coxcomb;
    }

    /// <summary>Creates and adds a Coxcomb to this plot.</summary>
    /// <param name="values">The values represented by the plot.</param>
    /// <returns>The created Coxcomb.</returns>
    public Coxcomb Coxcomb(IEnumerable<double> values)
    {
        List<PieSlice> slices = new();
        foreach (double value in values)
        {
            PieSlice slice = new() { Value = value, FillColor = Palette.GetColor(slices.Count).WithOpacity(), };

            slices.Add(slice);
        }

        Coxcomb coxcomb = new(slices);
        Plot.PlottableList.Add(coxcomb);
        return coxcomb;
    }

    /// <summary>Creates and adds a Crosshair to this plot.</summary>
    /// <param name="x">The x to use.</param>
    /// <param name="y">The y to use.</param>
    /// <returns>The created Crosshair.</returns>
    public Crosshair Crosshair(double x, double y)
    {
        Crosshair ch = new() { Position = new(x, y) };
        Color color = GetNextColor();
        ch.LineColor = color;
        ch.TextColor = color;
        Plot.PlottableList.Add(ch);
        return ch;
    }

    /// <summary>Creates and adds a DataLogger to this plot.</summary>
    /// <returns>The created DataLogger.</returns>
    public DataLogger DataLogger()
    {
        List<Coordinates> coordinates = [];
        return DataLogger(coordinates);
    }

    /// <summary>Creates and adds a DataLogger to this plot.</summary>
    /// <param name="coordinates">The coordinates to plot.</param>
    /// <returns>The created DataLogger.</returns>
    public DataLogger DataLogger(List<Coordinates> coordinates)
    {
        DataLogger logger = new(coordinates) { Color = GetNextColor(), };

        Plot.PlottableList.Add(logger);

        return logger;
    }

    /// <summary>Creates and adds a DataStreamer to this plot.</summary>
    /// <param name="points">The number of points to allocate.</param>
    /// <param name="period">The interval between samples.</param>
    /// <returns>The created DataStreamer.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public DataStreamer DataStreamer(int points, double period = 1)
    {
        double[] data = Generate.NaN(points);

        DataStreamer streamer = new(Plot, data) { Color = GetNextColor(), Period = period, };

        Plot.PlottableList.Add(streamer);

        return streamer;
    }

    /// <summary>Creates and adds a DataStreamerXY to this plot.</summary>
    /// <param name="capacity">The maximum number of points to retain.</param>
    /// <returns>The created DataStreamerXY.</returns>
    public DataStreamerXY DataStreamerXY(int capacity)
    {
        DataStreamerXY streamer = new(capacity) { Color = GetNextColor(), };

        Plot.PlottableList.Add(streamer);

        return streamer;
    }

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="center">The center coordinate of the shape.</param>
    /// <param name="radiusX">The radius along the x-axis.</param>
    /// <param name="radiusY">The radius along the y-axis.</param>
    /// <param name="rotation">An optional rotation applied to the shape.</param>
    /// <returns>The created Ellipse.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Ellipse Ellipse(Coordinates center, double radiusX, double radiusY, Angle? rotation = null)
    {
        Color color = GetNextColor();

        Ellipse ellipse = new() { Center = center, RadiusX = radiusX, RadiusY = radiusY, Rotation = rotation ?? Angle.FromDegrees(0), LineColor = color, };

        Plot.PlottableList.Add(ellipse);
        return ellipse;
    }

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="xCenter">The x-coordinate of the center.</param>
    /// <param name="yCenter">The y-coordinate of the center.</param>
    /// <param name="radiusX">The radius along the x-axis.</param>
    /// <param name="radiusY">The radius along the y-axis.</param>
    /// <param name="rotation">An optional rotation applied to the shape.</param>
    /// <returns>The created Ellipse.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Existing public parameter names support named-argument callers.")]
    public Ellipse Ellipse(double xCenter, double yCenter, double radiusX, double radiusY, Angle? rotation = null) => Ellipse(new(xCenter, yCenter), radiusX, radiusY, rotation);

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="center">The center coordinate of the shape.</param>
    /// <param name="radiusX">The radius along the x-axis.</param>
    /// <param name="radiusY">The radius along the y-axis.</param>
    /// <param name="startAngle">The starting angle of the arc or sector.</param>
    /// <param name="sweepAngle">The angular distance covered by the arc or sector.</param>
    /// <param name="rotation">An optional rotation applied to the shape.</param>
    /// <returns>The created Ellipse.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Ellipse EllipticalArc(Coordinates center, double radiusX, double radiusY, Angle startAngle, Angle sweepAngle, Angle? rotation = null)
    {
        Color color = GetNextColor();

        Ellipse ellipticalSector = new()
        {
            Center = center,
            RadiusX = radiusX,
            RadiusY = radiusY,
            StartAngle = startAngle,
            SweepAngle = sweepAngle,
            Rotation = rotation ?? Angle.FromDegrees(0),
            LineColor = color,
            IsSector = false,
        };

        Plot.PlottableList.Add(ellipticalSector);
        return ellipticalSector;
    }

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="xCenter">The x-coordinate of the center.</param>
    /// <param name="yCenter">The y-coordinate of the center.</param>
    /// <param name="radiusX">The radius along the x-axis.</param>
    /// <param name="radiusY">The radius along the y-axis.</param>
    /// <param name="startAngle">The starting angle of the arc or sector.</param>
    /// <param name="sweepAngle">The angular distance covered by the arc or sector.</param>
    /// <param name="rotation">An optional rotation applied to the shape.</param>
    /// <returns>The created Ellipse.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Existing public parameter names support named-argument callers.")]
    public Ellipse EllipticalArc(double xCenter, double yCenter, double radiusX, double radiusY, Angle startAngle, Angle sweepAngle, Angle? rotation = null) =>
        EllipticalArc(new(xCenter, yCenter), radiusX, radiusY, startAngle, sweepAngle, rotation);

    /// <summary>Creates and adds an Ellipse to this plot.</summary>
    /// <param name="center">The center coordinate of the shape.</param>
    /// <param name="radiusX">The radius along the x-axis.</param>
    /// <param name="radiusY">The radius along the y-axis.</param>
    /// <param name="startAngle">The starting angle of the arc or sector.</param>
    /// <param name="sweepAngle">The angular distance covered by the arc or sector.</param>
    /// <param name="rotation">An optional rotation applied to the shape.</param>
    /// <returns>The created Ellipse.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Ellipse EllipticalSector(Coordinates center, double radiusX, double radiusY, Angle startAngle, Angle sweepAngle, Angle? rotation = null)
    {
        Color color = GetNextColor();

        Ellipse ellipticalSector = new()
        {
            Center = center,
            RadiusX = radiusX,
            RadiusY = radiusY,
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
    /// <param name="radiusX">The radius along the x-axis.</param>
    /// <param name="radiusY">The radius along the y-axis.</param>
    /// <param name="startAngle">The starting angle of the arc or sector.</param>
    /// <param name="sweepAngle">The angular distance covered by the arc or sector.</param>
    /// <param name="rotation">An optional rotation applied to the shape.</param>
    /// <returns>The created Ellipse.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Existing public parameter names support named-argument callers.")]
    public Ellipse EllipticalSector(double xCenter, double yCenter, double radiusX, double radiusY, Angle startAngle, Angle sweepAngle, Angle? rotation = null) =>
        EllipticalSector(new(xCenter, yCenter), radiusX, radiusY, startAngle, sweepAngle, rotation);

    /// <summary>Creates and adds an ErrorBar to this plot.</summary>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="yErrors">The y-axis error magnitudes.</param>
    /// <returns>The created ErrorBar.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1305", Justification = "Existing public parameter names support named-argument callers.")]
    public ErrorBar ErrorBar(IReadOnlyList<double> xs, IReadOnlyList<double> ys, IReadOnlyList<double> yErrors)
    {
        ErrorBar eb = new(xs, ys, null, null, yErrors, yErrors) { Color = GetNextColor(), };

        Plot.PlottableList.Add(eb);
        return eb;
    }

    /// <summary>Fill the vertical range between two Y points for each X point.</summary>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys1">The first sequence of y-coordinate values.</param>
    /// <param name="ys2">The second sequence of y-coordinate values.</param>
    /// <returns>The created FillY.</returns>
    public FillY FillY(double[] xs, double[] ys1, double[] ys2)
    {
        List<(double, double, double)> data = new();

        for (int i = 0; i < xs.Length; i++)
        {
            data.Add((xs[i], ys1[i], ys2[i]));
        }

        return FillY(data);
    }

    /// <summary>Fill the vertical range between two Y points for each X point.</summary>
    /// <param name="scatter1">The scatter1 to use.</param>
    /// <param name="scatter2">The scatter2 to use.</param>
    /// <returns>The created FillY.</returns>
    public FillY FillY(Scatter scatter1, Scatter scatter2)
    {
        FillY rangePlot = new(scatter1, scatter2);
        rangePlot.FillStyle.Color = GetNextColor();
        Plot.PlottableList.Add(rangePlot);
        return rangePlot;
    }

    /// <summary>Fill the vertical range between two Y points for each X point.</summary>
    /// <param name="data">The data points to plot.</param>
    /// <returns>The created FillY.</returns>
    public FillY FillY(ICollection<(double X, double Top, double Bottom)> data)
    {
        FillY rangePlot = new();
        rangePlot.FillStyle.Color = GetNextColor();
        rangePlot.SetDataSource(data);
        Plot.PlottableList.Add(rangePlot);
        return rangePlot;
    }

    /// <summary>
    /// Fill the vertical range between two Y points for each X point
    /// This overload uses a custom function to calculate X, Y1, and Y2 values.
    /// </summary>
    /// <typeparam name="T">The type of values accepted by this overload.</typeparam>
    /// <param name="data">The data points to plot.</param>
    /// <param name="function">The function used to map each input value.</param>
    /// <returns>The created FillY.</returns>
    public FillY FillY<T>(ICollection<T> data, Func<T, (double X, double Top, double Bottom)> function)
    {
        var rangePlot = new FillY();
        rangePlot.FillStyle.Color = GetNextColor();
        rangePlot.SetDataSource(data, function);
        Plot.PlottableList.Add(rangePlot);
        return rangePlot;
    }

    /// <summary>Creates and adds a FunctionPlot to this plot.</summary>
    /// <param name="functionSource">The source that evaluates the plotted function.</param>
    /// <returns>The created FunctionPlot.</returns>
    public FunctionPlot Function(IFunctionSource functionSource)
    {
        FunctionPlot functionPlot = new(functionSource);
        functionPlot.LineStyle.Color = GetNextColor();

        Plot.PlottableList.Add(functionPlot);
        return functionPlot;
    }

    /// <summary>Creates and adds a FunctionPlot to this plot.</summary>
    /// <param name="func">The func to use.</param>
    /// <returns>The created FunctionPlot.</returns>
    public FunctionPlot Function(Func<double, double> func)
    {
        var functionSource = new FunctionSource(func);
        return Function(functionSource);
    }

    /// <summary>Creates and adds a heatmap for the supplied intensity data.</summary>
    /// <param name="intensities">The intensity value at each grid position.</param>
    /// <returns>The created Heatmap.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("PerformanceSharp", "PSH1020", Justification = "Existing rectangular-array overloads are part of the public API.")]
    public Heatmap Heatmap(double[, ] intensities)
    {
        Heatmap heatmap = new(intensities);
        Plot.PlottableList.Add(heatmap);
        return heatmap;
    }

    /// <summary>Creates and adds a heatmap for the supplied intensity data.</summary>
    /// <param name="values">The values represented by the plot.</param>
    /// <returns>The created Heatmap.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("PerformanceSharp", "PSH1020", Justification = "Existing rectangular-array overloads are part of the public API.")]
    public Heatmap Heatmap(Coordinates3d[, ] values)
    {
        int height = values.GetLength(0);
        int width = values.GetLength(1);
        double[, ] intensities = new double[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                intensities[y, x] = values[y, x].Z;
            }
        }

        return Heatmap(intensities);
    }

    /// <summary>Creates and adds bars for the supplied histogram.</summary>
    /// <param name="histogram">The histogram data to display.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <param name="disableBottomPadding">true to remove bottom-axis padding; otherwise, false.</param>
    /// <returns>The created HistogramBars.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public HistogramBars Histogram(Histogram histogram, Color? color = null, bool disableBottomPadding = true)
    {
        HistogramBars hb = new(histogram);
        Plot.PlottableList.Add(hb);

        foreach (var bar in hb.Bars)
        {
            bar.LineWidth = 0;
            bar.FillStyle.AntiAlias = false;
            bar.FillColor = color ?? Palette.GetColor(0);
        }

        if (disableBottomPadding)
        {
            Plot.Axes.Margins(bottom: 0);
        }

        return hb;
    }

    /// <summary>Creates and adds a HorizontalLine to this plot.</summary>
    /// <param name="y">The y to use.</param>
    /// <param name="width">The width to use.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <param name="pattern">The pattern to use.</param>
    /// <returns>The created HorizontalLine.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public HorizontalLine HorizontalLine(double y, float width = 2, Color? color = null, LinePattern pattern = default)
    {
        Color color2 = color ?? GetNextColor();
        HorizontalLine line = new() { LineWidth = width, LineColor = color2, LabelBackgroundColor = color2, LabelFontColor = Colors.White, LinePattern = pattern, Y = y };
        Plot.PlottableList.Add(line);
        return line;
    }

    /// <summary>Creates and adds a HorizontalSpan to this plot.</summary>
    /// <param name="x1">The first x-coordinate.</param>
    /// <param name="x2">The second x-coordinate.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created HorizontalSpan.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public HorizontalSpan HorizontalSpan(double x1, double x2, Color? color = null)
    {
        HorizontalSpan span = new() { X1 = x1, X2 = x2 };
        span.FillStyle.Color = color ?? GetNextColor().WithAlpha(SpanFillAlpha);
        span.LineStyle.Color = span.FillStyle.Color.WithAlpha(SpanBorderAlpha);
        Plot.PlottableList.Add(span);
        return span;
    }

    /// <summary>Creates and adds an ImageMarker to this plot.</summary>
    /// <param name="location">The location to use.</param>
    /// <param name="image">The image to use.</param>
    /// <param name="scale">The scale to use.</param>
    /// <returns>The created ImageMarker.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public ImageMarker ImageMarker(Coordinates location, Image image, float scale = 1)
    {
        ImageMarker marker = new() { Location = location, Image = image, Scale = scale, };

        Plot.PlottableList.Add(marker);

        return marker;
    }

    /// <summary>Creates and adds an ImageRect to this plot.</summary>
    /// <param name="image">The image to use.</param>
    /// <param name="rect">The rect to use.</param>
    /// <returns>The created ImageRect.</returns>
    public ImageRect ImageRect(Image image, CoordinateRect rect)
    {
        ImageRect marker = new() { Image = image, Rect = rect, };

        Plot.PlottableList.Add(marker);

        return marker;
    }
}
