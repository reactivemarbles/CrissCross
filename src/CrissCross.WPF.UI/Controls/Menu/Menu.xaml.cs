// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
