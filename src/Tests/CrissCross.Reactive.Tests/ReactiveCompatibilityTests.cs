// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using CrissCross.Reactive;
using ReactiveUI.Reactive;
using ReactiveUI.Reactive.Builder;

namespace CrissCross.Reactive.Tests;

/// <summary>Verifies System.Reactive compatibility for the shared reactive package source.</summary>
public sealed class ReactiveCompatibilityTests
{
    /// <summary>The projection multiplier used by the compatibility stream.</summary>
    private const int ProjectionMultiplier = 2;

    /// <summary>The first source value.</summary>
    private const int FirstValue = 3;

    /// <summary>The second source value.</summary>
    private const int SecondValue = 7;

    /// <summary>Verifies that ReactiveUI property streams compose with System.Reactive operators.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task PropertyStream_ComposesWithSystemReactiveOperators()
    {
        _ = RxAppBuilder.CreateReactiveUIBuilder().WithCoreServices().BuildApp();
        using var viewModel = new TestViewModel();
        var observedValues = new List<int>();
        using var subscription = viewModel
            .WhenAnyValue(x => x.Value)
            .Skip(1)
            .Select(value => value * ProjectionMultiplier)
            .Subscribe(observedValues.Add);

        viewModel.Value = FirstValue;
        viewModel.Value = SecondValue;

        await Assert
            .That(observedValues)
            .IsEquivalentTo([FirstValue * ProjectionMultiplier, SecondValue * ProjectionMultiplier]);
        await Assert.That(typeof(RxObject).Namespace).IsEqualTo("CrissCross.Reactive");
    }

    /// <summary>A minimal reactive view model used by compatibility tests.</summary>
    private sealed class TestViewModel : RxObject
    {
        /// <summary>Gets or sets the reactive value.</summary>
        public int Value
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }
}
