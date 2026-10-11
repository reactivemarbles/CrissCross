// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls.Automation.Windows;
#else
namespace CrissCross.Avalonia.UI.Controls.Automation.Windows;
#endif

/// <summary>Calls the documented Windows UI Automation ABI with explicit COM ownership.</summary>
[SupportedOSPlatform("windows")]
[RequiresDynamicCode("Windows UI Automation uses built-in COM callable wrappers and VARIANT marshaling.")]
[RequiresUnreferencedCode("Built-in COM callable wrappers require public provider interface metadata.")]
internal static class WindowsAutomationNative
{
    /// <summary>The native simple provider interface identifier.</summary>
    private static readonly Guid SimpleProviderId = new("d6dd68d1-86fd-4332-8666-9abedea2d24c");

    /// <summary>Returns the provider for a native accessibility request.</summary>
    /// <param name="hwnd">The window.</param>
    /// <param name="wordParameter">The message word parameter.</param>
    /// <param name="longParameter">The message long parameter.</param>
    /// <param name="provider">The provider.</param>
    /// <returns>The native message result.</returns>
    internal static nint ReturnProvider(nint hwnd, nint wordParameter, nint longParameter, IWindowsRawElementProviderSimple provider)
    {
        var pointer = GetProviderPointer(provider);
        try
        {
            return NativeMethods.UiaReturnRawElementProvider(hwnd, wordParameter, longParameter, pointer);
        }
        finally
        {
            _ = Marshal.Release(pointer);
        }
    }

    /// <summary>Gets the provider associated with the native window.</summary>
    /// <param name="hwnd">The window.</param>
    /// <returns>The host provider.</returns>
    internal static IWindowsRawElementProviderSimple? HostProvider(nint hwnd)
    {
        Marshal.ThrowExceptionForHR(NativeMethods.UiaHostProviderFromHwnd(hwnd, out var pointer));
        if (pointer == 0)
        {
            return null;
        }

        try
        {
            return (IWindowsRawElementProviderSimple)Marshal.GetObjectForIUnknown(pointer);
        }
        finally
        {
            _ = Marshal.Release(pointer);
        }
    }

    /// <summary>Raises a native automation event.</summary>
    /// <param name="provider">The provider.</param>
    /// <param name="eventId">The event identifier.</param>
    internal static void RaiseEvent(IWindowsRawElementProviderSimple provider, int eventId)
    {
        var pointer = GetProviderPointer(provider);
        try
        {
            Marshal.ThrowExceptionForHR(NativeMethods.UiaRaiseAutomationEvent(pointer, eventId));
        }
        finally
        {
            _ = Marshal.Release(pointer);
        }
    }

    /// <summary>Raises a property change with native VARIANT values.</summary>
    /// <param name="provider">The provider.</param>
    /// <param name="propertyId">The property identifier.</param>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The current value.</param>
    internal static unsafe void RaisePropertyChanged(IWindowsRawElementProviderSimple provider, int propertyId, object? oldValue, object? newValue)
    {
        var pointer = GetProviderPointer(provider);
        WindowsAutomationVariant previous = default;
        WindowsAutomationVariant current = default;
        try
        {
            Marshal.GetNativeVariantForObject(oldValue, (nint)(&previous));
            Marshal.GetNativeVariantForObject(newValue, (nint)(&current));
            Marshal.ThrowExceptionForHR(NativeMethods.UiaRaiseAutomationPropertyChangedEvent(pointer, propertyId, previous, current));
        }
        finally
        {
            _ = NativeMethods.VariantClear((nint)(&previous));
            _ = NativeMethods.VariantClear((nint)(&current));
            _ = Marshal.Release(pointer);
        }
    }

    /// <summary>Announces that cached children must be queried again.</summary>
    /// <param name="provider">The provider.</param>
    internal static void RaiseChildrenChanged(IWindowsRawElementProviderSimple provider)
    {
        var pointer = GetProviderPointer(provider);
        try
        {
            Marshal.ThrowExceptionForHR(NativeMethods.UiaRaiseStructureChangedEvent(pointer, WindowsStructureChangeType.ChildrenInvalidated, 0, 0));
        }
        finally
        {
            _ = Marshal.Release(pointer);
        }
    }

    /// <summary>Gets the native mixed text attribute sentinel.</summary>
    /// <returns>The reserved COM value.</returns>
    internal static object GetReservedMixedAttributeValue()
    {
        Marshal.ThrowExceptionForHR(NativeMethods.UiaGetReservedMixedAttributeValue(out var pointer));
        return TakeUnknown(pointer);
    }

    /// <summary>Gets the native unsupported text attribute sentinel.</summary>
    /// <returns>The reserved COM value.</returns>
    internal static object GetReservedNotSupportedValue()
    {
        Marshal.ThrowExceptionForHR(NativeMethods.UiaGetReservedNotSupportedValue(out var pointer));
        return TakeUnknown(pointer);
    }

    /// <summary>Queries the provider's exact native interface.</summary>
    /// <param name="provider">The provider.</param>
    /// <returns>An owned interface reference.</returns>
    private static nint GetProviderPointer(IWindowsRawElementProviderSimple provider)
    {
        var unknown = Marshal.GetIUnknownForObject(provider);
        try
        {
            var id = SimpleProviderId;
            Marshal.ThrowExceptionForHR(Marshal.QueryInterface(unknown, in id, out var pointer));
            return pointer;
        }
        finally
        {
            _ = Marshal.Release(unknown);
        }
    }

    /// <summary>Converts an owned COM reference into a managed wrapper.</summary>
    /// <param name="pointer">The owned COM reference.</param>
    /// <returns>The managed wrapper.</returns>
    private static object TakeUnknown(nint pointer)
    {
        try
        {
            return Marshal.GetObjectForIUnknown(pointer);
        }
        finally
        {
            _ = Marshal.Release(pointer);
        }
    }
}
