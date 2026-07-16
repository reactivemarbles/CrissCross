// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.WPF.Plot;

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Tests static, historic, live, styling, and technical-study plot APIs.</summary>
public sealed class TechnicalIndicatorsTests
{
    /// <summary>Provides the common price-series name used by indicator tests.</summary>
    private const string PriceSeriesName = "Price";

    /// <summary>Provides the common sample value used by timestamped source tests.</summary>
    private const double TimestampedSampleValue = 42;

    /// <summary>Verifies DateTime series are normalized to OLE Automation dates.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task DateTimeSeries_NormalizesOaDateAxis()
    {
        var timestamp = new DateTime(2026, 7, 15, 12, 30, 0, DateTimeKind.Utc);
        var series = PlotSeriesData.DateTime("Time", 0, [timestamp], [TimestampedSampleValue]);

        await Assert.That(series.XAxisKind).IsEqualTo(PlotXAxisKind.OADate);
        await Assert.That(series.X[0]).IsEqualTo(timestamp.ToOADate());
    }

    /// <summary>Verifies LTTB reduction preserves the time range and rendering budget.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task LargestTriangleThreeBuckets_ReducesAndPreservesEndpoints()
    {
        const int sourcePointCount = 10_000;
        const double wavePeriod = 20;
        const int renderBudget = 500;
        var x = Enumerable.Range(0, sourcePointCount).Select(static value => (double)value).ToArray();
        var y = x.Select(static value => Math.Sin(value / wavePeriod)).ToArray();
        var reduced = PlotDataReducer.LargestTriangleThreeBuckets(
            PlotSeriesData.Numeric("History", 0, x, y),
            renderBudget);

        await Assert.That(reduced.X).Count().IsEqualTo(renderBudget);
        await Assert.That(reduced.X[0]).IsEqualTo(x[0]);
        await Assert.That(reduced.X[^1]).IsEqualTo(x[^1]);
    }

    /// <summary>Verifies SMA warmup and rolling values.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task SimpleMovingAverage_UsesRollingWindow()
    {
        const int pointCount = 5;
        const int period = 3;
        var x = Enumerable.Range(0, pointCount).Select(static value => (double)value).ToArray();
        var y = Enumerable.Range(1, pointCount).Select(static value => (double)value).ToArray();
        var source = PlotSeriesData.Numeric("Value", 0, x, y);
        var result = TechnicalIndicators.SimpleMovingAverage(source, period);
        var expected = Enumerable
            .Range(period - 1, pointCount - period + 1)
            .Select(static value => (double)value)
            .ToArray();

        await Assert.That(double.IsNaN(result.Y[1])).IsTrue();
        await Assert.That(result.Y.Skip(period - 1).ToArray()).IsEquivalentTo(expected);
    }

    /// <summary>Verifies RSI reaches one hundred for a strictly increasing series.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task RelativeStrengthIndex_RisingSeriesReachesOneHundred()
    {
        const int sampleCount = 30;
        const double expectedRsi = 100;
        var values = Enumerable.Range(1, sampleCount).Select(static value => (double)value).ToArray();
        var source = PlotSeriesData.Numeric("Value", 0, values, values);
        var result = TechnicalIndicators.RelativeStrengthIndex(source);

        await Assert.That(result.Y[^1]).IsEqualTo(expectedRsi);
    }

    /// <summary>Verifies MACD histogram is the difference between MACD and its signal line.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task Macd_HistogramMatchesLineDifference()
    {
        const int sampleCount = 100;
        const double baseline = 50;
        const double wavePeriod = 5;
        var x = Enumerable.Range(0, sampleCount).Select(static value => (double)value).ToArray();
        var source = PlotSeriesData.Numeric(
            PriceSeriesName,
            0,
            x,
            x.Select(static value => baseline + Math.Sin(value / wavePeriod)).ToArray());
        var result = TechnicalIndicators.MovingAverageConvergenceDivergence(source);

        await Assert
            .That(result.Histogram.Y[^1])
            .IsEqualTo(result.Macd.Y[^1] - result.Signal.Y[^1]);
    }

