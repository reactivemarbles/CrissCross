// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Holds values and styles for drawing a box-and-whisker symbol.</summary>
public class Box : IHasFill, IHasLine
{
    /// <summary>Gets or sets the position of the box along its orientation axis.</summary>
    public double Position { get; set; }

    /// <summary>Gets or sets the minimum value of the box.</summary>
    public double BoxMin { get; set; }

    /// <summary>Gets or sets the maximum value of the box.</summary>
    public double BoxMax { get; set; }

    /// <summary>Gets or sets the optional middle value drawn inside the box.</summary>
    public double? BoxMiddle { get; set; }

    /// <summary>Gets or sets the optional minimum whisker value.</summary>
    public double? WhiskerMin { get; set; }

    /// <summary>Gets or sets the optional maximum whisker value.</summary>
    public double? WhiskerMax { get; set; }

    /// <summary>Gets or sets the width of the box.</summary>
    public double Width { get; set; } = 0.8;

    /// <summary>Gets or sets the whisker width as a fraction of the box width.</summary>
    public double WhiskerSizeFraction { get; set; } = 0.5;

    /// <summary>Gets the whisker width in coordinate units.</summary>
    public double WhiskerSize => Width * WhiskerSizeFraction;

    /// <summary>Gets or sets the orientation of the box.</summary>
    public Orientation Orientation { get; set; } = Orientation.Vertical;

    /// <summary>Gets or sets whether the box is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the fill style.</summary>
    public FillStyle FillStyle { get; set; } = new();

    /// <summary>Gets or sets the fill color.</summary>
    public Color FillColor { get => FillStyle.Color; set => FillStyle.Color = value; }

    /// <summary>Gets or sets the fill hatch color.</summary>
    public Color FillHatchColor { get => FillStyle.HatchColor; set => FillStyle.HatchColor = value; }

    /// <summary>Gets or sets the fill hatch.</summary>
    public IHatch? FillHatch { get => FillStyle.Hatch; set => FillStyle.Hatch = value; }

    /// <summary>Gets or sets the line style.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the obsolete fill-style alias.</summary>
    [Obsolete("use FillColor or FillStyle")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained as an obsolete public alias to preserve API compatibility.")]
    public FillStyle Fill { get => FillStyle; set => FillStyle = value; }

    /// <summary>Gets or sets the obsolete line-style alias.</summary>
    [Obsolete("use LineWidth, LineColor, or LineStyle")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained as an obsolete public alias to preserve API compatibility.")]
    public LineStyle Stroke { get => LineStyle; set => LineStyle = value; }

    /// <summary>Gets the axis limits occupied by this box and its whiskers.</summary>
    /// <returns>The axis limits of the box.</returns>
    public AxisLimits GetAxisLimits()
    {
        double minimumHorizontal = Position - (Width * Constants.Half);
        double maximumHorizontal = Position + (Width * Constants.Half);
        double minimumVertical = WhiskerMin.HasValue ? Math.Min(BoxMin, WhiskerMin.Value) : BoxMin;
        double maximumVertical = WhiskerMax.HasValue ? Math.Max(BoxMin, WhiskerMax.Value) : BoxMax;
        return new(minimumHorizontal, maximumHorizontal, minimumVertical, maximumVertical);
    }

    /// <summary>Renders this box-and-whisker symbol.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="axes">The axes used to convert coordinates to pixels.</param>
    public void Render(RenderPack rp, IAxes axes)
    {
        if (!IsVisible)
        {
            return;
        }

        Paint paint = rp.Paint;
        if (Orientation != Orientation.Vertical)
        {
            throw new NotSupportedException("Only vertical boxes are supported.");
        }

        CoordinateRect bodyRect = new(Position - (Width * Constants.Half), Position + (Width * Constants.Half), BoxMin, BoxMax);
        PixelRect bodyRectPx = axes.GetPixelRect(bodyRect);
        Drawing.FillRectangle(rp.Canvas, bodyRectPx, paint, FillStyle);
        Drawing.DrawRectangle(rp.Canvas, bodyRectPx, paint, LineStyle);

        if (BoxMiddle.HasValue)
        {
            CoordinateLine lineMid = new(bodyRect.Left, BoxMiddle.Value, bodyRect.Right, BoxMiddle.Value);
            PixelLine lineMidPx = axes.GetPixelLine(lineMid);
            LineStyle.Render(rp.Canvas, lineMidPx, paint);
        }

        if (WhiskerMax.HasValue)
        {
            CoordinateLine lineMax = new(Position, BoxMax, Position, WhiskerMax.Value);
            PixelLine lineMaxPx = axes.GetPixelLine(lineMax);
            LineStyle.Render(rp.Canvas, lineMaxPx, paint);

            CoordinateLine lineMaxAcross = new(Position - (WhiskerSize * Constants.Half), WhiskerMax.Value, Position + (WhiskerSize * Constants.Half), WhiskerMax.Value);
            PixelLine lineMaxAcrossPx = axes.GetPixelLine(lineMaxAcross);
            LineStyle.Render(rp.Canvas, lineMaxAcrossPx, paint);
        }

        if (!WhiskerMin.HasValue)
        {
            return;
        }

        CoordinateLine lineMin = new(Position, BoxMin, Position, WhiskerMin.Value);
        PixelLine lineMinPx = axes.GetPixelLine(lineMin);
        LineStyle.Render(rp.Canvas, lineMinPx, paint);

        CoordinateLine lineMinAcross = new(Position - (WhiskerSize * Constants.Half), WhiskerMin.Value, Position + (WhiskerSize * Constants.Half), WhiskerMin.Value);
        PixelLine lineMinAcrossPx = axes.GetPixelLine(lineMinAcross);
        LineStyle.Render(rp.Canvas, lineMinAcrossPx, paint);
    }

    /// <summary>Contains constants used when rendering boxes.</summary>
    private static class Constants
    {
        /// <summary>The fraction used to calculate half of a width.</summary>
        internal const double Half = 0.5;
    }
}
