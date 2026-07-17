// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Markup;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Markup;
#else
namespace CrissCross.WPF.UI.Markup;
#endif

/// <summary>Class for Xaml markup extension for static resource references.</summary>
/// <example>
/// <code lang="xml">
/// &lt;ui:Button
///     Appearance="Primary"
///     Content="WPF button with font icon"
///     Foreground={ui:ThemeResource SystemAccentColorPrimaryBrush} /&gt;
/// </code>
/// <code lang="xml">
/// &lt;ui:TextBox Foreground={ui:ThemeResource TextFillColorSecondaryBrush} /&gt;
/// </code>
/// </example>
[TypeConverter(typeof(DynamicResourceExtensionConverter))]
[ContentProperty(nameof(ResourceKey))]
[MarkupExtensionReturnType(typeof(object))]
public class ThemeResourceExtension : DynamicResourceExtension
{
    /// <summary>Initializes a new instance of the <see cref="ThemeResourceExtension"/> class.</summary>
    public ThemeResourceExtension() => ResourceKey = nameof(ThemeResource.ApplicationBackgroundBrush);

    /// <summary>Initializes a new instance of the <see cref="ThemeResourceExtension"/> class.</summary>
    /// <exception cref="ArgumentNullException">resourceKey.</exception>
    /// <param name="resourceKey">The resource key.</param>
    public ThemeResourceExtension(ThemeResource resourceKey)
    {
        if (resourceKey == ThemeResource.Unknown)
        {
            throw new ArgumentNullException(nameof(resourceKey));
        }

        ResourceKey = resourceKey.ToString();
    }
}
