// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Win32;
#else
namespace CrissCross.WPF.UI.Win32;
#endif

/// <summary>Common Window utilities.</summary>
internal static class Utilities
{
    /// <summary>Provides the first Windows 10 build number.</summary>
    private const int Windows10InitialBuild = 10_240;

    /// <summary>Provides the first Windows 11 build number.</summary>
    private const int Windows11InitialBuild = 22_000;

    /// <summary>Provides Windows 11 Insider build 22523.</summary>
    private const int Windows11InsiderBuild22523 = 22_523;

    /// <summary>Provides Windows 11 Insider build 22557.</summary>
    private const int Windows11InsiderBuild22557 = 22_557;

#if !NET5_0_OR_GREATER
    /// <summary>The Windows registry path that stores the current OS version.</summary>
    private const string CurrentVersionRegistryPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
#endif

    /// <summary>Stores the _platform value.</summary>
    private static readonly PlatformID _platform = Environment.OSVersion.Platform;

    /// <summary>Stores the _version value.</summary>
    private static readonly Version _version =
#if NET5_0_OR_GREATER
    Environment.OSVersion.Version;
#else
    GetOSVersionFromRegistry();
#endif

    /// <summary>Gets a value indicating whether the operating system is NT or newer.</summary>
    public static bool IsNT => _platform == PlatformID.Win32NT;

    /// <summary>Gets a value indicating whether the operating system version is greater than or equal to 6.0.</summary>
    public static bool IsOSVistaOrNewer => new Version(6, 0) <= _version;

    /// <summary>Gets a value indicating whether the operating system version is greater than or equal to 6.1.</summary>
    public static bool IsOSWindows7OrNewer => new Version(6, 1) <= _version;

    /// <summary>Gets a value indicating whether the operating system version is greater than or equal to 6.2.</summary>
    public static bool IsOSWindows8OrNewer => new Version(6, 2) <= _version;

    /// <summary>Gets whether the operating system build is Windows 10 or newer.</summary>
    public static bool IsOSWindows10OrNewer => _version.Build >= Windows10InitialBuild;

    /// <summary>Gets whether the operating system build is Windows 11 or newer.</summary>
    public static bool IsOSWindows11OrNewer => _version.Build >= Windows11InitialBuild;

    /// <summary>Gets whether the OS includes Windows 11 Insider build 22523.</summary>
    public static bool IsOSWindows11Insider1OrNewer => _version.Build >= Windows11InsiderBuild22523;

    /// <summary>Gets whether the OS includes Windows 11 Insider build 22557.</summary>
    public static bool IsOSWindows11Insider2OrNewer => _version.Build >= Windows11InsiderBuild22557;

    /// <summary>Gets whether Desktop Window Manager composition is enabled.</summary>
    public static bool IsCompositionEnabled
    {
        get
        {
            if (!IsOSVistaOrNewer)
            {
                return false;
            }

            _ = Dwmapi.DwmIsCompositionEnabled(out var pfEnabled);

            return pfEnabled != 0;
        }
    }

    /// <summary>Provides the SafeDispose member.</summary>
    /// <param name="disposable">The disposable value.</param>
    /// <typeparam name="T">The type.</typeparam>
    public static void SafeDispose<T>(ref T disposable)
        where T : IDisposable
    {
        // Dispose can safely be called on an object multiple times.
        IDisposable t = disposable;
        disposable = default!;

        if (t is null)
        {
            return;
        }

        t.Dispose();
    }

    /// <summary>Provides the SafeRelease member.</summary>
    /// <param name="comObject">The comObject value.</param>
    /// <typeparam name="T">The type.</typeparam>
    public static void SafeRelease<T>(ref T comObject)
        where T : class
    {
        var t = comObject;
        comObject = default!;

        if (t is null)
        {
            return;
        }

        Debug.Assert(Marshal.IsComObject(t), "Safe Release");
        _ = Marshal.ReleaseComObject(t);
    }

#if !NET5_0_OR_GREATER
    /// <summary>Tries to get the OS version from the Windows registry.</summary>
    /// <returns>The operating system version from the registry.</returns>
    private static Version GetOSVersionFromRegistry()
    {
        var major = GetRegistryVersionPart("CurrentMajorVersionNumber", 0);
        var minor = GetRegistryVersionPart("CurrentMinorVersionNumber", 1);
        var build = GetRegistryStringValueAsInt("CurrentBuildNumber");

        return new(major, minor, build);
    }

    /// <summary>Gets a registry version part with a fallback to the legacy CurrentVersion string.</summary>
    /// <param name="key">The registry key.</param>
    /// <param name="fallbackIndex">The legacy CurrentVersion segment index.</param>
    /// <returns>The parsed version part, or 0.</returns>
    private static int GetRegistryVersionPart(string key, int fallbackIndex) =>
        TryGetRegistryKey(CurrentVersionRegistryPath, key, out var value) && value is int versionPart
            ? versionPart
            : GetCurrentVersionPart(fallbackIndex);

    /// <summary>Gets a legacy CurrentVersion string segment as an integer.</summary>
    /// <param name="index">The segment index.</param>
    /// <returns>The parsed version segment, or 0.</returns>
    private static int GetCurrentVersionPart(int index)
    {
        if (
            !TryGetRegistryKey(CurrentVersionRegistryPath, "CurrentVersion", out var value)
            || value is not string version)
        {
            return 0;
        }

        var versionParts = version.Split('.');
        return versionParts.Length > index && int.TryParse(versionParts[index], out var versionPart) ? versionPart : 0;
    }

    /// <summary>Gets a registry string value as an integer.</summary>
    /// <param name="key">The registry key.</param>
    /// <returns>The parsed integer value, or 0.</returns>
    private static int GetRegistryStringValueAsInt(string key)
    {
        if (!TryGetRegistryKey(CurrentVersionRegistryPath, key, out var value) || value is not string text)
        {
            return 0;
        }

        return int.TryParse(text, out var result) ? result : 0;
    }

    /// <summary>Tries to get a registry key value.</summary>
    /// <param name="path">The registry path.</param>
    /// <param name="key">The registry key.</param>
    /// <param name="value">The registry value.</param>
    /// <returns><c>true</c> if the registry value was found; otherwise, <c>false</c>.</returns>
    private static bool TryGetRegistryKey(string path, string key, out object? value)
    {
        value = null;

        try
        {
            using var rk = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(path);

            if (rk is null)
            {
                return false;
            }

            value = rk.GetValue(key);

            return value is not null;
        }
        catch
        {
            return false;
        }
    }

#endif
}
