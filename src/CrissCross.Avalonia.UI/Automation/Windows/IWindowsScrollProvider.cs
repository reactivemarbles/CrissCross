// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Declares the native UI Automation ScrollProvider ABI.</summary>
[ComVisible(true)]
[Guid("b38b8077-1fc3-42a5-8cae-d40c2215055a")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowsScrollProvider
{
    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="horizontalAmount">The native horizontalAmount argument.</param>
    /// <param name="verticalAmount">The native verticalAmount argument.</param>
    void Scroll(int horizontalAmount, int verticalAmount);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="horizontalPercent">The native horizontalPercent argument.</param>
    /// <param name="verticalPercent">The native verticalPercent argument.</param>
    void SetScrollPercent(double horizontalPercent, double verticalPercent);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    double GetHorizontalScrollPercent();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    double GetVerticalScrollPercent();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    double GetHorizontalViewSize();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    double GetVerticalViewSize();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    bool ReadHorizontallyScrollable();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    bool ReadVerticallyScrollable();
}
