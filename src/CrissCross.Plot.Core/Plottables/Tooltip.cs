// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>
/// A tooltip displays a text bubble pointing to a specific location in X/Y space.
/// The position of the bubble moves according to the axis limits to best display the text in the data area.
/// </summary>
public class Tooltip : LabelStyleProperties, IPlottable, IHasLine, IHasFill
{
    /// <summary>Gets or sets location in coordinate space where the text of the tooltip will be displayed.</summary>
    public Coordinates LabelLocation { get; set; }

    /// <summary>Gets or sets location in coordinate space of the tip of the tail.</summary>
    public Coordinates TipLocation { get; set; }

    /// <summary>Gets or sets fractional size of the tail where it attaches to the tooltip body.</summary>
    public double TailWidthPercentage { get; set; } = 0.5;

    /// <summary>Gets or sets distance (in pixels) of empty space between all sides of the label and the edge of the tooltip body.</summary>
    public float Padding { get; set; } = 5;

    /// <summary>Gets or sets the style of the tooltip label.</summary>
    public override LabelStyle LabelStyle { get; set; } = new() { FontSize = Constants.DefaultFontSize };

    /// <summary>Gets or sets the tooltip outline style.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = Constants.DefaultLineWidth };

    /// <summary>Gets or sets the tooltip outline width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the tooltip outline pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the tooltip outline color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the tooltip fill style.</summary>
    public FillStyle FillStyle { get; set; } = new();

    /// <summary>Gets or sets the tooltip fill color.</summary>
    public Color FillColor { get => FillStyle.Color; set => FillStyle.Color = value; }

    /// <summary>Gets or sets the tooltip hatch color.</summary>
    public Color FillHatchColor { get => FillStyle.HatchColor; set => FillStyle.HatchColor = value; }

    /// <summary>Gets or sets the tooltip hatch pattern.</summary>
    public IHatch? FillHatch { get => FillStyle.Hatch; set => FillStyle.Hatch = value; }

    /// <summary>Gets or sets whether this tooltip is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position this tooltip.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this tooltip.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets the axis limits occupied by this tooltip and its tail.</summary>
    /// <returns>The axis limits of the tooltip and its tail.</returns>
    public AxisLimits GetAxisLimits() => new AxisLimits(LabelLocation)
            .Expanded(TipLocation);

    /// <summary>Renders this tooltip and its tail.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        MeasuredText measured = LabelStyle.Measure(LabelStyle.Text, rp.Paint);
        PixelRect bubbleBodyRect = measured.Rect(LabelAlignment)
            .Expand(new PixelPadding(
                Padding + LabelPixelPadding.Left,
                Padding + LabelPixelPadding.Right,
                Padding + LabelPixelPadding.Bottom,
                Padding + LabelPixelPadding.Top));

        Pixel px = Axes.GetPixel(LabelLocation);
        bubbleBodyRect = bubbleBodyRect.WithOffset(new(px.X, px.Y));

        using SKRoundRect roundedBody = new(bubbleBodyRect.ToSKRect(), Padding);
        using SKPathBuilder bodyBuilder = new();
        bodyBuilder.AddRoundRect(roundedBody);
        using SKPath bubbleBodyPath = bodyBuilder.Detach();

        var tailPoint = Axes.GetPixel(TipLocation).ToSKPoint();
        float tailWidth = Math.Min(bubbleBodyRect.Width, bubbleBodyRect.Height)
            * (float)NumericConversion.Clamp(TailWidthPercentage, 0, 1);

        SKPath getTooltipShapePath()
        {
            var bubbleCenter = bubbleBodyRect.Center.ToSKPoint();
            if (tailPoint == bubbleCenter)
            {
                return new(bubbleBodyPath);
            }

            (SKPoint tailBase1, SKPoint tailBase2) =
                GetTailBasePoints(tailPoint, bubbleCenter, tailWidth);
            using SKPathBuilder tailBuilder = new();
            tailBuilder.AddPoly([tailPoint, tailBase1, tailBase2]);
            using SKPath bubbleTailPath = tailBuilder.Detach();

            return bubbleBodyPath.Op(bubbleTailPath, SKPathOp.Union);
        }

        using SKPath path = getTooltipShapePath();
        Drawing.FillPath(rp.Canvas, rp.Paint, path, FillStyle);
        Drawing.DrawPath(rp.Canvas, rp.Paint, path, LineStyle);
        LabelStyle.Render(rp.Canvas, px, rp.Paint, LabelText);
    }

    /// <summary>Calculates the two base points of the tooltip tail.</summary>
    /// <param name="tailTip">The tip of the tail.</param>
    /// <param name="bodyCenter">The center of the tooltip body.</param>
    /// <param name="length">The width of the tail base.</param>
    /// <returns>The two tail base points.</returns>
    private static (SKPoint firstBasePoint, SKPoint secondBasePoint) GetTailBasePoints(SKPoint tailTip, SKPoint bodyCenter, float length)
    {
        SKPoint direction = bodyCenter - tailTip;
        float unitX = -direction.Y / direction.Length;
        float unitY = direction.X / direction.Length;
        float halfLength = length / Constants.HalfTailWidthDivisor;
        return (new(bodyCenter.X + (halfLength * unitX), bodyCenter.Y + (halfLength * unitY)),
                new(bodyCenter.X - (halfLength * unitX), bodyCenter.Y - (halfLength * unitY)));
    }

    /// <summary>Contains constants used by tooltips.</summary>
    internal static class Constants
    {
        /// <summary>The default label font size in pixels.</summary>
        internal const float DefaultFontSize = 14;

        /// <summary>The default outline width in pixels.</summary>
        internal const float DefaultLineWidth = 1;

        /// <summary>The divisor used to calculate half of the tail width.</summary>
        internal const float HalfTailWidthDivisor = 2;
    }
}
