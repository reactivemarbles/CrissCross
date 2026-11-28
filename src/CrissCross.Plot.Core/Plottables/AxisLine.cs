// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Provides common rendering and label behavior for axis lines.</summary>
public abstract class AxisLine : LabelStyleProperties, IPlottable, IRenderLast, IHasLine, IHasLegendText
{
    /// <summary>Gets or sets whether the axis line is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes associated with this line.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the line style.</summary>
    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    /// <summary>Gets or sets the line width.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the line pattern.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the line color.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the label style.</summary>
    public override LabelStyle LabelStyle { get; set; } = new();

    /// <summary>Gets or sets the text displayed by the line.</summary>
    public string Text { get => LabelText; set => LabelText = value; }

    /// <summary>Gets or sets the text displayed in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets the manual label alignment.</summary>
    public Alignment? ManualLabelAlignment { get; set; }

    /// <summary>Gets or sets the obsolete label alignment alias.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete alias remains for public API compatibility and forwards to ManualLabelAlignment.")]
    [Obsolete("Use ManualLabelAlignment")]
    public Alignment? TextAlignment { get => ManualLabelAlignment; set => ManualLabelAlignment = value; }

    /// <summary>Gets or sets the obsolete label style alias.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete alias remains for public API compatibility and forwards to LabelStyle.")]
    [Obsolete("Set LabelFontSize, LabelBold, LabelFontColor, or properties of the LabelStyle object.")]
    public LabelStyle Label { get => LabelStyle; set => LabelStyle = value; }

    /// <summary>Gets or sets the obsolete font-size alias.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete alias remains for public API compatibility and forwards to LabelFontSize.")]
    [Obsolete("Use LabelFontSize")]
    public float FontSize { get => LabelFontSize; set => LabelFontSize = value; }

    /// <summary>Gets or sets the obsolete bold-font alias.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete alias remains for public API compatibility and forwards to LabelBold.")]
    [Obsolete("Use LabelBold")]
    public bool FontBold { get => LabelBold; set => LabelBold = value; }

    /// <summary>Gets or sets the obsolete font-name alias.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete alias remains for public API compatibility and forwards to LabelFontName.")]
    [Obsolete("Use LabelFontName")]
    public string FontName { get => LabelFontName; set => LabelFontName = value; }

    /// <summary>Gets or sets the obsolete font-color alias.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete alias remains for public API compatibility and forwards to LabelFontColor.")]
    [Obsolete("Use LabelFontColor")]
    public Color FontColor { get => LabelFontColor; set => LabelFontColor = value; }

    /// <summary>Gets or sets the obsolete text-color alias.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete alias remains for public API compatibility and forwards to LabelFontColor.")]
    [Obsolete("Use LabelFontColor")]
    public Color TextColor { get => LabelFontColor; set => LabelFontColor = value; }

    /// <summary>Gets or sets the obsolete text-background-color alias.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete alias remains for public API compatibility and forwards to LabelFontColor.")]
    [Obsolete("Use LabelFontColor")]
    public Color TextBackgroundColor { get => LabelFontColor; set => LabelFontColor = value; }

    /// <summary>Gets or sets the obsolete text-rotation alias.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete alias remains for public API compatibility and forwards to LabelRotation.")]
    [Obsolete("Use LabelRotation")]
    public float TextRotation { get => LabelRotation; set => LabelRotation = value; }

    /// <summary>Gets or sets the obsolete text-size alias.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "The obsolete alias remains for public API compatibility and forwards to LabelFontSize.")]
    [Obsolete("Use LabelFontSize")]
    public float TextSize { get => LabelFontSize; set => LabelFontSize = value; }

    /// <summary>Gets or sets whether the label is positioned opposite the axis.</summary>
    public bool LabelOppositeAxis { get; set; }

    /// <summary>Gets or sets whether the line can be dragged.</summary>
    public bool IsDraggable { get; set; }

    /// <summary>Gets or sets whether the line is excluded from the legend.</summary>
    public bool ExcludeFromLegend { get; set; }

    /// <summary>Gets or sets whether the line contributes to autoscaling.</summary>
    public bool EnableAutoscale { get; set; } = true;

    /// <summary>Gets or sets the line and label background color.</summary>
    public Color Color
    {
        get => LineStyle.Color;
        set
        {
            LineStyle.Color = value;
            LabelStyle.BackgroundColor = value;
        }
    }

    /// <summary>Gets or sets the coordinate position of the axis line.</summary>
    public double Position { get; set; }

    /// <summary>Gets the legend item represented by this line.</summary>
    public IEnumerable<LegendItem> LegendItems =>
        LegendItem.Single(new() { Plottable = this, LabelText = ExcludeFromLegend ? string.Empty : LegendText, LineStyle = LineStyle, MarkerStyle = MarkerStyle.None, });

    /// <summary>Determines whether the line is under the specified coordinate rectangle.</summary>
    /// <param name="rect">The coordinate rectangle to test.</param>
    /// <returns><see langword="true"/> when the line is within the rectangle.</returns>
    public abstract bool IsUnderMouse(CoordinateRect rect);

    /// <summary>Gets the axis limits contributed by this line.</summary>
    /// <returns>The limits for the axes.</returns>
    public abstract AxisLimits GetAxisLimits();

    /// <summary>Renders this line.</summary>
    /// <param name="rp">The render pack to draw into.</param>
    public abstract void Render(RenderPack rp);

    /// <summary>Renders this line after other plot content.</summary>
    /// <param name="rp">The render pack to draw into.</param>
    public abstract void RenderLast(RenderPack rp);
}
