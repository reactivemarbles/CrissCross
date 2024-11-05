// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// DownloadProgressEventArgs.
/// </summary>
/// <seealso cref="System.Windows.RoutedEventArgs" />
/// <remarks>
/// Initializes a new instance of the <see cref="DownloadProgressEventArgs"/> class.
/// </remarks>
/// <param name="source">The source.</param>
/// <param name="progress">The progress.</param>
public class DownloadProgressEventArgs(object source, int progress) : RoutedEventArgs(AnimationBehavior.DownloadProgressEvent, source)
{
    /// <summary>
    /// Gets or sets the progress.
    /// </summary>
    /// <value>
    /// The progress.
    /// </value>
    public int Progress { get; set; } = progress;
}
