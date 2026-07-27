// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Reactive.Builder;
using Splat;

namespace CrissCross.Reactive.Tests;

/// <summary>Verifies observable collection and application-build behavior in the reactive shim.</summary>
public sealed class ReactiveMixinsTests
{
    /// <summary>The value used by the observable predicate.</summary>
    private const string MatchValue = "match";

    /// <summary>Verifies build-completion subscriptions execute when the mutable resolver completes setup.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task SetupComplete_NotifiesBuildCompleteSubscription()
    {
        _ = RxAppBuilder.CreateReactiveUIBuilder().WithCoreServices().BuildApp();
        var notified = false;
        var observer = new BuildAwareObject();
        using var subscription = observer.BuildCompleteDisposable(() => notified = true);

        AppLocator.CurrentMutable.SetupComplete();

        await Assert.That(notified).IsTrue();
    }

    /// <summary>Verifies property observables are created for every current reactive item.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ToListOfObservables_ProjectsEveryReactiveItem()
    {
        _ = RxAppBuilder.CreateReactiveUIBuilder().WithCoreServices().BuildApp();
        var first = new ObservableItem { Value = "first", };
        var second = new ObservableItem { Value = "second", };
        using var source = new StateSignal<IEnumerable<ObservableItem>>([first, second]);
        var receivedValues = false;
        using var subscription = source
            .ToListOfObservables(item => item.Value)
            .Subscribe(_ => receivedValues = true);

        await Assert.That(receivedValues).IsTrue();
    }

    /// <summary>Verifies matching state changes are propagated through the observable collection pipeline.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task AnyMatch_TracksReactivePropertyChanges()
    {
        _ = RxAppBuilder.CreateReactiveUIBuilder().WithCoreServices().BuildApp();
        var item = new ObservableItem { Value = "pending", };
        using var source = new StateSignal<IEnumerable<ObservableItem>>([item]);
        var matched = false;
        using var subscription = source
            .ToListOfObservables(entry => entry.Value)
            .AnyMatch(static value => value == MatchValue)
            .Subscribe(value => matched = value);

        item.Value = MatchValue;

        await Assert.That(matched).IsTrue();
    }

    /// <summary>Verifies an empty observable collection produces a non-matching state.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task AnyMatch_EmptyCollectionPublishesFalse()
    {
        _ = RxAppBuilder.CreateReactiveUIBuilder().WithCoreServices().BuildApp();
        using var source = new StateSignal<IEnumerable<ObservableItem>>([]);
        var matched = true;
        using var subscription = source
            .ToListOfObservables(entry => entry.Value)
            .AnyMatch(static value => value == MatchValue)
            .Subscribe(value => matched = value);

        await Assert.That(matched).IsFalse();
    }

    /// <summary>Provides an object that is eligible for build-completion callbacks.</summary>
    private sealed class BuildAwareObject : RxObject;

    /// <summary>Provides an observable reactive item.</summary>
    private sealed class ObservableItem : ReactiveObject
    {
        /// <summary>Gets or sets the observable value.</summary>
        public string? Value
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }
}
