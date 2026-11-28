// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Defines background fill and hatch styling for a plot element.</summary>
public interface IHasBackground
{
    /// <summary>Gets the complete background fill style.</summary>
    FillStyle BackgroundFillStyle { get; }

    /// <summary>Gets or sets the background color.</summary>
    Color BackgroundColor { get; set; }

    /// <summary>Gets or sets the background hatch color.</summary>
    Color BackgroundHatchColor { get; set; }

    /// <summary>Gets or sets the background hatch pattern.</summary>
    IHatch? BackgroundHatch { get; set; }
}
