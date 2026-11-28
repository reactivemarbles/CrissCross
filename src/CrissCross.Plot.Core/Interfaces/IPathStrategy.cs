// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Strategy for generating a path that connects a collection of pixels.</summary>
public interface IPathStrategy
{
    /// <summary>Creates a path connecting the supplied pixels.</summary>
    /// <param name="pixels">The pixels to connect.</param>
    /// <returns>A path through the supplied pixels.</returns>
    SKPath GetPath(IEnumerable<Pixel> pixels);
}
