// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables.Interactive;

/// <summary>Renders a vertical span with interactive edge and body handles.</summary>
public class InteractiveVerticalSpan : IPlottable, IHasInteractiveHandles
{
    /// <summary>Identifies the part of the span selected by an interactive handle.</summary>
    private enum Handles
    {
        /// <summary>The first edge of the span.</summary>
        Edge1,

        /// <summary>The body of the span.</summary>
        Body,

        /// <summary>The second edge of the span.</summary>
        Edge2
    }

    /// <summary>Gets or sets whether the span is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position the span.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this span.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets the cursor displayed over an edge handle.</summary>
    public Cursor EdgeCursor { get; set; } = Cursor.SizeNorthSouth;

    /// <summary>Gets or sets the cursor displayed over the span body.</summary>
    public Cursor BodyCursor { get; set; } = Cursor.SizeAll;

    /// <summary>Gets the line style used to render the span edges.</summary>
    public LineStyle LineStyle { get; } = new(Constants.LineWidth, Colors.Black);

    /// <summary>Gets the fill style used to render the span body.</summary>
    public FillStyle FillStyle { get; } = new(Colors.Black.WithOpacity(Constants.FillOpacity));

    /// <summary>Gets or sets the first y-coordinate of the span.</summary>
    public double Y1 { get; set; }

    /// <summary>Gets or sets the second y-coordinate of the span.</summary>
    public double Y2 { get; set; }

    /// <summary>Gets the lesser y-coordinate of the span.</summary>
    public double YMin => Math.Min(Y1, Y2);

    /// <summary>Gets the greater y-coordinate of the span.</summary>
    public double YMax => Math.Max(Y1, Y2);

    /// <summary>Gets the axis limits occupied by the span.</summary>
    /// <returns>The vertical limits of the span.</returns>
    public AxisLimits GetAxisLimits() => AxisLimits.VerticalOnly(YMin, YMax);

    /// <summary>Gets an interactive handle when the specified coordinate rectangle intersects the span.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns>The matching handle, or <see langword="null"/> when no handle is hit.</returns>
    public InteractiveHandle? GetHandle(CoordinateRect rect)
    {
        if (!IsVisible)
        {
            return null;
        }

        if (rect.ContainsY(Y1))
        {
            return new(this, EdgeCursor);
        }

        if (rect.ContainsY(Y2))
        {
            return new(this, EdgeCursor, (int)Handles.Edge2);
        }

        return rect.Center.Y >= YMin && rect.Center.Y <= YMax ? new InteractiveHandle(this, BodyCursor, (int)Handles.Body) : null;
    }

    /// <summary>Moves the selected span handle to the specified coordinate.</summary>
    /// <param name="handle">The handle being moved.</param>
    /// <param name="point">The new coordinate of the handle.</param>
    public virtual void MoveHandle(InteractiveHandle handle, Coordinates point)
    {
        if (handle.Index == (int)Handles.Edge1)
        {
            Y1 = point.Y;
        }
        else if (handle.Index == (int)Handles.Edge2)
        {
            Y2 = point.Y;
        }
        else if (handle.Index == (int)Handles.Body)
        {
            double span = Math.Abs(Y1 - Y2);
            Y1 = point.Y - (span / Constants.Half);
            Y2 = point.Y + (span / Constants.Half);
        }
    }

    /// <summary>Performs any required work when a span handle is pressed.</summary>
    /// <param name="handle">The pressed handle.</param>
    public virtual void PressHandle(InteractiveHandle handle) { }

    /// <summary>Performs any required work when a span handle is released.</summary>
    /// <param name="handle">The released handle.</param>
    public virtual void ReleaseHandle(InteractiveHandle handle) { }

    /// <summary>Renders the span fill and its horizontal edges.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        float minimumPixel = Axes.GetPixelY(YMin);
        float maximumPixel = Axes.GetPixelY(YMax);
        PixelRect pixelRect = new(rp.DataRect.Left, rp.DataRect.Right, minimumPixel, maximumPixel);
        FillStyle.Render(rp, pixelRect);
        PixelLine line1 = new(rp.DataRect.Left, minimumPixel, rp.DataRect.Right, minimumPixel);
        PixelLine line2 = new(rp.DataRect.Left, maximumPixel, rp.DataRect.Right, maximumPixel);
        LineStyle.Render(rp, line1);
        LineStyle.Render(rp, line2);
    }

    /// <summary>Contains default values used to render interactive spans.</summary>
    private static class Constants
    {
        /// <summary>The default width of span edges in pixels.</summary>
        public const float LineWidth = 2;

        /// <summary>The default fill opacity.</summary>
        public const float FillOpacity = 0.2F;

        /// <summary>The divisor used to calculate half the span width.</summary>
        public const double Half = 2;
    }
}
