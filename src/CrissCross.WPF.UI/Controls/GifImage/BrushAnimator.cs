// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Media.Animation;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.UI.Controls.Decoding;
#else
using CrissCross.WPF.UI.Controls.Decoding;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents BrushAnimator.</summary>
/// <seealso cref="Animator" />
public sealed class BrushAnimator : Animator
{
    /// <summary>Stores the _repeatBehavior value.</summary>
    private RepeatBehavior _repeatBehavior;

    /// <summary>Initializes a new instance of the <see cref="BrushAnimator"/> class.</summary>
    /// <param name="sourceStream">The sourceStream value.</param>
    /// <param name="sourceUri">The sourceUri value.</param>
    /// <param name="metadata">The metadata value.</param>
    /// <param name="repeatBehavior">The repeatBehavior value.</param>
    /// <param name="cacheFrameDataInMemory">The cacheFrameDataInMemory value.</param>
    private BrushAnimator(
        Stream sourceStream,
        Uri? sourceUri,
        GifDataStream metadata,
        RepeatBehavior repeatBehavior,
        bool cacheFrameDataInMemory)
        : base(sourceStream, sourceUri, metadata, repeatBehavior, cacheFrameDataInMemory)
    {
        Brush = new ImageBrush { ImageSource = Bitmap };
        RepeatBehavior = _repeatBehavior;
    }

    /// <summary>Gets the brush.</summary>
    /// <value>
    /// The brush.
    /// </value>
    public ImageBrush Brush { get; }

    /// <summary>Gets or sets the repeat behavior.</summary>
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

    /// <summary>Gets the animation source.</summary>
    /// <value>
    /// The animation source.
    /// </value>
    protected override object AnimationSource => Brush;

    /// <summary>Creates the asynchronous.</summary>
    /// <param name="sourceUri">The source URI.</param>
    /// <param name="repeatBehavior">The repeat behavior.</param>
    /// <returns>A BrushAnimator.</returns>
    public static Task<BrushAnimator> CreateAsync(Uri sourceUri, RepeatBehavior repeatBehavior) =>
        CreateAsync(sourceUri, repeatBehavior, false, null);

    /// <summary>Creates the asynchronous animator and reports loading progress.</summary>
    /// <param name="sourceUri">The source URI.</param>
    /// <param name="repeatBehavior">The repeat behavior.</param>
    /// <param name="progress">The progress.</param>
    /// <returns>A BrushAnimator.</returns>
    public static Task<BrushAnimator> CreateAsync(
        Uri sourceUri,
        RepeatBehavior repeatBehavior,
        IProgress<int>? progress) => CreateAsync(sourceUri, repeatBehavior, false, progress);

    /// <summary>Creates the asynchronous.</summary>
    /// <param name="sourceUri">The source URI.</param>
    /// <param name="repeatBehavior">The repeat behavior.</param>
    /// <param name="cacheFrameDataInMemory">if set to <c>true</c> [cache frame data in memory].</param>
    /// <returns>A BrushAnimator.</returns>
    public static Task<BrushAnimator> CreateAsync(
        Uri sourceUri,
        RepeatBehavior repeatBehavior,
        bool cacheFrameDataInMemory) => CreateAsync(sourceUri, repeatBehavior, cacheFrameDataInMemory, null);

    /// <summary>Creates the asynchronous animator and reports loading progress.</summary>
    /// <param name="sourceUri">The source URI.</param>
    /// <param name="repeatBehavior">The repeat behavior.</param>
    /// <param name="cacheFrameDataInMemory">Whether to cache frame data in memory.</param>
    /// <param name="progress">The progress.</param>
    /// <returns>A BrushAnimator.</returns>
    public static Task<BrushAnimator> CreateAsync(
        Uri sourceUri,
        RepeatBehavior repeatBehavior,
        bool cacheFrameDataInMemory,
        IProgress<int>? progress) =>
        CreateAsyncCore(
            sourceUri,
            progress,
            (stream, metadata) =>
                new BrushAnimator(stream, sourceUri, metadata, repeatBehavior, cacheFrameDataInMemory));

    /// <summary>Creates the asynchronous.</summary>
    /// <param name="sourceStream">The source stream.</param>
    /// <param name="repeatBehavior">The repeat behavior.</param>
    /// <returns>A BrushAnimator.</returns>
    public static Task<BrushAnimator> CreateAsync(Stream sourceStream, RepeatBehavior repeatBehavior) =>
        CreateAsync(sourceStream, repeatBehavior, false);

    /// <summary>Creates the asynchronous.</summary>
    /// <param name="sourceStream">The source stream.</param>
    /// <param name="repeatBehavior">The repeat behavior.</param>
    /// <param name="cacheFrameDataInMemory">if set to <c>true</c> [cache frame data in memory].</param>
    /// <returns>A BrushAnimator.</returns>
    public static Task<BrushAnimator> CreateAsync(
        Stream sourceStream,
        RepeatBehavior repeatBehavior,
        bool cacheFrameDataInMemory)
    {
        if (sourceStream is null)
        {
            throw new ArgumentNullException(nameof(sourceStream));
        }

        return CreateAsyncCore(
            sourceStream,
            metadata => new BrushAnimator(sourceStream, null, metadata, repeatBehavior, cacheFrameDataInMemory));
    }

    /// <summary>Gets the specified repeat behavior.</summary>
    /// <returns>A RepeatBehavior.</returns>
    protected override RepeatBehavior GetSpecifiedRepeatBehavior() => RepeatBehavior;
}
