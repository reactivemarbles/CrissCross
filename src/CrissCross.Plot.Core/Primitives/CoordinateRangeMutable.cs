// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a mutable range of values between two coordinates on a single axis.</summary>
/// <param name="min">The initial minimum coordinate.</param>
/// <param name="max">The initial maximum coordinate.</param>
public sealed class CoordinateRangeMutable(double min, double max) : IEquatable<CoordinateRangeMutable>
{
    /// <summary>Gets a range that has not yet been set.</summary>
    public static CoordinateRangeMutable NotSet => new(double.PositiveInfinity, double.NegativeInfinity);

    /// <summary>Gets an unbounded range.</summary>
    public static CoordinateRangeMutable Infinity => new(double.NegativeInfinity, double.PositiveInfinity);

    /// <summary>Gets or sets the minimum coordinate.</summary>
    public double Min { get; set; } = min;

    /// <summary>Gets or sets the maximum coordinate.</summary>
    public double Max { get; set; } = max;

    /// <summary>Gets the midpoint of the range.</summary>
    public double Center => (Min + Max) / Constants.HalfDivisor;

    /// <summary>Gets the distance between the bounds.</summary>
    public double Span => Max - Min;

    /// <summary>Gets whether the range has a non-zero, finite span.</summary>
    public bool HasBeenSet => NumericConversion.IsReal(Span) && Span != 0;

    /// <summary>Gets this range as an immutable coordinate range.</summary>
    public CoordinateRange ToCoordinateRange => new(Min, Max);

    /// <summary>Gets this range as an immutable range with ascending bounds.</summary>
    public CoordinateRange ToRectifiedCoordinateRange => Min < Max ? new(Min, Max) : new(Max, Min);

    /// <summary>Determines whether two mutable ranges have equal bounds.</summary>
    /// <param name="a">The first range.</param>
    /// <param name="b">The second range.</param>
    /// <returns><see langword="true"/> when both bounds are equal.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2464", Justification = "Retained for compatibility with the existing public value-equality API.")]
    public static bool operator ==(CoordinateRangeMutable a, CoordinateRangeMutable b) => a.Equals(b);

    /// <summary>Determines whether two mutable ranges have different bounds.</summary>
    /// <param name="a">The first range.</param>
    /// <param name="b">The second range.</param>
    /// <returns><see langword="true"/> when any bounds differ.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2464", Justification = "Retained for compatibility with the existing public value-equality API.")]
    public static bool operator !=(CoordinateRangeMutable a, CoordinateRangeMutable b) => !a.Equals(b);

    /// <summary>Formats the range bounds and span.</summary>
    /// <returns>A textual representation of the range.</returns>
    public override string ToString() => $"Min={Min}, Max={Max}, Span={Span}";

    /// <summary>Determines whether the range contains a position, including its bounds.</summary>
    /// <param name="position">The position to test.</param>
    /// <returns><see langword="true"/> when the position is within the range.</returns>
    public bool Contains(double position)
    {
        double lower = Math.Min(Min, Max);
        double upper = Math.Max(Min, Max);
        return position >= lower && position <= upper;
    }

    /// <summary>Expands the range to include a point.</summary>
    /// <param name="value">The value to include.</param>
    public void Expand(double value)
    {
        if (double.IsNaN(value))
        {
            return;
        }

        if (double.IsNaN(Min) || value < Min)
        {
            Min = value;
        }

        if (!double.IsNaN(Max) && value <= Max)
        {
            return;
        }

        Max = value;
    }

    /// <summary>Expands this range to include another mutable range.</summary>
    /// <param name="range">The range to include.</param>
    public void Expand(CoordinateRangeMutable range)
    {
        Expand(range.Min);
        Expand(range.Max);
    }

    /// <summary>Expands this range to include an immutable coordinate range.</summary>
    /// <param name="range">The range to include.</param>
    public void Expand(CoordinateRange range)
    {
        Expand(range.Min);
        Expand(range.Max);
    }

    /// <summary>Resets the range to indicate it has not been set.</summary>
    public void Reset()
    {
        Min = double.PositiveInfinity;
        Max = double.NegativeInfinity;
        if (!HasBeenSet)
        {
            return;
        }

        throw new InvalidOperationException();
    }

    /// <summary>Sets the range bounds.</summary>
    /// <param name="min">The minimum coordinate.</param>
    /// <param name="max">The maximum coordinate.</param>
    public void Set(double min, double max)
    {
        Min = min;
        Max = max;
    }

