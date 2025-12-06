// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents an icon that uses an image.
/// </summary>
public class ImageIcon : IconElement
{
    /// <summary>
    /// Property for <see cref="Source"/>.
    /// </summary>
    public static readonly StyledProperty<IImage?> SourceProperty =
        AvaloniaProperty.Register<ImageIcon, IImage?>(nameof(Source));

    static ImageIcon()
    {
        AffectsRender<ImageIcon>(SourceProperty);
    }

    /// <summary>
    /// Gets or sets the source for the image.
    /// </summary>
    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <inheritdoc/>
    public override void Render(DrawingContext context)
    {
        if (context is null || Source is null)
        {
            return;
        }

        base.Render(context);

        var destRect = new global::Avalonia.Rect(0, 0, Bounds.Width, Bounds.Height);
        context.DrawImage(Source, destRect);
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        if (Source is null)
        {
            return default;
        }

        return Source.Size;
    }
}
