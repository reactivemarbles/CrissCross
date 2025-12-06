// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents an image control with GIF animation support and additional properties.
/// </summary>
public class GifImage : global::Avalonia.Controls.Image
{
    /// <summary>
    /// Property for <see cref="CornerRadius"/>.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<GifImage, CornerRadius>(nameof(CornerRadius), new CornerRadius(0));

    /// <summary>
    /// Property for <see cref="StretchDirection"/>.
    /// </summary>
    public static readonly StyledProperty<StretchDirection> StretchDirectionProperty =
        AvaloniaProperty.Register<GifImage, StretchDirection>(nameof(StretchDirection), StretchDirection.Both);

    /// <summary>
    /// Property for <see cref="SourceUri"/>.
    /// </summary>
    public static readonly StyledProperty<Uri?> SourceUriProperty =
        AvaloniaProperty.Register<GifImage, Uri?>(nameof(SourceUri));

    /// <summary>
    /// Property for <see cref="AutoStart"/>.
    /// </summary>
    public static readonly StyledProperty<bool> AutoStartProperty =
        AvaloniaProperty.Register<GifImage, bool>(nameof(AutoStart), true);

    /// <summary>
    /// Property for <see cref="RepeatCount"/>.
    /// </summary>
    public static readonly StyledProperty<int> RepeatCountProperty =
        AvaloniaProperty.Register<GifImage, int>(nameof(RepeatCount), 0);

    /// <summary>
    /// Property for <see cref="AnimationSpeedRatio"/>.
    /// </summary>
    public static readonly StyledProperty<double> AnimationSpeedRatioProperty =
        AvaloniaProperty.Register<GifImage, double>(nameof(AnimationSpeedRatio), 1.0);

    /// <summary>
    /// Property for <see cref="IsAnimating"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsAnimatingProperty =
        AvaloniaProperty.Register<GifImage, bool>(nameof(IsAnimating), false);

    /// <summary>
    /// Property for <see cref="CurrentFrameIndex"/>.
    /// </summary>
    public static readonly StyledProperty<int> CurrentFrameIndexProperty =
        AvaloniaProperty.Register<GifImage, int>(nameof(CurrentFrameIndex), 0);

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

    /// <summary>
    /// Gets or sets the source URI for the GIF image.
    /// </summary>
    public Uri? SourceUri
    {
        get => GetValue(SourceUriProperty);
        set => SetValue(SourceUriProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to auto-start the animation.
    /// </summary>
    public bool AutoStart
    {
        get => GetValue(AutoStartProperty);
        set => SetValue(AutoStartProperty, value);
    }

    /// <summary>
    /// Gets or sets the repeat count for the animation. 0 means infinite.
    /// </summary>
    public int RepeatCount
    {
        get => GetValue(RepeatCountProperty);
        set => SetValue(RepeatCountProperty, value);
    }

    /// <summary>
    /// Gets or sets the animation speed ratio.
    /// </summary>
    public double AnimationSpeedRatio
    {
        get => GetValue(AnimationSpeedRatioProperty);
        set => SetValue(AnimationSpeedRatioProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the animation is currently playing.
    /// </summary>
    public bool IsAnimating
    {
        get => GetValue(IsAnimatingProperty);
        set => SetValue(IsAnimatingProperty, value);
    }

    /// <summary>
    /// Gets or sets the current frame index.
    /// </summary>
    public int CurrentFrameIndex
    {
        get => GetValue(CurrentFrameIndexProperty);
        set => SetValue(CurrentFrameIndexProperty, value);
    }

    /// <summary>
    /// Starts the GIF animation.
    /// </summary>
    public void StartAnimation()
    {
        IsAnimating = true;
    }

    /// <summary>
    /// Stops the GIF animation.
    /// </summary>
    public void StopAnimation()
    {
        IsAnimating = false;
    }

    /// <summary>
    /// Pauses the GIF animation.
    /// </summary>
    public void PauseAnimation()
    {
        IsAnimating = false;
    }

    /// <summary>
    /// Resets the animation to the first frame.
    /// </summary>
    public void ResetAnimation()
    {
        CurrentFrameIndex = 0;
        IsAnimating = false;
    }
}
