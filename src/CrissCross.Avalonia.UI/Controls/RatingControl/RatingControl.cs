// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Displays the rating scale with interactive star selection.
/// </summary>
public class RatingControl : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="Value"/>.
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty = AvaloniaProperty.Register<RatingControl, double>(
        nameof(Value), 0.0);

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
    /// Property for <see cref="StarSize"/>.
    /// </summary>
    public static readonly StyledProperty<double> StarSizeProperty = AvaloniaProperty.Register<RatingControl, double>(
        nameof(StarSize), 24.0);

    /// <summary>
    /// Property for <see cref="StarSpacing"/>.
    /// </summary>
    public static readonly StyledProperty<double> StarSpacingProperty = AvaloniaProperty.Register<RatingControl, double>(
        nameof(StarSpacing), 4.0);

    /// <summary>
    /// Property for <see cref="FilledBrush"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> FilledBrushProperty = AvaloniaProperty.Register<RatingControl, IBrush>(
        nameof(FilledBrush), Brushes.Gold);

    /// <summary>
    /// Property for <see cref="UnfilledBrush"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> UnfilledBrushProperty = AvaloniaProperty.Register<RatingControl, IBrush>(
        nameof(UnfilledBrush), Brushes.Gray);

    /// <summary>
    /// Property for <see cref="IsReadOnly"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<RatingControl, bool>(
        nameof(IsReadOnly), false);

    private global::Avalonia.Controls.StackPanel? _starsPanel;

    /// <summary>
    /// Gets or sets the rating value.
    /// </summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, Math.Clamp(value, 0, MaxRating));
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

    /// <summary>
    /// Gets or sets the star size.
    /// </summary>
    public double StarSize
    {
        get => GetValue(StarSizeProperty);
        set => SetValue(StarSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between stars.
    /// </summary>
    public double StarSpacing
    {
        get => GetValue(StarSpacingProperty);
        set => SetValue(StarSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the filled star brush.
    /// </summary>
    public IBrush FilledBrush
    {
        get => GetValue(FilledBrushProperty);
        set => SetValue(FilledBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the unfilled star brush.
    /// </summary>
    public IBrush UnfilledBrush
    {
        get => GetValue(UnfilledBrushProperty);
        set => SetValue(UnfilledBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the control is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnApplyTemplate(e);

        _starsPanel = e.NameScope.Find<global::Avalonia.Controls.StackPanel>("PART_StarsPanel");
        UpdateStars();
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ArgumentNullException.ThrowIfNull(change);

        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty || change.Property == MaxRatingProperty ||
            change.Property == FilledBrushProperty || change.Property == UnfilledBrushProperty ||
            change.Property == StarSizeProperty || change.Property == StarSpacingProperty)
        {
            UpdateStars();
        }
    }

    /// <inheritdoc/>
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPointerPressed(e);

        if (IsReadOnly || _starsPanel == null)
        {
            return;
        }

        var position = e.GetPosition(_starsPanel);
        UpdateValueFromPosition(position);
    }

    /// <inheritdoc/>
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPointerMoved(e);

        if (IsReadOnly || _starsPanel == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        var position = e.GetPosition(_starsPanel);
        UpdateValueFromPosition(position);
    }

    private void UpdateValueFromPosition(Point position)
    {
        if (_starsPanel == null || _starsPanel.Bounds.Width == 0)
        {
            return;
        }

        var starWidth = StarSize + StarSpacing;
        var starIndex = position.X / starWidth;

        if (HalfStarEnabled)
        {
            var fractionalPart = starIndex - Math.Floor(starIndex);
            Value = fractionalPart < 0.5
                ? Math.Floor(starIndex) + 0.5
                : Math.Ceiling(starIndex);
        }
        else
        {
            Value = Math.Ceiling(starIndex);
        }

        Value = Math.Clamp(Value, 0, MaxRating);
    }

    private void UpdateStars()
    {
        if (_starsPanel == null)
        {
            return;
        }

        _starsPanel.Children.Clear();

        for (var i = 0; i < MaxRating; i++)
        {
            var fillPercentage = Math.Clamp(Value - i, 0, 1);

            var starContainer = new global::Avalonia.Controls.Grid
            {
                Width = StarSize,
                Height = StarSize,
                Margin = new Thickness(0, 0, StarSpacing, 0)
            };

            // Background (unfilled) star
            var unfilledStar = CreateStarPath(UnfilledBrush);
            starContainer.Children.Add(unfilledStar);

            // Foreground (filled) star with clip
            if (fillPercentage > 0)
            {
                var filledStar = CreateStarPath(FilledBrush);
                filledStar.Clip = new RectangleGeometry(new Rect(0, 0, StarSize * fillPercentage, StarSize));
                starContainer.Children.Add(filledStar);
            }

            _starsPanel.Children.Add(starContainer);
        }
    }

    private global::Avalonia.Controls.Shapes.Path CreateStarPath(IBrush fill)
    {
        // Five-pointed star path
        return new global::Avalonia.Controls.Shapes.Path
        {
            Data = Geometry.Parse("M 12,0 L 15,9 L 24,9 L 17,15 L 20,24 L 12,18 L 4,24 L 7,15 L 0,9 L 9,9 Z"),
            Fill = fill,
            Stretch = Stretch.Uniform,
            Width = StarSize,
            Height = StarSize
        };
    }
}
