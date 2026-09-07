// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using ProcessValueOptions = CrissCross.Reactive.ProcessValueOptions;
using ProcessValueState = CrissCross.Reactive.ProcessValueState;
#else
using ProcessValueOptions = CrissCross.ProcessValueOptions;
using ProcessValueState = CrissCross.ProcessValueState;
#endif

namespace CrissCross.Avalonia.UI.Gallery.ViewModels;

/// <summary>View model for industrial process controls.</summary>
public sealed class IndustrialPageViewModel : RxObject
{
    /// <summary>The sample pressure measurement in bar.</summary>
    private const double PressureReading = 72.4;

    /// <summary>The pressure display maximum in bar.</summary>
    private const double PressureMaximum = 100;

    /// <summary>The high pressure alarm in bar.</summary>
    private const double PressureAlarm = 85;

    /// <summary>The sample temperature in degrees Celsius.</summary>
    private const double TemperatureReading = 96.8;

    /// <summary>The temperature display maximum in degrees Celsius.</summary>
    private const double TemperatureMaximum = 120;

    /// <summary>The high temperature alarm in degrees Celsius.</summary>
    private const double TemperatureAlarm = 90;

    /// <summary>The sample vibration measurement in millimeters per second.</summary>
    private const double VibrationReading = 8.2;

    /// <summary>The vibration display maximum in millimeters per second.</summary>
    private const double VibrationMaximum = 20;

    /// <summary>The high vibration alarm in millimeters per second.</summary>
    private const double VibrationAlarm = 12;

    /// <summary>Initializes a new instance of the <see cref="IndustrialPageViewModel"/> class.</summary>
    public IndustrialPageViewModel() => this.BuildComplete(() => DisplayName = "Industrial Controls");

    /// <summary>Gets a normal process reading.</summary>
    public ProcessValueState NormalPressure { get; } = new(
        "Pump discharge pressure",
        PressureReading,
        "bar",
        0D,
        PressureMaximum,
        new ProcessValueOptions { HighAlarmLimit = PressureAlarm });

    /// <summary>Gets a high-alarm process reading.</summary>
    public ProcessValueState HighTemperature { get; } = new(
        "Seal gas temperature",
        TemperatureReading,
        "degC",
        0D,
        TemperatureMaximum,
        new ProcessValueOptions { HighAlarmLimit = TemperatureAlarm });

    /// <summary>Gets a bad-quality process reading.</summary>
    public ProcessValueState BadQualityVibration { get; } = new(
        "Compressor vibration",
        VibrationReading,
        "mm/s",
        0D,
        VibrationMaximum,
        new ProcessValueOptions { HighAlarmLimit = VibrationAlarm, IsGoodQuality = false });
}
