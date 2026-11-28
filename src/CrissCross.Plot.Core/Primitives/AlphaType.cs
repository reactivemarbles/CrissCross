// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Specifies how alpha values are represented in image pixels.</summary>
public enum AlphaType
{
    /// <summary>The alpha representation is unknown.</summary>
    Unknown = 0,

    /// <summary>Pixels are fully opaque.</summary>
    Opaque,

    /// <summary>Color channels are premultiplied by alpha.</summary>
    Premultiplied,

    /// <summary>Color channels are not premultiplied by alpha.</summary>
    Unpremultiplied,
}
