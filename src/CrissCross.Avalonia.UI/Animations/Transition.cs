// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Animations;

/// <summary>
/// Available types of transitions.
/// </summary>
public enum Transition
{
    /// <summary>
    /// None.
    /// </summary>
    None,

    /// <summary>
    /// Change opacity.
    /// </summary>
    FadeIn,

    /// <summary>
    /// Change opacity and slide from bottom.
    /// </summary>
    FadeInWithSlide,

    /// <summary>
    /// Slide from bottom.
    /// </summary>
    SlideBottom,

    /// <summary>
    /// Slide from the right side.
    /// </summary>
    SlideRight,

    /// <summary>
    /// Slide from the left side.
    /// </summary>
    SlideLeft,
}
