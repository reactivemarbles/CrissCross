// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Releases the shared WPF runtime after the test assembly completes.</summary>
public static class WpfTestAssemblyCleanup
{
    /// <summary>Stops the gallery application and the dispatcher that owns its resources.</summary>
    /// <returns>The shutdown task.</returns>
    [After(HookType.Assembly)]
    public static Task Shutdown() => WpfTestDispatcher.ShutdownAsync();
}
