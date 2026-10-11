// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Declares the native UI Automation RawElementProviderFragmentRoot ABI.</summary>
[ComVisible(true)]
[Guid("620ce2a5-ab8f-40a9-86cb-de3c75599b58")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowsRawElementProviderFragmentRoot
{
    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="x">The native x argument.</param>
    /// <param name="y">The native y argument.</param>
    /// <returns>The native automation result.</returns>
    IWindowsRawElementProviderFragment? ElementProviderFromPoint(double x, double y);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    IWindowsRawElementProviderFragment? GetFocus();
}
