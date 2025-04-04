// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Simple Card with content and <see cref="Footer"/>.
/// </summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(Card), "Card.bmp")]
public class Card : System.Windows.Controls.ContentControl
{
    /// <summary>
    /// Property for <see cref="Footer"/>.
    /// </summary>
    public static readonly DependencyProperty FooterProperty = DependencyProperty.Register(
        nameof(Footer),
        typeof(object),
        typeof(Card),
        new PropertyMetadata(null, FooterChangedCallback));

    /// <summary>
    /// Property for <see cref="HasFooter"/>.
    /// </summary>
    public static readonly DependencyProperty HasFooterProperty = DependencyProperty.Register(
        nameof(HasFooter),
        typeof(bool),
        typeof(Card),
        new PropertyMetadata(false));

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
        get => (bool)GetValue(HasFooterProperty);
        internal set => SetValue(HasFooterProperty, value);
    }

    private static void FooterChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Card control)
        {
            return;
        }

        control.SetValue(HasFooterProperty, control.Footer != null);
    }
}
