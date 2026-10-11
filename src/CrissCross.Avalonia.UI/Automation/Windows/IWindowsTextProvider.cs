// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Declares the native UI Automation TextProvider ABI.</summary>
[ComVisible(true)]
[Guid("3589c92c-63f3-4367-99bb-ada653b77cf2")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowsTextProvider
{
    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
    IWindowsTextRangeProvider[]
    GetSelection();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
    IWindowsTextRangeProvider[]
    GetVisibleRanges();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="childElement">The native childElement argument.</param>
    /// <returns>The native automation result.</returns>
    IWindowsTextRangeProvider RangeFromChild(IWindowsRawElementProviderSimple childElement);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="point">The native point argument.</param>
    /// <returns>The native automation result.</returns>
    IWindowsTextRangeProvider RangeFromPoint(WindowsAutomationPoint point);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    IWindowsTextRangeProvider GetDocumentRange();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    WindowsSupportedTextSelection GetSupportedTextSelection();
}
