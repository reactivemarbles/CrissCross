// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents AnimationStartedEventArgs.</summary>
/// <seealso cref="System.Windows.RoutedEventArgs" />
/// <remarks>
/// Initializes a new instance of the <see cref="AnimationStartedEventArgs"/> class.
/// </remarks>
/// <param name="source">The source.</param>
public class AnimationStartedEventArgs(object source)
    : RoutedEventArgs(AnimationBehavior.AnimationStartedEvent, source);
