// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Reactive.Tests;

/// <summary>Exercises reactive-shim object lifetime and navigation event behavior.</summary>
public sealed class ReactiveCoreBehaviorTests
{
    /// <summary>The navigation parameter used to verify event propagation.</summary>
    private const int NavigationParameter = 42;

    /// <summary>Verifies changing the display name publishes the reactive property change.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task DisplayName_PublishesChangeAndRetainsValue()
    {
        using var viewModel = new TestRxObject();
        var changed = false;
        viewModel.PropertyChanged += (_, eventArgs) => changed = eventArgs.PropertyName == nameof(RxObject.DisplayName);

        viewModel.DisplayName = "Reactive customer";

        await Assert.That(changed).IsTrue();
        await Assert.That(viewModel.DisplayName).IsEqualTo("Reactive customer");
        await Assert.That(viewModel.Name).IsEqualTo(typeof(TestRxObject).FullName);
    }

    /// <summary>Verifies disposal disposes registered resources exactly once.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Dispose_DisposesOwnedResourcesAndRemainsIdempotent()
    {
        var viewModel = new TestRxObject();
        var disposable = new TrackingDisposable();
        viewModel.Add(disposable);

        viewModel.Dispose();
        viewModel.Dispose();

        await Assert.That(disposable.DisposeCount).IsEqualTo(1);
        await Assert.That(viewModel.IsDisposed).IsTrue();
    }

    /// <summary>Verifies navigation callbacks receive the specific reactive navigation event types.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NavigationCallbacks_ReceiveNavigationEvents()
    {
        using var viewModel = new TestRxObject();
        var navigated = new ViewModelNavigationEventArgs(null, viewModel, NavigationType.New, null, "reactive", NavigationParameter);
        var navigating = new ViewModelNavigatingEventArgs(viewModel, null, NavigationType.Back, null, "reactive", "previous");

        viewModel.WhenNavigatedTo(navigated, []);
        viewModel.WhenNavigatedFrom(navigated);
        viewModel.WhenNavigating(navigating);

        await Assert.That(viewModel.NavigatedTo).IsTrue();
        await Assert.That(viewModel.NavigatedFrom).IsTrue();
        await Assert.That(viewModel.Navigating).IsTrue();
        await Assert.That(navigated.NavigationParameter).IsEqualTo(NavigationParameter);
        await Assert.That(navigating.NavigationType).IsEqualTo(NavigationType.Back);
        await Assert.That(navigating.Cancel).IsFalse();
    }

    /// <summary>Verifies mutable navigation event members retain assigned values.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task NavigatingEvent_CanCancelAndUpdateHost()
    {
        var navigation = new ViewModelNavigatingEventArgs(null, null, NavigationType.Refresh, null, "before");
        navigation.Cancel = true;
        navigation.HostName = "after";

        await Assert.That(navigation.Cancel).IsTrue();
        await Assert.That(navigation.HostName).IsEqualTo("after");
        await Assert.That(navigation.NavigationType).IsEqualTo(NavigationType.Refresh);
    }

    /// <summary>Provides an observable reactive object with testable protected lifetime state.</summary>
    private sealed class TestRxObject : RxObject
    {
        /// <summary>Gets a value indicating whether navigation arrived at this object.</summary>
        public bool NavigatedTo { get; private set; }

        /// <summary>Gets a value indicating whether navigation left this object.</summary>
        public bool NavigatedFrom { get; private set; }

        /// <summary>Gets a value indicating whether navigation is in progress.</summary>
        public bool Navigating { get; private set; }

        /// <summary>Adds a disposable to the object lifetime.</summary>
        /// <param name="disposable">The resource to own.</param>
        public void Add(IDisposable disposable) => Disposables.Add(disposable);

        /// <inheritdoc/>
        public override void WhenNavigatedFrom(IViewModelNavigationEventArgs e) => NavigatedFrom = true;

        /// <inheritdoc/>
        public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables) => NavigatedTo = true;

        /// <inheritdoc/>
        public override void WhenNavigating(IViewModelNavigatingEventArgs e) => Navigating = true;
    }

    /// <summary>Tracks disposal calls without relying on framework-specific disposable helpers.</summary>
    private sealed class TrackingDisposable : IDisposable
    {
        /// <summary>Gets the number of disposal calls.</summary>
        public int DisposeCount { get; private set; }

        /// <inheritdoc/>
        public void Dispose() => DisposeCount++;
    }
}
