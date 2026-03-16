// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Control that draws a symmetrical arc with rounded edges.
/// </summary>
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

    static Arc()
    {
        AffectsGeometry<Arc>(StartAngleProperty, EndAngleProperty, SweepDirectionProperty);
        StrokeLineCapProperty.OverrideDefaultValue<Arc>(PenLineCap.Round);
    }

    /// <summary>
    /// Gets or sets the initial angle from which the arc will be drawn.
    /// </summary>
    public double StartAngle
    {
        get => GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }

    /// <summary>
    /// Gets or sets the final angle from which the arc will be drawn.
    /// </summary>
    public double EndAngle
    {
        get => GetValue(EndAngleProperty);
        set => SetValue(EndAngleProperty, value);
    }

    /// <summary>
    /// Gets or sets the direction to where the arc will be drawn.
    /// </summary>
    public SweepDirection SweepDirection
    {
        get => GetValue(SweepDirectionProperty);
        set => SetValue(SweepDirectionProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether one of the two larger arc sweeps is chosen.
    /// </summary>
    public bool IsLargeArc { get; private set; }

    /// <inheritdoc />
    protected override Geometry? CreateDefiningGeometry()
    {
        IsLargeArc = Math.Abs(EndAngle - StartAngle) > 180;

        var geometryStream = new StreamGeometry();
        var strokeThickness = StrokeThickness;
        var arcSize = new Size(
            Math.Max(0, (Bounds.Width - strokeThickness) / 2),
            Math.Max(0, (Bounds.Height - strokeThickness) / 2));

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

        geometryStream.Transform = new TranslateTransform(strokeThickness / 2, strokeThickness / 2);

        return geometryStream;
    }

    /// <summary>
    /// Draws a point on the coordinates of the given angle.
    /// </summary>
    /// <param name="angle">The angle at which to create the point.</param>
    /// <returns>A Point.</returns>
    private Point PointAtAngle(double angle)
    {
        var strokeThickness = StrokeThickness;

        if (SweepDirection == SweepDirection.CounterClockwise)
        {
            angle += 90;
            angle %= 360;
            if (angle < 0)
            {
                angle += 360;
            }

            var radAngle = angle * (Math.PI / 180);
            var xRadius = (Bounds.Width - strokeThickness) / 2;
            var yRadius = (Bounds.Height - strokeThickness) / 2;

            return new Point(
                xRadius + (xRadius * Math.Cos(radAngle)),
                yRadius - (yRadius * Math.Sin(radAngle)));
        }
        else
        {
            angle -= 90;
            angle %= 360;
            if (angle < 0)
            {
                angle += 360;
            }

            var radAngle = angle * (Math.PI / 180);
            var xRadius = (Bounds.Width - strokeThickness) / 2;
            var yRadius = (Bounds.Height - strokeThickness) / 2;

            return new Point(
                xRadius + (xRadius * Math.Cos(-radAngle)),
                yRadius - (yRadius * Math.Sin(-radAngle)));
        }
    }
}
