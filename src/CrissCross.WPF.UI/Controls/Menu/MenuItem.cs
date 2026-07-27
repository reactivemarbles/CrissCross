// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Extended MenuItem with SymbolRegular properties.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
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

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
