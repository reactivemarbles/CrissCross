// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders a Smith chart with constant-resistance and constant-reactance circles.</summary>
public class SmithChartAxis : IPlottable, IManagesAxisLimits
{
    /// <summary>The default alpha channel for chart lines.</summary>
    private const double DefaultLineAlpha = 0.2;

    /// <summary>The divisor used to scale label padding.</summary>
    private const double LabelPaddingDivisor = 2;

    /// <summary>The vertical label offset in pixels.</summary>
    private const float LabelVerticalOffsetPixels = 3;

    /// <summary>The number of degrees in a half-turn.</summary>
    private const double HalfTurnDegrees = 180;

    /// <summary>The number of degrees in a quarter-turn.</summary>
    private const double QuarterTurnDegrees = 90;

    /// <summary>The number of degrees in a full turn.</summary>
    private const double FullTurnDegrees = 360;

    /// <summary>The divisor used to calculate the base of circle intersections.</summary>
    private const double CircleIntersectionDivisor = 2;

    /// <summary>The tolerance for identifying a full-turn sweep.</summary>
    private const double FullTurnComparisonTolerance = 1e-10;

    /// <summary>Gets or sets a value indicating whether the chart is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to transform chart coordinates.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this axis.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets the constant-resistance circles displayed on the chart.</summary>
    public List<RealTick> RealTicks { get; } = [];

    /// <summary>Gets the constant-reactance curves displayed on the chart.</summary>
    public List<ImaginaryTick> ImaginaryTicks { get; } = [];

    /// <summary>Gets or sets the clockwise rotation from the default position, where zero points right.</summary>
    public Angle Rotation { get; set; } = Angle.FromDegrees(0);

    /// <summary>Gets or sets whether to adjust axis limits so chart units remain square.</summary>
    public bool ManageAxisLimits { get; set; } = true;

    /// <summary>Gets or sets the default style of the constant-resistance curves extending from the chart boundary.</summary>
    public LineStyle RealLineStyle { get; set; } = new() { Width = 1, Color = Colors.Black.WithAlpha(DefaultLineAlpha), };

    /// <summary>Gets or sets the default style of the constant-reactance curves.</summary>
    public LineStyle Imaginary { get; set; } = new() { Width = 1, Color = Colors.Black.WithAlpha(DefaultLineAlpha), };

    /// <summary>Gets the style used to render axis labels.</summary>
    public LabelStyle LabelStyle { get; } = new();

    /// <summary>Gets or sets the label text associated with the chart axis.</summary>
    public string? LabelText { get; set; }

    /// <summary>Gets or sets the fraction of a circle radius used to offset labels.</summary>
    public double LabelPaddingFraction { get; set; } = 0.1;

    /// <summary>Converts normalized impedance to its reflection coefficient.</summary>
    /// <param name="normalizedImpedance">The normalized resistance and reactance.</param>
    /// <returns>The reflection coefficient in Cartesian coordinates.</returns>
    public static Coordinates CalculateGamma(Coordinates normalizedImpedance)
    {
        // Γ = (z - 1) / (z + 1)
        //   = (R + jX - 1) / (R + jX + 1)
        //   = a + jc
        double r = normalizedImpedance.X;
        double x = normalizedImpedance.Y;

        // Calculate the denominator (R + jX + 1)
        double denominatorReal = r + 1;
        double denominatorImaginary = x;

        // Magnitude squared of the denominator
        double denominatorMagnitudeSquared =
            (denominatorReal * denominatorReal) + (denominatorImaginary * denominatorImaginary);

        // Calculate the real and imaginary parts of Γ
        double a = (((r - 1) * denominatorReal) + (x * denominatorImaginary)) / denominatorMagnitudeSquared;
        double c = ((x * denominatorReal) - ((r - 1) * denominatorImaginary)) / denominatorMagnitudeSquared;

        return new(a, c);
    }

