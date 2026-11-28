// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.AxisRules;

/// <summary>Snaps the horizontal axis limits to major tick marks.</summary>
/// <param name="xAxis">The horizontal axis to constrain.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleSharp",
    "SST1305",
    Justification = "The constructor parameter name is retained for existing named-argument callers.")]
public class SnapToTicksX(IXAxis xAxis) : IAxisRule
{
    /// <summary>The horizontal axis constrained by this rule.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Changing this public field to a property would break the existing binary API.")]
    public readonly IXAxis XAxis = xAxis;

    /// <summary>Minimum number of major ticks required to establish an interval.</summary>
    private const int MinimumMajorTickCount = 2;

    /// <summary>Divisor used to calculate half of a tick interval.</summary>
    private const double HalfTickIntervalDivisor = 2;

    /// <summary>Applies this rule by aligning the horizontal axis limits with major ticks.</summary>
    /// <param name="rp">The render pack containing the plot state.</param>
    /// <param name="beforeLayout">Whether the rule runs before layout.</param>
    public void Apply(RenderPack rp, bool beforeLayout)
    {
        if (beforeLayout || rp.Plot.LastRender.Count == 0)
        {
            return;
        }

        var state = GetAxisState(rp);
        if (SnapCalculations.AreCoordinatesClose(state.NewRight, state.OldRight) && SnapCalculations.AreCoordinatesClose(state.NewLeft, state.OldLeft))
        {
            return;
        }

        var lockedEdges = GetLockedEdges(rp);
        if (lockedEdges.AxisLocked)
        {
            return;
        }

        SnapAxisToTicks(rp, state, lockedEdges);
    }

    /// <summary>Calculates and applies snapped limits for the current tick interval.</summary>
    /// <param name="rp">The render pack used to regenerate ticks.</param>
    /// <param name="state">The preceding and current axis limits.</param>
    /// <param name="lockedEdges">The lock state for each axis edge.</param>
    private void SnapAxisToTicks(
        RenderPack rp,
        (bool Inverted, double OldRight, double OldLeft, double NewRight, double NewLeft, double CurrentMaximum) state,
        (bool AxisLocked, bool LeftIsLocked, bool RightIsLocked) lockedEdges)
    {
        var change = SnapCalculations.GetAxisChange(state);
        XAxis.RegenerateTicks(new(rp.DataRect.Width), rp.Paint);
        var ticks = GetMajorTickData();
        if (ticks.Count < MinimumMajorTickCount)
        {
            return;
        }

        double tickDelta = ticks.Second - ticks.First;
        (double newLeft, double newRight) = SnapCalculations.GetInitialSnappedLimits(state, change, ticks, tickDelta);
        if (change.IsPanning && Math.Abs(state.CurrentMaximum - state.OldRight) < tickDelta / HalfTickIntervalDivisor)
        {
            newRight = state.OldRight;
            newLeft = state.OldLeft;
        }

        if (lockedEdges.RightIsLocked)
        {
            newRight = state.OldRight;
        }

        if (lockedEdges.LeftIsLocked)
        {
            newLeft = state.OldLeft;
        }

        if (!SnapCalculations.AreCoordinatesClose(newLeft, newRight))
        {
            XAxis.Range.Set(newLeft, newRight);
        }

        if (!TryAdjustForUpdatedTickInterval(rp, state.Inverted, change, lockedEdges, tickDelta, ref newLeft, ref newRight))
        {
            return;
        }

        if (SnapCalculations.AreCoordinatesClose(newLeft, newRight))
        {
            return;
        }

        XAxis.Range.Set(newLeft, newRight);
    }

    /// <summary>Gets the preceding and current axis limits in display order.</summary>
    /// <param name="rp">The render pack containing the preceding render.</param>
    /// <returns>The inversion state and the preceding and current axis limits.</returns>
    private (bool Inverted, double OldRight, double OldLeft, double NewRight, double NewLeft, double CurrentMaximum) GetAxisState(RenderPack rp)
    {
        CoordinateRange previousLimits = rp.Plot.LastRender.AxisLimitsByAxis[XAxis];
        bool inverted = previousLimits.IsInverted;
        double oldRight = inverted ? previousLimits.Min : previousLimits.Max;
        double oldLeft = inverted ? previousLimits.Max : previousLimits.Min;
        return (inverted, oldRight, oldLeft, XAxis.Range.Max, XAxis.Range.Min, XAxis.Range.Max);
    }

