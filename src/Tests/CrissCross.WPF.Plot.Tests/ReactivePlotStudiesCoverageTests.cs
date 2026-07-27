// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.Plot;
#else
using CrissCross.WPF.Plot;
#endif

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Verifies reactive technical-study composition and plot metadata behavior.</summary>
public sealed class ReactivePlotStudiesCoverageTests
{
    /// <summary>The source series name used by study tests.</summary>
    private const string PriceSeriesName = "Price";

    /// <summary>The source series name used by Ichimoku tests.</summary>
    private const string MarketSeriesName = "Market";

    /// <summary>The default source axis.</summary>
    private const int DefaultAxisIndex = 1;

    /// <summary>The selected study output axis.</summary>
    private const int StudyAxisIndex = 3;

    /// <summary>The selected MACD output axis.</summary>
    private const int MacdAxisIndex = 4;

    /// <summary>The selected Bollinger output axis.</summary>
    private const int BollingerAxisIndex = 5;

    /// <summary>The selected Ichimoku output axis.</summary>
    private const int IchimokuAxisIndex = 6;

    /// <summary>The rolling period used by moving-average studies.</summary>
    private const int MovingAveragePeriod = 2;

    /// <summary>The rolling period used by Bollinger studies.</summary>
    private const int BollingerPeriod = 2;

    /// <summary>The standard-deviation multiplier used by Bollinger studies.</summary>
    private const double BollingerDeviation = 1.0;

    /// <summary>The maximum number of retained points.</summary>
    private const int RetainedPointLimit = 3;

    /// <summary>The expected number of source updates.</summary>
    private const int ExpectedUpdateCount = 3;

    /// <summary>The expected number of MACD studies.</summary>
    private const int ExpectedMacdStudyCount = 3;

    /// <summary>The expected number of Bollinger studies.</summary>
    private const int ExpectedBollingerStudyCount = 3;

    /// <summary>The expected number of Ichimoku studies.</summary>
    private const int ExpectedIchimokuStudyCount = 5;

    /// <summary>The number of values used to calculate MACD.</summary>
    private const int MacdValueCount = 5;

    /// <summary>The number of values used to calculate Ichimoku.</summary>
    private const int IchimokuValueCount = 80;

    /// <summary>The first generated value.</summary>
    private const int FirstValue = 1;

    /// <summary>The first generated X value.</summary>
    private const int FirstXValue = 0;

    /// <summary>The second update sequence.</summary>
    private const long SecondSequence = 1L;

    /// <summary>The third update sequence.</summary>
    private const long ThirdSequence = 2L;

    /// <summary>The expected styled-update sequence.</summary>
    private const long ExpectedSequence = 9L;

    /// <summary>The first expected retained X value.</summary>
    private const double FirstRetainedXValue = 3.0;

    /// <summary>The second expected retained X value.</summary>
    private const double SecondRetainedXValue = 4.0;

    /// <summary>The third expected retained X value.</summary>
    private const double ThirdRetainedXValue = 5.0;

    /// <summary>The first expected rolling average.</summary>
    private const double FirstExpectedAverage = 4.5;

    /// <summary>The second expected rolling average.</summary>
    private const double SecondExpectedAverage = 5.5;

    /// <summary>The offset used to generate three update values.</summary>
    private const long ThirdPointOffset = 2L;

