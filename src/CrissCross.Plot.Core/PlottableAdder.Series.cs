// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.DataSources;
using CrissCross.Plot.Plottables;
using Signal = CrissCross.Plot.Plottables.Signal;

namespace CrissCross.Plot;

/// <summary>Provides methods for creating and adding plottables to a plot.</summary>
public partial class PlottableAdder
{
    /// <summary>Creates and adds an OhlcPlot to this plot.</summary>
    /// <param name="ohlcs">The ohlcs to use.</param>
    /// <returns>The created OhlcPlot.</returns>
    public OhlcPlot OHLC(List<OHLC> ohlcs)
    {
        OHLCSourceList dataSource = new(ohlcs);
        OhlcPlot ohlc = new(dataSource);
        Plot.PlottableList.Add(ohlc);
        return ohlc;
    }

    /// <summary>Creates and adds a Phasor to this plot.</summary>
    /// <returns>The created Phasor.</returns>
    public Phasor Phasor()
    {
        Phasor phasor = new();

        Color color = GetNextColor().WithAlpha(PhasorAlpha);
        phasor.ArrowFillColor = color;
        phasor.ArrowLineColor = color;
        phasor.LabelStyle.ForeColor = phasor.ArrowFillColor;

        Plot.PlottableList.Add(phasor);
        return phasor;
    }

    /// <summary>Creates and adds a Phasor to this plot.</summary>
    /// <param name="points">The polar coordinates to add.</param>
    /// <returns>The created Phasor.</returns>
    public Phasor Phasor(IEnumerable<PolarCoordinates> points)
    {
        Phasor phasor = Phasor();
        phasor.Points.AddRange(points);
        return phasor;
    }

    /// <summary>Creates and adds a Pie to this plot.</summary>
    /// <param name="slices">The slices to use.</param>
    /// <returns>The created Pie.</returns>
    public Pie Pie(IList<PieSlice> slices)
    {
        Pie pie = new(slices);
        Plot.PlottableList.Add(pie);
        return pie;
    }

    /// <summary>Creates and adds a Pie to this plot.</summary>
    /// <param name="values">The values represented by the plot.</param>
    /// <returns>The created Pie.</returns>
    public Pie Pie(IEnumerable<double> values)
    {
        List<PieSlice> slices = new();
        foreach (double value in values)
        {
            PieSlice slice = new() { Value = value, FillColor = Palette.GetColor(slices.Count), };

            slices.Add(slice);
        }

        Pie pie = new(slices);
        Plot.PlottableList.Add(pie);
        return pie;
    }

    /// <summary>Adds the supplied plottable to this plot and returns the same instance.</summary>
    /// <param name="plottable">The plottable to use.</param>
    /// <returns>The same plottable instance that was added.</returns>
    public IPlottable Plottable(IPlottable plottable)
    {
        Plot.PlottableList.Add(plottable);
        return plottable;
    }

    /// <summary>Creates and adds a polar axis and hides the standard axes and grid.</summary>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="spokeLength">The spoke length to use.</param>
    /// <param name="circleCount">The circle count to use.</param>
    /// <param name="spokeCount">The spoke count to use.</param>
    /// <returns>The created PolarAxis.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public PolarAxis PolarAxis(double radius = 1.0, double spokeLength = 1.1, int circleCount = 5, int spokeCount = 12)
    {
        PolarAxis polarAxis = new();
        polarAxis.SetCircles(radius, circleCount);
        polarAxis.SetSpokes(spokeCount, radius * spokeLength);

        Plot.PlottableList.Add(polarAxis);
        Plot.HideAxesAndGrid();
        return polarAxis;
    }

    /// <summary>Creates and adds a Polygon to this plot.</summary>
    /// <param name="coordinates">The coordinates to plot.</param>
    /// <returns>The created Polygon.</returns>
    public Polygon Polygon(Coordinates[] coordinates)
    {
        Color color = GetNextColor();
        Polygon poly = new(coordinates) { LineColor = color, FillColor = color.WithAlpha(PlottableFillAlpha), };
        Plot.PlottableList.Add(poly);
        return poly;
    }

    /// <summary>Creates and adds a Polygon to this plot.</summary>
    /// <typeparam name="TX">The type of the x-coordinate values.</typeparam>
    /// <typeparam name="TY">The type of the y-coordinate values.</typeparam>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <returns>The created Polygon.</returns>
    public Polygon Polygon<TX, TY>(IEnumerable<TX> xs, IEnumerable<TY> ys)
    {
        Coordinates[] coordinates = NumericConversion.GenericToCoordinates(xs, ys);
        return Polygon(coordinates);
    }

