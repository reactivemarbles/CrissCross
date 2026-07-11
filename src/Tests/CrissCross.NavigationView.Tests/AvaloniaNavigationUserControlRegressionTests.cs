// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AvaloniaNavigationUserControl = CrissCross.Avalonia.NavigationUserControl;

namespace CrissCross.NavigationView.Tests;

/// <summary>Regression tests for Avalonia navigation host setup.</summary>
public class AvaloniaNavigationUserControlRegressionTests
{
    /// <summary>Provides the OnInitialized_WhenControlHasName_UsesExistingNameForNavigationHost member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task OnInitialized_WhenControlHasName_UsesExistingNameForNavigationHost()
    {
        var control = new TestNavigationUserControl
        {
            Name = "mainWindow"
        };

        control.InitializeForTest();

        await Assert.That(control.Name).IsEqualTo("mainWindow");
        await Assert.That(((IUseNavigation)control).Name).IsEqualTo("mainWindow");
        await Assert.That(control.NavigationFrame?.HostName).IsEqualTo("mainWindow");
        await Assert.That(control.NavigationFrame?.Name).IsEqualTo("mainWindow");
    }

    /// <summary>Provides the OnInitialized_WhenControlHasNoName_UsesGeneratedHostNameWithoutSettingElementName member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task OnInitialized_WhenControlHasNoName_UsesGeneratedHostNameWithoutSettingElementName()
    {
        var control = new TestNavigationUserControl();

        control.InitializeForTest();

        var hostName = ((IUseNavigation)control).Name;

        await Assert.That(control.Name).IsNull();
        await Assert.That(string.IsNullOrWhiteSpace(hostName)).IsFalse();
        await Assert.That(hostName!.StartsWith("__crisscross_navhost_NavigationUserControl_", StringComparison.Ordinal)).IsTrue();
        await Assert.That(control.NavigationFrame?.HostName).IsEqualTo(hostName);
        await Assert.That(control.NavigationFrame?.Name).IsEqualTo(hostName);
    }

    /// <summary>Provides the TestNavigationUserControl member.</summary>
    private sealed class TestNavigationUserControl : AvaloniaNavigationUserControl
    {
        /// <summary>Provides the InitializeForTest member.</summary>
        public void InitializeForTest() => OnInitialized();
    }
}