    /// <summary>Verifies a moving average retains the declared limit and resets on replacement data.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task SimpleMovingAverage_RetainsLimitAndResetsOnReplace()
    {
        var sourceUpdates = new Signal<ReactivePlotUpdate>();
        PlotSeriesKey sourceKey = new(PriceSeriesName, DefaultAxisIndex);
        var source = ReactivePlotSource.FromUpdates(sourceKey, PlotType.Signal, sourceUpdates, PlotXAxisKind.Numeric);
        var style = new ReactivePlotSeriesStyle { Color = "#112233" };
        var study = ReactivePlotStudies.SimpleMovingAverage(source, MovingAveragePeriod, StudyAxisIndex, style);
        List<ReactivePlotUpdate> results = [];
        using var subscription = study.Updates.Subscribe(results.Add);

        sourceUpdates.OnNext(CreateUpdate(ReactivePlotUpdateKind.Append, FirstXValue, FirstValue, DefaultAxisIndex, RetainedPointLimit, FirstValue));
        sourceUpdates.OnNext(CreateUpdate(ReactivePlotUpdateKind.Append, RetainedPointLimit, RetainedPointLimit + FirstValue, DefaultAxisIndex, RetainedPointLimit, SecondSequence));
        sourceUpdates.OnNext(CreateUpdate(ReactivePlotUpdateKind.Replace, ExpectedSequence, ExpectedSequence + FirstValue, DefaultAxisIndex, null, ThirdSequence));

        PlotSeriesKey expectedKey = new("Price SMA(2)", StudyAxisIndex);
        await Assert.That(results).Count().IsEqualTo(ExpectedUpdateCount);
        await Assert.That(results[0].Key).IsEqualTo(expectedKey);
        await Assert.That(results[1].X).IsEquivalentTo([FirstRetainedXValue, SecondRetainedXValue, ThirdRetainedXValue]);
        await Assert.That(results[1].Y[1]).IsEqualTo(FirstExpectedAverage);
        await Assert.That(results[1].Y[2]).IsEqualTo(SecondExpectedAverage);
        await Assert.That(double.IsNaN(results[2].Y[0])).IsTrue();
        await Assert.That(results[2].Style).IsSameReferenceAs(style);
    }

    /// <summary>Verifies MACD studies expose the intended plot types, keys, styles, and calculated data.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task MacdStudies_ExposeSeriesMetadataAndCalculatedData()
    {
        var staticSource = CreateStaticSource(PriceSeriesName, DefaultAxisIndex, MacdValueCount);
        var studies = ReactivePlotStudies.MovingAverageConvergenceDivergence(staticSource.Source, MovingAveragePeriod, RetainedPointLimit, MovingAveragePeriod, MacdAxisIndex);
        List<ReactivePlotUpdate> updates = ReadFirstUpdates(studies, staticSource);

        PlotSeriesKey macdKey = new("Price MACD", MacdAxisIndex);
        PlotSeriesKey signalKey = new("Price MACD Signal", MacdAxisIndex);
        PlotSeriesKey histogramKey = new("Price MACD Histogram", MacdAxisIndex);
        await Assert.That(updates).Count().IsEqualTo(ExpectedMacdStudyCount);
        await Assert.That(updates[0].Key).IsEqualTo(macdKey);
        await Assert.That(updates[1].Key).IsEqualTo(signalKey);
        await Assert.That(updates[2].Key).IsEqualTo(histogramKey);
        await Assert.That(updates[0].PlotType).IsEqualTo(PlotType.Line);
        await Assert.That(updates[1].PlotType).IsEqualTo(PlotType.Line);
        await Assert.That(updates[2].PlotType).IsEqualTo(PlotType.Bar);
        await Assert.That(updates[2].Style!.Color).IsEqualTo("#66BB6A");
        await Assert.That(updates[2].Y[^1]).IsEqualTo(updates[0].Y[^1] - updates[1].Y[^1]);
    }

