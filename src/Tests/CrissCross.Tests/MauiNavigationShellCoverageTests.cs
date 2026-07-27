// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.MAUI;

namespace CrissCross.Tests;

/// <summary>Exercises public NavigationShell routes that do not require a platform navigation stack.</summary>
public sealed class MauiNavigationShellCoverageTests
{
    /// <summary>Verifies unresolved generic navigation preserves prior history because no navigation is accepted.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateAndReset_WithUnresolvedViewModel_PreservesExistingHistory()
    {
        const int existingHistoryCount = 2;
        using var shell = new NavigationShell { Name = "maui-shell-unresolved-navigation" };
        var viewModel = new NavigationViewModel();
        shell.NavigationStack.Add(typeof(NavigationShell));
        shell.NavigationStack.Add(typeof(MauiNavigationShellCoverageTests));

        shell.NavigateAndReset(viewModel, contract: null, parameter: null);

        await Assert.That(shell.NavigationStack.Count).IsEqualTo(existingHistoryCount);
        await Assert.That(shell.NavigationStack).Contains(typeof(NavigationShell));
        await Assert.That(shell.NavigationStack).Contains(typeof(MauiNavigationShellCoverageTests));
        await Assert.That(shell.CanNavigateBack).IsFalse();
    }

    /// <summary>Provides a concrete routed view-model type for generic navigation.</summary>
    private sealed class NavigationViewModel : RxObject;
}
