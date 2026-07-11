// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CrissCross;

/// <summary>Represents platform-neutral selection state for a segmented control.</summary>
public sealed class SegmentedSelectionState
{
    /// <summary>Initializes a new instance of the <see cref="SegmentedSelectionState"/> class.</summary>
    /// <param name="items">The available segments.</param>
    /// <param name="selectedKey">The selected segment key.</param>
    public SegmentedSelectionState(IEnumerable<SegmentItem> items, string? selectedKey = null)
    {
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        Items = items.ToArray();
        SelectedKey = selectedKey;
        EnabledItems = Items.Where(static item => item.IsEnabled).ToArray();
        SelectedItem = selectedKey is null ? null : GetItem(selectedKey);
    }

    /// <summary>Gets the available segments.</summary>
    public IReadOnlyList<SegmentItem> Items { get; }

    /// <summary>Gets the selected segment key.</summary>
    public string? SelectedKey { get; }

    /// <summary>Gets the enabled segments.</summary>
    public IReadOnlyList<SegmentItem> EnabledItems { get; }

    /// <summary>Gets the selected segment, or <c>null</c> when no matching segment is selected.</summary>
    public SegmentItem? SelectedItem { get; }

    /// <summary>Gets a value indicating whether a segment is selected.</summary>
    public bool HasSelection => SelectedItem is not null;

    /// <summary>Gets the segment with the specified key.</summary>
    /// <param name="key">The stable segment key.</param>
    /// <returns>The matching segment, or <c>null</c> when no segment has the key.</returns>
    public SegmentItem? GetItem(string key) => Items.FirstOrDefault(item => string.Equals(item.Key, key, StringComparison.Ordinal));
}
