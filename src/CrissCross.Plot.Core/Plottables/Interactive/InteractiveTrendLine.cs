// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables.Interactive;

/// <summary>An infinite line defined by two points that extends infinitely in both directions.</summary>
public class InteractiveTrendLine : IPlottable, IHasInteractiveHandles
{
    /// <summary>Identifies one endpoint of the trend line.</summary>
    private enum Node
    {
        /// <summary>The first endpoint.</summary>
        Point1,

        /// <summary>The second endpoint.</summary>
        Point2
    }

    /// <summary>Gets or sets whether the trend line is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position the trend line.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the cursor displayed over the interactive handle.</summary>
    public Cursor Cursor { get; set; } = Cursor.Hand;

    /// <summary>Gets the line style used to render the trend line.</summary>
    public LineStyle LineStyle { get; } = new(Constants.LineWidth);

    /// <summary>Gets or sets the coordinates of the trend line.</summary>
    public CoordinateLine Line
    {
        get => MutableLine.CoordinateLine;
        set => MutableLine.Update(value);
    }

    /// <summary>Gets the marker style for the first endpoint.</summary>
    public MarkerStyle StartMarkerStyle { get; } = new(MarkerShape.FilledCircle, Constants.MarkerSize);

    /// <summary>Gets the marker style for the second endpoint.</summary>
    public MarkerStyle EndMarkerStyle { get; } = new(MarkerShape.FilledCircle, Constants.MarkerSize);

    /// <summary>Gets or sets the line and endpoint marker color.</summary>
    public Color Color
    {
        get => LineStyle.Color;
        set
        {
            LineStyle.Color = value;
            StartMarkerStyle.FillColor = value;
            EndMarkerStyle.FillColor = value;
        }
    }

    /// <summary>Gets the legend items for this trend line.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets the mutable coordinate line backing the public line property.</summary>
    private MutableCoordinateLine MutableLine { get; } = new();

    /// <summary>Gets the axis limits occupied by the trend line.</summary>
    /// <returns>The axis limits of the line coordinates.</returns>
    public AxisLimits GetAxisLimits() => MutableLine.AxisLimits;

    /// <summary>Gets an interactive handle for a point on the trend line.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns>The matching handle, or <see langword="null"/> when the line does not intersect the rectangle.</returns>
    public InteractiveHandle? GetHandle(CoordinateRect rect)
    {
        if (!IsVisible)
        {
            return null;
        }

        // First check if the mouse is directly over one of the control points
        if (rect.Contains(MutableLine.Point1))
        {
            return new(this, Cursor);
        }

        if (rect.Contains(MutableLine.Point2))
        {
            return new(this, Cursor, (int)Node.Point2);
        }

        // Check if the infinite line passes through the rect. This allows the user to manipulate the line without
        //   having to see one of the 2 points on the screen - fairly necessary due to being an infinite line
        if (!LineIntersectsRect(rect))
        {
            return null;
        }

        // Determine which control point is closer to the mouse and return that point.
        Coordinates mousePos = rect.Center;
        double dist1 = mousePos.Distance(MutableLine.Point1);
        double dist2 = mousePos.Distance(MutableLine.Point2);

        return dist1 < dist2
            ? new InteractiveHandle(this, Cursor)
            : new InteractiveHandle(this, Cursor, (int)Node.Point2);
    }

    /// <summary>Performs any required work when a trend-line handle is pressed.</summary>
    /// <param name="handle">The pressed handle.</param>
    public virtual void PressHandle(InteractiveHandle handle) { }

    /// <summary>Performs any required work when a trend-line handle is released.</summary>
    /// <param name="handle">The released handle.</param>
    public virtual void ReleaseHandle(InteractiveHandle handle) { }

    /// <summary>Moves a trend-line endpoint to the specified coordinate.</summary>
    /// <param name="handle">The handle being moved.</param>
    /// <param name="point">The new endpoint coordinate.</param>
    public virtual void MoveHandle(InteractiveHandle handle, Coordinates point)
    {
        if (handle.Index == (int)Node.Point1)
        {
            MutableLine.Point1 = point;
        }

        if (handle.Index != (int)Node.Point2)
        {
            return;
        }

        MutableLine.Point2 = point;
    }

    /// <summary>Renders the trend line and its endpoint markers.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        PixelLine pixelLine = Axes.GetPixelLine(MutableLine.CoordinateLine);

        // Calculate the slope and extend the line to the edges of the data area.
        PixelLine extendedLine = GeometryHelpers.ExtendLineToEdges(pixelLine, rp.DataRect);

