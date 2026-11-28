// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Records benchmark data for a plot render.</summary>
public class RenderBenchmark : IRenderAction
{
    /// <inheritdoc/>
    public void Render(RenderPack rp) => rp.Plot.Benchmark.Render(rp);
}
