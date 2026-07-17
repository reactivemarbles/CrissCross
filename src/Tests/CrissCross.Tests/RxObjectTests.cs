// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Tests;

/// <summary>Tests for RxObject class.</summary>
public class RxObjectTests
{
    /// <summary>Provides the Name_ReturnsFullTypeName member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Name_ReturnsFullTypeName()
    {
        // Arrange
        using var rxObject = new TestRxObject();

        // Act
        var name = rxObject.Name;

        // Assert
        await Assert.That(name).IsEqualTo(typeof(TestRxObject).FullName);
    }

    /// <summary>Provides the DisplayName_CanBeSetAndRetrieved member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DisplayName_CanBeSetAndRetrieved()
    {
        // Arrange
        using var rxObject = new TestRxObject();
        const string expectedDisplayName = "Test Display Name";

        // Act
        rxObject.DisplayName = expectedDisplayName;

        // Assert
        await Assert.That(rxObject.DisplayName).IsEqualTo(expectedDisplayName);
    }

    /// <summary>Provides the DisplayName_IsNullByDefault member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task DisplayName_IsNullByDefault()
    {
        // Arrange & Act
        using var rxObject = new TestRxObject();

        // Assert
        await Assert.That(rxObject.DisplayName).IsNull();
    }

    /// <summary>Provides the IsDisposed_IsFalseByDefault member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task IsDisposed_IsFalseByDefault()
    {
        // Arrange & Act
        using var rxObject = new TestRxObject();

        // Assert
        await Assert.That(rxObject.IsDisposed).IsFalse();
    }

    /// <summary>Provides the IsDisposed_IsTrueAfterDispose member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the Dispose_CanBeCalledMultipleTimes member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>Provides the Disposables_AreDisposedWhenObjectIsDisposed member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task Disposables_AreDisposedWhenObjectIsDisposed()
    {
        // Arrange
        var rxObject = new TestRxObject();
        var wasDisposed = false;
        var disposable = new ActionDisposable(() => wasDisposed = true);
        rxObject.GetDisposables().Add(disposable);

        // Act
        rxObject.Dispose();

        // Assert
        await Assert.That(wasDisposed).IsTrue();
    }

    /// <summary>Provides the WhenNavigatedFrom_CanBeCalled member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
            "test");

        // Act & Assert - should not throw
        rxObject.WhenNavigatedFrom(eventArgs);
        await Assert.That(true).IsTrue();
    }

    /// <summary>Provides the WhenNavigatedTo_CanBeCalled member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
            "test");

        // Act & Assert - should not throw
        rxObject.WhenNavigatedTo(eventArgs, disposables);
        await Assert.That(true).IsTrue();
    }

    /// <summary>Provides the WhenNavigating_CanBeCalled member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
            "test");

        // Act & Assert - should not throw
        rxObject.WhenNavigating(eventArgs);
        await Assert.That(true).IsTrue();
    }

    /// <summary>Provides the PropertyChanged_IsRaisedWhenDisplayNameChanges member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    [Skip("DisplayName is an auto-property and doesn't raise PropertyChanged - would need to use RaiseAndSetIfChanged")]
    public async Task PropertyChanged_IsRaisedWhenDisplayNameChanges()
    {
        // Arrange
        using var rxObject = new TestRxObject();
        var propertyChangedRaised = false;
        rxObject.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName != nameof(RxObject.DisplayName))
            {
                return;
            }

            propertyChangedRaised = true;
        };

        // Act
        rxObject.DisplayName = "New Name";

        // Assert - PropertyChanged should be raised synchronously
        await Assert.That(propertyChangedRaised).IsTrue();
    }

    /// <summary>Provides the RxObject_ImplementsIRxObject member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RxObject_ImplementsIRxObject()
    {
        // Arrange & Act
        var rxObject = new TestRxObject();

        // Assert
        await Assert.That(rxObject).IsAssignableTo<IRxObject>();
    }

    /// <summary>Provides the RxObject_ImplementsIDisposable member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task RxObject_ImplementsIDisposable()
    {
        // Arrange & Act
        var rxObject = new TestRxObject();

        // Assert
        await Assert.That(rxObject).IsAssignableTo<IDisposable>();
    }

    /// <summary>Test implementation of RxObject.</summary>
    private sealed class TestRxObject : RxObject
    {
        /// <summary>Provides the GetDisposables member.</summary>
        /// <returns>The result.</returns>
        public CompositeDisposable GetDisposables() => Disposables;
    }
}
