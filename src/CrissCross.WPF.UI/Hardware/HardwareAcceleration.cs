// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Hardware;

/// <summary>
/// Set of tools for hardware acceleration.
/// </summary>
public static class HardwareAcceleration
{
    /// <summary>
    /// Determines whether the provided rendering tier is supported.
    /// </summary>
    /// <param name="tier">Hardware acceleration rendering tier to check.</param>
    /// <returns><see langword="true"/> if tier is supported.</returns>
    public static bool IsSupported(RenderingTier tier) => RenderCapability.Tier >> 16 >= (int)tier;
}
