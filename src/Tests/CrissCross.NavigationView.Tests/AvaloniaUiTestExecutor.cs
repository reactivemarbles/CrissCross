// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Core.Interfaces;

namespace CrissCross.NavigationView.Tests;

/// <summary>Runs test bodies on the dispatcher that owns their Avalonia objects.</summary>
public sealed class AvaloniaUiTestExecutor : ITestExecutor
{
    /// <inheritdoc/>
    public async ValueTask ExecuteTest(TestContext context, Func<ValueTask> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        await AvaloniaTestUiThread.RunAsync(() => action().AsTask());
    }
}
