// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Classes with a <see cref="LineStyle"/> can implement this to guide addition of standard shortcuts to its most commonly used properties.</summary>
public interface IHasLine
{
    /// <summary>Gets or sets the complete line style.</summary>
    LineStyle LineStyle { get; set; }

    /// <summary>Gets or sets the line width.</summary>
    float LineWidth { get; set; }

    /// <summary>Gets or sets the line pattern.</summary>
    LinePattern LinePattern { get; set; }

    /// <summary>Gets or sets the line color.</summary>
    Color LineColor { get; set; }
}
