// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Declares the native UI Automation RangeValueProvider ABI.</summary>
[ComVisible(true)]
[Guid("36dc7aef-33e6-4691-afe1-2be7274b3d33")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowsRangeValueProvider
{
    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="value">The native value argument.</param>
    void SetValue(double value);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    double ReadValue();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    bool ReadIsReadOnly();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    double GetMaximum();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    double GetMinimum();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    double GetLargeChange();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    double GetSmallChange();
}
