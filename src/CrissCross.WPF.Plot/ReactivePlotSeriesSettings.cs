// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !REACTIVE_SHIM
using ReactiveUI;
#endif
#if REACTIVE_SHIM
using static ReactiveUI.Binding.Reactive.ReactiveUIBindingExtensions;
#else
using static ReactiveUI.Binding.ReactiveUIBindingExtensions;
#endif

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Stores editable presentation and retention settings for one attached reactive series.</summary>
public sealed class ReactivePlotSeriesSettings : RxObject
{
    /// <summary>Initializes a new instance of the <see cref="ReactivePlotSeriesSettings"/> class.</summary>
    /// <param name="key">The stable series identity.</param>
    /// <param name="plotType">The rendered chart type.</param>
    /// <param name="color">The initial series color.</param>
    public ReactivePlotSeriesSettings(PlotSeriesKey key, PlotType plotType, string color)
    {
        ThrowHelper.ThrowIfNull(color, nameof(color));
        Key = key;
        SeriesLabel = key.Name;
        PlotType = plotType;
        Color = color;
        LineMode = plotType == PlotType.Points ? PlotLineMode.MarkersOnly : PlotLineMode.LineOnly;
    }

    /// <summary>Gets or sets the series color.</summary>
    public string Color
    {
        get => field;
        set
        {
            ThrowHelper.ThrowIfNull(value, nameof(value));
            if (value.StartsWith('#'))
            {
                _ = ScottPlot.Color.FromHex(value);
            }
            else if (!System.Drawing.Color.FromName(value).IsKnownColor)
            {
                throw new ArgumentException("Enter a named color or hexadecimal color.", nameof(value));
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
        }
    } = string.Empty;

    /// <summary>Gets or sets the line width.</summary>
    public double LineWidth
    {
        get => field;
        set
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value < 0 || value > float.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Enter a finite non-negative size.");
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
        }
    } = 2;

    /// <summary>Gets or sets the marker size.</summary>
    public double MarkerSize
    {
        get => field;
        set
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value < 0 || value > float.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Enter a finite non-negative size.");
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
        }
    } = 5;

    /// <summary>Gets or sets the line and marker mode.</summary>
    public PlotLineMode LineMode
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the area baseline mode.</summary>
    public PlotBaselineMode BaselineMode
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = PlotBaselineMode.Zero;

    /// <summary>Gets or sets the custom area baseline.</summary>
    public double Baseline
    {
        get => field;
        set
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Enter a finite number.");
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    /// <summary>Gets or sets the legend inclusion.</summary>
    public bool ShowInLegend
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    /// <summary>Gets or sets the series visibility.</summary>
    public bool IsVisible
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    /// <summary>Gets or sets the pause state.</summary>
    public bool IsPaused
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the optional retained point limit.</summary>
    public int? MaxPoints
    {
        get => field;
        set
        {
            if (value is <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The point limit must be positive or empty.");
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    /// <summary>Gets the stable series identity.</summary>
    public PlotSeriesKey Key { get; }

    /// <summary>Gets the logical series name.</summary>
    public string SeriesName => Key.Name;

    /// <summary>Gets or sets the series name displayed in legends and selectors.</summary>
    public string SeriesLabel
    {
        get => field;
        set
        {
            ThrowHelper.ThrowIfNull(value, nameof(value));
            _ = this.RaiseAndSetIfChanged(ref field, value);
        }
    } = string.Empty;

    /// <summary>Gets or sets whether nearest-point cursor values are visible.</summary>
    public bool IsCrossHairVisible
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the rendered chart type.</summary>
    public PlotType PlotType { get; }

    /// <summary>Gets or sets how a fixed-buffer stream advances across the plot.</summary>
    public StreamerViewMode StreamerViewMode
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the X-axis interval between streamer samples.</summary>
    public double SamplePeriod
    {
        get => field;
        set
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Enter a finite positive sample interval.");
            }

            _ = this.RaiseAndSetIfChanged(ref field, value);
        }
    } = 1;

    /// <summary>Creates the effective immutable presentation style.</summary>
    /// <returns>The current series style.</returns>
    internal ReactivePlotSeriesStyle CreateStyle() => new()
    {
        Color = Color,
        LineWidth = (float)LineWidth,
        MarkerSize = (float)MarkerSize,
        LineMode = IsVisible ? LineMode : PlotLineMode.Hidden,
        BaselineMode = BaselineMode,
        Baseline = Baseline,
        ShowInLegend = ShowInLegend,
    };

    /// <summary>Initializes settings from a source's first update.</summary>
    /// <param name="update">The source update.</param>
    internal void Initialize(ReactivePlotUpdate update)
    {
        MaxPoints = update.MaxPoints;
        if (update.Style is not { } style)
        {
            return;
        }

        Color = style.Color ?? Color;
        LineWidth = style.LineWidth;
        MarkerSize = style.MarkerSize;
        LineMode = style.LineMode;
        BaselineMode = style.BaselineMode;
        Baseline = style.Baseline;
        ShowInLegend = style.ShowInLegend;
        IsVisible = style.LineMode != PlotLineMode.Hidden;
    }
}