    /// <summary>Creates and adds a PopulationSymbol to this plot.</summary>
    /// <param name="values">The values represented by the plot.</param>
    /// <param name="x">The x to use.</param>
    /// <returns>The created PopulationSymbol.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public PopulationSymbol Population(double[] values, double x = 0)
    {
        Color color = GetNextColor();
        Population pop = new(values);
        PopulationSymbol sym = new(pop);
        sym.X = x;
        sym.Bar.FillColor = color;
        sym.Box.FillColor = Colors.Black.WithLightness(PopulationBoxLightness);
        sym.Marker.MarkerLineColor = color;
        sym.Marker.MarkerFillColor = color;
        Plot.PlottableList.Add(sym);
        return sym;
    }

    /// <summary>Creates and adds a radar chart for the supplied data series.</summary>
    /// <returns>The created Radar.</returns>
    public Radar Radar()
    {
        Radar radar = new();

        Plot.PlottableList.Add(radar);

        Plot.HideAxesAndGrid();
        return radar;
    }

    /// <summary>Creates and adds a radar chart for the supplied data series.</summary>
    /// <param name="values">The values represented by the plot.</param>
    /// <returns>The created Radar.</returns>
    public Radar Radar(double[] values)
    {
        List<double[]> values2 = [values];

        return Radar(values2);
    }

    /// <summary>Creates and adds a radar chart for the supplied data series.</summary>
    /// <param name="values">The values represented by the plot.</param>
    /// <returns>The created Radar.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("PerformanceSharp", "PSH1020", Justification = "Existing rectangular-array overloads are part of the public API.")]
    public Radar Radar(double[, ] values)
    {
        List<double[]> valuesList = [];

        for (int i = 0; i < values.GetLength(0); i++)
        {
            double[] row = new double[values.GetLength(1)];
            for (int j = 0; j < row.Length; j++)
            {
                row[j] = values[i, j];
            }

            valuesList.Add(row);
        }

        return Radar(valuesList);
    }

    /// <summary>Creates and adds a radar chart for the supplied data series.</summary>
    /// <param name="series">The sequences of values to plot.</param>
    /// <returns>The created Radar.</returns>
    public Radar Radar(IEnumerable<IEnumerable<double>> series)
    {
        List<double[]> seriesValues = MaterializeRadarSeries(series);
        int spokeCount = GetRadarSpokeCount(seriesValues);
        Radar radar = new();
        double maxValue = AddRadarSeries(radar, seriesValues);
        radar.PolarAxis.SetCircles(maxValue, RadarCircleCount);
        radar.PolarAxis.SetSpokes(spokeCount, maxValue * RadarSpokeMargin, degreeLabels: false);
        Plot.PlottableList.Add(radar);
        Plot.HideAxesAndGrid();
        return radar;
    }

    /// <summary>Creates and adds a radial gauge plot for the supplied values.</summary>
    /// <param name="values">The values represented by the plot.</param>
    /// <returns>The created RadialGaugePlot.</returns>
    public RadialGaugePlot RadialGaugePlot(IEnumerable<double> values)
    {
        List<double> valuesList = [];
        valuesList.AddRange(values);

        Color[] colors = new Color[valuesList.Count];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Palette.GetColor(i);
        }

