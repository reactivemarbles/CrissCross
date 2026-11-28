// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Describes a plottable entry and its visual styles in a legend.</summary>
public class LegendItem : LabelStyleProperties, IHasMarker, IHasLine, IHasFill, IHasArrow
{
    /// <summary>Gets an empty sequence of legend items.</summary>
    public static IEnumerable<LegendItem> None => [];

    /// <summary>Gets or sets whether this legend item is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the plottable associated with this legend item.</summary>
    public IPlottable? Plottable { get; set; }

    /// <summary>Gets or sets the position of this item among explicitly ordered legend items.</summary>
    public int? Index { get; set; }

    /// <summary>Gets or sets the label style used to render this legend item.</summary>
    public sealed override LabelStyle LabelStyle { get; set; } = new() { Alignment = Alignment.MiddleLeft };

    /// <summary>Gets or sets the line style displayed by this legend item.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 0 };

    /// <summary>Gets or sets the width of the displayed line.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the pattern of the displayed line.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the color of the displayed line.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the fill style displayed by this legend item.</summary>
    public FillStyle FillStyle { get; set; } = new();

    /// <summary>Gets or sets the color used to fill the legend item.</summary>
    public Color FillColor { get => FillStyle.Color; set => FillStyle.Color = value; }

    /// <summary>Gets or sets the hatch color used to fill the legend item.</summary>
    public Color FillHatchColor { get => FillStyle.HatchColor; set => FillStyle.HatchColor = value; }

    /// <summary>Gets or sets the hatch pattern used to fill the legend item.</summary>
    public IHatch? FillHatch { get => FillStyle.Hatch; set => FillStyle.Hatch = value; }

    /// <summary>Gets or sets the outline line style.</summary>
    public LineStyle OutlineStyle { get; set; } = new() { Width = 0 };

    /// <summary>Gets or sets the outline width.</summary>
    public float OutlineWidth { get => OutlineStyle.Width; set => OutlineStyle.Width = value; }

    /// <summary>Gets or sets the outline pattern.</summary>
    public LinePattern OutlinePattern { get => OutlineStyle.Pattern; set => OutlineStyle.Pattern = value; }

    /// <summary>Gets or sets the outline color.</summary>
    public Color OutlineColor { get => OutlineStyle.Color; set => OutlineStyle.Color = value; }

    /// <summary>Gets or sets the marker style displayed by this legend item.</summary>
    public MarkerStyle MarkerStyle { get; set; } = new();

    /// <summary>Gets or sets the marker shape.</summary>
    public MarkerShape MarkerShape { get => MarkerStyle.Shape; set => MarkerStyle.Shape = value; }

    /// <summary>Gets or sets the marker size.</summary>
    public float MarkerSize { get => MarkerStyle.Size; set => MarkerStyle.Size = value; }

    /// <summary>Gets or sets the marker fill color.</summary>
    public Color MarkerFillColor { get => MarkerStyle.FillColor; set => MarkerStyle.FillColor = value; }

    /// <summary>Gets or sets the marker line color.</summary>
    public Color MarkerLineColor { get => MarkerStyle.LineColor; set => MarkerStyle.LineColor = value; }

    /// <summary>Gets or sets both the marker fill and line colors.</summary>
    public Color MarkerColor
    {
        get => MarkerFillColor;
        set
        {
            MarkerFillColor = value;
            MarkerLineColor = value;
        }
    }

    /// <summary>Gets or sets the marker line width.</summary>
    public float MarkerLineWidth { get => MarkerStyle.LineWidth; set => MarkerStyle.LineWidth = value; }

    /// <summary>Gets or sets the arrow style displayed by this legend item.</summary>
    public ArrowStyle ArrowStyle { get; set; } = new();

    /// <summary>Gets or sets the arrow line color.</summary>
    public Color ArrowLineColor { get => ArrowStyle.LineStyle.Color; set => ArrowStyle.LineStyle.Color = value; }

    /// <summary>Gets or sets the arrow line width.</summary>
    public float ArrowLineWidth { get => ArrowStyle.LineStyle.Width; set => ArrowStyle.LineStyle.Width = value; }

    /// <summary>Gets or sets the arrow fill color.</summary>
    public Color ArrowFillColor { get => ArrowStyle.FillStyle.Color; set => ArrowStyle.FillStyle.Color = value; }

    /// <summary>Gets or sets the minimum arrow length.</summary>
    public float ArrowMinimumLength { get => ArrowStyle.MinimumLength; set => ArrowStyle.MinimumLength = value; }

    /// <summary>Gets or sets the maximum arrow length.</summary>
    public float ArrowMaximumLength { get => ArrowStyle.MaximumLength; set => ArrowStyle.MaximumLength = value; }

    /// <summary>Gets or sets the arrow offset.</summary>
    public float ArrowOffset { get => ArrowStyle.Offset; set => ArrowStyle.Offset = value; }

    /// <summary>Gets or sets the anchor point for the arrow.</summary>
    public ArrowAnchor ArrowAnchor { get => ArrowStyle.Anchor; set => ArrowStyle.Anchor = value; }

    /// <summary>Gets or sets the arrow width.</summary>
    public float ArrowWidth { get => ArrowStyle.ArrowWidth; set => ArrowStyle.ArrowWidth = value; }

