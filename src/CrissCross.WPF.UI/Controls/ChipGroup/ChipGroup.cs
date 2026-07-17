// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;
#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents a group of selectable or removable chips/tags.</summary>
public class ChipGroup : ItemsControl
{
    /// <summary>Property for <see cref="GroupState"/>.</summary>
    public static readonly DependencyProperty GroupStateProperty = DependencyProperty.Register(
        nameof(GroupState),
        typeof(ChipGroupState),
        typeof(ChipGroup),
        new PropertyMetadata(null, OnGroupStateChanged));

    /// <summary>Property for <see cref="SelectionMode"/>.</summary>
    public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(
        nameof(SelectionMode),
        typeof(ChipGroupSelectionMode),
        typeof(ChipGroup),
        new PropertyMetadata(ChipGroupSelectionMode.None));

    /// <summary>Property for <see cref="SelectChipCommand"/>.</summary>
    public static readonly DependencyProperty SelectChipCommandProperty = DependencyProperty.Register(
        nameof(SelectChipCommand),
        typeof(ICommand),
        typeof(ChipGroup),
        new PropertyMetadata(null));

    /// <summary>Property for <see cref="RemoveChipCommand"/>.</summary>
    public static readonly DependencyProperty RemoveChipCommandProperty = DependencyProperty.Register(
        nameof(RemoveChipCommand),
        typeof(ICommand),
        typeof(ChipGroup),
        new PropertyMetadata(null));

    /// <summary>Gets or sets the shared chip group state.</summary>
    public ChipGroupState? GroupState
    {
        get => (ChipGroupState?)GetValue(GroupStateProperty);
        set => SetValue(GroupStateProperty, value);
    }

    /// <summary>Gets or sets the selection mode used by the chip group.</summary>
    public ChipGroupSelectionMode SelectionMode
    {
        get => (ChipGroupSelectionMode)GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    /// <summary>Gets or sets the command invoked to select or toggle a chip.</summary>
    public ICommand? SelectChipCommand
    {
        get => (ICommand?)GetValue(SelectChipCommandProperty);
        set => SetValue(SelectChipCommandProperty, value);
    }

    /// <summary>Gets or sets the command invoked to remove a chip.</summary>
    public ICommand? RemoveChipCommand
    {
        get => (ICommand?)GetValue(RemoveChipCommandProperty);
        set => SetValue(RemoveChipCommandProperty, value);
    }

    /// <summary>Provides the OnGroupStateChanged member.</summary>
    /// <param name="dependencyObject">The dependencyObject value.</param>
    /// <param name="args">The event arguments.</param>
    private static void OnGroupStateChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not ChipGroup chipGroup || args.NewValue is not ChipGroupState state)
        {
            return;
        }

        chipGroup.SetCurrentValue(SelectionModeProperty, state.SelectionMode);
        chipGroup.SetCurrentValue(ItemsSourceProperty, state.Chips);
    }
}
