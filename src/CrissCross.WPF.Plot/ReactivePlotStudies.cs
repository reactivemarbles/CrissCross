// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Composes technical-study sources over static, historic, or live reactive plot updates.</summary>
public static class ReactivePlotStudies
{
    /// <summary>Provides the conventional RSI period.</summary>
    private const int DefaultRelativeStrengthIndexPeriod = 14;

    /// <summary>Provides the conventional fast MACD period.</summary>
    private const int DefaultMacdFastPeriod = 12;

    /// <summary>Provides the conventional slow MACD period.</summary>
    private const int DefaultMacdSlowPeriod = 26;

    /// <summary>Provides the conventional MACD signal period.</summary>
    private const int DefaultMacdSignalPeriod = 9;

    /// <summary>Provides the conventional Bollinger Band period.</summary>
    private const int DefaultBollingerBandPeriod = 20;

    /// <summary>Provides the conventional Bollinger Band deviation multiplier.</summary>
    private const double DefaultBollingerBandDeviations = 2;

    /// <summary>Creates a reactive simple-moving-average overlay.</summary>
    /// <param name="source">The source to study.</param>
    /// <param name="period">The rolling period.</param>
    /// <returns>The derived plot source.</returns>
    public static IReactivePlotSource SimpleMovingAverage(IReactivePlotSource source, int period) =>
        SimpleMovingAverage(source, period, null, null);

    /// <summary>Creates a reactive simple-moving-average overlay.</summary>
    /// <param name="source">The source to study.</param>
    /// <param name="period">The rolling period.</param>
    /// <param name="axis">An optional output Y-axis index.</param>
    /// <param name="style">Optional overlay styling.</param>
    /// <returns>The derived plot source.</returns>
    public static IReactivePlotSource SimpleMovingAverage(
        IReactivePlotSource source,
        int period,
        int? axis,
        ReactivePlotSeriesStyle? style) =>
        CreateStudy(
            source,
            $"SMA({period})",
            data => TechnicalIndicators.SimpleMovingAverage(data, period),
            axis,
            PlotType.Line,
            style);

    /// <summary>Creates a reactive exponential-moving-average overlay.</summary>
    /// <param name="source">The source to study.</param>
    /// <param name="period">The smoothing period.</param>
    /// <returns>The derived plot source.</returns>
    public static IReactivePlotSource ExponentialMovingAverage(IReactivePlotSource source, int period) =>
        ExponentialMovingAverage(source, period, null, null);

    /// <summary>Creates a reactive exponential-moving-average overlay.</summary>
    /// <param name="source">The source to study.</param>
    /// <param name="period">The smoothing period.</param>
    /// <param name="axis">An optional output Y-axis index.</param>
    /// <param name="style">Optional overlay styling.</param>
    /// <returns>The derived plot source.</returns>
    public static IReactivePlotSource ExponentialMovingAverage(
        IReactivePlotSource source,
        int period,
        int? axis,
        ReactivePlotSeriesStyle? style) =>
        CreateStudy(
            source,
            $"EMA({period})",
            data => TechnicalIndicators.ExponentialMovingAverage(data, period),
            axis,
            PlotType.Line,
            style);

    /// <summary>Creates a reactive RSI study using its conventional period.</summary>
    /// <param name="source">The source to study.</param>
    /// <returns>The derived plot source.</returns>
    public static IReactivePlotSource RelativeStrengthIndex(IReactivePlotSource source) =>
        RelativeStrengthIndex(source, DefaultRelativeStrengthIndexPeriod, null, null);

    /// <summary>Creates a reactive RSI study with a selected period.</summary>
    /// <param name="source">The source to study.</param>
    /// <param name="period">The RSI period.</param>
    /// <returns>The derived plot source.</returns>
    public static IReactivePlotSource RelativeStrengthIndex(IReactivePlotSource source, int period) =>
        RelativeStrengthIndex(source, period, null, null);

