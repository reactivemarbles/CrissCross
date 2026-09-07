// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross;
using ReactiveUI;

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>Provides the reactive state for the curated WPF control catalog.</summary>
public class ControlCatalogViewModel : RxObject
{
    /// <summary>Defines the reactor temperature in the gallery example.</summary>
    private const double ReactorTemperature = 68.4D;

    /// <summary>Defines the temperature maximum in the gallery example.</summary>
    private const double TemperatureMaximum = 120D;

    /// <summary>Defines the temperature low alarm in the gallery example.</summary>
    private const double TemperatureLowAlarm = 15D;

    /// <summary>Defines the temperature high alarm in the gallery example.</summary>
    private const double TemperatureHighAlarm = 95D;

    /// <summary>Defines the line pressure in the gallery example.</summary>
    private const double LinePressure = 12.8D;

    /// <summary>Defines the pressure maximum in the gallery example.</summary>
    private const double PressureMaximum = 16D;

    /// <summary>Defines the pressure low alarm in the gallery example.</summary>
    private const double PressureLowAlarm = 2D;

    /// <summary>Defines the pressure high alarm in the gallery example.</summary>
    private const double PressureHighAlarm = 12D;

    /// <summary>Defines the flow rate in the gallery example.</summary>
    private const double FlowRate = 318D;

    /// <summary>Defines the flow maximum in the gallery example.</summary>
    private const double FlowMaximum = 500D;

    /// <summary>Defines the flow low alarm in the gallery example.</summary>
    private const double FlowLowAlarm = 50D;

    /// <summary>Defines the flow high alarm in the gallery example.</summary>
    private const double FlowHighAlarm = 450D;

    /// <summary>Initializes a new instance of the <see cref="ControlCatalogViewModel"/> class.</summary>
    public ControlCatalogViewModel()
    {
        RefreshCommand = ReactiveCommand.Create(Refresh);
        Refresh();
    }

    /// <summary>Gets the command that refreshes the deterministic catalog status.</summary>
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

    /// <summary>Gets a nominal process value example for industrial readout demos.</summary>
    public ProcessValueState NormalProcessValue { get; } = new(
        "Reactor temperature",
        ReactorTemperature,
        "deg C",
        0D,
        TemperatureMaximum,
        TemperatureLowAlarm,
        TemperatureHighAlarm);

    /// <summary>Gets a high-alarm process value example for industrial readout demos.</summary>
    public ProcessValueState AlarmProcessValue { get; } = new(
        "Line pressure",
        LinePressure,
        "bar",
        0D,
        PressureMaximum,
        PressureLowAlarm,
        PressureHighAlarm);

    /// <summary>Gets an unavailable process value example for industrial readout demos.</summary>
    public ProcessValueState BadQualityProcessValue { get; } = new(
        "Flow rate",
        FlowRate,
        "L/min",
        0D,
        FlowMaximum,
        new ProcessValueOptions { LowAlarmLimit = FlowLowAlarm, HighAlarmLimit = FlowHighAlarm, IsGoodQuality = false });

    /// <summary>Gets the status shown by the catalog's interactive command example.</summary>
    public string StatusText
    {
        get => field;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Refreshes the deterministic catalog status.</summary>
    private void Refresh() => StatusText = "Catalog controls are ready for interaction.";
}
