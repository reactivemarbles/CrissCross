// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Browser;
using Avalonia.ReactiveUI;

using CrissCross.Avalonia.Test;

[assembly: SupportedOSPlatform("browser")]

internal partial class Program
{
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();

    private static async Task Main(string[] args) => await BuildAvaloniaApp()
            .WithInterFont()
            .UseReactiveUI()
            .StartBrowserAppAsync("out");
}
