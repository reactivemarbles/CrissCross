// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents an image with additional properties.
/// </summary>
public class GifImage : global::Avalonia.Controls.Image
{
    /// <summary>
    /// Property for <see cref="CornerRadius"/>.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty = AvaloniaProperty.Register<GifImage, CornerRadius>(
        nameof(CornerRadius), new CornerRadius(0));

    /// <summary>
    /// Property for <see cref="StretchDirection"/>.
    /// </summary>
    public static readonly StyledProperty<StretchDirection> StretchDirectionProperty = AvaloniaProperty.Register<GifImage, StretchDirection>(
        nameof(StretchDirection), StretchDirection.Both);

    /// <summary>
    /// Gets or sets the CornerRadius.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the stretch direction.
    /// </summary>
    public StretchDirection StretchDirection
    {
        get => GetValue(StretchDirectionProperty);
        set => SetValue(StretchDirectionProperty, value);
    }
}
