// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Declares the native UI Automation TableItemProvider ABI.</summary>
[ComVisible(true)]
[Guid("b9734fa6-771f-4d78-9c90-2517999349cd")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowsTableItemProvider
{
    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
    IWindowsRawElementProviderSimple[]
    GetRowHeaderItems();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
    IWindowsRawElementProviderSimple[]
    GetColumnHeaderItems();
}
