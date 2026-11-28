// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables.Interactive;

/// <summary>Renders a horizontal line with an interactive handle.</summary>
public class InteractiveHorizontalLine : IPlottable, IHasInteractiveHandles
{
    /// <summary>Gets or sets whether the line is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and position the line.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this line.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets the cursor displayed over the interactive handle.</summary>
    public Cursor Cursor { get; set; } = Cursor.SizeNorthSouth;

    /// <summary>Gets the line style used to render the line.</summary>
    public LineStyle LineStyle { get; } = new(Constants.LineWidth, Colors.Black);

    /// <summary>Gets or sets the y-coordinate of the line.</summary>
    public double Y { get; set; }

    /// <summary>Gets the axis limits occupied by the line.</summary>
    /// <returns>The vertical limits at the line's y-coordinate.</returns>
    public AxisLimits GetAxisLimits() => AxisLimits.VerticalOnly(Y, Y);

    /// <summary>Gets the interactive handle when the line is visible within the specified rectangle.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns>The line's handle when it is visible and within the rectangle; otherwise, <see langword="null"/>.</returns>
    public InteractiveHandle? GetHandle(CoordinateRect rect) =>
        IsVisible && rect.ContainsY(Y) ? new InteractiveHandle(this, Cursor) : null;

    /// <summary>Moves the line handle to the specified coordinate.</summary>
    /// <param name="handle">The handle being moved.</param>
    /// <param name="point">The new coordinate of the handle.</param>
    public virtual void MoveHandle(InteractiveHandle handle, Coordinates point) => Y = point.Y;

    /// <summary>Performs any required work when the line handle is pressed.</summary>
    /// <param name="handle">The pressed handle.</param>
    public virtual void PressHandle(InteractiveHandle handle) { }

    /// <summary>Performs any required work when the line handle is released.</summary>
    /// <param name="handle">The released handle.</param>
    public virtual void ReleaseHandle(InteractiveHandle handle) { }

    /// <summary>Renders the interactive horizontal line.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        float y = Axes.GetPixelY(Y);
        PixelLine line = new(rp.DataRect.Left, y, rp.DataRect.Right, y);
        LineStyle.Render(rp.Canvas, line, rp.Paint);
    }

    /// <summary>Contains default values used by the interactive line.</summary>
    private static class Constants
    {
        /// <summary>The width of the line in pixels.</summary>
        public const float LineWidth = 2;
    }
}
