// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text;
using System.Xml.Linq;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Applies Word document defaults before named and direct paragraph formatting.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>The internal lookup key for document formatting defaults.</summary>
    private const string DocumentDefaultsKey = "crisscross:document-defaults";

    /// <summary>The internal lookup key for the default paragraph style.</summary>
    private const string DefaultParagraphStyleKey = "crisscross:paragraph-default";

    /// <summary>Registers document and default paragraph formatting.</summary>
    /// <param name="root">The styles document root.</param>
    /// <param name="styles">The style lookup.</param>
    private static void AddStyleDefaults(XElement? root, Dictionary<string, XElement> styles)
    {
        var defaults = root?.Element(Word + "docDefaults");
        if (defaults is not null)
        {
            styles[DocumentDefaultsKey] = new(
                Word + "style",
                defaults.Element(Word + "pPrDefault")?.Element(Word + "pPr"),
                defaults.Element(Word + "rPrDefault")?.Element(Word + "rPr"));
        }

        foreach (var style in root?.Elements(Word + "style") ?? [])
        {
            if (Value(style, "default") == "1" && Value(style, "type") == "paragraph")
            {
                styles[DefaultParagraphStyleKey] = style;
            }
        }
    }

    /// <summary>Resolves defaults and the explicitly selected paragraph style in precedence order.</summary>
    /// <param name="identifier">The explicit paragraph style identifier.</param>
    /// <param name="styles">The named styles.</param>
    /// <param name="css">The CSS destination.</param>
    private static void ReadParagraphNamedStyles(string? identifier, Dictionary<string, XElement> styles, StringBuilder css)
    {
        ReadNamedStyle(DocumentDefaultsKey, styles, css, [with(StringComparer.Ordinal)]);
        ReadNamedStyle(identifier ?? DefaultParagraphStyleKey, styles, css, [with(StringComparer.Ordinal)]);
    }
}
