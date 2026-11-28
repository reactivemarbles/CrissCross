// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables.Interactive;

/// <summary>Renders a vertical line segment with draggable endpoint and body handles.</summary>
public class InteractiveVerticalLineSegment : IPlottable, IHasInteractiveHandles
{
    /// <summary>Identifies the part of the line selected by an interactive handle.</summary>
    private enum Handle
    {
        /// <summary>The first endpoint.</summary>
        Edge1,

        /// <summary>The body of the segment.</summary>
        Body,

        /// <summary>The second endpoint.</summary>
        Edge2
    }

    /// <summary>Gets or sets whether the line segment is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position the segment.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this segment.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets the line style used to render the segment and handles.</summary>
    public LineStyle LineStyle { get; } = new(Constants.LineWidth, Colors.Black);

    /// <summary>Gets or sets the length of the endpoint handles in pixels.</summary>
    public float HandleLength { get; set; } = Constants.DefaultHandleLength;

    /// <summary>Gets or sets the x-coordinate of the segment.</summary>
    public double X { get; set; }

    /// <summary>Gets or sets the first y-coordinate of the segment.</summary>
    public double Y1 { get; set; }

    /// <summary>Gets or sets the second y-coordinate of the segment.</summary>
    public double Y2 { get; set; }

    /// <summary>Gets the lesser y-coordinate of the endpoints.</summary>
    public double YMin => Math.Min(Y1, Y2);

    /// <summary>Gets the greater y-coordinate of the endpoints.</summary>
    public double YMax => Math.Max(Y1, Y2);

    /// <summary>Gets the axis limits occupied by this segment.</summary>
    /// <returns>The horizontal and vertical ranges of the segment.</returns>
    public AxisLimits GetAxisLimits() => new(X, X, YMin, YMax);

    /// <summary>Gets an interactive handle when the specified rectangle intersects the segment.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns>The matching handle, or <see langword="null"/> when no handle is hit.</returns>
    public InteractiveHandle? GetHandle(CoordinateRect rect)
    {
        if (!IsVisible || !rect.ContainsX(X))
        {
            return null;
        }

        if (rect.ContainsY(Y1))
        {
            return new(this, Cursor.SizeNorthSouth);
        }

        if (rect.ContainsY(Y2))
        {
            return new(this, Cursor.SizeNorthSouth, (int)Handle.Edge2);
        }

        return rect.Center.Y >= YMin && rect.Center.Y <= YMax ? new InteractiveHandle(this, Cursor.SizeWestEast, (int)Handle.Body) : null;
    }

    /// <summary>Moves the selected line handle to the specified coordinate.</summary>
    /// <param name="handle">The handle being moved.</param>
    /// <param name="point">The new coordinate of the handle.</param>
    public virtual void MoveHandle(InteractiveHandle handle, Coordinates point)
    {
        if (handle.Index == (int)Handle.Edge1)
        {
            Y1 = point.Y;
        }
        else if (handle.Index == (int)Handle.Edge2)
        {
            Y2 = point.Y;
        }
        else if (handle.Index == (int)Handle.Body)
        {
            X = point.X;
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

        float x = Axes.GetPixelX(X);
        float leftHandleX = x - (HandleLength / Constants.Half);
        float rightHandleX = x + (HandleLength / Constants.Half);
        float y1 = Axes.GetPixelY(YMin);
        float y2 = Axes.GetPixelY(YMax);
        PixelLine lineBody = new(x, y1, x, y2);
        PixelLine lineLeft = new(leftHandleX, y1, rightHandleX, y1);
        PixelLine lineRight = new(leftHandleX, y2, rightHandleX, y2);
        LineStyle.Render(rp, lineBody);
        LineStyle.Render(rp, lineLeft);
        LineStyle.Render(rp, lineRight);
    }

    /// <summary>Contains line geometry constants.</summary>
    private static class Constants
    {
        /// <summary>The default line width in pixels.</summary>
        public const float LineWidth = 2;

        /// <summary>The default endpoint handle length in pixels.</summary>
        public const float DefaultHandleLength = 10;

        /// <summary>The divisor used to calculate half a handle length.</summary>
        public const float Half = 2;
    }
}
