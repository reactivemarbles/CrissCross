// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>The shaded region shown while the user middle-click-drags to zoom.</summary>
/// <param name="plot">The plot whose axes are updated when the zoom is applied.</param>
public class ZoomRectangle(Plot plot) : IZoomRectangle
{
    /// <summary>Gets or sets whether the zoom rectangle is visible.</summary>
    public bool IsVisible { get; set; }

    /// <summary>Gets or sets the fill style of the zoom rectangle.</summary>
    public FillStyle FillStyle { get; set; } = new() { Color = Colors.Red.WithAlpha(Constants.FillAlpha), AntiAlias = false };

    /// <summary>Gets or sets the outline style of the zoom rectangle.</summary>
    public LineStyle LineStyle { get; set; } = new() { Color = Colors.Red.WithAlpha(Constants.LineAlpha), Width = Constants.LineWidth, AntiAlias = false };

    /// <summary>Gets or sets the pixel where the zoom drag began.</summary>
    public Pixel MouseDown { get; set; }

    /// <summary>Gets or sets the current pixel where the zoom drag ends.</summary>
    public Pixel MouseUp { get; set; }

    /// <summary>Gets or sets whether the selection spans the plot's full horizontal extent.</summary>
    public bool HorizontalSpan { get; set; }

    /// <summary>Gets or sets whether the selection spans the plot's full vertical extent.</summary>
    public bool VerticalSpan { get; set; }

    /// <summary>Gets the plot associated with this zoom rectangle.</summary>
    public Plot Plot { get; } = plot;

    /// <summary>Applies the selected horizontal range to an X axis.</summary>
    /// <param name="xAxes">The horizontal axis to update.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1305", Justification = "Retained public parameter name for named-argument source compatibility.")]
    public void Apply(IXAxis xAxes)
    {
        if (HorizontalSpan)
        {
            return;
        }

        PixelRect dataRect = Plot.RenderManager.LastRender.DataRect;
        double x1 = xAxes.GetCoordinate(MouseDown.X, dataRect);
        double x2 = xAxes.GetCoordinate(MouseUp.X, dataRect);
        double minimumCoordinate = Math.Min(x1, x2);
        double maximumCoordinate = Math.Max(x1, x2);
        CoordinateRange horizontalRange = xAxes.IsInverted() ? new(maximumCoordinate, minimumCoordinate) : new(minimumCoordinate, maximumCoordinate);
        xAxes.Range.Set(horizontalRange);
    }

    /// <summary>Applies the selected vertical range to a Y axis.</summary>
    /// <param name="yAxes">The vertical axis to update.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1305", Justification = "Retained public parameter name for named-argument source compatibility.")]
    public void Apply(IYAxis yAxes)
    {
        if (VerticalSpan)
        {
            return;
        }

        PixelRect dataRect = Plot.RenderManager.LastRender.DataRect;
        double y1 = yAxes.GetCoordinate(MouseDown.Y, dataRect);
        double y2 = yAxes.GetCoordinate(MouseUp.Y, dataRect);
        double minimumCoordinate = Math.Min(y1, y2);
        double maximumCoordinate = Math.Max(y1, y2);
        CoordinateRange verticalRange = yAxes.IsInverted() ? new(maximumCoordinate, minimumCoordinate) : new(minimumCoordinate, maximumCoordinate);
        yAxes.Range.Set(verticalRange);
    }

    /// <summary>Renders the zoom selection within the data area.</summary>
    /// <param name="rp">The render context.</param>
    public void Render(RenderPack rp)
    {
        SKCanvas canvas = rp.Canvas;
        PixelRect dataRect = rp.DataRect;

        SKRect rect = new(MouseDown.X, MouseDown.Y, MouseUp.X, MouseUp.Y);

        rp.CanvasState.Save();
        canvas.ClipRect(dataRect.ToSKRect());

        if (HorizontalSpan)
        {
            rect.Left = dataRect.Left;
            rect.Right = dataRect.Right;
        }

        if (VerticalSpan)
        {
            rect.Bottom = dataRect.Bottom;
            rect.Top = dataRect.Top;
        }

        Drawing.FillRectangle(canvas, rect.ToPixelRect(), rp.Paint, FillStyle);
        Drawing.DrawRectangle(canvas, rect.ToPixelRect(), rp.Paint, LineStyle);
        rp.CanvasState.Restore();
    }

    /// <summary>Contains constants used by zoom rectangles.</summary>
    internal static class Constants
    {
        /// <summary>The alpha channel for the rectangle fill.</summary>
        internal const byte FillAlpha = 100;

        /// <summary>The alpha channel for the rectangle outline.</summary>
        internal const byte LineAlpha = 200;

        /// <summary>The width of the rectangle outline.</summary>
        internal const float LineWidth = 1;
    }
}
