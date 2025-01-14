// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Hardware;

/// <summary>
/// An <see cref="int"/> value whose high-order word corresponds to the rendering tier for the current thread.
/// <para>Starting in the .NET Framework 4, rendering tier 1 has been redefined to only include graphics hardware that supports DirectX 9.0 or greater. Graphics hardware that supports DirectX 7 or 8 is now defined as rendering tier 0.</para>
/// </summary>
public enum RenderingTier
{
    /// <summary>
    /// No graphics hardware acceleration is available for the application on the device.
    /// All graphics features use software acceleration. The DirectX version level is less than version 9.0.
    /// </summary>
    NoAcceleration = 0x0,

    /// <summary>
    /// Most of the graphics features of WPF will use hardware acceleration
    /// if the necessary system resources are available and have not been exhausted.
    /// This corresponds to a DirectX version that is greater than or equal to 9.0.
    /// </summary>
    PartialAcceleration = 0x1,

    /// <summary>
    /// Most of the graphics features of WPF will use hardware acceleration provided the
    /// necessary system resources have not been exhausted.
    /// This corresponds to a DirectX version that is greater than or equal to 9.0.
    /// </summary>
    FullAcceleration = 0x2
}
