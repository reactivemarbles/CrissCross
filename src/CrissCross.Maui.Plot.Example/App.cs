// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using ReactiveUI.Primitives.Concurrency;

namespace CrissCross.Maui.Plot.Example;

/// <summary>Hosts the native plotting showcase.</summary>
public sealed class App : Application
{
    /// <summary>The initial desktop window width.</summary>
    private const int WindowWidth = 1360;

    /// <summary>The initial desktop window height.</summary>
    private const int WindowHeight = 900;

    /// <inheritdoc/>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        _ = RxAppBuilder.CreateReactiveUIBuilder()
            .WithMainThreadScheduler(MauiDispatcherSequencer.For(Dispatcher))
            .BuildApp();
        var page = new MainPage();
        var window = new Window(page) { Title = "CrissCross MAUI Plot", Width = WindowWidth, Height = WindowHeight };
        window.Destroying += (_, _) => page.Dispose();
        return window;
    }
}
