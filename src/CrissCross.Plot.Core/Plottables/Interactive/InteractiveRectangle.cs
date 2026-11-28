// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables.Interactive;

/// <summary>Renders a rectangle with interactive edge and body handles.</summary>
public class InteractiveRectangle : IPlottable, IHasInteractiveHandles
{
    /// <summary>Identifies a rectangle edge or its body.</summary>
    private enum Handles
    {
        /// <summary>The left edge.</summary>
        X1,

        /// <summary>The top edge.</summary>
        Y1,

        /// <summary>The right edge.</summary>
        X2,

        /// <summary>The bottom edge.</summary>
        Y2,

        /// <summary>The rectangle body.</summary>
        Body
    }

    /// <summary>Gets or sets whether the rectangle is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position the rectangle.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this rectangle.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets the line style used to render the rectangle edges.</summary>
    public LineStyle LineStyle { get; } = new(Constants.LineWidth, Colors.Black);

    /// <summary>Gets the fill style used to render the rectangle body.</summary>
    public FillStyle FillStyle { get; } = new(Colors.Black.WithOpacity(Constants.FillOpacity));

    /// <summary>Gets or sets the first x-coordinate of the rectangle.</summary>
    public double X1 { get; set; }

    /// <summary>Gets or sets the second x-coordinate of the rectangle.</summary>
    public double X2 { get; set; }

    /// <summary>Gets or sets the first y-coordinate of the rectangle.</summary>
    public double Y1 { get; set; }

    /// <summary>Gets or sets the second y-coordinate of the rectangle.</summary>
    public double Y2 { get; set; }

    /// <summary>Gets the lesser x-coordinate of the rectangle.</summary>
    public double XMin => Math.Min(X1, X2);

    /// <summary>Gets the greater x-coordinate of the rectangle.</summary>
    public double XMax => Math.Max(X1, X2);

    /// <summary>Gets the lesser y-coordinate of the rectangle.</summary>
    public double YMin => Math.Min(Y1, Y2);

    /// <summary>Gets the greater y-coordinate of the rectangle.</summary>
    public double YMax => Math.Max(Y1, Y2);

    /// <summary>Gets or sets the rectangle's normalized coordinate bounds.</summary>
    public CoordinateRect Rect
    {
        get => new(XMin, XMax, YMin, YMax);
        set
        {
            X1 = value.Left;
            X2 = value.Right;
            Y1 = value.Top;
            Y2 = value.Bottom;
        }
    }

    /// <summary>Gets the axis limits occupied by the rectangle.</summary>
    /// <returns>The axis limits of the rectangle.</returns>
    public AxisLimits GetAxisLimits() => new(Rect);

    /// <summary>Gets an interactive handle when the specified coordinate rectangle intersects this rectangle.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns>The matching edge or body handle, or <see langword="null"/>.</returns>
    public InteractiveHandle? GetHandle(CoordinateRect rect) =>
        IsVisible ? GetXEdgeHandle(rect) ?? GetYEdgeHandle(rect) ?? GetBodyHandle(rect) : null;

    /// <summary>Moves the selected rectangle handle to the specified coordinate.</summary>
    /// <param name="handle">The handle being moved.</param>
    /// <param name="point">The new coordinate of the handle.</param>
    public virtual void MoveHandle(InteractiveHandle handle, Coordinates point)
    {
        if (handle.Index == (int)Handles.X1)
        {
            X1 = point.X;
        }
        else if (handle.Index == (int)Handles.X2)
        {
            X2 = point.X;
        }

        if (handle.Index == (int)Handles.Y1)
        {
            Y1 = point.Y;
        }
        else if (handle.Index == (int)Handles.Y2)
        {
            Y2 = point.Y;
        }
        else if (handle.Index == (int)Handles.Body)
        {
            double x1 = point.X - (Rect.Width / Constants.Half);
            double x2 = point.X + (Rect.Width / Constants.Half);
            double y1 = point.Y - (Rect.Height / Constants.Half);
            double y2 = point.Y + (Rect.Height / Constants.Half);
            Rect = new(x1, x2, y1, y2);
        }
    }

    /// <summary>Performs any required work when a rectangle handle is pressed.</summary>
    /// <param name="handle">The pressed handle.</param>
    public virtual void PressHandle(InteractiveHandle handle) { }

    /// <summary>Performs any required work when a rectangle handle is released.</summary>
    /// <param name="handle">The released handle.</param>
    public virtual void ReleaseHandle(InteractiveHandle handle) { }

    /// <summary>Renders the rectangle fill and edges.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        PixelRect pixelRect = Axes.GetPixelRect(Rect);
        FillStyle.Render(rp, pixelRect);
        LineStyle.Render(rp, pixelRect);
    }

    /// <summary>Gets the handle for the first or second vertical edge, if hit.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns>The matching vertical-edge handle, or <see langword="null"/>.</returns>
    private InteractiveHandle? GetXEdgeHandle(CoordinateRect rect)
    {
        bool centerWithinY = rect.Center.Y >= YMin && rect.Center.Y <= YMax;
        if (rect.ContainsX(X1) && centerWithinY)
        {
            return new(this, Cursor.SizeWestEast);
        }

        return rect.ContainsX(X2) && centerWithinY
            ? new InteractiveHandle(this, Cursor.SizeWestEast, (int)Handles.X2)
            : null;
    }

    /// <summary>Gets the handle for the first or second horizontal edge, if hit.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns>The matching horizontal-edge handle, or <see langword="null"/>.</returns>
    private InteractiveHandle? GetYEdgeHandle(CoordinateRect rect)
    {
        bool centerWithinX = rect.Center.X >= XMin && rect.Center.X <= XMax;
        if (rect.ContainsY(Y1) && centerWithinX)
        {
            return new(this, Cursor.SizeNorthSouth, (int)Handles.Y1);
        }

        return rect.ContainsY(Y2) && centerWithinX
            ? new InteractiveHandle(this, Cursor.SizeNorthSouth, (int)Handles.Y2)
            : null;
    }

    /// <summary>Gets the body handle when the rectangle contains the selection center.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns>The body handle, or <see langword="null"/>.</returns>
    private InteractiveHandle? GetBodyHandle(CoordinateRect rect)
    {
        bool centerWithinX = rect.Center.X >= XMin && rect.Center.X <= XMax;
        bool centerWithinY = rect.Center.Y >= YMin && rect.Center.Y <= YMax;
        return centerWithinX && centerWithinY ? new InteractiveHandle(this, Cursor.SizeAll, (int)Handles.Body) : null;
    }

    /// <summary>Contains geometry constants used to render and move the rectangle.</summary>
    private static class Constants
    {
        /// <summary>The default line width in pixels.</summary>
        public const float LineWidth = 2;

        /// <summary>The default fill opacity.</summary>
        public const float FillOpacity = 0.2F;

        /// <summary>The divisor used to calculate half a rectangle dimension.</summary>
        public const double Half = 2;
    }
}
