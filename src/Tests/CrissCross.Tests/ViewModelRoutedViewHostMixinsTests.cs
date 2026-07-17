// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace CrissCross.Tests;

/// <summary>Tests for ViewModelRoutedViewHostMixins class.</summary>
public partial class ViewModelRoutedViewHostMixinsTests
{
    /// <summary>Provides the ProfileContract member.</summary>
    private const string ProfileContract = "profile";

    /// <summary>Provides the propagation delay used by observable tests.</summary>
    private const int ObservablePropagationDelayMilliseconds = 100;

    /// <summary>Provides the _testCounter member.</summary>
    private static int _testCounter;

    /// <summary>Provides the INavigationTargetViewModel member.</summary>
    private interface INavigationTargetViewModel
    {
        /// <summary>Gets the navigation scope.</summary>
        string NavigationScope { get; }
    }

    /// <summary>Provides the INavigationTargetView member.</summary>
    private interface INavigationTargetView
    {
        /// <summary>Gets the view scope.</summary>
        string ViewScope { get; }
    }

    /// <summary>Provides the IUnregisteredNavigationKey member.</summary>
    private interface IUnregisteredNavigationKey
    {
        /// <summary>Gets the key.</summary>
        string Key { get; }
    }

    /// <summary>Provides the Setup member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Before(HookType.Test)]
    public async Task Setup()
    {
        // Note: We don't clear static dictionaries as it causes test isolation issues
        // Each test should use unique host names to avoid conflicts
        await Task.CompletedTask;
    }

    /// <summary>Verifies that the host generic navigation shim resolves and forwards a registered view model.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Navigate_GenericHostShim_ResolvesRegisteredViewModel()
    {
        var parameter = new NavigationParameter("generic-host");
        var expectedViewModel = new TestViewModel();
        IViewModelRoutedViewHost viewHost = new TestViewModelRoutedViewHost();
        RegisterTestViewModel(expectedViewModel, ProfileContract);

        viewHost.Navigate(
            new NavigationKeyRequest<TestViewModel>
            {
                Options = new NavigationRequestOptions { Contract = ProfileContract, Parameter = parameter },
            });

        var testHost = (TestViewModelRoutedViewHost)viewHost;
        await Assert.That(ReferenceEquals(testHost.LastViewModel, expectedViewModel)).IsTrue();
        await Assert.That(testHost.LastContract).IsEqualTo(ProfileContract);
        await Assert.That(ReferenceEquals(testHost.LastParameter, parameter)).IsTrue();
    }

