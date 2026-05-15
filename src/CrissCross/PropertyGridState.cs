// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CrissCross;

/// <summary>
/// Represents platform-neutral state for a descriptor-driven property inspector.
/// </summary>
public sealed class PropertyGridState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyGridState"/> class.
    /// </summary>
    /// <param name="descriptors">The property descriptors.</param>
    /// <param name="searchText">The optional search text.</param>
    /// <param name="isCommitting">A value indicating whether a commit operation is active.</param>
    public PropertyGridState(
        IReadOnlyList<PropertyDescriptorModel>? descriptors = null,
        string? searchText = null,
        bool isCommitting = false)
    {
        Descriptors = descriptors ?? [];
        SearchText = searchText;
        IsCommitting = isCommitting;
        VisibleDescriptors = Descriptors.Where(MatchesSearch).ToArray();
        Categories = VisibleDescriptors
            .GroupBy(static descriptor => descriptor.Category)
            .Select(static group => new PropertyDescriptorGroup(group.Key, group.ToArray()))
            .ToArray();
    }

    /// <summary>
    /// Gets the property descriptors.
    /// </summary>
    public IReadOnlyList<PropertyDescriptorModel> Descriptors { get; }

    /// <summary>
    /// Gets the optional search text.
    /// </summary>
    public string? SearchText { get; }

    /// <summary>
    /// Gets a value indicating whether a commit operation is active.
    /// </summary>
    public bool IsCommitting { get; }

    /// <summary>
    /// Gets the descriptors visible after search filtering.
    /// </summary>
    public IReadOnlyList<PropertyDescriptorModel> VisibleDescriptors { get; }

    /// <summary>
    /// Gets the visible descriptors grouped by category.
    /// </summary>
    public IReadOnlyList<PropertyDescriptorGroup> Categories { get; }

    /// <summary>
    /// Gets the total descriptor count.
    /// </summary>
    public int DescriptorCount => Descriptors.Count;

    /// <summary>
    /// Gets the visible descriptor count after search filtering.
    /// </summary>
    public int VisibleDescriptorCount => VisibleDescriptors.Count;

    /// <summary>
    /// Gets the editable descriptor count.
    /// </summary>
    public int EditableDescriptorCount => Descriptors.Count(static descriptor => descriptor.CanEdit);

    /// <summary>
    /// Gets the modified descriptor count.
    /// </summary>
    public int ModifiedDescriptorCount => Descriptors.Count(static descriptor => descriptor.IsModified);

    /// <summary>
    /// Gets the invalid descriptor count.
    /// </summary>
    public int InvalidDescriptorCount => Descriptors.Count(static descriptor => descriptor.IsInvalid);

    /// <summary>
    /// Gets a value indicating whether search text is active.
    /// </summary>
    public bool HasSearch => !string.IsNullOrWhiteSpace(SearchText);

    /// <summary>
    /// Gets a value indicating whether any descriptor is modified.
    /// </summary>
    public bool HasModifications => ModifiedDescriptorCount > 0;

    /// <summary>
    /// Gets a value indicating whether any descriptor has blocking validation.
    /// </summary>
    public bool HasValidationErrors => InvalidDescriptorCount > 0;

    /// <summary>
    /// Gets a value indicating whether modified descriptors can be committed.
    /// </summary>
    public bool CanCommit => HasModifications && !HasValidationErrors && !IsCommitting;

    /// <summary>
    /// Gets a value indicating whether at least one descriptor can be reset.
    /// </summary>
    public bool CanReset => Descriptors.Any(static descriptor => descriptor.CanReset) && !IsCommitting;

    /// <summary>
    /// Gets a compact inspector summary.
    /// </summary>
    public string SummaryText
    {
        get
        {
            if (DescriptorCount == 0)
            {
                return "No properties";
            }

            if (HasValidationErrors)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} properties, {1} invalid", DescriptorCount, InvalidDescriptorCount);
            }

            if (HasModifications)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} properties, {1} modified", DescriptorCount, ModifiedDescriptorCount);
            }

            return string.Format(CultureInfo.InvariantCulture, "{0} properties", DescriptorCount);
        }
    }

    /// <summary>
    /// Finds a descriptor by stable key.
    /// </summary>
    /// <param name="key">The descriptor key.</param>
    /// <returns>The descriptor when present; otherwise, <c>null</c>.</returns>
    public PropertyDescriptorModel? GetDescriptor(string key) => Descriptors.FirstOrDefault(descriptor => descriptor.Key == key);

    private static bool Contains(string source, string value) => source.IndexOf(value, System.StringComparison.OrdinalIgnoreCase) >= 0;

    private bool MatchesSearch(PropertyDescriptorModel descriptor)
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            return true;
        }

        var searchText = SearchText!.Trim();
        return Contains(descriptor.Key, searchText)
            || Contains(descriptor.DisplayName, searchText)
            || Contains(descriptor.Category, searchText)
            || Contains(descriptor.ValueDisplayText, searchText);
    }
}
