// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Rotating loading ring.</summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(ProgressRing), "ProgressRing.bmp")]
public class ProgressRing : System.Windows.Controls.Control
{
    /// <summary>Property for <see cref="Progress"/>.</summary>
    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
        nameof(Progress),
        typeof(double),
        typeof(ProgressRing),
        new PropertyMetadata(DefaultProgress, PropertyChangedCallback));

    /// <summary>Property for <see cref="IsIndeterminate"/>.</summary>
    public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(
        nameof(IsIndeterminate),
        typeof(bool),
        typeof(ProgressRing),
        new PropertyMetadata(false, Callbacks.OnIsIndeterminateChanged));

    /// <summary>Property for backward compatibility with common ProgressRing templates expecting IsActive.</summary>
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive),
        typeof(bool),
        typeof(ProgressRing),
        new PropertyMetadata(false, Callbacks.OnIsActiveChanged));

    /// <summary>Property for <see cref="EngAngle"/>.</summary>
    public static readonly DependencyProperty EngAngleProperty = DependencyProperty.Register(
        nameof(EngAngle),
        typeof(double),
        typeof(ProgressRing),
        new PropertyMetadata(180.0D));

    /// <summary>Property for <see cref="IndeterminateAngle"/>.</summary>
    public static readonly DependencyProperty IndeterminateAngleProperty = DependencyProperty.Register(
        nameof(IndeterminateAngle),
        typeof(double),
        typeof(ProgressRing),
        new PropertyMetadata(180.0D));

    /// <summary>Property for <see cref="CoverRingStroke"/>.</summary>
    public static readonly DependencyProperty CoverRingStrokeProperty = DependencyProperty.Register(
        nameof(CoverRingStroke),
        typeof(Brush),
        typeof(ProgressRing),
        new FrameworkPropertyMetadata(
            Brushes.Black,
            FrameworkPropertyMetadataOptions.AffectsRender
                | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender
                | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>Property for <see cref="CoverRingVisibility"/>.</summary>
    public static readonly DependencyProperty CoverRingVisibilityProperty = DependencyProperty.Register(
        nameof(CoverRingVisibility),
        typeof(Visibility),
        typeof(ProgressRing),
        new PropertyMetadata(System.Windows.Visibility.Visible));

    /// <summary>Default progress value.</summary>
    private const double DefaultProgress = 50.0D;

    /// <summary>Maximum supported progress value.</summary>
    private const double MaximumProgress = 100.0D;

    /// <summary>Minimum supported progress value.</summary>
    private const double MinimumProgress = 0.0D;

    /// <summary>Degree multiplier for a single progress percentage point.</summary>
    private const double DegreesPerProgressPercent = 3.6D;

    /// <summary>Degrees in a full circle.</summary>
    private const double FullCircleDegrees = 360.0D;

    /// <summary>Maximum arc angle before the path closes visually.</summary>
    private const double MaximumArcDegrees = 359.0D;

    /// <summary>Gets or sets the progress.</summary>
    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the ring shows generic animation.</summary>
    public bool IsIndeterminate
    {
        get => (bool)GetValue(IsIndeterminateProperty);
        set => SetValue(IsIndeterminateProperty, value);
    }

    /// <summary>Gets or sets whether the ring is active (alias of IsIndeterminate).</summary>
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <summary>Gets or sets the arc end angle.</summary>
    public double EngAngle
    {
        get => (double)GetValue(EngAngleProperty);
        set => SetValue(EngAngleProperty, value);
    }

    /// <summary>Gets the arc end angle when indeterminate.</summary>
    public double IndeterminateAngle
    {
        get => (double)GetValue(IndeterminateAngleProperty);
        internal set => SetValue(IndeterminateAngleProperty, value);
    }

    /// <summary>Gets or sets background ring fill.</summary>
    public Brush CoverRingStroke
    {
        get => (Brush)GetValue(CoverRingStrokeProperty);
        set => SetValue(CoverRingStrokeProperty, value);
    }

    /// <summary>Gets or sets background ring visibility.</summary>
    public Visibility CoverRingVisibility
    {
        get => (Visibility)GetValue(CoverRingVisibilityProperty);
        set => SetValue(CoverRingVisibilityProperty, value);
    }

    /// <summary>Validates the entered <see cref="Progress" /> and redraws the arc.</summary>
    /// <param name="d">The d.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    protected static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ProgressRing control)
        {
            return;
        }

        control.UpdateProgressAngle();
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

        var endAngle = DegreesPerProgressPercent * percentage;
        if (endAngle >= FullCircleDegrees)
        {
            endAngle = MaximumArcDegrees;
        }

        EngAngle = endAngle;
    }

    /// <summary>Contains dependency-property callbacks that must run after static field initialization.</summary>
    private static class Callbacks
    {
        /// <summary>Keeps <see cref="IsActive"/> synchronized with <see cref="IsIndeterminate"/>.</summary>
        /// <param name="dependencyObject">The progress ring.</param>
        /// <param name="e">The property-change data.</param>
        public static void OnIsIndeterminateChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not ProgressRing progressRing || e.NewValue is not bool isIndeterminate)
            {
                return;
            }

            progressRing.SetCurrentValue(IsActiveProperty, isIndeterminate);
        }

        /// <summary>Keeps <see cref="IsIndeterminate"/> synchronized with <see cref="IsActive"/>.</summary>
        /// <param name="dependencyObject">The progress ring.</param>
        /// <param name="e">The property-change data.</param>
        public static void OnIsActiveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not ProgressRing progressRing || e.NewValue is not bool isActive)
            {
                return;
            }

            progressRing.SetCurrentValue(IsIndeterminateProperty, isActive);
        }
    }
}
