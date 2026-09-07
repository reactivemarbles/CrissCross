// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Tests;

/// <summary>Verifies industrial measurement status, range normalization, and invalid-data handling.</summary>
public sealed class ProcessValueStateTests
{
    /// <summary>Defines the QuarterRange test value.</summary>
    private const double QuarterRange = 0.25;

    /// <summary>Defines the RaisedHighLimit test value.</summary>
    private const int RaisedHighLimit = 110;

    /// <summary>Defines the RangeMaximum test value.</summary>
    private const int RangeMaximum = 100;

    /// <summary>Defines the HighLimit test value.</summary>
    private const int HighLimit = 90;

    /// <summary>Defines the QuarterReading test value.</summary>
    private const int QuarterReading = 25;

    /// <summary>Defines the LowLimit test value.</summary>
    private const int LowLimit = 10;

    /// <summary>Defines the TwoItemsPerPage test value.</summary>
    private const int TwoItemsPerPage = 2;

    /// <summary>Verifies the complete valid measurement snapshot.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task ValidMeasurementProjectsEngineeringUnitsAndRange()
    {
        var state = new ProcessValueState(" Pressure ", QuarterReading, " bar ", 0, RangeMaximum, LowLimit, HighLimit);

        await Assert.That(state.Label).IsEqualTo("Pressure");
        await Assert.That(state.Value).IsEqualTo(QuarterReading);
        await Assert.That(state.Unit).IsEqualTo("bar");
        await Assert.That(state.Minimum).IsEqualTo(0);
        await Assert.That(state.Maximum).IsEqualTo(RangeMaximum);
        await Assert.That(state.LowAlarmLimit).IsEqualTo(LowLimit);
        await Assert.That(state.HighAlarmLimit).IsEqualTo(HighLimit);
        await Assert.That(state.IsGoodQuality).IsTrue();
        await Assert.That(state.Status).IsEqualTo(ProcessValueStatus.Normal);
        await Assert.That(state.HasValidValue).IsTrue();
        await Assert.That(state.IsAlarm).IsFalse();
        await Assert.That(state.ValueText).IsEqualTo("25");
        await Assert.That(state.DisplayText).IsEqualTo("25 bar");
        await Assert.That(state.StatusText).IsEqualTo("Normal");
        await Assert.That(state.NormalizedValue).IsEqualTo(QuarterRange);
        await Assert.That(state.Percentage).IsEqualTo(QuarterReading);
    }

    /// <summary>Verifies inclusive low and high alarm boundaries without changing the measured value.</summary>
    /// <param name="value">The measurement.</param>
    /// <param name="expectedStatus">The expected alarm condition.</param>
    /// <param name="expectedText">The expected text condition.</param>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    [Arguments(-1, ProcessValueStatus.LowAlarm, "Low alarm")]
    [Arguments(10, ProcessValueStatus.LowAlarm, "Low alarm")]
    [Arguments(90, ProcessValueStatus.HighAlarm, "High alarm")]
    [Arguments(101, ProcessValueStatus.HighAlarm, "High alarm")]
    public async Task AlarmLimitsAreInclusive(double value, ProcessValueStatus expectedStatus, string expectedText)
    {
        var state = new ProcessValueState(null, value, null, 0, RangeMaximum, LowLimit, HighLimit);

        await Assert.That(state.Status).IsEqualTo(expectedStatus);
        await Assert.That(state.StatusText).IsEqualTo(expectedText);
        await Assert.That(state.IsAlarm).IsTrue();
        await Assert.That(state.HasValidValue).IsTrue();
        await Assert.That(state.Value).IsEqualTo(value);
        await Assert.That(state.Label).IsEmpty();
        await Assert.That(state.Unit).IsEmpty();
        await Assert.That(state.DisplayText).IsEqualTo(state.ValueText);
    }

    /// <summary>Verifies a single configured alarm and an absent alarm do not create spurious conditions.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task OptionalAlarmLimitsAreIndependent()
    {
        await Assert.That(new ProcessValueState(null, 0, null, 0, RangeMaximum).Status).IsEqualTo(ProcessValueStatus.Normal);
        await Assert.That(new ProcessValueState(null, 0, null, 0, RangeMaximum, null, HighLimit).Status).IsEqualTo(ProcessValueStatus.Normal);
        await Assert.That(new ProcessValueState(null, RangeMaximum, null, 0, RangeMaximum, LowLimit, null).Status).IsEqualTo(ProcessValueStatus.Normal);
    }

