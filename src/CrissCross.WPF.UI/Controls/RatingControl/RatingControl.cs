// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using System.Windows.Input;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Displays the rating scale with interactions.
/// </summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(RatingControl), "RatingControl.bmp")]
[TemplatePart(Name = "PART_Star1", Type = typeof(SymbolIcon))]
[TemplatePart(Name = "PART_Star2", Type = typeof(SymbolIcon))]
[TemplatePart(Name = "PART_Star3", Type = typeof(SymbolIcon))]
[TemplatePart(Name = "PART_Star4", Type = typeof(SymbolIcon))]
[TemplatePart(Name = "PART_Star5", Type = typeof(SymbolIcon))]
public class RatingControl : System.Windows.Controls.ContentControl
{
    /// <summary>
    /// Property for <see cref="Value"/>.
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(double),
        typeof(RatingControl),
        new PropertyMetadata(0.0D, OnValuePropertyChanged));

    /// <summary>
    /// Property for <see cref="MaxRating"/>.
    /// </summary>
    public static readonly DependencyProperty MaxRatingProperty = DependencyProperty.Register(
        nameof(MaxRating),
        typeof(int),
        typeof(RatingControl),
        new PropertyMetadata(5));

    /// <summary>
    /// Property for <see cref="HalfStarEnabled"/>.
    /// </summary>
    public static readonly DependencyProperty HalfStarEnabledProperty = DependencyProperty.Register(
        nameof(HalfStarEnabled),
        typeof(bool),
        typeof(RatingControl),
        new PropertyMetadata(true));

    /// <summary>
    /// Routed event for <see cref="ValueChanged"/>.
    /// </summary>
    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(RatingControl));

    private const double MaxValue = 5.0D;
    private const double MinValue = 0.0D;
    private const int OffsetTolerance = 8;
    private const SymbolRegular StarSymbol = SymbolRegular.Star28;
    private const SymbolRegular StarHalfSymbol = SymbolRegular.StarHalf28;

    private SymbolIcon? _symbolIconStarOne;
    private SymbolIcon? _symbolIconStarTwo;
    private SymbolIcon? _symbolIconStarThree;
    private SymbolIcon? _symbolIconStarFour;
    private SymbolIcon? _symbolIconStarFive;

    /// <summary>
    /// Occurs after the user selects the rating.
    /// </summary>
    public event RoutedEventHandler ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    private enum StarValue
    {
        Empty,
        HalfFilled,
        Filled
    }

    /// <summary>
    /// Gets or sets the rating value.
    /// </summary>
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum allowed rating value.
    /// </summary>
    public int MaxRating
    {
        get => (int)GetValue(MaxRatingProperty);
        set => SetValue(MaxRatingProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets the value deciding whether half of the star can be selected.
    /// </summary>
    public bool HalfStarEnabled
    {
        get => (bool)GetValue(HalfStarEnabledProperty);
        set => SetValue(HalfStarEnabledProperty, value);
    }

    /// <summary>
    /// Is called when Template is changed.
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_Star1") is SymbolIcon starOne)
        {
            _symbolIconStarOne = starOne;
        }

        if (GetTemplateChild("PART_Star2") is SymbolIcon starTwo)
        {
            _symbolIconStarTwo = starTwo;
        }

        if (GetTemplateChild("PART_Star3") is SymbolIcon starThree)
        {
            _symbolIconStarThree = starThree;
        }

        if (GetTemplateChild("PART_Star4") is SymbolIcon starFour)
        {
            _symbolIconStarFour = starFour;
        }

        if (GetTemplateChild("PART_Star5") is SymbolIcon starFive)
        {
            _symbolIconStarFive = starFive;
        }

        UpdateStarsFromValue();
    }

    /// <summary>
    /// Is called when <see cref="Value" /> changes.
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    protected virtual void OnValueChanged(double oldValue)
    {
        if (Value > MaxValue)
        {
            Value = MaxValue;

            return;
        }

        if (Value < MinValue)
        {
            Value = MinValue;

            return;
        }

        if (!Value.Equals(oldValue))
        {
            RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
        }

        UpdateStarsFromValue();
    }

    /// <summary>
    /// Is called when mouse is moved away from the control.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);

        UpdateStarsFromValue();
    }

    /// <summary>
    /// Is called when mouse is moved around the control.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (e == null)
        {
            return;
        }

        var currentPossition = e.GetPosition(this);
        var mouseOffset = currentPossition.X * 100 / ActualWidth;

        if (e.LeftButton != MouseButtonState.Pressed)
        {
            UpdateStarsOnMousePreview(mouseOffset);
        }
    }

    /// <summary>
    /// Is called when mouse is cliked down.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        if (e == null)
        {
            return;
        }

        var currentPossition = e.GetPosition(this);
        var mouseOffset = currentPossition.X * 100 / ActualWidth;

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            UpdateStarsOnMouseClick(mouseOffset);
        }
    }

    /// <summary>
    /// Is called after lifting a keyboard key.
    /// </summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        if (e == null)
        {
            return;
        }

        if ((e.Key == Key.Right || e.Key == Key.Up) && Value < MaxValue)
        {
            Value += HalfStarEnabled ? 0.5D : 1;
        }

        if ((e.Key == Key.Left || e.Key == Key.Down) && Value > MinValue)
        {
            Value -= HalfStarEnabled ? 0.5D : 1;
        }
    }

    private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not RatingControl ratingControl)
        {
            return;
        }

        ratingControl.OnValueChanged((double)e.OldValue);
    }

    private void UpdateStarsOnMousePreview(double offsetPercentage) => SetStarsPresence(ExtractValueFromOffset(offsetPercentage));

    private void UpdateStarsOnMouseClick(double offsetPercentage)
    {
        var currentValue = ExtractValueFromOffset(offsetPercentage);

        Value = currentValue / 2D;
    }

    private void UpdateStarsFromValue() => SetStarsPresence(ExtractValueFromOffset(Value * 100 / 5));

    private void SetStarsPresence(int index)
    {
        switch (index)
        {
            case 10:
                UpdateStar(4, StarValue.Filled);
                UpdateStar(3, StarValue.Filled);
                UpdateStar(2, StarValue.Filled);
                UpdateStar(1, StarValue.Filled);
                UpdateStar(0, StarValue.Filled);
                break;

            case 9:
                UpdateStar(4, StarValue.HalfFilled);
                UpdateStar(3, StarValue.Filled);
                UpdateStar(2, StarValue.Filled);
                UpdateStar(1, StarValue.Filled);
                UpdateStar(0, StarValue.Filled);
                break;

            case 8:
                UpdateStar(4, StarValue.Empty);
                UpdateStar(3, StarValue.Filled);
                UpdateStar(2, StarValue.Filled);
                UpdateStar(1, StarValue.Filled);
                UpdateStar(0, StarValue.Filled);
                break;

            case 7:
                UpdateStar(4, StarValue.Empty);
                UpdateStar(3, StarValue.HalfFilled);
                UpdateStar(2, StarValue.Filled);
                UpdateStar(1, StarValue.Filled);
                UpdateStar(0, StarValue.Filled);
                break;

            case 6:
                UpdateStar(4, StarValue.Empty);
                UpdateStar(3, StarValue.Empty);
                UpdateStar(2, StarValue.Filled);
                UpdateStar(1, StarValue.Filled);
                UpdateStar(0, StarValue.Filled);
                break;

            case 5:
                UpdateStar(4, StarValue.Empty);
                UpdateStar(3, StarValue.Empty);
                UpdateStar(2, StarValue.HalfFilled);
                UpdateStar(1, StarValue.Filled);
                UpdateStar(0, StarValue.Filled);
                break;

            case 4:
                UpdateStar(4, StarValue.Empty);
                UpdateStar(3, StarValue.Empty);
                UpdateStar(2, StarValue.Empty);
                UpdateStar(1, StarValue.Filled);
                UpdateStar(0, StarValue.Filled);
                break;

            case 3:
                UpdateStar(4, StarValue.Empty);
                UpdateStar(3, StarValue.Empty);
                UpdateStar(2, StarValue.Empty);
                UpdateStar(1, StarValue.HalfFilled);
                UpdateStar(0, StarValue.Filled);
                break;

            case 2:
                UpdateStar(4, StarValue.Empty);
                UpdateStar(3, StarValue.Empty);
                UpdateStar(2, StarValue.Empty);
                UpdateStar(1, StarValue.Empty);
                UpdateStar(0, StarValue.Filled);
                break;

            case 1:
                UpdateStar(4, StarValue.Empty);
                UpdateStar(3, StarValue.Empty);
                UpdateStar(2, StarValue.Empty);
                UpdateStar(1, StarValue.Empty);
                UpdateStar(0, StarValue.HalfFilled);
                break;

            default:
                UpdateStar(4, StarValue.Empty);
                UpdateStar(3, StarValue.Empty);
                UpdateStar(2, StarValue.Empty);
                UpdateStar(1, StarValue.Empty);
                UpdateStar(0, StarValue.Empty);
                break;
        }
    }

    private void UpdateStar(int starIndex, StarValue starValue)
    {
        var selectedIcon = starIndex switch
        {
            1 => _symbolIconStarTwo,
            2 => _symbolIconStarThree,
            3 => _symbolIconStarFour,
            4 => _symbolIconStarFive,
            _ => _symbolIconStarOne,
        };

        if (selectedIcon is null)
        {
            return;
        }

        switch (starValue)
        {
            case StarValue.HalfFilled:
                selectedIcon.Filled = false;
                selectedIcon.Symbol = StarHalfSymbol;
                break;

            case StarValue.Filled:
                selectedIcon.Filled = true;
                selectedIcon.Symbol = StarSymbol;
                break;

            default:
                selectedIcon.Filled = false;
                selectedIcon.Symbol = StarSymbol;
                break;
        }
    }

    private int ExtractValueFromOffset(double offset)
    {
        var starValue = (int)(offset + OffsetTolerance) / 10;

        if (!HalfStarEnabled)
        {
            if (starValue < 2)
            {
                return 0;
            }

            if (starValue % 2 != 0)
            {
                starValue++;
            }
        }

        return starValue;
    }
}
