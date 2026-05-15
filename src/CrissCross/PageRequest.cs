// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CrissCross;

/// <summary>
/// Represents an immutable request for a page of data plus a stable query/filter/sort snapshot.
/// </summary>
public sealed class PageRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PageRequest"/> class.
    /// </summary>
    /// <param name="pageIndex">The zero-based requested page index.</param>
    /// <param name="pageSize">The requested page size.</param>
    /// <param name="sortKey">The explicit sort key, when present.</param>
    /// <param name="sortDescending">A value indicating whether the sort direction is descending.</param>
    /// <param name="queryState">The search/filter snapshot associated with the request.</param>
    public PageRequest(
        int pageIndex,
        int pageSize,
        string? sortKey = null,
        bool sortDescending = false,
        SearchQueryState? queryState = null)
    {
        PageIndex = pageIndex < 0 ? 0 : pageIndex;
        PageSize = pageSize < 1 ? 1 : pageSize;
        SortKey = sortKey;
        SortDescending = sortDescending;
        QueryState = queryState;
        ActiveFilters = queryState?.ActiveFilters ?? [];
        FilterSnapshotKey = string.Join("|", ActiveFilters.Select(static filter => filter.Key));
    }

    /// <summary>
    /// Gets the zero-based requested page index.
    /// </summary>
    public int PageIndex { get; }

    /// <summary>
    /// Gets the requested page size.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets the zero-based offset for data-source skip operations.
    /// </summary>
    public int Offset => PageIndex * PageSize;

    /// <summary>
    /// Gets the explicit sort key, when present.
    /// </summary>
    public string? SortKey { get; }

    /// <summary>
    /// Gets a value indicating whether the sort direction is descending.
    /// </summary>
    public bool SortDescending { get; }

    /// <summary>
    /// Gets the search/filter snapshot associated with the request.
    /// </summary>
    public SearchQueryState? QueryState { get; }

    /// <summary>
    /// Gets the active filter snapshot associated with the request.
    /// </summary>
    public IReadOnlyList<FilterToken> ActiveFilters { get; }

    /// <summary>
    /// Gets a stable key built from the active filter snapshot.
    /// </summary>
    public string FilterSnapshotKey { get; }

    /// <summary>
    /// Gets a value indicating whether a sort key was supplied.
    /// </summary>
    public bool HasSort => !string.IsNullOrWhiteSpace(SortKey);

    /// <summary>
    /// Gets a value indicating whether a query or filter snapshot is present.
    /// </summary>
    public bool HasQuery => QueryState?.HasQuery == true || ActiveFilters.Count > 0;

    /// <summary>
    /// Gets compact user-facing request text for diagnostics.
    /// </summary>
    public string DisplayText => string.Format(CultureInfo.InvariantCulture, "Page {0}, {1} per page", PageIndex + 1, PageSize);
}
