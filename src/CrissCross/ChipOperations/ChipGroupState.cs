// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Represents platform-neutral state for a group of chip/tag items.</summary>
public sealed class ChipGroupState
{
    /// <inheritdoc cref="ChipGroupState(IEnumerable{ChipModel}, ChipGroupSelectionMode)"/>
    public ChipGroupState(IEnumerable<ChipModel> chips)
        : this(chips, ChipGroupSelectionMode.None) { }

    /// <summary>Initializes a new instance of the <see cref="ChipGroupState"/> class.</summary>
    /// <param name="chips">The chips displayed by the group.</param>
    /// <param name="selectionMode">The selection mode used by the group.</param>
    public ChipGroupState(IEnumerable<ChipModel> chips, ChipGroupSelectionMode selectionMode)
    {
        ThrowHelper.ThrowIfNull(chips, nameof(chips));

        var allChips = new List<ChipModel>();
        var selectedChips = new List<ChipModel>();
        var removableChips = new List<ChipModel>();
        foreach (var chip in chips)
        {
            allChips.Add(chip);
            if (chip.IsSelected)
            {
                selectedChips.Add(chip);
            }

            if (chip.IsRemovable)
            {
                removableChips.Add(chip);
            }
        }

        Chips = allChips;
        SelectionMode = selectionMode;
        SelectedChips = selectedChips;
        RemovableChips = removableChips;
    }

    /// <summary>Gets the chips displayed by the group.</summary>
    public IReadOnlyList<ChipModel> Chips { get; }

    /// <summary>Gets the selection mode used by the group.</summary>
    public ChipGroupSelectionMode SelectionMode { get; }

    /// <summary>Gets the selected chips.</summary>
    public IReadOnlyList<ChipModel> SelectedChips { get; }

    /// <summary>Gets the removable chips.</summary>
    public IReadOnlyList<ChipModel> RemovableChips { get; }

    /// <summary>Gets a value indicating whether one or more chips are selected.</summary>
    public bool HasSelection => SelectedChips.Count > 0;

    /// <summary>Gets a value indicating whether the group permits multiple selected chips.</summary>
    public bool CanSelectMultiple => SelectionMode == ChipGroupSelectionMode.Multiple;

    /// <summary>Gets the chip with the specified key.</summary>
    /// <param name="key">The stable chip key.</param>
    /// <returns>The matching chip, or <c>null</c> when no chip has the key.</returns>
    public ChipModel? GetChip(string key)
    {
        foreach (var chip in Chips)
        {
            if (string.Equals(chip.Key, key, StringComparison.Ordinal))
            {
                return chip;
            }
        }

        return null;
    }
}
