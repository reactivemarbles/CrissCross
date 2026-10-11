// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Declares the native UI Automation RawElementProviderFragment ABI.</summary>
[ComVisible(true)]
[Guid("f7063da8-8359-439c-9297-bbc5299a7d87")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowsRawElementProviderFragment
{
    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="direction">The native direction argument.</param>
    /// <returns>The native automation result.</returns>
    IWindowsRawElementProviderFragment? Navigate(WindowsNavigateDirection direction);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)]
    int[]
    GetRuntimeId();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    WindowsAutomationRect GetBoundingRectangle();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
    IWindowsRawElementProviderFragmentRoot[]
    GetEmbeddedFragmentRoots();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    void SetFocus();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    IWindowsRawElementProviderFragmentRoot GetFragmentRoot();
}
