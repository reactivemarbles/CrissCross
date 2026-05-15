// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>
/// Represents a group of chips backed by a shared <see cref="ChipGroupState"/> snapshot.
/// </summary>
public partial class ChipGroup : FlexLayout
{
    /// <summary>
    /// Bindable property for <see cref="State"/>.
    /// </summary>
    public static readonly BindableProperty StateProperty = BindableProperty.Create(
        nameof(State),
        typeof(ChipGroupState),
        typeof(ChipGroup),
        propertyChanged: OnStateChanged);

    /// <summary>
    /// Bindable property for <see cref="SelectionMode"/>.
    /// </summary>
    public static readonly BindableProperty SelectionModeProperty = BindableProperty.Create(
        nameof(SelectionMode),
        typeof(ChipGroupSelectionMode),
        typeof(ChipGroup),
        ChipGroupSelectionMode.None);

    /// <summary>
    /// Bindable property for <see cref="SelectionCommand"/>.
    /// </summary>
    public static readonly BindableProperty SelectionCommandProperty = BindableProperty.Create(
        nameof(SelectionCommand),
        typeof(ICommand),
        typeof(ChipGroup));

    /// <summary>
    /// Bindable property for <see cref="RemoveCommand"/>.
    /// </summary>
    public static readonly BindableProperty RemoveCommandProperty = BindableProperty.Create(
        nameof(RemoveCommand),
        typeof(ICommand),
        typeof(ChipGroup));

    /// <summary>
    /// Gets or sets the chip group state projected by this control.
    /// </summary>
    public ChipGroupState? State
    {
        get => (ChipGroupState?)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>
    /// Gets or sets the chip group selection mode.
    /// </summary>
    public ChipGroupSelectionMode SelectionMode
    {
        get => (ChipGroupSelectionMode)GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    /// <summary>
    /// Gets or sets a command invoked when a chip is selected.
    /// </summary>
    public ICommand? SelectionCommand
    {
        get => (ICommand?)GetValue(SelectionCommandProperty);
        set => SetValue(SelectionCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command invoked when a chip is removed.
    /// </summary>
    public ICommand? RemoveCommand
    {
        get => (ICommand?)GetValue(RemoveCommandProperty);
        set => SetValue(RemoveCommandProperty, value);
    }

    /// <summary>
    /// Selects a chip by key and invokes chip-specific or group selection commands when permitted.
    /// </summary>
    /// <param name="key">The chip key.</param>
    /// <returns><c>true</c> when the chip command path was invoked; otherwise, <c>false</c>.</returns>
    public bool SelectChip(string key)
    {
        var chip = State?.GetChip(key);
        if (chip?.IsEnabled != true)
        {
            return false;
        }

        var command = chip.SelectCommand ?? SelectionCommand;
        if (command?.CanExecute(chip.Key) != true)
        {
            return false;
        }

        command.Execute(chip.Key);
        return true;
    }

    /// <summary>
    /// Removes a chip by key and invokes chip-specific or group remove commands when permitted.
    /// </summary>
    /// <param name="key">The chip key.</param>
    /// <returns><c>true</c> when the remove command path was invoked; otherwise, <c>false</c>.</returns>
    public bool RemoveChip(string key)
    {
        var chip = State?.GetChip(key);
        if (chip?.IsEnabled != true || !chip.IsRemovable)
        {
            return false;
        }

        var command = chip.RemoveCommand ?? RemoveCommand;
        if (command?.CanExecute(chip.Key) != true)
        {
            return false;
        }

        command.Execute(chip.Key);
        return true;
    }

    private static void OnStateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ChipGroup control)
        {
            control.ApplyState(newValue as ChipGroupState);
        }
    }

    private void ApplyState(ChipGroupState? state)
    {
        Children.Clear();
        SelectionMode = state?.SelectionMode ?? ChipGroupSelectionMode.None;

        if (state is null)
        {
            return;
        }

        foreach (var chip in state.Chips)
        {
            var button = new Button
            {
                Text = chip.Text,
                IsEnabled = chip.IsEnabled,
                Command = new Command(() => SelectChip(chip.Key))
            };
            Children.Add(button);
        }
    }
}
