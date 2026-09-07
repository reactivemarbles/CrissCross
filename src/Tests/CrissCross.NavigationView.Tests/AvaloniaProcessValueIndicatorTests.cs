// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Avalonia.UI.Controls;

namespace CrissCross.NavigationView.Tests;

/// <summary>Verifies Avalonia industrial readouts follow the shared state and reset contract.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class AvaloniaProcessValueIndicatorTests
{
    /// <summary>Verifies each condition updates every read-only projection before resetting to the safe fallback.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task StateTransitions_ProjectSharedConditionsAndReset()
    {
        const double maximum = 100;
        const double normal = 50;
        const double lowLimit = 10;
        const double highLimit = 90;
        const string label = "Level";
        ProcessValueState[] states =
        [
            new(label, normal, "%", 0, maximum, lowLimit, highLimit),
            new(label, 0, "%", 0, maximum, lowLimit, highLimit),
            new(label, maximum, "%", 0, maximum, lowLimit, highLimit),
            new(label, null, "%", 0, maximum),
            new(label, normal, "%", maximum, 0)
        ];
        var indicator = new ProcessValueIndicator();
        indicator.BeginInit();
        indicator.EndInit();
        foreach (var state in states)
        {
            indicator.State = state;
            await Assert.That(indicator.Label).IsEqualTo(state.Label);
            await Assert.That(indicator.DisplayText).IsEqualTo(state.DisplayText);
            await Assert.That(indicator.StatusText).IsEqualTo(state.StatusText);
            await Assert.That(indicator.Percentage).IsEqualTo(state.Percentage);
            await Assert.That(indicator.NormalizedValue).IsEqualTo(state.NormalizedValue);
            await Assert.That(indicator.HasValidValue).IsEqualTo(state.HasValidValue);
            await Assert.That(indicator.IsAlarm).IsEqualTo(state.IsAlarm);
            await Assert.That(indicator.Status).IsEqualTo(state.Status);
        }

        indicator.State = null;
        await Assert.That(indicator.Label).IsEqualTo("Process value");
        await Assert.That(indicator.DisplayText).IsEqualTo("—");
        await Assert.That(indicator.StatusText).IsEqualTo("Bad quality");
        await Assert.That(indicator.Percentage).IsEqualTo(0);
        await Assert.That(indicator.HasValidValue).IsFalse();
        await Assert.That(indicator.IsAlarm).IsFalse();
    }
}
