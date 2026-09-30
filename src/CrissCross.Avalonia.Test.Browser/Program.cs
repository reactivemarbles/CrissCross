// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using CrissCross.Avalonia.Test;
using ReactiveUI.Avalonia;

[assembly: SupportedOSPlatform("browser")]

namespace CrissCross.Avalonia.Test.Browser;

/// <summary>Provides the browser application entry point.</summary>
internal static class Program
{
    /// <summary>Avalonia configuration, don't remove; also used by visual designer.</summary>
    /// <returns>The configured Avalonia application builder.</returns>
    internal static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>();

    /// <summary>Starts the browser application.</summary>
    /// <returns>A task that represents the asynchronous startup operation.</returns>
    private static Task Main() => BuildAvaloniaApp()
            .WithInterFont()
            .UseReactiveUI(static b => { })
            .StartBrowserAppAsync("out");
}
