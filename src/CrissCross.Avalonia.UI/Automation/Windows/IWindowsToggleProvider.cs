// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Declares the native UI Automation ToggleProvider ABI.</summary>
[ComVisible(true)]
[Guid("56d00bd0-c4f4-433c-a836-1a52a57e0892")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowsToggleProvider
{
    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    void Toggle();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    int GetToggleState();
}
