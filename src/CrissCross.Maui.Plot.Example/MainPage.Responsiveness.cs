// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Globalization;
using Microsoft.Maui.Dispatching;
using ScottPlot.Plottables;

namespace CrissCross.Maui.Plot.Example;

/// <summary>Provides an opt-in native dispatcher and live-data responsiveness soak.</summary>
public sealed partial class MainPage
{
    /// <summary>The total duration of scenario cycling and steady live rendering.</summary>
    private const int ResponsivenessDurationSeconds = 120;

    /// <summary>The duration of the scenario cycling phase.</summary>
    private const int ResponsivenessCyclingSeconds = 60;

    /// <summary>The maximum permitted dispatcher sentinel latency.</summary>
    private const int ResponsivenessDeadlineSeconds = 5;

    /// <summary>The interval between dispatcher sentinels.</summary>
    private const int ResponsivenessIntervalMilliseconds = 250;

    /// <summary>The number of sentinels between transitions and progress records.</summary>
    private const uint ResponsivenessTransitionInterval = 8;

    /// <summary>The minimum verified scenario transitions.</summary>
    private const uint ResponsivenessMinimumTransitions = 12;

    /// <summary>The minimum observed live sample advances during each phase.</summary>
    private const uint ResponsivenessMinimumAdvances = 20;

    /// <summary>The number of distinct scenario commands.</summary>
    private const uint ResponsivenessScenarioCount = 4;

    /// <summary>The historic scenario command index.</summary>
    private const uint ResponsivenessHistoricScenario = 2;

    /// <summary>The line width alternation period.</summary>
    private const uint ResponsivenessStylePeriod = 2;

    /// <summary>The minimum coordinate difference indicating live progression.</summary>
    private const double ResponsivenessCoordinateTolerance = 0.000001;

    /// <summary>Cancels verification when the native window closes.</summary>
    private readonly CancellationTokenSource _verificationLifetime = new();

    /// <summary>Prevents automatic verification from restarting on repeated Loaded events.</summary>
    private bool _automaticVerificationStarted;

