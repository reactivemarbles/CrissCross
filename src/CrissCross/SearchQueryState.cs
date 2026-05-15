// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace CrissCross;

/// <summary>
/// Represents the platform-neutral state projected by a search box and filter bar.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SearchQueryState"/> class.
/// </remarks>
/// <param name="text">The current raw search text.</param>
/// <param name="debouncedText">The latest debounced search text.</param>
/// <param name="submittedText">The latest explicitly submitted search text.</param>
/// <param name="isSearching">A value indicating whether a search operation is active.</param>
/// <param name="resultCount">The current result count, when known.</param>
/// <param name="filters">The active filter tokens.</param>
public sealed class SearchQueryState(
    string? text = null,
    string? debouncedText = null,
    string? submittedText = null,
    bool isSearching = false,
    int? resultCount = null,
    IReadOnlyList<FilterToken>? filters = null)
{
    /// <summary>
    /// Gets the current raw search text.
    /// </summary>
    public string? Text { get; } = text;

    /// <summary>
    /// Gets the latest debounced search text.
    /// </summary>
    public string? DebouncedText { get; } = debouncedText;

    /// <summary>
    /// Gets the latest explicitly submitted search text.
    /// </summary>
    public string? SubmittedText { get; } = submittedText;

    /// <summary>
    /// Gets a value indicating whether a search operation is active.
    /// </summary>
    public bool IsSearching { get; } = isSearching;

    /// <summary>
    /// Gets the current result count, when known.
    /// </summary>
    public int? ResultCount { get; } = resultCount;

    /// <summary>
    /// Gets the configured filter tokens.
    /// </summary>
    public IReadOnlyList<FilterToken>? Filters { get; } = filters;

    /// <summary>
    /// Gets the trimmed current query text.
    /// </summary>
    public string NormalizedText => (Text ?? string.Empty).Trim();

    /// <summary>
    /// Gets a value indicating whether the state has current query text.
    /// </summary>
    public bool HasQuery => NormalizedText.Length > 0;

    /// <summary>
    /// Gets the active filter tokens.
    /// </summary>
    public IReadOnlyList<FilterToken> ActiveFilters => Filters ?? [];

    /// <summary>
    /// Gets the number of active filter tokens.
    /// </summary>
    public int ActiveFilterCount => ActiveFilters.Count;

    /// <summary>
    /// Gets a value indicating whether active filters are present.
    /// </summary>
    public bool IsFiltered => ActiveFilterCount > 0;

    /// <summary>
    /// Gets compact user-facing result count text.
    /// </summary>
    public string ResultSummary => ResultCount switch
    {
        null => string.Empty,
        1 => "1 result",
        var count => string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} results", count)
    };
}
