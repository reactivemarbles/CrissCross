// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Describes how to size and position an image inside a given rectangle.</summary>
public enum ImagePosition
{
    /// <summary>Image placed at the upper-left of the rectangle with no scaling.</summary>
    TopLeft,

    /// <summary>Image placed at the center of the rectangle with no scaling.</summary>
    Center,

    /// <summary>Fill image in X and Y to completely fill the area. The aspect ratio may change, appearing to distort the image.</summary>
    Stretch,

    /// <summary>
    /// Scale the image as large as possible such that it will fit
    /// entirely within the rectangle. This may result in whitespace
    /// on the edges if the image and rectangle have different aspect ratios.
    /// </summary>
    Fill,

    /// <summary>
    /// Scale the image by zooming in so it entirely fills the
    /// rectangle. If the aspect ratios differ, some of the image may
    /// lie outside the rectangle after scaling.
    /// </summary>
    Zoom,
}
