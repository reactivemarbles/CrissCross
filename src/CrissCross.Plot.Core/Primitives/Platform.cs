// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides platform-specific helpers for launching files and web browsers.</summary>
public static class Platform
{
    /// <summary>The operating system detected when this type is initialized.</summary>
#if NETFRAMEWORK
    private const OS ThisOS = OS.Windows;
#else
    private static readonly OS ThisOS = GetOS();
#endif

    /// <summary>Identifies the operating systems supported by this helper.</summary>
    private enum OS
    {
        /// <summary>Microsoft Windows operating system.</summary>
        Windows,

        /// <summary>Linux operating system.</summary>
        Linux,

        /// <summary>Apple macOS operating system.</summary>
        MacOS,

        /// <summary>An operating system not explicitly supported by this helper.</summary>
        Other,
    }

    /// <summary>Launches a web browser to a URL using a command appropriate for the operating system.</summary>
    /// <param name="url">The URL to open.</param>
    public static void LaunchWebBrowser(string url)
    {
        ProcessStartInfo startInfo = ThisOS switch
        {
            OS.Windows => new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true },
            OS.Linux => new ProcessStartInfo("xdg-open", url),
            OS.MacOS => new ProcessStartInfo("open", url),
            _ => throw new InvalidOperationException("Cannot launch a web browser on this OS"),
        };
        _ = Process.Start(startInfo);
    }

    /// <summary>Launches the specified file using the operating system's default application.</summary>
    /// <param name="filePath">The path to the file to launch.</param>
    public static void LaunchFile(string filePath)
    {
        filePath = Path.GetFullPath(filePath);
        ProcessStartInfo psi = new(filePath) { UseShellExecute = true };
        _ = Process.Start(psi);
    }

    #if !NETFRAMEWORK
    /// <summary>Detects the current operating system.</summary>
    /// <returns>The detected operating system.</returns>
    private static OS GetOS()
    {
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
        {
            return OS.Windows;
        }

        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
        {
            return OS.Linux;
        }

        return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX) ? OS.MacOS : OS.Other;
    }
#endif
}
