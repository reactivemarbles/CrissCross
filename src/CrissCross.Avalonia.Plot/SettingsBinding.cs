// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.Plot;
#else
namespace CrissCross.Avalonia.Plot;
#endif

/// <summary>Creates settings bindings with explicit property accessors.</summary>
internal static class SettingsBinding
{
    /// <summary>Describes a property outside the settings editor contract.</summary>
    private const string UnsupportedPropertyMessage = "Unsupported settings property.";

    /// <summary>Creates an editable binding without runtime property discovery.</summary>
    /// <param name="source">The settings object.</param>
    /// <param name="property">The property to edit.</param>
    /// <returns>The compiled binding.</returns>
    internal static CompiledBindingExtension Create(object source, string property)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(property);
        return source switch
        {
            LiveChart chart => CreateChart(chart, property),
            ReactivePlotSeriesSettings series => CreateSeries(series, property),
            ChartObjects chartObject => CreateObject(chartObject, property),
            _ => throw new ArgumentException("Unsupported settings source.", nameof(source)),
        };
    }

    /// <summary>Creates a binding for the strongly typed settings source.</summary>
    /// <param name="source">The settings source.</param>
    /// <param name="property">The edited property.</param>
    /// <returns>The compiled binding.</returns>
    private static CompiledBindingExtension CreateChart(LiveChart source, string property) => property switch
    {
        nameof(LiveChart.TitleContent) => CreateProperty(source, property, static item => item.TitleContent, static (item, value) => item.TitleContent = value),
        nameof(LiveChart.LegendPosition) => CreateProperty(source, property, static item => item.LegendPosition, static (item, value) => item.LegendPosition = value),
        nameof(LiveChart.UseFixedNumberOfPoints) => CreateProperty(source, property, static item => item.UseFixedNumberOfPoints, static (item, value) => item.UseFixedNumberOfPoints = value),
        nameof(LiveChart.NumberPointsPlotted) => CreateProperty(source, property, static item => item.NumberPointsPlotted, static (item, value) => item.NumberPointsPlotted = value),
        _ => throw new ArgumentException(UnsupportedPropertyMessage, nameof(property)),
    };

    /// <summary>Creates a binding for the strongly typed settings source.</summary>
    /// <param name="source">The settings source.</param>
    /// <param name="property">The edited property.</param>
    /// <returns>The compiled binding.</returns>
    private static CompiledBindingExtension CreateSeries(ReactivePlotSeriesSettings source, string property) => property switch
    {
        nameof(ReactivePlotSeriesSettings.SeriesLabel) => CreateProperty(source, property, static item => item.SeriesLabel, static (item, value) => item.SeriesLabel = value),
        nameof(ReactivePlotSeriesSettings.IsVisible) => CreateProperty(source, property, static item => item.IsVisible, static (item, value) => item.IsVisible = value),
        nameof(ReactivePlotSeriesSettings.IsPaused) => CreateProperty(source, property, static item => item.IsPaused, static (item, value) => item.IsPaused = value),
        nameof(ReactivePlotSeriesSettings.IsCrossHairVisible) => CreateProperty(source, property, static item => item.IsCrossHairVisible, static (item, value) => item.IsCrossHairVisible = value),
        nameof(ReactivePlotSeriesSettings.ShowInLegend) => CreateProperty(source, property, static item => item.ShowInLegend, static (item, value) => item.ShowInLegend = value),
        nameof(ReactivePlotSeriesSettings.Color) => CreateProperty(source, property, static item => item.Color, static (item, value) => item.Color = value),
        nameof(ReactivePlotSeriesSettings.LineWidth) => CreateProperty(source, property, static item => item.LineWidth, static (item, value) => item.LineWidth = value),
        nameof(ReactivePlotSeriesSettings.StreamerViewMode) => CreateProperty(source, property, static item => item.StreamerViewMode, static (item, value) => item.StreamerViewMode = value),
        nameof(ReactivePlotSeriesSettings.SamplePeriod) => CreateProperty(source, property, static item => item.SamplePeriod, static (item, value) => item.SamplePeriod = value),
        nameof(ReactivePlotSeriesSettings.MarkerSize) => CreateProperty(source, property, static item => item.MarkerSize, static (item, value) => item.MarkerSize = value),
        nameof(ReactivePlotSeriesSettings.LineMode) => CreateProperty(source, property, static item => item.LineMode, static (item, value) => item.LineMode = value),
        nameof(ReactivePlotSeriesSettings.BaselineMode) => CreateProperty(source, property, static item => item.BaselineMode, static (item, value) => item.BaselineMode = value),
        nameof(ReactivePlotSeriesSettings.Baseline) => CreateProperty(source, property, static item => item.Baseline, static (item, value) => item.Baseline = value),
        nameof(ReactivePlotSeriesSettings.MaxPoints) => CreateProperty(source, property, static item => item.MaxPoints, static (item, value) => item.MaxPoints = value),
        _ => throw new ArgumentException(UnsupportedPropertyMessage, nameof(property)),
    };

    /// <summary>Creates a binding for the strongly typed settings source.</summary>
    /// <param name="source">The settings source.</param>
    /// <param name="property">The edited property.</param>
    /// <returns>The compiled binding.</returns>
    private static CompiledBindingExtension CreateObject(ChartObjects source, string property) => property switch
    {
        nameof(ChartObjects.IsChecked) => CreateProperty(source, property, static item => item.IsChecked, static (item, value) => item.IsChecked = value),
        nameof(ChartObjects.IsPaused) => CreateProperty(source, property, static item => item.IsPaused, static (item, value) => item.IsPaused = value),
        nameof(ChartObjects.IsCrossHairVisible) => CreateProperty(source, property, static item => item.IsCrossHairVisible, static (item, value) => item.IsCrossHairVisible = value),
        nameof(ChartObjects.ItemName) => CreateProperty(source, property, static item => item.ItemName, static (item, value) => item.ItemName = value),
        nameof(ChartObjects.Color) => CreateProperty(source, property, static item => item.Color, static (item, value) => item.Color = value),
        nameof(ChartObjects.LineWidth) => CreateProperty(source, property, static item => item.LineWidth, static (item, value) => item.LineWidth = value),
        _ => throw new ArgumentException(UnsupportedPropertyMessage, nameof(property)),
    };

    /// <summary>Builds a compiled path from direct getter and setter delegates.</summary>
    /// <typeparam name="TSource">The settings source type.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="source">The settings source.</param>
    /// <param name="property">The property name.</param>
    /// <param name="getter">The direct property getter.</param>
    /// <param name="setter">The direct property setter.</param>
    /// <returns>The compiled binding.</returns>
    private static CompiledBindingExtension CreateProperty<TSource, TValue>(
        TSource source,
        string property,
        Func<TSource, TValue> getter,
        Action<TSource, TValue> setter)
        where TSource : class
    {
        var info = new ClrPropertyInfo(
            property,
            instance => getter((TSource)instance),
            (instance, value) => setter((TSource)instance, (TValue)value!),
            typeof(TValue));
        var path = new CompiledBindingPathBuilder()
            .Property(info, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor)
            .Build();
        return new(path) { Source = source, Mode = BindingMode.TwoWay };
    }
}
