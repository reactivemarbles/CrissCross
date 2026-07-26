// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Automation.Peers;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.AutomationPeers;
#else
using CrissCross.WPF.UI.AutomationPeers;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides the CardControl member.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class CardControl : System.Windows.Controls.Primitives.ButtonBase, IIconControl
{
    /// <summary>Property for <see cref="Header"/>.</summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(object),
        typeof(CardControl),
        new(null));

    /// <summary>Property for <see cref="Icon"/>.</summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(CardControl),
        new(null));

    /// <summary>Property for <see cref="CornerRadius"/>.</summary>
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(CardControl),
        new(new CornerRadius(0)));

    /// <summary>Gets or sets header is the data used to for the header of each item in the control.</summary>
    [Bindable(true)]
    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>Gets or sets displayed <see cref="IconElement"/>.</summary>
    [Bindable(true)]
    [Category("Appearance")]
    public IconElement? Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>Gets or sets the corner radius of the control.</summary>
    [Bindable(true)]
    [Category("Appearance")]
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementations
    /// for the Windows Presentation Foundation (WPF) infrastructure.</summary>
    /// <returns>
    /// The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementation.
    /// </returns>
    protected override AutomationPeer OnCreateAutomationPeer() => new CardControlAutomationPeer(this);
}
