// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Windows.Media.Animation;
using CrissCross.WPF.UI.Controls.Decoding;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// BrushAnimator.
/// </summary>
/// <seealso cref="CrissCross.WPF.UI.Controls.Animator" />
public sealed class BrushAnimator : Animator
{
    private RepeatBehavior _repeatBehavior;

    private BrushAnimator(Stream sourceStream, Uri? sourceUri, GifDataStream metadata, RepeatBehavior repeatBehavior, bool cacheFrameDataInMemory)
        : base(sourceStream, sourceUri, metadata, repeatBehavior, cacheFrameDataInMemory)
    {
        Brush = new ImageBrush { ImageSource = Bitmap };
        RepeatBehavior = _repeatBehavior;
    }

    /// <summary>
    /// Gets the brush.
    /// </summary>
    /// <value>
    /// The brush.
    /// </value>
    public ImageBrush Brush { get; }

    /// <summary>
    /// Gets or sets the repeat behavior.
    /// </summary>
    /// <value>
    /// The repeat behavior.
    /// </value>
    public RepeatBehavior RepeatBehavior
    {
        get => _repeatBehavior;

        set
        {
            _repeatBehavior = value;
            OnRepeatBehaviorChanged();
        }
    }

    /// <summary>
    /// Gets the animation source.
    /// </summary>
    /// <value>
    /// The animation source.
    /// </value>
    protected override object AnimationSource => Brush;

    /// <summary>
    /// Creates the asynchronous.
    /// </summary>
    /// <param name="sourceUri">The source URI.</param>
    /// <param name="repeatBehavior">The repeat behavior.</param>
    /// <param name="progress">The progress.</param>
    /// <returns>A BrushAnimator.</returns>
    public static Task<BrushAnimator> CreateAsync(Uri sourceUri, RepeatBehavior repeatBehavior, IProgress<int>? progress = null) =>
        CreateAsync(sourceUri, repeatBehavior, false, progress);

    /// <summary>
    /// Creates the asynchronous.
    /// </summary>
    /// <param name="sourceUri">The source URI.</param>
    /// <param name="repeatBehavior">The repeat behavior.</param>
    /// <param name="cacheFrameDataInMemory">if set to <c>true</c> [cache frame data in memory].</param>
    /// <param name="progress">The progress.</param>
    /// <returns>A BrushAnimator.</returns>
    public static Task<BrushAnimator> CreateAsync(Uri sourceUri, RepeatBehavior repeatBehavior, bool cacheFrameDataInMemory, IProgress<int>? progress = null) => CreateAsyncCore(
            sourceUri,
            progress,
            (stream, metadata) => new BrushAnimator(stream, sourceUri, metadata, repeatBehavior, cacheFrameDataInMemory));

    /// <summary>
    /// Creates the asynchronous.
    /// </summary>
    /// <param name="sourceStream">The source stream.</param>
    /// <param name="repeatBehavior">The repeat behavior.</param>
    /// <returns>A BrushAnimator.</returns>
    public static Task<BrushAnimator> CreateAsync(Stream sourceStream, RepeatBehavior repeatBehavior) => CreateAsync(sourceStream, repeatBehavior, false);

    /// <summary>
    /// Creates the asynchronous.
    /// </summary>
    /// <param name="sourceStream">The source stream.</param>
    /// <param name="repeatBehavior">The repeat behavior.</param>
    /// <param name="cacheFrameDataInMemory">if set to <c>true</c> [cache frame data in memory].</param>
    /// <returns>A BrushAnimator.</returns>
    public static Task<BrushAnimator> CreateAsync(Stream sourceStream, RepeatBehavior repeatBehavior, bool cacheFrameDataInMemory)
    {
        if (sourceStream == null)
        {
            throw new ArgumentNullException(nameof(sourceStream));
        }

        return CreateAsyncCore(
            sourceStream,
            metadata => new BrushAnimator(sourceStream, null, metadata, repeatBehavior, cacheFrameDataInMemory));
    }

    /// <summary>
    /// Gets the specified repeat behavior.
    /// </summary>
    /// <returns>A RepeatBehavior.</returns>
    protected override RepeatBehavior GetSpecifiedRepeatBehavior() => RepeatBehavior;
}
