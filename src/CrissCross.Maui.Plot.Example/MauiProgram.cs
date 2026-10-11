// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ScottPlot.Maui;

namespace CrissCross.Maui.Plot.Example;

/// <summary>Configures the native MAUI plotting application.</summary>
public static class MauiProgram
{
    /// <summary>Creates the application and registers native plot handlers and reactive scheduling.</summary>
    /// <returns>The configured MAUI application.</returns>
    public static MauiApp CreateMauiApp() => MauiApp.CreateBuilder()
        .UseMauiApp<App>()
        .UseScottPlot()
        .Build();
}
