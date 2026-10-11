// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Declares the native UI Automation TextRangeProvider ABI.</summary>
[ComVisible(true)]
[Guid("5347ad7b-c355-46f8-aff5-909033582f63")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowsTextRangeProvider
{
    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    IWindowsTextRangeProvider Clone();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="range">The native range argument.</param>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    bool Compare(IWindowsTextRangeProvider range);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="endpoint">The native endpoint argument.</param>
    /// <param name="targetRange">The native targetRange argument.</param>
    /// <param name="targetEndpoint">The native targetEndpoint argument.</param>
    /// <returns>The native automation result.</returns>
    int CompareEndpoints(WindowsTextPatternRangeEndpoint endpoint, IWindowsTextRangeProvider targetRange, WindowsTextPatternRangeEndpoint targetEndpoint);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="unit">The native unit argument.</param>
    void ExpandToEnclosingUnit(WindowsTextUnit unit);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="attributeId">The native attributeId argument.</param>
    /// <param name="value">The native value argument.</param>
    /// <param name="backward">The native backward argument.</param>
    /// <returns>The native automation result.</returns>
    IWindowsTextRangeProvider? FindAttribute(
        int attributeId,
        [MarshalAs(UnmanagedType.Struct)] object value,
        [MarshalAs(UnmanagedType.Bool)] bool backward);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="text">The native text argument.</param>
    /// <param name="backward">The native backward argument.</param>
    /// <param name="ignoreCase">The native ignoreCase argument.</param>
    /// <returns>The native automation result.</returns>
    IWindowsTextRangeProvider? FindText(
        [MarshalAs(UnmanagedType.BStr)] string text,
        [MarshalAs(UnmanagedType.Bool)] bool backward,
        [MarshalAs(UnmanagedType.Bool)] bool ignoreCase);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="attributeId">The native attributeId argument.</param>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.Struct)]
    object GetAttributeValue(int attributeId);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_R8)]
    double[]
    GetBoundingRectangles();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    IWindowsRawElementProviderSimple GetEnclosingElement();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="maxLength">The native maxLength argument.</param>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.BStr)]
    string GetText(int maxLength);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="unit">The native unit argument.</param>
    /// <param name="count">The native count argument.</param>
    /// <returns>The native automation result.</returns>
    int Move(WindowsTextUnit unit, int count);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="endpoint">The native endpoint argument.</param>
    /// <param name="unit">The native unit argument.</param>
    /// <param name="count">The native count argument.</param>
    /// <returns>The native automation result.</returns>
    int MoveEndpointByUnit(WindowsTextPatternRangeEndpoint endpoint, WindowsTextUnit unit, int count);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="endpoint">The native endpoint argument.</param>
    /// <param name="targetRange">The native targetRange argument.</param>
    /// <param name="targetEndpoint">The native targetEndpoint argument.</param>
    void MoveEndpointByRange(WindowsTextPatternRangeEndpoint endpoint, IWindowsTextRangeProvider targetRange, WindowsTextPatternRangeEndpoint targetEndpoint);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    void Select();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    void AddToSelection();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    void RemoveFromSelection();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="alignToTop">The native alignToTop argument.</param>
    void ScrollIntoView([MarshalAs(UnmanagedType.Bool)]
    bool alignToTop);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
    IWindowsRawElementProviderSimple[]
    GetChildren();
}
