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

/// <summary>Verifies editable table CSS against native Word properties and schema.</summary>
public sealed class DocxTableStyleTests
{
    /// <summary>The native document part path.</summary>
    private const string DocumentPartPath = "word/document.xml";

    /// <summary>The HTML style attribute name.</summary>
    private const string HtmlStyleAttribute = "style";

    /// <summary>The native table properties element name.</summary>
    private const string TablePropertiesElement = "tblPr";

    /// <summary>The native table borders element name.</summary>
    private const string TableBordersElement = "tblBorders";

    /// <summary>The WordprocessingML namespace.</summary>
    private static readonly XNamespace Word = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

    /// <summary>Fresh table and cell styles use native units and valid property order.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task FreshCss_ExportsNativeTableAndCellStyles()
    {
        var bytes = DocxDocumentConverter.ToDocx("""
            <table style='width:50%;padding:1px 2px 3px 4px;border:2px dashed #112233;background-color:#ABCDEF'>
            <tr><td style='width:120px;padding:6px;border:1px solid red;border-left:3px double blue;vertical-align:middle;background-color:rgb(10,20,30)'><p>Styled</p></td></tr>
            </table>
            """);
        var xml = ReadXml(bytes);
        await Assert.That(Value(xml, TablePropertiesElement, "tblW", "w")).IsEqualTo("2500");
        await Assert.That(Value(xml, TablePropertiesElement, "shd", "fill")).IsEqualTo("ABCDEF");
        await Assert.That(Value(xml, "tblCellMar", "left", "w")).IsEqualTo("60");
        await Assert.That(Value(xml, TableBordersElement, "top", "sz")).IsEqualTo("12");
        await Assert.That(Value(xml, "tcPr", "tcW", "w")).IsEqualTo("1800");
        await Assert.That(Value(xml, "tcPr", "vAlign", "val")).IsEqualTo("center");
        await Assert.That(Value(xml, "tcPr", "shd", "fill")).IsEqualTo("0A141E");
        await Assert.That(Value(xml, "tcBorders", "left", "val")).IsEqualTo("double");
        await Assert.That(SchemaErrors(bytes)).IsEmpty();
    }

    /// <summary>Unchanged visible CSS retains native border variants and metadata.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ImportedCss_UnchangedBorderSurvivesFillEdit()
    {
        var original = NativeBorderPackage();
        var html = new HtmlParser().ParseDocument(DocxDocumentConverter.ToHtml(original));
        var table = html.QuerySelector("table") ?? throw new InvalidDataException("Missing imported table.");
        var body = html.Body ?? throw new InvalidDataException("Missing document body.");
        table.SetAttribute(HtmlStyleAttribute, $"{table.GetAttribute(HtmlStyleAttribute)}background-color:#00FF00;");
        var bytes = DocxDocumentConverter.ToDocx(body.InnerHtml);
        var exported = ReadXml(bytes);
        await Assert.That(Value(exported, TableBordersElement, "top", "val")).IsEqualTo("dashDotStroked");
        await Assert.That(Value(exported, TableBordersElement, "top", "space")).IsEqualTo("7");
        await Assert.That(Value(exported, TablePropertiesElement, "shd", "fill")).IsEqualTo("00FF00");
        await Assert.That(SchemaErrors(bytes)).IsEmpty();
    }

    /// <summary>Editing an imported CSS border replaces its visible native formatting.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task ImportedCss_ChangedBorderUpdatesNativeFormatting()
    {
        var html = new HtmlParser().ParseDocument(DocxDocumentConverter.ToHtml(NativeBorderPackage()));
        var table = html.QuerySelector("table") ?? throw new InvalidDataException("Missing imported table.");
        var body = html.Body ?? throw new InvalidDataException("Missing document body.");
        table.SetAttribute(HtmlStyleAttribute, "border:4px dotted #445566;width:75%;padding:0;");
        var bytes = DocxDocumentConverter.ToDocx(body.InnerHtml);
        var xml = ReadXml(bytes);
        await Assert.That(Value(xml, TableBordersElement, "top", "val")).IsEqualTo("dotted");
        await Assert.That(Value(xml, TableBordersElement, "top", "sz")).IsEqualTo("24");
        await Assert.That(Value(xml, TableBordersElement, "top", "color")).IsEqualTo("445566");
        await Assert.That(Value(xml, TablePropertiesElement, "tblW", "w")).IsEqualTo("3750");
        await Assert.That(SchemaErrors(bytes)).IsEmpty();
    }

