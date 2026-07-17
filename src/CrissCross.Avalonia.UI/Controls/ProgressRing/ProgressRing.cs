// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Rotating loading ring.</summary>
public class ProgressRing : TemplatedControl
{
    /// <summary>Property for <see cref="Progress"/>.</summary>
    public static readonly StyledProperty<double> ProgressProperty = AvaloniaProperty.Register<ProgressRing, double>(
        nameof(Progress),
        50D);

    /// <summary>Property for <see cref="IsIndeterminate"/>.</summary>
    public static readonly StyledProperty<bool> IsIndeterminateProperty = AvaloniaProperty.Register<ProgressRing, bool>(
        nameof(IsIndeterminate),
        false);

    /// <summary>Property for backward compatibility with common ProgressRing templates expecting IsActive.</summary>
    public static readonly StyledProperty<bool> IsActiveProperty = AvaloniaProperty.Register<ProgressRing, bool>(
        nameof(IsActive),
        false);

    /// <summary>Property for <see cref="EngAngle"/>.</summary>
    public static readonly StyledProperty<double> EngAngleProperty = AvaloniaProperty.Register<ProgressRing, double>(
        nameof(EngAngle),
        180.0D);

    /// <summary>Property for <see cref="IndeterminateAngle"/>.</summary>
    public static readonly StyledProperty<double> IndeterminateAngleProperty = AvaloniaProperty.Register<
        ProgressRing,
        double
    >(nameof(IndeterminateAngle), 180.0D);

    /// <summary>Property for <see cref="CoverRingStroke"/>.</summary>
    public static readonly StyledProperty<IBrush> CoverRingStrokeProperty = AvaloniaProperty.Register<
        ProgressRing,
        IBrush
    >(nameof(CoverRingStroke), Brushes.Black);

    /// <summary>Property for <see cref="CoverRingVisibility"/>.</summary>
    public static readonly StyledProperty<bool> CoverRingVisibilityProperty = AvaloniaProperty.Register<
        ProgressRing,
        bool
    >(nameof(CoverRingVisibility), true);

    /// <summary>Minimum accepted progress percentage.</summary>
    private const double MinimumProgress = 0D;

    /// <summary>Maximum accepted progress percentage.</summary>
    private const double MaximumProgress = 100D;

    /// <summary>Degrees represented by one percentage point.</summary>
    private const double DegreesPerPercent = 3.6D;

    /// <summary>Degrees in a full circle.</summary>
    private const double FullCircleDegrees = 360D;

    /// <summary>Maximum rendered arc angle.</summary>
    private const double MaximumArcDegrees = 359D;

    /// <summary>Provides the ProgressRing member.</summary>
    static ProgressRing()
    {
        _ = ProgressProperty.Changed.AddClassHandler<ProgressRing>((x, e) => x.UpdateProgressAngle());
        _ = IsIndeterminateProperty.Changed.AddClassHandler<ProgressRing>(
            (x, e) => x.SetCurrentValue(IsActiveProperty, (bool)e.NewValue!));
        _ = IsActiveProperty.Changed.AddClassHandler<ProgressRing>(
            (x, e) => x.SetCurrentValue(IsIndeterminateProperty, (bool)e.NewValue!));
    }

    /// <summary>Gets or sets the progress.</summary>
    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the ring shows generic animation.</summary>
    public bool IsIndeterminate
    {
        get => GetValue(IsIndeterminateProperty);
        set => SetValue(IsIndeterminateProperty, value);
    }

    /// <summary>Gets or sets whether the ring is active (alias of IsIndeterminate).</summary>
    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <summary>Gets or sets the arc end angle.</summary>
    public double EngAngle
    {
        get => GetValue(EngAngleProperty);
        set => SetValue(EngAngleProperty, value);
    }

    /// <summary>Gets or sets the arc end angle when indeterminate.</summary>
    public double IndeterminateAngle
    {
        get => GetValue(IndeterminateAngleProperty);
        set => SetValue(IndeterminateAngleProperty, value);
    }

    /// <summary>Gets or sets background ring fill.</summary>
    public IBrush CoverRingStroke
    {
        get => GetValue(CoverRingStrokeProperty);
        set => SetValue(CoverRingStrokeProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether background ring is visible.</summary>
    public bool CoverRingVisibility
    {
        get => GetValue(CoverRingVisibilityProperty);
        set => SetValue(CoverRingVisibilityProperty, value);
    }

    /// <summary>Re-draws end angle depending on <see cref="Progress"/>.</summary>
    protected void UpdateProgressAngle()
    {
        var percentage = Progress;
        if (percentage > MaximumProgress)
        {
            percentage = MaximumProgress;
        }

        if (percentage < MinimumProgress)
        {
            percentage = MinimumProgress;
        }

        var endAngle = DegreesPerPercent * percentage;
        if (endAngle >= FullCircleDegrees)
        {
            endAngle = MaximumArcDegrees;
        }

        EngAngle = endAngle;
    }
}
