// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.UI.Extensions;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Class used to create identifiers of threads or tasks that can be performed multiple times within one instance.
/// <see cref="Current"/> represents roughly the time in microseconds at which it was taken.
/// </summary>
internal class EventIdentifier
{
    /// <summary>
    /// Gets or sets current identifier.
    /// </summary>
    public long Current { get; internal set; }

    /// <summary>
    /// Creates and gets the next identifier.
    /// </summary>
    public long GetNext()
    {
        UpdateIdentifier();

        return Current;
    }

    /// <summary>
    /// Checks if the identifiers are the same.
    /// </summary>
    public bool IsEqual(long storedId) => Current == storedId;

    /// <summary>
    /// Creates and assigns a random value with an extra time code if possible.
    /// </summary>
    private void UpdateIdentifier() => Current = DateTime.Now.GetMicroTimestamp();
}
