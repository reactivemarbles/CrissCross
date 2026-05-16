// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using CrissCross;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a group of selectable or removable chips/tags.
/// </summary>
public class ChipGroup : ItemsControl
{
    /// <summary>
    /// Property for <see cref="GroupState"/>.
    /// </summary>
    public static readonly StyledProperty<ChipGroupState?> GroupStateProperty = AvaloniaProperty.Register<ChipGroup, ChipGroupState?>(nameof(GroupState));

    /// <summary>
    /// Property for <see cref="SelectionMode"/>.
    /// </summary>
    public static readonly StyledProperty<ChipGroupSelectionMode> SelectionModeProperty = AvaloniaProperty.Register<ChipGroup, ChipGroupSelectionMode>(nameof(SelectionMode));

    /// <summary>
    /// Property for <see cref="SelectChipCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> SelectChipCommandProperty = AvaloniaProperty.Register<ChipGroup, ICommand?>(nameof(SelectChipCommand));

    /// <summary>
    /// Property for <see cref="RemoveChipCommand"/>.
    /// </summary>
    public static readonly StyledProperty<ICommand?> RemoveChipCommandProperty = AvaloniaProperty.Register<ChipGroup, ICommand?>(nameof(RemoveChipCommand));

    /// <summary>
    /// Gets or sets the shared chip group state.
    /// </summary>
    public ChipGroupState? GroupState
    {
        get => GetValue(GroupStateProperty);
        set => SetValue(GroupStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the selection mode used by the chip group.
    /// </summary>
    public ChipGroupSelectionMode SelectionMode
    {
        get => GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked to select or toggle a chip.
    /// </summary>
    public ICommand? SelectChipCommand
    {
        get => GetValue(SelectChipCommandProperty);
        set => SetValue(SelectChipCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked to remove a chip.
    /// </summary>
    public ICommand? RemoveChipCommand
    {
        get => GetValue(RemoveChipCommandProperty);
        set => SetValue(RemoveChipCommandProperty, value);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ArgumentNullException.ThrowIfNull(change);

        base.OnPropertyChanged(change);

        if (change.Property == GroupStateProperty && change.GetNewValue<ChipGroupState?>() is { } state)
        {
            SelectionMode = state.SelectionMode;
            ItemsSource = state.Chips;
        }
    }
}