    /// <summary>Verifies Bollinger Bands collapse to the mean for constant data.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task BollingerBands_ConstantSeriesCollapsesBands()
    {
        const int sampleCount = 30;
        const double constantValue = 12;
        var x = Enumerable.Range(0, sampleCount).Select(static value => (double)value).ToArray();
        var source = PlotSeriesData.Numeric(
            PriceSeriesName,
            0,
            x,
            Enumerable.Repeat(constantValue, x.Length).ToArray());
        var result = TechnicalIndicators.BollingerBands(source);

        await Assert.That(result.Middle.Y[^1]).IsEqualTo(constantValue);
        await Assert.That(result.Upper.Y[^1]).IsEqualTo(constantValue);
        await Assert.That(result.Lower.Y[^1]).IsEqualTo(constantValue);
    }

    /// <summary>Verifies Ichimoku produces displaced leading and lagging values.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task Ichimoku_ProducesDisplacedSeries()
    {
        const int sampleCount = 120;
        const double valueOffset = 10;
        const int displacement = 26;
        var x = Enumerable.Range(0, sampleCount).Select(static value => (double)value).ToArray();
        var source = PlotSeriesData.Numeric(
            PriceSeriesName,
            0,
            x,
            x.Select(static value => value + valueOffset).ToArray());
        var result = TechnicalIndicators.Ichimoku(source);

        await Assert.That(result.LeadingSpanA.Y.Any(double.IsFinite)).IsTrue();
        await Assert.That(result.LeadingSpanB.Y.Any(double.IsFinite)).IsTrue();
        await Assert.That(result.Lagging.Y[0]).IsEqualTo(source.Y[displacement]);
    }

    /// <summary>Verifies live DateTime sources preserve style, retention, and axis metadata.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task DateTimeLiveSource_EmitsStyledAppendUpdate()
    {
        const int retainedPointCount = 500;
        var timestamp = new DateTime(2026, 7, 15, 12, 30, 0, DateTimeKind.Utc);
        var style = new ReactivePlotSeriesStyle
        {
            Color = "#42A5F5",
            LineMode = PlotLineMode.LineAndMarkers,
        };
        var source = ReactivePlotSource.FromDateTimeLive(
            new PlotSeriesKey("Live", 1),
            Observable.Return((timestamp, TimestampedSampleValue)),
            maxPoints: retainedPointCount,
            style: style);

        var update = await source.Updates.FirstAsync();

        await Assert.That(source.XAxisKind).IsEqualTo(PlotXAxisKind.OADate);
        await Assert.That(update.Kind).IsEqualTo(ReactivePlotUpdateKind.Append);
        await Assert.That(update.MaxPoints).IsEqualTo(retainedPointCount);
        await Assert.That(update.Style).IsSameReferenceAs(style);
    }

    /// <summary>Verifies reactive studies retain source data and surface clear updates.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task ReactiveStudy_TransformsAppendAndClearUpdates()
    {
        const int appendedPointCount = 3;
        const int studyPeriod = 2;
        const double expectedLastAverage = 2.5;
        var key = new PlotSeriesKey("Live", 0);
        var appendedX = Enumerable
            .Range(0, appendedPointCount)
            .Select(static value => (double)value)
            .ToArray();
        var appendedY = Enumerable
            .Range(1, appendedPointCount)
            .Select(static value => (double)value)
            .ToArray();
        var updates = new[]
        {
            new ReactivePlotUpdate(
                key,
                PlotType.Signal,
                ReactivePlotUpdateKind.Append,
                appendedX,
                appendedY,
                PlotXAxisKind.Numeric,
                0),
            new ReactivePlotUpdate(
                key,
                PlotType.Signal,
                ReactivePlotUpdateKind.Clear,
                [],
                [],
                PlotXAxisKind.Numeric,
                1),
        };
        var source = ReactivePlotSource.FromUpdates(
            key,
            PlotType.Signal,
            updates.ToObservable(),
            PlotXAxisKind.Numeric);
        var study = ReactivePlotStudies.SimpleMovingAverage(source, studyPeriod);

        var results = await study.Updates.ToList();

        await Assert.That(results).Count().IsEqualTo(studyPeriod);
        await Assert.That(results[0].Kind).IsEqualTo(ReactivePlotUpdateKind.Replace);
        await Assert.That(results[0].Y[^1]).IsEqualTo(expectedLastAverage);
        await Assert.That(results[1].Kind).IsEqualTo(ReactivePlotUpdateKind.Clear);
    }
}
