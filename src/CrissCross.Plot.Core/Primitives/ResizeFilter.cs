// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Specifies the filter used when resizing an image.</summary>
public enum ResizeFilter
{
    /// <summary>Selects the nearest source pixel without interpolation.</summary>
    NearestNeighbor,

    /// <summary>Interpolates from the four nearest source pixels.</summary>
    Bilinear,

    /// <summary>Interpolates from a neighborhood of source pixels using a cubic filter.</summary>
    Bicubic,
}
