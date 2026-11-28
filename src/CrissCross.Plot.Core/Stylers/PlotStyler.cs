// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Stylers;

/// <summary>Retains the deprecated styling API for binary compatibility.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The deprecated type remains part of the public compatibility surface.")]
[Obsolete("This class is deprecated. See the ScottPlot Cookbook for styling information.", true)]
public class PlotStyler()
{
    /// <summary>Deprecated; set the figure and data backgrounds directly.</summary>
    /// <param name="figure">The figure background color.</param>
    /// <param name="data">The data background color.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete method remains part of the public compatibility surface.")]
    [Obsolete("This method is deprecated. Assign Plot.FigureBackground.Color instead.", true)]
    public void Background(Color figure, Color data) { }

    /// <summary>Deprecated; set axis colors directly.</summary>
    /// <param name="color">The axis color.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete method remains part of the public compatibility surface.")]
    [Obsolete("This method is deprecated. Call Plot.Axes.Color() instead.", true)]
    public void ColorAxes(Color color) { }

    /// <summary>Deprecated; set legend properties directly.</summary>
    /// <param name="background">The legend background color.</param>
    /// <param name="foreground">The legend foreground color.</param>
    /// <param name="border">The legend border color.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete method remains part of the public compatibility surface.")]
    [Obsolete("Reference Plot.Legend properties directly.", true)]
    public void ColorLegend(Color background, Color foreground, Color border) { }

    /// <summary>Deprecated; configure axis frames directly.</summary>
    /// <param name="left">The left-axis frame setting.</param>
    /// <param name="right">The right-axis frame setting.</param>
    /// <param name="bottom">The bottom-axis frame setting.</param>
    /// <param name="top">The top-axis frame setting.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete method remains part of the public compatibility surface.")]
    [Obsolete("This method is deprecated. Call Plot.Axes.Frame() methods instead.", true)]
    public void AxisFrame(float left, float right, float bottom, float top) { }

    /// <summary>Deprecated; set the plot font directly.</summary>
    /// <param name="fontName">The font family name.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete method remains part of the public compatibility surface.")]
    [Obsolete("This method is deprecated. Call Plot.Font.Set() instead.", true)]
    public void SetFont(string fontName) { }

    /// <summary>Deprecated; configure automatic font selection directly.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete method remains part of the public compatibility surface.")]
    [Obsolete("This method is deprecated. Call Plot.Font.Automatic() instead.", true)]
    public void SetBestFonts() { }

    /// <summary>Deprecated; configure the plot style directly.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete method remains part of the public compatibility surface.")]
    [Obsolete("This method is deprecated. Call Plot.SetStyle() instead.", true)]
    public void DarkMode() { }
}
