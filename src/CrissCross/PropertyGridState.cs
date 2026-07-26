// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Represents platform-neutral state for a descriptor-driven property inspector.</summary>
public sealed class PropertyGridState
{
    /// <summary>Formats the invalid-property summary text.</summary>
#if NET8_0_OR_GREATER
    private static readonly System.Text.CompositeFormat InvalidSummaryFormat = System.Text.CompositeFormat.Parse("{0} properties, {1} invalid");
#else
    private const string InvalidSummaryFormat = "{0} properties, {1} invalid";
#endif

    /// <summary>Formats the modified-property summary text.</summary>
#if NET8_0_OR_GREATER
    private static readonly System.Text.CompositeFormat ModifiedSummaryFormat = System.Text.CompositeFormat.Parse("{0} properties, {1} modified");
#else
    private const string ModifiedSummaryFormat = "{0} properties, {1} modified";
#endif

    /// <summary>Formats the property summary text.</summary>
#if NET8_0_OR_GREATER
    private static readonly System.Text.CompositeFormat PropertySummaryFormat = System.Text.CompositeFormat.Parse("{0} properties");
#else
    private const string PropertySummaryFormat = "{0} properties";
#endif

    /// <inheritdoc />
    public PropertyGridState()
        : this(null, null, false) { }

    /// <inheritdoc />
    public PropertyGridState(IReadOnlyList<PropertyDescriptorModel>? descriptors)
        : this(descriptors, null, false) { }

    /// <inheritdoc />
    public PropertyGridState(IReadOnlyList<PropertyDescriptorModel>? descriptors, string? searchText)
        : this(descriptors, searchText, false) { }

    /// <summary>Initializes a new instance of the <see cref="PropertyGridState"/> class.</summary>
    /// <param name="descriptors">The property descriptors.</param>
    /// <param name="searchText">The optional search text.</param>
    /// <param name="isCommitting">A value indicating whether a commit operation is active.</param>
    public PropertyGridState(IReadOnlyList<PropertyDescriptorModel>? descriptors, string? searchText, bool isCommitting)
    {
        Descriptors = descriptors ?? [];
        SearchText = searchText;
        IsCommitting = isCommitting;
        var visibleDescriptors = new List<PropertyDescriptorModel>();
        var descriptorsByCategory = new Dictionary<string, List<PropertyDescriptorModel>>();
        var categoryOrder = new List<string>();
        foreach (var descriptor in Descriptors)
        {
            if (!MatchesSearch(descriptor))
            {
                continue;
            }

            visibleDescriptors.Add(descriptor);
            if (!descriptorsByCategory.TryGetValue(descriptor.Category, out var categoryDescriptors))
            {
                categoryDescriptors = new();
                descriptorsByCategory.Add(descriptor.Category, categoryDescriptors);
                categoryOrder.Add(descriptor.Category);
            }

            categoryDescriptors.Add(descriptor);
        }

        var categories = new List<PropertyDescriptorGroup>(categoryOrder.Count);
        foreach (var category in categoryOrder)
        {
            categories.Add(new(category, descriptorsByCategory[category]));
        }

        VisibleDescriptors = visibleDescriptors;
        Categories = categories;
    }

    /// <summary>Gets the property descriptors.</summary>
    public IReadOnlyList<PropertyDescriptorModel> Descriptors { get; }

    /// <summary>Gets the optional search text.</summary>
    public string? SearchText { get; }

    /// <summary>Gets a value indicating whether a commit operation is active.</summary>
    public bool IsCommitting { get; }

    /// <summary>Gets the descriptors visible after search filtering.</summary>
    public IReadOnlyList<PropertyDescriptorModel> VisibleDescriptors { get; }

    /// <summary>Gets the visible descriptors grouped by category.</summary>
    public IReadOnlyList<PropertyDescriptorGroup> Categories { get; }

    /// <summary>Gets the total descriptor count.</summary>
    public int DescriptorCount => Descriptors.Count;

    /// <summary>Gets the visible descriptor count after search filtering.</summary>
    public int VisibleDescriptorCount => VisibleDescriptors.Count;

