// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using System.Windows.Input;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Displays the rating scale with interactions.</summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(RatingControl), "RatingControl.bmp")]
[TemplatePart(Name = "PART_Star1", Type = typeof(SymbolIcon))]
[TemplatePart(Name = "PART_Star2", Type = typeof(SymbolIcon))]
[TemplatePart(Name = "PART_Star3", Type = typeof(SymbolIcon))]
[TemplatePart(Name = "PART_Star4", Type = typeof(SymbolIcon))]
[TemplatePart(Name = "PART_Star5", Type = typeof(SymbolIcon))]
public class RatingControl : System.Windows.Controls.ContentControl
{
    /// <summary>Property for <see cref="Value"/>.</summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(double),
        typeof(RatingControl),
        new PropertyMetadata(0.0D, OnValuePropertyChanged));

    /// <summary>Property for <see cref="MaxRating"/>.</summary>
    public static readonly DependencyProperty MaxRatingProperty = DependencyProperty.Register(
        nameof(MaxRating),
        typeof(int),
        typeof(RatingControl),
        new PropertyMetadata(StarCount));

    /// <summary>Property for <see cref="HalfStarEnabled"/>.</summary>
    public static readonly DependencyProperty HalfStarEnabledProperty = DependencyProperty.Register(
        nameof(HalfStarEnabled),
        typeof(bool),
        typeof(RatingControl),
        new PropertyMetadata(true));

    /// <summary>Routed event for <see cref="ValueChanged"/>.</summary>
    public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(RatingControl));

    /// <summary>Provides the number of stars displayed by the control.</summary>
    private const int StarCount = 5;

    /// <summary>Provides the number of selectable rating units per star.</summary>
    private const int RatingUnitsPerStar = 2;

    /// <summary>Provides the half-star rating increment.</summary>
    private const double HalfStarValue = 0.5D;

    /// <summary>Provides the percentage scale used for pointer offsets.</summary>
    private const int PercentageScale = 100;

    /// <summary>Provides the percentage represented by one selectable rating unit.</summary>
    private const int PercentagePerRatingUnit = PercentageScale / (StarCount * RatingUnitsPerStar);

    /// <summary>Provides the first star index.</summary>
    private const int FirstStarIndex = 0;

    /// <summary>Provides the second star index.</summary>
    private const int SecondStarIndex = 1;

    /// <summary>Provides the third star index.</summary>
    private const int ThirdStarIndex = 2;

    /// <summary>Provides the fourth star index.</summary>
    private const int FourthStarIndex = 3;

    /// <summary>Provides the fifth star index.</summary>
    private const int FifthStarIndex = 4;

    /// <summary>Provides the MaxValue member.</summary>
    private const double MaxValue = StarCount;

    /// <summary>Provides the MinValue member.</summary>
    private const double MinValue = 0.0D;

    /// <summary>Provides the OffsetTolerance member.</summary>
    private const int OffsetTolerance = 8;

    /// <summary>Provides the StarSymbol member.</summary>
    private const SymbolRegular StarSymbol = SymbolRegular.Star28;

    /// <summary>Provides the StarHalfSymbol member.</summary>
    private const SymbolRegular StarHalfSymbol = SymbolRegular.StarHalf28;

    /// <summary>Stores the _symbolIconStarOne value.</summary>
    private SymbolIcon? _symbolIconStarOne;

    /// <summary>Stores the _symbolIconStarTwo value.</summary>
    private SymbolIcon? _symbolIconStarTwo;

    /// <summary>Stores the _symbolIconStarThree value.</summary>
    private SymbolIcon? _symbolIconStarThree;

    /// <summary>Stores the _symbolIconStarFour value.</summary>
    private SymbolIcon? _symbolIconStarFour;

    /// <summary>Stores the _symbolIconStarFive value.</summary>
    private SymbolIcon? _symbolIconStarFive;

    /// <summary>Occurs after the user selects the rating.</summary>
    public event RoutedEventHandler ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    /// <summary>Provides the StarValue member.</summary>
    private enum StarValue
    {
        /// <summary>Represents the Empty value.</summary>
        Empty,

        /// <summary>Represents the HalfFilled value.</summary>
        HalfFilled,

        /// <summary>Represents the Filled value.</summary>
        Filled
    }

    /// <summary>Gets or sets the rating value.</summary>
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>Gets or sets the maximum allowed rating value.</summary>
    public int MaxRating
    {
        get => (int)GetValue(MaxRatingProperty);
        set => SetValue(MaxRatingProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether gets or sets the value deciding whether half of the star can be selected.</summary>
    public bool HalfStarEnabled
    {
        get => (bool)GetValue(HalfStarEnabledProperty);
        set => SetValue(HalfStarEnabledProperty, value);
    }

    /// <summary>Is called when Template is changed.</summary>
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

    /// <summary>Is called when <see cref="Value" /> changes.</summary>
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

    /// <summary>Is called when mouse is moved away from the control.</summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);

        UpdateStarsFromValue();
    }

    /// <summary>Is called when mouse is moved around the control.</summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (e is null)
        {
            return;
        }

        var currentPossition = e.GetPosition(this);
        var mouseOffset = currentPossition.X * PercentageScale / ActualWidth;

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            return;
        }

        UpdateStarsOnMousePreview(mouseOffset);
    }

    /// <summary>Is called when mouse is cliked down.</summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        if (e is null)
        {
            return;
        }

        var currentPossition = e.GetPosition(this);
        var mouseOffset = currentPossition.X * PercentageScale / ActualWidth;

        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        UpdateStarsOnMouseClick(mouseOffset);
    }

    /// <summary>Is called after lifting a keyboard key.</summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        if (e is null)
        {
            return;
        }

        if ((e.Key == Key.Right || e.Key == Key.Up) && Value < MaxValue)
        {
            Value += HalfStarEnabled ? HalfStarValue : 1;
        }

        if ((e.Key != Key.Left && e.Key != Key.Down) || Value <= MinValue)
        {
            return;
        }

        Value -= HalfStarEnabled ? HalfStarValue : 1;
    }

    /// <summary>Provides the OnValuePropertyChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not RatingControl ratingControl)
        {
            return;
        }

        ratingControl.OnValueChanged((double)e.OldValue);
    }

    /// <summary>Provides the UpdateStarsOnMousePreview member.</summary>
    /// <param name="offsetPercentage">The offsetPercentage value.</param>
    private void UpdateStarsOnMousePreview(double offsetPercentage) => SetStarsPresence(ExtractValueFromOffset(offsetPercentage));

    /// <summary>Provides the UpdateStarsOnMouseClick member.</summary>
    /// <param name="offsetPercentage">The offsetPercentage value.</param>
    private void UpdateStarsOnMouseClick(double offsetPercentage)
    {
        var currentValue = ExtractValueFromOffset(offsetPercentage);

        Value = currentValue / RatingUnitsPerStar;
    }

    /// <summary>Provides the UpdateStarsFromValue member.</summary>
    private void UpdateStarsFromValue() => SetStarsPresence(ExtractValueFromOffset(Value * PercentageScale / MaxValue));

    /// <summary>Provides the SetStarsPresence member.</summary>
    /// <param name="index">The index value.</param>
    private void SetStarsPresence(int index)
    {
        for (var starIndex = FirstStarIndex; starIndex < StarCount; starIndex++)
        {
            var filledThreshold = (starIndex + 1) * RatingUnitsPerStar;
            var starValue = StarValue.Empty;
            if (index >= filledThreshold)
            {
                starValue = StarValue.Filled;
            }
            else if (index == filledThreshold - 1)
            {
                starValue = StarValue.HalfFilled;
            }

            UpdateStar(starIndex, starValue);
        }
    }

    /// <summary>Provides the UpdateStar member.</summary>
    /// <param name="starIndex">The starIndex value.</param>
    /// <param name="starValue">The starvalue.</param>
    private void UpdateStar(int starIndex, StarValue starValue)
    {
        var selectedIcon = starIndex switch
        {
            SecondStarIndex => _symbolIconStarTwo,
            ThirdStarIndex => _symbolIconStarThree,
            FourthStarIndex => _symbolIconStarFour,
            FifthStarIndex => _symbolIconStarFive,
            _ => _symbolIconStarOne,
        };

        if (selectedIcon is null)
        {
            return;
        }

        switch (starValue)
        {
            case StarValue.HalfFilled:
                {
                    selectedIcon.Filled = false;
                    selectedIcon.Symbol = StarHalfSymbol;
                    break;
                }

            case StarValue.Filled:
                {
                    selectedIcon.Filled = true;
                    selectedIcon.Symbol = StarSymbol;
                    break;
                }

            default:
                {
                    selectedIcon.Filled = false;
                    selectedIcon.Symbol = StarSymbol;
                    break;
                }
        }
    }

    /// <summary>Provides the ExtractValueFromOffset member.</summary>
    /// <param name="offset">The offset value.</param>
    /// <returns>The result.</returns>
    private int ExtractValueFromOffset(double offset)
    {
        var starValue = (int)(offset + OffsetTolerance) / PercentagePerRatingUnit;

        if (!HalfStarEnabled)
        {
            if (starValue < RatingUnitsPerStar)
            {
                return 0;
            }

            if (starValue % RatingUnitsPerStar != 0)
            {
                starValue++;
            }
        }

        return starValue;
    }
}
