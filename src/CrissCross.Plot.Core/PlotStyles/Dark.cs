// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.PlotStyles;

/// <summary>A dark plot style with a dark background and light foreground.</summary>
public class Dark : PlotStyle
{
    /// <summary>The foreground color used by axes and legend text.</summary>
    private const string LightForegroundHexColor = "#d7d7d7";

    /// <summary>Initializes a new instance of the <see cref="Dark"/> class.</summary>
    public Dark()
    {
        Palette = new Palettes.Penumbra();
        AxisColor = Color.FromHex(LightForegroundHexColor);
        GridMajorLineColor = Color.FromHex("#404040");
        FigureBackgroundColor = Color.FromHex("#181818");
        DataBackgroundColor = Color.FromHex("#1f1f1f");
        LegendBackgroundColor = Color.FromHex("#404040");
        LegendFontColor = Color.FromHex(LightForegroundHexColor);
        LegendOutlineColor = Color.FromHex(LightForegroundHexColor);
    }
}
