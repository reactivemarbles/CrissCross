// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ScottPlot;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Configures crosshair behavior and its time source.</summary>
public sealed class CrosshairUIOptions
{
    /// <summary>Initializes a new instance of the <see cref="CrosshairUIOptions"/> class.</summary>
    /// <param name="isXAxisDateTime">A value indicating whether the X-axis represents date/time values.</param>
    /// <param name="autoScale">A value indicating whether automatic scaling is enabled.</param>
    /// <param name="manualScale">A value indicating whether manual scaling is enabled.</param>
    /// <param name="coordinatesObservable">The optional source of pointer coordinates.</param>
    /// <param name="timeProvider">The time source used to initialize date/time crosshairs.</param>
    public CrosshairUIOptions(
        bool isXAxisDateTime,
        bool autoScale,
        bool manualScale,
        IObservable<Coordinates>? coordinatesObservable,
        TimeProvider timeProvider)
    {
        ThrowHelper.ThrowIfNull(timeProvider, nameof(timeProvider));

        IsXAxisDateTime = isXAxisDateTime;
        AutoScale = autoScale;
        ManualScale = manualScale;
        CoordinatesObservable = coordinatesObservable;
        TimeProvider = timeProvider;
    }

    /// <summary>Gets a value indicating whether the X-axis represents date/time values.</summary>
    public bool IsXAxisDateTime { get; }

    /// <summary>Gets a value indicating whether automatic scaling is enabled.</summary>
    public bool AutoScale { get; }

    /// <summary>Gets a value indicating whether manual scaling is enabled.</summary>
    public bool ManualScale { get; }

    /// <summary>Gets the optional source of pointer coordinates.</summary>
    public IObservable<Coordinates>? CoordinatesObservable { get; }

    /// <summary>Gets the time source used to initialize date/time crosshairs.</summary>
    public TimeProvider TimeProvider { get; }
}
