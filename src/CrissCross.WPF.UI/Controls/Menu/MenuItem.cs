// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Extended MenuItem with SymbolRegular properties.</summary>
public class MenuItem : System.Windows.Controls.MenuItem
{
    /// <summary>Provides the MenuItem member.</summary>
    static MenuItem() => IconProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(null));

    /// <summary>Gets or sets displayed <see cref="IconElement"/>.</summary>
    public new IconElement Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