    /// <summary>Finds the intersections of two circles, if they intersect at two distinct points.</summary>
    /// <param name="pt1">The center of the first circle.</param>
    /// <param name="r1">The radius of the first circle.</param>
    /// <param name="pt2">The center of the second circle.</param>
    /// <param name="r2">The radius of the second circle.</param>
    /// <returns>The two intersection points, or an empty sequence when there are not two distinct intersections.</returns>
    public static IEnumerable<Coordinates> FindIntersectionPoints(
        Coordinates pt1,
        double r1,
        Coordinates pt2,
        double r2)
    {
        // Calculate distance between circle centers
        double dx = pt2.X - pt1.X;
        double dy = pt2.Y - pt1.Y;
        double d = Math.Sqrt((dx * dx) + (dy * dy));

        // If the distance exceeds the sum of the radii of the two circles,
        // or is less than the difference in radii,
        // an exception is thrown (should not happen)
        if (d >= r1 + r2 || d <= Math.Abs(r1 - r2))
        {
            yield break;
        }

        // Calculate the base point of the intersection line
        double a = ((r1 * r1) - (r2 * r2) + (d * d)) / (CircleIntersectionDivisor * d);
        double h = Math.Sqrt((r1 * r1) - (a * a));

        // datum vector between circle centers
        double cx = pt1.X + (a * dx / d);
        double cy = pt1.Y + (a * dy / d);

        // Calculate intersection point
        double offsetX = h * dy / d;
        double offsetY = h * dx / d;

        yield return new(cx + offsetX, cy - offsetY);
        yield return new(cx - offsetX, cy + offsetY);
    }

    /// <summary>Adds a constant-resistance circle to the chart.</summary>
    /// <param name="value">The normalized resistance represented by the circle.</param>
    /// <returns>The new circle.</returns>
    public RealTick AddRealTick(double value)
    {
        RealTick part = new(value) { LineStyle = RealLineStyle.Clone(), };
        RealTicks.Add(part);
        return part;
    }

    /// <summary>Adds a constant-reactance curve to the chart.</summary>
    /// <param name="value">The normalized reactance represented by the curve.</param>
    /// <returns>The new curve.</returns>
    public ImaginaryTick AddImaginaryTick(double value)
    {
        ImaginaryTick part = new(value) { LineStyle = RealLineStyle.Clone(), };
        ImaginaryTicks.Add(part);
        return part;
    }

    /// <summary>Returns the Cartesian position of a point defined by its polar coordinates.</summary>
    /// <param name="radius">The point's radius.</param>
    /// <param name="angle">The point's angle.</param>
    /// <returns>The rotated Cartesian coordinates of the point.</returns>
    public Coordinates GetCoordinates(double radius, Angle angle) => GetCoordinates(new(radius, angle));

    /// <summary>Returns the Cartesian position of a polar-coordinate point.</summary>
    /// <param name="point">The point in polar coordinates.</param>
    /// <returns>The rotated Cartesian coordinates of the point.</returns>
    public Coordinates GetCoordinates(PolarCoordinates point) => point.WithAngle(point.Angle + Rotation).ToCartesian();

    /// <summary>Returns the reflection coefficient of the specified impedance.</summary>
    /// <param name="resistance">The normalized resistance.</param>
    /// <param name="reactance">The normalized reactance.</param>
    /// <returns>The impedance's reflection coefficient.</returns>
    public Coordinates GetCoordinates(double resistance, double reactance)
    {
        Coordinates normalizedImpedance = new(resistance, reactance);
        return CalculateGamma(normalizedImpedance);
    }

    /// <summary>Gets the axis limits enclosing the chart and its ticks.</summary>
    /// <returns>The combined limits of the chart elements.</returns>
    public AxisLimits GetAxisLimits()
    {
        AxisLimits limits = new(-1, 1, -1, 1);
        foreach (RealTick tick in RealTicks)
        {
            limits = limits.Expanded(tick.GetAxisLimits());
        }

        foreach (ImaginaryTick tick in ImaginaryTicks)
        {
            limits = limits.Expanded(tick.GetAxisLimits());
        }

        return limits;
    }

    /// <summary>Ensures square zoom behavior is applied to the plot axes.</summary>
    /// <param name="plot">The plot whose axes are updated.</param>
    public virtual void UpdateAxisLimits(Plot plot)
    {
        foreach (IAxisRule rule in plot.Axes.Rules)
        {
            if (rule is AxisRules.SquareZoomOut)
            {
                return;
            }
        }

        AxisRules.SquareZoomOut squareRule = new(Axes.XAxis, Axes.YAxis);
        plot.Axes.Rules.Add(squareRule);
    }

