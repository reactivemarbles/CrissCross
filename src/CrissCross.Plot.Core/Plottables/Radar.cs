// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders radar series as polygons over a polar axis.</summary>
public class Radar() : IPlottable, IManagesAxisLimits
{
    /// <summary>Gets or sets the polar axis drawn with the radar series.</summary>
    public PolarAxis PolarAxis { get; set; } = new() { Rotation = Angle.FromDegrees(Constants.AxisRotationDegrees), Clockwise = true };

    /// <summary>Gets the radar series and their rendering styles.</summary>
    public List<RadarSeries> Series { get; } = [];

    /// <summary>Gets or sets whether axis limits are adjusted to keep circles round.</summary>
    public bool ManageAxisLimits { get => PolarAxis.ManageAxisLimits; set => PolarAxis.ManageAxisLimits = value; }

    /// <summary>Gets or sets whether this plot is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets whether the polar axis is drawn above the series.</summary>
    public bool IsAxisAboveData { get; set; }

    /// <summary>Gets or sets the axes used to transform coordinates.</summary>
    public IAxes Axes
    {
        get => PolarAxis.Axes;
        set => PolarAxis.Axes = value;
    }

    /// <summary>Gets the legend items for the radar series.</summary>
    public IEnumerable<LegendItem> LegendItems
    {
        get
        {
            List<LegendItem> items = [with(capacity: Series.Count)];
            foreach (RadarSeries series in Series)
            {
                items.Add(new LegendItem { LabelText = series.LegendText, FillStyle = series.FillStyle, LineStyle = series.LineStyle });
            }

            return items;
        }
    }

    /// <summary>Updates the plot axes to maintain a square polar plotting area.</summary>
    /// <param name="plot">The plot whose axis rules are updated.</param>
    public virtual void UpdateAxisLimits(Plot plot) => PolarAxis.UpdateAxisLimits(plot);

    /// <summary>Gets the axis limits of the polar axis.</summary>
    /// <returns>The axis limits of the polar axis.</returns>
    public AxisLimits GetAxisLimits() => PolarAxis.GetAxisLimits();

    /// <summary>Renders the radar series and polar axis.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible || Series.Count == 0)
        {
            return;
        }

        if (!IsAxisAboveData)
        {
            PolarAxis.Render(rp);
        }

        for (int i = 0; i < Series.Count; i++)
        {
            Coordinates[] coordinates = PolarAxis.GetCoordinates(Series[i].Values);
            Pixel[] pixels = new Pixel[coordinates.Length];
            for (int j = 0; j < coordinates.Length; j++)
            {
                pixels[j] = Axes.GetPixel(coordinates[j]);
            }

            Drawing.FillPath(rp.Canvas, rp.Paint, pixels, Series[i].FillStyle);
            Drawing.DrawPath(rp.Canvas, rp.Paint, pixels, Series[i].LineStyle, close: true);
        }

        if (!IsAxisAboveData)
        {
            return;
        }

        PolarAxis.Render(rp);
    }

    /// <summary>Contains constants used by radar plots.</summary>
    internal static class Constants
    {
        /// <summary>The starting axis angle in degrees.</summary>
        internal const double AxisRotationDegrees = -90;
    }
}
