// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Applies the chosen backdrop effect to the selected window.</summary>
public static class WindowBackdrop
{
    /// <summary>Checks whether the selected backdrop type is supported on current platform.</summary>
    /// <param name="backdropType">Type of the backdrop.</param>
    /// <returns>
    ///   <see langword="true" /> if the selected backdrop type is supported on current platform.
    /// </returns>
    public static bool IsSupported(WindowBackdropType backdropType) => backdropType switch
    {
        WindowBackdropType.Auto => Win32.Utilities.IsOSWindows11Insider1OrNewer,
        WindowBackdropType.Tabbed => Win32.Utilities.IsOSWindows11Insider1OrNewer,
        WindowBackdropType.Mica => Win32.Utilities.IsOSWindows11OrNewer,
        WindowBackdropType.Acrylic => Win32.Utilities.IsOSWindows7OrNewer,
        WindowBackdropType.None => true,
        _ => false
    };

    /// <summary>Applies backdrop effect to the selected <see cref="System.Windows.Window" />.</summary>
    /// <param name="window">Selected window.</param>
    /// <param name="backdropType">Type of the backdrop.</param>
    /// <returns>
    ///   <see langword="true" /> if the operation was successfull, otherwise <see langword="false" />.
    /// </returns>
    public static bool ApplyBackdrop(System.Windows.Window? window, WindowBackdropType backdropType)
    {
        if (window is null)
        {
            return false;
        }

        if (window.IsLoaded)
        {
            var windowHandle = new WindowInteropHelper(window).Handle;

            return windowHandle == IntPtr.Zero ? false : ApplyBackdrop(windowHandle, backdropType);
        }

        window.Loaded += (sender, eventArgs) =>
        {
            var windowHandle = new WindowInteropHelper(sender as System.Windows.Window)?.Handle ?? IntPtr.Zero;

            if (windowHandle == IntPtr.Zero)
            {
                return;
            }

            _ = ApplyBackdrop(windowHandle, backdropType);
        };

        return true;
    }

    /// <summary>Applies backdrop effect to the selected handle.</summary>
    /// <param name="windowHandle">Window handle.</param>
    /// <param name="backdropType">Type of the backdrop.</param>
    /// <returns>
    ///   <see langword="true" /> if the operation was successfull, otherwise <see langword="false" />.
    /// </returns>
    public static bool ApplyBackdrop(IntPtr windowHandle, WindowBackdropType backdropType)
    {
        if (windowHandle == IntPtr.Zero)
        {
            return false;
        }

        if (!User32.IsWindow(windowHandle))
        {
            return false;
        }

        if (ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark)
        {
            _ = UnsafeNativeMethods.ApplyWindowDarkMode(windowHandle);
        }
        else
        {
            _ = UnsafeNativeMethods.RemoveWindowDarkMode(windowHandle);
        }

        _ = UnsafeNativeMethods.RemoveWindowCaption(windowHandle);

        // 22H1
        if (!Win32.Utilities.IsOSWindows11Insider1OrNewer)
        {
            return backdropType != WindowBackdropType.None ? ApplyLegacyMicaBackdrop(windowHandle) : false;
        }

        return backdropType switch
        {
            WindowBackdropType.Auto => ApplyDwmwWindowAttrubute(windowHandle, Dwmapi.DWMSBT.DWMSBT_AUTO),
            WindowBackdropType.Mica => ApplyDwmwWindowAttrubute(windowHandle, Dwmapi.DWMSBT.DWMSBT_MAINWINDOW),
            WindowBackdropType.Acrylic => ApplyDwmwWindowAttrubute(windowHandle, Dwmapi.DWMSBT.DWMSBT_TRANSIENTWINDOW),
            WindowBackdropType.Tabbed => ApplyDwmwWindowAttrubute(windowHandle, Dwmapi.DWMSBT.DWMSBT_TABBEDWINDOW),
            _ => ApplyDwmwWindowAttrubute(windowHandle, Dwmapi.DWMSBT.DWMSBT_DISABLE),
        };
    }

    /// <summary>Tries to remove backdrop effects if they have been applied to the <see cref="Window" />.</summary>
    /// <param name="window">The window from which the effect should be removed.</param>
    /// <returns>A bool.</returns>
    public static bool RemoveBackdrop(System.Windows.Window? window)
    {
        if (window is null)
        {
            return false;
        }

        var windowHandle = new WindowInteropHelper(window).Handle;

        return RemoveBackdrop(windowHandle);
    }

    /// <summary>Tries to remove all effects if they have been applied to the window handle.</summary>
    /// <param name="windowHandle">Pointer to the window handle.</param>
    /// <returns>A bool.</returns>
    public static bool RemoveBackdrop(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
        {
            return false;
        }

        _ = RestoreContentBackground(windowHandle);

        if (windowHandle == IntPtr.Zero)
        {
            return false;
        }

        if (!User32.IsWindow(windowHandle))
        {
            return false;
        }

        var attributePointer = 0; // Disable
        var backdropPvAttribute = (int)Dwmapi.DWMSBT.DWMSBT_DISABLE;

        _ = Dwmapi.DwmSetWindowAttribute(
            windowHandle,
            Dwmapi.DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT,
            ref attributePointer,
            Marshal.SizeOf<int>());

        _ = Dwmapi.DwmSetWindowAttribute(
            windowHandle,
            Dwmapi.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
            ref backdropPvAttribute,
            Marshal.SizeOf<int>());

        return true;
    }

