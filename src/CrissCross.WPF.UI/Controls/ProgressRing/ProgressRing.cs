// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Rotating loading ring.
/// </summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(ProgressRing), "ProgressRing.bmp")]
public class ProgressRing : System.Windows.Controls.Control
{
    /// <summary>
    /// Property for <see cref="Progress"/>.
    /// </summary>
    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
        nameof(Progress),
        typeof(double),
        typeof(ProgressRing),
        new PropertyMetadata(50d, PropertyChangedCallback));

    /// <summary>
    /// Property for <see cref="IsIndeterminate"/>.
    /// </summary>
    public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(
        nameof(IsIndeterminate),
        typeof(bool),
        typeof(ProgressRing),
        new PropertyMetadata(false, static (d, e) =>
        {
            if (d is ProgressRing pr && e.NewValue is bool b)
            {
                // keep IsActive in sync
                pr.SetCurrentValue(IsActiveProperty, b);
            }
        }));

    /// <summary>
    /// Property for backward compatibility with common ProgressRing templates expecting IsActive.
    /// Maps to <see cref="IsIndeterminate"/>.
    /// </summary>
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive),
        typeof(bool),
        typeof(ProgressRing),
        new PropertyMetadata(false, static (d, e) =>
        {
            if (d is ProgressRing pr && e.NewValue is bool b)
            {
                pr.SetCurrentValue(IsIndeterminateProperty, b);
            }
        }));

    /// <summary>
    /// Property for <see cref="EngAngle"/>.
    /// </summary>
    public static readonly DependencyProperty EngAngleProperty = DependencyProperty.Register(
        nameof(EngAngle),
        typeof(double),
        typeof(ProgressRing),
        new PropertyMetadata(180.0d));

    /// <summary>
    /// Property for <see cref="IndeterminateAngle"/>.
    /// </summary>
    public static readonly DependencyProperty IndeterminateAngleProperty = DependencyProperty.Register(
        nameof(IndeterminateAngle),
        typeof(double),
        typeof(ProgressRing),
        new PropertyMetadata(180.0d));

    /// <summary>
    /// Property for <see cref="CoverRingStroke"/>.
    /// </summary>
    public static readonly DependencyProperty CoverRingStrokeProperty = DependencyProperty.Register(
        nameof(CoverRingStroke),
        typeof(Brush),
        typeof(ProgressRing),
        new FrameworkPropertyMetadata(
            Brushes.Black,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender | FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Property for <see cref="CoverRingVisibility"/>.
    /// </summary>
    public static readonly DependencyProperty CoverRingVisibilityProperty = DependencyProperty.Register(
        nameof(CoverRingVisibility),
        typeof(Visibility),
        typeof(ProgressRing),
        new PropertyMetadata(System.Windows.Visibility.Visible));

    /// <summary>
    /// Gets or sets the progress.
    /// </summary>
    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the ring shows generic animation.
    /// </summary>
    public bool IsIndeterminate
    {
        get => (bool)GetValue(IsIndeterminateProperty);
        set => SetValue(IsIndeterminateProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the ring is active (alias of <see cref="IsIndeterminate"/>).
    /// </summary>
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <summary>
    /// Gets or sets the arc end angle.
    /// </summary>
    public double EngAngle
    {
        get => (double)GetValue(EngAngleProperty);
        set => SetValue(EngAngleProperty, value);
    }

    /// <summary>
    /// Gets the arc end angle when indeterminate.
    /// </summary>
    public double IndeterminateAngle
    {
        get => (double)GetValue(IndeterminateAngleProperty);
        internal set => SetValue(IndeterminateAngleProperty, value);
    }

    /// <summary>
    /// Gets or sets background ring fill.
    /// </summary>
    public Brush CoverRingStroke
    {
        get => (Brush)GetValue(CoverRingStrokeProperty);
        set => SetValue(CoverRingStrokeProperty, value);
    }

    /// <summary>
    /// Gets or sets background ring visibility.
    /// </summary>
    public Visibility CoverRingVisibility
    {
        get => (Visibility)GetValue(CoverRingVisibilityProperty);
        set => SetValue(CoverRingVisibilityProperty, value);
    }

    /// <summary>
    /// Validates the entered <see cref="Progress" /> and redraws the arc.
    /// </summary>
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
