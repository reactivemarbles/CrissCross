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

/// <summary>Inherited from the Expander control which can hide the collapsible content.</summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(CardExpander), "CardExpander.bmp")]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class CardExpander : System.Windows.Controls.Expander
{
    /// <summary>Property for <see cref="Icon"/>.</summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(CardExpander),
        new(null, null, IconSourceElementConverter.ConvertToIconElement));

    /// <summary>Property for <see cref="CornerRadius"/>.</summary>
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(CardExpander),
        new(new CornerRadius(4)));

    /// <summary>Property for <see cref="ContentPadding"/>.</summary>
    public static readonly DependencyProperty ContentPaddingProperty = DependencyProperty.Register(
        nameof(ContentPadding),
        typeof(Thickness),
        typeof(CardExpander),
        new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.AffectsParentMeasure));

    /// <summary>Gets or sets displayed <see cref="IconElement"/>.</summary>
    [Bindable(true)]
    [Category("Appearance")]
    public IconElement? Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>Gets or sets displayed <see cref="IconElement"/>.</summary>
    [Bindable(true)]
    [Category("Appearance")]
    public CornerRadius? CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>Gets or sets content padding Property.</summary>
    [Bindable(true)]
    [Category("Layout")]
    public Thickness ContentPadding
    {
        get => (Thickness)GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
