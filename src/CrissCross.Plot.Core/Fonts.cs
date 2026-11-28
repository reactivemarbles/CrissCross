// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.Collections.Concurrent;
using System.Text;
using CrissCross.Plot.FontResolvers;

namespace CrissCross.Plot;

/// <summary>Cross-platform tools for working with fonts.</summary>
public static class Fonts
{
    /* Typefaces are cached to improve performance. */
    /// <summary>The cache of resolved typefaces indexed by font family and style.</summary>
    private static readonly ConcurrentDictionary<(string FontName, FontWeight Weight, FontSlant Slant, FontSpacing Spacing), SKTypeface> TypefaceCache = [];

    /// <summary>Gets the font resolvers that create typefaces from names and style information.</summary>
    public static List<IFontResolver> FontResolvers { get; } = [new SystemFontResolver()];

    /// <summary>Gets or sets the default font weight.</summary>
    public static FontWeight? DefaultWeight { get; set; }

    /// <summary>Gets or sets the default font slant.</summary>
    public static FontSlant? DefaultSlant { get; set; }

    /// <summary>Gets or sets the default font spacing.</summary>
    public static FontSpacing? DefaultWidth { get; set; }

    /// <summary>Gets or sets the default typeface style.</summary>
    public static SKTypeface? DefaultFontStyle { get; set; }

    /// <summary>Gets or sets the font used for most text rendering.</summary>
    public static string Default { get; set; } = SystemFontFamilyResolver.InstalledSansFont();

    /// <summary>Gets or sets the name of an installed sans-serif font.</summary>
    public static string Sans { get; set; } = SystemFontFamilyResolver.InstalledSansFont();

    /// <summary>Gets or sets the name of an installed serif font.</summary>
    public static string Serif { get; set; } = SystemFontFamilyResolver.InstalledSerifFont();

    /// <summary>Gets or sets the name of an installed monospace font.</summary>
    public static string Monospace { get; set; } = SystemFontFamilyResolver.InstalledMonospaceFont();

    /// <summary>Gets the default system font name.</summary>
    public static string System => SystemFontResolver.DefaultSystemFont();

    /// <summary>Adds an installed TTF file as a font resolver using regular upright style.</summary>
    /// <param name="name">The font family name.</param>
    /// <param name="path">The path to the font file.</param>
    public static void AddFontFile(string name, string path) => AddFontFile(name, path, false, false);

    /// <summary>Adds an installed TTF file as a font resolver with the specified weight.</summary>
    /// <param name="name">The font family name.</param>
    /// <param name="path">The path to the font file.</param>
    /// <param name="bold">Whether the font should use bold weight.</param>
    public static void AddFontFile(string name, string path, bool bold) => AddFontFile(name, path, bold, false);

    /// <summary>Adds a font resolver that creates a typeface from a TTF file.</summary>
    /// <param name="name">The font family name.</param>
    /// <param name="path">The path to the font file.</param>
    /// <param name="bold">Whether the font should use bold weight.</param>
    /// <param name="italic">Whether the font should use italic slant.</param>
    public static void AddFontFile(string name, string path, bool bold, bool italic)
    {
        FontWeight weight = bold ? FontWeight.Bold : FontWeight.Normal;
        FontSlant slant = italic ? FontSlant.Italic : FontSlant.Upright;
        const FontSpacing width = FontSpacing.Normal;
        FileFontResolver resolver = new(name, path, weight, slant, width);
        FontResolvers.Add(resolver);
    }

