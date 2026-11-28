// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.FontResolvers;

/// <summary>Font resolver that creates a typeface from a specified TTF file.</summary>
/// <param name="name">The name of the font family.</param>
/// <param name="path">The path to the font file.</param>
/// <param name="weight">The requested font weight.</param>
/// <param name="slant">The requested font slant.</param>
/// <param name="width">The requested font width.</param>
public class FileFontResolver(string name, string path, FontWeight weight, FontSlant slant, FontSpacing width) : IFontResolver
{
    /// <summary>Initializes a new instance of the <see cref="FileFontResolver"/> class using legacy style flags.</summary>
    /// <param name="name">The name of the font family.</param>
    /// <param name="path">The path to the font file.</param>
    /// <param name="bold">Whether the font is bold.</param>
    /// <param name="italic">Whether the font is italic.</param>
    [Obsolete("Explicitly declare SkFontStyles")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2310",
        Justification = "The obsolete public overload is retained for source and binary compatibility.")]
    public FileFontResolver(string name, string path, bool bold, bool italic)
        : this(
            name,
            path,
            bold ? FontWeight.Bold : FontWeight.Normal,
            italic ? FontSlant.Italic : FontSlant.Upright,
            FontSpacing.Normal)
    {
    }

    /// <summary>Gets the font family name.</summary>
    private string FontName { get; } = name;

    /// <summary>Gets the full path to the font file.</summary>
    private string FontPath { get; } = File.Exists(path)
        ? Path.GetFullPath(path)
        : throw new FileNotFoundException(path);

    /// <summary>Gets the requested font weight.</summary>
    private FontWeight Weight { get; } = weight;

    /// <summary>Gets the requested font slant.</summary>
    private FontSlant Slant { get; } = slant;

    /// <summary>Gets the requested font width.</summary>
    private FontSpacing Width { get; } = width;

    /// <summary>Creates a typeface when the requested font settings match this resolver.</summary>
    /// <param name="fontName">The requested font family.</param>
    /// <param name="weight">The requested font weight.</param>
    /// <param name="slant">The requested font slant.</param>
    /// <param name="spacing">The requested font width.</param>
    /// <returns>The matching typeface, or <see langword="null"/> when this resolver does not match.</returns>
    public SKTypeface? CreateTypeface(string fontName, FontWeight weight, FontSlant slant, FontSpacing spacing) =>
        (FontName == fontName) && (Weight == weight) && (Slant == slant) && (Width == spacing)
            ? SKTypeface.FromFile(FontPath)
            : null;

    /// <summary>Creates a typeface using legacy bold and italic flags.</summary>
    /// <param name="fontName">The requested font family.</param>
    /// <param name="bold">Whether the font is bold.</param>
    /// <param name="italic">Whether the font is italic.</param>
    /// <returns>The matching typeface, or <see langword="null"/> when this resolver does not match.</returns>
    public SKTypeface? CreateTypeface(string fontName, bool bold, bool italic) => CreateTypeface(
            fontName,
            bold ? FontWeight.Bold : FontWeight.Normal,
            italic ? FontSlant.Italic : FontSlant.Upright,
            FontSpacing.Normal);

    /// <summary>Creates a typeface using legacy bold and italic flags with a specified width.</summary>
    /// <param name="fontName">The requested font family.</param>
    /// <param name="bold">Whether the font is bold.</param>
    /// <param name="italic">Whether the font is italic.</param>
    /// <param name="width">The requested font width.</param>
    /// <returns>The matching typeface, or <see langword="null"/> when this resolver does not match.</returns>
    public SKTypeface? CreateTypeface(string fontName, bool bold, bool italic, FontSpacing width) => CreateTypeface(
            fontName,
            bold ? FontWeight.Bold : FontWeight.Normal,
            italic ? FontSlant.Italic : FontSlant.Upright,
            width);

    /// <summary>Returns a string representation of this font resolver.</summary>
    /// <returns>The resolver's font family and style settings.</returns>
    public override string ToString() => $"File font resolver: name='{FontName}', weight={Weight}, slant={Slant}, width={Width}";
}
