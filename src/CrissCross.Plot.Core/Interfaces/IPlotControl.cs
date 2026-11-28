// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides access to a plot and platform-specific interactive control services.</summary>
public interface IPlotControl
{
    /// <summary>Gets the primary plot displayed by this control.</summary>
    Plot Plot { get; }

    /// <summary>Gets or sets the multiplot managed by this control.</summary>
    IMultiplot Multiplot { get; set; }

    /// <summary>Gets the processor that receives and responds to UI events.</summary>
    Interactivity.UserInputProcessor UserInputProcessor { get; }

    /// <summary>Gets or sets the platform-specific context-menu manager.</summary>
    IPlotMenu? Menu { get; set; }

    /// <summary>Gets the hardware-accelerated graphics context, or <see langword="null"/> when unavailable.</summary>
    GRContext? GRContext { get; }

    /// <summary>Gets or sets the display scaling used for pixel and coordinate conversions.</summary>
    float DisplayScale { get; set; }

    /// <summary>Renders the plot and updates the displayed image.</summary>
    void Refresh();

    /// <summary>Shows the default pop-up menu at a position in the control.</summary>
    /// <param name="position">The menu position in control pixels.</param>
    void ShowContextMenu(Pixel position);

    /// <summary>Detects the DPI scaling ratio of the present display.</summary>
    /// <returns>The display scale, where 1.0 means no scaling and 1.5 means 150% scaling.</returns>
    float DetectDisplayScale();

    /// <summary>Disposes the current plot and creates a new one for the control.</summary>
    void Reset();

    /// <summary>Loads the specified plot into the control.</summary>
    /// <param name="plot">The plot to load.</param>
    void Reset(Plot plot);

    /// <summary>Sets the platform-specific equivalent cursor.</summary>
    /// <param name="cursor">The cursor to display.</param>
    void SetCursor(Cursor cursor);
}
