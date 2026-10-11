// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Declares the native UI Automation GridProvider ABI.</summary>
[ComVisible(true)]
[Guid("b17d6187-0907-464b-a168-0ef17a1572b1")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowsGridProvider
{
    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="row">The native row argument.</param>
    /// <param name="column">The native column argument.</param>
    /// <returns>The native automation result.</returns>
    IWindowsRawElementProviderSimple GetItem(int row, int column);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    int GetRowCount();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    int GetColumnCount();
}
