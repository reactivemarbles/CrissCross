// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Extended <see cref="System.Windows.Controls.MenuItem"/> with <see cref="SymbolRegular"/> properties.
/// </summary>
public class MenuItem : System.Windows.Controls.MenuItem
{
    static MenuItem() =>
        IconProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Gets or sets displayed <see cref="IconElement"/>.
    /// </summary>
    public new IconElement Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
