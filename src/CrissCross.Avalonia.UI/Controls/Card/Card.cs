// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Simple Card with content and <see cref="Footer"/>.
/// </summary>
public class Card : global::Avalonia.Controls.ContentControl
{
    /// <summary>
    /// Property for <see cref="Footer"/>.
    /// </summary>
    public static readonly StyledProperty<object> FooterProperty = AvaloniaProperty.Register<Card, object>(
        nameof(Footer), null);

    /// <summary>
    /// Property for <see cref="HasFooter"/>.
    /// </summary>
    public static readonly StyledProperty<bool> HasFooterProperty = AvaloniaProperty.Register<Card, bool>(
        nameof(HasFooter), false);

    /// <summary>
    /// Property for <see cref="Header"/>.
    /// </summary>
    public static readonly StyledProperty<object> HeaderProperty = AvaloniaProperty.Register<Card, object>(
        nameof(Header), null);

    /// <summary>
    /// Property for <see cref="HasHeader"/>.
    /// </summary>
    public static readonly StyledProperty<bool> HasHeaderProperty = AvaloniaProperty.Register<Card, bool>(
        nameof(HasHeader), false);

    static Card()
    {
        FooterProperty.Changed.AddClassHandler<Card>((x, e) => x.SetValue(HasFooterProperty, x.Footer != null));
        HeaderProperty.Changed.AddClassHandler<Card>((x, e) => x.SetValue(HasHeaderProperty, x.Header != null));
    }

    /// <summary>
    /// Gets or sets additional content displayed at the bottom.
    /// </summary>
    public object Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether gets information whether the <see cref="Card"/> has a <see cref="Footer"/>.
    /// </summary>
    public bool HasFooter
    {
        get => GetValue(HasFooterProperty);
        private set => SetValue(HasFooterProperty, value);
    }

    /// <summary>
    /// Gets or sets additional content displayed at the top.
    /// </summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether the card has a header.
    /// </summary>
    public bool HasHeader
    {
        get => GetValue(HasHeaderProperty);
        private set => SetValue(HasHeaderProperty, value);
    }
}
