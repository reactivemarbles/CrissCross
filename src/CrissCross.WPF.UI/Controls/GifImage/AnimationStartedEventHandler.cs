// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Provides the AnimationStartedEventHandler member.</summary>
/// <param name="d">The d value.</param>
/// <param name="e">The event arguments.</param>
public delegate void AnimationStartedEventHandler(DependencyObject d, AnimationStartedEventArgs e);
