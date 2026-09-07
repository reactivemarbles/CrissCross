// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Maui.UI.Controls;

namespace CrissCross.Tests;

/// <summary>Verifies command execution state transitions.</summary>
public sealed class MauiCommandButtonStateTests
{
    /// <summary>Verifies ending execution clears the executing state.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task Execution_EndingWithoutAnOutcome_ReturnsToIdle()
    {
        var button = new CommandButton { IsExecuting = true };
        await Assert.That(button.State).IsEqualTo(CommandButtonState.Executing);
        button.IsExecuting = false;
        await Assert.That(button.State).IsEqualTo(CommandButtonState.Idle);
    }

    /// <summary>Verifies an explicitly supplied terminal outcome survives the execution transition.</summary>
    /// <param name="outcome">The terminal command outcome.</param>
    /// <returns>The asynchronous test.</returns>
    [Test]
    [Arguments(CommandButtonState.Failed)]
    [Arguments(CommandButtonState.Succeeded)]
    [Arguments(CommandButtonState.Cancelled)]
    public async Task Execution_EndingAfterAnOutcome_PreservesOutcome(CommandButtonState outcome)
    {
        var button = new CommandButton { IsExecuting = true, State = outcome };
        button.IsExecuting = false;
        await Assert.That(button.State).IsEqualTo(outcome);
    }
}
