// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Globalization;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Represents platform-neutral state for paged data navigation.</summary>
public sealed class PaginationState
{
    /// <summary>Initializes a new instance of the <see cref="PaginationState"/> class.</summary>
    /// <param name="pageIndex">The zero-based requested page index.</param>
    /// <param name="pageSize">The number of items requested per page.</param>
    /// <param name="totalItemCount">The total number of items available.</param>
    public PaginationState(int pageIndex, int pageSize, int totalItemCount)
    {
        PageSize = Math.Max(1, pageSize);
        TotalItemCount = Math.Max(0, totalItemCount);
        TotalPages = Math.Max(1, (int)Math.Ceiling(TotalItemCount / (double)PageSize));
        PageIndex = Clamp(pageIndex, 0, TotalPages - 1);
    }

    /// <summary>Gets the zero-based current page index.</summary>
    public int PageIndex { get; }

    /// <summary>Gets the one-based current page number for display.</summary>
    public int PageNumber => PageIndex + 1;

    /// <summary>Gets the requested page size.</summary>
    public int PageSize { get; }

    /// <summary>Gets the total number of items available.</summary>
    public int TotalItemCount { get; }

    /// <summary>Gets the total display page count.</summary>
    public int TotalPages { get; }

    /// <summary>Gets a value indicating whether the paged source contains one or more items.</summary>
    public bool HasItems => TotalItemCount > 0;

    /// <summary>Gets a value indicating whether navigation to the first page is available.</summary>
    public bool CanGoFirst => PageIndex > 0;

    /// <summary>Gets a value indicating whether navigation to the previous page is available.</summary>
    public bool CanGoPrevious => PageIndex > 0;

    /// <summary>Gets a value indicating whether navigation to the next page is available.</summary>
    public bool CanGoNext => PageIndex < TotalPages - 1;

    /// <summary>Gets a value indicating whether navigation to the last page is available.</summary>
    public bool CanGoLast => PageIndex < TotalPages - 1;

    /// <summary>Gets the one-based number of the first item displayed on the current page.</summary>
    public int FirstItemNumber => HasItems ? (PageIndex * PageSize) + 1 : 0;

    /// <summary>Gets the one-based number of the last item displayed on the current page.</summary>
    public int LastItemNumber => HasItems ? Math.Min(TotalItemCount, (PageIndex + 1) * PageSize) : 0;

    /// <summary>Gets compact user-facing item range text.</summary>
    public string SummaryText => HasItems
        ? string.Format(CultureInfo.InvariantCulture, "{0}-{1} of {2}", FirstItemNumber, LastItemNumber, TotalItemCount)
        : "No items";

    /// <summary>Creates a request for the specified page using the current page size.</summary>
    /// <param name="pageIndex">The zero-based requested page index.</param>
    /// <returns>A page request clamped to the current valid page range.</returns>
    public PageRequest CreateRequest(int pageIndex) => new(Clamp(pageIndex, 0, TotalPages - 1), PageSize);

    /// <summary>Clamps a value to the requested inclusive range.</summary>
    /// <param name="value">The requested value.</param>
    /// <param name="minimum">The minimum value.</param>
    /// <param name="maximum">The maximum value.</param>
    /// <returns>The clamped value.</returns>
    private static int Clamp(int value, int minimum, int maximum)
    {
        if (value < minimum)
        {
            return minimum;
        }

        return value > maximum ? maximum : value;
    }
}
