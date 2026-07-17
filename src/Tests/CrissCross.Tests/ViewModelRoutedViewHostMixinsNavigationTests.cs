// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace CrissCross.Tests;

/// <summary>Tests for ViewModelRoutedViewHostMixins class.</summary>
public partial class ViewModelRoutedViewHostMixinsTests
{
    /// <summary>Provides the NavigateToView_WithIUseNavigation_PreservesLegacyContractAndParameter member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_WithIUseNavigation_PreservesLegacyContractAndParameter()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var parameter = new NavigationParameter("legacy");
        using var vm = new TestViewModel(hostName);
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        RegisterTestViewModel(new TestViewModel(hostName), ProfileContract);
        setNav.SetMainNavigationHost(viewHost);

        // Act
        ((IUseNavigation)vm).NavigateToView(
            new NavigationKeyRequest<TestViewModel>
            {
                Options = new NavigationRequestOptions { Contract = ProfileContract, Parameter = parameter },
            });

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
        using var vm = new TestViewModel(hostName);
        using var setNav = new TestSetNavigationViewModel(otherHostName);
        var viewHost = new TestViewModelRoutedViewHost(otherHostName);
        setNav.SetMainNavigationHost(viewHost);

        // Act & Assert
        await Assert
            .That(() => ((IUseNavigation)vm).NavigateToView(new NavigationKeyRequest<TestViewModel>()))
            .Throws<KeyNotFoundException>();
    }

    /// <summary>Provides the NavigateToView_WithIUseNavigation_ThrowsWhenVmIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert
            .That(() => ((IUseNavigation)vm!).NavigateToView(new NavigationKeyRequest<TestViewModel>()))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Provides the NavigateToView_WithIUseHostedNavigation_Navigates member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_WithIUseHostedNavigation_Navigates()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        using var vm = new TestHostedViewModel();
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        RegisterTestViewModel(new TestViewModel(hostName));
        setNav.SetMainNavigationHost(viewHost);

        // Act
        vm.NavigateToView(
            new NavigationKeyRequest<TestViewModel>
            {
                Options = new NavigationRequestOptions { HostName = hostName },
            });

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.NavigationStack[0]).IsEqualTo(typeof(TestViewModel));
    }

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_Type_WithIUseHostedNavigation_UsesRegisteredViewModelInstance()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var parameter = new NavigationParameter("type-legacy");
        using var vm = new TestHostedViewModel();
        var expectedViewModel = new TestViewModel(hostName);
        var viewModelType = typeof(TestViewModel);
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        AppLocator.CurrentMutable.UnregisterAll<TestViewModel>(ProfileContract);
        AppLocator.CurrentMutable.RegisterConstant(expectedViewModel, ProfileContract);
        setNav.SetMainNavigationHost(viewHost);

        // Act
        vm.NavigateToView(
            viewModelType,
            new NavigationRequestOptions
            {
                HostName = hostName,
                Contract = ProfileContract,
                Parameter = parameter,
            });

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.NavigationStack[0]).IsEqualTo(typeof(TestViewModel));
        await Assert.That(ReferenceEquals(viewHost.LastViewModel, expectedViewModel)).IsTrue();
        await Assert.That(viewHost.LastContract).IsEqualTo(ProfileContract);
        await Assert.That(ReferenceEquals(viewHost.LastParameter, parameter)).IsTrue();
    }

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateTo_WithIUseNavigation_ResolvesInterfaceViewModelKeyToConcreteView()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var parameter = new NavigationParameter("primary-view-model-key");
        var expectedViewModel = new NavigationTargetViewModel();
        var expectedView = new NavigationTargetView();
        using var vm = new TestViewModel(hostName);
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestResolvedViewModelRoutedViewHost(hostName);
        RegisterNavigationRegistry(expectedViewModel, expectedView, ProfileContract);
        setNav.SetMainNavigationHost(viewHost);

        // Act
        ((IUseNavigation)vm).NavigateTo(
            new NavigationKeyRequest<INavigationTargetViewModel>
            {
                Options = new NavigationRequestOptions { Contract = ProfileContract, Parameter = parameter },
            });

        // Assert
        await Assert.That(viewHost.LastResolution).IsNotNull();
        await Assert.That(ReferenceEquals(viewHost.LastResolution!.ViewModel, expectedViewModel)).IsTrue();
        await Assert.That(ReferenceEquals(viewHost.LastResolution.View, expectedView)).IsTrue();
        await Assert.That(viewHost.LastResolution.View.GetType()).IsEqualTo(typeof(NavigationTargetView));
        await Assert.That(ReferenceEquals(expectedView.ViewModel, expectedViewModel)).IsTrue();
        await Assert.That(viewHost.LastResolution.Contract).IsEqualTo(ProfileContract);
        await Assert.That(ReferenceEquals(viewHost.LastResolution.Parameter, parameter)).IsTrue();
    }

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateTo_WithIUseHostedNavigation_ResolvesInterfaceViewKeyToConcreteView()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        var parameter = new NavigationParameter("hosted-view-key");
        var expectedViewModel = new NavigationTargetViewModel();
        var expectedView = new NavigationTargetView();
        using var vm = new TestHostedViewModel();
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestResolvedViewModelRoutedViewHost(hostName);
        RegisterNavigationRegistry(expectedViewModel, expectedView, ProfileContract);
        setNav.SetMainNavigationHost(viewHost);

        // Act
        vm.NavigateTo(
            new NavigationKeyRequest<INavigationTargetView>
            {
                Options = new NavigationRequestOptions
                {
                    HostName = hostName,
                    Contract = ProfileContract,
                    Parameter = parameter,
                },
            });

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
        using var vm = new TestViewModel(hostName);
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestResolvedViewModelRoutedViewHost(hostName);
        var navigationKey = typeof(INavigationTargetView);
        RegisterNavigationRegistry(expectedViewModel, expectedView, ProfileContract);
        setNav.SetMainNavigationHost(viewHost);

        // Act
        ((IUseNavigation)vm).NavigateTo(
            navigationKey,
            new NavigationRequestOptions { Contract = ProfileContract, Parameter = parameter });

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
        using var vm = new TestViewModel(hostName);
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestResolvedViewModelRoutedViewHost(hostName);
        AppLocator.CurrentMutable.UnregisterAll<IBidirectionalNavigator>();
        AppLocator.CurrentMutable.UnregisterAll<INavigationRegistry>();
        setNav.SetMainNavigationHost(viewHost);

        // Act & Assert
        await Assert
            .That(() =>
                ((IUseNavigation)vm).NavigateTo(new NavigationKeyRequest<INavigationTargetViewModel>()))
            .Throws<InvalidOperationException>();
    }

    /// <summary>Provides the NavigateTo_WithIUseNavigation_ThrowsWhenNavigationKeyMissing member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateTo_WithIUseNavigation_ThrowsWhenNavigationKeyMissing()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        using var vm = new TestViewModel(hostName);
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestResolvedViewModelRoutedViewHost(hostName);
        RegisterEmptyNavigationRegistry();
        setNav.SetMainNavigationHost(viewHost);

        // Act
        NavigationResolutionException exception;
        try
        {
            ((IUseNavigation)vm).NavigateTo(new NavigationKeyRequest<IUnregisteredNavigationKey>());
            throw new InvalidOperationException("Expected a navigation resolution exception.");
        }
        catch (NavigationResolutionException caughtException)
        {
            exception = caughtException;
        }

        // Assert
        await Assert.That(exception.SourceKind).IsEqualTo(NavigationSourceKind.View);
        await Assert.That(exception.SourceKey).IsEqualTo(typeof(IUnregisteredNavigationKey));
        await Assert.That(exception.KnownContracts.Count).IsEqualTo(0);
    }

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateTo_WithIUseHostedNavigation_ThrowsWhenHostDoesNotSupportResolvedNavigation()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        using var vm = new TestHostedViewModel();
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        RegisterNavigationRegistry(new NavigationTargetViewModel(), new NavigationTargetView(), ProfileContract);
        setNav.SetMainNavigationHost(viewHost);

        // Act & Assert
        await Assert
            .That(() =>
                vm.NavigateTo(
                    new NavigationKeyRequest<INavigationTargetViewModel>
                    {
                        Options = new NavigationRequestOptions
                        {
                            HostName = hostName,
                            Contract = ProfileContract,
                        },
                    }))
            .Throws<InvalidOperationException>();
    }

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToView_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        using var vm = new TestHostedViewModel();

        // Act & Assert
        await Assert
            .That(() =>
                ((IUseHostedNavigation)vm).NavigateToView(
                    new NavigationKeyRequest<TestViewModel>
                    {
                        Options = new NavigationRequestOptions { HostName = hostName },
                    }))
            .Throws<Exception>();
    }

    /// <summary>Provides the NavigateToViewAndClearHistory_WithIUseNavigation_NavigatesAndClears member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseNavigation_NavigatesAndClears()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        using var vm = new TestViewModel(hostName);
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        RegisterTestViewModel(new TestViewModel(hostName));
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        ((IUseNavigation)vm).NavigateToViewAndClearHistory(new NavigationKeyRequest<TestViewModel>());

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.CanNavigateBack).IsFalse();
    }

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        using var vm = new TestViewModel(hostName);

        // Act & Assert - Will throw exception when trying to access non-existent host
        await Assert
            .That(() =>
                ((IUseNavigation)vm).NavigateToViewAndClearHistory(new NavigationKeyRequest<TestViewModel>()))
            .Throws<Exception>();
    }

    /// <summary>Provides the NavigateToViewAndClearHistory_WithIUseNavigation_ThrowsWhenVmIsNull member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseNavigation_ThrowsWhenVmIsNull()
    {
        // Arrange
        TestViewModel? vm = null;

        // Act & Assert
        await Assert
            .That(() =>
                ((IUseNavigation)vm!).NavigateToViewAndClearHistory(new NavigationKeyRequest<TestViewModel>()))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseHostedNavigation_NavigatesAndClears()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        using var vm = new TestHostedViewModel();
        using var setNav = new TestSetNavigationViewModel(hostName);
        var viewHost = new TestViewModelRoutedViewHost(hostName);
        RegisterTestViewModel(new TestViewModel(hostName));
        setNav.SetMainNavigationHost(viewHost);
        viewHost.NavigationStack.Add(typeof(TestViewModel));

        // Act
        vm.NavigateToViewAndClearHistory(
            new NavigationKeyRequest<TestViewModel>
            {
                Options = new NavigationRequestOptions { HostName = hostName },
            });

        // Assert
        await Assert.That(viewHost.NavigationStack.Count).IsEqualTo(1);
        await Assert.That(viewHost.CanNavigateBack).IsFalse();
    }

    /// <summary>Verifies the documented behavior.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task NavigateToViewAndClearHistory_WithIUseHostedNavigation_ThrowsWhenSpecificHostNotRegistered()
    {
        // Arrange
        var hostName = GetUniqueHostName();
        using var vm = new TestHostedViewModel();

        // Act & Assert - Will throw exception when trying to access non-existent host
        await Assert
            .That(() =>
                ((IUseHostedNavigation)vm).NavigateToViewAndClearHistory(
                    new NavigationKeyRequest<TestViewModel>
                    {
                        Options = new NavigationRequestOptions { HostName = hostName },
                    }))
            .Throws<Exception>();
    }
}
