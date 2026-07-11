// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using ReactiveUI;
using Splat;

namespace CrissCross.Tests;

/// <summary>Tests for ViewModelRoutedViewHostMixins class.</summary>
public class ViewModelRoutedViewHostMixinsTests
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

        viewHost.Navigate<TestViewModel>(ProfileContract, parameter);

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

        viewHost.NavigateAndReset<TestViewModel>(ProfileContract, parameter);

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

        await Assert.That(() => viewHost!.Navigate<TestViewModel>()).Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the host generic reset shim rejects a null host.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateAndReset_GenericHostShimWithNullHost_ThrowsArgumentNullException()
    {
        IViewModelRoutedViewHost? viewHost = null;

        await Assert.That(() => viewHost!.NavigateAndReset<TestViewModel>()).Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that the host generic navigation shim rejects an unregistered view model.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Navigate_GenericHostShimWithUnregisteredViewModel_ThrowsInvalidOperationException()
    {
        IViewModelRoutedViewHost viewHost = new TestViewModelRoutedViewHost();
        AppLocator.CurrentMutable.UnregisterAll<TestHostedViewModel>();

        await Assert.That(() => viewHost.Navigate<TestHostedViewModel>()).Throws<InvalidOperationException>();
    }

    /// <summary>Verifies that the host generic reset shim rejects an unregistered view model.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateAndReset_GenericHostShimWithUnregisteredViewModel_ThrowsInvalidOperationException()
    {
        IViewModelRoutedViewHost viewHost = new TestViewModelRoutedViewHost();
        AppLocator.CurrentMutable.UnregisterAll<TestHostedViewModel>();

        await Assert.That(() => viewHost.NavigateAndReset<TestHostedViewModel>()).Throws<InvalidOperationException>();
    }

    /// <summary>Provides the SetMainNavigationHost_RegistersHost member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SetMainNavigationHost_RegistersHost()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var setNav = new TestSetNavigationViewModel(hostName);
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
        var viewHost = new TestViewModelRoutedViewHost("TestHost");

        // Act & Assert
        await Assert.That(() => setNav!.SetMainNavigationHost(viewHost)).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the SetMainNavigationHost_ThrowsWhenViewHostIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task SetMainNavigationHost_ThrowsWhenViewHostIsNull()
    {
        // Arrange
        var setNav = new TestSetNavigationViewModel("TestHost");
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
        var setNav = new TestSetNavigationViewModel(hostName);
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
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
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
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        vm.ClearHistory(hostName);

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(0);
    }

    /// <summary>Provides the ClearHistory_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
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
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
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
        var setNav = new TestSetNavigationViewModel(hostName);
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
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        var result = vm.NavigateBack(hostName);

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
    }

    /// <summary>Provides the NavigateBack_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
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
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        RegisterTestViewModel(new TestViewModel(hostName));
        setNav.SetMainNavigationHost(viewHost);

        // Act
        ((IUseNavigation)vm).NavigateToView<TestViewModel>();

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.NavigationStack[0]).IsEqualTo(typeof(TestViewModel));
    }

    /// <summary>Provides the NavigateToView_WithIUseNavigation_PreservesLegacyContractAndParameter member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_WithIUseNavigation_PreservesLegacyContractAndParameter()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var parameter = new NavigationParameter("legacy");
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        RegisterTestViewModel(new TestViewModel(hostName), ProfileContract);
        setNav.SetMainNavigationHost(viewHost);

        // Act
        ((IUseNavigation)vm).NavigateToView<TestViewModel>(ProfileContract, parameter);

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.NavigationStack[0]).IsEqualTo(typeof(TestViewModel));
        await Assert.That(viewHost.LastContract).IsEqualTo(ProfileContract);
        await Assert.That(ReferenceEquals(viewHost.LastParameter, parameter)).IsTrue();
    }

    /// <summary>Provides the NavigateToView_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var otherHostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(otherHostName);
        var viewHost = new TestViewModelRoutedViewHost(otherHostName);
        setNav.SetMainNavigationHost(viewHost);

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm).NavigateToView<TestViewModel>()).Throws<KeyNotFoundException>();
    }

    /// <summary>Provides the NavigateToView_WithIUseNavigation_ThrowsWhenVmIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm!).NavigateToView<TestViewModel>()).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the NavigateToView_WithIUseHostedNavigation_Navigates member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_WithIUseHostedNavigation_Navigates()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        RegisterTestViewModel(new TestViewModel(hostName));
        setNav.SetMainNavigationHost(viewHost);

        // Act
        vm.NavigateToView<TestViewModel>(hostName);

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.NavigationStack[0]).IsEqualTo(typeof(TestViewModel));
    }

    /// <summary>Provides the NavigateToView_Type_WithIUseHostedNavigation_UsesRegisteredViewModelInstance member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_Type_WithIUseHostedNavigation_UsesRegisteredViewModelInstance()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var parameter = new NavigationParameter("type-legacy");
        var vm = new TestHostedViewModel();
        var expectedViewModel = new TestViewModel(hostName);
        var viewModelType = typeof(TestViewModel);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        AppLocator.CurrentMutable.UnregisterAll<TestViewModel>(ProfileContract);
        AppLocator.CurrentMutable.RegisterConstant(expectedViewModel, ProfileContract);
        setNav.SetMainNavigationHost(viewHost);

        // Act
        vm.NavigateToView(viewModelType, hostName, ProfileContract, parameter);

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.NavigationStack[0]).IsEqualTo(typeof(TestViewModel));
        await Assert.That(ReferenceEquals(viewHost.LastViewModel, expectedViewModel)).IsTrue();
        await Assert.That(viewHost.LastContract).IsEqualTo(ProfileContract);
        await Assert.That(ReferenceEquals(viewHost.LastParameter, parameter)).IsTrue();
    }

    /// <summary>Provides the NavigateTo_WithIUseNavigation_ResolvesInterfaceViewModelKeyToConcreteView member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateTo_WithIUseNavigation_ResolvesInterfaceViewModelKeyToConcreteView()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var parameter = new NavigationParameter("primary-view-model-key");
        var expectedViewModel = new NavigationTargetViewModel();
        var expectedView = new NavigationTargetView();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestResolvedViewModelRoutedViewHost(hostName);
        RegisterNavigationRegistry(expectedViewModel, expectedView, ProfileContract);
        setNav.SetMainNavigationHost(viewHost);

        // Act
        ((IUseNavigation)vm).NavigateTo<INavigationTargetViewModel>(ProfileContract, parameter);

        // Assert
        await Assert.That(viewHost.LastResolution).IsNotNull();
        await Assert.That(ReferenceEquals(viewHost.LastResolution!.ViewModel, expectedViewModel)).IsTrue();
        await Assert.That(ReferenceEquals(viewHost.LastResolution.View, expectedView)).IsTrue();
        await Assert.That(viewHost.LastResolution.View.GetType()).IsEqualTo(typeof(NavigationTargetView));
        await Assert.That(ReferenceEquals(expectedView.ViewModel, expectedViewModel)).IsTrue();
        await Assert.That(viewHost.LastResolution.Contract).IsEqualTo(ProfileContract);
        await Assert.That(ReferenceEquals(viewHost.LastResolution.Parameter, parameter)).IsTrue();
    }

    /// <summary>Provides the NavigateTo_WithIUseHostedNavigation_ResolvesInterfaceViewKeyToConcreteView member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateTo_WithIUseHostedNavigation_ResolvesInterfaceViewKeyToConcreteView()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var parameter = new NavigationParameter("hosted-view-key");
        var expectedViewModel = new NavigationTargetViewModel();
        var expectedView = new NavigationTargetView();
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestResolvedViewModelRoutedViewHost(hostName);
        RegisterNavigationRegistry(expectedViewModel, expectedView, ProfileContract);
        setNav.SetMainNavigationHost(viewHost);

        // Act
        vm.NavigateTo<INavigationTargetView>(hostName, ProfileContract, parameter);

        // Assert
        await Assert.That(viewHost.LastResolution).IsNotNull();
        await Assert.That(ReferenceEquals(viewHost.LastResolution!.ViewModel, expectedViewModel)).IsTrue();
        await Assert.That(ReferenceEquals(viewHost.LastResolution.View, expectedView)).IsTrue();
        await Assert.That(viewHost.LastResolution.View.GetType()).IsEqualTo(typeof(NavigationTargetView));
        await Assert.That(viewHost.LastResolution.Contract).IsEqualTo(ProfileContract);
        await Assert.That(ReferenceEquals(viewHost.LastResolution.Parameter, parameter)).IsTrue();
    }

    /// <summary>Provides the NavigateTo_Type_WithIUseNavigation_ResolvesRuntimeInterfaceKey member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateTo_Type_WithIUseNavigation_ResolvesRuntimeInterfaceKey()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var parameter = new NavigationParameter("runtime-key");
        var expectedViewModel = new NavigationTargetViewModel();
        var expectedView = new NavigationTargetView();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestResolvedViewModelRoutedViewHost(hostName);
        var navigationKey = typeof(INavigationTargetView);
        RegisterNavigationRegistry(expectedViewModel, expectedView, ProfileContract);
        setNav.SetMainNavigationHost(viewHost);

        // Act
        ((IUseNavigation)vm).NavigateTo(navigationKey, ProfileContract, parameter);

        // Assert
        await Assert.That(viewHost.LastResolution).IsNotNull();
        await Assert.That(ReferenceEquals(viewHost.LastResolution!.ViewModel, expectedViewModel)).IsTrue();
        await Assert.That(ReferenceEquals(viewHost.LastResolution.View, expectedView)).IsTrue();
        await Assert.That(viewHost.LastResolution.Contract).IsEqualTo(ProfileContract);
        await Assert.That(ReferenceEquals(viewHost.LastResolution.Parameter, parameter)).IsTrue();
    }

    /// <summary>Provides the NavigateTo_WithIUseNavigation_ThrowsWhenRegistryMissing member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateTo_WithIUseNavigation_ThrowsWhenRegistryMissing()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestResolvedViewModelRoutedViewHost(hostName);
        AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        AppLocator.CurrentMutable.UnregisterAll<INavigationRegistry>();
        setNav.SetMainNavigationHost(viewHost);

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm).NavigateTo<INavigationTargetViewModel>()).Throws<InvalidOperationException>();
    }

    /// <summary>Provides the NavigateTo_WithIUseNavigation_ThrowsWhenNavigationKeyMissing member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateTo_WithIUseNavigation_ThrowsWhenNavigationKeyMissing()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestResolvedViewModelRoutedViewHost(hostName);
        RegisterEmptyNavigationRegistry();
        setNav.SetMainNavigationHost(viewHost);

        // Act
        var exception = CaptureNavigationResolutionException(() => ((IUseNavigation)vm).NavigateTo<IUnregisteredNavigationKey>());

        // Assert
        await Assert.That(exception.SourceKind).IsEqualTo(NavigationSourceKind.View);
        await Assert.That(exception.SourceKey).IsEqualTo(typeof(IUnregisteredNavigationKey));
        await Assert.That(exception.KnownContracts.Count).IsEqualTo(0);
    }

    /// <summary>Provides the NavigateTo_WithIUseHostedNavigation_ThrowsWhenHostDoesNotSupportResolvedNavigation member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateTo_WithIUseHostedNavigation_ThrowsWhenHostDoesNotSupportResolvedNavigation()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        RegisterNavigationRegistry(new NavigationTargetViewModel(), new NavigationTargetView(), ProfileContract);
        setNav.SetMainNavigationHost(viewHost);

        // Act & Assert
        await Assert.That(() => vm.NavigateTo<INavigationTargetViewModel>(hostName, ProfileContract)).Throws<InvalidOperationException>();
    }

    /// <summary>Provides the NavigateToView_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();

        // Act & Assert
        await Assert.That(() => ((IUseHostedNavigation)vm).NavigateToView<TestViewModel>(hostName)).Throws<Exception>();
    }

    /// <summary>Provides the NavigateToViewAndClearHistory_WithIUseNavigation_NavigatesAndClears member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseNavigation_NavigatesAndClears()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        RegisterTestViewModel(new TestViewModel(hostName));
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        ((IUseNavigation)vm).NavigateToViewAndClearHistory<TestViewModel>();

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.CanNavigateBack).IsFalse();
    }

    /// <summary>Provides the NavigateToViewAndClearHistory_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);

        // Act & Assert - Will throw exception when trying to access non-existent host
        await Assert.That(() => ((IUseNavigation)vm).NavigateToViewAndClearHistory<TestViewModel>()).Throws<Exception>();
    }

    /// <summary>Provides the NavigateToViewAndClearHistory_WithIUseNavigation_ThrowsWhenVmIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => ((IUseNavigation)vm!).NavigateToViewAndClearHistory<TestViewModel>()).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the NavigateToViewAndClearHistory_WithIUseHostedNavigation_NavigatesAndClears member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseHostedNavigation_NavigatesAndClears()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        RegisterTestViewModel(new TestViewModel(hostName));
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        vm.NavigateToViewAndClearHistory<TestViewModel>(hostName);

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.CanNavigateBack).IsFalse();
    }

    /// <summary>Provides the NavigateToViewAndClearHistory_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();

        // Act & Assert - Will throw exception when trying to access non-existent host
        await Assert.That(() => ((IUseHostedNavigation)vm).NavigateToViewAndClearHistory<TestViewModel>(hostName)).Throws<Exception>();
    }

    /// <summary>Provides the WhenNavigatedFrom_SetsUpNavigatedFromFlag member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedFrom_SetsUpNavigatedFromFlag()
    {
        // Arrange
        var vm = new TestViewModel("TestHost");
        var view = new TestView(vm);
        var called = false;

        // Act
        view.WhenNavigatedFrom(_ => called = true);

        // Assert
        await Assert.That(view.ISetupNavigatedFrom).IsTrue();
    }

    /// <summary>Provides the WhenNavigatedFrom_ThrowsWhenViewIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedFrom_ThrowsWhenViewIsNull()
    {
        // Arrange
        TestView? view = null;

        // Act & Assert
        await Assert.That(() => view!.WhenNavigatedFrom(_ => { })).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the WhenNavigatedTo_SetsUpNavigatedToFlag member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedTo_SetsUpNavigatedToFlag()
    {
        // Arrange
        var vm = new TestViewModel("TestHost");
        var view = new TestView(vm);
        var called = false;

        // Act
        view.WhenNavigatedTo((_, _) => called = true);

        // Assert
        await Assert.That(view.ISetupNavigatedTo).IsTrue();
    }

    /// <summary>Provides the WhenNavigatedTo_ThrowsWhenViewIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedTo_ThrowsWhenViewIsNull()
    {
        // Arrange
        TestView? view = null;

        // Act & Assert
        await Assert.That(() => view!.WhenNavigatedTo((_, _) => { })).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the WhenNavigating_SetsUpNavigatingFlag member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigating_SetsUpNavigatingFlag()
    {
        // Arrange
        var vm = new TestViewModel("TestHost");
        var view = new TestView(vm);

        // Act
        view.WhenNavigating(e => e);

        // Assert
        await Assert.That(view.ISetupNavigating).IsTrue();
    }

    /// <summary>Provides the WhenNavigating_ThrowsWhenViewIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigating_ThrowsWhenViewIsNull()
    {
        // Arrange
        TestView? view = null;

        // Act & Assert
        await Assert.That(() => view!.WhenNavigating(e => e)).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the WhenSetup_WithIUseNavigation_ReturnsObservable member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WhenSetup_WithIUseNavigation_ReturnsObservable()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        var resolver = AppLocator.CurrentMutable;
        resolver.SetupComplete();

        // Act
        setNav.SetMainNavigationHost(viewHost);
        var observable = vm.WhenSetup();

        var result = false;
        var subscription = observable.Subscribe(x => result = x);

        // Give time for the observable to propagate
        await Task.Delay(ObservablePropagationDelayMilliseconds);

        // Assert
        await Assert.That(result).IsTrue();

        subscription.Dispose();
    }

    /// <summary>Provides the WhenSetup_WithIUseHostedNavigation_ReturnsObservable member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WhenSetup_WithIUseHostedNavigation_ReturnsObservable()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);

        // Act
        setNav.SetMainNavigationHost(viewHost);
        var observable = vm.WhenSetup(hostName);

        var result = false;
        var subscription = observable.Subscribe(x => result = x);

        // Give time for the observable to propagate
        await Task.Delay(ObservablePropagationDelayMilliseconds);

        // Assert
        await Assert.That(result).IsTrue();

        subscription.Dispose();
    }

    /// <summary>Provides the CanNavigateBack_WithIUseNavigation_ReturnsObservable member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CanNavigateBack_WithIUseNavigation_ReturnsObservable()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestViewModel(hostName);
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        var resolver = AppLocator.CurrentMutable;
        resolver.SetupComplete();
        setNav.SetMainNavigationHost(viewHost);

        // Act
        var observable = vm.CanNavigateBack();

        var result = true;
        var subscription = observable.Subscribe(x => result = x);

        // Give time for the observable to propagate
        await Task.Delay(ObservablePropagationDelayMilliseconds);

        // Assert
        await Assert.That(result).IsFalse();

        subscription.Dispose();
    }

    /// <summary>Provides the CanNavigateBack_WithIUseNavigation_ThrowsWhenVmIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CanNavigateBack_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => vm!.CanNavigateBack()).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the CanNavigateBack_WithIUseHostedNavigation_ReturnsObservable member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CanNavigateBack_WithIUseHostedNavigation_ReturnsObservable()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var vm = new TestHostedViewModel();
        var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        setNav.SetMainNavigationHost(viewHost);

        // Act
        var observable = vm.CanNavigateBack(hostName);

        var result = true;
        var subscription = observable.Subscribe(x => result = x);

        // Give time for the observable to propagate
        await Task.Delay(ObservablePropagationDelayMilliseconds);

        // Assert
        await Assert.That(result).IsFalse();

        subscription.Dispose();
    }

    /// <summary>Provides the CanNavigateBack_WithIUseHostedNavigation_ThrowsWhenVmIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task CanNavigateBack_WithIUseHostedNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestHostedViewModel? vm = null;

        // Act & Assert
        await Assert.That(() => vm!.CanNavigateBack("TestHost")).Throws<ArgumentNullException>();
    }

    /// <summary>Provides the GetUniqueHostName member.</summary>
    /// <returns>The result.</returns>
    private static string GetUniqueHostName() => $"TestHost{Interlocked.Increment(ref _testCounter)}";

    /// <summary>Provides the RegisterNavigationRegistry member.</summary>
    /// <param name="viewModel">The view model value.</param>
    /// <param name="view">The view value.</param>
    /// <param name="contract">The contract value.</param>
    private static void RegisterNavigationRegistry(NavigationTargetViewModel viewModel, NavigationTargetView view, string? contract)
    {
        var registry = new NavigationRegistry();
        _ = registry.Register<INavigationTargetViewModel, NavigationTargetViewModel, INavigationTargetView, NavigationTargetView>(
            _ => viewModel,
            _ => view,
            contract);

        RegisterNavigationRegistry(registry);
    }

    /// <summary>Provides the RegisterEmptyNavigationRegistry member.</summary>
    private static void RegisterEmptyNavigationRegistry() => RegisterNavigationRegistry(new NavigationRegistry());

    /// <summary>Provides the RegisterNavigationRegistry member.</summary>
    /// <param name="registry">The registry value.</param>
    private static void RegisterNavigationRegistry(NavigationRegistry registry)
    {
        AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        AppLocator.CurrentMutable.UnregisterAll<INavigationRegistry>();
        AppLocator.CurrentMutable.RegisterConstant<INavigationRegistry>(registry);
        AppLocator.CurrentMutable.RegisterConstant(registry.CreateNavigator());
    }

    /// <summary>Provides the RegisterTestViewModel member.</summary>
    /// <param name="viewModel">The view model value.</param>
    /// <param name="contract">The contract value.</param>
    private static void RegisterTestViewModel(TestViewModel viewModel, string? contract = null)
    {
        AppLocator.CurrentMutable.UnregisterAll<TestViewModel>(contract);
        AppLocator.CurrentMutable.RegisterConstant(viewModel, contract);
    }

    /// <summary>Provides the CaptureNavigationResolutionException member.</summary>
    /// <param name="action">The action value.</param>
    /// <returns>The result.</returns>
    private static NavigationResolutionException CaptureNavigationResolutionException(Action action)
    {
        try
        {
            action();
        }
        catch (NavigationResolutionException exception)
        {
            return exception;
        }

        throw new InvalidOperationException("Expected a navigation resolution exception.");
    }

    /// <summary>Provides the TestViewModel member.</summary>
    /// <param name="name">The name value.</param>
    private sealed class TestViewModel(string? name = "") : RxObject, IUseNavigation
    {
        string? IUseNavigation.Name => name;
    }

    /// <summary>Provides the TestHostedViewModel member.</summary>
    private sealed class TestHostedViewModel : RxObject;

    /// <summary>Provides the NavigationTargetViewModel member.</summary>
    private sealed class NavigationTargetViewModel : RxObject, INavigationTargetViewModel
    {
        /// <summary>Gets the navigation scope.</summary>
        public string NavigationScope => "NavigationTarget";
    }

    /// <summary>Provides the NavigationTargetView member.</summary>
    private sealed class NavigationTargetView : INavigationTargetView, IViewFor<NavigationTargetViewModel>
    {
        /// <summary>Gets the view scope.</summary>
        public string ViewScope => "NavigationTarget";

        /// <summary>Gets or sets the value.</summary>
        public NavigationTargetViewModel? ViewModel { get; set; }

        /// <summary>Gets or sets the value.</summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (NavigationTargetViewModel?)value;
        }
    }

    /// <summary>Provides the TestSetNavigationViewModel member.</summary>
    /// <param name="name">The name value.</param>
    private sealed class TestSetNavigationViewModel(string? name = "TestHost") : RxObject, ISetNavigation
    {
        string? ISetNavigation.Name => name;
    }

    /// <summary>Provides the TestViewModelRoutedViewHost member.</summary>
    /// <param name="name">The name value.</param>
    private class TestViewModelRoutedViewHost(string name = "TestHost") : IViewModelRoutedViewHost
    {
        /// <summary>Provides the _currentViewModel member.</summary>
        private readonly StateSignal<INotifiyRoutableViewModel?> _currentViewModel = new(null);

        /// <summary>Provides the documented member.</summary>
        private readonly StateSignal<bool?> _canNavigateBack = new(false);

        /// <summary>Gets the value.</summary>
        public ObservableCollection<Type?> NavigationStack { get; } = new();

        /// <summary>Gets the value.</summary>
        public IObservable<INotifiyRoutableViewModel> CurrentViewModel => _currentViewModel.Where(x => x is not null)!;

        /// <summary>Gets or sets the value.</summary>
        public bool? CanNavigateBack
        {
            get => _canNavigateBack.Value;
            set => _canNavigateBack.OnNext(value);
        }

        /// <summary>Gets the value.</summary>
        public IObservable<bool?> CanNavigateBackObservable => _canNavigateBack;

        /// <summary>Gets or sets the value.</summary>
        public bool? NavigateBackIsEnabled { get; set; }

        /// <summary>Gets or sets the value.</summary>
        public string Name { get; set; } = name;

        /// <summary>Gets or sets the value.</summary>
        public string HostName
        {
            get => Name;
            set => Name = value;
        }

        /// <summary>Gets the value.</summary>
        public bool RequiresSetup => false;

        /// <summary>Gets the value.</summary>
        public IObservable<Unit> Activated => Observable.Return(Unit.Default);

        /// <summary>Gets the value.</summary>
        public IObservable<Unit> Deactivated => Observable.Return(Unit.Default);

        /// <summary>Gets or sets the value.</summary>
        public IFullLogger? Log { get; set; }

        /// <summary>Gets the value.</summary>
        public Type? LastNavigationType { get; protected set; }

        /// <summary>Gets the value.</summary>
        public IRxObject? LastViewModel { get; protected set; }

        /// <summary>Gets the value.</summary>
        public string? LastContract { get; protected set; }

        /// <summary>Gets the value.</summary>
        public object? LastParameter { get; protected set; }

        /// <summary>Provides the ClearHistory member.</summary>
        public void ClearHistory()
        {
            NavigationStack.Clear();
            CanNavigateBack = false;
        }

        /// <summary>Provides the Setup member.</summary>
        public void Setup()
        {
            // Setup logic
        }

        /// <summary>Provides the Navigate member.</summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="contract">The contract value.</param>
        /// <param name="parameter">The parameter value.</param>
        public void Navigate<T>(string? contract = null, object? parameter = null)
            where T : class, IRxObject
        {
            LastNavigationType = typeof(T);
            LastViewModel = null;
            LastContract = contract;
            LastParameter = parameter;
            NavigationStack.Add(typeof(T));
            CanNavigateBack = NavigationStack.Count > 1;
        }

        /// <summary>Provides the Navigate member.</summary>
        /// <param name="viewModel">The viewModel value.</param>
        /// <param name="contract">The contract value.</param>
        /// <param name="parameter">The parameter value.</param>
        public void Navigate(IRxObject viewModel, string? contract = null, object? parameter = null)
        {
            LastNavigationType = viewModel.GetType();
            LastViewModel = viewModel;
            LastContract = contract;
            LastParameter = parameter;
            NavigationStack.Add(viewModel.GetType());
            CanNavigateBack = NavigationStack.Count > 1;

            if (viewModel is not INotifiyRoutableViewModel routableViewModel)
            {
                return;
            }

            _currentViewModel.OnNext(routableViewModel);
        }

        /// <summary>Provides the NavigateAndReset member.</summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="contract">The contract value.</param>
        /// <param name="parameter">The parameter value.</param>
        public void NavigateAndReset<T>(string? contract = null, object? parameter = null)
            where T : class, IRxObject
        {
            LastNavigationType = typeof(T);
            LastViewModel = null;
            LastContract = contract;
            LastParameter = parameter;
            NavigationStack.Clear();
            NavigationStack.Add(typeof(T));
            CanNavigateBack = false;
        }

        /// <summary>Provides the NavigateAndReset member.</summary>
        /// <param name="viewModel">The viewModel value.</param>
        /// <param name="contract">The contract value.</param>
        /// <param name="parameter">The parameter value.</param>
        public void NavigateAndReset(IRxObject viewModel, string? contract = null, object? parameter = null)
        {
            LastNavigationType = viewModel.GetType();
            LastViewModel = viewModel;
            LastContract = contract;
            LastParameter = parameter;
            NavigationStack.Clear();
            NavigationStack.Add(viewModel.GetType());
            CanNavigateBack = false;
        }

        /// <summary>Provides the NavigateBack member.</summary>
        /// <param name="parameter">The parameter value.</param>
        /// <returns>The result.</returns>
        public IRxObject? NavigateBack(object? parameter = null)
        {
            if (NavigationStack.Count <= 1)
            {
                return null;
            }

            NavigationStack.RemoveAt(NavigationStack.Count - 1);
            CanNavigateBack = NavigationStack.Count > 1;

            return null;
        }

        /// <summary>Provides the Refresh member.</summary>
        public void Refresh()
        {
            // Refresh logic
        }
    }

    /// <summary>Provides the TestResolvedViewModelRoutedViewHost member.</summary>
    /// <param name="name">The name value.</param>
    private sealed class TestResolvedViewModelRoutedViewHost(string name = "TestHost") : TestViewModelRoutedViewHost(name), IResolvedViewModelRoutedViewHost
    {
        /// <summary>Gets the value.</summary>
        public NavigationResolution? LastResolution { get; private set; }

        /// <summary>Provides the Navigate member.</summary>
        /// <param name="resolution">The resolution value.</param>
        public void Navigate(NavigationResolution resolution)
        {
            LastResolution = resolution;
            LastNavigationType = resolution.ViewModel.GetType();
            LastViewModel = resolution.ViewModel;
            LastContract = resolution.Contract;
            LastParameter = resolution.Parameter;
            NavigationStack.Add(resolution.ViewModel.GetType());
            CanNavigateBack = NavigationStack.Count > 1;
        }

        /// <summary>Provides the NavigateAndReset member.</summary>
        /// <param name="resolution">The resolution value.</param>
        public void NavigateAndReset(NavigationResolution resolution)
        {
            LastResolution = resolution;
            LastNavigationType = resolution.ViewModel.GetType();
            LastViewModel = resolution.ViewModel;
            LastContract = resolution.Contract;
            LastParameter = resolution.Parameter;
            NavigationStack.Clear();
            NavigationStack.Add(resolution.ViewModel.GetType());
            CanNavigateBack = false;
        }
    }

    /// <summary>Provides the documented member.</summary>
    /// <param name="viewModel">The view model.</param>
    private sealed class TestView(object? viewModel = null) : INotifiyNavigation, IViewFor
    {
        /// <summary>Gets or sets the value.</summary>
        public bool ISetupNavigatedTo { get; set; }

        /// <summary>Gets or sets the value.</summary>
        public bool ISetupNavigatedFrom { get; set; }

        /// <summary>Gets or sets the value.</summary>
        public bool ISetupNavigating { get; set; }

        /// <summary>Gets the value.</summary>
        public CompositeDisposable CleanUp { get; } = new();

        /// <summary>Gets the value.</summary>
        public bool IsDisposed => CleanUp.IsDisposed;

        /// <summary>Gets the value.</summary>
        public IObservable<Unit> Activated => Observable.Return(Unit.Default);

        /// <summary>Gets or sets the value.</summary>
        public IObservable<Unit> Deactivated => Observable.Return(Unit.Default);

        /// <summary>Gets or sets the value.</summary>
        public object? ViewModel { get; set; } = viewModel;

        /// <summary>Provides the Dispose member.</summary>
        public void Dispose()
        {
            CleanUp?.Dispose();
        }
    }

    /// <summary>Provides the NavigationParameter member.</summary>
    /// <param name="Value">The value.</param>
    private sealed record NavigationParameter(string Value);
}
