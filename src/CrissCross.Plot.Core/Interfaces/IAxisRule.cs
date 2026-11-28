// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>These rules are applied just before each render.</summary>
public interface IAxisRule
{
    /// <summary>Applies this rule before or after plot layout.</summary>
    /// <param name="rp">The render pack containing the plot state.</param>
    /// <param name="beforeLayout">Whether the rule runs before layout.</param>
    void Apply(RenderPack rp, bool beforeLayout);
}
