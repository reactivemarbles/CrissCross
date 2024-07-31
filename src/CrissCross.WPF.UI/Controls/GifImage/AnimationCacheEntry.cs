// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Media.Animation;

namespace CrissCross.WPF.UI.Controls
{
    internal class AnimationCacheEntry(ObjectKeyFrameCollection keyFrames, Duration duration, int repeatCountFromMetadata)
    {
        public ObjectKeyFrameCollection KeyFrames { get; } = keyFrames;

        public Duration Duration { get; } = duration;

        public int RepeatCountFromMetadata { get; } = repeatCountFromMetadata;
    }
}
