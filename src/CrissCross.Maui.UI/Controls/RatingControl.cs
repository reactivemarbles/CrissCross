// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays a keyboard-accessible star rating with optional reactive command projection.</summary>
public class RatingControl : HorizontalStackLayout
{
    /// <summary>Bindable property for <see cref="Value"/>.</summary>
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(int),
        typeof(RatingControl),
        0,
        BindingMode.TwoWay,
        propertyChanged: static (view, _, _) => ((RatingControl)view).Refresh());

    /// <summary>Bindable property for <see cref="MaxRating"/>.</summary>
    public static readonly BindableProperty MaxRatingProperty = BindableProperty.Create(
        nameof(MaxRating),
        typeof(int),
        typeof(RatingControl),
        5,
        propertyChanged: static (view, _, _) => ((RatingControl)view).Refresh());

    /// <summary>Bindable property for <see cref="ValueChangedCommand"/>.</summary>
    public static readonly BindableProperty ValueChangedCommandProperty = BindableProperty.Create(nameof(ValueChangedCommand), typeof(ICommand), typeof(RatingControl));

    /// <summary>Bindable property for <see cref="IsReadOnly"/>.</summary>
    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(
        nameof(IsReadOnly),
        typeof(bool),
        typeof(RatingControl),
        false,
        propertyChanged: static (view, _, _) => ((RatingControl)view).Refresh());

    /// <summary>Gets or sets the selected whole-star rating.</summary>
    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>Gets or sets the number of stars available for selection.</summary>
    public int MaxRating
    {
        get => (int)GetValue(MaxRatingProperty);
        set => SetValue(MaxRatingProperty, value);
    }

    /// <summary>Gets or sets the command executed after a user selects a rating.</summary>
    public ICommand? ValueChangedCommand
    {
        get => (ICommand?)GetValue(ValueChangedCommandProperty);
        set => SetValue(ValueChangedCommandProperty, value);
    }

    /// <summary>Gets or sets whether user-driven rating changes are disabled.</summary>
    public new bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>Applies a user-selected rating and forwards it through <see cref="ValueChangedCommand"/>.</summary>
    /// <param name="value">The requested rating.</param>
    public void SetRating(int value)
    {
        if (IsReadOnly)
        {
            return;
        }

        Value = Math.Clamp(value, 0, Math.Max(MaxRating, 1));
        if (ValueChangedCommand?.CanExecute(Value) != true)
        {
            return;
        }

        ValueChangedCommand.Execute(Value);
    }

    /// <summary>Rebuilds the native buttons from the current bindable-property snapshot.</summary>
    private void Refresh()
    {
        Children.Clear();
        var maximum = Math.Max(MaxRating, 1);
        if (Value != Math.Clamp(Value, 0, maximum))
        {
            Value = Math.Clamp(Value, 0, maximum);
            return;
        }

        for (var index = 1; index <= maximum; index++)
        {
            var rating = index;
            var button = new Button { Text = rating <= Value ? "★" : "☆", IsEnabled = !IsReadOnly, Command = new Command(() => SetRating(rating)), };
            SemanticProperties.SetDescription(button, $"{rating} of {maximum} stars");
            Children.Add(button);
        }
    }
}
