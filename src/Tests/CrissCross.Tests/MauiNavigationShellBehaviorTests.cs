// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.MAUI;
using Microsoft.Maui.Controls;

namespace CrissCross.Tests;

/// <summary>Exercises the non-reactive-package MAUI navigation shell through its public behavior.</summary>
public sealed class MauiNavigationShellBehaviorTests
{
    /// <summary>Provides the expected stack size before a history trim.</summary>
    private const int InitialHistoryCount = 2;

    /// <summary>Verifies setup rejects a shell without a stable routing name.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Setup_RejectsBlankShellName()
    {
        using var shell = new NavigationShell();

        await Assert.That(() => shell.Setup()).Throws<ArgumentException>();
    }

    /// <summary>Verifies naming a shell registers it as a routed navigation host.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NamedShell_RegistersAndExposesDefaultState()
    {
        const string shellName = "maui-shell-registration";
        using var shell = new NavigationShell { Name = shellName };

        var registered = ViewModelRoutedViewHostMixins.NavigationHost.TryGetValue(shellName, out var host);

        await Assert.That(shell.HostName).IsEqualTo(shellName);
        await Assert.That(shell.RequiresSetup).IsFalse();
        await Assert.That(shell.CanNavigateBack).IsFalse();
        await Assert.That(shell.NavigateBackIsEnabled).IsTrue();
        await Assert.That(shell.NavigationStack).IsEmpty();
        await Assert.That(registered).IsTrue();
        await Assert.That(host).IsSameReferenceAs(shell);
    }

    /// <summary>Verifies refresh and clear operations publish consistent history state.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task HistoryOperations_WhenBackNavigationChanges_PreserveOrTrimAsConfigured()
    {
        const string shellName = "maui-shell-history";
        using var shell = new NavigationShell { Name = shellName, NavigateBackIsEnabled = true };
        var backStates = new List<bool?>();
        using var subscription = shell.CanNavigateBackObservable.Subscribe(backStates.Add);
        shell.NavigationStack.Add(typeof(MauiNavigationShellBehaviorTests));
        shell.NavigationStack.Add(typeof(NavigationShell));

        shell.Refresh();
        var preservedCount = shell.NavigationStack.Count;
        shell.NavigateBackIsEnabled = false;
        shell.Refresh();
        Type? retainedType = null;
        foreach (var entry in shell.NavigationStack)
        {
            retainedType = entry;
        }

        var backResult = shell.NavigateBack("ignored");
        shell.ClearHistory();

        await Assert.That(preservedCount).IsEqualTo(InitialHistoryCount);
        await Assert.That(retainedType).IsEqualTo(typeof(NavigationShell));
        await Assert.That(backResult).IsNull();
        await Assert.That(shell.NavigationStack).IsEmpty();
        await Assert.That(backStates).Contains(false);
    }

    /// <summary>Verifies resolved navigation guards and page projection behavior.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ResolvedNavigationAndPageProjection_EnforcePublicContracts()
    {
        const string shellName = "maui-shell-resolution";
        using var shell = new NavigationShellProbe { Name = shellName };
        var page = new ContentPage();
        var nonPage = new object();
        NavigationResolution? nullResolution = null;

        await Assert.That(() => shell.Navigate(nullResolution!)).Throws<ArgumentNullException>();
        await Assert.That(() => shell.NavigateAndReset(nullResolution!)).Throws<ArgumentNullException>();
        await Assert.That(shell.ProjectPage(page)).IsSameReferenceAs(page);
        await Assert.That(shell.ProjectPage(nonPage)).IsNull();
    }

    /// <summary>Verifies disposal is safe when repeated.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Dispose_WhenRepeated_IsIdempotent()
    {
        var shell = new NavigationShell();

        shell.Dispose();
        shell.Dispose();

        await Assert.That(shell.NavigationStack).IsEmpty();
    }

    /// <summary>Exposes protected page projection for direct contract testing.</summary>
    private sealed class NavigationShellProbe : NavigationShell
    {
        /// <summary>Projects a value to a MAUI page.</summary>
        /// <param name="value">The value to project.</param>
        /// <returns>The page value, when applicable.</returns>
        public Page? ProjectPage(object value) => ToPage(value);
    }
}
