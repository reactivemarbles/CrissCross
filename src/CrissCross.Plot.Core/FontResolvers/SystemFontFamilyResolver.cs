// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.FontResolvers;

/// <summary>Provides installed system font family names for the font facade and resolver.</summary>
internal static class SystemFontFamilyResolver
{
    /// <summary>Gets the installed sans-serif font family name.</summary>
    /// <returns>The preferred installed sans-serif font.</returns>
    internal static string InstalledSansFont()
    {
        // Prefer the system default because it is probably the best for international users
        string font = SKTypeface.Default.FamilyName;

        // Favor "Open Sans" over "Segoe UI" because better anti-aliasing
        var installedFonts = GetInstalledFonts();
        if (font == "Segoe UI" && installedFonts.Contains("Open Sans"))
        {
            font = "Open Sans";
        }

        return font;
    }

    /// <summary>Gets the installed monospace font family name.</summary>
    /// <returns>The preferred installed monospace font.</returns>
    internal static string InstalledMonospaceFont()
    {
        var installedFonts = GetInstalledFonts();

        foreach (string preferredFont in (string[])["Roboto Mono", "Consolas", "DejaVu Sans Mono", "Courier"])
        {
            if (installedFonts.Contains(preferredFont))
            {
                return SKTypeface.FromFamilyName(preferredFont).FamilyName;
            }
        }

        return SKTypeface.Default.FamilyName;
    }

    /// <summary>Gets the installed serif font family name.</summary>
    /// <returns>The preferred installed serif font.</returns>
    internal static string InstalledSerifFont()
    {
        var installedFonts = GetInstalledFonts();

        foreach (string preferredFont in (string[])["Times New Roman", "DejaVu Serif", "Times"])
        {
            if (installedFonts.Contains(preferredFont))
            {
                return SKTypeface.FromFamilyName(preferredFont).FamilyName;
            }
        }

        return SKTypeface.Default.FamilyName;
    }

    /// <summary>Determines whether the specified font family is installed.</summary>
    /// <param name="fontName">The font family name to look up.</param>
    /// <returns><see langword="true"/> when the font family is installed.</returns>
    internal static bool IsInstalled(string fontName) => GetInstalledFonts().Contains(fontName);

    /// <summary>Gets the installed font family names for case-insensitive lookup.</summary>
    /// <returns>The installed font family names.</returns>
    private static HashSet<string> GetInstalledFonts() => new(SKFontManager.Default.FontFamilies, StringComparer.InvariantCultureIgnoreCase);
}
