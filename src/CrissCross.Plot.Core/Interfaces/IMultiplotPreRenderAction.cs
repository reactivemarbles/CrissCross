// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>
/// An action that may be included in the collection of <see cref="IMultiplot.PreRenderActions"/>
/// which gets invoked at the start of a multiplot render, before any subplots are rendered.
/// Useful for coordinating information across subplots (e.g., shared axes or plottable positions).
/// </summary>
public interface IMultiplotPreRenderAction
{
    /// <summary>Called at the start of a multiplot render, before any of the subplots are rendered.</summary>
    void Invoke();
}