        RadialGaugePlot radialGaugePlot = new(valuesList.ToArray(), colors);
        Plot.PlottableList.Add(radialGaugePlot);
        Plot.HideGrid();
        Plot.Layout.Frameless();
        return radialGaugePlot;
    }

    /// <summary>Create a bar plot to represent a collection of named ranges.</summary>
    /// <param name="ranges">The ranges to use.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <param name="horizontal">true to orient the plot horizontally; otherwise, false.</param>
    /// <returns>The created BarPlot.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public BarPlot Ranges(List<(string name, CoordinateRange range)> ranges, Color? color = null, bool horizontal = false)
    {
        Color barColor = color ?? GetNextColor();

        // create a bar plot from the collection of ranges
        Bar[] bars = new Bar[ranges.Count];
        for (int i = 0; i < ranges.Count; i++)
        {
            bars[i] = new() { ValueBase = ranges[i].range.Min, Value = ranges[i].range.Max, Position = i, FillColor = barColor, };
        }

        BarPlot bp = Bars(bars);
        bp.Horizontal = horizontal;

        // use manaul tick labels displaying category names
        double[] positions = new double[bars.Length];
        string[] labels = new string[ranges.Count];
        for (int i = 0; i < ranges.Count; i++)
        {
            positions[i] = bars[i].Position;
            labels[i] = ranges[i].name;
        }

        if (horizontal)
        {
            Plot.Axes.Left.SetTicks(positions, labels);
        }
        else
        {
            Plot.Axes.Bottom.SetTicks(positions, labels);
        }

        return bp;
    }

    /// <summary>Creates and adds a Rectangle to this plot.</summary>
    /// <param name="rect">The rect to use.</param>
    /// <returns>The created Rectangle.</returns>
    public Rectangle Rectangle(CoordinateRect rect) => Rectangle(rect.Left, rect.Right, rect.Top, rect.Bottom);

    /// <summary>Creates and adds a Rectangle to this plot.</summary>
    /// <param name="left">The left to use.</param>
    /// <param name="right">The right to use.</param>
    /// <param name="bottom">The bottom to use.</param>
    /// <param name="top">The top to use.</param>
    /// <returns>The created Rectangle.</returns>
    public Rectangle Rectangle(double left, double right, double bottom, double top)
    {
        Color color = GetNextColor();
        Rectangle rp = new() { X1 = left, X2 = right, Y1 = bottom, Y2 = top, LineColor = color, FillColor = color.WithAlpha(PlottableFillAlpha), };

        Plot.PlottableList.Add(rp);
        return rp;
    }

    /// <summary>Creates and adds a ScaleBar to this plot.</summary>
    /// <param name="width">The width to use.</param>
    /// <param name="height">The height to use.</param>
    /// <returns>The created ScaleBar.</returns>
    public ScaleBar ScaleBar(double width, double height)
    {
        ScaleBar scalebar = new() { Width = width, Height = height, };

        Plot.PlottableList.Add(scalebar);
        return scalebar;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <param name="source">The source that provides plot data.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter Scatter(IScatterSource source, Color? color = null)
    {
        Color nextColor = color ?? GetNextColor();
        Scatter scatter = new(source) { LineColor = nextColor, MarkerFillColor = nextColor, MarkerLineColor = nextColor, };
        Plot.PlottableList.Add(scatter);
        return scatter;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <param name="x">The x to use.</param>
    /// <param name="y">The y to use.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter Scatter(double x, double y, Color? color = null)
    {
        double[] xs = { x };
        double[] ys = { y };
        ScatterSourceDoubleArray source = new(xs, ys);
        return Scatter(source, color);
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter Scatter(double[] xs, double[] ys, Color? color = null)
    {
        ScatterSourceDoubleArray source = new(xs, ys);
        return Scatter(source, color);
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <param name="point">The coordinate to plot.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter Scatter(Coordinates point, Color? color = null)
    {
        Coordinates[] coordinates = { point };
        ScatterSourceCoordinatesArray source = new(coordinates);
        return Scatter(source, color);
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <param name="coordinates">The coordinates to plot.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter Scatter(Coordinates[] coordinates, Color? color = null)
    {
        ScatterSourceCoordinatesArray source = new(coordinates);
        return Scatter(source, color);
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <param name="coordinates">The coordinates to plot.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter Scatter(List<Coordinates> coordinates, Color? color = null)
    {
        ScatterSourceCoordinatesList source = new(coordinates);
        return Scatter(source, color);
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <typeparam name="T1">The type of the x-coordinate values.</typeparam>
    /// <typeparam name="T2">The type of the y-coordinate values.</typeparam>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter Scatter<T1, T2>(T1[] xs, T2[] ys, Color? color = null)
    {
        Color nextColor = color ?? GetNextColor();
        ScatterSourceGenericArray<T1, T2> source = new(xs, ys);
        Scatter scatter = new(source);
        scatter.LineStyle.Color = nextColor;
        scatter.MarkerStyle.FillColor = nextColor;
        Plot.PlottableList.Add(scatter);
        return scatter;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <typeparam name="T1">The type of the x-coordinate values.</typeparam>
    /// <typeparam name="T2">The type of the y-coordinate values.</typeparam>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter Scatter<T1, T2>(List<T1> xs, List<T2> ys, Color? color = null)
    {
        Color nextColor = color ?? GetNextColor();
        ScatterSourceGenericList<T1, T2> source = new(xs, ys);
        Scatter scatter = new(source);
        scatter.LineStyle.Color = nextColor;
        scatter.MarkerStyle.FillColor = nextColor;
        Plot.PlottableList.Add(scatter);
        return scatter;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <param name="source">The source that provides plot data.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter ScatterLine(IScatterSource source, Color? color = null)
    {
        var scatter = Scatter(source, color);
        scatter.MarkerSize = 0;
        return scatter;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter ScatterLine(double[] xs, double[] ys, Color? color = null)
    {
        var scatter = Scatter(xs, ys, color);
        scatter.MarkerSize = 0;
        return scatter;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <param name="coordinates">The coordinates to plot.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter ScatterLine(Coordinates[] coordinates, Color? color = null)
    {
        var scatter = Scatter(coordinates, color);
        scatter.MarkerSize = 0;
        return scatter;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <param name="coordinates">The coordinates to plot.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter ScatterLine(List<Coordinates> coordinates, Color? color = null)
    {
        var scatter = Scatter(coordinates, color);
        scatter.MarkerSize = 0;
        return scatter;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <typeparam name="T1">The type of the x-coordinate values.</typeparam>
    /// <typeparam name="T2">The type of the y-coordinate values.</typeparam>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter ScatterLine<T1, T2>(T1[] xs, T2[] ys, Color? color = null)
    {
        var scatter = Scatter(xs, ys, color);
        scatter.MarkerSize = 0;
        return scatter;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <typeparam name="T1">The type of the x-coordinate values.</typeparam>
    /// <typeparam name="T2">The type of the y-coordinate values.</typeparam>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter ScatterLine<T1, T2>(List<T1> xs, List<T2> ys, Color? color = null)
    {
        var scatter = Scatter(xs, ys, color);
        scatter.MarkerSize = 0;
        return scatter;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <param name="source">The source that provides plot data.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter ScatterPoints(IScatterSource source, Color? color = null)
    {
        var scatter = Scatter(source, color);
        scatter.LineWidth = 0;
        return scatter;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter ScatterPoints(double[] xs, double[] ys, Color? color = null)
    {
        var scatter = Scatter(xs, ys, color);
        scatter.LineWidth = 0;
        return scatter;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <param name="coordinates">The coordinates to plot.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter ScatterPoints(Coordinates[] coordinates, Color? color = null)
    {
        var scatter = Scatter(coordinates, color);
        scatter.LineWidth = 0;
        return scatter;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <param name="coordinates">The coordinates to plot.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter ScatterPoints(List<Coordinates> coordinates, Color? color = null)
    {
        var scatter = Scatter(coordinates, color);
        scatter.LineWidth = 0;
        return scatter;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <typeparam name="T1">The type of the x-coordinate values.</typeparam>
    /// <typeparam name="T2">The type of the y-coordinate values.</typeparam>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter ScatterPoints<T1, T2>(T1[] xs, T2[] ys, Color? color = null)
    {
        var scatter = Scatter(xs, ys, color);
        scatter.LineWidth = 0;
        return scatter;
    }

    /// <summary>Creates and adds a Scatter to this plot.</summary>
    /// <typeparam name="T1">The type of the x-coordinate values.</typeparam>
    /// <typeparam name="T2">The type of the y-coordinate values.</typeparam>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Scatter.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Scatter ScatterPoints<T1, T2>(List<T1> xs, List<T2> ys, Color? color = null)
    {
        var scatter = Scatter(xs, ys, color);
        scatter.LineWidth = 0;
        return scatter;
    }

    /// <summary>Creates and adds a Signal to this plot.</summary>
    /// <param name="source">The source that provides plot data.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Signal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Signal Signal(ISignalSource source, Color? color = null)
    {
        Signal sig = new(source) { Color = color ?? GetNextColor() };

        Plot.PlottableList.Add(sig);

        return sig;
    }

    /// <summary>Creates and adds a Signal to this plot.</summary>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="period">The interval between samples.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Signal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Signal Signal(double[] ys, double period = 1, Color? color = null)
    {
        SignalSourceDouble source = new(ys, period);
        return Signal(source, color);
    }

    /// <summary>Creates and adds a Signal to this plot.</summary>
    /// <typeparam name="T">The type of values accepted by this overload.</typeparam>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="period">The interval between samples.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Signal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Signal Signal<T>(T[] ys, double period = 1, Color? color = null)
    {
        SignalSourceGenericArray<T> source = new(ys, period);
        return Signal(source, color);
    }

    /// <summary>Creates and adds a Signal to this plot.</summary>
    /// <typeparam name="T">The type of values accepted by this overload.</typeparam>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="period">The interval between samples.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Signal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Signal Signal<T>(IReadOnlyList<T> ys, double period = 1, Color? color = null)
    {
        SignalSourceGenericList<T> source = new(ys, period);
        return Signal(source, color);
    }

    /// <summary>Creates and adds a Signal to this plot.</summary>
    /// <typeparam name="T">The type of values accepted by this overload.</typeparam>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="period">The interval between samples.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created Signal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Signal SignalConst<T>(T[] ys, double period = 1, Color? color = null)
        where T : struct, IComparable
    {
        SignalConstSource<T> source = new(ys, period);
        return Signal(source, color);
    }

    /// <summary>Creates and adds a SignalXY to this plot.</summary>
    /// <param name="source">The source that provides plot data.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created SignalXY.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public SignalXY SignalXY(ISignalXYSource source, Color? color = null)
    {
        SignalXY sig = new(source) { Color = color ?? GetNextColor() };

        Plot.PlottableList.Add(sig);

        return sig;
    }

    /// <summary>Creates and adds a SignalXY to this plot.</summary>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created SignalXY.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public SignalXY SignalXY(double[] xs, double[] ys, Color? color = null)
    {
        SignalXYSourceDoubleArray source = new(xs, ys);
        return SignalXY(source, color);
    }

    /// <summary>Creates and adds a SignalXY to this plot.</summary>
    /// <typeparam name="TX">The type of the x-coordinate values.</typeparam>
    /// <typeparam name="TY">The type of the y-coordinate values.</typeparam>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created SignalXY.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public SignalXY SignalXY<TX, TY>(TX[] xs, TY[] ys, Color? color = null)
    {
        var source = new SignalXYSourceGenericArray<TX, TY>(xs, ys);
        return SignalXY(source, color);
    }

    /// <summary>Creates and adds a SignalXY to this plot.</summary>
    /// <typeparam name="TX">The type of the x-coordinate values.</typeparam>
    /// <typeparam name="TY">The type of the y-coordinate values.</typeparam>
    /// <param name="xs">The x-coordinate values.</param>
    /// <param name="ys">The y-coordinate values.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created SignalXY.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public SignalXY SignalXY<TX, TY>(IReadOnlyList<TX> xs, IReadOnlyList<TY> ys, Color? color = null)
    {
        SignalXYSourceGenericList<TX, TY> source = new(xs, ys);
        return SignalXY(source, color);
    }

    /// <summary>Creates and adds an axis for a Smith chart.</summary>
    /// <returns>The created SmithChartAxis.</returns>
    public SmithChartAxis SmithChartAxis()
    {
        SmithChartAxis smithChartAxis = new();

        foreach (var position in (double[])[SmithChartThirty, SmithChartFive, SmithChartTwo, SmithChartOne, SmithChartHalf, SmithChartFifth, SmithChartZero])
        {
            var realPart = smithChartAxis.AddRealTick(position);
            if (position is SmithChartOne or SmithChartZero)
            {
                realPart.LineStyle.Color = Colors.Black;
            }
        }

        double[] imaginaryPositions =
        [
            SmithChartThirty,
            SmithChartFive,
            SmithChartTwo,
            SmithChartOne,
            SmithChartHalf,
            SmithChartFifth,
            SmithChartZero,
            -SmithChartFifth,
            -SmithChartHalf,
            -SmithChartOne,
            -SmithChartTwo,
            -SmithChartFive,
            -SmithChartThirty
        ];
        foreach (double position in imaginaryPositions)
        {
            var imaginaryPart = smithChartAxis.AddImaginaryTick(position);
            if (position == 0)
            {
                imaginaryPart.LineStyle.Color = Colors.Black;
            }
        }

        Plot.PlottableList.Add(smithChartAxis);

        Plot.HideAxesAndGrid();
        return smithChartAxis;
    }

    /// <summary>Place a stacked bar chart at a single position.</summary>
    /// <param name="ranges">The ranges to use.</param>
    /// <param name="palette">The optional palette used to color the series.</param>
    /// <param name="horizontal">true to orient the plot horizontally; otherwise, false.</param>
    /// <returns>The created BarPlot[].</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public BarPlot[] StackedRanges(List<(string name, double[] edgeValues)> ranges, IPalette? palette = null, bool horizontal = false)
    {
        BarPlot[] bps = new BarPlot[ranges.Count];
        for (int i = 0; i < ranges.Count; i++)
        {
            double[] edgeValues = ranges[i].edgeValues;
            Bar[] bars = new Bar[edgeValues.Length - 1];
            for (int j = 0; j < bars.Length; j++)
            {
                bars[j] = new() { ValueBase = edgeValues[j], Value = edgeValues[j + 1], Position = i, FillColor = (palette ?? Palette).GetColor(j), };
            }

            bps[i] = Bars(bars);
            bps[i].Horizontal = horizontal;
        }

        string[] labels = new string[ranges.Count];
        for (int i = 0; i < ranges.Count; i++)
        {
            labels[i] = ranges[i].name;
        }

        double[] positions = Generate.Consecutive(labels.Length);
        if (horizontal)
        {
            Plot.Axes.Left.SetTicks(positions, labels);
        }
        else
        {
            Plot.Axes.Bottom.SetTicks(positions, labels);
        }

        return bps;
    }

    /// <summary>Creates and adds a text label at the supplied plot coordinates.</summary>
    /// <param name="text">The text content to display.</param>
    /// <param name="location">The location to use.</param>
    /// <returns>The created Text.</returns>
    public Text Text(string text, Coordinates location) => Text(text, location.X, location.Y);

    /// <summary>Creates and adds a text label at the supplied plot coordinates.</summary>
    /// <param name="text">The text content to display.</param>
    /// <param name="x">The x to use.</param>
    /// <param name="y">The y to use.</param>
    /// <returns>The created Text.</returns>
    public Text Text(string text, double x, double y)
    {
        Text txt = new() { LabelText = text ?? string.Empty, LabelBackgroundColor = Colors.Transparent, LabelBorderColor = Colors.Transparent, Location = new(x, y) };
        Plot.PlottableList.Add(txt);

        return txt;
    }

    /// <summary>Creates and adds a Tooltip to this plot.</summary>
    /// <param name="tipLocation">The location of the tooltip tip.</param>
    /// <param name="text">The text content to display.</param>
    /// <param name="labelLocation">The location of the label.</param>
    /// <returns>The created Tooltip.</returns>
    public Tooltip Tooltip(Coordinates tipLocation, string text, Coordinates labelLocation)
    {
        Tooltip tooltip = new()
        {
            LabelText = text ?? string.Empty,
            FillColor = Colors.White,
            LineColor = Colors.Black,
            LabelFontColor = Colors.Black,
            LabelLocation = labelLocation,
            TipLocation = tipLocation,
            LabelAlignment = Alignment.MiddleCenter,
        };

        Plot.PlottableList.Add(tooltip);
        return tooltip;
    }

    /// <summary>Creates and adds a Tooltip to this plot.</summary>
    /// <param name="tipX">The x-coordinate of the tooltip tip.</param>
    /// <param name="tipY">The y-coordinate of the tooltip tip.</param>
    /// <param name="text">The text content to display.</param>
    /// <param name="labelX">The x-coordinate of the label.</param>
    /// <param name="labelY">The y-coordinate of the label.</param>
    /// <returns>The created Tooltip.</returns>
    public Tooltip Tooltip(double tipX, double tipY, string text, double labelX, double labelY)
    {
        Coordinates labelLocation = new(labelX, labelY);
        Coordinates tipLocation = new(tipX, tipY);
        return Tooltip(tipLocation, text, labelLocation);
    }

    /// <summary>Creates and adds a triangular axis with the requested orientation and layout.</summary>
    /// <param name="clockwise">true to orient the axis clockwise; otherwise, false.</param>
    /// <param name="hideAxisAndGrid">true to hide the standard axes and grid; otherwise, false.</param>
    /// <param name="useSquareAxisUnits">true to use square axis units; otherwise, false.</param>
    /// <returns>The created TriangularAxis.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public TriangularAxis TriangularAxis(bool clockwise = true, bool hideAxisAndGrid = true, bool useSquareAxisUnits = true)
    {
        TriangularAxis ta = new(clockwise);
        Plot.PlottableList.Add(ta);

        if (hideAxisAndGrid)
        {
            Plot.HideAxesAndGrid();
        }

        if (useSquareAxisUnits)
        {
            Plot.Axes.SquareUnits();
        }

        return ta;
    }

    /// <summary>Creates and adds a VectorField to this plot.</summary>
    /// <param name="vectors">The vectors to use.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created VectorField.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public VectorField VectorField(IList<RootedCoordinateVector> vectors, Color? color = null)
    {
        VectorFieldDataSourceCoordinatesList vs = new(vectors);
        VectorField field = new(vs);
        field.ArrowStyle.LineStyle.Color = color ?? GetNextColor();
        field.ArrowStyle.LineStyle.Width = VectorFieldLineWidth;
        Plot.PlottableList.Add(field);
        return field;
    }

    /// <summary>Creates and adds a VerticalLine to this plot.</summary>
    /// <param name="x">The x to use.</param>
    /// <param name="width">The width to use.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <param name="pattern">The pattern to use.</param>
    /// <returns>The created VerticalLine.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public VerticalLine VerticalLine(double x, float width = 2, Color? color = null, LinePattern pattern = default)
    {
        Color color2 = color ?? GetNextColor();
        VerticalLine line = new() { LineWidth = width, LineColor = color2, LabelBackgroundColor = color2, LinePattern = pattern, X = x };
        Plot.PlottableList.Add(line);
        return line;
    }

    /// <summary>Creates and adds a VerticalSpan to this plot.</summary>
    /// <param name="y1">The first y-coordinate.</param>
    /// <param name="y2">The second y-coordinate.</param>
    /// <param name="color">An optional color override; when omitted, the next palette color is used.</param>
    /// <returns>The created VerticalSpan.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public VerticalSpan VerticalSpan(double y1, double y2, Color? color = null)
    {
        VerticalSpan span = new() { Y1 = y1, Y2 = y2 };
        span.FillStyle.Color = color ?? GetNextColor().WithAlpha(SpanFillAlpha);
        span.LineStyle.Color = span.FillStyle.Color.WithAlpha(SpanBorderAlpha);
        Plot.PlottableList.Add(span);
        return span;
    }

    /// <summary>Materializes each radar series into an array.</summary>
    /// <param name="series">The radar series to materialize.</param>
    /// <returns>The values of each series.</returns>
    private static List<double[]> MaterializeRadarSeries(IEnumerable<IEnumerable<double>> series)
    {
        List<double[]> seriesValues = [];
        foreach (IEnumerable<double> values in series)
        {
            List<double> valuesList = [];
            valuesList.AddRange(values);
            seriesValues.Add(valuesList.ToArray());
        }

        return seriesValues;
    }

    /// <summary>Validates the radar-series lengths and returns the spoke count.</summary>
    /// <param name="seriesValues">The materialized radar series.</param>
    /// <returns>The number of values in every series.</returns>
    private static int GetRadarSpokeCount(List<double[]> seriesValues)
    {
        if (seriesValues.Count == 0)
        {
            throw new InvalidOperationException("Sequence contains no elements");
        }

        int spokeCount = seriesValues[0].Length;
        foreach (double[] values in seriesValues)
        {
            if (values.Length != spokeCount)
            {
                throw new InvalidOperationException("Every collection in the series must have the same number of items");
            }

            if (values.Length == 0)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }
        }

        return spokeCount;
    }

    /// <summary>Adds radar-series plottables and finds the greatest value.</summary>
    /// <param name="radar">The radar plot to populate.</param>
    /// <param name="seriesValues">The radar series values.</param>
    /// <returns>The maximum value across all series.</returns>
    private double AddRadarSeries(Radar radar, List<double[]> seriesValues)
    {
        double maxValue = double.NegativeInfinity;
        for (int seriesIndex = 0; seriesIndex < seriesValues.Count; seriesIndex++)
        {
            double[] values = seriesValues[seriesIndex];
            Color color = Palette.GetColor(seriesIndex);
            RadarSeries radarSeries = new() { Values = values, FillColor = color.WithOpacity(), LineColor = color.WithOpacity(1), };
            radar.Series.Add(radarSeries);
            foreach (double value in values)
            {
                if (double.IsNaN(value))
                {
                    maxValue = double.NaN;
                }
                else if (!double.IsNaN(maxValue) && value > maxValue)
                {
                    maxValue = value;
                }
            }
        }

        return maxValue;
    }
}
