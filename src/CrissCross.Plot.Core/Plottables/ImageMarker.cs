// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders an image centered at a data-coordinate location.</summary>
public class ImageMarker : IPlottable
{
    /// <summary>Gets or sets whether this image marker is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position this image.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the image to render.</summary>
    public Image? Image { get; set; }

    /// <summary>Gets or sets the data-coordinate location of the image center.</summary>
    public Coordinates Location { get; set; }

    /// <summary>Gets or sets the image scale factor.</summary>
    public float Scale { get; set; } = 1;

    /// <summary>Gets or sets whether image rendering uses antialiasing.</summary>
    public bool AntiAlias { get; set; } = true;

    /// <summary>Gets the legend items for this image marker.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets the axis limits occupied by this image marker.</summary>
    /// <returns>The axis limits of the marker location.</returns>
    public AxisLimits GetAxisLimits() => AxisLimits.FromPoint(Location);

    /// <summary>Renders the image centered at its data-coordinate location.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible || Image is null)
        {
            return;
        }

        Pixel centerPixel = Axes.GetPixel(Location);
        float leftPixel = centerPixel.X - (Image.Width * Constants.Half * Scale);
        float rightPixel = centerPixel.X + (Image.Width * Constants.Half * Scale);
        float bottomPixel = centerPixel.Y + (Image.Height * Constants.Half * Scale);
        float topPixel = centerPixel.Y - (Image.Height * Constants.Half * Scale);
        PixelRect imageRect = new(leftPixel, rightPixel, bottomPixel, topPixel);
        Image.Render(rp.Canvas, imageRect, rp.Paint, AntiAlias);
    }

    /// <summary>Contains image geometry constants.</summary>
    private static class Constants
    {
        /// <summary>The half-size multiplier used to position an image around its center.</summary>
        public const float Half = 0.5F;
    }
}
