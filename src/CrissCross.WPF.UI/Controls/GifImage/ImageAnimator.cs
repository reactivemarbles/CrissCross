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

/// <summary>Provides the ImageAnimator member.</summary>
internal sealed class ImageAnimator : Animator
{
    /// <summary>Stores the _image value.</summary>
    private readonly System.Windows.Controls.Image _image;

    /// <summary>Initializes a new instance of the <see cref="ImageAnimator"/> class.</summary>
    /// <param name="sourceStream">The sourceStream value.</param>
    /// <param name="sourceUri">The sourceUri value.</param>
    /// <param name="metadata">The metadata value.</param>
    /// <param name="repeatBehavior">The repeatBehavior value.</param>
    /// <param name="image">The image value.</param>
    public ImageAnimator(
        Stream sourceStream,
        Uri sourceUri,
        GifDataStream metadata,
        RepeatBehavior repeatBehavior,
        System.Windows.Controls.Image image)
        : this(sourceStream, sourceUri, metadata, repeatBehavior, image, false) { }

    /// <summary>Initializes a new instance of the <see cref="ImageAnimator"/> class.</summary>
    /// <param name="sourceStream">The sourceStream value.</param>
    /// <param name="sourceUri">The sourceUri value.</param>
    /// <param name="metadata">The metadata value.</param>
    /// <param name="repeatBehavior">The repeatBehavior value.</param>
    /// <param name="image">The image value.</param>
    /// <param name="cacheFrameDataInMemory">The cacheFrameDataInMemory value.</param>
    public ImageAnimator(
        Stream sourceStream,
        Uri sourceUri,
        GifDataStream metadata,
        RepeatBehavior repeatBehavior,
        System.Windows.Controls.Image image,
        bool cacheFrameDataInMemory)
        : base(sourceStream, sourceUri, metadata, repeatBehavior, cacheFrameDataInMemory)
    {
        _image = image;
        OnRepeatBehaviorChanged(); // in case the value has changed during creation
    }

    protected override object AnimationSource => _image;

    /// <summary>Provides the CreateAsync member.</summary>
    /// <param name="sourceUri">The sourceUri value.</param>
    /// <param name="repeatBehavior">The repeatBehavior value.</param>
    /// <param name="progress">The progress value.</param>
    /// <param name="image">The image value.</param>
    /// <returns>The result.</returns>
    public static Task<ImageAnimator> CreateAsync(
        Uri sourceUri,
        RepeatBehavior repeatBehavior,
        IProgress<int> progress,
        System.Windows.Controls.Image image) => CreateAsync(sourceUri, repeatBehavior, progress, image, false);

    /// <summary>Provides the CreateAsync member.</summary>
    /// <param name="sourceUri">The sourceUri value.</param>
    /// <param name="repeatBehavior">The repeatBehavior value.</param>
    /// <param name="progress">The progress value.</param>
    /// <param name="image">The image value.</param>
    /// <param name="cacheFrameDataInMemory">The cacheFrameDataInMemory value.</param>
    /// <returns>The result.</returns>
    public static Task<ImageAnimator> CreateAsync(
        Uri sourceUri,
        RepeatBehavior repeatBehavior,
        IProgress<int> progress,
        System.Windows.Controls.Image image,
        bool cacheFrameDataInMemory) =>
        CreateAsyncCore(
            sourceUri,
            progress,
            (stream, metadata) =>
                new ImageAnimator(stream, sourceUri, metadata, repeatBehavior, image, cacheFrameDataInMemory));

    /// <summary>Provides the CreateAsync member.</summary>
    /// <param name="sourceStream">The sourceStream value.</param>
    /// <param name="repeatBehavior">The repeatBehavior value.</param>
    /// <param name="image">The image value.</param>
    /// <returns>The result.</returns>
    public static Task<ImageAnimator> CreateAsync(
        Stream sourceStream,
        RepeatBehavior repeatBehavior,
        System.Windows.Controls.Image image) => CreateAsync(sourceStream, repeatBehavior, image);

    /// <summary>Provides the CreateAsync member.</summary>
    /// <param name="sourceStream">The sourceStream value.</param>
    /// <param name="repeatBehavior">The repeatBehavior value.</param>
    /// <param name="image">The image value.</param>
    /// <param name="cacheFrameDataInMemory">The cacheFrameDataInMemory value.</param>
    /// <returns>The result.</returns>
    public static Task<ImageAnimator> CreateAsync(
        Stream sourceStream,
        RepeatBehavior repeatBehavior,
        System.Windows.Controls.Image image,
        bool cacheFrameDataInMemory) =>
        CreateAsyncCore(
            sourceStream,
            metadata =>
                new ImageAnimator(sourceStream, null!, metadata, repeatBehavior, image, cacheFrameDataInMemory));

    protected override RepeatBehavior GetSpecifiedRepeatBehavior() => AnimationBehavior.GetRepeatBehavior(_image);
}
