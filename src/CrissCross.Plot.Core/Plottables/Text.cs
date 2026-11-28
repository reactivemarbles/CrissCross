// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders text at a coordinate with configurable label styling.</summary>
public class Text : LabelStyleProperties, IPlottable
{
    /// <summary>Gets or sets the obsolete label-style field.</summary>
    [Obsolete("Interact properties in this class (e.g., LabelFontColor) or properties of LabelStyle", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1401", Justification = "Retained as an obsolete public field for API compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained as an obsolete public field for API compatibility.")]
    public readonly LabelStyle Label = null!;

    /// <summary>Gets or sets the text location in plot coordinates.</summary>
    public Coordinates Location { get; set; }

    /// <summary>Gets or sets the horizontal pixel offset.</summary>
    public float OffsetX { get; set; }

    /// <summary>Gets or sets the vertical pixel offset.</summary>
    public float OffsetY { get; set; }

    /// <summary>Gets or sets the label style used to render the text.</summary>
    public override LabelStyle LabelStyle { get; set; } = new() { FontSize = Constants.DefaultFontSize };

    /// <summary>Gets or sets the label alignment.</summary>
    public Alignment Alignment { get => LabelAlignment; set => LabelAlignment = value; }

    /// <summary>Gets or sets whether this text is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position this text.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this text plottable.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets the obsolete alias for <see cref="LabelStyleProperties.LabelFontColor"/>.</summary>
    [Obsolete("Use LabelFontColor")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility.")]
    public Color Color { get => LabelFontColor; set => LabelFontColor = value; }

    /// <summary>Gets or sets the obsolete alias for <see cref="LabelStyleProperties.LabelFontColor"/>.</summary>
    [Obsolete("Use LabelFontColor")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility.")]
    public Color FontColor { get => LabelFontColor; set => LabelFontColor = value; }

    /// <summary>Gets or sets the obsolete alias for <see cref="LabelStyleProperties.LabelBackgroundColor"/>.</summary>
    [Obsolete("Use LabelBackgroundColor")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility.")]
    public Color BackgroundColor { get => LabelBackgroundColor; set => LabelBackgroundColor = value; }

    /// <summary>Gets or sets the obsolete alias for <see cref="LabelStyleProperties.LabelBackgroundColor"/>.</summary>
    [Obsolete("use LabelBackgroundColor")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility.")]
    public Color BackColor { get => LabelBackgroundColor; set => LabelBackgroundColor = value; }

    /// <summary>Gets or sets the obsolete alias for <see cref="LabelStyleProperties.LabelBorderColor"/>.</summary>
    [Obsolete("use LabelBorderColor")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility.")]
    public Color BorderColor { get => LabelBorderColor; set => LabelBorderColor = value; }

    /// <summary>Gets or sets the obsolete alias for <see cref="LabelStyleProperties.LabelBorderWidth"/>.</summary>
    [Obsolete("use LabelBorderWidth")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility.")]
    public float BorderWidth { get => LabelBorderWidth; set => LabelBorderWidth = value; }

    /// <summary>Gets or sets the obsolete alias for label padding.</summary>
    [Obsolete("use LabelPadding or LabelPixelPadding")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility.")]
    public float Padding { get => LabelPixelPadding.Top; set => LabelPadding = value; }

    /// <summary>Gets or sets the obsolete alias for <see cref="LabelStyleProperties.LabelFontSize"/>.</summary>
    [Obsolete("use LabelFontSize")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility.")]
    public float Size { get => LabelFontSize; set => LabelFontSize = value; }

    /// <summary>Gets or sets the obsolete alias for <see cref="LabelStyleProperties.LabelFontSize"/>.</summary>
    [Obsolete("use LabelFontSize")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility.")]
    public float FontSize { get => LabelFontSize; set => LabelFontSize = value; }

    /// <summary>Gets or sets the obsolete alias for <see cref="LabelStyleProperties.LabelBold"/>.</summary>
    [Obsolete("use LabelBold")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility.")]
    public bool Bold { get => LabelBold; set => LabelBold = value; }

    /// <summary>Gets or sets the obsolete alias for <see cref="LabelStyleProperties.LabelRotation"/>.</summary>
    [Obsolete("use LabelRotation")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility.")]
    public float Rotation { get => LabelRotation; set => LabelRotation = value; }

    /// <summary>Gets or sets the obsolete alias for <see cref="LabelStyleProperties.LabelFontName"/>.</summary>
    [Obsolete("use LabelFontName")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility.")]
    public string FontName { get => LabelFontName; set => LabelFontName = value; }

    /// <summary>Gets or sets the obsolete alias for <see cref="LabelStyleProperties.LabelLineSpacing"/>.</summary>
    [Obsolete("use LabelLineSpacing")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility.")]
    public float? LineSpacing { get => LabelLineSpacing; set => LabelLineSpacing = value; }

    /// <summary>Gets the axis limits occupied by this text.</summary>
    /// <returns>The axis limits of the text location.</returns>
    public AxisLimits GetAxisLimits() => new(Location);

    /// <summary>Renders this text at its plot location.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        Pixel pixelLocation = Axes.GetPixel(Location);
        pixelLocation.X += OffsetX;
        pixelLocation.Y += OffsetY;
        LabelStyle.Render(rp.Canvas, pixelLocation, rp.Paint);
    }

    /// <summary>Contains constants used by text plottables.</summary>
    internal static class Constants
    {
        /// <summary>The default text font size in pixels.</summary>
        internal const float DefaultFontSize = 14;
    }
}