    /// <summary>Finds whether the axis or either edge is locked by another rule.</summary>
    /// <param name="rp">The render pack containing the plot rules.</param>
    /// <returns>The axis and edge lock state.</returns>
    private (bool AxisLocked, bool LeftIsLocked, bool RightIsLocked) GetLockedEdges(RenderPack rp)
    {
        bool leftIsLocked = false;
        bool rightIsLocked = false;
        foreach (IAxisRule rule in rp.Plot.Axes.Rules)
        {
            if (rule is LockedHorizontal lockedHorizontalRule && lockedHorizontalRule.XAxis == XAxis)
            {
                return (true, false, false);
            }

            if (rule is LockedLeft lockedLeftRule && lockedLeftRule.XAxis == XAxis)
            {
                leftIsLocked = true;
            }

            if (rule is LockedRight lockedRightRule && lockedRightRule.XAxis == XAxis)
            {
                rightIsLocked = true;
            }
        }

        return (false, leftIsLocked, rightIsLocked);
    }

    /// <summary>Gets major tick positions and their range.</summary>
    /// <returns>The count, first two major ticks, and minimum and maximum tick positions.</returns>
    private (int Count, double First, double Second, double Minimum, double Maximum) GetMajorTickData()
    {
        int tickCount = 0;
        double firstTick = 0D;
        double secondTick = 0D;
        double minimumTick = double.PositiveInfinity;
        double maximumTick = double.NegativeInfinity;

        foreach (Tick tick in XAxis.TickGenerator.Ticks)
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

    /// <summary>Recalculates the snapped bounds if changing the limits changed the tick interval.</summary>
    /// <param name="rp">The render pack used to regenerate ticks.</param>
    /// <param name="inverted">Whether the axis is inverted.</param>
    /// <param name="change">The zoom and pan state.</param>
    /// <param name="lockedEdges">The lock state for each axis edge.</param>
    /// <param name="tickDelta">The original tick interval.</param>
    /// <param name="newLeft">The left limit to update.</param>
    /// <param name="newRight">The right limit to update.</param>
    /// <returns><see langword="true"/> when updated limits should be applied.</returns>
    private bool TryAdjustForUpdatedTickInterval(
        RenderPack rp,
        bool inverted,
        (bool ZoomedInRight, bool ZoomedInLeft, bool IsPanning) change,
        (bool AxisLocked, bool LeftIsLocked, bool RightIsLocked) lockedEdges,
        double tickDelta,
        ref double newLeft,
        ref double newRight)
    {
        XAxis.RegenerateTicks(new(rp.DataRect.Width), rp.Paint);
        var ticks = GetMajorTickData();
        if (ticks.Count < MinimumMajorTickCount)
        {
            return false;
        }

        double newTickDelta = ticks.Second - ticks.First;
        if (SnapCalculations.AreCoordinatesClose(newTickDelta, tickDelta))
        {
            return false;
        }

        SnapCalculations.AdjustRightLimit(ticks, inverted, change.ZoomedInRight, lockedEdges.RightIsLocked, newTickDelta, ref newRight);
        SnapCalculations.AdjustLeftLimit(ticks, inverted, change.ZoomedInLeft, lockedEdges.LeftIsLocked, newTickDelta, ref newLeft);
        return true;
    }

    /// <summary>Contains calculations that do not depend on the axis instance.</summary>
    private static class SnapCalculations
    {
        /// <summary>Relative tolerance used when comparing coordinate values.</summary>
        private const double CoordinateComparisonTolerance = 1e-12;

        /// <summary>Calculates outward-snapped limits and moves zoomed edges inward when needed.</summary>
        /// <param name="state">The preceding and current axis limits.</param>
        /// <param name="change">The zoom and pan state.</param>
        /// <param name="ticks">The current major tick data.</param>
        /// <param name="tickDelta">The distance between adjacent major ticks.</param>
        /// <returns>The snapped left and right limits.</returns>
        public static (double Left, double Right) GetInitialSnappedLimits(
            (bool Inverted, double OldRight, double OldLeft, double NewRight, double NewLeft, double CurrentMaximum) state,
            (bool ZoomedInRight, bool ZoomedInLeft, bool IsPanning) change,
            (int Count, double First, double Second, double Minimum, double Maximum) ticks,
            double tickDelta)
        {
            double newRight = state.Inverted ? ticks.Minimum - tickDelta : ticks.Maximum + tickDelta;
            double newLeft = state.Inverted ? ticks.Maximum + tickDelta : ticks.Minimum - tickDelta;

            if (change.ZoomedInRight)
            {
                while (Math.Abs(newRight - state.OldLeft) >= Math.Abs(state.OldRight - state.OldLeft))
                {
                    newRight += state.Inverted ? tickDelta : -tickDelta;
                }
            }

            if (change.ZoomedInLeft)
            {
                while (Math.Abs(newLeft - state.OldRight) >= Math.Abs(state.OldLeft - state.OldRight))
                {
                    newLeft += state.Inverted ? -tickDelta : tickDelta;
                }
            }

            return (newLeft, newRight);
        }

        /// <summary>Determines which side was zoomed and whether the axis is panning.</summary>
        /// <param name="state">The preceding and current axis limits.</param>
        /// <returns>The zoom and pan state.</returns>
        public static (bool ZoomedInRight, bool ZoomedInLeft, bool IsPanning) GetAxisChange(
            (bool Inverted, double OldRight, double OldLeft, double NewRight, double NewLeft, double CurrentMaximum) state)
        {
            bool zoomedInRight = Math.Abs(state.NewRight - state.OldLeft) < Math.Abs(state.OldRight - state.OldLeft);
            bool zoomedInLeft = Math.Abs(state.NewLeft - state.OldRight) < Math.Abs(state.OldLeft - state.OldRight);
            bool isPanning = (zoomedInLeft ^ zoomedInRight)
                && !AreCoordinatesClose(state.NewLeft, state.OldLeft)
                && !AreCoordinatesClose(state.NewRight, state.OldRight);
            return (zoomedInRight, zoomedInLeft, isPanning);
        }

        /// <summary>Compares coordinate values using a scale-aware tolerance.</summary>
        /// <param name="first">The first coordinate.</param>
        /// <param name="second">The second coordinate.</param>
        /// <returns><see langword="true"/> when the coordinates are sufficiently close.</returns>
        public static bool AreCoordinatesClose(double first, double second)
        {
            double tolerance = CoordinateComparisonTolerance * Math.Max(1, Math.Max(Math.Abs(first), Math.Abs(second)));
            return Math.Abs(first - second) <= tolerance;
        }

        /// <summary>Adjusts the right limit after the major tick interval changes.</summary>
        /// <param name="ticks">The updated major tick data.</param>
        /// <param name="inverted">Whether the axis is inverted.</param>
        /// <param name="zoomedIn">Whether the right edge was zoomed in.</param>
        /// <param name="isLocked">Whether the right edge is locked.</param>
        /// <param name="tickDelta">The updated major tick interval.</param>
        /// <param name="newRight">The right limit to update.</param>
        public static void AdjustRightLimit(
            (int Count, double First, double Second, double Minimum, double Maximum) ticks,
            bool inverted,
            bool zoomedIn,
            bool isLocked,
            double tickDelta,
            ref double newRight)
        {
            double rightTick = inverted ? ticks.Minimum : ticks.Maximum;
            if (AreCoordinatesClose(newRight, rightTick) || isLocked)
            {
                return;
            }

            double rightOutsideTick = inverted ? ticks.Minimum - tickDelta : ticks.Maximum + tickDelta;
            newRight = zoomedIn ? rightTick : rightOutsideTick;
        }

        /// <summary>Adjusts the left limit after the major tick interval changes.</summary>
        /// <param name="ticks">The updated major tick data.</param>
        /// <param name="inverted">Whether the axis is inverted.</param>
        /// <param name="zoomedIn">Whether the left edge was zoomed in.</param>
        /// <param name="isLocked">Whether the left edge is locked.</param>
        /// <param name="tickDelta">The updated major tick interval.</param>
        /// <param name="newLeft">The left limit to update.</param>
        public static void AdjustLeftLimit(
            (int Count, double First, double Second, double Minimum, double Maximum) ticks,
            bool inverted,
            bool zoomedIn,
            bool isLocked,
            double tickDelta,
            ref double newLeft)
        {
            double leftTick = inverted ? ticks.Maximum : ticks.Minimum;
            if (AreCoordinatesClose(newLeft, leftTick) || isLocked)
            {
                return;
            }

            double leftOutsideTick = inverted ? ticks.Maximum + tickDelta : ticks.Minimum - tickDelta;
            newLeft = zoomedIn ? leftTick : leftOutsideTick;
        }
    }
}
