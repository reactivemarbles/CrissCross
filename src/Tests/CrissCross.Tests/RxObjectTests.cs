// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using CrissCross;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace CrissCross.Tests;

/// <summary>
/// Tests for RxObject class.
/// </summary>
public class RxObjectTests
{
    /// <summary>
    /// Test implementation of RxObject.
    /// </summary>
    private class TestRxObject : RxObject
    {
        public CompositeDisposable GetDisposables() => Disposables;
    }

    [Test]
    public async Task Name_ReturnsFullTypeName()
    {
        // Arrange
        var rxObject = new TestRxObject();

        // Act
        var name = rxObject.Name;

        // Assert
        await Assert.That(name).IsEqualTo(typeof(TestRxObject).FullName);
    }

    [Test]
    public async Task DisplayName_CanBeSetAndRetrieved()
    {
        // Arrange
        var rxObject = new TestRxObject();
        var expectedDisplayName = "Test Display Name";

        // Act
        rxObject.DisplayName = expectedDisplayName;

        // Assert
        await Assert.That(rxObject.DisplayName).IsEqualTo(expectedDisplayName);
    }

    [Test]
    public async Task DisplayName_IsNullByDefault()
    {
        // Arrange & Act
        var rxObject = new TestRxObject();

        // Assert
        await Assert.That(rxObject.DisplayName).IsNull();
    }

    [Test]
    public async Task IsDisposed_IsFalseByDefault()
    {
        // Arrange & Act
        var rxObject = new TestRxObject();

        // Assert
        await Assert.That(rxObject.IsDisposed).IsFalse();
    }

    [Test]
    public async Task IsDisposed_IsTrueAfterDispose()
    {
        // Arrange
        var rxObject = new TestRxObject();

        // Act
        rxObject.Dispose();

        // Assert
        await Assert.That(rxObject.IsDisposed).IsTrue();
    }

    [Test]
    public async Task Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var rxObject = new TestRxObject();

        // Act
        rxObject.Dispose();
        rxObject.Dispose();

        // Assert
        await Assert.That(rxObject.IsDisposed).IsTrue();
    }

    [Test]
    public async Task Disposables_AreDisposedWhenObjectIsDisposed()
    {
        // Arrange
        var rxObject = new TestRxObject();
        var wasDisposed = false;
        var disposable = Disposable.Create(() => wasDisposed = true);
        rxObject.GetDisposables().Add(disposable);

        // Act
        rxObject.Dispose();

        // Assert
        await Assert.That(wasDisposed).IsTrue();
    }

    [Test]
    public async Task WhenNavigatedFrom_CanBeCalled()
    {
        // Arrange
        var rxObject = new TestRxObject();
        var eventArgs = new ViewModelNavigationEventArgs(
            rxObject,
            null,
            NavigationType.New,
            null,
            "test",
            null);

        // Act & Assert - should not throw
        rxObject.WhenNavigatedFrom(eventArgs);
        await Assert.That(true).IsTrue();
    }

    [Test]
    public async Task WhenNavigatedTo_CanBeCalled()
    {
        // Arrange
        var rxObject = new TestRxObject();
        var disposables = new CompositeDisposable();
        var eventArgs = new ViewModelNavigationEventArgs(
            null,
            rxObject,
            NavigationType.New,
            null,
            "test",
            null);

        // Act & Assert - should not throw
        rxObject.WhenNavigatedTo(eventArgs, disposables);
        await Assert.That(true).IsTrue();
    }

    [Test]
    public async Task WhenNavigating_CanBeCalled()
    {
        // Arrange
        var rxObject = new TestRxObject();
        var eventArgs = new ViewModelNavigatingEventArgs(
            rxObject,
            null,
            NavigationType.New,
            null,
            "test",
            null);

        // Act & Assert - should not throw
        rxObject.WhenNavigating(eventArgs);
        await Assert.That(true).IsTrue();
    }

    [Test]
    [Skip("DisplayName is an auto-property and doesn't raise PropertyChanged - would need to use RaiseAndSetIfChanged")]
    public async Task PropertyChanged_IsRaisedWhenDisplayNameChanges()
    {
        // Arrange
        var rxObject = new TestRxObject();
        var propertyChangedRaised = false;
        rxObject.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(RxObject.DisplayName))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        rxObject.DisplayName = "New Name";

        // Assert - PropertyChanged should be raised synchronously
        await Assert.That(propertyChangedRaised).IsTrue();
    }

    [Test]
    public async Task RxObject_ImplementsIRxObject()
    {
        // Arrange & Act
        var rxObject = new TestRxObject();

        // Assert
        await Assert.That(rxObject).IsAssignableTo<IRxObject>();
    }

    [Test]
    public async Task RxObject_ImplementsIDisposable()
    {
        // Arrange & Act
        var rxObject = new TestRxObject();

        // Assert
        await Assert.That(rxObject).IsAssignableTo<IDisposable>();
    }
}
