// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.AxisRules;

/// <summary>Adjusts the vertical axis limits so they align with major tick intervals.</summary>
/// <param name="yAxis">The vertical axis whose limits are snapped to its ticks.</param>
public class SnapToTicksY(
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1305",
        Justification = "Keep the public constructor parameter name for named-argument compatibility.")]
    IYAxis yAxis) : IAxisRule
{
    /// <summary>Gets the axis whose limits are snapped to its major ticks.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1401",
        Justification = "This public field is retained to preserve the existing public API.")]
    public readonly IYAxis YAxis = yAxis;

    /// <summary>The minimum major tick count required to determine a tick interval.</summary>
    private const int MinimumMajorTickCount = 2;

    /// <summary>Applies this rule after layout to snap the vertical axis limits to major ticks.</summary>
    /// <param name="rp">The render data used to determine axis limits and tick positions.</param>
    /// <param name="beforeLayout">Whether the rule is being applied before layout.</param>
    public void Apply(RenderPack rp, bool beforeLayout)
    {
        if (beforeLayout || rp.Plot.LastRender.Count <= 0)
        {
            return;
        }

        ApplyToRenderedAxis(rp);
    }

    /// <summary>Compares axis values exactly to preserve the original range and tick-spacing behavior.</summary>
    /// <param name="first">The first axis value.</param>
    /// <param name="second">The second axis value.</param>
    /// <returns><see langword="true"/> when the values are exactly equal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1473",
        Justification = "Axis limits and tick intervals retain the existing exact comparison semantics; a tolerance would change when snapping occurs.")]
    private static bool AreExactlyEqual(double first, double second) => first == second;

    /// <summary>Updates the limits when regeneration changes the major tick interval.</summary>
    /// <param name="state">The axis orientation, zoom and lock state, prior limits, and mutable candidate limits.</param>
    /// <param name="ticks">The regenerated major tick count and positions.</param>
    /// <param name="tickDelta">The previous major tick interval.</param>
    /// <returns><see langword="true"/> when the tick interval changed.</returns>
    private static bool UpdateLimitsForChangedTickInterval(
        ref (
            bool Inverted,
            bool ZoomedInTop,
            bool ZoomedInBottom,
            bool IsPanning,
            bool TopIsLocked,
            bool BottomIsLocked,
            double OldTop,
            double OldBottom,
            double NewMaximum,
            double NewTop,
            double NewBottom) state,
        (int Count, double First, double Second, double Minimum, double Maximum) ticks,
        double tickDelta)
    {
        var newTickDelta = ticks.Second - ticks.First;
        if (AreExactlyEqual(newTickDelta, tickDelta))
        {
            return false;
        }

        var topTick = state.Inverted ? ticks.Minimum : ticks.Maximum;
        if (!AreExactlyEqual(state.NewTop, topTick) && !state.TopIsLocked)
        {
            state.NewTop = GetTopLimitForChangedTickInterval(
                (state.Inverted, state.ZoomedInTop),
                ticks,
                newTickDelta);
        }

        var bottomTick = state.Inverted ? ticks.Maximum : ticks.Minimum;
        if (AreExactlyEqual(state.NewBottom, bottomTick) || state.BottomIsLocked)
        {
            return true;
        }

        state.NewBottom = GetBottomLimitForChangedTickInterval(
            (state.Inverted, state.ZoomedInBottom),
            ticks,
            newTickDelta);

        return true;
    }

    /// <summary>Calculates the top limit for a changed major tick interval.</summary>
    /// <param name="state">The axis orientation and top zoom state.</param>
    /// <param name="ticks">The regenerated major tick positions.</param>
    /// <param name="tickDelta">The regenerated major tick interval.</param>
    /// <returns>The adjusted top limit.</returns>
    private static double GetTopLimitForChangedTickInterval(
        (bool Inverted, bool ZoomedInTop) state,
        (int Count, double First, double Second, double Minimum, double Maximum) ticks,
        double tickDelta)
    {
        if (state.ZoomedInTop)
        {
            return state.Inverted ? ticks.Minimum : ticks.Maximum;
        }

        return state.Inverted ? ticks.Minimum - tickDelta : ticks.Maximum + tickDelta;
    }

    /// <summary>Calculates the bottom limit for a changed major tick interval.</summary>
    /// <param name="state">The axis orientation and bottom zoom state.</param>
    /// <param name="ticks">The regenerated major tick positions.</param>
    /// <param name="tickDelta">The regenerated major tick interval.</param>
    /// <returns>The adjusted bottom limit.</returns>
    private static double GetBottomLimitForChangedTickInterval(
        (bool Inverted, bool ZoomedInBottom) state,
        (int Count, double First, double Second, double Minimum, double Maximum) ticks,
        double tickDelta)
    {
        if (state.ZoomedInBottom)
        {
            return state.Inverted ? ticks.Maximum : ticks.Minimum;
        }

        return state.Inverted ? ticks.Maximum + tickDelta : ticks.Minimum - tickDelta;
    }

    /// <summary>Snaps the candidate limits to the current tick interval while respecting zoom and lock state.</summary>
    /// <param name="state">The mutable axis orientation, zoom, lock, and candidate-limit state.</param>
    /// <param name="ticks">The current major tick count and positions.</param>
    private static void SnapToCurrentTicks(
        ref (
            bool Inverted,
            bool ZoomedInTop,
            bool ZoomedInBottom,
            bool IsPanning,
            bool TopIsLocked,
            bool BottomIsLocked,
            double OldTop,
            double OldBottom,
            double NewMaximum,
            double NewTop,
            double NewBottom) state,
        (int Count, double First, double Second, double Minimum, double Maximum) ticks)
    {
        var tickDelta = ticks.Second - ticks.First;
        state.NewTop = state.Inverted ? ticks.Minimum - tickDelta : ticks.Maximum + tickDelta;
        state.NewBottom = state.Inverted ? ticks.Maximum + tickDelta : ticks.Minimum - tickDelta;

        AdjustTopForZoom(ref state, tickDelta);
        AdjustBottomForZoom(ref state, tickDelta);
        ApplyPanAndLocks(ref state, tickDelta);
    }

    /// <summary>Adjusts the top limit to preserve the current zoom.</summary>
    /// <param name="state">The axis orientation, zoom state, and mutable candidate limits.</param>
    /// <param name="tickDelta">The current major tick interval.</param>
    private static void AdjustTopForZoom(
        ref (
            bool Inverted,
            bool ZoomedInTop,
            bool ZoomedInBottom,
            bool IsPanning,
            bool TopIsLocked,
            bool BottomIsLocked,
            double OldTop,
            double OldBottom,
            double NewMaximum,
            double NewTop,
            double NewBottom) state,
        double tickDelta)
    {
        if (!state.ZoomedInTop)
        {
            return;
        }

        while (Math.Abs(state.NewTop - state.OldBottom) >= Math.Abs(state.OldTop - state.OldBottom))
        {
            state.NewTop += state.Inverted ? tickDelta : -tickDelta;
        }
    }

    /// <summary>Adjusts the bottom limit to preserve the current zoom.</summary>
    /// <param name="state">The axis orientation, zoom state, and mutable candidate limits.</param>
    /// <param name="tickDelta">The current major tick interval.</param>
    private static void AdjustBottomForZoom(
        ref (
            bool Inverted,
            bool ZoomedInTop,
            bool ZoomedInBottom,
            bool IsPanning,
            bool TopIsLocked,
            bool BottomIsLocked,
            double OldTop,
            double OldBottom,
            double NewMaximum,
            double NewTop,
            double NewBottom) state,
        double tickDelta)
    {
        if (!state.ZoomedInBottom)
        {
            return;
        }

        while (Math.Abs(state.NewBottom - state.OldTop) >= Math.Abs(state.OldBottom - state.OldTop))
        {
            state.NewBottom += state.Inverted ? -tickDelta : tickDelta;
        }
    }

    /// <summary>Preserves the pan position and applies any locked endpoints.</summary>
    /// <param name="state">The mutable pan, lock, and candidate-limit state.</param>
    /// <param name="tickDelta">The current major tick interval.</param>
    private static void ApplyPanAndLocks(
        ref (
            bool Inverted,
            bool ZoomedInTop,
            bool ZoomedInBottom,
            bool IsPanning,
            bool TopIsLocked,
            bool BottomIsLocked,
            double OldTop,
            double OldBottom,
            double NewMaximum,
            double NewTop,
            double NewBottom) state,
        double tickDelta)
    {
        if (state.IsPanning && Math.Abs(state.NewMaximum - state.OldTop) < (tickDelta / MinimumMajorTickCount))
        {
            state.NewTop = state.OldTop;
            state.NewBottom = state.OldBottom;
        }

        if (state.TopIsLocked)
        {
            state.NewTop = state.OldTop;
        }

        state.NewBottom = state.BottomIsLocked ? state.OldBottom : state.NewBottom;
    }

    /// <summary>Applies snapping to the rendered axis and corrects for changed tick spacing.</summary>
    /// <param name="rp">The render data used to determine axis limits and tick positions.</param>
    private void ApplyToRenderedAxis(RenderPack rp)
    {
        var oldLimits = rp.Plot.LastRender.AxisLimitsByAxis[YAxis];
        var inverted = oldLimits.IsInverted;
        var oldTop = inverted ? oldLimits.Min : oldLimits.Max;
        var oldBottom = inverted ? oldLimits.Max : oldLimits.Min;
        var newLimits = YAxis.Range;
        var state = (
            Inverted: inverted,
            ZoomedInTop: false,
            ZoomedInBottom: false,
            IsPanning: false,
            TopIsLocked: false,
            BottomIsLocked: false,
            OldTop: oldTop,
            OldBottom: oldBottom,
            NewMaximum: newLimits.Max,
            NewTop: newLimits.Max,
            NewBottom: newLimits.Min);

        if (AreExactlyEqual(state.NewTop, state.OldTop) && AreExactlyEqual(state.NewBottom, state.OldBottom))
        {
            return;
        }

        if (!TryGetLockedLimits(rp, ref state))
        {
            return;
        }

        state.ZoomedInTop = Math.Abs(state.NewTop - state.OldBottom) < Math.Abs(state.OldTop - state.OldBottom);
        state.ZoomedInBottom = Math.Abs(state.NewBottom - state.OldTop) < Math.Abs(state.OldBottom - state.OldTop);
        state.IsPanning = (state.ZoomedInBottom ^ state.ZoomedInTop)
            && !AreExactlyEqual(state.NewTop, state.OldTop)
            && !AreExactlyEqual(state.NewBottom, state.OldBottom);

        var ticks = RegenerateAndGetMajorTicks(rp);
        if (ticks.Count < MinimumMajorTickCount)
        {
            return;
        }

        SnapToCurrentTicks(ref state, ticks);
        SetRangeIfValid(state.NewBottom, state.NewTop);
        ApplyTickIntervalCorrection(rp, ref state, ticks.Second - ticks.First);
    }

    /// <summary>Checks for axis locks and records the applicable top and bottom locks.</summary>
    /// <param name="rp">The render data containing the axis rules.</param>
    /// <param name="state">The mutable lock state for this axis.</param>
    /// <returns><see langword="false"/> when the axis has a vertical lock.</returns>
    private bool TryGetLockedLimits(
        RenderPack rp,
        ref (
            bool Inverted,
            bool ZoomedInTop,
            bool ZoomedInBottom,
            bool IsPanning,
            bool TopIsLocked,
            bool BottomIsLocked,
            double OldTop,
            double OldBottom,
            double NewMaximum,
            double NewTop,
            double NewBottom) state)
    {
        foreach (var rule in rp.Plot.Axes.Rules)
        {
            if (rule is LockedVertical lockedVerticalRule && lockedVerticalRule.YAxis == YAxis)
            {
                return false;
            }

            if (rule is LockedTop lockedTopRule && lockedTopRule.YAxis == YAxis)
            {
                state.TopIsLocked = true;
            }

            if (rule is LockedBottom lockedBottomRule && lockedBottomRule.YAxis == YAxis)
            {
                state.BottomIsLocked = true;
            }
        }

        return true;
    }

    /// <summary>Regenerates axis ticks and summarizes the major tick positions.</summary>
    /// <param name="rp">The render data used to regenerate the ticks.</param>
    /// <returns>The major tick count, first two positions, and minimum and maximum positions.</returns>
    private (int Count, double First, double Second, double Minimum, double Maximum) RegenerateAndGetMajorTicks(RenderPack rp)
    {
        YAxis.RegenerateTicks(new(rp.DataRect.Height), rp.Paint);

        var tickCount = 0;
        var firstTick = 0D;
        var secondTick = 0D;
        var minimumTick = double.PositiveInfinity;
        var maximumTick = double.NegativeInfinity;

        foreach (var tick in YAxis.TickGenerator.Ticks)
        {
            if (!tick.IsMajor)
            {
                continue;
            }

            if (tickCount == 0)
            {
                firstTick = tick.Position;
            }
            else if (tickCount == 1)
            {
                secondTick = tick.Position;
            }

            tickCount++;
            minimumTick = Math.Min(minimumTick, tick.Position);
            maximumTick = Math.Max(maximumTick, tick.Position);
        }

        return (tickCount, firstTick, secondTick, minimumTick, maximumTick);
    }

    /// <summary>Regenerates ticks after snapping and applies a correction if their interval changed.</summary>
    /// <param name="rp">The render data used to regenerate the ticks.</param>
    /// <param name="state">The mutable axis state and candidate limits.</param>
    /// <param name="tickDelta">The tick interval before the candidate limits were applied.</param>
    private void ApplyTickIntervalCorrection(
        RenderPack rp,
        ref (
            bool Inverted,
            bool ZoomedInTop,
            bool ZoomedInBottom,
            bool IsPanning,
            bool TopIsLocked,
            bool BottomIsLocked,
            double OldTop,
            double OldBottom,
            double NewMaximum,
            double NewTop,
            double NewBottom) state,
        double tickDelta)
    {
        var updatedTicks = RegenerateAndGetMajorTicks(rp);
        if (updatedTicks.Count < MinimumMajorTickCount)
        {
            return;
        }

        if (!UpdateLimitsForChangedTickInterval(ref state, updatedTicks, tickDelta))
        {
            return;
        }

        SetRangeIfValid(state.NewBottom, state.NewTop);
    }

    /// <summary>Sets the axis range when the candidate endpoints are different.</summary>
    /// <param name="bottom">The candidate lower endpoint.</param>
    /// <param name="top">The candidate upper endpoint.</param>
    private void SetRangeIfValid(double bottom, double top)
    {
        if (AreExactlyEqual(bottom, top))
        {
            return;
        }

        YAxis.Range.Set(bottom, top);
    }
}