    /// <summary>Tries to remove background from <see cref="Window"/> and it's composition area.</summary>
    /// <param name="window">Window to manipulate.</param>
    /// <returns><see langword="true"/> if operation was successful.</returns>
    public static bool RemoveBackground(System.Windows.Window? window)
    {
        if (window is null)
        {
            return false;
        }

        // Remove background from visual root
        window.SetCurrentValue(System.Windows.Controls.Control.BackgroundProperty, Brushes.Transparent);

        var windowHandle = new WindowInteropHelper(window).Handle;

        if (windowHandle == IntPtr.Zero)
        {
            return false;
        }

        var windowSource = HwndSource.FromHwnd(windowHandle);

        // Remove background from client area
        if (windowSource?.Handle == IntPtr.Zero || windowSource?.CompositionTarget is null)
        {
            return true;
        }

        windowSource.CompositionTarget.BackgroundColor = Colors.Transparent;

        return true;
    }

    /// <summary>Removes the titlebar background.</summary>
    /// <param name="window">The window.</param>
    /// <returns>A bool.</returns>
    public static bool RemoveTitlebarBackground(System.Windows.Window? window)
    {
        if (window is null)
        {
            return false;
        }

        var windowHandle = new WindowInteropHelper(window).Handle;

        if (windowHandle == IntPtr.Zero)
        {
            return false;
        }

        var windowSource = HwndSource.FromHwnd(windowHandle);

        // Remove background from client area
        if (windowSource?.Handle == IntPtr.Zero || windowSource?.CompositionTarget is null)
        {
            return true;
        }

        // NOTE: https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute
        // Specifying DWMWA_COLOR_DEFAULT (value 0xFFFFFFFF) for the color will reset the window back to using the system's default behavior for the caption color.
        var titlebarPvAttribute = 0xFFFFFFFEu;

        _ = Dwmapi.DwmSetWindowAttribute(
            windowSource.Handle,
            Dwmapi.DWMWINDOWATTRIBUTE.DWMWA_CAPTION_COLOR,
            ref titlebarPvAttribute,
            Marshal.SizeOf<uint>());

        return true;
    }

    /// <summary>Provides the ApplyDwmwWindowAttrubute member.</summary>
    /// <param name="windowHandle">The window handle.</param>
    /// <param name="dwmSbt">The dwmSbt value.</param>
    /// <returns>The result.</returns>
    private static bool ApplyDwmwWindowAttrubute(IntPtr windowHandle, Dwmapi.DWMSBT dwmSbt)
    {
        if (windowHandle == IntPtr.Zero)
        {
            return false;
        }

        if (!User32.IsWindow(windowHandle))
        {
            return false;
        }

        var backdropPvAttribute = (int)dwmSbt;

        var dwmApiResult = Dwmapi.DwmSetWindowAttribute(
            windowHandle,
            Dwmapi.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
            ref backdropPvAttribute,
            Marshal.SizeOf<int>());

        return dwmApiResult == HRESULT.S_OK;
    }

    /// <summary>Provides the ApplyLegacyMicaBackdrop member.</summary>
    /// <param name="windowHandle">The window handle.</param>
    /// <returns>The result.</returns>
    private static bool ApplyLegacyMicaBackdrop(IntPtr windowHandle)
    {
        var backdropPvAttribute = 1; // Enable

        // TODO: Validate HRESULT
        var dwmApiResult = Dwmapi.DwmSetWindowAttribute(
            windowHandle,
            Dwmapi.DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT,
            ref backdropPvAttribute,
            Marshal.SizeOf<int>());

        return dwmApiResult == HRESULT.S_OK;
    }

    /// <summary>Provides the RestoreContentBackground member.</summary>
    /// <param name="windowHandle">The window handle.</param>
    /// <returns>The result.</returns>
    private static bool RestoreContentBackground(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
        {
            return false;
        }

        if (!User32.IsWindow(windowHandle))
        {
            return false;
        }

        var windowSource = HwndSource.FromHwnd(windowHandle);

        // Restore client area background
        if (windowSource?.Handle != IntPtr.Zero && windowSource?.CompositionTarget is not null)
        {
            windowSource.CompositionTarget.BackgroundColor = SystemColors.WindowColor;
        }

        if (windowSource?.RootVisual is not System.Windows.Window window)
        {
            return true;
        }

        var backgroundBrush = window.Resources["ApplicationBackgroundBrush"];

        // Manual fallback
        if (backgroundBrush is not SolidColorBrush)
        {
            backgroundBrush = GetFallbackBackgroundBrush();
        }

        window.Background = (SolidColorBrush)backgroundBrush;

        return true;
    }

    /// <summary>Provides the GetFallbackBackgroundBrush member.</summary>
    /// <returns>The result.</returns>
    private static SolidColorBrush GetFallbackBackgroundBrush() => ApplicationThemeManager.GetAppTheme() switch
    {
        ApplicationTheme.HighContrast => ApplicationThemeManager.GetSystemTheme() switch
        {
            SystemTheme.HC1 => new SolidColorBrush(Color.FromArgb(0xFF, 0x2D, 0x32, 0x36)),
            SystemTheme.HC2 => new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x00)),
            SystemTheme.HCBlack => new SolidColorBrush(Color.FromArgb(0xFF, 0x20, 0x20, 0x20)),
            _ => new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFA, 0xEF)),
        },
        ApplicationTheme.Dark => new SolidColorBrush(Color.FromArgb(0xFF, 0x20, 0x20, 0x20)),
        _ => new SolidColorBrush(Color.FromArgb(0xFF, 0xFA, 0xFA, 0xFA))
    };
}
