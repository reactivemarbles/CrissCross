// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

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
