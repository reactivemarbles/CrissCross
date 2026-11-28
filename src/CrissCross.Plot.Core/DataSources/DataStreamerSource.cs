// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.DataSources;

/// <summary>Stores and manages a fixed-length circular buffer of streamed values.</summary>
public class DataStreamerSource
{
    /// <summary>Initializes a new instance of the <see cref="DataStreamerSource"/> class.</summary>
    /// <param name="data">The circular buffer to manage.</param>
    public DataStreamerSource(double[] data)
    {
        Data = data;
    }

    /// <summary>
    /// Gets the fixed-length circular buffer used to add data at the position defined by <see cref="NextIndex"/>.
    /// Values in this array should not be modified externally when <see cref="CrissCross.Plot.Plottables.DataStreamer.ManageAxisLimits"/> is enabled.
    /// </summary>
    public double[] Data { get; }

    /// <summary>Gets the index in <see cref="Data"/> where the next point will be added.</summary>
    public int NextIndex { get; private set; }

    /// <summary>Gets the index in <see cref="Data"/> holding the newest data point.</summary>
    public int NewestIndex { get; private set; }

    /// <summary>Gets value of the most recently added data point.</summary>
    public double NewestPoint => Data[NewestIndex];

    /// <summary>Gets the number of visible data points to display.</summary>
    public int Length => Data.Length;

    /// <summary>Gets the total number of data points added.</summary>
    public int CountTotal { get; private set; }

    /// <summary>
    /// Gets or sets total of data points added the last time this plottable was rendered.
    /// This can be compared with <see cref="CountTotal"/> to determine if a new render is required.
    /// </summary>
    public int CountTotalOnLastRender { get; set; } = -1;

    /// <summary>Gets the minimum value of all known data, not just data in view.</summary>
    public double DataMin { get; private set; } = double.PositiveInfinity;

    /// <summary>Gets the maximum value of all known data, not just data in view.</summary>
    public double DataMax { get; private set; } = double.NegativeInfinity;

    /// <summary>Gets or sets the horizontal offset of the streamed data.</summary>
    public double OffsetX { get; set; }

    /// <summary>Gets or sets the vertical offset of the streamed data.</summary>
    public double OffsetY { get; set; }

    /// <summary>Gets or sets the horizontal distance between samples.</summary>
    public double SamplePeriod { get; set; } = 1;

    /// <summary>Adds a new value to the circular buffer.</summary>
    /// <param name="value">The value to add.</param>
    public void Add(double value)
    {
        Data[NextIndex] = value;
        NextIndex++;

        if (NextIndex >= Data.Length)
        {
            NextIndex = 0;
        }

        NewestIndex = NextIndex - 1;
        if (NewestIndex < 0)
        {
            NewestIndex = Data.Length - 1;
        }

        DataMin = Math.Min(value, DataMin);
        DataMax = Math.Max(value, DataMax);

        CountTotal++;
    }

    /// <summary>Adds a collection of values to the circular buffer.</summary>
    /// <param name="values">The values to add.</param>
    public void AddRange(IEnumerable<double> values)
    {
        foreach (double value in values)
        {
            Add(value);
        }
    }

    /// <summary>Clears the buffer by setting all values to the specified value.</summary>
    /// <param name="value">The value assigned to every buffer entry.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The optional default is retained to preserve the established source and binary API.")]
    public void Clear(double value = 0)
    {
        for (int i = 0; i < Data.Length; i++)
        {
            Data[i] = value;
        }

        DataMin = value;
        DataMax = value;

        NewestIndex = 0;
        NextIndex = 0;
        CountTotal = 0;
    }

    /// <summary>Gets axis limits for the streamed data.</summary>
    /// <param name="tight">Whether to use extrema of the current buffer.</param>
    /// <returns>The horizontal and vertical axis limits.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2309",
        Justification = "The optional default is retained to preserve the established source and binary API.")]
    public AxisLimits GetAxisLimits(bool tight = true)
    {
        if (double.IsInfinity(DataMin) || double.IsInfinity(DataMax))
        {
            return AxisLimits.NoLimits;
        }

        double minimumXCoordinate = OffsetX;
        double maximumXCoordinate = minimumXCoordinate + (Data.Length * SamplePeriod);
        CoordinateRange horizontalRange = new(minimumXCoordinate, maximumXCoordinate);
        CoordinateRange verticalRange = tight ? CoordinateRange.Extrema(Data) : new(DataMin, DataMax);
        return new(horizontalRange, verticalRange);
    }
}