    /// <summary>Gets the editable descriptor count.</summary>
    public int EditableDescriptorCount => CountEditableDescriptors();

    /// <summary>Gets the modified descriptor count.</summary>
    public int ModifiedDescriptorCount => CountModifiedDescriptors();

    /// <summary>Gets the invalid descriptor count.</summary>
    public int InvalidDescriptorCount => CountInvalidDescriptors();

    /// <summary>Gets a value indicating whether search text is active.</summary>
    public bool HasSearch => !string.IsNullOrWhiteSpace(SearchText);

    /// <summary>Gets a value indicating whether any descriptor is modified.</summary>
    public bool HasModifications => ModifiedDescriptorCount > 0;

    /// <summary>Gets a value indicating whether any descriptor has blocking validation.</summary>
    public bool HasValidationErrors => InvalidDescriptorCount > 0;

    /// <summary>Gets a value indicating whether modified descriptors can be committed.</summary>
    public bool CanCommit => HasModifications && !HasValidationErrors && !IsCommitting;

    /// <summary>Gets a value indicating whether at least one descriptor can be reset.</summary>
    public bool CanReset => !IsCommitting && HasResettableDescriptor();

    /// <summary>Gets a compact inspector summary.</summary>
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
                return string.Format(
                    CultureInfo.InvariantCulture,
                    InvalidSummaryFormat,
                    DescriptorCount,
                    InvalidDescriptorCount);
            }

            return HasModifications
                ? string.Format(
                    CultureInfo.InvariantCulture,
                    ModifiedSummaryFormat,
                    DescriptorCount,
                    ModifiedDescriptorCount)
                : string.Format(CultureInfo.InvariantCulture, PropertySummaryFormat, DescriptorCount);
        }
    }

    /// <summary>Finds a descriptor by stable key.</summary>
    /// <param name="key">The descriptor key.</param>
    /// <returns>The descriptor when present; otherwise, <c>null</c>.</returns>
    public PropertyDescriptorModel? GetDescriptor(string key)
    {
        foreach (var descriptor in Descriptors)
        {
            if (descriptor.Key == key)
            {
                return descriptor;
            }
        }

        return null;
    }

    /// <summary>Determines whether one string contains another using ordinal-ignore-case comparison.</summary>
    /// <param name="source">The source text.</param>
    /// <param name="value">The value to find.</param>
    /// <returns><c>true</c> when the source contains the value; otherwise, <c>false</c>.</returns>
#if NETFRAMEWORK
    private static bool Contains(string source, string value) =>
        source.IndexOf(value, System.StringComparison.OrdinalIgnoreCase) >= 0;
#else
    private static bool Contains(string source, string value) =>
        source.Contains(value, System.StringComparison.OrdinalIgnoreCase);
#endif

    /// <summary>Determines whether a descriptor matches the current search text.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns><c>true</c> when the descriptor matches; otherwise, <c>false</c>.</returns>
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

    /// <summary>Counts editable descriptors.</summary>
    /// <returns>The editable descriptor count.</returns>
    private int CountEditableDescriptors()
    {
        var count = 0;
        foreach (var descriptor in Descriptors)
        {
            if (descriptor.CanEdit)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>Counts modified descriptors.</summary>
    /// <returns>The modified descriptor count.</returns>
    private int CountModifiedDescriptors()
    {
        var count = 0;
        foreach (var descriptor in Descriptors)
        {
            if (descriptor.IsModified)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>Counts invalid descriptors.</summary>
    /// <returns>The invalid descriptor count.</returns>
    private int CountInvalidDescriptors()
    {
        var count = 0;
        foreach (var descriptor in Descriptors)
        {
            if (descriptor.IsInvalid)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>Determines whether a descriptor can be reset.</summary>
    /// <returns><c>true</c> when a descriptor can be reset; otherwise, <c>false</c>.</returns>
    private bool HasResettableDescriptor()
    {
        foreach (var descriptor in Descriptors)
        {
            if (descriptor.CanReset)
            {
                return true;
            }
        }

        return false;
    }
}