    /// <summary>Verifies that the host generic reset shim resolves a view model and clears history.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateAndReset_GenericHostShim_ResolvesRegisteredViewModelAndClearsHistory()
    {
        var parameter = new NavigationParameter("generic-reset");
        var expectedViewModel = new TestViewModel();
        var testHost = new TestViewModelRoutedViewHost();
        IViewModelRoutedViewHost viewHost = testHost;
        testHost.NavigationStack.Add(typeof(TestHostedViewModel));
        RegisterTestViewModel(expectedViewModel, ProfileContract);

        viewHost.NavigateAndReset(
            new NavigationKeyRequest<TestViewModel>
            {
                Options = new NavigationRequestOptions { Contract = ProfileContract, Parameter = parameter },
            });

        await Assert.That(testHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(ReferenceEquals(testHost.LastViewModel, expectedViewModel)).IsTrue();
        await Assert.That(testHost.LastContract).IsEqualTo(ProfileContract);
        await Assert.That(ReferenceEquals(testHost.LastParameter, parameter)).IsTrue();
    }

    /// <summary>Verifies that the host generic navigation shim rejects a null host.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Navigate_GenericHostShimWithNullHost_ThrowsArgumentNullException()
    {
        IViewModelRoutedViewHost? viewHost = null;

        await Assert
            .That(() => viewHost!.Navigate(new NavigationKeyRequest<TestViewModel>()))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the host generic reset shim rejects a null host.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateAndReset_GenericHostShimWithNullHost_ThrowsArgumentNullException()
    {
        IViewModelRoutedViewHost? viewHost = null;

        await Assert
            .That(() => viewHost!.NavigateAndReset(new NavigationKeyRequest<TestViewModel>()))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the host generic navigation shim rejects an unregistered view model.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Navigate_GenericHostShimWithUnregisteredViewModel_ThrowsInvalidOperationException()
    {
        IViewModelRoutedViewHost viewHost = new TestViewModelRoutedViewHost();
        AppLocator.CurrentMutable.UnregisterAll<TestHostedViewModel>();

        await Assert
            .That(() => viewHost.Navigate(new NavigationKeyRequest<TestHostedViewModel>()))
            .Throws<InvalidOperationException>();
    }

    /// <summary>Verifies that the host generic reset shim rejects an unregistered view model.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateAndReset_GenericHostShimWithUnregisteredViewModel_ThrowsInvalidOperationException()
    {
        IViewModelRoutedViewHost viewHost = new TestViewModelRoutedViewHost();
        AppLocator.CurrentMutable.UnregisterAll<TestHostedViewModel>();

        await Assert
            .That(() => viewHost.NavigateAndReset(new NavigationKeyRequest<TestHostedViewModel>()))
            .Throws<InvalidOperationException>();
    }

    /// <summary>Provides the SetMainNavigationHost_RegistersHost member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SetMainNavigationHost_RegistersHost()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);

        // Act
        setNav.SetMainNavigationHost(viewHost);

        // Assert
        await Assert.That(ViewModelRoutedViewHostMixins.NavigationHost.ContainsKey(hostName)).IsTrue();
        await Assert.That(ViewModelRoutedViewHostMixins.NavigationHost[hostName]).IsEqualTo(viewHost);
    }

    /// <summary>Provides the SetMainNavigationHost_ThrowsWhenSetNavigationIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SetMainNavigationHost_ThrowsWhenSetNavigationIsNull()
    {
        // Arrange
        TestSetNavigationViewModel? setNav = null;
        var viewHost = new TestViewModelRoutedViewHost();

        // Act & Assert
        await Assert.That(() => setNav!.SetMainNavigationHost(viewHost)).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the SetMainNavigationHost_ThrowsWhenViewHostIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SetMainNavigationHost_ThrowsWhenViewHostIsNull()
    {
        // Arrange
        var setNav = new TestSetNavigationViewModel();
        TestViewModelRoutedViewHost? viewHost = null;

        // Act & Assert
        await Assert.That(() => setNav.SetMainNavigationHost(viewHost!)).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the SetMainNavigationHost_ReplacesExistingHostForSameKey member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SetMainNavigationHost_ReplacesExistingHostForSameKey()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        using var setNav = new TestSetNavigationViewModel(hostName);
        var staleHost = new TestViewModelRoutedViewHost(hostName);
        var replacementHost = new TestViewModelRoutedViewHost(hostName);

        // Act
        setNav.SetMainNavigationHost(staleHost);
        setNav.SetMainNavigationHost(replacementHost);

        // Assert - recreated shells/windows must not keep navigating through stale hosts.
        await Assert.That(ViewModelRoutedViewHostMixins.NavigationHost[hostName]).IsEqualTo(replacementHost);
    }

    /// <summary>Provides the ClearHistory_WithIUseNavigation_ClearsHostHistory member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ClearHistory_WithIUseNavigation_ClearsHostHistory()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        using var vm = new TestViewModel(hostName);
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        vm.ClearHistory();

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(0);
    }

    /// <summary>Provides the ClearHistory_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ClearHistory_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);

        // Act & Assert
        await Assert.That(() => vm.ClearHistory()).Throws<Exception>();
    }

    /// <summary>Provides the ClearHistory_WithIUseNavigation_ThrowsWhenVmIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ClearHistory_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => vm!.ClearHistory()).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the ClearHistory_WithIUseHostedNavigation_ClearsHostHistory member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ClearHistory_WithIUseHostedNavigation_ClearsHostHistory()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        using var vm = new TestHostedViewModel();
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        vm.ClearHistory(hostName);

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(0);
    }

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ClearHistory_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();

        // Act & Assert
        await Assert.That(() => vm.ClearHistory(hostName)).Throws<Exception>();
    }

    /// <summary>Provides the ClearHistory_WithIUseHostedNavigation_ClearsSpecifiedHost member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ClearHistory_WithIUseHostedNavigation_ClearsSpecifiedHost()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        using var vm = new TestHostedViewModel();
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        vm.ClearHistory(hostName);

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(0);
    }

    /// <summary>Provides the NavigateBack_WithIUseNavigation_NavigatesBack member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateBack_WithIUseNavigation_NavigatesBack()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        ((IUseNavigation)vm).NavigateBack();

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
    }

    /// <summary>Provides the NavigateBack_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateBack_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm).NavigateBack()).Throws<Exception>();
    }

    /// <summary>Provides the NavigateBack_WithIUseNavigation_ThrowsWhenVmIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateBack_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm!).NavigateBack()).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the NavigateBack_WithIUseHostedNavigation_NavigatesBack member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateBack_WithIUseHostedNavigation_NavigatesBack()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        _ = vm.NavigateBack(hostName);

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
    }

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateBack_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();

        // Act & Assert
        await Assert.That(() => vm.NavigateBack(hostName)).Throws<Exception>();
    }

    /// <summary>Provides the NavigateToView_WithIUseNavigation_Navigates member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_WithIUseNavigation_Navigates()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        RegisterTestViewModel(new TestViewModel(hostName));
        setNav.SetMainNavigationHost(viewHost);

        // Act
        ((IUseNavigation)vm).NavigateToView(new NavigationKeyRequest<TestViewModel>());

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.NavigationStack[0]).IsEqualTo(typeof(TestViewModel));
    }
}
