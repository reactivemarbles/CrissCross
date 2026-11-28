// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Stylers;

/// <summary>Helper methods for setting fonts to all components of a plot.</summary>
/// <param name="plot">The plot whose fonts are managed.</param>
public class FontStyler(Plot plot)
{
    /// <summary>The plot whose fonts are managed.</summary>
    private readonly Plot _plot = plot;

    /// <summary>Applies a font to common plot components and sets the default for future plot objects.</summary>
    /// <param name="fontName">The requested font family.</param>
    /// <param name="weight">The requested font weight.</param>
    /// <param name="slant">The requested font slant.</param>
    /// <param name="spacing">The requested font spacing.</param>
    /// <returns>The resolved typeface, or <see langword="null"/> if it cannot be located.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "The font-style defaults are part of the existing public API.")]
    public SKTypeface? Set(
        string fontName,
        FontWeight weight = FontWeight.Normal,
        FontSlant slant = FontSlant.Upright,
        FontSpacing spacing = FontSpacing.Normal)
    {
        // do nothing if the font can't be located.
        SKTypeface? typeFace = Fonts.GetTypeface(fontName, weight, slant, spacing);
        if (typeFace is null)
        {
            return null;
        }

        // set default font so future added objects will use it
        Fonts.Default = fontName;
        Fonts.DefaultFontStyle = typeFace;

        // title
        _plot.Axes.Title.Label.FontName = fontName;
        _plot.Axes.Title.Label.Font = typeFace;

        // axis labels and ticks
        foreach (IAxis axis in _plot.Axes.GetAxes())
        {
            axis.Label.FontName = fontName;
            axis.Label.Font = typeFace;
            axis.TickLabelStyle.FontName = fontName;
            axis.TickLabelStyle.Font = typeFace;
        }

        // controls newly instantated LabelStyle objects
        Fonts.DefaultFontStyle = typeFace;

        return typeFace;
    }

    /// <summary>
    /// Detects the best font to apply to every label in the plot based on the characters they contain.
    /// If the best font for a label cannot be detected, the most recently configured font will be used.
    /// </summary>
    public void Automatic()
    {
        // title
        _plot.Axes.Title.Label.SetBestFont();

        // axis labels and ticks
        foreach (IAxis axis in _plot.Axes.GetAxes())
        {
            axis.Label.SetBestFont();
            axis.TickLabelStyle.SetBestFont();
        }

        _plot.Legend.SetBestFontOnEachRender = true;
    }
}