    /// <summary>Verifies Bollinger overlays retain source axis metadata and calculate all three lines.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task BollingerBands_ExposeStyledLinesForSelectedAxis()
    {
        var staticSource = CreateStaticSource(PriceSeriesName, DefaultAxisIndex, RetainedPointLimit + FirstValue);
        var bands = ReactivePlotStudies.BollingerBands(staticSource.Source, BollingerPeriod, BollingerDeviation, BollingerAxisIndex);
        List<ReactivePlotUpdate> updates = ReadFirstUpdates(bands, staticSource);

        PlotSeriesKey middleKey = new("Price Bollinger Middle", BollingerAxisIndex);
        PlotSeriesKey upperKey = new("Price Bollinger Upper", BollingerAxisIndex);
        PlotSeriesKey lowerKey = new("Price Bollinger Lower", BollingerAxisIndex);
        await Assert.That(updates).Count().IsEqualTo(ExpectedBollingerStudyCount);
        await Assert.That(updates[0].Key).IsEqualTo(middleKey);
        await Assert.That(updates[1].Key).IsEqualTo(upperKey);
        await Assert.That(updates[2].Key).IsEqualTo(lowerKey);
        await Assert.That(updates[0].Style!.Color).IsEqualTo("#42A5F5");
        await Assert.That(updates[1].Style!.Color).IsEqualTo("#90CAF9");
        await Assert.That(updates[2].Style!.Color).IsEqualTo("#90CAF9");
        await Assert.That(updates[1].Y[^1] > updates[0].Y[^1]).IsTrue();
        await Assert.That(updates[2].Y[^1] < updates[0].Y[^1]).IsTrue();
    }

    /// <summary>Verifies Ichimoku overlays create all named lines with their fixed style palette.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task Ichimoku_ExposesAllNamedStyledStudies()
    {
        var staticSource = CreateStaticSource(MarketSeriesName, DefaultAxisIndex, IchimokuValueCount);
        var studies = ReactivePlotStudies.Ichimoku(staticSource.Source, IchimokuAxisIndex);
        List<ReactivePlotUpdate> updates = ReadFirstUpdates(studies, staticSource);

        await Assert.That(updates).Count().IsEqualTo(ExpectedIchimokuStudyCount);
        PlotSeriesKey conversionKey = new("Market Ichimoku Conversion", IchimokuAxisIndex);
        PlotSeriesKey baseKey = new("Market Ichimoku Base", IchimokuAxisIndex);
        PlotSeriesKey spanAKey = new("Market Ichimoku Span A", IchimokuAxisIndex);
        PlotSeriesKey spanBKey = new("Market Ichimoku Span B", IchimokuAxisIndex);
        PlotSeriesKey laggingKey = new("Market Ichimoku Lagging", IchimokuAxisIndex);
        await Assert.That(updates[0].Key).IsEqualTo(conversionKey);
        await Assert.That(updates[1].Key).IsEqualTo(baseKey);
        await Assert.That(updates[2].Key).IsEqualTo(spanAKey);
        await Assert.That(updates[3].Key).IsEqualTo(spanBKey);
        await Assert.That(updates[4].Key).IsEqualTo(laggingKey);
        await Assert.That(updates[0].Style!.Color).IsEqualTo("#EF5350");
        await Assert.That(updates[1].Style!.Color).IsEqualTo("#42A5F5");
        await Assert.That(updates[2].Style!.Color).IsEqualTo("#66BB6A");
        await Assert.That(updates[3].Style!.Color).IsEqualTo("#EF5350");
        await Assert.That(updates[4].Style!.Color).IsEqualTo("#AB47BC");
    }

    /// <summary>Verifies static themes and the series-to-update convenience overloads preserve their public metadata.</summary>
    /// <returns>A task representing the assertion operation.</returns>
    [Test]
    public async Task ThemesAndSeriesUpdates_PreserveExpectedMetadata()
    {
        var style = new ReactivePlotSeriesStyle { Color = "#123456" };
        var series = PlotSeriesData.Numeric(
            "Orders",
            DefaultAxisIndex,
            [FirstValue, FirstRetainedXValue],
            [SecondRetainedXValue, ThirdRetainedXValue]);
        var defaultUpdate = series.ToUpdate(PlotType.Scatter);
        var styledUpdate = series.ToUpdate(PlotType.Line, style);
        var sequencedUpdate = series.ToUpdate(PlotType.Bar, ExpectedSequence, style);

        await Assert.That(ReactivePlotTheme.Dark.FigureBackground).IsEqualTo("#252526");
        await Assert.That(ReactivePlotTheme.Light.DataBackground).IsEqualTo("#FFFFFF");
        await Assert.That(ReactivePlotTheme.Light.Axis).IsEqualTo("#202020");
        await Assert.That(defaultUpdate.Sequence).IsEqualTo(0L);
        await Assert.That(defaultUpdate.Style).IsNull();
        await Assert.That(styledUpdate.Style).IsSameReferenceAs(style);
        await Assert.That(sequencedUpdate.Sequence).IsEqualTo(ExpectedSequence);
        await Assert.That(sequencedUpdate.PlotType).IsEqualTo(PlotType.Bar);
        await Assert.That(static () => new PlotSeriesData(new("Broken", 0), [1.0], [])).Throws<ArgumentException>();
    }

