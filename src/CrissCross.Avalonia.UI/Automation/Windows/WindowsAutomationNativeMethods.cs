// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Declares blittable native UI Automation entry points.</summary>
[SupportedOSPlatform("windows")]
internal static partial class NativeMethods
{
    /// <summary>Returns a provider for a native accessibility request.</summary>
    /// <param name="hwnd">The window.</param>
    /// <param name="wordParameter">The word parameter.</param>
    /// <param name="longParameter">The long parameter.</param>
    /// <param name="provider">The native provider pointer.</param>
    /// <returns>The message result.</returns>
    [LibraryImport("UIAutomationCore.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial nint UiaReturnRawElementProvider(nint hwnd, nint wordParameter, nint longParameter, nint provider);

    /// <summary>Gets the native host provider.</summary>
    /// <param name="hwnd">The window.</param>
    /// <param name="provider">The returned provider.</param>
    /// <returns>The HRESULT.</returns>
    [LibraryImport("UIAutomationCore.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial int UiaHostProviderFromHwnd(nint hwnd, out nint provider);

    /// <summary>Raises an automation event.</summary>
    /// <param name="provider">The provider.</param>
    /// <param name="eventId">The event identifier.</param>
    /// <returns>The HRESULT.</returns>
    [LibraryImport("UIAutomationCore.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial int UiaRaiseAutomationEvent(nint provider, int eventId);

    /// <summary>Raises a property event.</summary>
    /// <param name="provider">The provider.</param>
    /// <param name="propertyId">The property identifier.</param>
    /// <param name="oldValue">The old native VARIANT.</param>
    /// <param name="newValue">The new native VARIANT.</param>
    /// <returns>The HRESULT.</returns>
    [LibraryImport("UIAutomationCore.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial int UiaRaiseAutomationPropertyChangedEvent(
        nint provider,
        int propertyId,
        WindowsAutomationVariant oldValue,
        WindowsAutomationVariant newValue);

    /// <summary>Raises a structure event.</summary>
    /// <param name="provider">The provider.</param>
    /// <param name="changeType">The structure change.</param>
    /// <param name="runtimeId">The runtime identifier pointer.</param>
    /// <param name="runtimeIdLength">The runtime identifier length.</param>
    /// <returns>The HRESULT.</returns>
    [LibraryImport("UIAutomationCore.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial int UiaRaiseStructureChangedEvent(nint provider, WindowsStructureChangeType changeType, nint runtimeId, int runtimeIdLength);

    /// <summary>Gets the reserved mixed attribute value.</summary>
    /// <param name="value">The owned COM value.</param>
    /// <returns>The HRESULT.</returns>
    [LibraryImport("UIAutomationCore.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial int UiaGetReservedMixedAttributeValue(out nint value);

    /// <summary>Gets the reserved unsupported attribute value.</summary>
    /// <param name="value">The owned COM value.</param>
    /// <returns>The HRESULT.</returns>
    [LibraryImport("UIAutomationCore.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial int UiaGetReservedNotSupportedValue(out nint value);

    /// <summary>Releases a native VARIANT's owned contents.</summary>
    /// <param name="value">The VARIANT address.</param>
    /// <returns>The HRESULT.</returns>
    [LibraryImport("oleaut32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static partial int VariantClear(nint value);
}
