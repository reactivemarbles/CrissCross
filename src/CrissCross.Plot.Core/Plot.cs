// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.Grids;
using CrissCross.Plot.Rendering;
using CrissCross.Plot.Stylers;

namespace CrissCross.Plot;

/// <summary>Owns the plottables, axes, layout, and rendering state for a figure.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST2300",
    Justification = "Adding a protected Dispose(bool) member could conflict with existing derived types; preserve the established IDisposable contract.")]
public class Plot : IDisposable
{
    /// <summary>Style for the background of the entire figure.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp.Maintainability",
        "SST1401",
        Justification = "This public field is part of the established API; changing it to a property would be binary-breaking.")]
    public BackgroundStyle FigureBackground = new() { Color = Colors.White };

    /// <summary>Style for the data area within the axis frames.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp.Maintainability",
        "SST1401",
        Justification = "This public field is part of the established API; changing it to a property would be binary-breaking.")]
    public BackgroundStyle DataBackground = new() { Color = Colors.Transparent };

    /// <summary>Initializes a new instance of the <see cref="Plot"/> class.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2403",
        Justification = "The owned managers retain this instance during initialization but are not published to callers until construction completes.")]
    public Plot()
    {
        Axes = new(this);
        Add = new(this);
        Font = new(this);
        RenderManager = new(this);
        Legend = new(this);
        Layout = new(this);
        ZoomRectangle = new Plottables.ZoomRectangle(this);
    }

    /// <summary>Gets the plottables in their rendering order.</summary>
    public List<IPlottable> PlottableList { get; } = [];

    /// <summary>Gets the helper used to add plottables to this plot.</summary>
    public PlottableAdder Add { get; }

    /// <summary>Gets the manager responsible for rendering this plot.</summary>
    public RenderManager RenderManager { get; }

    /// <summary>Gets the layout and render details from the most recent render.</summary>
    public RenderDetails LastRender => RenderManager.LastRender;

    /// <summary>Gets the layout manager for this plot.</summary>
    public LayoutManager Layout { get; }

    /// <summary>Gets or sets the border style drawn around the figure.</summary>
    public LineStyle FigureBorder { get; set; } = LineStyle.None;

    /// <summary>Gets or sets the border style drawn around the data area.</summary>
    public LineStyle DataBorder { get; set; } = LineStyle.None;

    /// <summary>Gets or sets the interactive zoom rectangle used by this plot.</summary>
    public IZoomRectangle ZoomRectangle { get; set; }

    /// <summary>Gets or sets the scale applied when converting between screen pixels and plot pixels.</summary>
    public double ScaleFactor { get => ScaleFactorF; set => ScaleFactorF = (float)value; }

    /// <summary>Gets the axes and panels associated with this plot.</summary>
    public AxisManager Axes { get; }

    /// <summary>Gets the obsolete plot styler.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310",
        Justification = "This obsolete member is retained to preserve the published public API.")]
    [Obsolete("This class is deprecated. Use SetStyle() and GetStyle(). See the ScottPlot Cookbook for details.", true)]
    public PlotStyler Style { get; } = new();

    /// <summary>Gets the font styler for this plot.</summary>
    public FontStyler Font { get; }

    /// <summary>Gets or sets the legend for this plot.</summary>
    public Legend Legend { get; set; }

    /// <summary>Gets the default grid for this plot.</summary>
    public DefaultGrid Grid => Axes.DefaultGrid;

    /// <summary>Gets or sets the benchmark plottable used to measure rendering performance.</summary>
    public IPlottable Benchmark { get; set; } = new Plottables.Benchmark();

    /// <summary>Gets the synchronization object used by rendering operations.</summary>
    /// <remarks>Code that edits plot state can lock this object to avoid rendering inconsistent state.</remarks>
    public object Sync { get; } = new();

    /// <summary>Gets or sets the interactive control associated with this plot, if any.</summary>
    public IPlotControl? PlotControl { get; set; }

    /// <summary>Gets or sets the callback raised when the hovered interactive handle changes.</summary>
    public EventHandler<InteractiveHandle?>? HandleHoverChanged { get; set; }

    /// <summary>Gets or sets the callback raised when an interactive handle is pressed.</summary>
    public EventHandler<InteractiveHandle>? HandlePressed { get; set; }

    /// <summary>Gets or sets the callback raised when an interactive handle is moved.</summary>
    public EventHandler<InteractiveHandle>? HandleMoved { get; set; }

    /// <summary>Gets or sets the callback raised when an interactive handle is released.</summary>
    public EventHandler<InteractiveHandle>? HandleReleased { get; set; }

    /// <summary>Returns the default grid associated with the specified plot.</summary>
    /// <returns>This obsolete method always returns <see langword="null"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310",
        Justification = "This obsolete member is retained to preserve the published public API.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1024",
        Justification = "This obsolete public method is retained to preserve the published API; changing it to a property would break compatibility.")]
    [Obsolete("This method is deprecated. Access Plot.Grid instead.", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1204", Justification = "This obsolete static method is kept with the method group after construction and properties.")]
    public static DefaultGrid GetDefaultGrid() => null!;

    /// <summary>Disposes the resources owned by this plot.</summary>
    public void Dispose()
    {
        DataBackground?.Dispose();
        FigureBackground?.Dispose();

        foreach (IPlottable plottable in PlottableList)
        {
            if (plottable is IDisposable disposablePlottable)
            {
                disposablePlottable.Dispose();
            }
        }

        PlottableList.Clear();
        GC.SuppressFinalize(this);
    }

    /// <summary>Maps plot coordinates to screen pixels using the default axes and most recent layout.</summary>
    /// <param name="coordinates">The plot coordinates to map.</param>
    /// <returns>The corresponding screen pixel.</returns>
    public Pixel GetPixel(Coordinates coordinates) => GetPixel(coordinates, Axes.Bottom, Axes.Left);

    /// <summary>Maps plot coordinates to screen pixels using the specified axes and most recent layout.</summary>
    /// <param name="coordinates">The plot coordinates to map.</param>
    /// <param name="xAxis">The horizontal axis used for the mapping.</param>
    /// <param name="yAxis">The vertical axis used for the mapping.</param>
    /// <returns>The corresponding screen pixel.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1305",
        Justification = "The established public axis parameter names preserve named-argument source compatibility.")]
    public Pixel GetPixel(Coordinates coordinates, IXAxis xAxis, IYAxis yAxis)
    {
        float xPixel = xAxis.GetPixel(coordinates.X, RenderManager.LastRender.DataRect);
        float yPixel = yAxis.GetPixel(coordinates.Y, RenderManager.LastRender.DataRect);

        return new Pixel(xPixel, yPixel).Multiply(ScaleFactorF);
    }

    /// <summary>Maps a screen pixel to plot coordinates using the most recent layout.</summary>
    /// <param name="pixel">The screen pixel to map.</param>
    /// <param name="xAxis">The horizontal axis to use, or <see langword="null"/> to use the bottom axis.</param>
    /// <param name="yAxis">The vertical axis to use, or <see langword="null"/> to use the left axis.</param>
    /// <returns>The corresponding plot coordinates.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The optional axis defaults are part of the established public API; removing them would break source callers.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1305",
        Justification = "The established public axis parameter names preserve named-argument source compatibility.")]
    public Coordinates GetCoordinates(Pixel pixel, IXAxis? xAxis = null, IYAxis? yAxis = null)
    {
        Pixel scaledPixel = pixel.Divide(ScaleFactorF);
        PixelRect dataRect = RenderManager.LastRender.DataRect;
        double xCoordinate = (xAxis ?? Axes.Bottom).GetCoordinate(scaledPixel.X, dataRect);
        double yCoordinate = (yAxis ?? Axes.Left).GetCoordinate(scaledPixel.Y, dataRect);
        return new(xCoordinate, yCoordinate);
    }

    /// <summary>Maps screen pixel coordinates to plot coordinates using the most recent layout.</summary>
    /// <param name="x">The horizontal screen-pixel position.</param>
    /// <param name="y">The vertical screen-pixel position.</param>
    /// <param name="xAxis">The horizontal axis to use, or <see langword="null"/> to use the bottom axis.</param>
    /// <param name="yAxis">The vertical axis to use, or <see langword="null"/> to use the left axis.</param>
    /// <returns>The corresponding plot coordinates.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The optional axis defaults are part of the established public API; removing them would break source callers.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1305",
        Justification = "The established public coordinate and axis parameter names preserve named-argument source compatibility.")]
    public Coordinates GetCoordinates(float x, float y, IXAxis? xAxis = null, IYAxis? yAxis = null)
    {
        Pixel pixel = new(x, y);
        return GetCoordinates(pixel, xAxis, yAxis);
    }

    /// <summary>Creates a coordinate rectangle centered at a screen pixel, using the most recent layout.</summary>
    /// <param name="x">The horizontal center position in screen pixels.</param>
    /// <param name="y">The vertical center position in screen pixels.</param>
    /// <param name="radius">The half-width and half-height of the hit area in screen pixels.</param>
    /// <param name="xAxis">The horizontal axis to use, or <see langword="null"/> to use the bottom axis.</param>
    /// <param name="yAxis">The vertical axis to use, or <see langword="null"/> to use the left axis.</param>
    /// <returns>The coordinate rectangle corresponding to the pixel hit area.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The optional radius and axis defaults are part of the established public API; removing them would break source callers.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1305",
        Justification = "The established public coordinate and axis parameter names preserve named-argument source compatibility.")]
    public CoordinateRect GetCoordinateRect(float x, float y, float radius = 10, IXAxis? xAxis = null, IYAxis? yAxis = null)
    {
        float leftPixel = (x - radius) / ScaleFactorF;
        float rightPixel = (x + radius) / ScaleFactorF;
        float topPixel = (y - radius) / ScaleFactorF;
        float bottomPixel = (y + radius) / ScaleFactorF;

        PixelRect dataRect = RenderManager.LastRender.DataRect;
        double firstXCoordinate = (xAxis ?? Axes.Bottom).GetCoordinate(leftPixel, dataRect);
        double secondXCoordinate = (xAxis ?? Axes.Bottom).GetCoordinate(rightPixel, dataRect);
        double firstYCoordinate = (yAxis ?? Axes.Left).GetCoordinate(topPixel, dataRect);
        double secondYCoordinate = (yAxis ?? Axes.Left).GetCoordinate(bottomPixel, dataRect);

        // rectify rectangles for inverted axes
        double left = Math.Min(firstXCoordinate, secondXCoordinate);
        double right = Math.Max(firstXCoordinate, secondXCoordinate);
        double bottom = Math.Min(firstYCoordinate, secondYCoordinate);
        double top = Math.Max(firstYCoordinate, secondYCoordinate);

        return new(left, right, bottom, top);
    }

    /// <summary>Creates a coordinate rectangle centered at a screen pixel, using the most recent layout.</summary>
    /// <param name="pixel">The center point in screen pixels.</param>
    /// <param name="radius">The half-width and half-height of the hit area in screen pixels.</param>
    /// <param name="xAxis">The horizontal axis to use, or <see langword="null"/> to use the bottom axis.</param>
    /// <param name="yAxis">The vertical axis to use, or <see langword="null"/> to use the left axis.</param>
    /// <returns>The coordinate rectangle corresponding to the pixel hit area.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The optional radius and axis defaults are part of the established public API; removing them would break source callers.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1305",
        Justification = "The established public axis parameter names preserve named-argument source compatibility.")]
    public CoordinateRect GetCoordinateRect(Pixel pixel, float radius = 10, IXAxis? xAxis = null, IYAxis? yAxis = null) => GetCoordinateRect(pixel.X, pixel.Y, radius, xAxis, yAxis);

    /// <summary>Creates a coordinate rectangle centered at plot coordinates using the most recent layout.</summary>
    /// <param name="coordinates">The center point in plot coordinates.</param>
    /// <param name="radius">The half-width and half-height of the hit area in screen pixels.</param>
    /// <param name="xAxis">The horizontal axis to use, or <see langword="null"/> to use the bottom axis.</param>
    /// <param name="yAxis">The vertical axis to use, or <see langword="null"/> to use the left axis.</param>
    /// <returns>The coordinate rectangle corresponding to the hit area.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The optional radius and axis defaults are part of the established public API; removing them would break source callers.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1305",
        Justification = "The established public axis parameter names preserve named-argument source compatibility.")]
    public CoordinateRect GetCoordinateRect(Coordinates coordinates, float radius = 10, IXAxis? xAxis = null, IYAxis? yAxis = null)
    {
        radius /= ScaleFactorF;

        PixelRect dataRect = RenderManager.LastRender.DataRect;
        double radiusX = (xAxis ?? Axes.Bottom).GetCoordinateDistance(radius, dataRect);
        double radiusY = (yAxis ?? Axes.Left).GetCoordinateDistance(radius, dataRect);
        return coordinates.ToRect(radiusX, radiusY);
    }

    /// <summary>Gets the axis containing the specified pixel, if any.</summary>
    /// <param name="pixel">The screen pixel to inspect.</param>
    /// <returns>The axis at the pixel, or <see langword="null"/> when no axis contains it.</returns>
    public IAxis? GetAxis(Pixel pixel)
    {
        IPanel? panel = GetPanel(pixel, axesOnly: true);
        return panel is IAxis axis ? axis : null;
    }

    /// <summary>Gets the panel containing the specified pixel, if any.</summary>
    /// <param name="pixel">The screen pixel to inspect.</param>
    /// <param name="axesOnly">Whether to consider only axis panels.</param>
    /// <returns>The panel at the pixel, or <see langword="null"/> when no matching panel contains it.</returns>
    public IPanel? GetPanel(Pixel pixel, bool axesOnly)
    {
        PixelRect dataRect = RenderManager.LastRender.Layout.DataRect;
        using Paint paint = Paint.NewDisposablePaint();
        IPanel[] panels = Axes.GetPanels();

        // Reverse here so the "highest" axis is returned in the case some overlap.
        for (int panelIndex = panels.Length - 1; panelIndex >= 0; panelIndex--)
        {
            IPanel panel = panels[panelIndex];
            if (axesOnly && panel is not IAxis)
            {
                continue;
            }

            float axisPanelSize = RenderManager.LastRender.Layout.PanelSizes[panel];
            float axisPanelOffset = RenderManager.LastRender.Layout.PanelOffsets[panel];
            PixelRect axisRect = panel.GetPanelRect(dataRect, axisPanelSize, axisPanelOffset, paint);
            if (axisRect.Contains(pixel))
            {
                return panel;
            }
        }

        return null;
    }

    /// <summary>Gets the topmost interactive handle within the specified screen-pixel radius.</summary>
    /// <param name="xPixel">The horizontal screen-pixel position to inspect.</param>
    /// <param name="yPixel">The vertical screen-pixel position to inspect.</param>
    /// <param name="radius">The hit-test radius in screen pixels.</param>
    /// <param name="xAxis">The horizontal axis to use, or <see langword="null"/> to use the bottom axis.</param>
    /// <param name="yAxis">The vertical axis to use, or <see langword="null"/> to use the left axis.</param>
    /// <returns>The topmost matching handle, or <see langword="null"/> when none is found.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The optional hit-radius and axis defaults are part of the established public API; removing them would break source callers.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1305",
        Justification = "The established public pixel and axis parameter names preserve named-argument source compatibility.")]
    public InteractiveHandle? GetInteractiveHandle(float xPixel, float yPixel, float radius = 10, IXAxis? xAxis = null, IYAxis? yAxis = null)
    {
        CoordinateRect rect = GetCoordinateRect(xPixel, yPixel, radius, xAxis, yAxis);
        for (int index = PlottableList.Count - 1; index >= 0; index--)
        {
            if (PlottableList[index] is not IHasInteractiveHandles plottable)
            {
                continue;
            }

            InteractiveHandle? handle = plottable.GetHandle(rect);
            if (handle is not null)
            {
                return handle;
            }
        }

        return null;
    }

    /// <summary>This obsolete overload does not render; use an image, surface, or canvas overload instead.</summary>
    /// <param name="width">The requested render width in pixels.</param>
    /// <param name="height">The requested render height in pixels.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310",
        Justification = "This obsolete member is retained to preserve the published public API.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "This obsolete signature retains its optional defaults for source compatibility.")]
    [Obsolete("Call GetImage() to create a new image, RenderInMemory() to force a render for layout purposes, or Render() to render onto an existing SkiaSharp surface or canvas.", true)]
    public void Render(int width = 400, int height = 300) { }

    /// <summary>Renders the plot onto an existing canvas with the specified dimensions.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="width">The canvas width in pixels.</param>
    /// <param name="height">The canvas height in pixels.</param>
    public void Render(SKCanvas canvas, int width, int height)
    {
        PixelRect rect = new(0, width, height, 0);
        Render(canvas, rect);
    }

    /// <summary>Renders the plot onto an existing canvas inside the specified rectangle.</summary>
    /// <param name="canvas">The destination canvas.</param>
    /// <param name="rect">The area of the canvas to render into.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1901",
        Justification = "Locking this public synchronization object preserves the documented contract for callers that coordinate plot edits with rendering.")]
    public void Render(SKCanvas canvas, PixelRect rect)
    {
        lock (Sync)
        {
            RenderManager.Render(canvas, rect);
        }
    }

    /// <summary>Renders onto a surface using its local clip bounds.</summary>
    /// <param name="surface">The destination surface.</param>
    public void Render(SKSurface surface) => Render(surface.Canvas, surface.Canvas.LocalClipBounds.ToPixelRect());

    /// <summary>Renders the plot to an in-memory image to update its layout.</summary>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The optional dimensions are part of the established public API; removing their defaults would break source callers.")]
    public void RenderInMemory(int width = 400, int height = 300)
    {
        using Image image = GetImage(width, height);
    }

    /// <summary>Creates an image of the specified dimensions and renders the plot into it.</summary>
    /// <param name="width">The image width in pixels; must be positive.</param>
    /// <param name="height">The image height in pixels; must be positive.</param>
    /// <returns>The rendered image, which the caller must dispose.</returns>
    public Image GetImage(int width, int height)
    {
        if (width < 1)
        {
            throw new ArgumentException($"{nameof(width)} must be greater than 0");
        }

        if (height < 1)
        {
            throw new ArgumentException($"{nameof(height)} must be greater than 0");
        }

        SKImageInfo info = new(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        using SKSurface surface = SKSurface.Create(info);

        Render(surface.Canvas, width, height);
        return new(surface);
    }

    /// <summary>This obsolete method returns the plot as PNG HTML; use <see cref="GetPngHtml"/> or <see cref="GetSvgHtml"/>.</summary>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <returns>The HTML image element.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310",
        Justification = "This obsolete member is retained to preserve the published public API.")]
    [Obsolete("This method is deprecated. Use GetPngHtml() or GetSvgHtml()", true)]
    public string GetImageHtml(int width, int height) => GetPngHtml(width, height);

    /// <summary>Renders the plot as a PNG and returns an HTML <c>img</c> element containing its Base64 data.</summary>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <param name="classContent">The value of the element's <c>class</c> attribute.</param>
    /// <param name="styleContent">The value of the element's <c>style</c> attribute.</param>
    /// <returns>The HTML image element.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The optional HTML attribute defaults are part of the established public API; removing them would break source callers.")]
    public string GetPngHtml(int width, int height, string classContent = "", string styleContent = "")
    {
        using Image image = GetImage(width, height);
        byte[] bytes = image.GetImageBytes();
        return ImageOperations.GetImageHtml(bytes, "png", classContent, styleContent);
    }

    /// <summary>Renders the plot as SVG and returns markup suitable for insertion into HTML.</summary>
    /// <param name="width">The SVG width in pixels.</param>
    /// <param name="height">The SVG height in pixels.</param>
    /// <returns>The SVG markup.</returns>
    public string GetSvgHtml(int width, int height) => GetSvgXml(width, height);

    /// <summary>Renders the plot and saves it as a JPEG image.</summary>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <param name="quality">The JPEG quality from 0 to 100.</param>
    /// <returns>Information about the saved image and render.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The established JPEG quality default is retained for source compatibility.")]
    public SavedImageInfo SaveJpeg(string filePath, int width, int height, int quality = 85)
    {
        using Image image = GetImage(width, height);
        return image.SaveJpeg(filePath, quality).WithRenderDetails(RenderManager.LastRender);
    }

    /// <summary>Renders the plot and saves it as a PNG image.</summary>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <returns>Information about the saved image and render.</returns>
    public SavedImageInfo SavePng(string filePath, int width, int height)
    {
        using Image image = GetImage(width, height);
        return image.SavePng(filePath).WithRenderDetails(RenderManager.LastRender);
    }

    /// <summary>Renders the plot and saves it as a bitmap image.</summary>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <returns>Information about the saved image and render.</returns>
    public SavedImageInfo SaveBmp(string filePath, int width, int height)
    {
        using Image image = GetImage(width, height);
        return image.SaveBmp(filePath).WithRenderDetails(RenderManager.LastRender);
    }

    /// <summary>Renders the plot and saves it as a WebP image.</summary>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <param name="quality">The WebP quality from 0 to 100.</param>
    /// <returns>Information about the saved image and render.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The established WebP quality default is retained for source compatibility.")]
    public SavedImageInfo SaveWebp(string filePath, int width, int height, int quality = 85)
    {
        using Image image = GetImage(width, height);
        return image.SaveWebp(filePath, quality).WithRenderDetails(RenderManager.LastRender);
    }

    /// <summary>Renders the plot and saves it as an SVG image.</summary>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="width">The SVG width in pixels.</param>
    /// <param name="height">The SVG height in pixels.</param>
    /// <returns>Information about the saved image and render.</returns>
    public SavedImageInfo SaveSvg(string filePath, int width, int height)
    {
        string xml = GetSvgXml(width, height);
        File.WriteAllText(filePath, xml);
        return new SavedImageInfo(filePath, xml.Length).WithRenderDetails(RenderManager.LastRender);
    }

    /// <summary>Renders the plot and returns its SVG markup.</summary>
    /// <param name="width">The SVG width in pixels.</param>
    /// <param name="height">The SVG height in pixels.</param>
    /// <returns>The rendered SVG document.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1901",
        Justification = "Locking this public synchronization object preserves the documented contract for callers that coordinate plot edits with rendering.")]
    public string GetSvgXml(int width, int height)
    {
        RenderInMemory(width, height);
        using SvgImage svgImage = new(width, height);
        lock (Sync)
        {
            bool originalClearState = RenderManager.ClearCanvasBeforeEachRender;
            try
            {
                RenderManager.ClearCanvasBeforeEachRender = false;
                Render(svgImage.Canvas, width, height);
                return svgImage.GetXml();
            }
            finally
            {
                RenderManager.ClearCanvasBeforeEachRender = originalClearState;
            }
        }
    }

    /// <summary>Saves the plot using the image format inferred from the file extension.</summary>
    /// <param name="filePath">The destination file path, whose extension selects the format.</param>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <returns>Information about the saved image and render.</returns>
    public SavedImageInfo Save(string filePath, int width, int height)
    {
        ImageFormat format = ImageFormats.FromFilename(filePath);
        return Save(filePath, width, height, format);
    }

    /// <summary>Renders the plot and saves it in the specified image format.</summary>
    /// <param name="filePath">The destination file path.</param>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <param name="format">The image format to write.</param>
    /// <param name="quality">The encoding quality for formats that support it.</param>
    /// <returns>Information about the saved image and render.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The established quality default is retained for source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2485",
        Justification = "Only SVG and raster formats are supported; unsupported formats are deliberately rejected.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "RCS1079",
        Justification = "Only SVG and raster formats are supported; unsupported formats are deliberately rejected.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2485", Justification = "Only SVG and raster formats are supported; unsupported formats are deliberately rejected.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "RCS1079", Justification = "Only SVG and raster formats are supported; unsupported formats are deliberately rejected.")]
    public SavedImageInfo Save(string filePath, int width, int height, ImageFormat format, int quality = 85)
    {
        if (format == ImageFormat.Svg)
        {
            return SaveSvg(filePath, width, height).WithRenderDetails(RenderManager.LastRender);
        }

        if (format.IsRasterFormat())
        {
            using Image image = GetImage(width, height);
            return image.Save(filePath, format, quality).WithRenderDetails(RenderManager.LastRender);
        }

        throw new NotImplementedException(format.ToString());
    }

    /// <summary>Renders the plot and returns the encoded image bytes.</summary>
    /// <param name="width">The image width in pixels.</param>
    /// <param name="height">The image height in pixels.</param>
    /// <param name="format">The image format to encode.</param>
    /// <returns>The encoded image bytes.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The established BMP format default is retained for source compatibility.")]
    public byte[] GetImageBytes(int width, int height, ImageFormat format = ImageFormat.Bmp)
    {
        using Image image = GetImage(width, height);
        return image.GetImageBytes(format);
    }

    /// <summary>Creates a raster image containing the legend.</summary>
    /// <returns>The legend image, which the caller must dispose.</returns>
    public Image GetLegendImage() => Legend.GetImage();

    /// <summary>Returns the legend as SVG markup.</summary>
    /// <returns>The legend's SVG document.</returns>
    public string GetLegendSvgXml() => Legend.GetSvgXml();

    /// <summary>Gets the plottables in their current rendering order.</summary>
    /// <returns>A live enumerable over the plot's plottable list.</returns>
    public IEnumerable<IPlottable> GetPlottables() => PlottableList;

    /// <summary>Gets plottables of the specified type in their current rendering order.</summary>
    /// <typeparam name="T">The plottable type to select.</typeparam>
    /// <returns>The matching plottables.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2307",
        Justification = "This generic API is retained for existing callers and provides the established type-filtering call shape.")]
    public IEnumerable<T> GetPlottables<T>()
        where T : IPlottable
    {
        foreach (IPlottable plottable in PlottableList)
        {
            if (plottable is T matchingPlottable)
            {
                yield return matchingPlottable;
            }
        }
    }

    /// <summary>Removes every occurrence of the specified plottable from this plot.</summary>
    /// <param name="plottable">The plottable to remove.</param>
    public void Remove(IPlottable plottable)
    {
        while (PlottableList.Contains(plottable))
        {
            _ = PlottableList.Remove(plottable);
        }
    }

    /// <summary>Removes the specified panel from this plot's axes.</summary>
    /// <param name="panel">The panel to remove.</param>
    public void Remove(IPanel panel) => Axes.Remove(panel);

    /// <summary>Removes the specified axis from this plot.</summary>
    /// <param name="axis">The axis to remove.</param>
    public void Remove(IAxis axis) => Axes.Remove(axis);

    /// <summary>Removes plottables whose runtime type exactly matches the specified type.</summary>
    /// <param name="plotType">The exact runtime type to remove.</param>
    public void Remove(Type plotType)
    {
        for (int index = PlottableList.Count - 1; index >= 0; index--)
        {
            if (PlottableList[index].GetType() == plotType)
            {
                PlottableList.RemoveAt(index);
            }
        }
    }

    /// <summary>Removes all plottables assignable to the specified type.</summary>
    /// <typeparam name="T">The plottable type to remove.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2307",
        Justification = "This generic API is retained for existing callers and provides the established type-filtering call shape.")]
    public void Remove<T>()
        where T : IPlottable => _ = PlottableList.RemoveAll(static x => x is T);

    /// <summary>Removes plottables of the specified type that satisfy a predicate.</summary>
    /// <param name="predicate">Returns <see langword="true"/> for each plottable to remove.</param>
    /// <typeparam name="T">The plottable type to examine.</typeparam>
    public void Remove<T>(Func<T, bool> predicate)
        where T : IPlottable
    {
        List<T> toRemove = [];
        foreach (IPlottable plottable in PlottableList)
        {
            if (plottable is T typedPlottable && predicate(typedPlottable))
            {
                toRemove.Add(typedPlottable);
            }
        }

        foreach (T plottable in toRemove)
        {
            _ = PlottableList.Remove(plottable);
        }
    }

    /// <summary>Moves the specified plottable to the top of the rendering order.</summary>
    /// <param name="plottable">The plottable to move.</param>
    public void MoveToTop(IPlottable plottable) => MoveToFront(plottable);

    /// <summary>Moves the specified plottable to the bottom of the rendering order.</summary>
    /// <param name="plottable">The plottable to move.</param>
    public void MoveToBottom(IPlottable plottable) => MoveToBack(plottable);

    /// <summary>Moves the specified plottable to the end of the list so it renders last.</summary>
    /// <param name="plottable">The plottable to move.</param>
    public void MoveToFront(IPlottable plottable)
    {
        int index = PlottableList.IndexOf(plottable);

        if (index < 0)
        {
            return;
        }

        PlottableList.RemoveAt(index);
        PlottableList.Add(plottable);
    }

    /// <summary>Moves the specified plottable to the start of the list so it renders first.</summary>
    /// <param name="plottable">The plottable to move.</param>
    public void MoveToBack(IPlottable plottable)
    {
        int index = PlottableList.IndexOf(plottable);

        if (index < 0)
        {
            return;
        }

        PlottableList.RemoveAt(index);
        PlottableList.Insert(0, plottable);
    }

    /// <summary>Disable visibility for all axes and grids.</summary>
    public void HideAxesAndGrid()
    {
        Axes.Frameless();
        HideGrid();
    }

    /// <summary>Enable visibility for all axes and grids.</summary>
    public void ShowAxesAndGrid()
    {
        Axes.Frameless(false);
        ShowGrid();
    }

    /// <summary>Disable visibility for all grids.</summary>
    public void HideGrid() => Axes.AllGrids.ForEach(static x => x.IsVisible = false);

    /// <summary>Enable visibility for all grids.</summary>
    public void ShowGrid() => Axes.AllGrids.ForEach(static x => x.IsVisible = true);

    /// <summary>Shows the legend without changing its current placement.</summary>
    /// <returns>The legend for further configuration.</returns>
    public Legend ShowLegend()
    {
        Legend.IsVisible = true;
        return Legend;
    }

    /// <summary>Shows the legend at the specified alignment.</summary>
    /// <param name="alignment">The legend's alignment within the data area.</param>
    /// <returns>The legend for further configuration.</returns>
    public Legend ShowLegend(Alignment alignment)
    {
        Legend.IsVisible = true;
        Legend.Alignment = alignment;
        return Legend;
    }

    /// <summary>Shows the legend with the specified alignment and orientation.</summary>
    /// <param name="alignment">The legend's alignment within the data area.</param>
    /// <param name="orientation">The direction in which legend items are arranged.</param>
    /// <returns>The legend for further configuration.</returns>
    public Legend ShowLegend(Alignment alignment, Orientation orientation)
    {
        Legend.IsVisible = true;
        Legend.Alignment = alignment;
        Legend.Orientation = orientation;
        return Legend;
    }

    /// <summary>Shows the legend with the specified manual items.</summary>
    /// <param name="items">The items to display in the legend.</param>
    /// <param name="location">The alignment of the legend within the data area.</param>
    /// <returns>The legend for further configuration.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The established legend alignment default is retained for source compatibility.")]
    public Legend ShowLegend(IEnumerable<LegendItem> items, Alignment location = Alignment.LowerRight)
    {
        _ = ShowLegend(location);
        Legend.ManualItems.Clear();
        Legend.ManualItems.AddRange(items);
        return Legend;
    }

    /// <summary>
    /// Hide the default legend (inside the data area) and create a new legend panel
    /// placed on the edge of the figure outside the data area.
    /// </summary>
    /// <param name="edge">The figure edge on which to place the legend panel.</param>
    /// <returns>The added legend panel.</returns>
    public Panels.LegendPanel ShowLegend(Edge edge)
    {
        _ = HideLegend();

        Legend.Orientation = edge.IsVertical()
            ? Orientation.Vertical
            : Orientation.Horizontal;

        Panels.LegendPanel legendPanel = new(Legend) { Edge = edge, Alignment = Alignment.MiddleCenter, };

        Axes.AddPanel(legendPanel);

        return legendPanel;
    }

    /// <summary>Hides the legend.</summary>
    /// <returns>The legend for further configuration.</returns>
    public Legend HideLegend()
    {
        Legend.IsVisible = false;
        return Legend;
    }

    /// <summary>Removes all plottables from the plot.</summary>
    public void Clear() => PlottableList.Clear();

    /// <summary>Removes all plottables assignable to the specified type.</summary>
    /// <typeparam name="T">The plottable type to remove.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2307",
        Justification = "This generic API is retained for existing callers and provides the established type-filtering call shape.")]
    public void Clear<T>()
        where T : IPlottable => Remove<T>();

    /// <summary>Sets the title text and makes the title visible.</summary>
    /// <param name="text">The new title, or <see langword="null"/> to keep the current text.</param>
    /// <param name="size">The title font size, or <see langword="null"/> to keep the current size.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The established optional title defaults are retained for source compatibility.")]
    public void Title(string? text = null, float? size = null)
    {
        Axes.Title.Label.Text = text ?? Axes.Title.Label.Text;
        Axes.Title.IsVisible = true;
        if (!size.HasValue)
        {
            return;
        }

        Axes.Title.Label.FontSize = size.Value;
    }

    /// <summary>Shows or hides the plot title.</summary>
    /// <param name="show"><see langword="true"/> to show the title; otherwise, <see langword="false"/>.</param>
    public void Title(bool show) => Axes.Title.IsVisible = show;

    /// <summary>Sets the bottom-axis label text and optionally its font size.</summary>
    /// <param name="label">The label text.</param>
    /// <param name="size">The font size, or <see langword="null"/> to leave it unchanged.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The established optional label-size default is retained for source compatibility.")]
    public void XLabel(string label, float? size = null)
    {
        Axes.Bottom.Label.Text = label;
        if (!size.HasValue)
        {
            return;
        }

        Axes.Bottom.Label.FontSize = size.Value;
    }

    /// <summary>Sets the left-axis label text and optionally its font size.</summary>
    /// <param name="label">The label text.</param>
    /// <param name="size">The font size, or <see langword="null"/> to leave it unchanged.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The established optional label-size default is retained for source compatibility.")]
    public void YLabel(string label, float? size = null)
    {
        Axes.Left.Label.Text = label;
        if (!size.HasValue)
        {
            return;
        }

        Axes.Left.Label.FontSize = size.Value;
    }

    /// <summary>Creates a snapshot of this plot's style settings.</summary>
    /// <returns>The style settings for this plot.</returns>
    public PlotStyle GetStyle() => PlotStyle.FromPlot(this);

    /// <summary>Applies the specified style settings to this plot.</summary>
    /// <param name="style">The settings to apply.</param>
    public void SetStyle(PlotStyle style) => style.Apply(this);

    /// <summary>Copies the style settings from another plot to this plot.</summary>
    /// <param name="otherPlot">The plot whose style settings are copied.</param>
    public void SetStyle(Plot otherPlot) => SetStyle(otherPlot.GetStyle());

    /// <summary>Shows or hides diagnostic details for every axis panel.</summary>
    /// <param name="enable"><see langword="true"/> to show diagnostic details; otherwise, <see langword="false"/>.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The established optional enabled default is retained for source compatibility.")]
    public void Developer_ShowAxisDetails(bool enable = true)
    {
        foreach (IPanel panel in Axes.GetAxes())
        {
            panel.ShowDebugInformation = enable;
        }
    }

    /// <summary>Gets the single-precision scale factor used by pixel-coordinate conversion.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1201", Justification = "This internal helper follows the public members to satisfy accessibility ordering.")]
    internal float ScaleFactorF { get; private set; } = 1.0F;
}
