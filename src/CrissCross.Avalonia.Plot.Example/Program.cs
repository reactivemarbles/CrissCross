// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using ReactiveUI.Avalonia;

namespace CrissCross.Avalonia.Plot.Example;

/// <summary>Starts the desktop plotting example.</summary>
internal static class Program
{
    /// <summary>Configures Avalonia and ReactiveUI.</summary>
    /// <returns>The configured application builder.</returns>
    internal static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithInterFont()
        .LogToTrace()
        .UseReactiveUI(static builder => { });

    /// <summary>Runs the desktop application.</summary>
    /// <param name="args">The command-line arguments.</param>
    [STAThread]
    private static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
}
