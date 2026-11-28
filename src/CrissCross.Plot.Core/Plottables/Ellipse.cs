// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders an ellipse, elliptical sector, or annulus.</summary>
public class Ellipse : IPlottable, IHasLine, IHasFill, IHasLegendText
{
    /// <summary>Gets or sets the center coordinate of the ellipse.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Retained public field for source and binary compatibility.")]
    public Coordinates Center = Coordinates.Origin;

    /// <summary>Gets or sets whether this ellipse is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this ellipse.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this ellipse.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, LineStyle);

    /// <summary>Gets or sets the line style used to outline the ellipse.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the fill style used to draw the ellipse interior.</summary>
    public FillStyle FillStyle { get; set; } = new() { Color = Colors.Transparent };

    /// <summary>Gets or sets the fill color.</summary>
    public Color FillColor { get => FillStyle.Color; set => FillStyle.Color = value; }

    /// <summary>Gets or sets the fill hatch color.</summary>
    public Color FillHatchColor { get => FillStyle.HatchColor; set => FillStyle.HatchColor = value; }

    /// <summary>Gets or sets the fill hatch.</summary>
    public IHatch? FillHatch { get => FillStyle.Hatch; set => FillStyle.Hatch = value; }

    /// <summary>Gets or sets the legacy legend label.</summary>
    [Obsolete("use LegendText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete alias for source compatibility.")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text shown for this ellipse in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets the horizontal radius in axis units.</summary>
    public double RadiusX
    {
        get => field;
        set => field = Math.Abs(value);
    }
    = 10;

    /// <summary>Gets or sets the vertical radius in axis units.</summary>
    public double RadiusY
    {
        get => field;
        set => field = Math.Abs(value);
    }
    = 10;

    /// <summary>Gets or sets the horizontal inner annular radius in axis units.</summary>
    public double InnerRadiusX { get; set; }

    /// <summary>Gets or sets the vertical inner annular radius in axis units.</summary>
    public double InnerRadiusY { get; set; }

    /// <summary>Gets or sets the rotation angle of the ellipse.</summary>
    public Angle Rotation { get; set; } = Angle.FromDegrees(0);

    /// <summary>Gets whether both inner radii are greater than zero.</summary>
    public bool IsAnnulus => InnerRadiusX > 0 && InnerRadiusY > 0;

    /// <summary>Gets or sets whether the ellipse is rendered as a sector rather than an arc.</summary>
    public bool IsSector { get; set; } = true;

    /// <summary>Gets or sets the start angle of the arc or sector.</summary>
    public Angle StartAngle { get; set; } = Angle.FromDegrees(0);

    /// <summary>Gets or sets the sweep angle of the arc or sector.</summary>
    public Angle SweepAngle { get; set; } = Angle.FromDegrees(Constants.FullTurnDegrees);

    /// <summary>Gets the axis limits occupied by the ellipse.</summary>
    /// <returns>The axis-aligned bounds of the ellipse.</returns>
    public AxisLimits GetAxisLimits()
    {
        if (Rotation.Normalized.Degrees == 0)
        {
            return new(Center.ToRect(RadiusX, RadiusY));
        }

        double radians = -Rotation.Normalized.Radians;
        double cosineSquared = Math.Pow(Math.Cos(radians), Constants.SquarePower);
        double sineSquared = Math.Pow(Math.Sin(radians), Constants.SquarePower);
        double radiusXSquared = Math.Pow(RadiusX, Constants.SquarePower);
        double radiusYSquared = Math.Pow(RadiusY, Constants.SquarePower);
        double x = Math.Sqrt((radiusXSquared * cosineSquared) + (radiusYSquared * sineSquared));
        double y = Math.Sqrt((radiusXSquared * sineSquared) + (radiusYSquared * cosineSquared));
        return new(Center.ToRect(x, y));
    }

    /// <summary>Renders the ellipse, sector, or annulus.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible || !Constants.IsFinite(RadiusX) || !Constants.IsFinite(RadiusY))
        {
            return;
        }

        rp.Canvas.Translate(Axes.GetPixel(Center).ToSKPoint());
        rp.Canvas.RotateDegrees((float)-Rotation.Normalized.Degrees);

        float rx = Axes.GetPixelX(RadiusX) - Axes.GetPixelX(0);
        float ry = Axes.GetPixelY(RadiusY) - Axes.GetPixelY(0);
        PixelRect rect = new(-rx, rx, ry, -ry);

        if (IsAnnulus)
        {
            RenderAnnulus(rp, rp.Paint, rect);
        }
        else
        {
            RenderEllipse(rp, rp.Paint, rect);
        }
    }

