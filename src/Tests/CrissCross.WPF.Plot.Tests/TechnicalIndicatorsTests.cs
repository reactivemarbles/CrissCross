// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.Plot;
#else
using CrissCross.WPF.Plot;
#endif

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
        var x = CreateSequentialValues(0, sourcePointCount);
        var y = CreateSineValues(x, 0, wavePeriod);
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
        var x = CreateSequentialValues(0, pointCount);
        var y = CreateSequentialValues(1, pointCount);
        var source = PlotSeriesData.Numeric("Value", 0, x, y);
        var result = TechnicalIndicators.SimpleMovingAverage(source, period);
        var expected = CreateSequentialValues(period - 1, pointCount - period + 1);

        await Assert.That(double.IsNaN(result.Y[1])).IsTrue();
        await Assert.That(CopyValuesFrom(result.Y, period - 1)).IsEquivalentTo(expected);
    }

    /// <summary>Verifies RSI reaches one hundred for a strictly increasing series.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task RelativeStrengthIndex_RisingSeriesReachesOneHundred()
    {
        const int sampleCount = 30;
        const double expectedRsi = 100;
        var values = CreateSequentialValues(1, sampleCount);
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
        var x = CreateSequentialValues(0, sampleCount);
        var source = PlotSeriesData.Numeric(
            PriceSeriesName,
            0,
            x,
            CreateSineValues(x, baseline, wavePeriod));
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
        var x = CreateSequentialValues(0, sampleCount);
        var source = PlotSeriesData.Numeric(
            PriceSeriesName,
            0,
            x,
            CreateConstantValues(constantValue, x.Length));
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
        var x = CreateSequentialValues(0, sampleCount);
        var source = PlotSeriesData.Numeric(
            PriceSeriesName,
            0,
            x,
            AddOffset(x, valueOffset));
        var result = TechnicalIndicators.Ichimoku(source);

        await Assert.That(ContainsFiniteValue(result.LeadingSpanA.Y)).IsTrue();
        await Assert.That(ContainsFiniteValue(result.LeadingSpanB.Y)).IsTrue();
        await Assert.That(result.Lagging.Y[0]).IsEqualTo(source.Y[displacement]);
    }

    /// <summary>Verifies live DateTime sources preserve style, retention, and axis metadata.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task DateTimeLiveSource_EmitsStyledAppendUpdate()
    {
        const int retainedPointCount = 500;
        var timestamp = new DateTime(2026, 7, 15, 12, 30, 0, DateTimeKind.Utc);
        var style = new ReactivePlotSeriesStyle { Color = "#42A5F5", LineMode = PlotLineMode.LineAndMarkers };
        PlotSeriesKey key = new("Live", 1);
        var source = ReactivePlotSource.FromDateTimeLive(
            key,
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
        var appendedX = CreateSequentialValues(0, appendedPointCount);
        var appendedY = CreateSequentialValues(1, appendedPointCount);
        ReactivePlotUpdate[] updates =
        [
            new(
                key,
                PlotType.Signal,
                ReactivePlotUpdateKind.Append,
                appendedX,
                appendedY,
                PlotXAxisKind.Numeric,
                0),
            new(
                key,
                PlotType.Signal,
                ReactivePlotUpdateKind.Clear,
                [],
                [],
                PlotXAxisKind.Numeric,
                1),
        ];
        var source = ReactivePlotSource.FromUpdates(
            key,
            PlotType.Signal,
            updates.ToObservable(),
            PlotXAxisKind.Numeric);
        var study = ReactivePlotStudies.SimpleMovingAverage(source, studyPeriod);

        List<ReactivePlotUpdate> results = [];
        using var subscription = study.Updates.Subscribe(results.Add);

        await Assert.That(results).Count().IsEqualTo(studyPeriod);
        await Assert.That(results[0].Kind).IsEqualTo(ReactivePlotUpdateKind.Replace);
        await Assert.That(results[0].Y[^1]).IsEqualTo(expectedLastAverage);
        await Assert.That(results[1].Kind).IsEqualTo(ReactivePlotUpdateKind.Clear);
    }

    /// <summary>Creates sequential double values.</summary>
    /// <param name="start">The first value.</param>
    /// <param name="count">The number of values to create.</param>
    /// <returns>The sequential values.</returns>
    private static double[] CreateSequentialValues(int start, int count)
    {
        var values = new double[count];
        for (var index = 0; index < count; index++)
        {
            values[index] = start + index;
        }

        return values;
    }

    /// <summary>Creates sine values from source X values.</summary>
    /// <param name="coordinates">The source X values.</param>
    /// <param name="baseline">The baseline value.</param>
    /// <param name="wavePeriod">The sine-wave period.</param>
    /// <returns>The generated Y values.</returns>
    private static double[] CreateSineValues(double[] coordinates, double baseline, double wavePeriod)
    {
        var values = new double[coordinates.Length];
        for (var index = 0; index < coordinates.Length; index++)
        {
            values[index] = baseline + Math.Sin(coordinates[index] / wavePeriod);
        }

        return values;
    }

    /// <summary>Creates repeated values.</summary>
    /// <param name="value">The value to repeat.</param>
    /// <param name="count">The number of values to create.</param>
    /// <returns>The repeated values.</returns>
    private static double[] CreateConstantValues(double value, int count)
    {
        var values = new double[count];
        for (var index = 0; index < count; index++)
        {
            values[index] = value;
        }

        return values;
    }

    /// <summary>Adds an offset to each source value.</summary>
    /// <param name="source">The source values.</param>
    /// <param name="offset">The offset to add.</param>
    /// <returns>The offset values.</returns>
    private static double[] AddOffset(double[] source, double offset)
    {
        var values = new double[source.Length];
        for (var index = 0; index < source.Length; index++)
        {
            values[index] = source[index] + offset;
        }

        return values;
    }

    /// <summary>Copies values from the requested start index.</summary>
    /// <param name="source">The source values.</param>
    /// <param name="startIndex">The first index to copy.</param>
    /// <returns>The copied tail values.</returns>
    private static double[] CopyValuesFrom(IReadOnlyList<double> source, int startIndex)
    {
        var values = new double[source.Count - startIndex];
        for (var index = startIndex; index < source.Count; index++)
        {
            values[index - startIndex] = source[index];
        }

        return values;
    }

    /// <summary>Determines whether a sequence contains a finite value.</summary>
    /// <param name="values">The values to inspect.</param>
    /// <returns>A value indicating whether a finite value exists.</returns>
    private static bool ContainsFiniteValue(IEnumerable<double> values)
    {
        foreach (var value in values)
        {
            if (double.IsFinite(value))
            {
                return true;
            }
        }

        return false;
    }
}
