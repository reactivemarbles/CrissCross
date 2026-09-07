// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using ReactiveIndicator = CrissCross.Reactive.WPF.UI.Controls.ProcessValueIndicator;
using ReactiveOptions = CrissCross.Reactive.ProcessValueOptions;
using ReactiveState = CrissCross.Reactive.ProcessValueState;
using ReactiveStatus = CrissCross.Reactive.ProcessValueStatus;
using StandardIndicator = CrissCross.WPF.UI.Controls.ProcessValueIndicator;
using StandardOptions = CrissCross.ProcessValueOptions;
using StandardState = CrissCross.ProcessValueState;
using StandardStatus = CrissCross.ProcessValueStatus;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Exercises WPF process value indicator state projection.</summary>
public sealed class ProcessValueIndicatorTests
{
    /// <summary>The fallback label used when no process value state is supplied.</summary>
    private const string FallbackLabel = "Process value";

    /// <summary>The fallback display text used when no process value reading is available.</summary>
    private const string FallbackDisplayText = "—";

    /// <summary>The fallback status text used when no process value state is supplied.</summary>
    private const string FallbackStatusText = "Bad quality";

    /// <summary>The line pressure label used by projection tests.</summary>
    private const string LinePressureLabel = "Line pressure";

    /// <summary>The tank level label used by reactive projection tests.</summary>
    private const string TankLevelLabel = "Tank level";

    /// <summary>The flow rate label used by bad-quality projection tests.</summary>
    private const string FlowRateLabel = "Flow rate";

    /// <summary>The engineering unit used by pressure readings.</summary>
    private const string PressureUnit = "bar";

    /// <summary>The engineering unit used by percentage readings.</summary>
    private const string PercentageUnit = "%";

    /// <summary>The engineering unit used by flow readings.</summary>
    private const string FlowUnit = "L/min";

    /// <summary>The minimum range value used by the tests.</summary>
    private const double MinimumValue = 0D;

    /// <summary>The pressure reading used by standard projection tests.</summary>
    private const double PressureValue = 8D;

    /// <summary>The pressure maximum and high-alarm value.</summary>
    private const double PressureMaximum = 10D;

    /// <summary>The pressure low-alarm value.</summary>
    private const double PressureLowAlarm = 2D;

    /// <summary>The tank-level reading used by reactive projection tests.</summary>
    private const double TankLevelValue = 72D;

    /// <summary>The normalized percentage maximum.</summary>
    private const double PercentageMaximum = 100D;

    /// <summary>The tank-level low-alarm value.</summary>
    private const double TankLowAlarm = 10D;

    /// <summary>The tank-level high-alarm value.</summary>
    private const double TankHighAlarm = 90D;

    /// <summary>The flow reading used by bad-quality projection tests.</summary>
    private const double FlowValue = 318D;

    /// <summary>The flow display maximum.</summary>
    private const double FlowMaximum = 500D;

    /// <summary>The flow low-alarm value.</summary>
    private const double FlowLowAlarm = 50D;

    /// <summary>The flow high-alarm value.</summary>
    private const double FlowHighAlarm = 450D;

    /// <summary>Verifies each high-contrast palette distinguishes industrial status conditions.</summary>
    /// <param name="theme">The high-contrast resource dictionary name.</param>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    [Arguments("HCWhite")]
    [Arguments("HCBlack")]
    [Arguments("HC1")]
    [Arguments("HC2")]
    public async Task HighContrastPalettes_DistinguishProcessStates(string theme)
    {
        var colors = await RunOnStaThreadAsync(() => ReadStatusColors(theme));
        await Assert.That(colors.Success).IsNotEqualTo(colors.Caution);
        await Assert.That(colors.Success).IsNotEqualTo(colors.Critical);
        await Assert.That(colors.Caution).IsNotEqualTo(colors.Critical);
        await Assert.That(colors.Background).IsNotEqualTo(Colors.Red);
        await Assert.That(colors.Success).IsNotEqualTo(Colors.Red);
        await Assert.That(colors.Caution).IsNotEqualTo(Colors.Red);
        await Assert.That(colors.Critical).IsNotEqualTo(Colors.Red);
    }

