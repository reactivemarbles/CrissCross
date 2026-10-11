// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Text;
using System.Xml.Linq;
using AngleSharp.Dom;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Preserves native numbering definitions and supported marker styles.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>The default native numbering format.</summary>
    private const string DecimalNumberFormat = "decimal";

    /// <summary>The abstract numbering definition element.</summary>
    private const string AbstractNumberElement = "abstractNum";

    /// <summary>The initial numbering value element.</summary>
    private const string StartElement = "start";

    /// <summary>Marks imported paragraphs whose native list membership was changed.</summary>
    private const string NumberingResetAttribute = "data-docx-numbering-reset";

    /// <summary>Checks whether imported paragraph numbering must be disabled.</summary>
    /// <param name="paragraph">The source paragraph.</param>
    /// <returns>Whether native numbering must be reset.</returns>
    private static bool HasNumberingReset(IElement? paragraph) =>
        paragraph?.GetAttribute(NumberingResetAttribute) == "true" || paragraph?.ParentElement?.GetAttribute(NumberingResetAttribute) == "true";

    /// <summary>Reads the effective numbering level, including instance overrides.</summary>
    /// <param name="parts">The package parts.</param>
    /// <param name="path">The story path.</param>
    /// <param name="rels">The story relationships.</param>
    /// <param name="id">The numbering instance.</param>
    /// <param name="level">The native level.</param>
    /// <returns>The effective format and first number.</returns>
    private static (string Format, string Start) ReadListLevel(
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels,
        string id,
        int level)
    {
        var root = ReadNumberingRoot(parts, path, rels);
        return root is null ? (DecimalNumberFormat, "1") : ReadEffectiveListLevel(root, id, level);
    }

    /// <summary>Loads the numbering part referenced by the story.</summary>
    /// <param name="parts">The package parts.</param>
    /// <param name="path">The story path.</param>
    /// <param name="rels">The story relationships.</param>
    /// <returns>The numbering root when the part is available.</returns>
    private static XElement? ReadNumberingRoot(
        Dictionary<string, byte[]> parts,
        string path,
        Dictionary<string, PackageRelationship> rels)
    {
        var relationship = FindReaderRelationship(rels, "/numbering");
        return relationship?.External == false && parts.TryGetValue(ResolvePart(path, relationship.Target), out var bytes)
            ? ReadXml(bytes).Root
            : null;
    }

    /// <summary>Resolves a numbering instance and level to its effective properties.</summary>
    /// <param name="root">The numbering root.</param>
    /// <param name="id">The numbering instance identifier.</param>
    /// <param name="level">The native level.</param>
    /// <returns>The effective format and first number.</returns>
    private static (string Format, string Start) ReadEffectiveListLevel(XElement? root, string id, int level)
    {
        var number = FindReaderElement(root, Word + "num", ReaderNumberId, id);
        var abstractId = Value(number?.Element(Word + ReaderAbstractNumberId));
        var definition = FindReaderElement(root, Word + AbstractNumberElement, ReaderAbstractNumberId, abstractId);
        var index = level.ToString(CultureInfo.InvariantCulture);
        var original = FindReaderElement(definition, Word + "lvl", ReaderListLevel, index);
        var replacement = FindReaderElement(number, Word + "lvlOverride", ReaderListLevel, index);
        var effective = replacement?.Element(Word + "lvl") ?? original;
        return (
            Value(effective?.Element(Word + "numFmt")) ?? DecimalNumberFormat,
            Value(replacement?.Element(Word + "startOverride")) ?? Value(effective?.Element(Word + StartElement)) ?? "1");
    }

    /// <summary>Writes a list opening with effective numbering properties.</summary>
    /// <param name="output">The HTML output.</param>
    /// <param name="tag">The list element name.</param>
    /// <param name="numbering">The effective numbering properties.</param>
    private static void AppendListOpening(StringBuilder output, string tag, (string Format, string Start) numbering)
    {
        _ = output.Append('<').Append(tag);
        if (tag == "ol")
        {
            Attribute(output, StartElement, numbering.Start);
        }

        Attribute(output, "style", $"list-style-type:{NumberingCss(numbering.Format)}");
        _ = output.Append('>');
    }

    /// <summary>Maps Word formats to CSS marker styles.</summary>
    /// <param name="format">The Word format.</param>
    /// <returns>The marker style.</returns>
    private static string NumberingCss(string format) => format switch
    {
        "upperRoman" => "upper-roman",
        "lowerRoman" => "lower-roman",
        "upperLetter" => "upper-alpha",
        "lowerLetter" => "lower-alpha",
        "bullet" => "disc",
        _ => DecimalNumberFormat,
    };

    /// <summary>Writes numbering while retaining imported native definitions.</summary>
    private sealed partial class PackageWriter
    {
        /// <summary>Applies explicit paragraph numbering or suppresses inherited native numbering.</summary>
        /// <param name="element">The HTML paragraph.</param>
        /// <param name="properties">The native paragraph properties.</param>
        private static void ApplyParagraphNumbering(IElement element, XElement properties)
        {
            if (HasNumberingReset(element))
            {
                SetNumber(properties, "0", "0");
                return;
            }

            if (element.GetAttribute("data-docx-num-id") is not { } number)
            {
                return;
            }

            SetNumber(properties, number, element.GetAttribute("data-docx-level") ?? "0");
        }

        /// <summary>Maps HTML list styles to Word numbering formats.</summary>
        /// <param name="list">The source list.</param>
        /// <returns>The native numbering format.</returns>
        private static string ListFormat(IElement list)
        {
            var style = Styles(list).GetValueOrDefault("list-style-type");
            return style switch
            {
                "upper-roman" => "upperRoman",
                "lower-roman" => "lowerRoman",
                "upper-alpha" or "upper-latin" => "upperLetter",
                "lower-alpha" or "lower-latin" => "lowerLetter",
                "disc" or "circle" or "square" => "bullet",
                _ => list.LocalName == "ul" ? "bullet" : DecimalNumberFormat,
            };
        }

        /// <summary>Reuses an imported identifier or allocates the requested marker style.</summary>
        /// <param name="list">The source list.</param>
        /// <returns>The numbering instance identifier.</returns>
        private string ListNumbering(IElement list)
        {
            foreach (var item in list.Children)
            {
                if (item.LocalName == "li" && PreservedNumbering(FirstParagraph(item)) is { } preserved)
                {
                    return preserved;
                }
            }

            var number = CreateNumbering(list.LocalName == "ul", list.GetAttribute("start"));
            var instance = FindReaderElement(_numbering, Word + "num", WriterNumberId, number);
            var abstractId = Value(instance?.Element(Word + WriterAbstractNumberId));
            var definition = FindReaderElement(_numbering, Word + AbstractNumberElement, WriterAbstractNumberId, abstractId);
            var format = ListFormat(list);
            if (definition is not null)
            {
                foreach (var level in definition.Elements(Word + "lvl"))
                {
                    Set(level, "numFmt", format);
                }
            }

            return number;
        }

        /// <summary>Writes one list item and any nested lists.</summary>
        /// <param name="item">The source list item.</param>
        /// <param name="target">The Word parent.</param>
        /// <param name="level">The nesting level.</param>
        /// <param name="number">The list numbering identifier.</param>
        private void WriteListItem(IElement item, XElement target, int level, string number)
        {
            var sourceParagraph = FirstParagraph(item);
            var paragraph = WriteParagraph(sourceParagraph ?? item);
            var properties = paragraph.Element(Word + "pPr");
            if (properties is null)
            {
                properties = new(Word + "pPr");
                paragraph.AddFirst(properties);
            }

            SetNumber(
                properties,
                PreservedNumbering(sourceParagraph) ?? number,
                sourceParagraph?.GetAttribute("data-docx-level") ?? level.ToString(CultureInfo.InvariantCulture));
            target.Add(paragraph);
            foreach (var nested in item.Children)
            {
                if (nested.LocalName is "ol" or "ul")
                {
                    WriteList(nested, target, level + 1);
                }
            }
        }

        /// <summary>Finds a paragraph's existing native numbering instance.</summary>
        /// <param name="paragraph">The imported paragraph.</param>
        /// <returns>The native identifier when its definition exists.</returns>
        private string? PreservedNumbering(IElement? paragraph)
        {
            if (HasNumberingReset(paragraph))
            {
                return null;
            }

            var id = paragraph?.GetAttribute("data-docx-num-id");
            var relationship = NumberingRelationship();
            if (id is null || id == "0" || relationship?.External != false)
            {
                return null;
            }

            var root = _numbering ?? ReadNumberingRoot(_parts, _path, _rels);
            var number = FindReaderElement(root, Word + "num", WriterNumberId, id);
            var abstractId = Value(number?.Element(Word + WriterAbstractNumberId));
            return FindReaderElement(root, Word + AbstractNumberElement, WriterAbstractNumberId, abstractId) is not null ? id : null;
        }
    }
}
