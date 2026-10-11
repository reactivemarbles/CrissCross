// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Represents native UI Automation geometry with sequential doubles.</summary>
/// <param name="Left">The left coordinate.</param>
/// <param name="Top">The top coordinate.</param>
/// <param name="Width">The width coordinate.</param>
/// <param name="Height">The height coordinate.</param>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct WindowsAutomationRect(double Left, double Top, double Width, double Height)
{
    /// <summary>Gets the left coordinate.</summary>
    public double X => Left;

    /// <summary>Gets the top coordinate.</summary>
    public double Y => Top;
}
