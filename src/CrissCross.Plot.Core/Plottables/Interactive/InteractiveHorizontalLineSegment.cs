// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables.Interactive;

/// <summary>Renders a horizontal line segment with draggable endpoint and body handles.</summary>
public class InteractiveHorizontalLineSegment : IPlottable, IHasInteractiveHandles
{
    /// <summary>Identifies the part of the line selected by an interactive handle.</summary>
    private enum Handle
    {
        /// <summary>The first endpoint of the segment.</summary>
        Edge1,

        /// <summary>The body of the segment.</summary>
        Body,

        /// <summary>The second endpoint of the segment.</summary>
        Edge2
    }

    /// <summary>Gets or sets whether the line segment is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position the segment.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this line segment.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets the line style used to render the segment and handles.</summary>
    public LineStyle LineStyle { get; } = new(Constants.LineWidth, Colors.Black);

    /// <summary>Gets or sets the length of the endpoint handles in pixels.</summary>
    public float HandleLength { get; set; } = Constants.DefaultHandleLength;

    /// <summary>Gets or sets the y-coordinate of the segment.</summary>
    public double Y { get; set; }

    /// <summary>Gets or sets the x-coordinate of the first endpoint.</summary>
    public double X1 { get; set; }

    /// <summary>Gets or sets the x-coordinate of the second endpoint.</summary>
    public double X2 { get; set; }

    /// <summary>Gets the lesser x-coordinate of the endpoints.</summary>
    public double XMin => Math.Min(X1, X2);

    /// <summary>Gets the greater x-coordinate of the endpoints.</summary>
    public double XMax => Math.Max(X1, X2);

    /// <summary>Gets the axis limits occupied by this line segment.</summary>
    /// <returns>The horizontal and vertical ranges of the segment.</returns>
    public AxisLimits GetAxisLimits() => new(XMin, XMax, Y, Y);

    /// <summary>Gets an interactive handle when the specified coordinate rectangle intersects the segment.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns>The matching handle, or <see langword="null"/> when no handle is hit.</returns>
    public InteractiveHandle? GetHandle(CoordinateRect rect)
    {
        if (!IsVisible || !rect.ContainsY(Y))
        {
            return null;
        }

        if (rect.ContainsX(X1))
        {
            return new(this, Cursor.SizeWestEast);
        }

        if (rect.ContainsX(X2))
        {
            return new(this, Cursor.SizeWestEast, (int)Handle.Edge2);
        }

        return rect.Center.X >= XMin && rect.Center.X <= XMax ? new InteractiveHandle(this, Cursor.SizeNorthSouth, (int)Handle.Body) : null;
    }

    /// <summary>Moves the selected line handle to the specified coordinate.</summary>
    /// <param name="handle">The handle being moved.</param>
    /// <param name="point">The new coordinate of the handle.</param>
    public virtual void MoveHandle(InteractiveHandle handle, Coordinates point)
    {
        if (handle.Index == (int)Handle.Edge1)
        {
            X1 = point.X;
        }
        else if (handle.Index == (int)Handle.Edge2)
        {
            X2 = point.X;
        }
        else if (handle.Index == (int)Handle.Body)
        {
            Y = point.Y;
        }
    }

    /// <summary>Performs any required work when a line handle is pressed.</summary>
    /// <param name="handle">The pressed handle.</param>
    public virtual void PressHandle(InteractiveHandle handle) { }

    /// <summary>Performs any required work when a line handle is released.</summary>
    /// <param name="handle">The released handle.</param>
    public virtual void ReleaseHandle(InteractiveHandle handle) { }

    /// <summary>Renders the line segment and endpoint handles.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        float y = Axes.GetPixelY(Y);
        float handleBottom = y + (HandleLength / Constants.Half);
        float handleTop = y - (HandleLength / Constants.Half);
        float x1 = Axes.GetPixelX(XMin);
        float x2 = Axes.GetPixelX(XMax);
        PixelLine lineBody = new(x1, y, x2, y);
        PixelLine lineLeft = new(x1, handleBottom, x1, handleTop);
        PixelLine lineRight = new(x2, handleBottom, x2, handleTop);
        LineStyle.Render(rp, lineBody);
        LineStyle.Render(rp, lineLeft);
        LineStyle.Render(rp, lineRight);
    }

    /// <summary>Contains line geometry constants.</summary>
    private static class Constants
    {
        /// <summary>The default width of the line in pixels.</summary>
        public const float LineWidth = 2;

        /// <summary>The default endpoint handle length in pixels.</summary>
        public const float DefaultHandleLength = 10;

        /// <summary>The divisor used to calculate half a handle length.</summary>
        public const float Half = 2;
    }
}
