// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Xml.Linq;
using AngleSharp.Dom;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Applies inherited HTML typography to Word text.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>Reads the typography inherited from enclosing HTML elements.</summary>
    /// <param name="source">The containing element.</param>
    /// <returns>The inherited run properties.</returns>
    private static XElement InheritedRunProperties(INode source)
    {
        var ancestors = new Stack<IElement>();
        for (var element = source as IElement; element is not null; element = element.ParentElement)
        {
            ancestors.Push(element);
        }

        var properties = new XElement(Word + "rPr");
        foreach (var element in ancestors)
        {
            var styles = Styles(element);
            ApplyTypographyStyles(styles, properties);
            _ = styles.Remove("background");
            _ = styles.Remove("background-color");
            ApplyColorStyles(styles, properties);

            if (element.LocalName == "th")
            {
                Set(properties, "b", "1");
            }
        }

        return properties;
    }
}
