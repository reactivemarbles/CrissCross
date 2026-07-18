// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Control that draws a symmetrical arc with rounded edges.</summary>
public class Arc : Shape
{
    /// <summary>Identifies the <see cref="StartAngle"/> dependency property.</summary>
    public static readonly StyledProperty<double> StartAngleProperty =
        AvaloniaProperty.Register<Arc, double>(nameof(StartAngle), 0.0);

    /// <summary>Identifies the <see cref="EndAngle"/> dependency property.</summary>
    public static readonly StyledProperty<double> EndAngleProperty =
        AvaloniaProperty.Register<Arc, double>(nameof(EndAngle), 0.0);

    /// <summary>Identifies the <see cref="SweepDirection"/> dependency property.</summary>
    public static readonly StyledProperty<SweepDirection> SweepDirectionProperty =
        AvaloniaProperty.Register<Arc, SweepDirection>(nameof(SweepDirection), SweepDirection.Clockwise);

    /// <summary>Provides the Arc member.</summary>
    static Arc()
    {
        AffectsGeometry<Arc>(StartAngleProperty, EndAngleProperty, SweepDirectionProperty);
        StrokeLineCapProperty.OverrideDefaultValue<Arc>(PenLineCap.Round);
    }

    /// <summary>Gets or sets the initial angle from which the arc will be drawn.</summary>
    public double StartAngle
    {
        get => GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }

    /// <summary>Gets or sets the final angle from which the arc will be drawn.</summary>
    public double EndAngle
    {
        get => GetValue(EndAngleProperty);
        set => SetValue(EndAngleProperty, value);
    }

    /// <summary>Gets or sets the direction to where the arc will be drawn.</summary>
    public SweepDirection SweepDirection
    {
        get => GetValue(SweepDirectionProperty);
        set => SetValue(SweepDirectionProperty, value);
    }

    /// <summary>Gets a value indicating whether one of the two larger arc sweeps is chosen.</summary>
    public bool IsLargeArc { get; private set; }

    /// <inheritdoc />
    protected override Geometry? CreateDefiningGeometry()
    {
        const double degreesInHalfCircle = 180.0;
        const double half = 2.0;

        IsLargeArc = Math.Abs(EndAngle - StartAngle) > degreesInHalfCircle;

        var geometryStream = new StreamGeometry();
        var strokeThickness = StrokeThickness;
        var arcSize = new Size(
            Math.Max(0, (Bounds.Width - strokeThickness) / half),
            Math.Max(0, (Bounds.Height - strokeThickness) / half));

        using (var context = geometryStream.Open())
        {
            context.BeginFigure(PointAtAngle(Math.Min(StartAngle, EndAngle)), false);

            context.ArcTo(
                PointAtAngle(Math.Max(StartAngle, EndAngle)),
                arcSize,
                0,
                IsLargeArc,
                SweepDirection);
        }

        geometryStream.Transform = new TranslateTransform(strokeThickness / half, strokeThickness / half);

        return geometryStream;
    }

    /// <summary>Draws a point on the coordinates of the given angle.</summary>
    /// <param name="angle">The angle at which to create the point.</param>
    /// <returns>A Point.</returns>
    private Point PointAtAngle(double angle)
    {
        const double degreesInHalfCircle = 180.0;
        const double degreesInRightAngle = 90.0;
        const double degreesInFullCircle = 360.0;
        const double half = 2.0;

        var strokeThickness = StrokeThickness;

        if (SweepDirection == SweepDirection.CounterClockwise)
        {
            angle += degreesInRightAngle;
            angle %= degreesInFullCircle;
            if (angle < 0)
            {
                angle += degreesInFullCircle;
            }

            var radAngle = angle * (Math.PI / degreesInHalfCircle);
            var horizontalRadius = (Bounds.Width - strokeThickness) / half;
            var verticalRadius = (Bounds.Height - strokeThickness) / half;

            return new Point(
                horizontalRadius + (horizontalRadius * Math.Cos(radAngle)),
                verticalRadius - (verticalRadius * Math.Sin(radAngle)));
        }

        angle -= degreesInRightAngle;
        angle %= degreesInFullCircle;
        if (angle < 0)
        {
            angle += degreesInFullCircle;
        }

        var clockwiseRadAngle = angle * (Math.PI / degreesInHalfCircle);
        var clockwiseHorizontalRadius = (Bounds.Width - strokeThickness) / half;
        var clockwiseVerticalRadius = (Bounds.Height - strokeThickness) / half;

        return new Point(
            clockwiseHorizontalRadius + (clockwiseHorizontalRadius * Math.Cos(-clockwiseRadAngle)),
            clockwiseVerticalRadius - (clockwiseVerticalRadius * Math.Sin(-clockwiseRadAngle)));
    }
}
