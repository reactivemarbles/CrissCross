// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Details about a completed render.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "PerformanceSharp",
    "PSH1005",
    Justification = "Field-by-field value equality would change existing ValueType.Equals semantics for reference-valued collections and caller-supplied arrays.")]
public readonly struct RenderDetails
{
    /// <summary>Size of the plot image in pixel units</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Replacing this public field with a property would break the existing binary API.")]
    public readonly PixelRect FigureRect;

    /// <summary>Size of the data area of the plot in pixel units</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Replacing this public field with a property would break the existing binary API.")]
    public readonly PixelRect DataRect;

    /// <summary>Distance between the data area and the edge of the figure</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Replacing this public field with a property would break the existing binary API.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1456",
        Justification = "RenderDetails is a readonly struct, so C# requires its instance fields to be readonly.")]
    public readonly PixelPadding Padding;

    /// <summary>Total time required to render this image</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Replacing this public field with a property would break the existing binary API.")]
    public readonly TimeSpan Elapsed;

    /// <summary>Time the render was completed</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Replacing this public field with a property would break the existing binary API.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2016",
        Justification = "Changing the public field type from DateTime to DateTimeOffset would break the existing API.")]
    public readonly DateTime Timestamp;

    /// <summary>Each step of the render and how long it took to execute</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Replacing this public field with a property would break the existing binary API.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2322",
        Justification = "Changing this public array to a read-only collection would alter the existing caller-visible API.")]
    public readonly (string ActionName, TimeSpan Duration)[] TimedActions;

    /// <summary>Axis limits of the primary axes for this render</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Replacing this public field with a property would break the existing binary API.")]
    public readonly AxisLimits AxisLimits;

    /// <summary>Axis limits of the primary axes for the previous render</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Replacing this public field with a property would break the existing binary API.")]
    public readonly AxisLimits PreviousAxisLimits;

    /// <summary>Axis limits for every axis</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Replacing this public field with a property would break the existing binary API.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2322",
        Justification = "Changing this public dictionary to a read-only collection would alter the existing caller-visible API.")]
    public readonly Dictionary<IAxis, CoordinateRange> AxisLimitsByAxis;

    /// <summary>Axis limits of all axes from the previous render</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Replacing this public field with a property would break the existing binary API.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2322",
        Justification = "Changing this public dictionary to a read-only collection would alter the existing caller-visible API.")]
    public readonly Dictionary<IAxis, CoordinateRange> PreviousAxisLimitsByAxis;

    /// <summary>Arrangement of all panels</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Replacing this public field with a property would break the existing binary API.")]
    public readonly Layout Layout;

    /// <summary>The number of total renders including this one</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1401",
        Justification = "Replacing this public field with a property would break the existing binary API.")]
    public readonly int Count;

    /// <summary>Initializes a new instance of the <see cref="RenderDetails"/> class.</summary>
    /// <param name="rp">The render pack containing the completed render data.</param>
    /// <param name="actionTimes">The render actions and their elapsed times.</param>
    /// <param name="lastRender">The details from the preceding render.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2010",
        Justification = "RenderDetails records completion at construction; adding a TimeProvider parameter would change the existing public constructor API.")]
    public RenderDetails(RenderPack rp, (string ActionName, TimeSpan Duration)[] actionTimes, RenderDetails lastRender)
    {
        // TODO: extend actionTimes report individual plottables, axes, etc.
        FigureRect = rp.FigureRect;
        DataRect = rp.DataRect;
        Padding = new(rp.FigureRect.Size, rp.DataRect);
        Elapsed = rp.Elapsed;
        Timestamp = DateTime.UtcNow;
        TimedActions = actionTimes;
        AxisLimits = rp.Plot.Axes.GetLimits();
        PreviousAxisLimits = lastRender.AxisLimits;
        PreviousAxisLimitsByAxis = lastRender.AxisLimitsByAxis;

        var axisLimitsByAxis = new Dictionary<IAxis, CoordinateRange>();
        foreach (IAxis axis in rp.Plot.Axes.GetAxes())
        {
            axisLimitsByAxis.Add(axis, axis.Range.ToCoordinateRange);
        }

        AxisLimitsByAxis = axisLimitsByAxis;
        Layout = rp.Layout;
        Count = lastRender.Count + 1;
        AxisLimitsChanged = AxisLimitsChangedChecker();
        SizeChanged = !DataRect.Equals(lastRender.DataRect);
    }

    /// <summary>Gets a value indicating whether axis limits changed since the preceding render.</summary>
    public bool AxisLimitsChanged { get; }

    /// <summary>Gets a value indicating whether the data area size changed since the preceding render.</summary>
    public bool SizeChanged { get; }

    /// <summary>Gets the number of pixels per horizontal coordinate unit.</summary>
    public double PxPerUnitX => DataRect.Width / AxisLimits.HorizontalSpan;

    /// <summary>Gets the number of pixels per vertical coordinate unit.</summary>
    public double PxPerUnitY => DataRect.Height / AxisLimits.VerticalSpan;

    /// <summary>Gets the number of horizontal coordinate units per pixel.</summary>
    public double UnitsPerPxX => AxisLimits.HorizontalSpan / DataRect.Width;

    /// <summary>Gets the number of vertical coordinate units per pixel.</summary>
    public double UnitsPerPxY => AxisLimits.VerticalSpan / DataRect.Height;

    /// <summary>Search the <see cref="AxisLimitsByAxis"/> for the specified <paramref name="axis"/> and calculate the pixels per unit.</summary>
    /// <param name="axis">the X-Axis to search for</param>
    /// <param name="value">the pxPerUnitX</param>
    /// <returns>
    /// <see langword="true"/> if the axis was in the collection and a result was calculated. Otherwise <see langword="false"/>
    /// <br/> When false, the <paramref name="value"/> will be set to <see cref="PxPerUnitX"/>
    /// </returns>
    public bool TryGetPixelPerUnitX(IXAxis? axis, out double value)
    {
        // To-DO : Convert to 'GetPixelPerUnitX' ? Throw if axis not found ?
        if (axis is not null && AxisLimitsByAxis.TryGetValue(axis, out var range) && range.Span != 0)
        {
            value = DataRect.Width / range.Span;
            return true;
        }

        value = PxPerUnitX;
        return false;
    }

    /// <summary>Search the <see cref="AxisLimitsByAxis"/> for the specified <paramref name="axis"/> and calculate the pixels per unit.</summary>
    /// <param name="axis">the Y-Axis to search for</param>
    /// <param name="value">the pxPerUnitX</param>
    /// <returns>
    /// <see langword="true"/> if the axis was in the collection and a result was calculated. Otherwise <see langword="false"/>
    /// <br/> When false, the <paramref name="value"/> will be set to <see cref="PxPerUnitY"/>
    /// </returns>
    public bool TryGetPixelPerUnitY(IYAxis? axis, out double value)
    {
        if (axis is not null && AxisLimitsByAxis.TryGetValue(axis, out var range) && range.Span != 0)
        {
            value = DataRect.Height / range.Span;
            return true;
        }

        value = PxPerUnitY;
        return false;
    }

    /// <summary>Search the <see cref="AxisLimitsByAxis"/> for the specified <paramref name="axis"/> and calculate the pixels per unit.</summary>
    /// <param name="axis">the X-Axis to search for</param>
    /// <param name="value">the pxPerUnitX</param>
    /// <returns>
    /// <see langword="true"/> if the axis was in the collection and a result was calculated. Otherwise <see langword="false"/>
    /// <br/> When false, the <paramref name="value"/> will be set to <see cref="PxPerUnitX"/>
    /// </returns>
    public bool TryGetUnitPerPixelX(IXAxis? axis, out double value)
    {
        if (axis is not null && AxisLimitsByAxis.TryGetValue(axis, out var range))
        {
            value = range.Span / DataRect.Width;
            return true;
        }

        value = PxPerUnitX;
        return false;
    }

    /// <summary>Search the <see cref="AxisLimitsByAxis"/> for the specified <paramref name="axis"/> and calculate the pixels per unit.</summary>
    /// <param name="axis">the Y-Axis to search for</param>
    /// <param name="value">the pxPerUnitX</param>
    /// <returns>
    /// <see langword="true"/> if the axis was in the collection and a result was calculated. Otherwise <see langword="false"/>
    /// <br/> When false, the <paramref name="value"/> will be set to <see cref="PxPerUnitY"/>
    /// </returns>
    public bool TryGetUnitPerPixelY(IYAxis? axis, out double value)
    {
        if (axis is not null && AxisLimitsByAxis.TryGetValue(axis, out var range))
        {
            value = range.Span / DataRect.Height;
            return true;
        }

        value = PxPerUnitY;
        return false;
    }

    /// <summary>Determines whether any axis limit changed since the preceding render.</summary>
    /// <returns><see langword="true"/> when the axis limits changed; otherwise, <see langword="false"/>.</returns>
    private bool AxisLimitsChangedChecker()
    {
        if (PreviousAxisLimitsByAxis is null)
        {
            return true;
        }

        foreach (IAxis axis in AxisLimitsByAxis.Keys)
        {
            if (axis is null)
            {
                continue;
            }

            if (!PreviousAxisLimitsByAxis.ContainsKey(axis))
            {
                return true;
            }

            CoordinateRange rangeNow = AxisLimitsByAxis[axis];
            CoordinateRange rangeBefore = PreviousAxisLimitsByAxis[axis];
            if (rangeNow != rangeBefore)
            {
                return true;
            }
        }

        return false;
    }
}
