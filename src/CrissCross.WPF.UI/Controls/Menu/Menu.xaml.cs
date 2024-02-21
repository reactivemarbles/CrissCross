// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

namespace CrissCross.WPF.UI.Styles.Controls;

/// <summary>
/// Extension to the menu.
/// </summary>
public partial class Menu : ResourceDictionary
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Menu"/> class.
    /// Sets menu alignment on initialization.
    /// </summary>
    public Menu() => Initialize();

    private static void Initialize()
    {
        if (!SystemParameters.MenuDropAlignment)
        {
            return;
        }

        var fieldInfo = typeof(SystemParameters).GetField(
            "_menuDropAlignment",
            BindingFlags.NonPublic | BindingFlags.Static);
        fieldInfo?.SetValue(null, false);
    }
}
