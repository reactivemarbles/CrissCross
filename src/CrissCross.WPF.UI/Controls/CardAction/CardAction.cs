// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Converters;
#else
using CrissCross.WPF.UI.Converters;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Inherited from the ButtonBase interactive card styled according to Fluent Design.</summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(CardAction), "CardAction.bmp")]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class CardAction : System.Windows.Controls.Primitives.ButtonBase
{
    /// <summary>Property for <see cref="IsChevronVisible"/>.</summary>
    public static readonly DependencyProperty IsChevronVisibleProperty = DependencyProperty.Register(
        nameof(IsChevronVisible),
        typeof(bool),
        typeof(CardAction),
        new(true));

    /// <summary>Property for <see cref="Icon"/>.</summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(CardAction),
        new(null, null, IconSourceElementConverter.ConvertToIconElement));

    /// <summary>Gets or sets a value indicating whether gets or sets information whether to display the chevron icon on
    /// the right side of the card.</summary>
    [Bindable(true)]
    [Category("Appearance")]
    public bool IsChevronVisible
    {
        get => (bool)GetValue(IsChevronVisibleProperty);
        set => SetValue(IsChevronVisibleProperty, value);
    }

    /// <summary>Gets or sets displayed <see cref="IconElement"/>.</summary>
    [Bindable(true)]
    [Category("Appearance")]
    public IconElement? Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
