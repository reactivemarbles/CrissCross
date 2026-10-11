// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using AngleSharp.Html.Parser;
using CrissCross.Avalonia.UI.Controls;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;

namespace CrissCross.NavigationView.Tests;

/// <summary>Checks numbering against native package XML and Office schemas.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class DocxNumberingTests
{
    /// <summary>The native numbering part path.</summary>
    private const string NumberingPartPath = "word/numbering.xml";

    /// <summary>The native document part path.</summary>
    private const string DocumentPartPath = "word/document.xml";

    /// <summary>The expected list start value.</summary>
    private const int ExpectedListStart = 7;

    /// <summary>The native numbering identifier element and attribute.</summary>
    private const string NativeNumberId = "numId";

    /// <summary>The native paragraph numbering property.</summary>
    private const string NativeNumberProperties = "numPr";

    /// <summary>The native abstract numbering identifier.</summary>
    private const string NativeAbstractNumberId = "abstractNumId";

    /// <summary>The native numbering format element.</summary>
    private const string NativeNumberFormat = "numFmt";

    /// <summary>The first imported list item text.</summary>
    private const string FirstItemText = "First";

    /// <summary>The WordprocessingML namespace.</summary>
    private static readonly XNamespace Word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

    /// <summary>Imported custom numbering and instance start overrides survive text edits.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task NativeNumbering_EditExport_PreservesDefinitionsAndReferences()
    {
        var fixture = CreateNumberingFixture();
        var original = ReadPart(fixture, NumberingPartPath);
        var html = DocxDocumentConverter.ToHtml(fixture);
        var parsed = new HtmlParser().ParseDocument(html);
        var list = parsed.QuerySelector("ol") ?? throw new InvalidDataException("Missing ordered list.");
        var style = list.GetAttribute("style") ?? throw new InvalidDataException("Missing ordered list style.");
        await Assert.That(list.GetAttribute("start")).IsEqualTo(ExpectedListStart.ToString(System.Globalization.CultureInfo.InvariantCulture));
        await Assert.That(style).Contains("upper-roman");
        var output = DocxDocumentConverter.ToDocx(html.Replace(FirstItemText, "Edited", StringComparison.Ordinal));
        await Assert.That(XNode.DeepEquals(original, ReadPart(output, NumberingPartPath))).IsTrue();
        var before = ReadPart(fixture, DocumentPartPath);
        var after = ReadPart(output, DocumentPartPath);
        await Assert.That(Value(before, Word + NativeNumberProperties, Word + NativeNumberId, Word + "val"))
            .IsEqualTo(Value(after, Word + NativeNumberProperties, Word + NativeNumberId, Word + "val"));
        await Assert.That(after.ToString()).Contains("Edited");
        await Assert.That(ValidationErrors(output)).IsEmpty();
    }

    /// <summary>Fresh Roman lists export their start and format as valid native properties.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task HtmlRomanList_Export_UsesNativeFormatAndStart()
    {
        var bytes = DocxDocumentConverter.ToDocx("<ol start='7' style='list-style-type:lower-roman'><li>First</li><li>Second</li></ol>");
        var numbering = ReadPart(bytes, NumberingPartPath);
        await Assert.That(Value(numbering, Word + "lvl", Word + NativeNumberFormat, Word + "val")).IsEqualTo("lowerRoman");
        await Assert.That(Value(numbering, Word + "lvl", Word + "start", Word + "val"))
            .IsEqualTo(ExpectedListStart.ToString(System.Globalization.CultureInfo.InvariantCulture));
        await Assert.That(ValidationErrors(bytes)).IsEmpty();
    }

    /// <summary>Removing an imported list overrides preserved native numbering.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ImportedList_ToggleOff_ExportsExplicitNumberingReset()
    {
        var editor = ImportEditor(CreateNumberingFixture());
        editor.Select(0, FirstItemText.Length);
        editor.ToggleNumberedList();
        var bytes = ExportEditor(editor);
        await Assert.That(Value(ReadPart(bytes, DocumentPartPath), Word + NativeNumberProperties, Word + NativeNumberId, Word + "val")).IsEqualTo("0");
        await Assert.That(ValidationErrors(bytes)).IsEmpty();
    }

    /// <summary>Changing an imported Roman list to bullets allocates native bullet numbering.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ImportedList_ToggleBullets_ExportsNewBulletNumbering()
    {
        var fixture = CreateNumberingFixture();
        var originalId = Value(ReadPart(fixture, DocumentPartPath), Word + NativeNumberProperties, Word + NativeNumberId, Word + "val");
        var editor = ImportEditor(fixture);
        editor.Select(0, FirstItemText.Length);
        editor.ToggleBulletedList();
        var bytes = ExportEditor(editor);
        var numberId = Value(ReadPart(bytes, DocumentPartPath), Word + NativeNumberProperties, Word + NativeNumberId, Word + "val");
        await Assert.That(numberId == originalId).IsFalse();
        await Assert.That(numberId == "0").IsFalse();
        await Assert.That(MarkerFormat(ReadPart(bytes, NumberingPartPath), numberId)).IsEqualTo("bullet");
        await Assert.That(ValidationErrors(bytes)).IsEmpty();
    }

    /// <summary>Imports a native package through the public editor API.</summary>
    /// <param name="bytes">The package bytes.</param>
    /// <returns>The loaded editor.</returns>
    private static RichTextBox ImportEditor(byte[] bytes)
    {
        using var input = new MemoryStream(bytes);
        var editor = new RichTextBox();
        editor.Load(input, RichTextDataFormat.Docx);
        return editor;
    }

    /// <summary>Exports an edited document through the public editor API.</summary>
    /// <param name="editor">The edited control.</param>
    /// <returns>The native package.</returns>
    private static byte[] ExportEditor(RichTextBox editor)
    {
        using var output = new MemoryStream();
        editor.Save(output, RichTextDataFormat.Docx);
        return output.ToArray();
    }

    /// <summary>Resolves a paragraph's numbering identifier to its native marker format.</summary>
    /// <param name="xml">The numbering definitions.</param>
    /// <param name="numberId">The paragraph's numbering identifier.</param>
    /// <returns>The level-zero marker format.</returns>
    private static string MarkerFormat(XDocument xml, string numberId)
    {
        var number = FindIdentifier(xml, "num", NativeNumberId, numberId);
        var abstractId = RequiredChild(number, NativeAbstractNumberId).Attribute(Word + "val")?.Value
            ?? throw new InvalidDataException("Missing abstract numbering reference.");
        var definition = FindIdentifier(xml, "abstractNum", NativeAbstractNumberId, abstractId);
        return RequiredChild(RequiredChild(definition, "lvl"), NativeNumberFormat).Attribute(Word + "val")?.Value
            ?? throw new InvalidDataException("Missing native marker format.");
    }

    /// <summary>Finds a native numbering element with the requested identifier.</summary>
    /// <param name="xml">The numbering document.</param>
    /// <param name="name">The element name.</param>
    /// <param name="attribute">The identifier attribute.</param>
    /// <param name="identifier">The identifier value.</param>
    /// <returns>The matching native definition.</returns>
    private static XElement FindIdentifier(XDocument xml, string name, string attribute, string identifier)
    {
        foreach (var element in xml.Descendants(Word + name))
        {
            if ((string?)element.Attribute(Word + attribute) == identifier)
            {
                return element;
            }
        }

        throw new InvalidDataException($"Missing native {name} definition {identifier}.");
    }

    /// <summary>Injects native properties without using the converter's numbering reader.</summary>
    /// <returns>The modified package.</returns>
    private static byte[] CreateNumberingFixture()
    {
        var bytes = DocxDocumentConverter.ToDocx("<ol><li>First</li><li>Second</li></ol>");
        using var stream = new MemoryStream();
        stream.Write(bytes);
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Update, true))
        {
            var entry = archive.GetEntry(NumberingPartPath) ?? throw new InvalidDataException("Missing numbering part.");
            XDocument numbering;
            using (var input = entry.Open())
            {
                numbering = XDocument.Load(input);
            }

            var level = FirstDescendant(numbering, "lvl") ?? throw new InvalidDataException("Missing numbering level.");
            RequiredChild(level, NativeNumberFormat).SetAttributeValue(Word + "val", "upperRoman");
            RequiredChild(level, "lvlText").SetAttributeValue(Word + "val", "Article %1)");
            var number = FirstDescendant(numbering, "num") ?? throw new InvalidDataException("Missing numbering instance.");
            number.Add(new XElement(
                Word + "lvlOverride",
                new XAttribute(Word + "ilvl", 0),
                new XElement(Word + "startOverride", new XAttribute(Word + "val", ExpectedListStart))));
            entry.Delete();
            using var output = archive.CreateEntry(NumberingPartPath).Open();
            numbering.Save(output);
        }

        return stream.ToArray();
    }

    /// <summary>Reads native XML directly from its ZIP entry.</summary>
    /// <param name="bytes">The package bytes.</param>
    /// <param name="path">The entry path.</param>
    /// <returns>The native XML.</returns>
    private static XDocument ReadPart(byte[] bytes, string path)
    {
        using var stream = new MemoryStream(bytes);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = archive.GetEntry(path) ?? throw new InvalidDataException($"Missing {path} part.");
        using var input = entry.Open();
        return XDocument.Load(input);
    }

    /// <summary>Finds a native descendant without relying on text matching.</summary>
    /// <param name="document">The XML document.</param>
    /// <param name="name">The Word element name.</param>
    /// <returns>The first matching element.</returns>
    private static XElement? FirstDescendant(XDocument document, string name)
    {
        using var enumerator = document.Descendants(Word + name).GetEnumerator();
        return enumerator.MoveNext() ? enumerator.Current : null;
    }

    /// <summary>Reads a required native XML attribute through a checked element path.</summary>
    /// <param name="xml">The document containing the native elements.</param>
    /// <param name="container">The container element name.</param>
    /// <param name="child">The child element name.</param>
    /// <param name="attribute">The attribute name.</param>
    /// <returns>The required attribute value.</returns>
    private static string Value(XDocument xml, XName container, XName child, XName attribute)
    {
        var parent = FirstDescendant(xml, container.LocalName)
            ?? throw new InvalidDataException($"Missing {container.LocalName} element.");
        var element = parent.Element(child)
            ?? throw new InvalidDataException($"Missing {child.LocalName} element.");
        var value = element.Attribute(attribute)
            ?? throw new InvalidDataException($"Missing {attribute.LocalName} attribute.");
        return value.Value;
    }

    /// <summary>Gets a required direct child element.</summary>
    /// <param name="parent">The parent element.</param>
    /// <param name="name">The child local name.</param>
    /// <returns>The required child element.</returns>
    private static XElement RequiredChild(XElement parent, string name) =>
        parent.Element(Word + name) ?? throw new InvalidDataException($"Missing {name} element.");

    /// <summary>Collects independent schema validation failures.</summary>
    /// <param name="bytes">The package bytes.</param>
    /// <returns>The validation failures.</returns>
    private static List<string> ValidationErrors(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var package = WordprocessingDocument.Open(stream, false);
        List<string> errors = [];
        foreach (var error in new OpenXmlValidator(FileFormatVersions.Microsoft365).Validate(package))
        {
            errors.Add(error.Description ?? "Open XML validation error.");
        }

        return errors;
    }
}
