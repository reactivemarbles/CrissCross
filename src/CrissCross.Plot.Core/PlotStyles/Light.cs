// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.PlotStyles;

/// <summary>A light plot style with a light background and dark foreground.</summary>
public class Light : PlotStyle
{
    /// <summary>The opacity used for major grid lines.</summary>
    private const float MajorGridOpacity = 0.1F;

    /// <summary>Initializes a new instance of the <see cref="Light"/> class.</summary>
    public Light()
    {
        Palette = new Palettes.Category10();
        AxisColor = Colors.Black;
        GridMajorLineColor = Colors.Black.WithOpacity(MajorGridOpacity);
        FigureBackgroundColor = Colors.White;
        DataBackgroundColor = Colors.White;
        LegendBackgroundColor = Colors.White;
        LegendFontColor = Colors.Black;
        LegendOutlineColor = Colors.Black;
    }
}
