// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using CrissCross.Avalonia.Test;
using ReactiveUI.Avalonia;

[assembly: SupportedOSPlatform("browser")]

/// <summary>Provides the browser application entry point.</summary>
internal static class Program
{
    /// <summary>Avalonia configuration, don't remove; also used by visual designer.</summary>
    /// <returns>The configured Avalonia application builder.</returns>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();

    /// <summary>Starts the browser application.</summary>
    /// <param name="args">The command-line arguments passed to the application.</param>
    /// <returns>A task that represents the asynchronous startup operation.</returns>
    public static async Task Main(string[] args) => await BuildAvaloniaApp()
            .WithInterFont()
            .UseReactiveUI(b => { })
            .StartBrowserAppAsync("out");
}
