// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Represents an icon that uses an IconSource as its content.</summary>
public class IconSourceElement : IconElement
{
    /// <summary>Property for <see cref="IconSource"/>.</summary>
    public static readonly StyledProperty<IconSource?> IconSourceProperty = AvaloniaProperty.Register<
        IconSourceElement,
        IconSource?
    >(nameof(IconSource));

    /// <summary>Gets or sets the IconSource.</summary>
    public IconSource? IconSource
    {
        get => GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>Creates the icon element.</summary>
    /// <returns>An IconElement.</returns>
    public IconElement? CreateIconElement() => IconSource?.CreateIconElement();
}
