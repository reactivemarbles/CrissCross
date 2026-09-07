// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.NavigationView.Tests;

/// <summary>Initializes Avalonia and ReactiveUI services for navigation-view tests.</summary>
public static class AvaloniaTestAssemblyInitialization
{
    /// <summary>Builds the Avalonia platform and ReactiveUI activation services once for the test assembly.</summary>
    /// <returns>The initialization task.</returns>
    [Before(HookType.Assembly)]
    public static Task Initialize() => AvaloniaTestUiThread.EnsureStartedAsync();

    /// <summary>Stops the shared Avalonia UI thread after the test assembly completes.</summary>
    /// <returns>The shutdown task.</returns>
    [After(HookType.Assembly)]
    public static Task Shutdown() => AvaloniaTestUiThread.ShutdownAsync();
}
