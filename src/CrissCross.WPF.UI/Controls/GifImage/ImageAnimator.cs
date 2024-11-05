// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Windows.Media.Animation;
using CrissCross.WPF.UI.Controls.Decoding;

namespace CrissCross.WPF.UI.Controls;

internal class ImageAnimator : Animator
{
    private readonly System.Windows.Controls.Image _image;

    public ImageAnimator(Stream sourceStream, Uri sourceUri, GifDataStream metadata, RepeatBehavior repeatBehavior, System.Windows.Controls.Image image)
        : this(sourceStream, sourceUri, metadata, repeatBehavior, image, false)
    {
    }

    public ImageAnimator(Stream sourceStream, Uri sourceUri, GifDataStream metadata, RepeatBehavior repeatBehavior, System.Windows.Controls.Image image, bool cacheFrameDataInMemory)
        : base(sourceStream, sourceUri, metadata, repeatBehavior, cacheFrameDataInMemory)
    {
        _image = image;
        OnRepeatBehaviorChanged(); // in case the value has changed during creation
    }

    protected override object AnimationSource => _image;

    public static Task<ImageAnimator> CreateAsync(Uri sourceUri, RepeatBehavior repeatBehavior, IProgress<int> progress, System.Windows.Controls.Image image) =>
        CreateAsync(sourceUri, repeatBehavior, progress, image, false);

    public static Task<ImageAnimator> CreateAsync(Uri sourceUri, RepeatBehavior repeatBehavior, IProgress<int> progress, System.Windows.Controls.Image image, bool cacheFrameDataInMemory) =>
        CreateAsyncCore(
            sourceUri,
            progress,
            (stream, metadata) => new ImageAnimator(stream, sourceUri, metadata, repeatBehavior, image, cacheFrameDataInMemory));

    public static Task<ImageAnimator> CreateAsync(Stream sourceStream, RepeatBehavior repeatBehavior, System.Windows.Controls.Image image) =>
        CreateAsync(sourceStream, repeatBehavior, image);

    public static Task<ImageAnimator> CreateAsync(Stream sourceStream, RepeatBehavior repeatBehavior, System.Windows.Controls.Image image, bool cacheFrameDataInMemory) =>
        CreateAsyncCore(
            sourceStream,
            metadata => new ImageAnimator(sourceStream, null!, metadata, repeatBehavior, image, cacheFrameDataInMemory));

    protected override RepeatBehavior GetSpecifiedRepeatBehavior() => AnimationBehavior.GetRepeatBehavior(_image);
}
