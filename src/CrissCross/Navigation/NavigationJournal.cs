// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace CrissCross;

/// <summary>Provides platform-neutral back and forward navigation journal operations.</summary>
public static class NavigationJournal
{
    /// <summary>Gets a value indicating whether the journal can move to a previous entry.</summary>
    /// <param name="journal">The ordered journal entries.</param>
    /// <param name="currentIndex">The current entry index.</param>
    /// <returns><see langword="true"/> when a previous entry exists; otherwise, <see langword="false"/>.</returns>
    public static bool CanGoBack(IReadOnlyList<string> journal, int currentIndex)
    {
        if (journal is null)
        {
            throw new ArgumentNullException(nameof(journal));
        }

        return journal.Count > 0 && currentIndex > 0 && currentIndex < journal.Count;
    }

    /// <summary>Gets a value indicating whether the journal can move to a later entry.</summary>
    /// <param name="journal">The ordered journal entries.</param>
    /// <param name="currentIndex">The current entry index.</param>
    /// <returns><see langword="true"/> when a later entry exists; otherwise, <see langword="false"/>.</returns>
    public static bool CanGoForward(IReadOnlyList<string> journal, int currentIndex)
    {
        if (journal is null)
        {
            throw new ArgumentNullException(nameof(journal));
        }

        return currentIndex >= 0 && currentIndex < journal.Count - 1;
    }

    /// <summary>Records a new navigation entry and discards any forward entries after the current index.</summary>
    /// <param name="journal">The mutable ordered journal entries.</param>
    /// <param name="currentIndex">The current entry index, updated to the recorded entry.</param>
    /// <param name="entryId">The navigation entry identifier to record.</param>
    public static void Record(IList<string> journal, ref int currentIndex, string entryId)
    {
        if (journal is null)
        {
            throw new ArgumentNullException(nameof(journal));
        }

        ThrowHelper.ThrowIfNullOrWhiteSpace(entryId, nameof(entryId));

        if (currentIndex < journal.Count - 1)
        {
            var firstForwardIndex = Math.Max(currentIndex + 1, 0);
            for (var i = journal.Count - 1; i >= firstForwardIndex; i--)
            {
                journal.RemoveAt(i);
            }
        }

        journal.Add(entryId);
        currentIndex = journal.Count - 1;
    }

    /// <summary>Calculates the previous journal entry without mutating the journal.</summary>
    /// <param name="journal">The ordered journal entries.</param>
    /// <param name="currentIndex">The current entry index.</param>
    /// <param name="nextIndex">
    /// The index of the previous entry when one exists; otherwise, the supplied current index.
    /// </param>
    /// <param name="entryId">The previous entry identifier when one exists; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when a previous entry exists; otherwise, <see langword="false"/>.</returns>
    public static bool TryMoveBack(
        IReadOnlyList<string> journal,
        int currentIndex,
        out int nextIndex,
        out string? entryId)
    {
        if (journal is null)
        {
            throw new ArgumentNullException(nameof(journal));
        }

        if (!CanGoBack(journal, currentIndex))
        {
            nextIndex = currentIndex;
            entryId = null;
            return false;
        }

        nextIndex = currentIndex - 1;
        entryId = journal[nextIndex];
        return true;
    }

    /// <summary>Calculates the next journal entry without mutating the journal.</summary>
    /// <param name="journal">The ordered journal entries.</param>
    /// <param name="currentIndex">The current entry index.</param>
    /// <param name="nextIndex">
    /// The index of the next entry when one exists; otherwise, the supplied current index.
    /// </param>
    /// <param name="entryId">The next entry identifier when one exists; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when a next entry exists; otherwise, <see langword="false"/>.</returns>
    public static bool TryMoveForward(
        IReadOnlyList<string> journal,
        int currentIndex,
        out int nextIndex,
        out string? entryId)
    {
        if (journal is null)
        {
            throw new ArgumentNullException(nameof(journal));
        }

        if (!CanGoForward(journal, currentIndex))
        {
            nextIndex = currentIndex;
            entryId = null;
            return false;
        }

        nextIndex = currentIndex + 1;
        entryId = journal[nextIndex];
        return true;
    }

    /// <summary>Clears all entries and resets the current index to an empty-journal state.</summary>
    /// <param name="journal">The mutable ordered journal entries.</param>
    /// <param name="currentIndex">The current entry index, reset to <c>-1</c>.</param>
    public static void Clear(ICollection<string> journal, ref int currentIndex)
    {
        if (journal is null)
        {
            throw new ArgumentNullException(nameof(journal));
        }

        journal.Clear();
        currentIndex = -1;
    }
}
