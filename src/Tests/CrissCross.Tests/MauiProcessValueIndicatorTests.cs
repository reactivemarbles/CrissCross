// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Maui.UI.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CrissCross.Tests;

/// <summary>Exercises MAUI process-value projections from the shared industrial state model.</summary>
public sealed class MauiProcessValueIndicatorTests
{
    /// <summary>The process label used by layout and theme tests.</summary>
    private const string LevelLabel = "Level";

    /// <summary>The status child position in the composed readout.</summary>
    private const int StatusIndex = 3;

    /// <summary>The range child position in the composed readout.</summary>
    private const int RangeIndex = 2;

    /// <summary>The allowed rounding error for percentage projection.</summary>
    private const double PercentageTolerance = 1e-10;

    /// <summary>Defines the HighTemperature test value.</summary>
    private const double HighTemperature = 126.8;

    /// <summary>Defines the NormalFraction test value.</summary>
    private const double NormalFraction = 0.55;

    /// <summary>Defines the SpeedMaximum test value.</summary>
    private const int SpeedMaximum = 500;

    /// <summary>Defines the TemperatureMaximum test value.</summary>
    private const int TemperatureMaximum = 140;

    /// <summary>Defines the TemperatureAlarm test value.</summary>
    private const int TemperatureAlarm = 120;

    /// <summary>Defines the RangeMaximum test value.</summary>
    private const int RangeMaximum = 100;

    /// <summary>Defines the HighLimit test value.</summary>
    private const int HighLimit = 90;

    /// <summary>Defines the NormalReading test value.</summary>
    private const int NormalReading = 55;

    /// <summary>Defines the LowLimit test value.</summary>
    private const int LowLimit = 10;

    /// <summary>Verifies a missing state projects the shared safe fallback.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task State_WhenNull_ProjectsEmptyFallback()
    {
        var indicator = new ProcessValueIndicator();

        await Assert.That(indicator.State).IsNull();
        await Assert.That(indicator.Label).IsEqualTo("Process value");
        await Assert.That(indicator.DisplayText).IsEqualTo("—");
        await Assert.That(indicator.StatusText).IsEqualTo("Bad quality");
        await Assert.That(indicator.Percentage).IsEqualTo(0);
        await Assert.That(indicator.NormalizedValue).IsEqualTo(0);
        await Assert.That(indicator.HasValidValue).IsFalse();
        await Assert.That(indicator.IsAlarm).IsFalse();
        await Assert.That(indicator.Status).IsEqualTo(ProcessValueStatus.BadQuality);
        await Assert.That(SemanticProperties.GetDescription(indicator)).IsEqualTo("Process value — Bad quality");
    }

    /// <summary>Verifies normal state projects value, range, and accessible text.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task State_WhenNormal_ProjectsReadoutRangeAndStatus()
    {
        var state = new ProcessValueState("Tank level", NormalReading, "%", 0, RangeMaximum, LowLimit, HighLimit);
        var indicator = new ProcessValueIndicator { State = state };

        await Assert.That(indicator.Label).IsEqualTo("Tank level");
        await Assert.That(indicator.DisplayText).IsEqualTo("55 %");
        await Assert.That(indicator.StatusText).IsEqualTo("Normal");
        await Assert.That(Math.Abs(indicator.Percentage - NormalReading)).IsLessThan(PercentageTolerance);
        await Assert.That(indicator.NormalizedValue).IsEqualTo(NormalFraction);
        await Assert.That(indicator.HasValidValue).IsTrue();
        await Assert.That(indicator.IsAlarm).IsFalse();
        await Assert.That(indicator.Status).IsEqualTo(ProcessValueStatus.Normal);
        await Assert.That(SemanticProperties.GetDescription(indicator)).Contains("Tank level 55 % Normal");
    }