    /// <summary>Runs scenario cycling and steady rendering monitored outside the UI synchronization context.</summary>
    /// <param name="outputDirectory">The directory receiving progress and final results.</param>
    /// <returns>The background watchdog task.</returns>
    public Task VerifyResponsivenessAsync(string outputDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);
        var dispatcher = Dispatcher;
        var cancellationToken = _verificationLifetime.Token;
        return Task.Run(() => RunResponsivenessWatchdogAsync(outputDirectory, dispatcher, cancellationToken), cancellationToken);
    }

    /// <summary>Counts native sample changes independently for each phase.</summary>
    /// <param name="state">The watchdog counters.</param>
    /// <param name="snapshot">The native snapshot.</param>
    /// <param name="transition">Whether a scenario changed.</param>
    /// <param name="steady">Whether the steady live phase has started.</param>
    private static void UpdateResponsivenessActivity(ResponsivenessState state, ResponsivenessSnapshot snapshot, bool transition, bool steady)
    {
        if (transition)
        {
            state.PreviousLiveX = snapshot.LiveX;
            state.Transitions++;
        }

        if (snapshot.LiveX is { } x && state.PreviousLiveX is { } previousX && Math.Abs(x - previousX) > ResponsivenessCoordinateTolerance)
        {
            state.PreviousLiveX = x;
            if (steady)
            {
                state.SteadyLiveAdvances++;
            }
            else
            {
                state.LiveAdvances++;
            }
        }

        state.PreviousLiveX = snapshot.LiveX;
        state.Probes++;
        state.SteadyStarted = steady;
        state.LastTransition = transition;
        if (!snapshot.HasNativeRender)
        {
            return;
        }

        state.NativeRenders++;
    }

    /// <summary>Formats background progress without accessing native controls.</summary>
    /// <param name="state">The watchdog counters.</param>
    /// <returns>The invariant progress report.</returns>
    private static string FormatResponsivenessProgress(ResponsivenessState state) =>
        $"elapsed={Stopwatch.GetElapsedTime(state.Started).TotalSeconds.ToString("F2", CultureInfo.InvariantCulture)}s; sentinels={state.Probes}; "
        + $"transitions={state.Transitions}; live-advances={state.LiveAdvances}; steady-live-advances={state.SteadyLiveAdvances}; native-renders={state.NativeRenders}; "
        + $"max-latency={state.MaximumLatency.TotalMilliseconds.ToString("F2", CultureInfo.InvariantCulture)}ms";

    /// <summary>Validates the native data rectangle and axes and formats their diagnostic values.</summary>
    /// <param name="plot">The actual native plot model.</param>
    /// <returns>The diagnostics and whether a nonzero native render has occurred.</returns>
    private static (string Render, bool HasRender) GetResponsivenessRender(ScottPlot.Plot plot)
    {
        var limits = plot.Axes.GetLimits();
        var rect = plot.LastRender.DataRect;
        var nativeRender = FormattableString.Invariant($"data-rect={rect.Width:F2}x{rect.Height:F2}; x=[{limits.Left:R},{limits.Right:R}]; y=[{limits.Bottom:R},{limits.Top:R}]");
        if (!double.IsFinite(rect.Width) || !double.IsFinite(rect.Height) || !double.IsFinite(limits.Left) || !double.IsFinite(limits.Right)
            || !double.IsFinite(limits.Bottom) || !double.IsFinite(limits.Top) || limits.Left >= limits.Right || limits.Bottom >= limits.Top)
        {
            throw new InvalidOperationException($"Native render geometry or axis limits are invalid: {nativeRender}");
        }

        return (nativeRender, rect.Width > 0 && rect.Height > 0);
    }

    /// <summary>Monitors and writes results independently of a potentially blocked native dispatcher.</summary>
    /// <param name="outputDirectory">The report directory.</param>
    /// <param name="dispatcher">The native dispatcher captured on the UI thread.</param>
    /// <param name="cancellationToken">The window lifetime.</param>
    /// <returns>The asynchronous watchdog.</returns>
    private async Task RunResponsivenessWatchdogAsync(string outputDirectory, IDispatcher dispatcher, CancellationToken cancellationToken)
    {
        _ = Directory.CreateDirectory(outputDirectory);
        var resultPath = Path.Combine(outputDirectory, "responsiveness.txt");
        var progressPath = Path.Combine(outputDirectory, "responsiveness-progress.txt");
        await File.WriteAllTextAsync(resultPath, "RUNNING: 120-second native responsiveness soak.\n", cancellationToken).ConfigureAwait(false);
        await File.WriteAllTextAsync(progressPath, "Native deadline=5s; interval=250ms; cycling=60s; steady-live=60s.\n", cancellationToken).ConfigureAwait(false);
        var state = new ResponsivenessState();
        try
        {
            while (Stopwatch.GetElapsedTime(state.Started) < TimeSpan.FromSeconds(ResponsivenessDurationSeconds))
            {
                var snapshot = await ProbeResponsivenessAsync(dispatcher, state, cancellationToken).ConfigureAwait(false);
                if (state.LastTransition || state.Probes % ResponsivenessTransitionInterval == 0)
                {
                    var progress = $"phase={(state.SteadyStarted ? "steady-live" : "cycling")}; {FormatResponsivenessProgress(state)}; {snapshot.NativeRender}\n";
                    await File.AppendAllTextAsync(progressPath, progress, cancellationToken).ConfigureAwait(false);
                }

                await Task.Delay(TimeSpan.FromMilliseconds(ResponsivenessIntervalMilliseconds), cancellationToken).ConfigureAwait(false);
            }

            if (state.Transitions < ResponsivenessMinimumTransitions || state.LiveAdvances < ResponsivenessMinimumAdvances
                || state.SteadyLiveAdvances < ResponsivenessMinimumAdvances || state.NativeRenders == 0)
            {
                throw new InvalidOperationException("Insufficient native rendering, transitions or live sample advances during both phases.");
            }

            await File.WriteAllTextAsync(resultPath, $"PASS: {FormatResponsivenessProgress(state)}; deadline=5000ms.\n", cancellationToken).ConfigureAwait(false);
        }
        catch (Exception error)
        {
            // Write from the watchdog even when the UI thread cannot resume its awaiting continuation.
            await File.WriteAllTextAsync(resultPath, $"FAIL: {FormatResponsivenessProgress(state)}\n{error}\n", CancellationToken.None).ConfigureAwait(false);
            throw;
        }
    }

    /// <summary>Posts one bounded native sentinel and updates background-only activity counters.</summary>
    /// <param name="dispatcher">The native dispatcher.</param>
    /// <param name="state">The watchdog counters.</param>
    /// <param name="cancellationToken">The window lifetime.</param>
    /// <returns>The dispatcher snapshot.</returns>
    private async Task<ResponsivenessSnapshot> ProbeResponsivenessAsync(IDispatcher dispatcher, ResponsivenessState state, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var steady = Stopwatch.GetElapsedTime(state.Started) >= TimeSpan.FromSeconds(ResponsivenessCyclingSeconds);
        var transition = steady ? !state.SteadyStarted : state.Probes % ResponsivenessTransitionInterval == 0;
        var currentTransition = steady ? 0 : state.Transitions;
        var completion = new TaskCompletionSource<ResponsivenessSnapshot>(TaskCreationOptions.RunContinuationsAsynchronously);
        var started = Stopwatch.GetTimestamp();
        if (!dispatcher.Dispatch(() => CompleteResponsivenessProbe(completion, transition, currentTransition, cancellationToken)))
        {
            throw new InvalidOperationException("The native dispatcher rejected the responsiveness sentinel.");
        }

        var snapshot = await completion.Task.WaitAsync(TimeSpan.FromSeconds(ResponsivenessDeadlineSeconds), cancellationToken).ConfigureAwait(false);
        var latency = Stopwatch.GetElapsedTime(started);
        if (latency > state.MaximumLatency)
        {
            state.MaximumLatency = latency;
        }

        UpdateResponsivenessActivity(state, snapshot, transition, steady);
        return snapshot;
    }

    /// <summary>Completes a sentinel only after native UI work and snapshot capture finish.</summary>
    /// <param name="completion">The independent watchdog completion.</param>
    /// <param name="transition">Whether to switch scenarios.</param>
    /// <param name="currentTransition">The scenario index.</param>
    /// <param name="cancellationToken">The window lifetime.</param>
    private void CompleteResponsivenessProbe(
        TaskCompletionSource<ResponsivenessSnapshot> completion,
        bool transition,
        uint currentTransition,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (transition)
            {
                ApplyResponsivenessTransition(currentTransition);
            }

            completion.SetResult(CaptureResponsivenessSnapshot());
        }
        catch (Exception error)
        {
            completion.SetException(error);
        }
    }

    /// <summary>Reads and validates actual native geometry only on the dispatcher.</summary>
    /// <returns>The current native snapshot.</returns>
    private ResponsivenessSnapshot CaptureResponsivenessSnapshot()
    {
        var plot = _chart.ViewModel.MauiPlot1vm?.Plot ?? throw new InvalidOperationException("The native plot host is unavailable.");
        double? liveX = null;
        foreach (var plottable in plot.GetPlottables())
        {
            if (plottable is DataLogger logger && logger.Data.Coordinates.Count > 0)
            {
                liveX = logger.Data.Coordinates[logger.Data.Coordinates.Count - 1].X;
                break;
            }
        }

        var (nativeRender, hasNativeRender) = GetResponsivenessRender(plot);
        return new(liveX, nativeRender, hasNativeRender);
    }

    /// <summary>Cycles native sources, settings, themes and rolling retention on the UI thread.</summary>
    /// <param name="transition">The transition number.</param>
    private void ApplyResponsivenessTransition(uint transition)
    {
        const double narrowLine = 2;
        const double wideLine = 3;
        var command = (transition % ResponsivenessScenarioCount) switch
        {
            0 => _viewModel.ShowAllChartTypesCommand,
            1 => _viewModel.ShowLiveCommand,
            ResponsivenessHistoricScenario => _viewModel.ShowHistoricCommand,
            _ => _viewModel.ShowIndicatorsCommand,
        };
        using var execution = command.Execute().Subscribe();
        _chart.OpenPlotSettings();
        _chart.UseFixedNumberOfPoints = transition % ResponsivenessScenarioCount == 1;
        _chart.NumberPointsPlotted = _chart.UseFixedNumberOfPoints ? BoundedPointBudget : DefaultPointBudget;
        using var themeExecution = _viewModel.ToggleThemeCommand.Execute().Subscribe();
        foreach (var settings in _chart.ViewModel.SeriesSettings)
        {
            settings.LineWidth = transition % ResponsivenessStylePeriod == 0 ? narrowLine : wideLine;
        }
    }

    /// <summary>Stores counters owned exclusively by the background watchdog.</summary>
    private sealed class ResponsivenessState
    {
        /// <summary>Gets the watchdog start timestamp.</summary>
        public long Started { get; } = Stopwatch.GetTimestamp();

        /// <summary>Gets or sets acknowledged native sentinels.</summary>
        public uint Probes { get; set; }

        /// <summary>Gets or sets the transition count.</summary>
        public uint Transitions { get; set; }

        /// <summary>Gets or sets cycling live advances.</summary>
        public uint LiveAdvances { get; set; }

        /// <summary>Gets or sets steady live advances.</summary>
        public uint SteadyLiveAdvances { get; set; }

        /// <summary>Gets or sets nonzero native renders.</summary>
        public uint NativeRenders { get; set; }

        /// <summary>Gets or sets whether the steady phase has started.</summary>
        public bool SteadyStarted { get; set; }

        /// <summary>Gets or sets whether the last sentinel changed scenarios.</summary>
        public bool LastTransition { get; set; }

        /// <summary>Gets or sets the previous native live coordinate.</summary>
        public double? PreviousLiveX { get; set; }

        /// <summary>Gets or sets the maximum acknowledged dispatcher latency.</summary>
        public TimeSpan MaximumLatency { get; set; }
    }

    /// <summary>Captures immutable native data for the background report.</summary>
    /// <param name="LiveX">The latest logger coordinate.</param>
    /// <param name="NativeRender">The actual rendering diagnostics.</param>
    /// <param name="HasNativeRender">Whether the actual native data rectangle is nonzero.</param>
    private sealed record ResponsivenessSnapshot(double? LiveX, string NativeRender, bool HasNativeRender);
}