    /// <summary>Creates a reactive Relative Strength Index study.</summary>
    /// <param name="source">The source to study.</param>
    /// <param name="period">The RSI period.</param>
    /// <param name="axis">An optional output Y-axis index.</param>
    /// <param name="style">Optional study styling.</param>
    /// <returns>The derived plot source.</returns>
    public static IReactivePlotSource RelativeStrengthIndex(
        IReactivePlotSource source,
        int period,
        int? axis,
        ReactivePlotSeriesStyle? style) =>
        CreateStudy(
            source,
            $"RSI({period})",
            data => TechnicalIndicators.RelativeStrengthIndex(data, period),
            axis,
            PlotType.Line,
            style);

    /// <summary>Creates reactive MACD studies using conventional periods.</summary>
    /// <param name="source">The source to study.</param>
    /// <returns>The MACD line, signal line, and histogram sources.</returns>
    public static IReadOnlyList<IReactivePlotSource> MovingAverageConvergenceDivergence(IReactivePlotSource source) =>
        MovingAverageConvergenceDivergence(
            source,
            DefaultMacdFastPeriod,
            DefaultMacdSlowPeriod,
            DefaultMacdSignalPeriod,
            null);

    /// <summary>Creates reactive MACD studies on a selected output axis.</summary>
    /// <param name="source">The source to study.</param>
    /// <param name="axis">The output Y-axis index.</param>
    /// <returns>The MACD line, signal line, and histogram sources.</returns>
    public static IReadOnlyList<IReactivePlotSource> MovingAverageConvergenceDivergence(
        IReactivePlotSource source,
        int? axis) =>
        MovingAverageConvergenceDivergence(
            source,
            DefaultMacdFastPeriod,
            DefaultMacdSlowPeriod,
            DefaultMacdSignalPeriod,
            axis);

    /// <summary>Creates reactive MACD, signal, and histogram studies.</summary>
    /// <param name="source">The source to study.</param>
    /// <param name="fastPeriod">The fast EMA period.</param>
    /// <param name="slowPeriod">The slow EMA period.</param>
    /// <param name="signalPeriod">The signal EMA period.</param>
    /// <param name="axis">An optional output Y-axis index.</param>
    /// <returns>The MACD line, signal line, and histogram sources.</returns>
    public static IReadOnlyList<IReactivePlotSource> MovingAverageConvergenceDivergence(
        IReactivePlotSource source,
        int fastPeriod,
        int slowPeriod,
        int signalPeriod,
        int? axis)
    {
        var shared = Share(source);
        return
        [
            CreateStudy(
                shared,
                "MACD",
                data =>
                    TechnicalIndicators
                        .MovingAverageConvergenceDivergence(data, fastPeriod, slowPeriod, signalPeriod)
                        .Macd,
                axis,
                PlotType.Line,
                new() { Color = "#7E57C2" }),
            CreateStudy(
                shared,
                "MACD Signal",
                data =>
                    TechnicalIndicators
                        .MovingAverageConvergenceDivergence(data, fastPeriod, slowPeriod, signalPeriod)
                        .Signal,
                axis,
                PlotType.Line,
                new() { Color = "#FFA726" }),
            CreateStudy(
                shared,
                "MACD Histogram",
                data =>
                    TechnicalIndicators
                        .MovingAverageConvergenceDivergence(data, fastPeriod, slowPeriod, signalPeriod)
                        .Histogram,
                axis,
                PlotType.Bar,
                new() { Color = "#66BB6A" }),];
    }

    /// <summary>Creates reactive Bollinger Bands using conventional settings.</summary>
    /// <param name="source">The source to study.</param>
    /// <returns>The three band sources.</returns>
    public static IReadOnlyList<IReactivePlotSource> BollingerBands(IReactivePlotSource source) =>
        BollingerBands(source, DefaultBollingerBandPeriod, DefaultBollingerBandDeviations, null);

    /// <summary>Creates reactive Bollinger Bands on a selected output axis.</summary>
    /// <param name="source">The source to study.</param>
    /// <param name="axis">The output Y-axis index.</param>
    /// <returns>The three band sources.</returns>
    public static IReadOnlyList<IReactivePlotSource> BollingerBands(IReactivePlotSource source, int? axis) =>
        BollingerBands(source, DefaultBollingerBandPeriod, DefaultBollingerBandDeviations, axis);