    /// <summary>Sets this range from an immutable coordinate range.</summary>
    /// <param name="range">The range to copy.</param>
    public void Set(CoordinateRange range)
    {
        if (range.IsInverted)
        {
            Max = range.Min;
            Min = range.Max;
        }
        else
        {
            Min = range.Min;
            Max = range.Max;
        }
    }

    /// <summary>Copies the bounds from another mutable range.</summary>
    /// <param name="range">The range to copy.</param>
    public void Set(CoordinateRangeMutable range)
    {
        Min = range.Min;
        Max = range.Max;
    }

    /// <summary>Copies the bounds from an axis.</summary>
    /// <param name="otherAxis">The axis to copy.</param>
    public void Set(IAxis otherAxis)
    {
        Min = otherAxis.Min;
        Max = otherAxis.Max;
    }

    /// <summary>Moves both bounds by a coordinate delta.</summary>
    /// <param name="delta">The amount to pan.</param>
    public void Pan(double delta)
    {
        Min += delta;
        Max += delta;
    }

    /// <summary>Pans the range by a pointer movement in pixels.</summary>
    /// <param name="mouseDeltaPx">The pointer movement in pixels.</param>
    /// <param name="dataSizePx">The pixel size of the data area.</param>
    public void PanMouse(float mouseDeltaPx, float dataSizePx)
    {
        double pixelsPerUnit = dataSizePx / Span;
        double delta = mouseDeltaPx / pixelsPerUnit;
        Pan(delta);
    }

    /// <summary>Zooms the range by a fraction around its center.</summary>
    /// <param name="frac">The zoom fraction.</param>
    public void ZoomFrac(double frac) => ZoomFrac(frac, Center);

    /// <summary>Zooms the range by a fraction around a coordinate.</summary>
    /// <param name="frac">The zoom fraction.</param>
    /// <param name="zoomTo">The coordinate to zoom around.</param>
    public void ZoomFrac(double frac, double zoomTo)
    {
        double spanLeftX = zoomTo - Min;
        double spanRightX = Max - zoomTo;
        Min = zoomTo - (spanLeftX / frac);
        Max = zoomTo + (spanRightX / frac);
    }

    /// <summary>Zooms out by a multiplier while preserving the center.</summary>
    /// <param name="multiple">The span multiplier.</param>
    public void ZoomOut(double multiple)
    {
        double newSpan = Span * multiple;
        double halfSpan = newSpan / Constants.HalfDivisor;
        Set(Center - halfSpan, Center + halfSpan);
    }

    /// <summary>Zooms the range in response to a pointer movement.</summary>
    /// <param name="deltaPx">The pointer movement in pixels.</param>
    /// <param name="dataSizePx">The pixel size of the data area.</param>
    public void ZoomMouseDelta(float deltaPx, float dataSizePx)
    {
        double deltaFraction = deltaPx / (Math.Abs(deltaPx) + dataSizePx);
        double zoomFraction = Math.Pow(Constants.ZoomBase, deltaFraction);
        ZoomFrac(zoomFraction);
    }

    /// <summary>Determines whether this range equals another mutable range.</summary>
    /// <param name="other">The range to compare.</param>
    /// <returns><see langword="true"/> when both bounds are equal.</returns>
    public bool Equals(CoordinateRangeMutable? other) =>
        other is not null && Equals(Min, other.Min) && Equals(Max, other.Max);

    /// <summary>Determines whether this range equals the specified object.</summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><see langword="true"/> when the object is an equal mutable range.</returns>
    public override bool Equals(object? obj) => obj is CoordinateRangeMutable other && Equals(other);

    /// <summary>Gets a hash code for the range bounds.</summary>
    /// <returns>A hash code based on the mutable bounds.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST1482", Justification = "The mutable bounds and value equality are retained for public API compatibility.")]
    public override int GetHashCode() => Min.GetHashCode() ^ Max.GetHashCode();

    /// <summary>Contains constants used by mutable coordinate ranges.</summary>
    private static class Constants
    {
        /// <summary>The divisor used to calculate half a range span.</summary>
        internal const double HalfDivisor = 2;

        /// <summary>The base used to scale mouse-wheel zoom changes.</summary>
        internal const double ZoomBase = 10;
    }
}