    /// <summary>Renders the Smith chart axis and its labels.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        RenderConstantRealCircles(rp, rp.Paint);
        RenderConstantImaginaryCurves(rp, rp.Paint);
        RenderCircleLabels(rp, rp.Paint);
    }

    /// <summary>Renders labels attached to the imaginary ticks.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for rendering.</param>
    private void RenderCircleLabels(RenderPack rp, Paint paint)
    {
        foreach (PolarAxisCircle circle in ImaginaryTicks)
        {
            Coordinates c = GetCoordinates(radius: circle.Radius, angle: circle.LabelAngle);
            Pixel px = Axes.GetPixel(c);
            circle.LabelStyle.Render(rp.Canvas, px, paint, circle.LabelText);
        }
    }

    /// <summary>Renders the constant-resistance circles and their labels.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for rendering.</param>
    private void RenderConstantRealCircles(RenderPack rp, Paint paint)
    {
        double pixelsPerUnit = rp.DataRect.Width / Axes.XAxis.Width;
        LabelStyle.Rotation = (float)Rotation.Degrees - (float)QuarterTurnDegrees;
        LabelStyle.Alignment = Alignment.LowerLeft;

        foreach (RealTick part in RealTicks)
        {
            float radiusPx = (float)(pixelsPerUnit * part.Radius);
            Pixel originPx = Axes.GetPixel(part.Origin);
            Drawing.DrawCircle(rp.Canvas, originPx, radiusPx, part.LineStyle, paint);

            if (part.Re > 0)
            {
                LabelStyle.Text = $"{part.Re}";
                Coordinates labelPoint = new PolarCoordinates(
                    part.Radius * (1 + (LabelPaddingFraction / LabelPaddingDivisor)),
                    Angle.FromDegrees(HalfTurnDegrees))
                    .ToCartesian()
                    .WithDelta(part.Origin.X, part.Origin.Y);
                Pixel labelPixel = Axes.GetPixel(labelPoint).WithOffset(0F, -LabelVerticalOffsetPixels);
                LabelStyle.Render(rp.Canvas, labelPixel, paint);
            }
        }
    }

    /// <summary>Renders the constant-reactance curves and their labels.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for rendering.</param>
    private void RenderConstantImaginaryCurves(RenderPack rp, Paint paint)
    {
        double pixelsPerUnit = rp.DataRect.Width / Axes.XAxis.Width;
        LabelStyle.Rotation = (float)Rotation.Degrees;
        LabelStyle.Alignment = Alignment.MiddleCenter;

        foreach (ImaginaryTick tick in ImaginaryTicks)
        {
            float radiusPx = (float)(pixelsPerUnit * tick.Radius);
            Pixel originPx = Axes.GetPixel(tick.Origin);
            if (tick.Lm == 0)
            {
                Drawing.DrawLine(
                    rp.Canvas,
                    paint,
                    Axes.GetPixel(ImaginaryTick.GetPointOnCircle(new(), 1, Angle.FromDegrees(HalfTurnDegrees))),
                    Axes.GetPixel(ImaginaryTick.GetPointOnCircle(new(), 1, Angle.FromDegrees(0))),
                    tick.LineStyle);

                PolarCoordinates labelPoint =
                    new(1 + LabelPaddingFraction, Angle.FromDegrees(0));

                LabelStyle.Text = "∞";
                LabelStyle.Render(rp.Canvas, Axes.GetPixel(labelPoint.ToCartesian()), paint);

                LabelStyle.Text = "0";
                labelPoint = labelPoint.WithAngleDegrees(HalfTurnDegrees);
                LabelStyle.Render(rp.Canvas, Axes.GetPixel(labelPoint.ToCartesian()), paint);
            }
            else if (Math.Abs(tick.SweepAngle.Degrees - FullTurnDegrees) <= FullTurnComparisonTolerance)
            {
                Drawing.DrawCircle(rp.Canvas, originPx, radiusPx, tick.LineStyle, paint);
            }
            else
            {
                PixelRect rect = new(
                    originPx.X - radiusPx,
                    originPx.X + radiusPx,
                    originPx.Y + radiusPx,
                    originPx.Y - radiusPx);
                Drawing.DrawArc(
                    rp.Canvas,
                    paint,
                    tick.LineStyle,
                    rect,
                    (float)-tick.StartAngle.Degrees,
                    (float)-tick.SweepAngle.Degrees);

                Coordinates point = (tick.Lm < 0) ? tick.Points[0] : tick.Points[1];
                LabelStyle.Text = $"{(tick.Lm < 0 ? '-' : '+')}j{Math.Abs(tick.Lm)}";

                Angle angle = ImaginaryTick.GetAngle(point, new(0, 0));
                PolarCoordinates labelPoint =
                    new(1 + LabelPaddingFraction, angle);
                Pixel labelPixel = Axes.GetPixel(labelPoint.ToCartesian());
                LabelStyle.Render(rp.Canvas, labelPixel, paint);
            }
        }
    }

    /// <summary>Represents a constant-resistance circle on the Smith chart.</summary>
    public sealed class RealTick : PolarAxisCircle
    {
        /// <summary>Initializes a new instance of the <see cref="RealTick"/> class.</summary>
        /// <param name="re">The normalized resistance represented by the circle.</param>
        public RealTick(double re)
            : base(Math.Abs(1 / (1 + re)))
        {
            Re = re;

            double p = 1 / (1 + re);
            Origin = new(1 - p, 0);
        }

        /// <summary>Gets or sets the normalized resistance represented by this circle.</summary>
        public double Re { get; set; }

        /// <summary>Gets the axis limits required to display this circle.</summary>
        /// <returns>The limits enclosing the circle.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design",
            "CA1024:Use properties where appropriate",
            Justification = "This established public method is retained for API compatibility.")]
        public AxisLimits GetAxisLimits() => new(
                Origin.X - Radius,
                Origin.X + Radius,
                Origin.Y - Radius,
                Origin.Y + Radius);
    }

    /// <summary>Represents a constant-reactance curve on the Smith chart.</summary>
    public sealed class ImaginaryTick : PolarAxisCircle
    {
        /// <summary>The number of distinct intersections required for an arc.</summary>
        private const int RequiredIntersectionPointCount = 2;

        /// <summary>Initializes a new instance of the <see cref="ImaginaryTick"/> class.</summary>
        /// <param name="lm">The normalized reactance represented by the curve.</param>
        /// <param name="center">The center of the chart circle.</param>
        /// <param name="radius">The radius of the chart circle.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "RoslynCommonAnalyzers",
            "SST2309:Optional public API parameters",
            Justification = "The optional center and radius defaults are retained for existing source compatibility.")]
        public ImaginaryTick(
            double lm,
            Coordinates center = default,
            double radius = 1)
            : base(Math.Abs(1 / lm))
        {
            Lm = lm;
            Origin = new(1, 1 / lm);

            List<Coordinates> points = [];
            points.AddRange(FindIntersectionPoints(center, radius, Origin, Radius));
            Points = points;
            if (Points.Count < RequiredIntersectionPointCount)
            {
                return;
            }

            Angle ang1 = GetAngle(Points[0], Origin);
            Angle ang2 = GetAngle(Points[1], Origin);

            StartAngle = ang2;
            SweepAngle = ang1 - ang2;
        }

        /// <summary>Gets or sets the normalized reactance represented by this curve.</summary>
        public double Lm { get; set; }

        /// <summary>Gets the intersections of the curve and the chart boundary.</summary>
        public IReadOnlyList<Coordinates> Points { get; }

        /// <summary>Calculates the angle of a point relative to the center of a circle.</summary>
        /// <param name="point">Target point.</param>
        /// <param name="center">The coordinates of the center of the circle.</param>
        /// <returns>The normalized angle from the center to the point.</returns>
        public static Angle GetAngle(Coordinates point, Coordinates center)
        {
            // Make sure the angle range is between 0 and 360.
            return Angle
                .FromRadians(Math.Atan2(point.Y - center.Y, point.X - center.X))
                .Normalized;
        }

        /// <summary>Calculates a point on a circle at the specified angle.</summary>
        /// <param name="center">The coordinates of the circle's center.</param>
        /// <param name="radius">The circle's radius.</param>
        /// <param name="angle">The angle of the target point.</param>
        /// <returns>The coordinates of the target point.</returns>
        public static Coordinates GetPointOnCircle(
            Coordinates center,
            double radius,
            Angle angle) => new(
                center.X + (radius * Math.Cos(angle.Radians)),
                center.Y + (radius * Math.Sin(angle.Radians)));

        /// <summary>Gets the axis limits required to display this curve.</summary>
        /// <returns>The limits enclosing the curve, or no limits for zero reactance.</returns>
        public AxisLimits GetAxisLimits()
        {
            if (Lm == 0)
            {
                return AxisLimits.NoLimits;
            }

            if (SweepAngle.Normalized.Degrees == 0)
            {
                return new(
                    Origin.X - Radius,
                    Origin.X + Radius,
                    Origin.Y - Radius,
                    Origin.Y + Radius);
            }

            Coordinates pt1 = GetPointOnCircle(Origin, Radius, StartAngle);
            Coordinates pt2 = GetPointOnCircle(Origin, Radius, StartAngle + SweepAngle);
            return new(
                Math.Min(pt1.X, pt2.X),
                Math.Max(pt1.X, pt2.X),
                Math.Min(pt1.Y, pt2.Y),
                Math.Max(pt1.Y, pt2.Y));
        }
    }
}
