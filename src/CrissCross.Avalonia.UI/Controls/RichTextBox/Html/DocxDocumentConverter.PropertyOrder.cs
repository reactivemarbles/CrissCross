// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Xml.Linq;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Keeps edited Word properties in their schema-defined sequence.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>The property sequence for each editable Word property container.</summary>
    private static readonly Dictionary<string, string[]> PropertySequences = new(StringComparer.Ordinal)
    {
        ["pPr"] = ["pStyle", "keepNext", "keepLines", "pageBreakBefore", "framePr", "widowControl", "numPr", "suppressLineNumbers",
            "pBdr", "shd", "tabs", "suppressAutoHyphens", "kinsoku", "wordWrap", "overflowPunct", "topLinePunct", "autoSpaceDE", "autoSpaceDN",
            "bidi", "adjustRightInd", "snapToGrid", "spacing", "ind", "contextualSpacing", "mirrorIndents", "suppressOverlap", "jc", "textDirection",
            "textAlignment", "textboxTightWrap", "outlineLvl", "divId", "cnfStyle", "rPr", "sectPr", "pPrChange"],
        ["rPr"] = ["rStyle", "rFonts", "b", "bCs", "i", "iCs", "caps", "smallCaps", "strike", "dStrike", "outline", "shadow", "emboss",
            "imprint", "noProof", "snapToGrid", "vanish", "webHidden", "color", "spacing", "w", "kern", "position", "sz", "szCs", "highlight", "u",
            "effect", "bdr", "shd", "fitText", "vertAlign", "rtl", "cs", "em", "lang", "eastAsianLayout", "specVanish", "oMath", "rPrChange"],
        ["tcPr"] = ["cnfStyle", "tcW", "gridSpan", "hMerge", "vMerge", "tcBorders", "shd", "noWrap", "tcMar", "textDirection", "tcFitText",
            "vAlign", "hideMark", "headers", "cellIns", "cellDel", "cellMerge", "tcPrChange"],
        ["tblPr"] = ["tblStyle", "tblpPr", "tblOverlap", "bidiVisual", "tblStyleRowBandSize", "tblStyleColBandSize", "tblW", "jc",
            "tblCellSpacing", "tblInd", "tblBorders", "shd", "tblLayout", "tblCellMar", "tblLook", "tblCaption", "tblDescription", "tblPrChange"],
        ["sectPr"] = ["headerReference", "footerReference", "footnotePr", "endnotePr", "type", "pgSz", "pgMar", "paperSrc", "pgBorders",
            "lnNumType", "pgNumType", "cols", "formProt", "vAlign", "noEndnote", "titlePg", "textDirection", "bidi", "rtlGutter", "docGrid",
            "printerSettings", "sectPrChange"],
    };

    /// <summary>Sorts property children without changing their values.</summary>
    /// <param name="root">The Word XML root.</param>
    private static void OrderWordProperties(XElement root)
    {
        foreach (var element in root.DescendantsAndSelf())
        {
            if (element.Name.Namespace == Word && PropertySequences.TryGetValue(element.Name.LocalName, out var sequence))
            {
                OrderWordPropertyChildren(element, sequence);
            }
        }
    }

    /// <summary>Uses stable ordering so unknown extension properties retain their relative order.</summary>
    /// <param name="element">The properties container.</param>
    /// <param name="sequence">The schema sequence.</param>
    private static void OrderWordPropertyChildren(XElement element, string[] sequence)
    {
        var properties = new List<(XElement Element, int Rank, int Index)>();
        var index = 0;
        foreach (var child in element.Elements())
        {
            var rank = child.Name.Namespace == Word ? Array.IndexOf(sequence, child.Name.LocalName) : -1;
            properties.Add((child, rank < 0 ? int.MaxValue : rank, index));
            index++;
        }

        properties.Sort(static (left, right) => left.Rank != right.Rank ? left.Rank.CompareTo(right.Rank) : left.Index.CompareTo(right.Index));
        element.RemoveNodes();
        foreach (var property in properties)
        {
            element.Add(property.Element);
        }
    }
}
