// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables.Interactive;

/// <summary>Renders a marker that can be moved through an interactive handle.</summary>
public class InteractiveMarker : IPlottable, IHasInteractiveHandles
{
    /// <summary>Gets or sets the x-coordinate of the marker.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Retained public field for source and binary compatibility.")]
    public double X;

    /// <summary>Gets or sets the y-coordinate of the marker.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Retained public field for source and binary compatibility.")]
    public double Y;

    /// <summary>Gets or sets whether the marker is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position the marker.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the cursor displayed over the marker handle.</summary>
    public Cursor Cursor { get; set; } = Cursor.Hand;

    /// <summary>Gets or sets the marker style.</summary>
    public MarkerStyle MarkerStyle { get; set; } = new(MarkerShape.FilledCircle, Constants.MarkerSize, Colors.Black);

    /// <summary>Gets the legend items for this marker.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets the marker coordinate.</summary>
    public Coordinates Point
    {
        get => new(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    /// <summary>Gets the axis limits occupied by the marker.</summary>
    /// <returns>The axis limits of the marker coordinate.</returns>
    public AxisLimits GetAxisLimits() => new(Point);

    /// <summary>Gets an interactive handle when the specified rectangle contains the marker.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns>The marker handle when the marker is contained; otherwise, <see langword="null"/>.</returns>
    public InteractiveHandle? GetHandle(CoordinateRect rect) =>
        IsVisible && rect.Contains(Point) ? new InteractiveHandle(this, Cursor) : null;

    /// <summary>Moves the marker handle to the specified coordinate.</summary>
    /// <param name="handle">The marker handle being moved.</param>
    /// <param name="point">The new coordinate of the marker.</param>
    public virtual void MoveHandle(InteractiveHandle handle, Coordinates point) => Point = point;

    /// <summary>Performs any required work when the marker handle is pressed.</summary>
    /// <param name="handle">The pressed handle.</param>
    public virtual void PressHandle(InteractiveHandle handle) { }

    /// <summary>Performs any required work when the marker handle is released.</summary>
    /// <param name="handle">The released handle.</param>
    public virtual void ReleaseHandle(InteractiveHandle handle) { }

    /// <summary>Renders the marker.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        Pixel markerPixel = Axes.GetPixel(Point);
        MarkerStyle.Render(rp, markerPixel);
    }

    /// <summary>Contains default values used to render the interactive marker.</summary>
    private static class Constants
    {
        /// <summary>The default marker size in pixels.</summary>
        public const float MarkerSize = 15;
    }
}
