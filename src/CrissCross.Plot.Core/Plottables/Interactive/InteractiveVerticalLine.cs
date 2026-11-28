// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables.Interactive;

/// <summary>Renders a vertical line with an interactive handle and label.</summary>
public class InteractiveVerticalLine : LabelStyleProperties, IPlottable, IRenderLast, IHasInteractiveHandles, IHasLine, IHasLegendText
{
    /// <summary>Gets or sets whether the line is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position the line.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this line.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets the cursor displayed over the line handle.</summary>
    public Cursor Cursor { get; set; } = Cursor.SizeWestEast;

    /// <summary>Gets or sets the line style.</summary>
    public LineStyle LineStyle { get; set; } = new(Constants.LineWidth, Colors.Black);

    /// <summary>Gets or sets whether the label is positioned opposite the line.</summary>
    public bool LabelOppositeAxis { get; set; }

    /// <summary>Gets or sets the x-coordinate of the line.</summary>
    public double X { get; set; }

    /// <summary>Gets or sets the label style used by this line.</summary>
    public override LabelStyle LabelStyle { get; set; } = new() { ForeColor = Colors.White, Bold = true, Padding = Constants.LabelPadding };

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the text shown in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets the line label text.</summary>
    public string Text { get => LabelStyle.Text; set => LabelStyle.Text = value; }

    /// <summary>Gets the axis limits occupied by the line.</summary>
    /// <returns>The horizontal limits at the line's x-coordinate.</returns>
    public AxisLimits GetAxisLimits() => AxisLimits.HorizontalOnly(X, X);

    /// <summary>Gets an interactive handle when the specified rectangle contains the line.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns>The line handle when the rectangle contains the line; otherwise, <see langword="null"/>.</returns>
    public InteractiveHandle? GetHandle(CoordinateRect rect) =>
        IsVisible && rect.ContainsX(X) ? new InteractiveHandle(this, Cursor) : null;

    /// <summary>Moves the line handle to the specified coordinate.</summary>
    /// <param name="handle">The handle being moved.</param>
    /// <param name="point">The new coordinate of the line.</param>
    public virtual void MoveHandle(InteractiveHandle handle, Coordinates point) => X = point.X;

    /// <summary>Performs any required work when the line handle is pressed.</summary>
    /// <param name="handle">The pressed handle.</param>
    public virtual void PressHandle(InteractiveHandle handle) { }

    /// <summary>Performs any required work when the line handle is released.</summary>
    /// <param name="handle">The released handle.</param>
    public virtual void ReleaseHandle(InteractiveHandle handle) { }

    /// <summary>Renders the vertical line.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        float x = Axes.GetPixelX(X);
        PixelLine line = new(x, rp.DataRect.Bottom, x, rp.DataRect.Top);
        LineStyle.Render(rp.Canvas, line, rp.Paint);
    }

    /// <summary>Renders the line label after other plot content.</summary>
    /// <param name="rp">The render context.</param>
    public void RenderLast(RenderPack rp)
    {
        if (!LabelStyle.IsVisible || string.IsNullOrEmpty(LabelStyle.Text))
        {
            return;
        }

        float x = Axes.GetPixelX(X);
        if (!rp.DataRect.ContainsX(x))
        {
            return;
        }

        float y = LabelOppositeAxis
            ? rp.DataRect.Top - LabelStyle.PixelPadding.Top
            : rp.DataRect.Bottom + LabelStyle.PixelPadding.Bottom;
        LabelStyle.Alignment = LabelOppositeAxis
            ? Alignment.LowerCenter
            : Alignment.UpperCenter;
        rp.CanvasState.DisableClipping();
        LabelStyle.Render(rp.Canvas, new(x, y), rp.Paint);
    }

    /// <summary>Contains default values for vertical-line rendering.</summary>
    private static class Constants
    {
        /// <summary>The default width of the line in pixels.</summary>
        public const float LineWidth = 2;

        /// <summary>The default padding of the line label in pixels.</summary>
        public const float LabelPadding = 5;
    }
}
