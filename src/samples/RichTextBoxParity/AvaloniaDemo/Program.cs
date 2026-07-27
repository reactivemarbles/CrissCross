// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Avalonia;
using ReactiveUI.Avalonia;

namespace RichTextBoxParity.AvaloniaDemo;

/// <summary>Program entry point for the Avalonia RichTextBox parity demo.</summary>
internal static class Program
{
    /// <summary>Runs the desktop application.</summary>
    /// <param name="args">Command-line arguments.</param>
    [STAThread]
    internal static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    /// <summary>Builds the Avalonia application.</summary>
    /// <returns>The configured application builder.</returns>
    internal static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI(static _ => { });
}
