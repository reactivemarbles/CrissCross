// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Net;
using System.Text;
using System.Xml.Linq;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Reads native Word PAGE and NUMPAGES fields as dynamic editor placeholders.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>The Word field character element name.</summary>
    private const string FieldCharacterName = "fldChar";

    /// <summary>The Word field character type attribute.</summary>
    private const string FieldCharacterTypeName = "fldCharType";

    /// <summary>Reads a simple page field without flattening its dynamic meaning.</summary>
    /// <param name="element">The Word element.</param>
    /// <param name="output">The HTML destination.</param>
    /// <returns>Whether a field was projected.</returns>
    private static bool ReadSimplePageField(XElement element, StringBuilder output)
    {
        if (element.Name != Word + "fldSimple")
        {
            return false;
        }

        var field = PageFieldName(Value(element, "instr"));
        if (field is null)
        {
            return false;
        }

        var fallback = new StringBuilder();
        foreach (var text in element.Descendants(Word + "t"))
        {
            _ = fallback.Append(text.Value);
        }

        AppendPageField(output, field, fallback.ToString());
        return true;
    }

    /// <summary>Reads a complete same-paragraph complex page field.</summary>
    /// <param name="element">The first field run.</param>
    /// <param name="output">The HTML destination.</param>
    /// <param name="consumed">The following runs consumed by the field.</param>
    /// <returns>Whether a page field was projected.</returns>
    private static bool ReadComplexPageField(XElement element, StringBuilder output, HashSet<XElement> consumed)
    {
        if (element.Name != Word + "r" || Value(element.Element(Word + FieldCharacterName), FieldCharacterTypeName) != "begin")
        {
            return false;
        }

        var fieldNodes = new List<XElement>();
        var instruction = new StringBuilder();
        var fallback = new StringBuilder();
        var finished = CollectPageField(element, fieldNodes, instruction, fallback);
        var field = PageFieldName(instruction.ToString());
        if (!finished || field is null)
        {
            return false;
        }

        foreach (var node in fieldNodes)
        {
            _ = consumed.Add(node);
        }

        AppendPageField(output, field, fallback.ToString());
        return true;
    }

    /// <summary>Collects field instructions and cached display text until the matching end.</summary>
    /// <param name="start">The first run.</param>
    /// <param name="nodes">The field nodes.</param>
    /// <param name="instruction">The instruction destination.</param>
    /// <param name="fallback">The cached text destination.</param>
    /// <returns>Whether a complete nonnested field was found.</returns>
    private static bool CollectPageField(XElement start, List<XElement> nodes, StringBuilder instruction, StringBuilder fallback)
    {
        var first = true;
        for (XNode? node = start; node is not null; node = node.NextNode)
        {
            if (node is not XElement element)
            {
                continue;
            }

            var kind = Value(element.Element(Word + FieldCharacterName), FieldCharacterTypeName);
            if (!first && kind == "begin")
            {
                return false;
            }

            first = false;
            nodes.Add(element);
            AppendFieldText(element, instruction, fallback);
            if (kind == "end")
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Collects a field run's instructions and cached text.</summary>
    /// <param name="element">The field run.</param>
    /// <param name="instruction">The instruction destination.</param>
    /// <param name="fallback">The cached text destination.</param>
    private static void AppendFieldText(XElement element, StringBuilder instruction, StringBuilder fallback)
    {
        foreach (var text in element.Elements(Word + "instrText"))
        {
            _ = instruction.Append(text.Value);
        }

        foreach (var text in element.Elements(Word + "t"))
        {
            _ = fallback.Append(text.Value);
        }
    }

    /// <summary>Recognizes supported page fields while accepting Word format switches.</summary>
    /// <param name="instruction">The field instruction.</param>
    /// <returns>The supported field name.</returns>
    private static string? PageFieldName(string? instruction)
    {
        var tokens = instruction?.Trim().Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
        if (tokens is null || tokens.Length == 0)
        {
            return null;
        }

        var name = tokens[0].ToUpperInvariant();
        return name is "PAGE" or "NUMPAGES" ? name : null;
    }

    /// <summary>Writes an escaped dynamic field placeholder.</summary>
    /// <param name="output">The HTML destination.</param>
    /// <param name="field">The field name.</param>
    /// <param name="fallback">The cached display text.</param>
    private static void AppendPageField(StringBuilder output, string field, string fallback)
    {
        _ = output.Append("<span");
        Attribute(output, "data-page-field", field);
        _ = output.Append('>').Append(WebUtility.HtmlEncode(fallback)).Append("</span>");
    }
}