    /// <summary>Verifies the standard WPF control projects null state as an empty fallback.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task StandardIndicator_WithNullState_ExposesEmptyFallback()
    {
        var snapshot = await RunOnStaThreadAsync(static () => Capture(new StandardIndicator()));

        await Assert.That(snapshot.Label).IsEqualTo(FallbackLabel);
        await Assert.That(snapshot.DisplayText).IsEqualTo(FallbackDisplayText);
        await Assert.That(snapshot.StatusText).IsEqualTo(FallbackStatusText);
        await Assert.That(snapshot.Percentage).IsEqualTo(MinimumValue);
        await Assert.That(snapshot.NormalizedValue).IsEqualTo(MinimumValue);
        await Assert.That(snapshot.HasValidValue).IsFalse();
        await Assert.That(snapshot.IsAlarm).IsFalse();
        await Assert.That(snapshot.Status).IsEqualTo((StandardStatus?)StandardStatus.BadQuality);
    }

    /// <summary>Verifies standard WPF state projection for templates and bindings.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task StandardIndicator_WithState_ProjectsTemplateBindableValues()
    {
        StandardState state = new(
            LinePressureLabel,
            PressureValue,
            PressureUnit,
            MinimumValue,
            PressureMaximum,
            PressureLowAlarm,
            PressureValue);
        var snapshot = await RunOnStaThreadAsync(() => Capture(new StandardIndicator { State = state }));

        await Assert.That(snapshot.Label).IsEqualTo(state.Label);
        await Assert.That(snapshot.DisplayText).IsEqualTo(state.DisplayText);
        await Assert.That(snapshot.StatusText).IsEqualTo(state.StatusText);
        await Assert.That(snapshot.Percentage).IsEqualTo(state.Percentage);
        await Assert.That(snapshot.NormalizedValue).IsEqualTo(state.NormalizedValue);
        await Assert.That(snapshot.HasValidValue).IsEqualTo(state.HasValidValue);
        await Assert.That(snapshot.IsAlarm).IsTrue();
        await Assert.That(snapshot.Status).IsEqualTo((StandardStatus?)StandardStatus.HighAlarm);
    }

    /// <summary>Verifies reactive WPF state projection from the reactive namespace variant.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveIndicator_WithState_ProjectsTemplateBindableValues()
    {
        ReactiveState state = new(
            TankLevelLabel,
            TankLevelValue,
            PercentageUnit,
            MinimumValue,
            PercentageMaximum,
            CreateBadQualityReactiveOptions());
        var snapshot = await RunOnStaThreadAsync(() => Capture(new ReactiveIndicator { State = state }));

        await Assert.That(snapshot.Label).IsEqualTo(state.Label);
        await Assert.That(snapshot.DisplayText).IsEqualTo(state.DisplayText);
        await Assert.That(snapshot.StatusText).IsEqualTo(state.StatusText);
        await Assert.That(snapshot.Percentage).IsEqualTo(state.Percentage);
        await Assert.That(snapshot.NormalizedValue).IsEqualTo(state.NormalizedValue);
        await Assert.That(snapshot.HasValidValue).IsFalse();
        await Assert.That(snapshot.IsAlarm).IsFalse();
        await Assert.That(snapshot.ReactiveStatus).IsEqualTo((ReactiveStatus?)ReactiveStatus.BadQuality);
    }

    /// <summary>Verifies the standard WPF control projects explicit bad-quality options.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task StandardIndicator_WithBadQualityOptions_ProjectsUnavailableStatus()
    {
        StandardState state = new(
            FlowRateLabel,
            FlowValue,
            FlowUnit,
            MinimumValue,
            FlowMaximum,
            CreateBadQualityStandardOptions());
        var snapshot = await RunOnStaThreadAsync(() => Capture(new StandardIndicator { State = state }));

        await Assert.That(snapshot.DisplayText).IsEqualTo(state.DisplayText);
        await Assert.That(snapshot.StatusText).IsEqualTo(FallbackStatusText);
        await Assert.That(snapshot.Percentage).IsEqualTo(MinimumValue);
        await Assert.That(snapshot.HasValidValue).IsFalse();
        await Assert.That(snapshot.Status).IsEqualTo((StandardStatus?)StandardStatus.BadQuality);
    }

