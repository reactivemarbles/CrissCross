// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Declares the native UI Automation RawElementProviderSimple ABI.</summary>
[ComVisible(true)]
[Guid("d6dd68d1-86fd-4332-8666-9abedea2d24c")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IWindowsRawElementProviderSimple
{
    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    int GetProviderOptions();

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="patternId">The native patternId argument.</param>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.IUnknown)]
    object? GetPatternProvider(int patternId);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <param name="propertyId">The native propertyId argument.</param>
    /// <returns>The native automation result.</returns>
    [return: MarshalAs(UnmanagedType.Struct)]
    object? GetPropertyValue(int propertyId);

    /// <summary>Exposes the corresponding native UI Automation operation.</summary>
    /// <returns>The native automation result.</returns>
    IWindowsRawElementProviderSimple? GetHostRawElementProvider();
}
