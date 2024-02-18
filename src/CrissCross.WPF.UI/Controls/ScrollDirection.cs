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
/// Direction of <see cref="System.Windows.Controls.ScrollViewer"/>.
/// <para>Based on <see href="https://github.com/sbaeumlisberger/VirtualizingWrapPanel"/>.</para>
/// </summary>
public enum ScrollDirection
{
    /// <summary>
    /// Vertical scroll direction.
    /// </summary>
    Vertical,

    /// <summary>
    /// Horizontal scroll direction.
    /// </summary>
    Horizontal
}
