// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Rotating loading ring.
/// </summary>
public class ProgressRing : global::Avalonia.Controls.Control
{
    /// <summary>
    /// Property for <see cref="Progress"/>.
    /// </summary>
    public static readonly StyledProperty<double> ProgressProperty = AvaloniaProperty.Register<ProgressRing, double>(
        nameof(Progress), 50d);

    /// <summary>
    /// Property for <see cref="IsIndeterminate"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsIndeterminateProperty = AvaloniaProperty.Register<ProgressRing, bool>(
        nameof(IsIndeterminate), false);

    /// <summary>
    /// Property for backward compatibility with common ProgressRing templates expecting IsActive.
    /// Maps to <see cref="IsIndeterminate"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsActiveProperty = AvaloniaProperty.Register<ProgressRing, bool>(
        nameof(IsActive), false);

    /// <summary>
    /// Property for <see cref="EngAngle"/>.
    /// </summary>
    public static readonly StyledProperty<double> EngAngleProperty = AvaloniaProperty.Register<ProgressRing, double>(
        nameof(EngAngle), 180.0d);

    /// <summary>
    /// Property for <see cref="IndeterminateAngle"/>.
    /// </summary>
    public static readonly StyledProperty<double> IndeterminateAngleProperty = AvaloniaProperty.Register<ProgressRing, double>(
        nameof(IndeterminateAngle), 180.0d);

    /// <summary>
    /// Property for <see cref="CoverRingStroke"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> CoverRingStrokeProperty = AvaloniaProperty.Register<ProgressRing, IBrush>(
        nameof(CoverRingStroke), Brushes.Black);

    /// <summary>
    /// Property for <see cref="CoverRingVisibility"/>.
    /// </summary>
    public static readonly StyledProperty<bool> CoverRingVisibilityProperty = AvaloniaProperty.Register<ProgressRing, bool>(
        nameof(CoverRingVisibility), true);

    static ProgressRing()
    {
        ProgressProperty.Changed.AddClassHandler<ProgressRing>((x, e) => x.UpdateProgressAngle());
        IsIndeterminateProperty.Changed.AddClassHandler<ProgressRing>((x, e) => x.SetCurrentValue(IsActiveProperty, (bool)e.NewValue!));
        IsActiveProperty.Changed.AddClassHandler<ProgressRing>((x, e) => x.SetCurrentValue(IsIndeterminateProperty, (bool)e.NewValue!));
    }

    /// <summary>
    /// Gets or sets the progress.
    /// </summary>
    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the ring shows generic animation.
    /// </summary>
    public bool IsIndeterminate
    {
        get => GetValue(IsIndeterminateProperty);
        set => SetValue(IsIndeterminateProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the ring is active (alias of <see cref="IsIndeterminate"/>).
    /// </summary>
    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <summary>
    /// Gets or sets the arc end angle.
    /// </summary>
    public double EngAngle
    {
        get => GetValue(EngAngleProperty);
        set => SetValue(EngAngleProperty, value);
    }

    /// <summary>
    /// Gets the arc end angle when indeterminate.
    /// </summary>
    public double IndeterminateAngle
    {
        get => GetValue(IndeterminateAngleProperty);
        private set => SetValue(IndeterminateAngleProperty, value);
    }

    /// <summary>
    /// Gets background ring fill.
    /// </summary>
    public IBrush CoverRingStroke
    {
        get => GetValue(CoverRingStrokeProperty);
        private set => SetValue(CoverRingStrokeProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether gets background ring visibility.
    /// </summary>
    public bool CoverRingVisibility
    {
        get => GetValue(CoverRingVisibilityProperty);
        private set => SetValue(CoverRingVisibilityProperty, value);
    }

    /// <summary>
    /// Re-draws end angle depending on <see cref="Progress"/>.
    /// </summary>
    protected void UpdateProgressAngle()
    {
        var percentage = Progress;
        if (percentage > 100)
        {
            percentage = 100;
        }

        if (percentage < 0)
        {
            percentage = 0;
        }

        var endAngle = 3.6d * percentage;
        if (endAngle >= 360)
        {
            endAngle = 359;
        }

        EngAngle = endAngle;
    }
}
