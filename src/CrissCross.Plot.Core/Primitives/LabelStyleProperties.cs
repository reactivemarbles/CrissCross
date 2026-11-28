// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides shortcuts to commonly used properties of an associated <see cref="LabelStyle"/>.</summary>
public abstract class LabelStyleProperties : IHasLabel
{
    /// <summary>Gets or sets the label style used by this object.</summary>
    public abstract LabelStyle LabelStyle { get; set; }

    LabelStyle IHasLabel.LabelStyle => LabelStyle;

    /// <summary>Gets or sets the horizontal label offset.</summary>
    public float LabelOffsetX { get => LabelStyle.OffsetX; set => LabelStyle.OffsetX = value; }

    /// <summary>Gets or sets the vertical label offset.</summary>
    public float LabelOffsetY { get => LabelStyle.OffsetY; set => LabelStyle.OffsetY = value; }

    /// <summary>Gets or sets the label rotation.</summary>
    public float LabelRotation { get => LabelStyle.Rotation; set => LabelStyle.Rotation = value; }

    /// <summary>Sets the label padding.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1421", Justification = "The write-only property is part of the existing public interface contract.")]
    public float LabelPadding { set => LabelStyle.Padding = value; }

    /// <summary>Gets or sets the label padding in pixels.</summary>
    public PixelPadding LabelPixelPadding { get => LabelStyle.PixelPadding; set => LabelStyle.PixelPadding = value; }

    /// <summary>Gets the pixel rectangle used for the last label render.</summary>
    public PixelRect LabelLastRenderPixelRect => LabelStyle.LastRenderPixelRect;

    /// <summary>Gets or sets the label text.</summary>
    public string LabelText { get => LabelStyle.Text; set => LabelStyle.Text = value; }

    /// <summary>Gets or sets the label alignment.</summary>
    public Alignment LabelAlignment { get => LabelStyle.Alignment; set => LabelStyle.Alignment = value; }

    /// <summary>Gets or sets the label font family name.</summary>
    public string LabelFontName { get => LabelStyle.FontName; set => LabelStyle.FontName = value; }

    /// <summary>Gets or sets the label font size.</summary>
    public float LabelFontSize { get => LabelStyle.FontSize; set => LabelStyle.FontSize = value; }

    /// <summary>Gets or sets the label line spacing.</summary>
    public float? LabelLineSpacing { get => LabelStyle.LineSpacing; set => LabelStyle.LineSpacing = value; }

    /// <summary>Gets or sets whether the label font is italic.</summary>
    public bool LabelItalic { get => LabelStyle.Italic; set => LabelStyle.Italic = value; }

    /// <summary>Gets or sets whether the label font is bold.</summary>
    public bool LabelBold { get => LabelStyle.Bold; set => LabelStyle.Bold = value; }

    /// <summary>Gets or sets whether the label is underlined.</summary>
    public bool LabelUnderline { get => LabelStyle.Underline; set => LabelStyle.Underline = value; }

    /// <summary>Gets or sets the label underline width.</summary>
    public double LabelUnderlineWidth { get => LabelStyle.UnderlineWidth; set => LabelStyle.UnderlineWidth = value; }

    /// <summary>Gets or sets the label underline offset.</summary>
    public double LabelUnderlineOffset { get => LabelStyle.UnderlineOffset; set => LabelStyle.UnderlineOffset = value; }

    /// <summary>Gets or sets the label font color.</summary>
    public Color LabelFontColor { get => LabelStyle.ForeColor; set => LabelStyle.ForeColor = value; }

    /// <summary>Gets or sets the label background color.</summary>
    public Color LabelBackgroundColor { get => LabelStyle.BackgroundColor; set => LabelStyle.BackgroundColor = value; }

    /// <summary>Gets or sets the label border radius.</summary>
    public float LabelBorderRadius { get => LabelStyle.BorderRadius; set => LabelStyle.BorderRadius = value; }

    /// <summary>Gets or sets the horizontal label border radius.</summary>
    public float LabelBorderRadiusX { get => LabelStyle.BorderRadiusX; set => LabelStyle.BorderRadiusX = value; }

    /// <summary>Gets or sets the vertical label border radius.</summary>
    public float LabelBorderRadiusY { get => LabelStyle.BorderRadiusY; set => LabelStyle.BorderRadiusY = value; }

    /// <summary>Gets or sets the label border width.</summary>
    public float LabelBorderWidth { get => LabelStyle.BorderWidth; set => LabelStyle.BorderWidth = value; }

    /// <summary>Gets or sets the label border color.</summary>
    public Color LabelBorderColor { get => LabelStyle.BorderColor; set => LabelStyle.BorderColor = value; }

    /// <summary>Gets or sets the label shadow color.</summary>
    public Color LabelShadowColor { get => LabelStyle.ShadowColor; set => LabelStyle.ShadowColor = value; }

    /// <summary>Gets or sets the label shadow offset.</summary>
    public PixelOffset LabelShadowOffset { get => LabelStyle.ShadowOffset; set => LabelStyle.ShadowOffset = value; }
}
