// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Animations;
#else
namespace CrissCross.Avalonia.UI.Animations;
#endif

/// <summary>Available types of transitions.</summary>
public enum Transition
{
    /// <summary>None member.</summary>
    None,

    /// <summary>Change opacity.</summary>
    FadeIn,

    /// <summary>Change opacity and slide from bottom.</summary>
    FadeInWithSlide,

    /// <summary>Slide from bottom.</summary>
    SlideBottom,

    /// <summary>Slide from the right side.</summary>
    SlideRight,

    /// <summary>Slide from the left side.</summary>
    SlideLeft,
}
