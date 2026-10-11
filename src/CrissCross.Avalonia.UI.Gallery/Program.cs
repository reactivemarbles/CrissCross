// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Avalonia;
using CrissCross.Avalonia.UI.Controls;
using CrissCross.Avalonia.UI.Gallery.Views;
using ReactiveUI.Avalonia;

namespace CrissCross.Avalonia.UI.Gallery;

/// <summary>Program entry point.</summary>
internal static class Program
{
    /// <summary>
    /// Initialization code. Don't use any Avalonia, third-party APIs or any
    /// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    /// yet and stuff might break.
    /// </summary>
    /// <param name="args">The arguments.</param>
    [STAThread]
    [RequiresUnreferencedCode("The optional native rich text automation verification mode uses classic COM provider metadata.")]
    internal static void Main(string[] args)
    {
        var applicationArguments = new List<string>();
        foreach (var argument in args)
        {
            if (argument == "--native-richtext-automation")
            {
                MainWindow.WindowsAutomationFactory = static window =>
                {
                    if (!OperatingSystem.IsWindows() || !RuntimeFeature.IsDynamicCodeSupported)
                    {
                        throw new NotSupportedException("Native rich text automation verification requires Windows and runtime COM support.");
                    }

                    return RichTextWindowsAutomation.Enable(window);
                };
            }
            else
            {
                applicationArguments.Add(argument);
            }
        }

        _ = BuildAvaloniaApp().StartWithClassicDesktopLifetime(applicationArguments.ToArray());
    }

    /// <summary>Avalonia configuration, don't remove; also used by visual designer.</summary>
    /// <returns>AppBuilder.</returns>
    internal static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI(static _ => { });
}