    /// <summary>Verifies bad data is visibly unavailable and cannot appear as a healthy measurement.</summary>
    /// <param name="value">The invalid measurement.</param>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    [Arguments(null)]
    [Arguments(double.NaN)]
    [Arguments(double.PositiveInfinity)]
    [Arguments(double.NegativeInfinity)]
    public async Task InvalidMeasurementsShowBadQuality(double? value)
    {
        var state = new ProcessValueState(null, value, null, 0, RangeMaximum);

        await Assert.That(state.Status).IsEqualTo(ProcessValueStatus.BadQuality);
        await Assert.That(state.StatusText).IsEqualTo("Bad quality");
        await Assert.That(state.HasValidValue).IsFalse();
        await Assert.That(state.IsAlarm).IsFalse();
        await Assert.That(state.ValueText).IsEqualTo("—");
        await Assert.That(state.NormalizedValue).IsEqualTo(0);
    }

    /// <summary>Verifies bad source quality takes precedence over alarm thresholds.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task BadSourceQualityOverridesAlarm()
    {
        var options = new ProcessValueOptions { LowAlarmLimit = LowLimit, HighAlarmLimit = HighLimit, IsGoodQuality = false };
        var state = new ProcessValueState(null, RangeMaximum, null, 0, RangeMaximum, options);
        options.IsGoodQuality = true;
        options.HighAlarmLimit = RaisedHighLimit;

        await Assert.That(state.Status).IsEqualTo(ProcessValueStatus.BadQuality);
        await Assert.That(state.IsGoodQuality).IsFalse();
    }

    /// <summary>Verifies invalid configuration takes precedence over unavailable data.</summary>
    /// <param name="minimum">The range minimum.</param>
    /// <param name="maximum">The range maximum.</param>
    /// <param name="low">The optional low alarm.</param>
    /// <param name="high">The optional high alarm.</param>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    [Arguments(0, 0, null, null)]
    [Arguments(100, 0, null, null)]
    [Arguments(double.NaN, 100, null, null)]
    [Arguments(double.NegativeInfinity, 100, null, null)]
    [Arguments(0, double.NaN, null, null)]
    [Arguments(0, double.PositiveInfinity, null, null)]
    [Arguments(0, 100, double.NaN, null)]
    [Arguments(0, 100, double.PositiveInfinity, null)]
    [Arguments(0, 100, null, double.NaN)]
    [Arguments(0, 100, null, double.NegativeInfinity)]
    [Arguments(0, 100, 50, 50)]
    [Arguments(0, 100, 90, 10)]
    public async Task InvalidConfigurationCannotDisplayNormal(double minimum, double maximum, double? low, double? high)
    {
        var state = new ProcessValueState(null, null, null, minimum, maximum, low, high);

        await Assert.That(state.Status).IsEqualTo(ProcessValueStatus.InvalidConfiguration);
        await Assert.That(state.StatusText).IsEqualTo("Invalid configuration");
        await Assert.That(state.HasValidValue).IsFalse();
        await Assert.That(state.NormalizedValue).IsEqualTo(0);
    }

    /// <summary>Verifies clamping at and beyond both endpoints, plus overflow-safe range normalization.</summary>
    /// <param name="value">The measurement.</param>
    /// <param name="minimum">The range minimum.</param>
    /// <param name="maximum">The range maximum.</param>
    /// <param name="expected">The expected normalized result.</param>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    [Arguments(-1, 0, 100, 0)]
    [Arguments(0, 0, 100, 0)]
    [Arguments(100, 0, 100, 1)]
    [Arguments(101, 0, 100, 1)]
    [Arguments(0, -double.MaxValue, double.MaxValue, 0.5)]
    [Arguments(0, -double.Epsilon, double.Epsilon, 0.5)]
    public async Task RangeNormalizationIsFiniteAndClamped(double value, double minimum, double maximum, double expected)
    {
        var state = new ProcessValueState(null, value, null, minimum, maximum);

        await Assert.That(state.NormalizedValue).IsEqualTo(expected);
    }

    /// <summary>Verifies paging over the maximum item count does not overflow the final item number.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task FinalPageOfLargeDatasetDoesNotOverflow()
    {
        var state = new PaginationState(int.MaxValue, TwoItemsPerPage, int.MaxValue);

        await Assert.That(state.FirstItemNumber).IsEqualTo(int.MaxValue);
        await Assert.That(state.LastItemNumber).IsEqualTo(int.MaxValue);
        await Assert.That(state.SummaryText).IsEqualTo("2147483647-2147483647 of 2147483647");
    }

    /// <summary>Verifies an unrepresentable skip offset fails explicitly instead of wrapping to a negative offset.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task UnrepresentablePageOffsetThrowsWithoutCorruptingDisplayText()
    {
        var request = new PageRequest(int.MaxValue, TwoItemsPerPage);

        await Assert.That(() => request.Offset).Throws<OverflowException>();
        await Assert.That(request.DisplayText).IsEqualTo("Page 2147483648, 2 per page");
        await Assert.That(new PageRequest(int.MaxValue, 1).Offset).IsEqualTo(int.MaxValue);
    }
}
