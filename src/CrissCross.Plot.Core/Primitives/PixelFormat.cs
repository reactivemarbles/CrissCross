// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Specifies the byte ordering used to represent pixels.</summary>
public enum PixelFormat
{
    /// <summary>The pixel format is unknown or unsupported.</summary>
    Unknown = 0,

    /// <summary>Four-channel pixels ordered blue, green, red, alpha.</summary>
    Bgra8888,

    /// <summary>Four-channel pixels ordered red, green, blue, alpha.</summary>
    Rgba8888,
}
