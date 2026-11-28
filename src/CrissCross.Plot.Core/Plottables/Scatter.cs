// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Diagnostics.CodeAnalysis;
using CrissCross.Plot.DataSources;

namespace CrissCross.Plot.Plottables;

/// <summary>Plots a collection of points with optional lines, markers, and area fills.</summary>
/// <param name="data">The source of the scatter plot data.</param>
public class Scatter(IScatterSource data) : IPlottable, IHasLine, IHasMarker, IHasLegendText, IDataSource, IGetNearest
{
    /// <summary>Controls whether this scatter plot connects points with lines.</summary>
    [SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Changing this public field to a property would break API compatibility.")]
    public ConnectStyle ConnectStyle;

    /// <summary>The default marker size in pixels.</summary>
    private const float DefaultMarkerSize = 5;

    /// <summary>The default fill opacity.</summary>
    private const double DefaultFillAlpha = 0.2;

    /// <summary>The number of output pixels generated for each input point in a step path.</summary>
    private const int PointsPerStep = 2;

    /// <summary>The data source when it also implements <see cref="IDataSource"/>.</summary>
    private readonly IDataSource? _dataSource = data as IDataSource;

    /// <summary>Gets or sets the legacy legend label.</summary>
    [Obsolete("use LegendText")]
    [SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility; callers should use LegendText.")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text displayed for this plot in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets whether this plot is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to transform plot coordinates.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the style of lines connecting the points.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the width of connecting lines.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the pattern of connecting lines.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the color of connecting lines.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the first data index to render.</summary>
    public int MinRenderIndex { get => Data.MinRenderIndex; set => Data.MinRenderIndex = value; }

    /// <summary>Gets or sets the last data index to render.</summary>
    public int MaxRenderIndex { get => Data.MaxRenderIndex; set => Data.MaxRenderIndex = value; }

    /// <summary>Gets or sets the marker style.</summary>
    public MarkerStyle MarkerStyle { get; set; } = new() { LineWidth = 1, Size = DefaultMarkerSize, Shape = MarkerShape.FilledCircle };

    /// <summary>Gets or sets the marker shape.</summary>
    public MarkerShape MarkerShape { get => MarkerStyle.Shape; set => MarkerStyle.Shape = value; }

    /// <summary>Gets or sets the marker size.</summary>
    public float MarkerSize { get => MarkerStyle.Size; set => MarkerStyle.Size = value; }

    /// <summary>Gets or sets the marker fill color.</summary>
    public Color MarkerFillColor { get => MarkerStyle.FillColor; set => MarkerStyle.FillColor = value; }

    /// <summary>Gets or sets the marker outline color.</summary>
    public Color MarkerLineColor { get => MarkerStyle.LineColor; set => MarkerStyle.LineColor = value; }

    /// <summary>Gets or sets the marker color.</summary>
    public Color MarkerColor { get => MarkerStyle.MarkerColor; set => MarkerStyle.MarkerColor = value; }

    /// <summary>Gets or sets the width of the marker outline.</summary>
    public float MarkerLineWidth { get => MarkerStyle.LineWidth; set => MarkerStyle.LineWidth = value; }

    /// <summary>Gets the scatter data source.</summary>
    public IScatterSource Data { get; } = data;

    /// <summary>Gets the legend item for this scatter plot.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, MarkerStyle, LineStyle);

    /// <summary>Gets or sets whether to fill the area between the points and <see cref="FillYValue"/>.</summary>
    public bool FillY { get; set; }

    /// <summary>Gets or sets whether the area below <see cref="FillYValue"/> is filled.</summary>
    public bool FillYBelow { get; set; } = true;

    /// <summary>Gets or sets whether the area above <see cref="FillYValue"/> is filled.</summary>
    public bool FillYAbove { get; set; } = true;

    /// <summary>Gets or sets the Y value that bounds the fill area.</summary>
    public double FillYValue { get; set; }

    /// <summary>Gets or sets the color used to fill the area above <see cref="FillYValue"/>.</summary>
    public Color FillYAboveColor { get; set; } = Colors.Blue.WithAlpha(DefaultFillAlpha);

    /// <summary>Gets or sets the color used to fill the area below <see cref="FillYValue"/>.</summary>
    public Color FillYBelowColor { get; set; } = Colors.Blue.WithAlpha(DefaultFillAlpha);

    /// <summary>Gets or sets the fill color for both sides of <see cref="FillYValue"/>.</summary>
    public Color FillYColor
    {
        get => FillYAboveColor;
        set
        {
            FillYAboveColor = value;
            FillYBelowColor = value;
        }
    }

    /// <summary>Gets the gradient color positions used when filling the area.</summary>
    public List<AxisGradientColorPosition> ColorPositions { get; } = [];

    /// <summary>Gets or sets the direction of the fill gradient.</summary>
    public AxisGradientDirection AxisGradientDirection { get; set; } = AxisGradientDirection.Horizontal;

    /// <summary>Gets or sets the horizontal coordinate offset.</summary>
    public double OffsetX { get; set; }

    /// <summary>Gets or sets the vertical coordinate offset.</summary>
    public double OffsetY { get; set; }

    /// <summary>Gets or sets the horizontal coordinate scale.</summary>
    public double ScaleX { get; set; } = 1;

    /// <summary>Gets or sets the vertical coordinate scale.</summary>
    public double ScaleY { get; set; } = 1;

    /// <summary>Gets or sets whether points are connected with smooth curves.</summary>
    public bool Smooth
    {
        get => PathStrategy is PathStrategies.CubicSpline;
        set => PathStrategy = value
            ? new PathStrategies.CubicSpline()
            : new PathStrategies.Straight();
    }

    /// <summary>
    /// Gets or sets the curve tension, enabling <see cref="Smooth"/> when set.
    /// Low tensions tend to overshoot data points; high tensions approach straight lines.
    /// </summary>
    public double SmoothTension
    {
        get => PathStrategy is PathStrategies.CubicSpline cs ? cs.Tension : 0;
        set => PathStrategy = new PathStrategies.CubicSpline() { Tension = value };
    }

    /// <summary>Gets or sets the strategy used to generate the path connecting points.</summary>
    public IPathStrategy PathStrategy { get; set; } = new PathStrategies.Straight();

    /// <summary>Gets or sets the color used for the line and markers.</summary>
    public Color Color
    {
        get => LineStyle.Color;
        set
        {
            LineStyle.Color = value;
            MarkerStyle.FillColor = value;
            MarkerStyle.LineColor = value;
        }
    }

    /// <summary>Gets stepped display pixels for the supplied scatter points.</summary>
    /// <param name="pixels">The original scatter point pixels.</param>
    /// <param name="right">Whether each step extends right before rising or falling.</param>
    /// <returns>The pixels that form the stepped path.</returns>
    public static IReadOnlyList<Pixel> GetStepDisplayPixels(IReadOnlyList<Pixel> pixels, bool right)
    {
        int offsetX = right ? 1 : 0;
        int offsetY = right ? 0 : 1;
        return pixels.GrowView(PointsPerStep, (step, originalIndex, originalList) =>
            step == 0 || originalIndex >= originalList.Count - 1
                ? originalList[originalIndex]
                : new Pixel(
                    originalList[originalIndex + offsetX].X,
                    originalList[originalIndex + offsetY].Y))
            .TakeView((pixels.Count * PointsPerStep) - 1);
    }

    /// <summary>Calculates the axis limits occupied by this plot.</summary>
    /// <returns>The axis limits after scaling and offset are applied.</returns>
    public AxisLimits GetAxisLimits()
    {
        ExpandingAxisLimits limits = new(Data.GetLimits());

        if (FillY)
        {
            limits.ExpandY(FillYValue);
        }

        return new(
            left: (limits.Left * ScaleX) + OffsetX,
            right: (limits.Right * ScaleX) + OffsetX,
            bottom: (limits.Bottom * ScaleY) + OffsetY,
            top: (limits.Top * ScaleY) + OffsetY);
    }

    /// <summary>Gets the data point nearest to the specified location.</summary>
    /// <param name="mouseLocation">The location to search for, in plot coordinates.</param>
    /// <param name="renderInfo">The render details used to map coordinates.</param>
    /// <param name="maxDistance">The maximum distance at which a point is considered.</param>
    /// <returns>The nearest data point, or the default point when none is within range.</returns>
    public DataPoint GetNearest(Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance = 15) =>
        Data is IDataSource ds && ds.IsSorted()
            ? DataSourceUtilities.GetNearest(this, mouseLocation, renderInfo, maxDistance, Axes.XAxis, Axes.YAxis)
            : DataSourceUtilities.GetNearest(GetIDataSource(), mouseLocation, renderInfo, maxDistance, Axes.XAxis, Axes.YAxis);

    /// <summary>Gets the data point with the nearest X coordinate to the specified location.</summary>
    /// <param name="mouseLocation">The location to search for, in plot coordinates.</param>
    /// <param name="renderInfo">The render details used to map coordinates.</param>
    /// <param name="maxDistance">The maximum distance at which a point is considered.</param>
    /// <returns>The nearest data point by X coordinate, or the default point when none is within range.</returns>
    public DataPoint GetNearestX(Coordinates mouseLocation, RenderDetails renderInfo, float maxDistance = 15) =>
        Data is IDataSource ds && ds.IsSorted()
            ? DataSourceUtilities.GetNearestXFast(this, mouseLocation, renderInfo, maxDistance, Axes.XAxis)
            : DataSourceUtilities.GetNearestX(GetIDataSource(), mouseLocation, renderInfo, maxDistance, Axes.XAxis);

    /// <summary>Gets an optimized data source for iterating over this plot's points.</summary>
    /// <returns>The scatter data source, or a coordinate data source that applies the plot's scaling and offsets.</returns>
    public IDataSource GetIDataSource() => Data is IDataSource
        ? this
        : new CoordinateDataSource(Data.GetScatterPoints()) { XOffset = OffsetX, XScale = ScaleX, YOffset = OffsetY, YScale = ScaleY };

    /// <summary>Renders the scatter points, connecting lines, markers, and optional fill.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        var coordinates = Data.GetScatterPoints();
        IReadOnlyList<Pixel> markerPixels = coordinates.SelectView(coordinate =>
            Axes.GetPixel(new((coordinate.X * ScaleX) + OffsetX, (coordinate.Y * ScaleY) + OffsetY)));

        if (markerPixels.Count == 0)
        {
            return;
        }

        IReadOnlyList<Pixel> linePixels = ConnectStyle switch
        {
            ConnectStyle.Straight => markerPixels,
            ConnectStyle.StepHorizontal => GetStepDisplayPixels(markerPixels, true),
            ConnectStyle.StepVertical => GetStepDisplayPixels(markerPixels, false),
            _ => throw new ArgumentOutOfRangeException(nameof(ConnectStyle), ConnectStyle, "Unsupported connection style."),
        };

        using SKPath path = PathStrategy.GetPath(linePixels);

        if (FillY)
        {
            RenderFill(rp, path, markerPixels, linePixels);
        }

        Drawing.DrawLines(rp.Canvas, rp.Paint, path, LineStyle);
        Drawing.DrawMarkers(rp.Canvas, rp.Paint, markerPixels, MarkerStyle);
    }

    /// <summary>Clips and renders a section of the scatter fill.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="fillPath">The path defining the fill area.</param>
    /// <param name="fillStyle">The fill style to use.</param>
    /// <param name="clipRect">The rectangle used to clip the fill.</param>
    /// <param name="color">The fill color.</param>
    private static void RenderFillSection(RenderPack rp, SKPath fillPath, FillStyle fillStyle, PixelRect clipRect, Color color)
    {
        rp.CanvasState.Save();
        rp.CanvasState.Clip(clipRect);
        fillStyle.Color = color;
        Drawing.FillPath(rp.Canvas, rp.Paint, fillPath, fillStyle, clipRect);
        rp.CanvasState.Restore();
    }

    /// <summary>Renders the optional fill beneath the scatter plot.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="path">The scatter line path.</param>
    /// <param name="markerPixels">The marker positions in pixels.</param>
    /// <param name="linePixels">The line positions in pixels.</param>
    private void RenderFill(RenderPack rp, SKPath path, IReadOnlyList<Pixel> markerPixels, IReadOnlyList<Pixel> linePixels)
    {
        FillStyle fillStyle = new() { IsVisible = true };

        if (ColorPositions.Count > 0)
        {
            fillStyle.Hatch = Gradient.FromAxisLimits(rp, Data.GetLimits(), AxisGradientDirection, Axes, ColorPositions);
        }

        PixelRect dataPixelRect = new(markerPixels);
        PixelRect linePixelRect = new(linePixels);
        float fillYValuePixel = Axes.YAxis.GetPixel(FillYValue + OffsetY, rp.DataRect);

        using SKPathBuilder builder = new(path);
        builder.LineTo(linePixelRect.Right, fillYValuePixel);
        builder.LineTo(linePixelRect.Left, fillYValuePixel);
        using SKPath fillPath = builder.Detach();

        if (AxisGradientDirection == AxisGradientDirection.Horizontal)
        {
            RenderHorizontalFill(rp, fillPath, fillStyle, dataPixelRect, linePixelRect, fillYValuePixel);
        }
        else if (AxisGradientDirection == AxisGradientDirection.Vertical)
        {
            RenderVerticalFill(rp, fillPath, fillStyle, linePixelRect);
        }
    }

    /// <summary>Renders a horizontal gradient or solid fill section.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="fillPath">The path defining the fill area.</param>
    /// <param name="fillStyle">The fill style to use.</param>
    /// <param name="dataPixelRect">The data bounds in pixels.</param>
    /// <param name="linePixelRect">The scatter line bounds in pixels.</param>
    /// <param name="fillYValuePixel">The pixel coordinate of the fill baseline.</param>
    private void RenderHorizontalFill(RenderPack rp, SKPath fillPath, FillStyle fillStyle, PixelRect dataPixelRect, PixelRect linePixelRect, float fillYValuePixel)
    {
        bool fillBothSides = fillYValuePixel < dataPixelRect.Bottom && fillYValuePixel > dataPixelRect.Top;
        bool fillBelowOnly = fillYValuePixel <= dataPixelRect.Top;
        bool fillAboveOnly = fillYValuePixel >= dataPixelRect.Bottom;

        if (fillBothSides || fillAboveOnly)
        {
            PixelRect aboveRect = new(rp.DataRect.Left, rp.DataRect.Right, fillYValuePixel, linePixelRect.Top);
            RenderFillSection(rp, fillPath, fillStyle, aboveRect, ColorPositions.Count > 0 ? Colors.Black : FillYAboveColor);
        }

        if (!fillBothSides && !fillBelowOnly)
        {
            return;
        }

        PixelRect belowRect = new(rp.DataRect.Left, rp.DataRect.Right, linePixelRect.Bottom, fillYValuePixel);
        RenderFillSection(rp, fillPath, fillStyle, belowRect, ColorPositions.Count > 0 ? Colors.Black : FillYBelowColor);
    }

    /// <summary>Renders a vertical gradient or solid fill section.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="fillPath">The path defining the fill area.</param>
    /// <param name="fillStyle">The fill style to use.</param>
    /// <param name="linePixelRect">The scatter line bounds in pixels.</param>
    private void RenderVerticalFill(RenderPack rp, SKPath fillPath, FillStyle fillStyle, PixelRect linePixelRect)
    {
        PixelRect fullRect = new(rp.DataRect.Left, rp.DataRect.Right, linePixelRect.Bottom, linePixelRect.Top);
        RenderFillSection(rp, fillPath, fillStyle, fullRect, ColorPositions.Count > 0 ? Colors.Black : FillYColor);
    }

    bool IDataSource.IsSorted() => _dataSource?.IsSorted() ?? false;

    bool IDataSource.PreferCoordinates => _dataSource?.PreferCoordinates ?? true;

    int IDataSource.Length => _dataSource?.Length ?? Data.GetScatterPoints().Count;

    int IDataSource.MinRenderIndex => Data.MinRenderIndex;

    int IDataSource.MaxRenderIndex => Data.MaxRenderIndex;

    int IDataSource.GetXClosestIndex(Coordinates mouseLocation)
    {
        if (OffsetX is 0 && ScaleX is 1 && Data is IDataSource ds)
        {
            return ds.GetXClosestIndex(mouseLocation);
        }

        var points = Data.GetScatterPoints();
        if (points.IsAscending(BinarySearchComparer.Instance))
        {
            return points.GetClosestIndex(
                DataSourceUtilities.UnScaleCoordinate(mouseLocation, ScaleX, OffsetX, ScaleY, OffsetY),
                new(0, points.Count - 1),
                BinarySearchComparer.Instance);
        }

        int closestIndex = -1;
        double closestDistance = double.PositiveInfinity;
        mouseLocation = DataSourceUtilities.UnScaleCoordinate(mouseLocation, ScaleX, OffsetX, ScaleY, OffsetY);

        for (int i = 0; i < points.Count; i++)
        {
            double dx = mouseLocation.X - points[i].X;

            if (dx < closestDistance)
            {
                if (dx is 0)
                {
                    return i;
                }

                closestDistance = dx;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    Coordinates IDataSource.GetCoordinate(int index) => _dataSource?.GetCoordinate(index) ?? Data.GetScatterPoints()[index];

    Coordinates IDataSource.GetCoordinateScaled(int index)
    {
        IDataSource dataSource = this;
        return dataSource.PreferCoordinates
            ? DataSourceUtilities.ScaleCoordinate(dataSource.GetCoordinate(index), ScaleX, OffsetX, ScaleY, OffsetY)
            : new Coordinates() { X = dataSource.GetXScaled(index), Y = dataSource.GetYScaled(index) };
    }

    double IDataSource.GetX(int index) => _dataSource?.GetX(index) ?? Data.GetScatterPoints()[index].X;

    double IDataSource.GetXScaled(int index) => DataSourceUtilities.ScaleXY(
        value: _dataSource?.GetX(index) ?? Data.GetScatterPoints()[index].X,
        scalingFactor: ScaleX,
        offset: OffsetX);

    double IDataSource.GetY(int index) => _dataSource?.GetY(index) ?? Data.GetScatterPoints()[index].Y;

    double IDataSource.GetYScaled(int index) => DataSourceUtilities.ScaleXY(
        value: _dataSource?.GetY(index) ?? Data.GetScatterPoints()[index].Y,
        scalingFactor: ScaleY,
        offset: OffsetY);
}
