// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Rendering.RenderActions;

/// <summary>Applies axis rules before calculating the layout.</summary>
public class ApplyAxisRulesBeforeLayout : IRenderAction
{
    /// <inheritdoc/>
    public void Render(RenderPack rp) => rp.Plot.Axes.Rules.ForEach(x => x.Apply(rp, beforeLayout: true));
}
