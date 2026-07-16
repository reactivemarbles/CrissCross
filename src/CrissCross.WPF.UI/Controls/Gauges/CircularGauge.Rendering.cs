// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Contains CircularGauge scale and range rendering operations.</summary>
public sealed partial class CircularGauge
{
    /// <summary>Creates a scale translation at the specified radius and angle.</summary>
    /// <param name="radius">The scale radius.</param>
    /// <param name="angleRadians">The angle in radians.</param>
    /// <returns>The scale translation.</returns>
    private static TranslateTransform CreateScaleTranslation(double radius, double angleRadians) =>
        new() { X = (int)(radius * Math.Cos(angleRadians)), Y = (int)(radius * Math.Sin(angleRadians)) };

    /// <summary>Draw the range indicator.</summary>
    private void DrawRangeIndicator()
    {
        var realworldunit = ScaleSweepAngle / (MaxValue - MinValue);
        var optimalStartAngle = double.NaN;
        var optimalEndAngle = double.NaN;
        var db = 0D;

        // Checking whether the OptimalRangeStartvalue is -
        if (OptimalRangeStartValue < 0)
        {
            db = MinValue + Math.Abs(OptimalRangeStartValue);
            optimalStartAngle = Math.Abs(db * realworldunit);
        }
        else
        {
            db = Math.Abs(MinValue) + OptimalRangeStartValue;
            optimalStartAngle = db * realworldunit;
        }

        // Checking whether the OptimalRangeEndvalue is -
        if (OptimalRangeEndValue < 0)
        {
            db = MinValue + Math.Abs(OptimalRangeEndValue);
            optimalEndAngle = Math.Abs(db * realworldunit);
        }
        else
        {
            db = Math.Abs(MinValue) + OptimalRangeEndValue;
            optimalEndAngle = db * realworldunit;
        }

        // Calculating the angle for optimal Start value
        var optimalStartAngleFromStart = ScaleStartAngle + optimalStartAngle;

        // Calculating the angle for optimal Start value
        var optimalEndAngleFromStart = ScaleStartAngle + optimalEndAngle;

        // Calculating the Radius of the two arc for segment
        _arcradius1 = RangeIndicatorRadius + RangeIndicatorThickness;
        _arcradius2 = RangeIndicatorRadius;
        var endAngle = ScaleStartAngle + ScaleSweepAngle;

        // Calculating the Points for the below Optimal Range segment from the center of the gauge
        var isReflexAngle = Math.Abs(optimalStartAngleFromStart - ScaleStartAngle) > SemiCircleDegrees;
        _rangeIndicator1 = DrawSegment(
            GetCircumferencePoint(ScaleStartAngle, _arcradius1),
            GetCircumferencePoint(ScaleStartAngle, _arcradius2),
            GetCircumferencePoint(optimalStartAngleFromStart, _arcradius2),
            GetCircumferencePoint(optimalStartAngleFromStart, _arcradius1),
            isReflexAngle,
            BelowOptimalRangeColor);

        // Calculating the Points for the Optimal Range segment from the center of the gauge
        var isReflexAngle1 = Math.Abs(optimalEndAngleFromStart - optimalStartAngleFromStart) > SemiCircleDegrees;
        _rangeIndicator2 = DrawSegment(
            GetCircumferencePoint(optimalStartAngleFromStart, _arcradius1),
            GetCircumferencePoint(optimalStartAngleFromStart, _arcradius2),
            GetCircumferencePoint(optimalEndAngleFromStart, _arcradius2),
            GetCircumferencePoint(optimalEndAngleFromStart, _arcradius1),
            isReflexAngle1,
            OptimalRangeColor);

        // Calculating the Points for the Above Optimal Range segment from the center of the gauge
        var isReflexAngle2 = Math.Abs(endAngle - optimalEndAngleFromStart) > SemiCircleDegrees;
        _rangeIndicator3 = DrawSegment(
            GetCircumferencePoint(optimalEndAngleFromStart, _arcradius1),
            GetCircumferencePoint(optimalEndAngleFromStart, _arcradius2),
            GetCircumferencePoint(endAngle, _arcradius2),
            GetCircumferencePoint(endAngle, _arcradius1),
            isReflexAngle2,
            AboveOptimalRangeColor);
    }

