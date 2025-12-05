// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Displays the rating scale.
/// </summary>
public class RatingControl : global::Avalonia.Controls.ContentControl
{
    /// <summary>
    /// Property for <see cref="Value"/>.
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty = AvaloniaProperty.Register<RatingControl, double>(
        nameof(Value), 0.0D);

    /// <summary>
    /// Property for <see cref="MaxRating"/>.
    /// </summary>
    public static readonly StyledProperty<int> MaxRatingProperty = AvaloniaProperty.Register<RatingControl, int>(
        nameof(MaxRating), 5);

    /// <summary>
    /// Property for <see cref="HalfStarEnabled"/>.
    /// </summary>
    public static readonly StyledProperty<bool> HalfStarEnabledProperty = AvaloniaProperty.Register<RatingControl, bool>(
        nameof(HalfStarEnabled), true);

    /// <summary>
    /// Gets or sets the rating value.
    /// </summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum allowed rating value.
    /// </summary>
    public int MaxRating
    {
        get => GetValue(MaxRatingProperty);
        set => SetValue(MaxRatingProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether half of the star can be selected.
    /// </summary>
    public bool HalfStarEnabled
    {
        get => GetValue(HalfStarEnabledProperty);
        set => SetValue(HalfStarEnabledProperty, value);
    }
}
