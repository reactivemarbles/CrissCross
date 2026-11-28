// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>
/// This object holds many common plot style customizations
/// and facilitates switching between styles or copying styles
/// from one plot to another.
/// </summary>
public class PlotStyle
{
    /// <summary>Gets or sets the plot palette.</summary>
    public IPalette? Palette { get; set; }

    /// <summary>Gets or sets the figure background color.</summary>
    public Color? FigureBackgroundColor { get; set; }

    /// <summary>Gets or sets the data-area background color.</summary>
    public Color? DataBackgroundColor { get; set; }

    /// <summary>Gets or sets the axis color.</summary>
    public Color? AxisColor { get; set; }

    /// <summary>Gets or sets the major grid line color.</summary>
    public Color? GridMajorLineColor { get; set; }

    /// <summary>Gets or sets the legend background color.</summary>
    public Color? LegendBackgroundColor { get; set; }

    /// <summary>Gets or sets the legend font color.</summary>
    public Color? LegendFontColor { get; set; }

    /// <summary>Gets or sets the legend outline color.</summary>
    public Color? LegendOutlineColor { get; set; }

    /// <summary>Creates a style containing the settings represented by the given plot.</summary>
    /// <param name="plot">The plot whose style settings are read.</param>
    /// <returns>A style populated from the plot.</returns>
    public static PlotStyle FromPlot(Plot plot)
    {
        Color axisColor;
        using (var axes = plot.Axes.GetAxes().GetEnumerator())
        {
            if (!axes.MoveNext())
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            axisColor = axes.Current.FrameLineStyle.Color;
        }

        return new PlotStyle
        {
            // plot
            Palette = plot.Add.Palette,
            FigureBackgroundColor = plot.FigureBackground.Color,
            DataBackgroundColor = plot.DataBackground.Color,

            // axes and grids
            AxisColor = axisColor,
            GridMajorLineColor = plot.Grid.MajorLineColor,

            // legend
            LegendBackgroundColor = plot.Legend.BackgroundColor,
            LegendFontColor = plot.Legend.FontColor,
            LegendOutlineColor = plot.Legend.OutlineColor,
        };
    }

    /// <summary>Applies these style settings to the given plot.</summary>
    /// <param name="plot">The plot to update.</param>
    public void Apply(Plot plot)
    {
        // plot
        if (FigureBackgroundColor.HasValue)
        {
            plot.FigureBackground.Color = FigureBackgroundColor.Value;
        }

        if (DataBackgroundColor.HasValue)
        {
            plot.DataBackground.Color = DataBackgroundColor.Value;
        }

        if (Palette is not null)
        {
            plot.Add.Palette = Palette;
        }

        // axes and grids
        if (AxisColor.HasValue)
        {
            plot.Axes.Color(AxisColor.Value);
        }

        if (GridMajorLineColor.HasValue)
        {
            plot.Grid.MajorLineColor = GridMajorLineColor.Value;
        }

        // legend
        if (LegendBackgroundColor.HasValue)
        {
            plot.Legend.BackgroundColor = LegendBackgroundColor.Value;
        }

        if (LegendFontColor.HasValue)
        {
            plot.Legend.FontColor = LegendFontColor.Value;
        }

        if (!LegendOutlineColor.HasValue)
        {
            return;
        }

        plot.Legend.OutlineColor = LegendOutlineColor.Value;
    }

    /// <summary>Returns a style with values cleared when they match the given style.</summary>
    /// <param name="other">The style to compare with this style.</param>
    /// <returns>A style containing only values that differ.</returns>
    public PlotStyle WhereDifferentFrom(PlotStyle other) => new()
        {
            // TODO: can this be done with reflection?
            // plot
            Palette = Palette != other.Palette ? Palette : null,
            FigureBackgroundColor = FigureBackgroundColor != other.FigureBackgroundColor ? FigureBackgroundColor : null,
            DataBackgroundColor = DataBackgroundColor != other.DataBackgroundColor ? DataBackgroundColor : null,

            // axes and grids
            AxisColor = AxisColor != other.AxisColor ? AxisColor : null,
            GridMajorLineColor = GridMajorLineColor != other.GridMajorLineColor ? GridMajorLineColor : null,

            // legend
            LegendBackgroundColor = LegendBackgroundColor != other.LegendBackgroundColor ? LegendBackgroundColor : null,
            LegendFontColor = LegendFontColor != other.LegendFontColor ? LegendFontColor : null,
            LegendOutlineColor = LegendOutlineColor != other.LegendOutlineColor ? LegendOutlineColor : null,
        };
}