    /// <summary>Creates reactive middle, upper, and lower Bollinger Band studies.</summary>
    /// <param name="source">The source to study.</param>
    /// <param name="period">The rolling period.</param>
    /// <param name="standardDeviations">The standard-deviation multiplier.</param>
    /// <param name="axis">An optional output Y-axis index.</param>
    /// <returns>The three band sources.</returns>
    public static IReadOnlyList<IReactivePlotSource> BollingerBands(
        IReactivePlotSource source,
        int period,
        double standardDeviations,
        int? axis)
    {
        var shared = Share(source);
        return
        [
            CreateStudy(
                shared,
                "Bollinger Middle",
                data => TechnicalIndicators.BollingerBands(data, period, standardDeviations).Middle,
                axis,
                PlotType.Line,
                new() { Color = "#42A5F5" }),
            CreateStudy(
                shared,
                "Bollinger Upper",
                data => TechnicalIndicators.BollingerBands(data, period, standardDeviations).Upper,
                axis,
                PlotType.Line,
                new() { Color = "#90CAF9" }),
            CreateStudy(
                shared,
                "Bollinger Lower",
                data => TechnicalIndicators.BollingerBands(data, period, standardDeviations).Lower,
                axis,
                PlotType.Line,
                new() { Color = "#90CAF9" }),];
    }

    /// <summary>Creates the five reactive Ichimoku study sources.</summary>
    /// <param name="source">The source to study.</param>
    /// <returns>The conversion, base, leading-span, and lagging sources.</returns>
    public static IReadOnlyList<IReactivePlotSource> Ichimoku(IReactivePlotSource source) => Ichimoku(source, null);

    /// <summary>Creates the five reactive Ichimoku study sources.</summary>
    /// <param name="source">The source to study.</param>
    /// <param name="axis">An optional output Y-axis index.</param>
    /// <returns>The conversion, base, leading-span, and lagging sources.</returns>
    public static IReadOnlyList<IReactivePlotSource> Ichimoku(IReactivePlotSource source, int? axis)
    {
        var shared = Share(source);
        return
        [
            CreateStudy(
                shared,
                "Ichimoku Conversion",
                data => TechnicalIndicators.Ichimoku(data).Conversion,
                axis,
                PlotType.Line,
                new() { Color = "#EF5350" }),
            CreateStudy(
                shared,
                "Ichimoku Base",
                data => TechnicalIndicators.Ichimoku(data).Base,
                axis,
                PlotType.Line,
                new() { Color = "#42A5F5" }),
            CreateStudy(
                shared,
                "Ichimoku Span A",
                data => TechnicalIndicators.Ichimoku(data).LeadingSpanA,
                axis,
                PlotType.Line,
                new() { Color = "#66BB6A" }),
            CreateStudy(
                shared,
                "Ichimoku Span B",
                data => TechnicalIndicators.Ichimoku(data).LeadingSpanB,
                axis,
                PlotType.Line,
                new() { Color = "#EF5350" }),
            CreateStudy(
                shared,
                "Ichimoku Lagging",
                data => TechnicalIndicators.Ichimoku(data).Lagging,
                axis,
                PlotType.Line,
                new() { Color = "#AB47BC" }),];
    }

    /// <summary>Creates one stateful derived source.</summary>
    /// <param name="source">The source to study.</param>
    /// <param name="suffix">The output series suffix.</param>
    /// <param name="calculate">The pure study calculation.</param>
    /// <param name="axis">The optional output axis.</param>
    /// <param name="plotType">The output plot type.</param>
    /// <param name="style">Optional output styling.</param>
    /// <returns>The derived source.</returns>
    private static ReactivePlotSource CreateStudy(
        IReactivePlotSource source,
        string suffix,
        Func<PlotSeriesData, PlotSeriesData> calculate,
        int? axis,
        PlotType plotType,
        ReactivePlotSeriesStyle? style)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var definition = new StudyDefinition(suffix, calculate, axis, plotType, style);
        var declaredKey = CreateKey(source.Key, definition.Suffix, definition.Axis);
        var updates = Observable.Defer(() =>
        {
            var x = new List<double>();
            var y = new List<double>();
            return source.Updates.Select(update => Transform(update, x, y, definition));
        });

