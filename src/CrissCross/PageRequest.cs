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

/// <summary>Represents an immutable request for a page of data plus a stable query/filter/sort snapshot.</summary>
public sealed class PageRequest
{
    /// <summary>Formats the page request display text.</summary>
#if NET8_0_OR_GREATER
    private static readonly System.Text.CompositeFormat DisplayTextFormat = System.Text.CompositeFormat.Parse("Page {0}, {1} per page");
#else
    private const string DisplayTextFormat = "Page {0}, {1} per page";
#endif

    /// <inheritdoc />
    public PageRequest(int pageIndex, int pageSize)
        : this(pageIndex, pageSize, null, false, null) { }

    /// <inheritdoc />
    public PageRequest(int pageIndex, int pageSize, string? sortKey)
        : this(pageIndex, pageSize, sortKey, false, null) { }

    /// <inheritdoc />
    public PageRequest(int pageIndex, int pageSize, string? sortKey, bool sortDescending)
        : this(pageIndex, pageSize, sortKey, sortDescending, null) { }

    /// <summary>Initializes a new instance of the <see cref="PageRequest"/> class.</summary>
    /// <param name="pageIndex">The zero-based requested page index.</param>
    /// <param name="pageSize">The requested page size.</param>
    /// <param name="sortKey">The explicit sort key, when present.</param>
    /// <param name="sortDescending">A value indicating whether the sort direction is descending.</param>
    /// <param name="queryState">The search/filter snapshot associated with the request.</param>
    public PageRequest(int pageIndex, int pageSize, string? sortKey, bool sortDescending, SearchQueryState? queryState)
    {
        PageIndex = pageIndex < 0 ? 0 : pageIndex;
        PageSize = pageSize < 1 ? 1 : pageSize;
        SortKey = sortKey;
        SortDescending = sortDescending;
        QueryState = queryState;
        ActiveFilters = queryState?.ActiveFilters ?? [];
        FilterSnapshotKey = CreateFilterSnapshotKey(ActiveFilters);
    }

    /// <summary>Gets the zero-based requested page index.</summary>
    public int PageIndex { get; }

    /// <summary>Gets the requested page size.</summary>
    public int PageSize { get; }

    /// <summary>Gets the zero-based offset for data-source skip operations.</summary>
    public int Offset => PageIndex * PageSize;

    /// <summary>Gets the explicit sort key, when present.</summary>
    public string? SortKey { get; }

    /// <summary>Gets a value indicating whether the sort direction is descending.</summary>
    public bool SortDescending { get; }

    /// <summary>Gets the search/filter snapshot associated with the request.</summary>
    public SearchQueryState? QueryState { get; }

    /// <summary>Gets the active filter snapshot associated with the request.</summary>
    public IReadOnlyList<FilterToken> ActiveFilters { get; }

    /// <summary>Gets a stable key built from the active filter snapshot.</summary>
    public string FilterSnapshotKey { get; }

    /// <summary>Gets a value indicating whether a sort key was supplied.</summary>
    public bool HasSort => !string.IsNullOrWhiteSpace(SortKey);

    /// <summary>Gets a value indicating whether a query or filter snapshot is present.</summary>
    public bool HasQuery => QueryState?.HasQuery == true || ActiveFilters.Count > 0;

    /// <summary>Gets compact user-facing request text for diagnostics.</summary>
    public string DisplayText =>
        string.Format(CultureInfo.InvariantCulture, DisplayTextFormat, PageIndex + 1, PageSize);

    /// <summary>Creates the stable key for a filter snapshot.</summary>
    /// <param name="filters">The filters to include.</param>
    /// <returns>The stable snapshot key.</returns>
    private static string CreateFilterSnapshotKey(IReadOnlyList<FilterToken> filters)
    {
        if (filters.Count == 0)
        {
            return string.Empty;
        }

        var builder = new System.Text.StringBuilder();
        for (var index = 0; index < filters.Count; index++)
        {
            if (index > 0)
            {
                _ = builder.Append('|');
            }

            _ = builder.Append(filters[index].Key);
        }

        return builder.ToString();
    }
}