    /// <summary>Verifies alarm and quality states are surfaced without relying on color alone.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task State_WhenAlarmOrBadQuality_ProjectsStatusText()
    {
        var alarmState = new ProcessValueState("Pressure", HighTemperature, "bar", 0, TemperatureMaximum, new ProcessValueOptions { HighAlarmLimit = TemperatureAlarm });
        var alarm = new ProcessValueIndicator { State = alarmState };
        var badQualityState = new ProcessValueState("Flow", null, "L/min", 0, SpeedMaximum, new ProcessValueOptions { IsGoodQuality = false });
        var badQuality = new ProcessValueIndicator { State = badQualityState };

        await Assert.That(alarm.IsAlarm).IsTrue();
        await Assert.That(alarm.Status).IsEqualTo(ProcessValueStatus.HighAlarm);
        await Assert.That(alarm.StatusText).IsEqualTo("High alarm");
        await Assert.That(alarm.HasValidValue).IsTrue();
        await Assert.That(badQuality.IsAlarm).IsFalse();
        await Assert.That(badQuality.Status).IsEqualTo(ProcessValueStatus.BadQuality);
        await Assert.That(badQuality.StatusText).IsEqualTo("Bad quality");
        await Assert.That(badQuality.HasValidValue).IsFalse();
    }

    /// <summary>Verifies theme changes recolor the visible status and range without replacing the reading.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ThemeColors_WhenChanged_UpdateComposedViews()
    {
        var indicator = new ProcessValueIndicator
        {
            State = new(LevelLabel, NormalReading, "%", 0, RangeMaximum, LowLimit, HighLimit),
            NormalStatusColor = Colors.Green,
            AlarmStatusColor = Colors.Red,
            BadQualityStatusColor = Colors.Gray,
            InvalidConfigurationStatusColor = Colors.Orange,
            RangeTrackColor = Colors.Black
        };
        var layout = (VerticalStackLayout)indicator.Content;
        var status = (Label)layout.Children[StatusIndex];
        var track = (Grid)layout.Children[RangeIndex];
        var fill = (BoxView)track.Children[0];

        await Assert.That(status.TextColor).IsEqualTo(Colors.Green);
        await Assert.That(fill.Color).IsEqualTo(Colors.Green);
        await Assert.That(track.BackgroundColor).IsEqualTo(Colors.Black);
        indicator.NormalStatusColor = Colors.Blue;
        await Assert.That(status.TextColor).IsEqualTo(Colors.Blue);
        indicator.State = new(LevelLabel, RangeMaximum, "%", 0, RangeMaximum, LowLimit, HighLimit);
        await Assert.That(status.TextColor).IsEqualTo(Colors.Red);
        indicator.State = null;
        await Assert.That(status.TextColor).IsEqualTo(Colors.Gray);
        indicator.State = new(LevelLabel, NormalReading, "%", RangeMaximum, 0);
        await Assert.That(status.TextColor).IsEqualTo(Colors.Orange);
        await Assert.That(status.Text).IsEqualTo("Invalid configuration");
    }

    /// <summary>Verifies arranged range fills resize and clear when a reading becomes unavailable.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Range_WhenArranged_TracksReadingAndAvailableWidth()
    {
        const double initialWidth = 200;
        const double resizedWidth = 400;
        const double barHeight = 8;
        const double smallestReading = 0.01;
        const double minimumVisibleWidth = 2;
        var indicator = new ProcessValueIndicator { State = new(LevelLabel, NormalReading, "%", 0, RangeMaximum) };
        var layout = (VerticalStackLayout)indicator.Content;
        var track = (Grid)layout.Children[RangeIndex];
        var fill = (BoxView)track.Children[0];

        _ = ((IView)track).Arrange(new(0, 0, initialWidth, barHeight));
        await Assert.That(Math.Abs(fill.WidthRequest - (initialWidth * NormalFraction))).IsLessThan(PercentageTolerance);
        _ = ((IView)track).Arrange(new(0, 0, resizedWidth, barHeight));
        await Assert.That(Math.Abs(fill.WidthRequest - (resizedWidth * NormalFraction))).IsLessThan(PercentageTolerance);
        indicator.State = new(LevelLabel, smallestReading, "%", 0, RangeMaximum);
        await Assert.That(fill.WidthRequest).IsEqualTo(minimumVisibleWidth);
        indicator.State = null;
        await Assert.That(fill.WidthRequest).IsEqualTo(0);
    }
}