    /// <summary>Drawing the scale with the Scale Radius.</summary>
    private void DrawScale()
    {
        if (_rootGrid is null)
        {
            return;
        }

        // Calculate one major tick angle
        var majorTickUnitAngle = ScaleSweepAngle / MajorDivisionsCount;

        // Obtaining One major ticks value
        var majorTicksUnitValue = (MaxValue - MinValue) / MajorDivisionsCount;
        majorTicksUnitValue = Math.Round(majorTicksUnitValue, ScaleValuePrecision);
        var minValue = MinValue;

        // Drawing Major scale ticks
        for (var angle = ScaleStartAngle; angle <= (ScaleStartAngle + ScaleSweepAngle); angle += majorTickUnitAngle)
        {
            if (!TryDrawMajorTick(angle, majorTicksUnitValue, ref minValue, out var label, out var tick))
            {
                break;
            }

            DrawMinorTicks(angle, majorTickUnitAngle, minValue);

            _ht.Add("tb_" + _numberOfpoints, label);
            _ht.Add("rec_" + _numberOfpoints, tick);
            _numberOfpoints++;
        }
    }

    /// <summary>Draws one major tick and its value label.</summary>
    /// <param name="angle">The tick angle.</param>
    /// <param name="majorTickUnitValue">The value represented by one major division.</param>
    /// <param name="minValue">The next label value.</param>
    /// <param name="label">The created label.</param>
    /// <param name="tick">The created tick.</param>
    /// <returns><see langword="true"/> when a tick was drawn; otherwise, <see langword="false"/>.</returns>
    private bool TryDrawMajorTick(
        double angle,
        double majorTickUnitValue,
        ref double minValue,
        out TextBlock label,
        out Rectangle tick)
    {
        var indicatorRadians = angle * Math.PI / SemiCircleDegrees;
        var tickTransform = new TransformGroup();
        tickTransform.Children.Add(new RotateTransform { Angle = angle });
        tickTransform.Children.Add(CreateScaleTranslation(ScaleRadius, indicatorRadians));

        label = new TextBlock
        {
            Height = ScaleLabelSize.Height,
            Width = ScaleLabelSize.Width,
            FontSize = ScaleLabelFontSize,
            Foreground = ScaleForeground,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            RenderTransform = CreateScaleTranslation(ScaleLabelRadius, indicatorRadians),
        };

        minValue = Math.Round(minValue, ScaleValuePrecision);
        if (minValue > Math.Round(MaxValue, ScaleValuePrecision))
        {
            tick = null!;
            return false;
        }

        label.Text = minValue.ToString(CultureInfo.InvariantCulture);
        minValue += majorTickUnitValue;
        tick = new Rectangle
        {
            Height = MajorTickSize.Height,
            Width = MajorTickSize.Width,
            Fill = ScaleForeground,
            RenderTransformOrigin = new(CenterOrigin, CenterOrigin),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            RenderTransform = tickTransform,
        };
        _ = _rootGrid!.Children.Add(tick);
        _ = _rootGrid.Children.Add(label);
        return true;
    }

    /// <summary>Draws the minor ticks for one major scale division.</summary>
    /// <param name="angle">The current major tick angle.</param>
    /// <param name="majorTickUnitAngle">The angle represented by one major division.</param>
    /// <param name="nextMajorValue">The next major label value.</param>
    private void DrawMinorTicks(double angle, double majorTickUnitAngle, double nextMajorValue)
    {
        if (
            angle >= (ScaleStartAngle + ScaleSweepAngle)
            || Math.Round(nextMajorValue, ScaleValuePrecision) > Math.Round(MaxValue, ScaleValuePrecision))
        {
            return;
        }

        var minorTickUnitAngle = majorTickUnitAngle / MinorDivisionsCount;
        for (
            var minorAngle = angle + minorTickUnitAngle;
            minorAngle < (angle + majorTickUnitAngle);
            minorAngle += minorTickUnitAngle)
        {
            var minorTickRadians = minorAngle * Math.PI / SemiCircleDegrees;
            var minorTickTransform = new TransformGroup();
            minorTickTransform.Children.Add(new RotateTransform { Angle = minorAngle });
            minorTickTransform.Children.Add(CreateScaleTranslation(ScaleRadius, minorTickRadians));

            var tick = new Rectangle
            {
                Height = MinorTickSize.Height,
                Width = MinorTickSize.Width,
                Fill = ScaleForeground,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                RenderTransformOrigin = new(CenterOrigin, CenterOrigin),
                RenderTransform = minorTickTransform,
            };
            _ = _rootGrid!.Children.Add(tick);
            _ht.Add("mr_" + _numberOfpoints + "_" + _numberOfMinorpoints, tick);
            _numberOfMinorpoints++;
        }
    }