    /// <summary>Creates a source that publishes one static replacement update.</summary>
    /// <param name="name">The source series name.</param>
    /// <param name="axis">The source axis.</param>
    /// <param name="count">The number of points to publish.</param>
    /// <returns>The normalized reactive source.</returns>
    private static StaticSource CreateStaticSource(string name, int axis, int count)
    {
        var x = CreateSequentialValues(FirstXValue, count);
        var y = CreateSequentialValues(FirstValue, count);
        PlotSeriesKey key = new(name, axis);
        var updates = new Signal<ReactivePlotUpdate>();
        var update = new ReactivePlotUpdate(key, PlotType.Signal, ReactivePlotUpdateKind.Replace, x, y, PlotXAxisKind.Numeric, 0);
        var source = ReactivePlotSource.FromUpdates(key, PlotType.Signal, updates, PlotXAxisKind.Numeric);
        return new(source, updates, update);
    }

    /// <summary>Reads the first update published by each study.</summary>
    /// <param name="studies">The studies to inspect.</param>
    /// <param name="source">The source that publishes the shared replacement update.</param>
    /// <returns>The first update from each study.</returns>
    private static List<ReactivePlotUpdate> ReadFirstUpdates(IReadOnlyList<IReactivePlotSource> studies, StaticSource source)
    {
        List<ReactivePlotUpdate> updates = [];
        List<IDisposable> subscriptions = [];
        foreach (var study in studies)
        {
            subscriptions.Add(study.Updates.Subscribe(updates.Add));
        }

        source.Updates.OnNext(source.Update);
        foreach (var subscription in subscriptions)
        {
            subscription.Dispose();
        }

        return updates;
    }

    /// <summary>Creates one normalized source update with sequential values.</summary>
    /// <param name="kind">The update kind.</param>
    /// <param name="firstX">The first X value.</param>
    /// <param name="firstY">The first Y value.</param>
    /// <param name="axis">The source axis.</param>
    /// <param name="maxPoints">The optional retained-point limit.</param>
    /// <param name="sequence">The update sequence.</param>
    /// <returns>The normalized update.</returns>
    private static ReactivePlotUpdate CreateUpdate(
        ReactivePlotUpdateKind kind,
        long firstX,
        long firstY,
        int axis,
        int? maxPoints,
        long sequence)
    {
        PlotSeriesKey key = new(PriceSeriesName, axis);
        return new(
            key,
            PlotType.Signal,
            kind,
            [firstX, firstX + 1L, firstX + ThirdPointOffset],
            [firstY, firstY + 1L, firstY + ThirdPointOffset],
            PlotXAxisKind.Numeric,
            sequence,
            maxPoints);
    }

    /// <summary>Creates sequential values.</summary>
    /// <param name="start">The first value.</param>
    /// <param name="count">The number of values.</param>
    /// <returns>The generated values.</returns>
    private static double[] CreateSequentialValues(int start, int count)
    {
        var values = new double[count];
        for (var index = FirstXValue; index < count; index++)
        {
            values[index] = start + index;
        }

        return values;
    }

    /// <summary>Groups one manually-driven static source and its normalized replacement update.</summary>
    /// <param name="Source">The reactive source consumed by studies.</param>
    /// <param name="Updates">The signal used to publish the replacement.</param>
    /// <param name="Update">The replacement update to publish.</param>
    private sealed record StaticSource(IReactivePlotSource Source, Signal<ReactivePlotUpdate> Updates, ReactivePlotUpdate Update);
}
