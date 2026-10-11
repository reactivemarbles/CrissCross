// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Declares the native UI Automation ExpandCollapseProvider ABI.</summary>
[ComVisible(true)]
[Guid("d847d3a5-cab0-4a98-8c32-ecb45c59ad24")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowsExpandCollapseProvider
{
    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    void Expand();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    void Collapse();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    int GetExpandCollapseState();
}
