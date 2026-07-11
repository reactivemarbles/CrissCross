// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
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
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    /// <summary>Builds the Avalonia application.</summary>
    /// <returns>The configured application builder.</returns>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI(b => { });
}
