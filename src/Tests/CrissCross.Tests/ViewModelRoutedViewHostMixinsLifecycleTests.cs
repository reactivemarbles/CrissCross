// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace CrissCross.Tests;

/// <summary>Tests for ViewModelRoutedViewHostMixins class.</summary>
public partial class ViewModelRoutedViewHostMixinsTests
{
    /// <summary>Provides the default host name for lifecycle tests.</summary>
    private const string LifecycleTestHostName = "TestHost";

    /// <summary>Provides the WhenNavigatedFrom_SetsUpNavigatedFromFlag member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedFrom_SetsUpNavigatedFromFlag()
    {
        // Arrange
        using var vm = new TestViewModel(LifecycleTestHostName);
        using var view = new TestView(vm);
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
        using var vm = new TestViewModel(LifecycleTestHostName);
        using var view = new TestView(vm);
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
        using var vm = new TestViewModel(LifecycleTestHostName);
        using var view = new TestView(vm);

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
        using var vm = new TestViewModel(hostName);
        using var setNav = new TestSetNavigationViewModel(hostName);
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
        using var vm = new TestHostedViewModel();
        using var setNav = new TestSetNavigationViewModel(hostName);
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
        using var vm = new TestViewModel(hostName);
        using var setNav = new TestSetNavigationViewModel(hostName);
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
        using var vm = new TestHostedViewModel();
        using var setNav = new TestSetNavigationViewModel(hostName);
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
        await Assert.That(() => vm!.CanNavigateBack(LifecycleTestHostName)).Throws<ArgumentNullException>();
    }
}
