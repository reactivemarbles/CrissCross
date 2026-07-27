// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using CrissCross.Avalonia.UI.Gallery;
using ReactiveUI.Avalonia;

namespace CrissCross.NavigationView.Tests;

/// <summary>Initializes Avalonia and ReactiveUI services for navigation-view tests.</summary>
public static class AvaloniaTestAssemblyInitialization
{
    /// <summary>Builds the Avalonia platform and ReactiveUI activation services once for the test assembly.</summary>
    [Before(HookType.Assembly)]
    public static void Initialize() => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseReactiveUI(static _ => { })
        .SetupWithoutStarting();
}