    /// <summary>Gets or sets the arrowhead axis length.</summary>
    public float ArrowheadAxisLength { get => ArrowStyle.ArrowheadAxisLength; set => ArrowStyle.ArrowheadAxisLength = value; }

    /// <summary>Gets or sets the arrowhead length.</summary>
    public float ArrowheadLength { get => ArrowStyle.ArrowheadLength; set => ArrowStyle.ArrowheadLength = value; }

    /// <summary>Gets or sets the arrowhead width.</summary>
    public float ArrowheadWidth { get => ArrowStyle.ArrowheadWidth; set => ArrowStyle.ArrowheadWidth = value; }

    /// <summary>Gets or sets the legacy line style.</summary>
    [Obsolete("use LineStyle")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310:Deprecated public API", Justification = "Retained to preserve the established public API.")]
    public LineStyle Line { get => LineStyle; set => LineStyle = value; }

    /// <summary>Gets or sets the legacy marker style.</summary>
    [Obsolete("use MarkerStyle")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310:Deprecated public API", Justification = "Retained to preserve the established public API.")]
    public MarkerStyle Marker { get => MarkerStyle; set => MarkerStyle = value; }

    /// <summary>Gets or sets the legacy label text.</summary>
    [Obsolete("use LabelText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310:Deprecated public API", Justification = "Retained to preserve the established public API.")]
    public string Label { get => LabelText; set => LabelText = value; }

    /// <summary>Creates a sequence containing one legend item.</summary>
    /// <param name="item">The item to include.</param>
    /// <returns>A sequence containing the specified item.</returns>
    public static IEnumerable<LegendItem> Single(LegendItem item) => [item];

    /// <summary>Creates a legend item with the specified marker style.</summary>
    /// <param name="plottable">The associated plottable.</param>
    /// <param name="label">The legend label.</param>
    /// <param name="markerStyle">The marker style to display.</param>
    /// <returns>A sequence containing the created item.</returns>
    public static IEnumerable<LegendItem> Single(IPlottable plottable, string label, MarkerStyle markerStyle)
    {
        LegendItem item = new() { Plottable = plottable, LabelText = label, MarkerStyle = markerStyle, LineStyle = LineStyle.None, };

        return Single(item);
    }

    /// <summary>Creates a legend item with the specified marker and line styles.</summary>
    /// <param name="plottable">The associated plottable.</param>
    /// <param name="label">The legend label.</param>
    /// <param name="markerStyle">The marker style to display.</param>
    /// <param name="lineStyle">The line style to display.</param>
    /// <returns>A sequence containing the created item.</returns>
    public static IEnumerable<LegendItem> Single(IPlottable plottable, string label, MarkerStyle markerStyle, LineStyle lineStyle)
    {
        LegendItem item = new() { Plottable = plottable, LabelText = label, MarkerStyle = markerStyle, LineStyle = lineStyle, };

        return Single(item);
    }

    /// <summary>Creates a legend item with the specified line style.</summary>
    /// <param name="plottable">The associated plottable.</param>
    /// <param name="label">The legend label.</param>
    /// <param name="lineStyle">The line style to display.</param>
    /// <returns>A sequence containing the created item.</returns>
    public static IEnumerable<LegendItem> Single(IPlottable plottable, string label, LineStyle lineStyle)
    {
        LegendItem item = new() { Plottable = plottable, LabelText = label, MarkerStyle = MarkerStyle.None, LineStyle = lineStyle, };

        return Single(item);
    }

    /// <summary>Creates a legend item with the specified fill style.</summary>
    /// <param name="plottable">The associated plottable.</param>
    /// <param name="label">The legend label.</param>
    /// <param name="fillStyle">The fill style to display.</param>
    /// <returns>A sequence containing the created item.</returns>
    public static IEnumerable<LegendItem> Single(IPlottable plottable, string label, FillStyle fillStyle)
    {
        LegendItem item = new() { Plottable = plottable, LabelText = label, MarkerStyle = MarkerStyle.None, FillStyle = fillStyle, LineStyle = LineStyle.None, };

        return Single(item);
    }

    /// <summary>Creates a legend item with the specified fill and line styles.</summary>
    /// <param name="plottable">The associated plottable.</param>
    /// <param name="label">The legend label.</param>
    /// <param name="fillStyle">The fill style to display.</param>
    /// <param name="lineStyle">The line style to display.</param>
    /// <returns>A sequence containing the created item.</returns>
    public static IEnumerable<LegendItem> Single(IPlottable plottable, string label, FillStyle fillStyle, LineStyle lineStyle)
    {
        LegendItem item = new() { Plottable = plottable, LabelText = label, MarkerStyle = MarkerStyle.None, FillStyle = fillStyle, LineStyle = lineStyle, };

        return Single(item);
    }

    /// <summary>Creates a legend item with the specified line and marker styles.</summary>
    /// <param name="plottable">The associated plottable.</param>
    /// <param name="label">The legend label.</param>
    /// <param name="lineStyle">The line style to display.</param>
    /// <param name="markerStyle">The marker style to display.</param>
    /// <returns>A sequence containing the created item.</returns>
    public static IEnumerable<LegendItem> Single(IPlottable plottable, string label, LineStyle lineStyle, MarkerStyle markerStyle)
    {
        LegendItem item = new() { Plottable = plottable, LabelText = label, MarkerStyle = markerStyle, LineStyle = lineStyle, };

        return Single(item);
    }
}
