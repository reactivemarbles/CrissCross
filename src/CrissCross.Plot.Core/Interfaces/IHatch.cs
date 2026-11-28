// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Creates shaders that render a hatch pattern.</summary>
public interface IHatch
{
    /// <summary>Creates a hatch shader for the specified colors and bounds.</summary>
    /// <param name="backgroundColor">The background color.</param>
    /// <param name="hatchColor">The color of the hatch lines.</param>
    /// <param name="rect">The bounds of the hatch pattern.</param>
    /// <returns>The shader that renders the hatch pattern.</returns>
    SKShader GetShader(Color backgroundColor, Color hatchColor, PixelRect rect);
}
