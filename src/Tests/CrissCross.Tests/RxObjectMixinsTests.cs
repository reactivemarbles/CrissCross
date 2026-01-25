// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using Splat;

namespace CrissCross.Tests;

/// <summary>
/// Tests for RxObjectMixins class.
/// </summary>
public class RxObjectMixinsTests
{
    private class TestReactiveObject : ReactiveObject
    {
        private string? _testProperty;

        public string? TestProperty
        {
            get => _testProperty;
            set => this.RaiseAndSetIfChanged(ref _testProperty, value);
        }
    }

    private class TestRxObject : RxObject;

    [Test]
    public async Task SetupComplete_RaisesBuildCompleteSignal()
    {
        // Arrange
        var resolver = Locator.CurrentMutable;
        var signalReceived = false;
        var testObject = new TestRxObject();

        var subscription = testObject.BuildCompleteDisposable(() => signalReceived = true);

        // Act
        resolver.SetupComplete();

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Assert
        await Assert.That(signalReceived).IsTrue();

        subscription.Dispose();
    }

    [Test]
    public async Task BuildComplete_ExecutesActionWhenSetupComplete()
    {
        // Arrange
        var resolver = Locator.CurrentMutable;
        var actionExecuted = false;
        var testObject = new TestRxObject();

        testObject.BuildComplete(() => actionExecuted = true);

        // Act
        resolver.SetupComplete();

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Assert
        await Assert.That(actionExecuted).IsTrue();
    }

    [Test]
    public async Task BuildCompleteDisposable_ReturnsDisposable()
    {
        // Arrange
        var testObject = new TestRxObject();
        var actionExecuted = false;

        // Act
        var disposable = testObject.BuildCompleteDisposable(() => actionExecuted = true);

        // Assert
        await Assert.That(disposable).IsNotNull();
        await Assert.That(disposable).IsAssignableTo<IDisposable>();

        disposable.Dispose();
    }

    [Test]
    public async Task BuildCompleteDisposable_CanBeDisposed()
    {
        // Arrange
        var resolver = Locator.CurrentMutable;
        var testObject = new TestRxObject();
        var actionExecuted = false;

        var disposable = testObject.BuildCompleteDisposable(() => actionExecuted = true);

        // Act
        disposable.Dispose();
        resolver.SetupComplete();

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Assert - action should not execute after disposal
        await Assert.That(actionExecuted).IsFalse();
    }

    [Test]
    public async Task ToListOfObservables_ConvertsListToObservables()
    {
        // Arrange
        var obj1 = new TestReactiveObject { TestProperty = "Value1" };
        var obj2 = new TestReactiveObject { TestProperty = "Value2" };
        var list = new List<TestReactiveObject> { obj1, obj2 };
        var subject = new BehaviorSubject<IEnumerable<TestReactiveObject>>(list);

        // Act
        var result = subject.ToListOfObservables(x => x.TestProperty);

        var observableList = new List<IObservable<string?>>();
        var subscription = result.Subscribe(l => observableList = l.ToList());

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Assert
        await Assert.That(observableList.Count).IsEqualTo(2);

        subscription.Dispose();
    }

    [Test]
    public async Task ToListOfObservables_UpdatesWhenSourceChanges()
    {
        // Arrange
        var obj1 = new TestReactiveObject { TestProperty = "Value1" };
        var list = new List<TestReactiveObject> { obj1 };
        var subject = new BehaviorSubject<IEnumerable<TestReactiveObject>>(list);

        var observableList = new List<IObservable<string?>>();
        var result = subject.ToListOfObservables(x => x.TestProperty);
        var subscription = result.Subscribe(l => observableList = l.ToList());

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Act - Add new object
        var obj2 = new TestReactiveObject { TestProperty = "Value2" };
        list.Add(obj2);
        subject.OnNext(list);

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Assert
        await Assert.That(observableList.Count).IsEqualTo(2);

        subscription.Dispose();
    }

    [Test]
    public async Task AnyMatch_ReturnsTrueWhenPredicateMatches()
    {
        // Arrange
        var obj1 = new TestReactiveObject { TestProperty = "Match" };
        var obj2 = new TestReactiveObject { TestProperty = "NoMatch" };
        var list = new List<TestReactiveObject> { obj1, obj2 };
        var subject = new BehaviorSubject<IEnumerable<TestReactiveObject>>(list);

        var observableList = subject.ToListOfObservables(x => x.TestProperty);
        var anyMatch = observableList.AnyMatch(x => x == "Match");

        var result = false;
        var subscription = anyMatch.Subscribe(x => result = x);

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Assert
        await Assert.That(result).IsTrue();

        subscription.Dispose();
    }

    [Test]
    public async Task AnyMatch_ReturnsFalseWhenPredicateDoesNotMatch()
    {
        // Arrange
        var obj1 = new TestReactiveObject { TestProperty = "NoMatch1" };
        var obj2 = new TestReactiveObject { TestProperty = "NoMatch2" };
        var list = new List<TestReactiveObject> { obj1, obj2 };
        var subject = new BehaviorSubject<IEnumerable<TestReactiveObject>>(list);

        var observableList = subject.ToListOfObservables(x => x.TestProperty);
        var anyMatch = observableList.AnyMatch(x => x == "Match");

        var result = true;
        var subscription = anyMatch.Subscribe(x => result = x);

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Assert
        await Assert.That(result).IsFalse();

        subscription.Dispose();
    }

    [Test]
    public async Task AnyMatch_UpdatesWhenObservableChanges()
    {
        // Arrange
        var obj1 = new TestReactiveObject { TestProperty = "NoMatch" };
        var list = new List<TestReactiveObject> { obj1 };
        var subject = new BehaviorSubject<IEnumerable<TestReactiveObject>>(list);

        var observableList = subject.ToListOfObservables(x => x.TestProperty);
        var anyMatch = observableList.AnyMatch(x => x == "Match");

        var result = false;
        var subscription = anyMatch.Subscribe(x => result = x);

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Act - Change property to match
        obj1.TestProperty = "Match";

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Assert
        await Assert.That(result).IsTrue();

        subscription.Dispose();
    }

    [Test]
    public async Task ToListOfObservables_HandlesNullSource()
    {
        // Arrange
        var subject = new BehaviorSubject<IEnumerable<TestReactiveObject>?>(null);
        var result = subject.ToListOfObservables(x => x.TestProperty)!;

        var receivedValue = false;
        var subscription = result.Subscribe(_ => receivedValue = true);

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Assert - Should not crash and should not emit for null
        await Assert.That(receivedValue).IsFalse();

        subscription.Dispose();
    }

    [Test]
    public async Task ToListOfObservables_HandlesEmptySource()
    {
        // Arrange
        var list = new List<TestReactiveObject>();
        var subject = new BehaviorSubject<IEnumerable<TestReactiveObject>>(list);
        var result = subject.ToListOfObservables(x => x.TestProperty);

        var observableList = new List<IObservable<string?>>();
        var subscription = result.Subscribe(l => observableList = l.ToList());

        // Give time for the observable to propagate
        await Task.Delay(100);

        // Assert - Should handle empty list
        await Assert.That(observableList.Count).IsEqualTo(0);

        subscription.Dispose();
    }
}
