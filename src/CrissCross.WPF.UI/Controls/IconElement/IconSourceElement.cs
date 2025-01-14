// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Markup;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents an icon that uses an IconSource as its content.
/// </summary>
[ContentProperty(nameof(IconSource))]
public class IconSourceElement : IconElement
{
    /// <summary>Identifies the <see cref="IconSource"/> dependency property.</summary>
    public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
        nameof(IconSource),
        typeof(IconSource),
        typeof(IconSourceElement),
        new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Gets or sets <see cref="IconSource"/>.
    /// </summary>
    public IconSource? IconSource
    {
        get => (IconSource?)GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Creates the icon element.
    /// </summary>
    /// <returns>An IconElement.</returns>
    public IconElement? CreateIconElement() => IconSource?.CreateIconElement();

    /// <summary>
    /// Initializes the children.
    /// </summary>
    /// <returns>A UIElement.</returns>
    /// <exception cref="System.InvalidOperationException">Use {nameof(CreateIconElement)}.</exception>
    protected override UIElement InitializeChildren()
    {
        // TODO: Come up with an elegant solution
        throw new InvalidOperationException($"Use {nameof(CreateIconElement)}");
    }
}
