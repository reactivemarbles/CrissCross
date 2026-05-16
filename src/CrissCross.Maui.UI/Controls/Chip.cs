// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>
/// Represents a selectable or removable chip projected from <see cref="ChipModel"/>.
/// </summary>
public class Chip : Button
{
    /// <summary>
    /// Bindable property for <see cref="Model"/>.
    /// </summary>
    public static readonly BindableProperty ModelProperty = BindableProperty.Create(
        nameof(Model),
        typeof(ChipModel),
        typeof(Chip),
        propertyChanged: OnModelChanged);

    /// <summary>
    /// Bindable property for <see cref="IsSelected"/>.
    /// </summary>
    public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
        nameof(IsSelected),
        typeof(bool),
        typeof(Chip));

    /// <summary>
    /// Bindable property for <see cref="RemoveCommand"/>.
    /// </summary>
    public static readonly BindableProperty RemoveCommandProperty = BindableProperty.Create(
        nameof(RemoveCommand),
        typeof(ICommand),
        typeof(Chip));

    /// <summary>
    /// Gets or sets the chip model projected by this control.
    /// </summary>
    public ChipModel? Model
    {
        get => (ChipModel?)GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the chip is selected.
    /// </summary>
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    /// <summary>
    /// Gets or sets a command invoked when removing the chip.
    /// </summary>
    public ICommand? RemoveCommand
    {
        get => (ICommand?)GetValue(RemoveCommandProperty);
        set => SetValue(RemoveCommandProperty, value);
    }

    private static void OnModelChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is Chip chip && newValue is ChipModel model)
        {
            chip.Text = model.Text;
            chip.IsEnabled = model.IsInteractive;
            chip.SetValue(IsSelectedProperty, model.IsSelected);
            chip.SetValue(RemoveCommandProperty, model.RemoveCommand);
        }
    }
}
