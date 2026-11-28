// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Defines outline styling for a plot element.</summary>
public interface IHasOutline
{
    /// <summary>Gets the complete outline style.</summary>
    LineStyle OutlineStyle { get; }

    /// <summary>Gets or sets the outline width.</summary>
    float OutlineWidth { get; set; }

    /// <summary>Gets or sets the outline pattern.</summary>
    LinePattern OutlinePattern { get; set; }

    /// <summary>Gets or sets the outline color.</summary>
    Color OutlineColor { get; set; }
}