    /// <summary>Loads actual high-contrast resources on their WPF owner thread.</summary>
    /// <param name="theme">The resource dictionary name.</param>
    /// <returns>The status and background palette colors.</returns>
    private static (Color Success, Color Caution, Color Critical, Color Background) ReadStatusColors(string theme)
    {
        var resources = new ResourceDictionary { Source = new($"pack://application:,,,/CrissCross.WPF.UI;component/Resources/Theme/{theme}.xaml") };
        return (
            ((SolidColorBrush)resources["SystemFillColorSuccessBrush"]).Color,
            ((SolidColorBrush)resources["SystemFillColorCautionBrush"]).Color,
            ((SolidColorBrush)resources["SystemFillColorCriticalBrush"]).Color,
            (Color)resources["CardBackgroundFillColorDefault"]);
    }

    /// <summary>Creates standard bad-quality process value options.</summary>
    /// <returns>The configured standard options.</returns>
    private static StandardOptions CreateBadQualityStandardOptions() => new() { LowAlarmLimit = FlowLowAlarm, HighAlarmLimit = FlowHighAlarm, IsGoodQuality = false, };

    /// <summary>Creates reactive bad-quality process value options.</summary>
    /// <returns>The configured reactive options.</returns>
    private static ReactiveOptions CreateBadQualityReactiveOptions() => new() { LowAlarmLimit = TankLowAlarm, HighAlarmLimit = TankHighAlarm, IsGoodQuality = false, };

    /// <summary>Captures standard indicator projection values.</summary>
    /// <param name="indicator">The indicator to inspect.</param>
    /// <returns>The captured projection values.</returns>
    private static IndicatorSnapshot Capture(StandardIndicator indicator) => new(
        indicator.Label,
        indicator.DisplayText,
        indicator.StatusText,
        indicator.Percentage,
        indicator.NormalizedValue,
        indicator.HasValidValue,
        indicator.IsAlarm,
        indicator.Status,
        null);

    /// <summary>Captures reactive indicator projection values.</summary>
    /// <param name="indicator">The indicator to inspect.</param>
    /// <returns>The captured projection values.</returns>
    private static IndicatorSnapshot Capture(ReactiveIndicator indicator) => new(
        indicator.Label,
        indicator.DisplayText,
        indicator.StatusText,
        indicator.Percentage,
        indicator.NormalizedValue,
        indicator.HasValidValue,
        indicator.IsAlarm,
        null,
        indicator.Status);

    /// <summary>Runs a factory on an STA thread for WPF dependency object access.</summary>
    /// <param name="action">The factory to run.</param>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <returns>A task that completes with the factory result.</returns>
    private static Task<TResult> RunOnStaThreadAsync<TResult>(Func<TResult> action)
    {
        var completion = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(
            () =>
            {
                try
                {
                    completion.SetResult(action());
                }
                catch (Exception exception)
                {
                    completion.SetException(exception);
                }
            });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return completion.Task;
    }

    /// <summary>Stores process value projection values captured from an indicator instance.</summary>
    /// <param name="Label">The projected label.</param>
    /// <param name="DisplayText">The projected display text.</param>
    /// <param name="StatusText">The projected status text.</param>
    /// <param name="Percentage">The projected percentage.</param>
    /// <param name="NormalizedValue">The projected normalized value.</param>
    /// <param name="HasValidValue">Whether the reading is valid.</param>
    /// <param name="IsAlarm">Whether the reading is in alarm.</param>
    /// <param name="Status">The standard status value, when captured from the standard control.</param>
    /// <param name="ReactiveStatus">The reactive status value, when captured from the reactive control.</param>
    private sealed record IndicatorSnapshot(
        string Label,
        string DisplayText,
        string StatusText,
        double Percentage,
        double NormalizedValue,
        bool HasValidValue,
        bool IsAlarm,
        StandardStatus? Status,
        ReactiveStatus? ReactiveStatus);
}
