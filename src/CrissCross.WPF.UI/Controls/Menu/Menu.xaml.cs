// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Styles.Controls;

/// <summary>Extension to the menu.</summary>
public partial class Menu : ResourceDictionary
{
    /// <summary>Initializes a new instance of the <see cref="Menu"/> class.</summary>
    public Menu() => Initialize();

    /// <summary>Provides the Initialize member.</summary>
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
