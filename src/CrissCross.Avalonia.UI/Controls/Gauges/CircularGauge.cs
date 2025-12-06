// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// A circular gauge control that displays a value within a specified range using a radial scale and pointer.
/// </summary>
public class CircularGauge : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="Value"/>.
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<CircularGauge, double>(nameof(Value), 0.0);

    /// <summary>
    /// Property for <see cref="MinValue"/>.
    /// </summary>
    public static readonly StyledProperty<double> MinValueProperty =
        AvaloniaProperty.Register<CircularGauge, double>(nameof(MinValue), 0.0);

    /// <summary>
    /// Property for <see cref="MaxValue"/>.
    /// </summary>
    public static readonly StyledProperty<double> MaxValueProperty =
        AvaloniaProperty.Register<CircularGauge, double>(nameof(MaxValue), 100.0);

    /// <summary>
    /// Property for <see cref="Radius"/>.
    /// </summary>
    public static readonly StyledProperty<double> RadiusProperty =
        AvaloniaProperty.Register<CircularGauge, double>(nameof(Radius), 100.0);

    /// <summary>
    /// Property for <see cref="ScaleStartAngle"/>.
    /// </summary>
    public static readonly StyledProperty<double> ScaleStartAngleProperty =
        AvaloniaProperty.Register<CircularGauge, double>(nameof(ScaleStartAngle), 135.0);

    /// <summary>
    /// Property for <see cref="ScaleSweepAngle"/>.
    /// </summary>
    public static readonly StyledProperty<double> ScaleSweepAngleProperty =
        AvaloniaProperty.Register<CircularGauge, double>(nameof(ScaleSweepAngle), 270.0);

    /// <summary>
    /// Property for <see cref="MajorDivisionsCount"/>.
    /// </summary>
    public static readonly StyledProperty<int> MajorDivisionsCountProperty =
        AvaloniaProperty.Register<CircularGauge, int>(nameof(MajorDivisionsCount), 10);

    /// <summary>
    /// Property for <see cref="MinorDivisionsCount"/>.
    /// </summary>
    public static readonly StyledProperty<int> MinorDivisionsCountProperty =
        AvaloniaProperty.Register<CircularGauge, int>(nameof(MinorDivisionsCount), 5);

    /// <summary>
    /// Property for <see cref="PointerLength"/>.
    /// </summary>
    public static readonly StyledProperty<double> PointerLengthProperty =
        AvaloniaProperty.Register<CircularGauge, double>(nameof(PointerLength), 70.0);

    /// <summary>
    /// Property for <see cref="PointerCapRadius"/>.
    /// </summary>
    public static readonly StyledProperty<double> PointerCapRadiusProperty =
        AvaloniaProperty.Register<CircularGauge, double>(nameof(PointerCapRadius), 10.0);

    /// <summary>
    /// Property for <see cref="PointerColor"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> PointerColorProperty =
        AvaloniaProperty.Register<CircularGauge, IBrush>(nameof(PointerColor), Brushes.Red);

    /// <summary>
    /// Property for <see cref="ScaleColor"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> ScaleColorProperty =
        AvaloniaProperty.Register<CircularGauge, IBrush>(nameof(ScaleColor), Brushes.Gray);

    /// <summary>
    /// Property for <see cref="OptimalRangeColor"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> OptimalRangeColorProperty =
        AvaloniaProperty.Register<CircularGauge, IBrush>(nameof(OptimalRangeColor), Brushes.Green);

    /// <summary>
    /// Property for <see cref="BelowOptimalRangeColor"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> BelowOptimalRangeColorProperty =
        AvaloniaProperty.Register<CircularGauge, IBrush>(nameof(BelowOptimalRangeColor), Brushes.Yellow);

    /// <summary>
    /// Property for <see cref="AboveOptimalRangeColor"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> AboveOptimalRangeColorProperty =
        AvaloniaProperty.Register<CircularGauge, IBrush>(nameof(AboveOptimalRangeColor), Brushes.Red);

    /// <summary>
    /// Property for <see cref="OptimalRangeStartValue"/>.
    /// </summary>
    public static readonly StyledProperty<double> OptimalRangeStartValueProperty =
        AvaloniaProperty.Register<CircularGauge, double>(nameof(OptimalRangeStartValue), 20.0);

    /// <summary>
    /// Property for <see cref="OptimalRangeEndValue"/>.
    /// </summary>
    public static readonly StyledProperty<double> OptimalRangeEndValueProperty =
        AvaloniaProperty.Register<CircularGauge, double>(nameof(OptimalRangeEndValue), 80.0);

    /// <summary>
    /// Property for <see cref="DialText"/>.
    /// </summary>
    public static readonly StyledProperty<string> DialTextProperty =
        AvaloniaProperty.Register<CircularGauge, string>(nameof(DialText), "Gauge");

    /// <summary>
    /// Property for <see cref="Unit"/>.
    /// </summary>
    public static readonly StyledProperty<string> UnitProperty =
        AvaloniaProperty.Register<CircularGauge, string>(nameof(Unit), string.Empty);

    /// <summary>
    /// Property for <see cref="ShowValue"/>.
    /// </summary>
    public static readonly StyledProperty<bool> ShowValueProperty =
        AvaloniaProperty.Register<CircularGauge, bool>(nameof(ShowValue), true);

    /// <summary>
    /// Property for <see cref="Decimals"/>.
    /// </summary>
    public static readonly StyledProperty<int> DecimalsProperty =
        AvaloniaProperty.Register<CircularGauge, int>(nameof(Decimals), 0);

    /// <summary>
    /// Property for <see cref="PointerAngle"/>.
    /// </summary>
    public static readonly StyledProperty<double> PointerAngleProperty =
        AvaloniaProperty.Register<CircularGauge, double>(nameof(PointerAngle), 135.0);

    private Canvas? _scaleCanvas;

    static CircularGauge()
    {
        ValueProperty.Changed.AddClassHandler<CircularGauge>((x, _) => x.UpdatePointerAngle());
        MinValueProperty.Changed.AddClassHandler<CircularGauge>((x, _) => x.RedrawScale());
        MaxValueProperty.Changed.AddClassHandler<CircularGauge>((x, _) => x.RedrawScale());
        MajorDivisionsCountProperty.Changed.AddClassHandler<CircularGauge>((x, _) => x.RedrawScale());
        MinorDivisionsCountProperty.Changed.AddClassHandler<CircularGauge>((x, _) => x.RedrawScale());
        ScaleStartAngleProperty.Changed.AddClassHandler<CircularGauge>((x, _) => x.RedrawScale());
        ScaleSweepAngleProperty.Changed.AddClassHandler<CircularGauge>((x, _) => x.RedrawScale());
        RadiusProperty.Changed.AddClassHandler<CircularGauge>((x, _) => x.RedrawScale());
        BoundsProperty.Changed.AddClassHandler<CircularGauge>((x, _) => x.RedrawScale());
    }

    /// <summary>
    /// Gets or sets the current value displayed by the gauge.
    /// </summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum value of the gauge scale.
    /// </summary>
    public double MinValue
    {
        get => GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum value of the gauge scale.
    /// </summary>
    public double MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the radius of the gauge.
    /// </summary>
    public double Radius
    {
        get => GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the start angle of the scale in degrees (0 = right, 90 = bottom, 180 = left, 270 = top).
    /// </summary>
    public double ScaleStartAngle
    {
        get => GetValue(ScaleStartAngleProperty);
        set => SetValue(ScaleStartAngleProperty, value);
    }

    /// <summary>
    /// Gets or sets the sweep angle of the scale in degrees.
    /// </summary>
    public double ScaleSweepAngle
    {
        get => GetValue(ScaleSweepAngleProperty);
        set => SetValue(ScaleSweepAngleProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of major divisions on the scale.
    /// </summary>
    public int MajorDivisionsCount
    {
        get => GetValue(MajorDivisionsCountProperty);
        set => SetValue(MajorDivisionsCountProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of minor divisions between major divisions.
    /// </summary>
    public int MinorDivisionsCount
    {
        get => GetValue(MinorDivisionsCountProperty);
        set => SetValue(MinorDivisionsCountProperty, value);
    }

    /// <summary>
    /// Gets or sets the length of the pointer.
    /// </summary>
    public double PointerLength
    {
        get => GetValue(PointerLengthProperty);
        set => SetValue(PointerLengthProperty, value);
    }

    /// <summary>
    /// Gets or sets the radius of the pointer cap.
    /// </summary>
    public double PointerCapRadius
    {
        get => GetValue(PointerCapRadiusProperty);
        set => SetValue(PointerCapRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the pointer color.
    /// </summary>
    public IBrush PointerColor
    {
        get => GetValue(PointerColorProperty);
        set => SetValue(PointerColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the scale color.
    /// </summary>
    public IBrush ScaleColor
    {
        get => GetValue(ScaleColorProperty);
        set => SetValue(ScaleColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the optimal range color.
    /// </summary>
    public IBrush OptimalRangeColor
    {
        get => GetValue(OptimalRangeColorProperty);
        set => SetValue(OptimalRangeColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the below optimal range color.
    /// </summary>
    public IBrush BelowOptimalRangeColor
    {
        get => GetValue(BelowOptimalRangeColorProperty);
        set => SetValue(BelowOptimalRangeColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the above optimal range color.
    /// </summary>
    public IBrush AboveOptimalRangeColor
    {
        get => GetValue(AboveOptimalRangeColorProperty);
        set => SetValue(AboveOptimalRangeColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the optimal range start value.
    /// </summary>
    public double OptimalRangeStartValue
    {
        get => GetValue(OptimalRangeStartValueProperty);
        set => SetValue(OptimalRangeStartValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the optimal range end value.
    /// </summary>
    public double OptimalRangeEndValue
    {
        get => GetValue(OptimalRangeEndValueProperty);
        set => SetValue(OptimalRangeEndValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the dial text.
    /// </summary>
    public string DialText
    {
        get => GetValue(DialTextProperty);
        set => SetValue(DialTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the unit text.
    /// </summary>
    public string Unit
    {
        get => GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show the value.
    /// </summary>
    public bool ShowValue
    {
        get => GetValue(ShowValueProperty);
        set => SetValue(ShowValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of decimal places.
    /// </summary>
    public int Decimals
    {
        get => GetValue(DecimalsProperty);
        set => SetValue(DecimalsProperty, value);
    }

    /// <summary>
    /// Gets or sets the pointer angle.
    /// </summary>
    public double PointerAngle
    {
        get => GetValue(PointerAngleProperty);
        set => SetValue(PointerAngleProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnApplyTemplate(e);

        _scaleCanvas = e.NameScope.Find<Canvas>("PART_ScaleCanvas");

        DrawScale();
        UpdatePointerAngle();
    }

    private void UpdatePointerAngle()
    {
        var clampedValue = Math.Clamp(Value, MinValue, MaxValue);
        var range = MaxValue - MinValue;
        if (range <= 0)
        {
            return;
        }

        var normalizedValue = (clampedValue - MinValue) / range;

        // Subtract 90 to convert from standard math angles (0=right) to screen angles (0=up)
        var newAngle = ScaleStartAngle + (normalizedValue * ScaleSweepAngle) - 90;

        PointerAngle = newAngle;
    }

    private void RedrawScale()
    {
        DrawScale();
        UpdatePointerAngle();
    }

    private void DrawScale()
    {
        if (_scaleCanvas == null || Bounds.Width == 0 || Bounds.Height == 0)
        {
            return;
        }

        _scaleCanvas.Children.Clear();

        // Use the actual bounds of the control for centering
        var centerX = Bounds.Width / 2;
        var centerY = Bounds.Height / 2;
        var actualRadius = Math.Min(centerX, centerY) - 10; // Leave margin for labels

        var scaleRadius = actualRadius * 0.85;
        var majorTickLength = actualRadius * 0.12;
        var minorTickLength = actualRadius * 0.06;

        var majorTickUnitAngle = ScaleSweepAngle / MajorDivisionsCount;
        var majorTicksUnitValue = (MaxValue - MinValue) / MajorDivisionsCount;

        // Draw major and minor ticks
        for (var i = 0; i <= MajorDivisionsCount; i++)
        {
            var angle = ScaleStartAngle + (i * majorTickUnitAngle);
            var angleRadian = angle * Math.PI / 180;

            // Major tick
            var startX = centerX + (scaleRadius * Math.Cos(angleRadian));
            var startY = centerY + (scaleRadius * Math.Sin(angleRadian));
            var endX = centerX + ((scaleRadius - majorTickLength) * Math.Cos(angleRadian));
            var endY = centerY + ((scaleRadius - majorTickLength) * Math.Sin(angleRadian));

            var majorTick = new Line
            {
                StartPoint = new Point(startX, startY),
                EndPoint = new Point(endX, endY),
                Stroke = ScaleColor,
                StrokeThickness = 2
            };
            _scaleCanvas.Children.Add(majorTick);

            // Scale label
            var labelRadius = scaleRadius - majorTickLength - 12;
            var labelX = centerX + (labelRadius * Math.Cos(angleRadian));
            var labelY = centerY + (labelRadius * Math.Sin(angleRadian));

            var labelValue = MinValue + (i * majorTicksUnitValue);
            var label = new TextBlock
            {
                Text = labelValue.ToString($"F{Decimals}"),
                FontSize = 9,
                Foreground = ScaleColor,
            };

            // Offset label to center it on the position
            Canvas.SetLeft(label, labelX - 12);
            Canvas.SetTop(label, labelY - 6);
            _scaleCanvas.Children.Add(label);

            // Minor ticks (except after last major tick)
            if (i < MajorDivisionsCount)
            {
                var minorTickUnitAngle = majorTickUnitAngle / MinorDivisionsCount;
                for (var j = 1; j < MinorDivisionsCount; j++)
                {
                    var minorAngle = angle + (j * minorTickUnitAngle);
                    var minorAngleRadian = minorAngle * Math.PI / 180;

                    var minorStartX = centerX + (scaleRadius * Math.Cos(minorAngleRadian));
                    var minorStartY = centerY + (scaleRadius * Math.Sin(minorAngleRadian));
                    var minorEndX = centerX + ((scaleRadius - minorTickLength) * Math.Cos(minorAngleRadian));
                    var minorEndY = centerY + ((scaleRadius - minorTickLength) * Math.Sin(minorAngleRadian));

                    var minorTick = new Line
                    {
                        StartPoint = new Point(minorStartX, minorStartY),
                        EndPoint = new Point(minorEndX, minorEndY),
                        Stroke = ScaleColor,
                        StrokeThickness = 1
                    };
                    _scaleCanvas.Children.Add(minorTick);
                }
            }
        }
    }
}