    /// <summary>Cell typography reaches text in an ordinary nested paragraph.</summary>
    /// <returns>The assertion task.</returns>
    [Test]
    public async Task CellTypography_InheritsIntoNestedParagraph()
    {
        var bytes = DocxDocumentConverter.ToDocx(
            "<table><tr><td style='font-family:Consolas;font-size:20px;color:#123456'><p>Inherited</p></td></tr></table>");
        var xml = ReadXml(bytes);
        await Assert.That(Value(xml, "rPr", "rFonts", "ascii")).IsEqualTo("Consolas");
        await Assert.That(Value(xml, "rPr", "sz", "val")).IsEqualTo("30");
        await Assert.That(Value(xml, "rPr", "color", "val")).IsEqualTo("123456");
        await Assert.That(SchemaErrors(bytes)).IsEmpty();
    }

    /// <summary>Creates a package with a native border variant CSS cannot represent exactly.</summary>
    /// <returns>The native package.</returns>
    private static byte[] NativeBorderPackage()
    {
        var bytes = DocxDocumentConverter.ToDocx("<table style='border:2px dashed #112233'><tr><td>Native</td></tr></table>");
        using var stream = new MemoryStream();
        stream.Write(bytes);
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Update, true))
        {
            var entry = archive.GetEntry(DocumentPartPath) ?? throw new InvalidDataException("Missing document part.");
            XElement xml;
            using (var input = entry.Open())
            {
                xml = XElement.Load(input);
            }

            var border = Find(xml, TableBordersElement).Element(Word + "top") ?? throw new InvalidDataException("Missing border.");
            border.SetAttributeValue(Word + "val", "dashDotStroked");
            border.SetAttributeValue(Word + "space", "7");
            entry.Delete();
            using var output = archive.CreateEntry(DocumentPartPath).Open();
            xml.Save(output);
        }

        return stream.ToArray();
    }

    /// <summary>Reads the main native Word XML part.</summary>
    /// <param name="bytes">The package bytes.</param>
    /// <returns>The main Word XML.</returns>
    private static XElement ReadXml(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = archive.GetEntry(DocumentPartPath) ?? throw new InvalidDataException("Missing document part.");
        using var part = entry.Open();
        return XElement.Load(part);
    }

    /// <summary>Finds the first native element with a particular name.</summary>
    /// <param name="xml">The document XML.</param>
    /// <param name="name">The native local name.</param>
    /// <returns>The matching element.</returns>
    private static XElement Find(XElement xml, string name)
    {
        using var enumerator = xml.Descendants(Word + name).GetEnumerator();
        return enumerator.MoveNext()
            ? enumerator.Current
            : throw new InvalidDataException($"Missing {name} element.");
    }

    /// <summary>Reads a required native XML attribute through a checked element path.</summary>
    /// <param name="xml">The document containing the native elements.</param>
    /// <param name="container">The container element local name.</param>
    /// <param name="child">The child element local name.</param>
    /// <param name="attribute">The attribute local name.</param>
    /// <returns>The required attribute value.</returns>
    private static string Value(XElement xml, string container, string child, string attribute)
    {
        var parent = Find(xml, container);
        var element = parent.Element(Word + child)
            ?? throw new InvalidDataException($"Missing {child} element.");
        var value = element.Attribute(Word + attribute)
            ?? throw new InvalidDataException($"Missing {attribute} attribute.");
        return value.Value;
    }

    /// <summary>Collects independent Open XML schema failures.</summary>
    /// <param name="bytes">The package bytes.</param>
    /// <returns>The validation messages.</returns>
    private static List<string> SchemaErrors(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var document = WordprocessingDocument.Open(stream, false);
        var errors = new List<string>();
        foreach (var error in new OpenXmlValidator(FileFormatVersions.Microsoft365).Validate(document))
        {
            errors.Add(error.Description);
        }

        return errors;
    }
}