        LineStyle.Render(rp.Canvas, extendedLine, rp.Paint);
        StartMarkerStyle.Render(rp.Canvas, pixelLine.Pixel1, rp.Paint);
        EndMarkerStyle.Render(rp.Canvas, pixelLine.Pixel2, rp.Paint);
    }

    /// <summary>Determines whether the infinite line passes through or near the given rectangle.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns><see langword="true"/> when the line intersects the rectangle.</returns>
    private bool LineIntersectsRect(CoordinateRect rect)
    {
        CoordinateLine line = MutableLine.CoordinateLine;
        var deltaX = line.X2 - line.X1;
        if (Math.Abs(deltaX) < Constants.CoordinateTolerance)
        {
            return rect.ContainsX(line.X1);
        }

        var deltaY = line.Y2 - line.Y1;
        if (Math.Abs(deltaY) < Constants.CoordinateTolerance)
        {
            return rect.ContainsY(line.Y1);
        }

        var slope = deltaY / deltaX;
        var interceptY = line.Y1 - (slope * line.X1);
        var leftEdgeY = (slope * rect.Left) + interceptY;
        var rightEdgeY = (slope * rect.Right) + interceptY;
        if (rect.ContainsY(leftEdgeY) || rect.ContainsY(rightEdgeY))
        {
            return true;
        }

        return (leftEdgeY < rect.Bottom && rightEdgeY > rect.Top)
            || (leftEdgeY > rect.Top && rightEdgeY < rect.Bottom);
    }

    /// <summary>Contains static geometry calculations for trend-line rendering.</summary>
    internal static class GeometryHelpers
    {
        /// <summary>Finds the intersections of the line with the rectangle edges.</summary>
        /// <param name="dataRect">The rectangle to intersect.</param>
        /// <param name="leftEdgeY">The y-coordinate where the line meets the left edge.</param>
        /// <param name="rightEdgeY">The y-coordinate where the line meets the right edge.</param>
        /// <param name="topEdgeX">The x-coordinate where the line meets the top edge.</param>
        /// <param name="bottomEdgeX">The x-coordinate where the line meets the bottom edge.</param>
        /// <returns>The intersection points contained by the rectangle.</returns>
        internal static List<Pixel> GetIntersections(PixelRect dataRect, float leftEdgeY, float rightEdgeY, float topEdgeX, float bottomEdgeX)
        {
            List<Pixel> intersections = new();
            if (leftEdgeY >= dataRect.Top && leftEdgeY <= dataRect.Bottom)
            {
                intersections.Add(new(dataRect.Left, leftEdgeY));
            }

            if (rightEdgeY >= dataRect.Top && rightEdgeY <= dataRect.Bottom)
            {
                intersections.Add(new(dataRect.Right, rightEdgeY));
            }

            if (topEdgeX >= dataRect.Left && topEdgeX <= dataRect.Right)
            {
                intersections.Add(new(topEdgeX, dataRect.Top));
            }

            if (bottomEdgeX >= dataRect.Left && bottomEdgeX <= dataRect.Right)
            {
                intersections.Add(new(bottomEdgeX, dataRect.Bottom));
            }

            return intersections;
        }

        /// <summary>Extends a line segment to the edges of a pixel rectangle.</summary>
        /// <param name="line">The line to extend.</param>
        /// <param name="dataRect">The rectangle whose edges constrain the extended line.</param>
        /// <returns>The line segment extended to the rectangle edges.</returns>
        internal static PixelLine ExtendLineToEdges(PixelLine line, PixelRect dataRect)
        {
            float deltaX = line.X2 - line.X1;
            float deltaY = line.Y2 - line.Y1;

            if (Math.Abs(deltaX) < Constants.PixelTolerance)
            {
                return new(line.X1, dataRect.Top, line.X1, dataRect.Bottom);
            }

            if (Math.Abs(deltaY) < Constants.PixelTolerance)
            {
                return new(dataRect.Left, line.Y1, dataRect.Right, line.Y1);
            }

            float slope = deltaY / deltaX;
            float interceptY = line.Y1 - (slope * line.X1);
            float leftEdgeY = (slope * dataRect.Left) + interceptY;
            float rightEdgeY = (slope * dataRect.Right) + interceptY;
            float topEdgeX = (dataRect.Top - interceptY) / slope;
            float bottomEdgeX = (dataRect.Bottom - interceptY) / slope;
            List<Pixel> intersections = GetIntersections(dataRect, leftEdgeY, rightEdgeY, topEdgeX, bottomEdgeX);

            return intersections.Count >= Constants.MinimumIntersectionCount
                ? new PixelLine(intersections[0], intersections[1])
                : line;
        }
    }

    /// <summary>Contains rendering and hit-testing constants.</summary>
    private static class Constants
    {
        /// <summary>The default line width in pixels.</summary>
        public const float LineWidth = 2;

        /// <summary>The default endpoint marker size in pixels.</summary>
        public const float MarkerSize = 8;

        /// <summary>The minimum number of intersections required to extend the line.</summary>
        public const int MinimumIntersectionCount = 2;

        /// <summary>The tolerance used to detect a vertical or horizontal pixel line.</summary>
        public const float PixelTolerance = 0.001F;

        /// <summary>The tolerance used to detect a vertical or horizontal coordinate line.</summary>
        public const double CoordinateTolerance = 1e-10;
    }
}