    /// <summary>Legacy API that is unavailable; use <see cref="GetTypeface(string, FontWeight, FontSlant, FontSpacing)"/> instead.</summary>
    /// <param name="fontName">The font family name.</param>
    /// <returns>This legacy API always throws.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2310",
        Justification = "Retained obsolete public API for compatibility; it is compile-time obsolete and cannot be removed without a breaking change.")]
    [Obsolete("To determine if a font exists, call GetTypeface() and check for null", true)]
    public static bool Exists(string fontName)
    {
        throw new NotFiniteNumberException();
    }

    /// <summary>Legacy API that is unavailable; use <see cref="GetTypeface(string, bool, bool)"/> instead.</summary>
    /// <param name="fontName">The font family name.</param>
    /// <param name="bold">Whether to use bold weight.</param>
    /// <param name="italic">Whether to use italic slant.</param>
    /// <returns>This legacy API always throws.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2310",
        Justification = "Retained obsolete public API for compatibility; it is compile-time obsolete and cannot be removed without a breaking change.")]
    [Obsolete("To determine if a font exists, call GetTypeface() and check for null", true)]
    public static bool Exists(string fontName, bool bold, bool italic)
    {
        throw new NotFiniteNumberException();
    }

    /// <summary>Gets a typeface for the requested font name and style, using a cached typeface when available.</summary>
    /// <param name="fontName">The font family name.</param>
    /// <param name="weight">The requested font weight.</param>
    /// <param name="slant">The requested font slant.</param>
    /// <param name="spacing">The requested font spacing.</param>
    /// <returns>The matching typeface, or the default typeface when no resolver matches.</returns>
    public static SKTypeface GetTypeface(string fontName, FontWeight weight, FontSlant slant, FontSpacing spacing)
    {
        var typefaceCacheKey = (fontName, weight, slant, spacing);

        if (TypefaceCache.TryGetValue(typefaceCacheKey, out SKTypeface? cachedTypeface) && cachedTypeface is not null)
        {
            return cachedTypeface;
        }

        foreach (IFontResolver resolver in FontResolvers)
        {
            SKTypeface? resolvedTypeface = resolver.CreateTypeface(fontName, weight, slant, spacing);
            if (resolvedTypeface is not null)
            {
                _ = TypefaceCache.TryAdd(typefaceCacheKey, resolvedTypeface);
                return resolvedTypeface;
            }
        }

        SKTypeface defaultTypeface = SystemFontResolver.CreateDefaultTypeface();
        _ = TypefaceCache.TryAdd(typefaceCacheKey, defaultTypeface);
        return defaultTypeface;
    }

    /// <summary>Gets a typeface for the requested font name and basic style.</summary>
    /// <param name="fontName">The font family name.</param>
    /// <param name="bold">Whether to use bold weight.</param>
    /// <param name="italic">Whether to use italic slant.</param>
    /// <returns>The matching typeface, or the default typeface when no resolver matches.</returns>
    public static SKTypeface GetTypeface(string fontName, bool bold, bool italic)
    {
        FontWeight weight = bold ? FontWeight.Bold : FontWeight.Normal;
        FontSlant slant = italic ? FontSlant.Italic : FontSlant.Upright;
        const FontSpacing width = FontSpacing.Normal;
        return GetTypeface(fontName, weight, slant, width);
    }

    /// <summary>Gets a typeface for the requested font name, style, and spacing.</summary>
    /// <param name="fontName">The font family name.</param>
    /// <param name="bold">Whether to use bold weight.</param>
    /// <param name="italic">Whether to use italic slant.</param>
    /// <param name="width">The requested font spacing.</param>
    /// <returns>The matching typeface, or the default typeface when no resolver matches.</returns>
    public static SKTypeface GetTypeface(string fontName, bool bold, bool italic, FontSpacing width)
    {
        FontWeight weight = bold ? FontWeight.Bold : FontWeight.Normal;
        FontSlant slant = italic ? FontSlant.Italic : FontSlant.Upright;
        return GetTypeface(fontName, weight, slant, width);
    }

    /// <summary>Finds an installed system font that best supports the characters in the text.</summary>
    /// <param name="text">The text whose characters should be supported.</param>
    /// <returns>The best matching font family, or the default font when no ideal match is found.</returns>
    public static string Detect(string text)
    {
        // TODO: Use the CrissCross.Plot default font. Maybe give as an input parameter?
        // TODO: Replace unrenderable characters with "�"
        //         Should replace characters consisting of multiple code points, as well as missing glyphs.
        // We ignore Unicode characters that are made up of multiple code points since it looks like they
        // cannot be rendered without more work, e.g. by using HarfBuzz or similar at higher levels.
        // The function breaks down the input text string into "text elements" (=grapheme clusters).
        // A text element can consist of multiple Unicode code points.
        // Each code point can consist of 1 or 2 C# 'char's (=UTF-16 code units).
        if (string.IsNullOrWhiteSpace(text))
        {
            return Fonts.Default;
        }

        string defaultFontFamily = GetDefaultFontFamily(); // Should use the CrissCross.Plot default font instead
        List<int> standaloneCodePoints = GetStandaloneCodePoints(text);

        List<string> candidateFontNames = GetCandidateFontsForString(standaloneCodePoints);

        if (candidateFontNames.Count == 0)
        {
            return string.Empty; // TODO: Signal an error somehow? Could return default font name.
        }

        // We prefer the default font if it can render the string without missing glyphs.
        if (candidateFontNames.Contains(defaultFontFamily)
            && CountMissingGlyphs(defaultFontFamily, standaloneCodePoints) == 0)
        {
            return defaultFontFamily;
        }

        string bestFontName = candidateFontNames[0];
        int fewestMissingGlyphs = CountMissingGlyphs(bestFontName, standaloneCodePoints);
        for (int i = 1; i < candidateFontNames.Count; i++)
        {
            string candidateFontName = candidateFontNames[i];
            int missingGlyphCount = CountMissingGlyphs(candidateFontName, standaloneCodePoints);
            if (missingGlyphCount < fewestMissingGlyphs)
            {
                bestFontName = candidateFontName;
                fewestMissingGlyphs = missingGlyphCount;
            }
        }

        return bestFontName;
    }

    /// <summary>Gets the family name of the system's default typeface.</summary>
    /// <returns>The system default font family, or an empty string when unavailable.</returns>
    public static string GetDefaultFontFamily()
    {
        using SKTypeface typeface = SKFontManager.Default.MatchCharacter(' ');

        return typeface is not null ? typeface.FamilyName : string.Empty;
    }

    /// <summary>Gets installed font families that contain at least one requested code point.</summary>
    /// <param name="standaloneCodePoints">The Unicode code points to match.</param>
    /// <returns>The distinct candidate font families.</returns>
    public static List<string> GetCandidateFontsForString(List<int> standaloneCodePoints)
    {
        HashSet<string> candidateFontNames = []; // Using HashSet to avoid duplicates

        foreach (int standaloneCodePoint in standaloneCodePoints)
        {
            using SKTypeface? typeface = SKFontManager.Default.MatchCharacter(standaloneCodePoint);
            if (typeface is not null)
            {
                _ = candidateFontNames.Add(typeface.FamilyName);
            }

            _ = char.ConvertFromUtf32(standaloneCodePoint);
        }

        return [.. candidateFontNames];
    }

    /// <summary>Counts code points that are missing from the specified font.</summary>
    /// <param name="fontName">The font family to inspect.</param>
    /// <param name="standaloneCodePoints">The Unicode code points to test.</param>
    /// <returns>The number of missing glyphs, or <see cref="int.MaxValue"/> when the typeface is unavailable.</returns>
    public static int CountMissingGlyphs(string fontName, List<int> standaloneCodePoints)
    {
        int missingGlyphCount = 0;

        using var typeface = SKTypeface.FromFamilyName(fontName);
        if (typeface is not null)
        {
            using SKFont font = new(typeface);
            foreach (int standaloneCodePoint in standaloneCodePoints)
            {
                if (!font.ContainsGlyph(standaloneCodePoint))
                {
                    missingGlyphCount++;
                }
            }

            StringBuilder text = new();
            foreach (int codePoint in standaloneCodePoints)
            {
                _ = text.Append(char.ConvertFromUtf32(codePoint));
            }

            Debug.WriteLine($"Input '{text}': Font {fontName} has {missingGlyphCount} items with missing glyphs");
        }
        else
        {
            missingGlyphCount = int.MaxValue;
        }

        return missingGlyphCount;
    }

    /// <summary>Gets standalone Unicode code points from text.</summary>
    /// <param name="inputText">The text to inspect.</param>
    /// <returns>The code points that occur as standalone text elements.</returns>
    public static List<int> GetStandaloneCodePoints(string inputText)
    {
        List<string> textElements = ConvertStringToTextElements(inputText);
        List<List<int>> codePoints = [];
        foreach (string textElement in textElements)
        {
            codePoints.Add(ConvertTextElementToUtf32CodePoints(textElement));
        }

        return GetStandaloneCodePoints(codePoints);
    }

    /// <summary>Filters code point lists to include only standalone text elements.</summary>
    /// <param name="codePointLists">The code points for each text element.</param>
    /// <returns>The single code points from text elements containing exactly one code point.</returns>
    public static List<int> GetStandaloneCodePoints(IEnumerable<List<int>> codePointLists)
    {
        // Take a list of code point lists as input.
        // Each code point list must correspond to a single text element (grapheme cluster).
        // Filter the list to retain the code point lists made up of single code points,
        // i.e. remove combining characters, etc.
        // Return as a flattened list of the remaining standalone code points.
        List<int> codePoints = [];

        foreach (List<int> codePointList in codePointLists)
        {
            if (codePointList.Count == 1)
            {
                codePoints.Add(codePointList[0]);
            }
        }

        return codePoints;
    }

    /// <summary>Splits a string into Unicode text elements.</summary>
    /// <param name="textString">The string to split.</param>
    /// <returns>The text elements in their original order.</returns>
    public static List<string> ConvertStringToTextElements(string textString)
    {
        List<string> resultList = [];

        TextElementEnumerator chars = StringInfo.GetTextElementEnumerator(textString);
        while (chars.MoveNext())
        {
            string textElement = chars.GetTextElement();

            resultList.Add(textElement);
        }

        return resultList;
    }

    /// <summary>Converts a text element (grapheme cluster) into its Unicode code points.</summary>
    /// <param name="textElement">The text element to convert.</param>
    /// <returns>The UTF-32 code points represented by the text element.</returns>
    public static List<int> ConvertTextElementToUtf32CodePoints(string textElement)
    {
        const int SurrogatePairCodeUnitLength = 2;
        List<int> resultList = [];

        int i = 0;
        while (i < textElement.Length)
        {
            // ArgumentOutOfRangeException is possible for malformed input strings,
            // we let that propagate up to the user
            int codePoint = char.ConvertToUtf32(textElement, i);
            resultList.Add(codePoint);

            // Did we consume one or two chars?
            if (char.IsHighSurrogate(textElement, i))
            {
                i += SurrogatePairCodeUnitLength;
            }
            else
            {
                i++;
            }
        }

        return resultList;
    }

    /// <summary>Resets the default font styles and family names to system defaults.</summary>
    public static void Reset()
    {
        DefaultWeight = null;
        DefaultSlant = null;
        DefaultWidth = null;
        DefaultFontStyle = null;

        Default = SystemFontFamilyResolver.InstalledSansFont();
        Sans = SystemFontFamilyResolver.InstalledSansFont();
        Serif = SystemFontFamilyResolver.InstalledSerifFont();
        Monospace = SystemFontFamilyResolver.InstalledMonospaceFont();
    }
}
