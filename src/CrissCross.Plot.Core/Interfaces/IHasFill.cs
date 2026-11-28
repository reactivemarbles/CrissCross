// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Defines fill and hatch styling for a plot element.</summary>
public interface IHasFill
{
    /// <summary>Gets or sets the complete fill style.</summary>
    FillStyle FillStyle { get; set; }

    /// <summary>Gets or sets the fill color.</summary>
    Color FillColor { get; set; }

    /// <summary>Gets or sets the fill hatch color.</summary>
    Color FillHatchColor { get; set; }

    /// <summary>Gets or sets the fill hatch pattern.</summary>
    IHatch? FillHatch { get; set; }
}
