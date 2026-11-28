// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Draws a rectangle with configurable fill, outline, and legend text.</summary>
public class Rectangle : IPlottable, IHasLine, IHasFill, IHasLegendText
{
    /// <summary>Gets or sets whether this rectangle is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position and render this rectangle.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend item for this rectangle.</summary>
    public IEnumerable<LegendItem> LegendItems => [new LegendItem() { LabelText = LegendText, FillStyle = FillStyle, OutlineStyle = LineStyle, }];

    /// <summary>Gets or sets the rectangle outline style.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = Constants.DefaultLineWidth };

    /// <summary>Gets or sets the rectangle outline width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the rectangle outline pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the rectangle outline color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the rectangle fill style.</summary>
    public FillStyle FillStyle { get; set; } = new();

    /// <summary>Gets or sets the rectangle fill color.</summary>
    public Color FillColor { get => FillStyle.Color; set => FillStyle.Color = value; }

    /// <summary>Gets or sets the rectangle hatch color.</summary>
    public Color FillHatchColor { get => FillStyle.HatchColor; set => FillStyle.HatchColor = value; }

    /// <summary>Gets or sets the rectangle hatch pattern.</summary>
    public IHatch? FillHatch { get => FillStyle.Hatch; set => FillStyle.Hatch = value; }

    /// <summary>Gets or sets the obsolete alias for <see cref="LegendText"/>.</summary>
    [Obsolete("use LegendText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained to preserve the obsolete public API; callers should use LegendText.")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text displayed for this rectangle in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets the first horizontal coordinate.</summary>
    public double X1 { get; set; }

    /// <summary>Gets or sets the second horizontal coordinate.</summary>
    public double X2 { get; set; }

    /// <summary>Gets or sets the first vertical coordinate.</summary>
    public double Y1 { get; set; }

    /// <summary>Gets or sets the second vertical coordinate.</summary>
    public double Y2 { get; set; }

    /// <summary>Gets or sets the normalized coordinate rectangle occupied by this shape.</summary>
    public CoordinateRect CoordinateRect
    {
        get
        {
            double left = Math.Min(X1, X2);
            double right = Math.Max(X1, X2);
            double bottom = Math.Min(Y1, Y2);
            double top = Math.Max(Y1, Y2);
            return new(left, right, bottom, top);
        }
        set
        {
            X1 = value.Left;
            X2 = value.Right;
            Y1 = value.Bottom;
            Y2 = value.Top;
        }
    }

    /// <summary>Gets the axis limits occupied by this rectangle.</summary>
    /// <returns>The axis limits of the rectangle.</returns>
    public AxisLimits GetAxisLimits() => new(CoordinateRect);

    /// <summary>Renders this rectangle.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        PixelRect rect = Axes.GetPixelRect(CoordinateRect);
        Drawing.FillRectangle(rp.Canvas, rect, rp.Paint, FillStyle);
        Drawing.DrawRectangle(rp.Canvas, rect, rp.Paint, LineStyle);
    }

    /// <summary>Contains constants used by rectangle plottables.</summary>
    internal static class Constants
    {
        /// <summary>The default outline width in pixels.</summary>
        internal const float DefaultLineWidth = 1;
    }
}