    /// <summary>Drawing the segment with two arc and two line.</summary>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    /// <param name="p4">The p4.</param>
    /// <param name="reflexangle">if set to <c>true</c> [reflexangle].</param>
    /// <param name="clr">The color.</param>
    /// <returns>The Path.</returns>
    private Path DrawSegment(Point p1, Point p2, Point p3, Point p4, bool reflexangle, Brush clr)
    {
        // Segment Geometry
        var segments = new PathSegmentCollection
        {
            new LineSegment { Point = p2 },
            new ArcSegment
            {
                Size = new(_arcradius2, _arcradius2),
                Point = p3,
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = reflexangle,
            },
            new LineSegment { Point = p4 },
            new ArcSegment
            {
                Size = new(_arcradius1, _arcradius1),
                Point = p1,
                SweepDirection = SweepDirection.Counterclockwise,
                IsLargeArc = reflexangle,
            },
        };

        // First line segment from pt p1 - pt p2
        // Arc drawn from pt p2 - pt p3 with the RangeIndicatorRadius
        // Second line segment from pt p3 - pt p4
        // Arc drawn from pt p4 - pt p1 with the Radius of arcradius1
        // Defining the segment path properties
        var rangestrokecolor = Equals(clr, Brushes.Transparent) ? clr : Brushes.White;
        var range = new Path
        {
            StrokeLineJoin = PenLineJoin.Round,
            Stroke = rangestrokecolor,
            Fill = clr,
            Opacity = RangeSegmentOpacity,
            StrokeThickness = RangeSegmentStrokeThickness,
            Data = new PathGeometry
            {
                Figures =
                [
                    new PathFigure
                    {
                        IsClosed = true,
                        StartPoint = p1,
                        Segments = segments,
                    },],
            },
        };

        // Set Z index of range indicator
        range.SetValue(Panel.ZIndexProperty, RangeSegmentZIndex);

        // Adding the segment to the root grid
        _rootGrid?.Children.Add(range);

        return range;
    }

    /// <summary>Provides the GetCircumferencePoint member.</summary>
    /// <param name="angle">The angle value.</param>
    /// <param name="radius">The radius value.</param>
    /// <returns>The result.</returns>
    private Point GetCircumferencePoint(double angle, double radius)
    {
        var angleRadian = angle * Math.PI / SemiCircleDegrees;

        // Radius-- is the Radius of the gauge
        return new Point(Radius + (radius * Math.Cos(angleRadian)), Radius + (radius * Math.Sin(angleRadian)));
    }

    /// <summary>Provides the MovePointer member.</summary>
    /// <param name="angleValue">The anglevalue.</param>
    private void MovePointer(double angleValue)
    {
        if (
            _pointer?.RenderTransform is not TransformGroup transformGroup
            || transformGroup.Children[0] is not RotateTransform rotateTransform)
        {
            return;
        }

        rotateTransform.Angle = angleValue;
    }

    /// <summary>Provides the Sb_Completed member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void Sb_Completed(object? sender, EventArgs e)
    {
        if (Value > OptimalRangeEndValue)
        {
            _lightIndicator?.Fill = GetRangeIndicatorGradEffect(AboveOptimalRangeColor);
        }
        else if (Value <= OptimalRangeEndValue && Value >= OptimalRangeStartValue)
        {
            _lightIndicator?.Fill = GetRangeIndicatorGradEffect(OptimalRangeColor);
        }
        else if (Value < OptimalRangeStartValue)
        {
            _lightIndicator?.Fill = GetRangeIndicatorGradEffect(BelowOptimalRangeColor);
        }
    }
}
