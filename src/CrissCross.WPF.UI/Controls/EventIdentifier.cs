// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Class used to create identifiers of threads or tasks that can be performed multiple times within one instance.
/// <see cref="Current"/> represents roughly the time in microseconds at which it was taken.
/// </summary>
internal sealed class EventIdentifier
{
    /// <summary>Gets the Gets or sets current identifier. value.</summary>
    public long Current { get; internal set; }

    /// <summary>Creates and gets the next identifier.</summary>
    /// <returns>The result.</returns>
    public long GetNext()
    {
        UpdateIdentifier();

        return Current;
    }

    /// <summary>Checks if the identifiers are the same.</summary>
    /// <param name="storedId">The storedId value.</param>
    /// <returns>The result.</returns>
    public bool IsEqual(long storedId) => Current == storedId;

    /// <summary>Creates and assigns a random value with an extra time code if possible.</summary>
    private void UpdateIdentifier() => Current = DateTimeOffset.Now.ToUnixTimeMilliseconds();
}
