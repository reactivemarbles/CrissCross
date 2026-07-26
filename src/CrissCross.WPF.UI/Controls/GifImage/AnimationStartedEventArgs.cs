// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
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
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class AnimationStartedEventArgs(object source) : RoutedEventArgs(AnimationBehavior.AnimationStartedEvent, source)
{
    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;
}
