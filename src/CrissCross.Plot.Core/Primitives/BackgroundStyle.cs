// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Defines the background color, fill, and optional image for a plot.</summary>
public class BackgroundStyle : IDisposable
{
    /// <summary>Gets or sets whether to antialias the background image.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1401",
        Justification = "AntiAlias is an existing public field; replacing it with a property would break the compiled API.")]
    public bool AntiAlias = true;

    /// <summary>Holds the decoded bitmap used to manage the background image resources.</summary>
    private SKBitmap? _bitmap;

    /// <summary>Gets or sets the background color.</summary>
    public Color Color { get => FillStyle.Color; set => FillStyle.Color = value; }

    /// <summary>Gets or sets the background fill style.</summary>
    public FillStyle FillStyle { get; set; } = new() { IsVisible = true, Color = Colors.White, };

    /// <summary>Gets or sets how the background image is sized and positioned.</summary>
    public ImagePosition ImagePosition { get; set; } = ImagePosition.Stretch;

    /// <summary>Gets or sets the background image.</summary>
    public Image? Image
    {
        get;
        set
        {
            _bitmap?.Dispose();
            _bitmap = null;
            field = value;

            if (value is null)
            {
                return;
            }

            byte[] bytes = value.GetImageBytes();
            _bitmap = SKBitmap.Decode(bytes); // TODO: SKImage instead?
        }
    }

    /// <summary>Releases the bitmap owned by this background style.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Gets the rectangle occupied by the background image in a target area.</summary>
    /// <param name="targetRect">The target rectangle.</param>
    /// <returns>The image rectangle, or <see cref="PixelRect.Zero"/> when no image is set.</returns>
    public PixelRect GetImageRect(PixelRect targetRect) => Image is null
        ? PixelRect.Zero
        : ImagePosition.GetRect(Image.Size, targetRect);

    /// <summary>Renders the background within the specified target area.</summary>
    /// <param name="rp">The render pack providing the canvas and paint.</param>
    /// <param name="target">The target rectangle.</param>
    public void Render(RenderPack rp, PixelRect target)
    {
        Drawing.FillRectangle(rp.Canvas, target, rp.Paint, FillStyle);

        if (Image is null)
        {
            return;
        }

        PixelRect imgRect = ImagePosition.GetRect(Image.Size, target);
        Image.Render(rp.Canvas, imgRect, rp.Paint, AntiAlias);
    }

    /// <summary>Releases resources owned by this background style.</summary>
    /// <param name="disposing">Whether managed resources should be released.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _bitmap?.Dispose();
        _bitmap = null;
    }
}
