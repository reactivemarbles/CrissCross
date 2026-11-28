// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders an image inside a data-coordinate rectangle.</summary>
public class ImageRect : IPlottable
{
    /// <summary>Gets or sets whether this image is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position this image.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the image to render.</summary>
    public Image? Image { get; set; }

    /// <summary>Gets or sets the data-coordinate rectangle occupied by the image.</summary>
    public CoordinateRect Rect { get; set; } = new(Constants.MinimumCoordinate, Constants.MaximumCoordinate, Constants.MinimumCoordinate, Constants.MaximumCoordinate);

    /// <summary>Gets or sets whether image rendering uses antialiasing.</summary>
    public bool AntiAlias { get; set; } = true;

    /// <summary>Gets the legend items for this image.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets the axis limits occupied by this image.</summary>
    /// <returns>The axis limits of the image rectangle.</returns>
    public AxisLimits GetAxisLimits() => new(Rect);

    /// <summary>Renders the image inside its data-coordinate rectangle.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible || Image is null)
        {
            return;
        }

        PixelRect imagePixelRect = Axes.GetPixelRect(Rect);
        Image.Render(rp.Canvas, imagePixelRect, rp.Paint, AntiAlias);
    }

    /// <summary>Contains default values for the unit coordinate rectangle.</summary>
    private static class Constants
    {
        /// <summary>The minimum coordinate of the unit rectangle.</summary>
        public const double MinimumCoordinate = 0;

        /// <summary>The maximum coordinate of the unit rectangle.</summary>
        public const double MaximumCoordinate = 1;
    }
}