    /// <summary>Renders the filled and outlined annulus.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="rect">The outer pixel bounds.</param>
    protected virtual void RenderAnnulus(RenderPack rp, Paint paint, PixelRect rect)
    {
        float innerRx = Axes.GetPixelX(InnerRadiusX) - Axes.GetPixelX(0);
        float innerRy = Axes.GetPixelY(InnerRadiusY) - Axes.GetPixelY(0);
        PixelRect innerRect = new(-innerRx, innerRx, innerRy, -innerRy);
        if (SweepAngle.Normalized.Degrees == 0
            || Math.Abs(SweepAngle.Degrees) >= Constants.FullTurnDegrees)
        {
            Drawing.FillEllipticalAnnulus(rp.Canvas, paint, FillStyle, rect, innerRect);
            Drawing.DrawEllipticalAnnulus(rp.Canvas, paint, LineStyle, rect, innerRect);
        }
        else
        {
            double startAngle = StartAngle.Normalized.Degrees;
            Drawing.FillAnnularSector(rp.Canvas, paint, FillStyle, rect, innerRect, (float)-startAngle, (float)-SweepAngle.Degrees);
            Drawing.DrawAnnularSector(rp.Canvas, paint, LineStyle, rect, innerRect, (float)-startAngle, (float)-SweepAngle.Degrees);
        }
    }

    /// <summary>Renders the filled and outlined ellipse or sector.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    /// <param name="rect">The pixel bounds.</param>
    protected virtual void RenderEllipse(RenderPack rp, Paint paint, PixelRect rect)
    {
        if (SweepAngle.Normalized.Degrees == 0
            || Math.Abs(SweepAngle.Degrees) >= Constants.FullTurnDegrees)
        {
            Drawing.FillOval(rp.Canvas, paint, FillStyle, rect);
            Drawing.DrawOval(rp.Canvas, paint, LineStyle, rect);
        }
        else
        {
            double startAngle = StartAngle.Normalized.Degrees;
            if (IsSector)
            {
                Drawing.FillSector(rp.Canvas, paint, FillStyle, rect, (float)-startAngle, (float)-SweepAngle.Degrees);
                Drawing.DrawSector(rp.Canvas, paint, LineStyle, rect, (float)-startAngle, (float)-SweepAngle.Degrees);
            }
            else
            {
                Drawing.DrawEllipticalArc(rp.Canvas, paint, LineStyle, rect, (float)-startAngle, (float)-SweepAngle.Degrees);
            }
        }
    }

    /// <summary>Stores mathematical constants used by the ellipse implementation.</summary>
    private static class Constants
    {
        /// <summary>The number of degrees in one full turn.</summary>
        public const int FullTurnDegrees = 360;

        /// <summary>The exponent used when squaring a value.</summary>
        public const int SquarePower = 2;

        /// <summary>Determines whether a number is finite.</summary>
        /// <param name="value">The value to inspect.</param>
        /// <returns><see langword="true"/> when finite; otherwise, <see langword="false"/>.</returns>
        public static bool IsFinite(double value) => !(double.IsInfinity(value) || double.IsNaN(value));
    }
}
