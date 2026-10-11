// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Declares the native UI Automation SelectionItemProvider ABI.</summary>
[ComVisible(true)]
[Guid("2acad808-b2d4-452d-a407-91ff1ad167b2")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowsSelectionItemProvider
{
    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    void Select();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    void AddToSelection();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    void RemoveFromSelection();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    bool ReadIsSelected();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    IWindowsRawElementProviderSimple? GetSelectionContainer();
}
