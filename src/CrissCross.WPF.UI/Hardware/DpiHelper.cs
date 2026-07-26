// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Window = System.Windows.Window;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Hardware;
#else
namespace CrissCross.WPF.UI.Hardware;
#endif

/// <summary>Provides access to various DPI-related methods.</summary>
internal static class DpiHelper
{
    /// <summary>Default DPI value.</summary>
    internal const int DefaultDpi = 96;

    /// <summary>Stores the _transformToDevice value.</summary>
    [ThreadStatic]
    private static Matrix _transformToDevice;

    /// <summary>Stores the _transformToDip value.</summary>
    [ThreadStatic]
    private static Matrix _transformToDip;

    /// <summary>Gets DPI of the selected <see cref="Window"/>.</summary>
    /// <param name="window">The window that you want to get information about.</param>
    /// <returns>The result.</returns>
    internal static DisplayDpi GetWindowDpi(Window? window) => window is null
        ? new DisplayDpi(DefaultDpi, DefaultDpi)
        : GetWindowDpi(new WindowInteropHelper(window).Handle);

    /// <summary>Gets DPI of the selected <see cref="Window"/> based on it's handle.</summary>
    /// <param name="windowHandle">Handle of the window that you want to get information about.</param>
    /// <returns>The result.</returns>
    internal static DisplayDpi GetWindowDpi(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero || !UnsafeNativeMethods.IsValidWindow(windowHandle))
        {
            return new(DefaultDpi, DefaultDpi);
        }

        var windowDpi = (int)User32.GetDpiForWindow(windowHandle);

        return new(windowDpi, windowDpi);
    }

    /// <summary>Gets the DPI values when no visual is available.</summary>
    /// <returns>The standard WPF device-independent DPI values.</returns>
    internal static DisplayDpi GetSystemDpi() => new(DefaultDpi, DefaultDpi);

    /// <summary>Convert a point in device independent pixels (1/96") to a point in the system coordinates.</summary>
    /// <param name="logicalPoint">A point in the logical coordinate system.</param>
    /// <param name="dpiScaleX">Horizontal DPI scale.</param>
    /// <param name="dpiScaleY">Vertical DPI scale.</param>
    /// <returns>Returns the parameter converted to the system's coordinates.</returns>
    internal static Point LogicalPixelsToDevice(Point logicalPoint, double dpiScaleX, double dpiScaleY)
    {
        _transformToDevice = Matrix.Identity;
        _transformToDevice.Scale(dpiScaleX, dpiScaleY);

        return _transformToDevice.Transform(logicalPoint);
    }

    /// <summary>Convert a point in system coordinates to a point in device independent pixels (1/96").</summary>
    /// <param name="devicePoint">The devicePoint value.</param>
    /// <param name="dpiScaleX">The dpiScaleX value.</param>
    /// <param name="dpiScaleY">The dpiScaleY value.</param>
    /// <returns>Returns the parameter converted to the device independent coordinate system.</returns>
    internal static Point DevicePixelsToLogical(Point devicePoint, double dpiScaleX, double dpiScaleY)
    {
        _transformToDip = Matrix.Identity;
        _transformToDip.Scale(1D / dpiScaleX, 1D / dpiScaleY);

        return _transformToDip.Transform(devicePoint);
    }

    /// <summary>Provides the LogicalRectToDevice member.</summary>
    /// <param name="logicalRectangle">The logicalRectangle value.</param>
    /// <param name="dpiScaleX">The dpiScaleX value.</param>
    /// <param name="dpiScaleY">The dpiScaleY value.</param>
    /// <returns>The result.</returns>
    internal static Rect LogicalRectToDevice(Rect logicalRectangle, double dpiScaleX, double dpiScaleY)
    {
        var topLeft = LogicalPixelsToDevice(
            new(logicalRectangle.Left, logicalRectangle.Top),
            dpiScaleX,
            dpiScaleY);
        var bottomRight = LogicalPixelsToDevice(
            new(logicalRectangle.Right, logicalRectangle.Bottom),
            dpiScaleX,
            dpiScaleY);

        return new(topLeft, bottomRight);
    }

    /// <summary>Provides the DeviceRectToLogical member.</summary>
    /// <param name="deviceRectangle">The deviceRectangle value.</param>
    /// <param name="dpiScaleX">The dpiScaleX value.</param>
    /// <param name="dpiScaleY">The dpiScaleY value.</param>
    /// <returns>The result.</returns>
    internal static Rect DeviceRectToLogical(Rect deviceRectangle, double dpiScaleX, double dpiScaleY)
    {
        var topLeft = DevicePixelsToLogical(new(deviceRectangle.Left, deviceRectangle.Top), dpiScaleX, dpiScaleY);
        var bottomRight = DevicePixelsToLogical(
            new(deviceRectangle.Right, deviceRectangle.Bottom),
            dpiScaleX,
            dpiScaleY);

        return new(topLeft, bottomRight);
    }

    /// <summary>Provides the LogicalSizeToDevice member.</summary>
    /// <param name="logicalSize">The logicalSize value.</param>
    /// <param name="dpiScaleX">The dpiScaleX value.</param>
    /// <param name="dpiScaleY">The dpiScaleY value.</param>
    /// <returns>The result.</returns>
    internal static Size LogicalSizeToDevice(Size logicalSize, double dpiScaleX, double dpiScaleY)
    {
        var pt = LogicalPixelsToDevice(new(logicalSize.Width, logicalSize.Height), dpiScaleX, dpiScaleY);

        return new Size { Width = pt.X, Height = pt.Y };
    }

    /// <summary>Provides the DeviceSizeToLogical member.</summary>
    /// <param name="deviceSize">The deviceSize value.</param>
    /// <param name="dpiScaleX">The dpiScaleX value.</param>
    /// <param name="dpiScaleY">The dpiScaleY value.</param>
    /// <returns>The result.</returns>
    internal static Size DeviceSizeToLogical(Size deviceSize, double dpiScaleX, double dpiScaleY)
    {
        var pt = DevicePixelsToLogical(new(deviceSize.Width, deviceSize.Height), dpiScaleX, dpiScaleY);

        return new(pt.X, pt.Y);
    }

    /// <summary>Provides the LogicalThicknessToDevice member.</summary>
    /// <param name="logicalThickness">The logicalThickness value.</param>
    /// <param name="dpiScaleX">The dpiScaleX value.</param>
    /// <param name="dpiScaleY">The dpiScaleY value.</param>
    /// <returns>The result.</returns>
    internal static Thickness LogicalThicknessToDevice(Thickness logicalThickness, double dpiScaleX, double dpiScaleY)
    {
        var topLeft = LogicalPixelsToDevice(
            new(logicalThickness.Left, logicalThickness.Top),
            dpiScaleX,
            dpiScaleY);
        var bottomRight = LogicalPixelsToDevice(
            new(logicalThickness.Right, logicalThickness.Bottom),
            dpiScaleX,
            dpiScaleY);

        return new(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
    }
}