        return new ReactivePlotSource(declaredKey, plotType, updates) { XAxisKind = source.XAxisKind };
    }

    /// <summary>Transforms one source update into a derived replacement update.</summary>
    /// <param name="update">The source update.</param>
    /// <param name="x">Retained X values.</param>
    /// <param name="y">Retained Y values.</param>
    /// <param name="definition">The immutable study definition.</param>
    /// <returns>The derived update.</returns>
    private static ReactivePlotUpdate Transform(
        ReactivePlotUpdate update,
        List<double> x,
        List<double> y,
        StudyDefinition definition)
    {
        var key = CreateKey(update.Key, definition.Suffix, definition.Axis);
        if (update.Kind == ReactivePlotUpdateKind.Clear)
        {
            x.Clear();
            y.Clear();
            return new(
                key,
                definition.PlotType,
                ReactivePlotUpdateKind.Clear,
                [],
                [],
                update.XAxisKind,
                update.Sequence,
                Style: definition.Style);
        }

        if (update.Kind == ReactivePlotUpdateKind.Replace)
        {
            x.Clear();
            y.Clear();
        }

        x.AddRange(update.X);
        y.AddRange(update.Y);
        TrimToLimit(x, y, update.MaxPoints);
        var input = new PlotSeriesData(update.Key, x.ToArray(), y.ToArray(), update.XAxisKind);
        var calculated = definition.Calculate(input);
        var result = calculated.Derive(key, calculated.Y);
        return result.ToUpdate(definition.PlotType, update.Sequence, definition.Style);
    }

    /// <summary>Creates an output series key.</summary>
    /// <param name="sourceKey">The input key.</param>
    /// <param name="suffix">The output suffix.</param>
    /// <param name="axis">The optional output axis.</param>
    /// <returns>The output key.</returns>
    private static PlotSeriesKey CreateKey(PlotSeriesKey sourceKey, string suffix, int? axis) =>
        new(string.IsNullOrWhiteSpace(sourceKey.Name) ? suffix : $"{sourceKey.Name} {suffix}", axis ?? sourceKey.Axis);

    /// <summary>Shares a source update subscription between a related group of studies.</summary>
    /// <param name="source">The source to share.</param>
    /// <returns>The shared source.</returns>
    private static ReactivePlotSource Share(IReactivePlotSource source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return new ReactivePlotSource(source.Key, source.PlotType, source.Updates.Publish().RefCount())
        {
            XAxisKind = source.XAxisKind,
        };
    }

    /// <summary>Applies a rolling point limit to retained study data.</summary>
    /// <param name="x">Retained X values.</param>
    /// <param name="y">Retained Y values.</param>
    /// <param name="maxPoints">The optional point limit.</param>
    private static void TrimToLimit(List<double> x, List<double> y, int? maxPoints)
    {
        if (maxPoints is not { } limit || limit <= 0 || x.Count <= limit)
        {
            return;
        }

        var removeCount = x.Count - limit;
        x.RemoveRange(0, removeCount);
        y.RemoveRange(0, removeCount);
    }

    /// <summary>Groups the immutable definition used by a derived study.</summary>
    /// <param name="Suffix">The output suffix.</param>
    /// <param name="Calculate">The pure study calculation.</param>
    /// <param name="Axis">The optional output axis.</param>
    /// <param name="PlotType">The output plot type.</param>
    /// <param name="Style">Optional output styling.</param>
    private sealed record StudyDefinition(
        string Suffix,
        Func<PlotSeriesData, PlotSeriesData> Calculate,
        int? Axis,
        PlotType PlotType,
        ReactivePlotSeriesStyle? Style);
}
