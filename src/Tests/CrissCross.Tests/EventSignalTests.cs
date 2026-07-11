// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Tests;

/// <summary>Tests for EventSignal.</summary>
public class EventSignalTests
{
    /// <summary>Provides the From_ForwardsEventArgsAndUnsubscribesDeterministically member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task From_ForwardsEventArgsAndUnsubscribesDeterministically()
    {
        // Arrange
        var source = new EventSource();
        var received = new List<TestEventArgs>();
        var args = new TestEventArgs("first");
        var afterDisposeArgs = new TestEventArgs("after-dispose");
        var observable = CreateEventSignal(source);

        // Act
        var subscription = observable.Subscribe(received.Add);
        source.Raise(args);
        subscription.Dispose();
        subscription.Dispose();
        source.Raise(afterDisposeArgs);

        // Assert
        await Assert.That(source.AddCount).IsEqualTo(1);
        await Assert.That(source.RemoveCount).IsEqualTo(1);
        await Assert.That(received.Count).IsEqualTo(1);
        await Assert.That(ReferenceEquals(received[0], args)).IsTrue();
    }

    /// <summary>Provides the CreateEventSignal member.</summary>
    /// <param name="source">The source value.</param>
    /// <returns>The result.</returns>
    private static IObservable<TestEventArgs> CreateEventSignal(EventSource source) =>
        EventSignal.From<TestEventArgs>(source.AddHandler, source.RemoveHandler);

    /// <summary>Provides the TestEventArgs member.</summary>
    private sealed class TestEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="TestEventArgs"/> class.</summary>
        /// <param name="value">The value.</param>
        public TestEventArgs(string value)
        {
            Value = value;
        }

        /// <summary>Gets the value.</summary>
        public string Value { get; }
    }

    /// <summary>Provides the EventSource member.</summary>
    private sealed class EventSource
    {
        /// <summary>Provides the _raised member.</summary>
        private EventHandler<TestEventArgs>? _raised;

        /// <summary>Provides the Raised member.</summary>
        public event EventHandler<TestEventArgs> Raised
        {
            add
            {
                AddCount++;
                _raised += value;
            }

            remove
            {
                RemoveCount++;
                _raised -= value;
            }
        }

        /// <summary>Gets the value.</summary>
        public int AddCount { get; private set; }

        /// <summary>Gets the value.</summary>
        public int RemoveCount { get; private set; }

        /// <summary>Provides the AddHandler member.</summary>
        /// <param name="handler">The handler to subscribe.</param>
        public void AddHandler(EventHandler<TestEventArgs> handler)
        {
            Raised += handler;
        }

        /// <summary>Provides the RemoveHandler member.</summary>
        /// <param name="handler">The handler to unsubscribe.</param>
        public void RemoveHandler(EventHandler<TestEventArgs> handler)
        {
            Raised -= handler;
        }

        /// <summary>Provides the Raise member.</summary>
        /// <param name="args">The args value.</param>
        public void Raise(TestEventArgs args) => _raised?.Invoke(this, args);
    }
}
