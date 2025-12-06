// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents an icon that uses a path as its content.
/// </summary>
public class PathIcon : IconElement
{
    /// <summary>
    /// Property for <see cref="Data"/>.
    /// </summary>
    public static readonly StyledProperty<Geometry?> DataProperty =
        AvaloniaProperty.Register<PathIcon, Geometry?>(nameof(Data));

    static PathIcon()
    {
        AffectsRender<PathIcon>(DataProperty);
    }

    /// <summary>
    /// Gets or sets a <see cref="Geometry"/> that specifies the shape to be drawn.
    /// </summary>
    public Geometry? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    /// <inheritdoc/>
    public override void Render(DrawingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        base.Render(context);

        if (Data is null)
        {
            return;
        }

        var pen = new Pen(Foreground, 1);
        context.DrawGeometry(Foreground, pen, Data);
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        if (Data is null)
        {
            return default;
        }

        return Data.Bounds.Size;
    }
}
