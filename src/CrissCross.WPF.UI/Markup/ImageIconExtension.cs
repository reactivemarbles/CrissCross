// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Markup;

namespace CrissCross.WPF.UI.Markup;

/// <summary>
/// Custom <see cref="MarkupExtension"/> which can provide <see cref="ImageIcon"/>.
/// </summary>
/// <example>
/// <code lang="xml">
/// &lt;ui:Button
///     Appearance="Primary"
///     Content="WPF button with font icon"
///     Icon="{ui:ImageIcon '/my-icon.png'}" /&gt;
/// </code>
/// <code lang="xml">
/// &lt;ui:Button Icon="{ui:ImageIcon 'pack://application:,,,/Assets/iconImage.png'}" /&gt;
/// </code>
/// <code lang="xml">
/// &lt;ui:HyperlinkButton Icon="{ui:ImageIcon 'pack://application:,,,/Assets/iconImage.png'}" /&gt;
/// </code>
/// <code lang="xml">
/// &lt;ui:TitleBar Icon="{ui:ImageIcon 'pack://application:,,,/Assets/iconImage.png'}" /&gt;
/// </code>
/// </example>
/// <remarks>
/// Initializes a new instance of the <see cref="ImageIconExtension"/> class.
/// </remarks>
/// <param name="source">The source.</param>
[ContentProperty(nameof(Source))]
[MarkupExtensionReturnType(typeof(ImageIcon))]
public class ImageIconExtension(ImageSource? source) : MarkupExtension
{
    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    /// <value>
    /// The source.
    /// </value>
    [ConstructorArgument("source")]
    public ImageSource? Source { get; set; } = source;

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>
    /// The width.
    /// </value>
    public double Width { get; set; } = 16D;

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>
    /// The height.
    /// </value>
    public double Height { get; set; } = 16D;

    /// <summary>
    /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
    /// </summary>
    /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
    /// <returns>
    /// The object value to set on the property where the extension is applied.
    /// </returns>
    public override object ProvideValue(IServiceProvider serviceProvider) => new ImageIcon
    {
        Source = Source,
        Width = Width,
        Height = Height
    };
}
