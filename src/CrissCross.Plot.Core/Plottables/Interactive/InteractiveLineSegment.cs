// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables.Interactive;

/// <summary>Renders a straight line segment with interactive endpoint handles.</summary>
public class InteractiveLineSegment : IPlottable, IHasInteractiveHandles
{
    /// <summary>Identifies one endpoint of the line.</summary>
    private enum Node
    {
        /// <summary>The first endpoint.</summary>
        Point1,

        /// <summary>The second endpoint.</summary>
        Point2
    }

    /// <summary>Gets or sets whether the line segment is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position the segment.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the cursor displayed over an endpoint.</summary>
    public Cursor Cursor { get; set; } = Cursor.Hand;

    /// <summary>Gets the line style used to render the segment.</summary>
    public LineStyle LineStyle { get; } = new(Constants.LineWidth);

    /// <summary>Gets or sets the coordinates of the line segment.</summary>
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

    /// <summary>Gets the legend items for this line segment.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets the mutable coordinate line backing the public line property.</summary>
    private MutableCoordinateLine MutableLine { get; } = new();

    /// <summary>Gets the axis limits occupied by this segment.</summary>
    /// <returns>The axis limits of the line coordinates.</returns>
    public AxisLimits GetAxisLimits() => MutableLine.AxisLimits;

    /// <summary>Gets an interactive handle when an endpoint lies in the specified rectangle.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns>The matching endpoint handle, or <see langword="null"/>.</returns>
    public InteractiveHandle? GetHandle(CoordinateRect rect)
    {
        if (!IsVisible)
        {
            return null;
        }

        if (rect.Contains(MutableLine.Point1))
        {
            return new(this, Cursor);
        }

        return rect.Contains(MutableLine.Point2) ? new InteractiveHandle(this, Cursor, (int)Node.Point2) : null;
    }

    /// <summary>Performs any required work when an endpoint handle is pressed.</summary>
    /// <param name="handle">The pressed handle.</param>
    public virtual void PressHandle(InteractiveHandle handle) { }

    /// <summary>Performs any required work when an endpoint handle is released.</summary>
    /// <param name="handle">The released handle.</param>
    public virtual void ReleaseHandle(InteractiveHandle handle) { }

    /// <summary>Moves an endpoint handle to the specified coordinate.</summary>
    /// <param name="handle">The handle being moved.</param>
    /// <param name="point">The new coordinate of the handle.</param>
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

    /// <summary>Renders the line segment and endpoint markers.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        PixelLine pixelLine = Axes.GetPixelLine(MutableLine.CoordinateLine);
        LineStyle.Render(rp.Canvas, pixelLine, rp.Paint);
        StartMarkerStyle.Render(rp.Canvas, pixelLine.Pixel1, rp.Paint);
        EndMarkerStyle.Render(rp.Canvas, pixelLine.Pixel2, rp.Paint);
    }

    /// <summary>Contains default values used to render the line segment.</summary>
    private static class Constants
    {
        /// <summary>The default line width in pixels.</summary>
        public const float LineWidth = 2;

        /// <summary>The default endpoint marker size in pixels.</summary>
        public const float MarkerSize = 8;
    }
}
