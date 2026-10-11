// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Xml.Linq;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Enables first and even page stories in Word section and document settings.</summary>
internal static partial class DocxDocumentConverter
{
    /// <summary>The different even and odd page setting.</summary>
    private const string EvenPageSetting = "evenAndOddHeaders";

    /// <summary>The settings schema children preceding the even and odd page setting.</summary>
    private static readonly HashSet<string> SettingsBeforeEvenPages =
    [
        with(StringComparer.Ordinal),
        "writeProtection", "view", "zoom", "removePersonalInformation", "removeDateAndTime", "doNotDisplayPageBoundaries",
        "displayBackgroundShape", "printPostScriptOverText", "printFractionalCharacterWidth", "printFormsData", "embedTrueTypeFonts",
        "embedSystemFonts", "saveSubsetFonts", "saveFormsData", "mirrorMargins", "alignBordersAndEdges", "bordersDoNotSurroundHeader",
        "bordersDoNotSurroundFooter", "gutterAtTop", "hideSpellingErrors", "hideGrammaticalErrors", "activeWritingStyle", "proofState",
        "formsDesign", "attachedTemplate", "linkStyles", "stylePaneFormatFilter", "stylePaneSortMethod", "documentType", "mailMerge",
        "revisionView", "trackRevisions", "doNotTrackMoves", "doNotTrackFormatting", "documentProtection", "autoFormatOverride",
        "styleLockTheme", "styleLockQFSet", "defaultTabStop", "autoHyphenation", "consecutiveHyphenLimit", "hyphenationZone",
        "doNotHyphenateCaps", "showEnvelope", "summaryLength", "clickAndTypeStyle", "defaultTableStyle",
    ];

    /// <summary>Inserts the different even and odd page setting at its schema position.</summary>
    /// <param name="settings">The Word document settings.</param>
    private static void EnableEvenPageSetting(XElement settings)
    {
        if (settings.Element(Word + EvenPageSetting) is { } existing)
        {
            existing.SetAttributeValue(Word + "val", "1");
            return;
        }

        var setting = new XElement(Word + EvenPageSetting, new XAttribute(Word + "val", "1"));
        foreach (var child in settings.Elements())
        {
            if (child.Name.Namespace == Word && SettingsBeforeEvenPages.Contains(child.Name.LocalName))
            {
                continue;
            }

            child.AddBeforeSelf(setting);
            return;
        }

        settings.Add(setting);
    }

    /// <summary>Composes page story activation settings.</summary>
    private sealed partial class PackageWriter
    {
        /// <summary>Enables the Word setting required for the selected header/footer variant.</summary>
        /// <param name="properties">The section properties.</param>
        /// <param name="variant">The region variant.</param>
        private void EnablePageVariant(XElement properties, string variant)
        {
            if (variant == "first")
            {
                Set(properties, "titlePg", "1");
            }

            if (variant != "even")
            {
                return;
            }

            var relationship = FindReaderRelationship(_rels, "/settings");
            var path = relationship is not null ? ResolvePart(_path, relationship.Target) : ResolvePart(_path, "settings.xml");
            if (relationship is null)
            {
                _ = AddRelationship("settings", "settings.xml", false);
            }

            var settings = _parts.TryGetValue(path, out var bytes)
                ? ReadXml(bytes).Root ?? throw new InvalidDataException("Empty Word settings.")
                : new XElement(Word + "settings");
            EnableEvenPageSetting(settings);
            _parts[path] = XmlBytes(settings);
            AddContentType(path, "application/vnd.openxmlformats-officedocument.wordprocessingml.settings+xml");
        }
    }
}
