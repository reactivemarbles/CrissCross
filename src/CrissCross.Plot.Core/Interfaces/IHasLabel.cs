// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides label styling settings for plottables.</summary>
internal interface IHasLabel
{
    /// <summary>Gets the label style.</summary>
    LabelStyle LabelStyle { get; }

    /// <summary>Gets or sets the horizontal label offset.</summary>
    float LabelOffsetX { get; set; }

    /// <summary>Gets or sets the vertical label offset.</summary>
    float LabelOffsetY { get; set; }

    /// <summary>Gets or sets the label rotation angle.</summary>
    float LabelRotation { get; set; }

    /// <summary>Sets the label padding.</summary>
    float LabelPadding { set; }

    /// <summary>Gets or sets the label padding in pixels.</summary>
    PixelPadding LabelPixelPadding { get; set; }

    /// <summary>Gets or sets the label text.</summary>
    string LabelText { get; set; }

    /// <summary>Gets or sets the label alignment.</summary>
    Alignment LabelAlignment { get; set; }

    /// <summary>Gets or sets the label font family name.</summary>
    string LabelFontName { get; set; }

    /// <summary>Gets or sets the label font size.</summary>
    float LabelFontSize { get; set; }

    /// <summary>Gets or sets the label line spacing.</summary>
    float? LabelLineSpacing { get; set; }

    /// <summary>Gets or sets whether the label font is italic.</summary>
    bool LabelItalic { get; set; }

    /// <summary>Gets or sets whether the label font is bold.</summary>
    bool LabelBold { get; set; }

    /// <summary>Gets or sets the label font color.</summary>
    Color LabelFontColor { get; set; }

    /// <summary>Gets or sets the label background color.</summary>
    Color LabelBackgroundColor { get; set; }

    /// <summary>Gets or sets the label border width.</summary>
    float LabelBorderWidth { get; set; }

    /// <summary>Gets or sets the label border color.</summary>
    Color LabelBorderColor { get; set; }

    /// <summary>Gets or sets the label border radius.</summary>
    float LabelBorderRadius { get; set; }

    /// <summary>Gets or sets the label shadow color.</summary>
    Color LabelShadowColor { get; set; }

    /// <summary>Gets or sets the label shadow offset.</summary>
    PixelOffset LabelShadowOffset { get; set; }
}
