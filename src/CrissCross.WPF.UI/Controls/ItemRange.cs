// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

//// Based on VirtualizingWrapPanel created by S. Bäumlisberger licensed under MIT license.
//// https://github.com/sbaeumlisberger/VirtualizingWrapPanel
//// Copyright (C) S. Bäumlisberger
//// All Rights Reserved.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Items range.
/// <para>Based on <see href="https://github.com/sbaeumlisberger/VirtualizingWrapPanel"/>.</para>
/// </summary>
public readonly record struct ItemRange
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemRange"/> struct.
    /// </summary>
    /// <param name="startIndex">The start index.</param>
    /// <param name="endIndex">The end index.</param>
    public ItemRange(int startIndex, int endIndex)
        : this()
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
    }

    /// <summary>
    /// Gets the start index.
    /// </summary>
    /// <value>
    /// The start index.
    /// </value>
    public int StartIndex { get; }
    /// <summary>
    /// Gets the end index.
    /// </summary>
    /// <value>
    /// The end index.
    /// </value>
    public int EndIndex { get; }

    /// <summary>
    /// Determines whether this instance contains the object.
    /// </summary>
    /// <param name="itemIndex">Index of the item.</param>
    /// <returns>
    ///   <c>true</c> if [contains] [the specified item index]; otherwise, <c>false</c>.
    /// </returns>
    public readonly bool Contains(int itemIndex) => itemIndex >= StartIndex && itemIndex <= EndIndex;
}
