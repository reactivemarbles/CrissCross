// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using AvaloniaNavigationUserControl = CrissCross.Avalonia.NavigationUserControl;

namespace CrissCross.NavigationView.Tests;

/// <summary>
/// Regression tests for Avalonia navigation host setup.
/// </summary>
public class AvaloniaNavigationUserControlRegressionTests
{
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

    private sealed class TestNavigationUserControl : AvaloniaNavigationUserControl
    {
        public void InitializeForTest() => OnInitialized();
    }
}
