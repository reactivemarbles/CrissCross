// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// AnimationErrorEventArgs.
/// </summary>
/// <seealso cref="RoutedEventArgs" />
/// <remarks>
/// Initializes a new instance of the <see cref="AnimationErrorEventArgs"/> class.
/// </remarks>
/// <param name="source">The source.</param>
/// <param name="exception">The exception.</param>
/// <param name="kind">The kind.</param>
public class AnimationErrorEventArgs(object source, Exception exception, AnimationErrorKind kind) : RoutedEventArgs(AnimationBehavior.ErrorEvent, source)
{
    /// <summary>
    /// Gets the exception.
    /// </summary>
    /// <value>
    /// The exception.
    /// </value>
    public Exception Exception { get; } = exception;

    /// <summary>
    /// Gets the kind.
    /// </summary>
    /// <value>
    /// The kind.
    /// </value>
    public AnimationErrorKind Kind { get; } = kind;
}
