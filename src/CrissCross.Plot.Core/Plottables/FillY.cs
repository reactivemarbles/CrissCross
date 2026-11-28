// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Fills the area between two scatter series.</summary>
public class FillY : IPlottable, IHasLine, IHasFill, IHasMarker, IHasLegendText
{
    /// <summary>Initializes a new instance of the <see cref="FillY"/> class.</summary>
    public FillY()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="FillY"/> class from two scatter series.</summary>
    /// <param name="scatter1">The first scatter series forming one edge of the filled area.</param>
    /// <param name="scatter2">The second scatter series forming the other edge of the filled area.</param>
    public FillY(Scatter scatter1, Scatter scatter2)
    {
        IReadOnlyList<Coordinates> data1 = scatter1.Data.GetScatterPoints();
        IReadOnlyList<Coordinates> data2 = scatter2.Data.GetScatterPoints();
        Coordinates[] data = new Coordinates[data1.Count + data2.Count];

        for (int i = 0; i < data1.Count; i++)
        {
            data[i] = data1[i];
        }

        for (int i = 0; i < data2.Count; i++)
        {
            data[data1.Count + i] = data2[data2.Count - 1 - i];
        }

        Poly = new(data);
    }

    /// <summary>Gets or sets the legacy legend label.</summary>
    [Obsolete("use LegendText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete alias for source compatibility.")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text shown for this filled area in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets whether this filled area is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to render and measure this filled area.</summary>
    public IAxes Axes { get => Poly.Axes; set => Poly.Axes = value; }

    /// <summary>Gets the legend items for this filled area.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.Single(this, LegendText, FillStyle, LineStyle);

    /// <summary>Gets or sets the fill style.</summary>
    public FillStyle FillStyle { get => Poly.FillStyle; set => Poly.FillStyle = value; }

    /// <summary>Gets or sets the fill color.</summary>
    public Color FillColor { get => FillStyle.Color; set => FillStyle.Color = value; }

    /// <summary>Gets or sets the fill hatch color.</summary>
    public Color FillHatchColor { get => FillStyle.HatchColor; set => FillStyle.HatchColor = value; }

    /// <summary>Gets or sets the fill hatch.</summary>
    public IHatch? FillHatch { get => FillStyle.Hatch; set => FillStyle.Hatch = value; }

    /// <summary>Gets or sets the outline line style.</summary>
    public LineStyle LineStyle { get => Poly.LineStyle; set => Poly.LineStyle = value; }

    /// <summary>Gets or sets the outline width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the outline pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the outline color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the marker style for polygon vertices.</summary>
    public MarkerStyle MarkerStyle { get => Poly.MarkerStyle; set => Poly.MarkerStyle = value; }

    /// <summary>Gets or sets the marker shape.</summary>
    public MarkerShape MarkerShape { get => MarkerStyle.Shape; set => MarkerStyle.Shape = value; }

    /// <summary>Gets or sets the marker size.</summary>
    public float MarkerSize { get => MarkerStyle.Size; set => MarkerStyle.Size = value; }

    /// <summary>Gets or sets the marker fill color.</summary>
    public Color MarkerFillColor { get => MarkerStyle.FillColor; set => MarkerStyle.FillColor = value; }

    /// <summary>Gets or sets the marker line color.</summary>
    public Color MarkerLineColor { get => MarkerStyle.LineColor; set => MarkerStyle.LineColor = value; }

    /// <summary>Gets or sets the marker color.</summary>
    public Color MarkerColor { get => MarkerStyle.MarkerColor; set => MarkerStyle.MarkerColor = value; }

    /// <summary>Gets or sets the marker line width.</summary>
    public float MarkerLineWidth { get => MarkerStyle.LineWidth; set => MarkerStyle.LineWidth = value; }

    /// <summary>Gets the polygon used to render the filled area.</summary>
    private Polygon Poly { get; } = Polygon.Empty;

    /// <summary>Sets the data points used to create the filled area.</summary>
    /// <param name="items">The x-coordinate and upper/lower values for each point.</param>
    public void SetDataSource(ICollection<(double X, double Top, double Bottom)> items)
    {
        Coordinates[] all = new Coordinates[items.Count * Constants.CoordinatesPerItem];
        int i = 0;
        foreach (var item in items)
        {
            Coordinates top = new(item.X, item.Top);
            all[i] = new(item.X, item.Bottom);
            all[items.Count + (items.Count - 1 - i)] = top;
            i++;
        }

        Poly.UpdateCoordinates(all);
    }

    /// <summary>Sets the data points by projecting each item to x, top, and bottom values.</summary>
    /// <typeparam name="T">The type of input item.</typeparam>
    /// <param name="items">The items to project.</param>
    /// <param name="coordinateSolver">The function that extracts x, top, and bottom values.</param>
    public void SetDataSource<T>(ICollection<T> items, Func<T, (double X, double Top, double Bottom)> coordinateSolver)
    {
        Coordinates[] all = new Coordinates[items.Count * Constants.CoordinatesPerItem];
        int i = 0;
        foreach (var item in items)
        {
            var coords = coordinateSolver(item);
            Coordinates top = new(coords.X, coords.Top);
            all[i] = new(coords.X, coords.Bottom);
            all[items.Count + (items.Count - 1 - i)] = top;
            i++;
        }

        Poly.UpdateCoordinates(all);
    }

    /// <summary>Gets the axis limits occupied by the filled area.</summary>
    /// <returns>The axis limits of the underlying polygon.</returns>
    public AxisLimits GetAxisLimits() => Poly is null ? AxisLimits.NoLimits : Poly.GetAxisLimits();

    /// <summary>Renders the filled area.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp) => Poly.Render(rp);

    /// <summary>Stores constants used by the fill polygon builder.</summary>
    private static class Constants
    {
        /// <summary>The number of polygon coordinates generated per item.</summary>
        public const int CoordinatesPerItem = 2;
    }
}
