// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.AxisPanels;
using CrissCross.Plot.Grids;

namespace CrissCross.Plot;

/// <summary>Manages axes, grids, panels, scaling, and linked-axis behavior for a plot.</summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "RoslynCommonAnalyzers",
    "SST1201",
    Justification = "Existing member layout is part of the established source shape; reordering would add churn without changing behavior.")]
public class AxisManager
{
    /// <summary>The default horizontal margin fraction.</summary>
    private const double DefaultHorizontalMargin = 0.1;

    /// <summary>The default vertical margin fraction.</summary>
    private const double DefaultVerticalMargin = 0.15;

    /// <summary>The default left margin fraction.</summary>
    private const double DefaultLeftMargin = 0.05;

    /// <summary>The default right margin fraction.</summary>
    private const double DefaultRightMargin = 0.05;

    /// <summary>The default bottom margin fraction.</summary>
    private const double DefaultBottomMargin = 0.07;

    /// <summary>The default top margin fraction.</summary>
    private const double DefaultTopMargin = 0.07;

    /// <summary>The default zoom fraction.</summary>
    private const double DefaultZoom = 1.0;

    /// <summary>The zoom fraction at the invalid boundary.</summary>
    private const double InvalidZoom = 0;

    /// <summary>The minimum pixel size for the bottom axis.</summary>
    private const float BottomAxisMinimumSize = 50;

    /// <summary>The margin fraction used for tight margins.</summary>
    private const double NoMargin = 0;

    /// <summary>The validation message used for invalid zoom fractions.</summary>
    private const string ZoomFractionErrorMessage = "zoom fraction must be >= 0";

    /// <summary>Gets the plot managed by this instance.</summary>
    private readonly Plot _plot;

    /// <summary>Rules for updating axes of other plots when this plot's axis limits change.</summary>
    private readonly List<LinkedAxisRule> _linkedAxisRules = [];

    /// <summary>Initializes a new instance of the <see cref="AxisManager"/> class.</summary>
    /// <param name="plot">The plot whose axes will be managed.</param>
    public AxisManager(Plot plot)
    {
        _plot = plot;

        // setup the default primary X and Y axes
        IXAxis primaryHorizontalAxis = new BottomAxis();
        IYAxis primaryVerticalAxis = new LeftAxis();
        XAxes.Add(primaryHorizontalAxis);
        YAxes.Add(primaryVerticalAxis);

        // add a secondary axes with no label to get right side ticks and padding
        IXAxis secondaryHorizontalAxis = new TopAxis();
        IYAxis secondaryVerticalAxis = new RightAxis();
        XAxes.Add(secondaryHorizontalAxis);
        YAxes.Add(secondaryVerticalAxis);

        // setup the default grid with the primary axes
        DefaultGrid = new(primaryHorizontalAxis, primaryVerticalAxis);
    }

    /// <summary>Performs the default continuous autoscaling action for a render frame.</summary>
    /// <param name="renderPack">The render state used for autoscaling.</param>
    public static void DefaultContinuousAutoscaleAction(RenderPack renderPack) => renderPack.Plot.Axes.AutoScale();

    /// <summary>Determines whether the supplied sequence contains no plottables.</summary>
    /// <param name="plottables">The plottables to inspect.</param>
    /// <returns><see langword="true"/> if the sequence contains no plottables; otherwise, <see langword="false"/>.</returns>
    private static bool HasNoPlottables(IEnumerable<IPlottable> plottables)
    {
        IEnumerator<IPlottable> enumerator = plottables.GetEnumerator();
        try
        {
            return !enumerator.MoveNext();
        }
        finally
        {
            enumerator.Dispose();
        }
    }

    /// <summary>Returns the supplied plottables that belong to this plot.</summary>
    /// <param name="plottables">The plottables to filter.</param>
    /// <returns>An array containing plottables owned by this plot.</returns>
    private IPlottable[] GetPlottablesInPlot(IEnumerable<IPlottable> plottables)
    {
        List<IPlottable> result = [];
        foreach (IPlottable plottable in plottables)
        {
            if (_plot.PlottableList.Contains(plottable))
            {
                result.Add(plottable);
            }
        }

        return [.. result];
    }

    /// <summary>Gets or sets logic that determines padding around the data area when <see cref="AutoScale(bool?, bool?)"/> is called.</summary>
    public IAutoScaler AutoScaler { get; set; } = new AutoScalers.FractionalAutoScaler();

    /// <summary>Gets the primary horizontal axis above the plot.</summary>
    public IXAxis Top
    {
        get
        {
            foreach (IXAxis axis in XAxes)
            {
                if (axis.Edge == Edge.Top)
                {
                    return axis;
                }
            }

            throw new InvalidOperationException("No top axis has been added to the plot.");
        }
    }

    /// <summary>Gets a special panel.</summary>
    public Panels.TitlePanel Title { get; } = new();

    /// <summary>Gets horizontal axes.</summary>
    internal List<IXAxis> XAxes { get; } = [];

    /// <summary>Gets vertical axes.</summary>
    internal List<IYAxis> YAxes { get; } = [];

    /// <summary>Gets panels take up space on one side of the data area (like a colorbar).</summary>
    internal List<IPanel> Panels { get; } = [];

    /// <summary>All axes.</summary>
    /// <returns>The result of the operation.</returns>
    public IEnumerable<IAxis> GetAxes() => [.. XAxes, .. YAxes];

    /// <summary>All axes with the given edge.</summary>
    /// <param name="edge">The edge value.</param>
    /// <returns>The result of the operation.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "PSH1100",
        Justification = "The explicit iteration constructs a compatibility-preserving result array without an intermediate LINQ sequence.")]
    public IEnumerable<IAxis> GetAxes(Edge edge)
    {
        List<IAxis> axes = [];
        foreach (IAxis axis in GetAxes())
        {
            if (axis.Edge == edge)
            {
                axes.Add(axis);
            }
        }

        return axes;
    }

    /// <summary>All X axes.</summary>
    /// <returns>The result of the operation.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1024", Justification = "GetXAxes is an established public method API and changing it to a property would be a breaking API change.")]
    public IEnumerable<IXAxis> GetXAxes() => XAxes;

    /// <summary>All Y axes.</summary>
    /// <returns>The result of the operation.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1024", Justification = "GetYAxes is an established public method API and changing it to a property would be a breaking API change.")]
    public IEnumerable<IYAxis> GetYAxes() => YAxes;

    /// <summary>Returns all axes, panels, and the title.</summary>
    /// <returns>The result of the operation.</returns>
    public IPanel[] GetPanels() => [.. GetAxes(), .. Panels, Title];

    /// <summary>Gets the primary horizontal axis below the plot.</summary>
    public IXAxis Bottom
    {
        get
        {
            foreach (IXAxis axis in XAxes)
            {
                if (axis.Edge == Edge.Bottom)
                {
                    return axis;
                }
            }

            throw new InvalidOperationException("No bottom axis has been added to the plot.");
        }
    }

    /// <summary>Gets the primary vertical axis to the left of the plot.</summary>
    public IYAxis Left
    {
        get
        {
            foreach (IYAxis axis in YAxes)
            {
                if (axis.Edge == Edge.Left)
                {
                    return axis;
                }
            }

            throw new InvalidOperationException("No left axis has been added to the plot.");
        }
    }

    /// <summary>Gets the primary vertical axis to the right of the plot.</summary>
    public IYAxis Right
    {
        get
        {
            foreach (IYAxis axis in YAxes)
            {
                if (axis.Edge == Edge.Right)
                {
                    return axis;
                }
            }

            throw new InvalidOperationException("No right axis has been added to the plot.");
        }
    }

    /// <summary>Gets indicates whether the axis limits have been set (manually or by autoscale).</summary>
    public bool LimitsHaveBeenSet => Bottom.Range.HasBeenSet && Left.Range.HasBeenSet;

    /// <summary>
    /// Gets or sets the standard grid that is added when a Plot is created.
    /// Users can achieve custom grid functionality by disabling the visibility
    /// of this grid and adding their own classes to the List of <see cref="CustomGrids"/>.
    /// </summary>
    public DefaultGrid DefaultGrid { get; set; }

    /// <summary>Gets list of custom grids. If in use, it is likely the default grid visibility should be disabled.</summary>
    public List<IGrid> CustomGrids { get; } = [];

    /// <summary>Gets return the <see cref="DefaultGrid"/> and all <see cref="CustomGrids"/></summary>
    public List<IGrid> AllGrids => [DefaultGrid, .. CustomGrids];

    /// <summary>Gets rules that are applied before each render.</summary>
    public List<IAxisRule> Rules { get; } = [];

    /// <summary>
    /// Gets or sets if enabled, AutoScale() will be called at the start of each render.
    /// This can negatively impact performance of plots with an extremely large number of data points.
    /// </summary>
    public bool ContinuouslyAutoscale { get; set; }

    /// <summary>
    /// Gets or sets when <see cref="ContinuouslyAutoscale"/> is true, 
    /// this action is called before each frame is rendered.
    /// Users can assign their own static function to customize continuous autoscaling behavior.
    /// </summary>
    public Action<RenderPack> ContinuousAutoscaleAction { get; set; } = DefaultContinuousAutoscaleAction;

    /// <summary>Apply a single color to the label, tick labels, tick marks, and frame of all axes.</summary>
    /// <param name="color">The color value.</param>
    public void Color(Color color)
    {
        foreach (IAxis iaxis in _plot.Axes.GetAxes())
        {
            if (iaxis is AxisBase axis)
            {
                axis.Color(color);
            }
        }

        _plot.Axes.Title.Label.ForeColor = color;
    }

    /// <summary>Applies a single color to the label, tick labels, tick marks, and frame of the specified axis.</summary>
    /// <param name="axis">The axis to color.</param>
    /// <param name="color">The color to apply.</param>
    public void Color(IAxis axis, Color color)
    {
        if (axis is AxisBase ab)
        {
            ab.Color(color);
        }

        _plot.Axes.Title.Label.ForeColor = color;
    }

    /// <summary>Set visibility of the frame on every axis.</summary>
    /// <param name="enable">The enable value.</param>
    public void Frame(bool enable)
    {
        foreach (IAxis iaxis in _plot.Axes.GetAxes())
        {
            if (iaxis is AxisBase axis)
            {
                axis.FrameLineStyle.IsVisible = enable;
            }
        }
    }

    /// <summary>Set thickness of the frame on every axis.</summary>
    /// <param name="width">The width value.</param>
    public void FrameWidth(float width)
    {
        foreach (IAxis iaxis in _plot.Axes.GetAxes())
        {
            if (iaxis is AxisBase axis)
            {
                axis.FrameLineStyle.Width = width;
            }
        }
    }

    /// <summary>Set color of the frame on every axis.</summary>
    /// <param name="color">The color value.</param>
    public void FrameColor(Color color)
    {
        foreach (IAxis iaxis in _plot.Axes.GetAxes())
        {
            if (iaxis is AxisBase axis)
            {
                axis.FrameLineStyle.Color = color;
            }
        }
    }

    /// <summary>Remove all axes that lie on the given edge.</summary>
    /// <param name="edge">The edge value.</param>
    public void Remove(Edge edge)
    {
        foreach (IAxis axis in GetAxes(edge))
        {
            if (axis is IXAxis xAxis)
            {
                _ = _plot.Axes.XAxes.Remove(xAxis);
            }

            if (axis is IYAxis yAxis)
            {
                _ = _plot.Axes.YAxes.Remove(yAxis);
            }
        }
    }

    /// <summary>Remove the given axis from the plot.</summary>
    /// <param name="axis">The axis value.</param>
    public void Remove(IAxis axis)
    {
        for (int index = XAxes.Count - 1; index >= 0; index--)
        {
            if (XAxes[index] == axis)
            {
                XAxes.RemoveAt(index);
            }
        }

        for (int index = YAxes.Count - 1; index >= 0; index--)
        {
            if (YAxes[index] == axis)
            {
                YAxes.RemoveAt(index);
            }
        }
    }

    /// <summary>Remove the given Panel.</summary>
    /// <param name="panel">The panel value.</param>
    public void Remove(IPanel panel) => _ = Panels.Remove(panel);

    /// <summary>Adds a panel to the plot.</summary>
    /// <param name="panel">The panel to add.</param>
    public void AddPanel(IPanel panel) => Panels.Add(panel);

    /// <summary>Remove all bottom axes, create a DateTime bottom axis, add it to the plot, and return it.</summary>
    /// <returns>The result of the operation.</returns>
    public DateTimeXAxis DateTimeTicksBottom()
    {
        // remove all bottom axes
        _plot.Axes.Remove(Edge.Bottom);

        // create a new bottom axis and add it
        DateTimeXAxis dateAxis = new();
        _plot.Axes.XAxes.Add(dateAxis);

        // setup the grid to use the new bottom axis
        _plot.Axes.DefaultGrid.XAxis = dateAxis;

        // update plottables to use the new axis
        foreach (IPlottable plottable in _plot.GetPlottables())
        {
            if (plottable.Axes.XAxis is not null)
            {
                plottable.Axes.XAxis = dateAxis;
            }
        }

        // autoscale the new axis to fit data already on the plot
        AutoScale();

        return dateAxis;
    }

    /// <summary>Remove all bottom axes, create a numeric bottom axis, add it to the plot, and return it.</summary>
    /// <returns>The result of the operation.</returns>
    public BottomAxis NumericTicksBottom()
    {
        // remove all bottom axes
        _plot.Axes.Remove(Edge.Bottom);

        // create a new bottom axis and add it
        BottomAxis numericAxis = new();
        _plot.Axes.XAxes.Add(numericAxis);

        // setup the grid to use the new bottom axis
        _plot.Axes.DefaultGrid.XAxis = numericAxis;

        // update plottables to use the new axis
        foreach (IPlottable plottable in _plot.GetPlottables())
        {
            if (plottable.Axes.XAxis is not null)
            {
                plottable.Axes.XAxis = numericAxis;
            }
        }

        // autoscale the new axis to fit data already on the plot
        AutoScale();

        return numericAxis;
    }

    /// <summary>Configures multiplier notation for the given axis.</summary>
    /// <param name="axis">The axis that will use multiplier notation.</param>
    /// <returns>The tick modifier label that displays the multiplier.</returns>
    public Plottables.TickModifierLabel SetupMultiplierNotation(IAxis axis)
    {
        // ignore multiple requests to setup multiplier notation on the same axis
        foreach (Plottables.TickModifierLabel existing in _plot.GetPlottables<Plottables.TickModifierLabel>())
        {
            if (existing.Axes.XAxis == axis || existing.Axes.YAxis == axis)
            {
                return existing;
            }
        }

        // use a custom tick formatter to create properly sized placeholder tick labels
        TickGenerators.NumericAutomatic tickGen = new();
        axis.TickGenerator = tickGen;
        tickGen.LabelFormatter = static (x) => "-00.00";

        // create tick modifier labels and add them to the plot
        Plottables.TickModifierLabel tickModifier = new(axis);
        _ = _plot.Add.Plottable(tickModifier);

        // add extra space on the bottom to make room for the label
        if (axis is BottomAxis)
        {
            axis.MinimumSize = BottomAxisMinimumSize;
        }

        // add a pre-render hook to modify tick labels just before each render
        _plot.RenderManager.RenderStarting += (s, e) => tickModifier.UpdateTickLabels();

        return tickModifier;
    }

    /// <summary>Adds a vertical axis to the plot.</summary>
    /// <param name="axis">The vertical axis to add.</param>
    public void AddYAxis(IYAxis axis) => YAxes.Add(axis);

    /// <summary>Adds a horizontal axis to the plot.</summary>
    /// <param name="axis">The horizontal axis to add.</param>
    public void AddXAxis(IXAxis axis) => XAxes.Add(axis);

    /// <summary>Crete a new axis, add it to the plot, and return it.</summary>
    /// <returns>The result of the operation.</returns>
    public LeftAxis AddLeftAxis()
    {
        LeftAxis axis = new();
        YAxes.Add(axis);
        return axis;
    }

    /// <summary>Adds the given axis to the left side of the plot.</summary>
    /// <param name="axis">The left axis to add.</param>
    public void AddLeftAxis(IYAxis axis)
    {
        if (axis.Edge != Edge.Left)
        {
            throw new InvalidOperationException("The given axis is not a Left axis");
        }

        YAxes.Add(axis);
    }

    /// <summary>Crete a new axis, add it to the plot, and return it.</summary>
    /// <returns>The result of the operation.</returns>
    public RightAxis AddRightAxis()
    {
        RightAxis axis = new();
        YAxes.Add(axis);
        return axis;
    }

    /// <summary>Adds the given axis to the right side of the plot.</summary>
    /// <param name="axis">The right axis to add.</param>
    public void AddRightAxis(IYAxis axis)
    {
        if (axis.Edge != Edge.Right)
        {
            throw new InvalidOperationException("The given axis is not a Right axis");
        }

        YAxes.Add(axis);
    }

    /// <summary>Crete a new axis, add it to the plot, and return it.</summary>
    /// <returns>The result of the operation.</returns>
    public BottomAxis AddBottomAxis()
    {
        BottomAxis axis = new();
        XAxes.Add(axis);
        return axis;
    }

    /// <summary>Adds the given axis to the bottom side of the plot.</summary>
    /// <param name="axis">The bottom axis to add.</param>
    public void AddBottomAxis(IXAxis axis)
    {
        if (axis.Edge != Edge.Bottom)
        {
            throw new InvalidOperationException("The given axis is not a Bottom axis");
        }

        XAxes.Add(axis);
    }

    /// <summary>Crete a new axis, add it to the plot, and return it.</summary>
    /// <returns>The result of the operation.</returns>
    public TopAxis AddTopAxis()
    {
        TopAxis axis = new();
        XAxes.Add(axis);
        return axis;
    }

    /// <summary>Adds the given axis to the top side of the plot.</summary>
    /// <param name="axis">The top axis to add.</param>
    public void AddTopAxis(IXAxis axis)
    {
        if (axis.Edge != Edge.Top)
        {
            throw new InvalidOperationException("The given axis is not a Top axis");
        }

        XAxes.Add(axis);
    }

    /// <summary>Sets horizontal limits for the specified axis.</summary>
    /// <param name="left">The minimum horizontal limit.</param>
    /// <param name="right">The maximum horizontal limit.</param>
    /// <param name="xAxis">The horizontal axis whose limits will be changed.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "xAxis is an established public parameter name and is retained for named-argument compatibility.")]
    public void SetLimitsX(double left, double right, IXAxis xAxis)
    {
        xAxis.Min = left;
        xAxis.Max = right;
        if (!xAxis.Range.HasBeenSet)
        {
            return;
        }

        AutoScaler.InvertedX = left > right;
    }

    /// <summary>Sets horizontal limits for the default bottom axis.</summary>
    /// <param name="left">The minimum horizontal limit.</param>
    /// <param name="right">The maximum horizontal limit.</param>
    public void SetLimitsX(double left, double right) => SetLimitsX(left, right, Bottom);

    /// <summary>Sets horizontal limits from a rectangular coordinate range.</summary>
    /// <param name="limits">The coordinate limits to apply.</param>
    public void SetLimitsX(CoordinateRect limits) => SetLimitsX(limits.Left, limits.Right, Bottom);

    /// <summary>Sets horizontal limits from an axis-limit value.</summary>
    /// <param name="limits">The axis limits to apply.</param>
    public void SetLimitsX(AxisLimits limits) => SetLimitsX(limits.Left, limits.Right);

    /// <summary>Sets horizontal limits for the specified axis from an axis-limit value.</summary>
    /// <param name="limits">The axis limits to apply.</param>
    /// <param name="xAxis">The horizontal axis whose limits will be changed.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "xAxis is an established public parameter name and is retained for named-argument compatibility.")]
    public void SetLimitsX(AxisLimits limits, IXAxis xAxis) => SetLimitsX(limits.Left, limits.Right, xAxis);

    /// <summary>Sets vertical limits for the specified axis.</summary>
    /// <param name="bottom">The minimum vertical limit.</param>
    /// <param name="top">The maximum vertical limit.</param>
    /// <param name="yAxis">The vertical axis whose limits will be changed.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "yAxis is an established public parameter name and is retained for named-argument compatibility.")]
    public void SetLimitsY(double bottom, double top, IYAxis yAxis)
    {
        yAxis.Min = bottom;
        yAxis.Max = top;

        if (!yAxis.Range.HasBeenSet)
        {
            return;
        }

        AutoScaler.InvertedY = bottom > top;
    }

    /// <summary>Sets vertical limits for the default left axis.</summary>
    /// <param name="bottom">The minimum vertical limit.</param>
    /// <param name="top">The maximum vertical limit.</param>
    public void SetLimitsY(double bottom, double top) => SetLimitsY(bottom, top, Left);

    /// <summary>Sets vertical limits from a rectangular coordinate range.</summary>
    /// <param name="limits">The coordinate limits to apply.</param>
    public void SetLimitsY(CoordinateRect limits) => SetLimitsY(limits.Bottom, limits.Top, Left);

    /// <summary>Sets vertical limits from an axis-limit value.</summary>
    /// <param name="limits">The axis limits to apply.</param>
    public void SetLimitsY(AxisLimits limits) => SetLimitsY(limits.Bottom, limits.Top);

    /// <summary>Sets vertical limits for the specified axis from an axis-limit value.</summary>
    /// <param name="limits">The axis limits to apply.</param>
    /// <param name="yAxis">The vertical axis whose limits will be changed.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "yAxis is an established public parameter name and is retained for named-argument compatibility.")]
    public void SetLimitsY(AxisLimits limits, IYAxis yAxis) => SetLimitsY(limits.Bottom, limits.Top, yAxis);

    /// <summary>Sets all default axis limits.</summary>
    /// <param name="left">The minimum horizontal limit.</param>
    /// <param name="right">The maximum horizontal limit.</param>
    /// <param name="bottom">The minimum vertical limit.</param>
    /// <param name="top">The maximum vertical limit.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "Left, right, bottom, and top are established public parameter names retained for compatibility.")]
    public void SetLimits(double left, double right, double bottom, double top)
    {
        SetLimitsX(left, right, Bottom);
        SetLimitsY(bottom, top, Left);
    }

    /// <summary>Sets limits for the specified horizontal and vertical axes.</summary>
    /// <param name="left">The minimum horizontal limit.</param>
    /// <param name="right">The maximum horizontal limit.</param>
    /// <param name="bottom">The minimum vertical limit.</param>
    /// <param name="top">The maximum vertical limit.</param>
    /// <param name="xAxis">The horizontal axis whose limits will be changed.</param>
    /// <param name="yAxis">The vertical axis whose limits will be changed.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "xAxis and yAxis are established public parameter names retained for named-argument compatibility.")]
    public void SetLimits(double left, double right, double bottom, double top, IXAxis xAxis, IYAxis yAxis)
    {
        SetLimitsX(left, right, xAxis);
        SetLimitsY(bottom, top, yAxis);
    }

    /// <summary>Sets specified default-axis limits while preserving each unspecified current limit.</summary>
    /// <param name="left">The optional minimum horizontal limit.</param>
    /// <param name="right">The optional maximum horizontal limit.</param>
    /// <param name="bottom">The optional minimum vertical limit.</param>
    /// <param name="top">The optional maximum vertical limit.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional nullable limits are part of the established public API; removing them would break source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "Left, right, bottom, and top are established public named-argument identifiers.")]
    public void SetLimits(double? left = null, double? right = null, double? bottom = null, double? top = null)
    {
        SetLimitsX(left ?? Bottom.Min, right ?? Bottom.Max);
        SetLimitsY(bottom ?? Left.Min, top ?? Left.Max);
    }

    /// <summary>Sets all default axis limits from a rectangular coordinate range.</summary>
    /// <param name="rect">The rectangular coordinate range.</param>
    public void SetLimits(CoordinateRect rect) => SetLimits(rect.Left, rect.Right, rect.Bottom, rect.Top);

    /// <summary>Sets the default axis limits.</summary>
    /// <param name="limits">The limits to apply.</param>
    public void SetLimits(AxisLimits limits) => SetLimits(limits, Bottom, Left);

    /// <summary>Sets limits for the specified horizontal and vertical axes.</summary>
    /// <param name="limits">The axis limits to apply.</param>
    /// <param name="xAxis">The horizontal axis whose limits will be changed.</param>
    /// <param name="yAxis">The vertical axis whose limits will be changed.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "xAxis and yAxis are established public parameter names retained for named-argument compatibility.")]
    public void SetLimits(AxisLimits limits, IXAxis xAxis, IYAxis yAxis)
    {
        SetLimitsX(limits.Left, limits.Right, xAxis);
        SetLimitsY(limits.Bottom, limits.Top, yAxis);
    }

    /// <summary>Sets default axis limits from horizontal and vertical coordinate ranges.</summary>
    /// <param name="xRange">The horizontal coordinate range.</param>
    /// <param name="yRange">The vertical coordinate range.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "xRange and yRange are established public parameter names retained for named-argument compatibility.")]
    public void SetLimits(CoordinateRange xRange, CoordinateRange yRange)
    {
        AxisLimits limits = new(xRange.Min, xRange.Max, yRange.Min, yRange.Max);
        SetLimits(limits);
    }

    /// <summary>Adjust the horizontal axis so values descend from left to right.</summary>
    public void InvertX()
    {
        if (!LimitsHaveBeenSet)
        {
            AutoScale();
        }

        AxisLimits limits = GetLimits();
        double horizontalMin = Math.Min(limits.Left, limits.Right);
        double horizontalMax = Math.Max(limits.Left, limits.Right);
        SetLimitsX(horizontalMax, horizontalMin);
    }

    /// <summary>Adjust the horizontal axis so values ascend from left to right.</summary>
    public void RectifyX()
    {
        if (!LimitsHaveBeenSet)
        {
            AutoScale();
        }

        AxisLimits limits = GetLimits();
        double horizontalMin = Math.Min(limits.Left, limits.Right);
        double horizontalMax = Math.Max(limits.Left, limits.Right);
        SetLimitsX(horizontalMin, horizontalMax);
    }

    /// <summary>Adjust the vertical axis so values descend from bottom to top.</summary>
    public void InvertY()
    {
        if (!LimitsHaveBeenSet)
        {
            AutoScale();
        }

        AxisLimits limits = GetLimits();
        double verticalMin = Math.Min(limits.Bottom, limits.Top);
        double verticalMax = Math.Max(limits.Bottom, limits.Top);
        SetLimitsY(verticalMax, verticalMin);
    }

    /// <summary>Adjust the vertical axis so values ascend from bottom to top.</summary>
    public void RectifyY()
    {
        if (!LimitsHaveBeenSet)
        {
            AutoScale();
        }

        AxisLimits limits = GetLimits();
        double verticalMin = Math.Min(limits.Bottom, limits.Top);
        double verticalMax = Math.Max(limits.Bottom, limits.Top);
        SetLimitsY(verticalMin, verticalMax);
    }

    /// <summary>Return the 2D axis limits for the default axes.</summary>
    /// <returns>The result of the operation.</returns>
    public AxisLimits GetLimits() => GetLimits(Bottom, Left);

    /// <summary>Returns the 2D axis limits for the given horizontal and vertical axes.</summary>
    /// <param name="xAxis">The horizontal axis.</param>
    /// <param name="yAxis">The vertical axis.</param>
    /// <returns>The axis limits for the given axes.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "xAxis and yAxis are established public named-argument identifiers.")]
    public AxisLimits GetLimits(IXAxis xAxis, IYAxis yAxis) => new(xAxis.Min, xAxis.Max, yAxis.Min, yAxis.Max);

    /// <summary>Return the 2D axis limits for the given X/Y axis pair.</summary>
    /// <param name="axes">The axes value.</param>
    /// <returns>The result of the operation.</returns>
    public AxisLimits GetLimits(IAxes axes) => GetLimits(axes.XAxis, axes.YAxis);

    /// <summary>Return the 2D axis limits of data for all plottables using the default axes.</summary>
    /// <returns>The result of the operation.</returns>
    public AxisLimits GetDataLimits() => GetDataLimits(_plot.Axes.Bottom, _plot.Axes.Left);

    /// <summary>Returns data limits for plottables using the given horizontal and vertical axes.</summary>
    /// <param name="xAxis">The horizontal axis.</param>
    /// <param name="yAxis">The vertical axis.</param>
    /// <returns>The combined data limits for matching plottables.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "xAxis and yAxis are established public named-argument identifiers.")]
    public AxisLimits GetDataLimits(IXAxis xAxis, IYAxis yAxis)
    {
        ExpandingAxisLimits expandingLimits = new();

        foreach (IPlottable plottable in _plot.PlottableList)
        {
            if (plottable.Axes.XAxis != xAxis || plottable.Axes.YAxis != yAxis)
            {
                continue;
            }

            expandingLimits.Expand(plottable.GetAxisLimits());
        }

        return expandingLimits.AxisLimits;
    }

    /// <summary>Adds the default X and Y axes to all plottables with unset axes.</summary>
    public void ReplaceNullAxesWithDefaults()
    {
        foreach (var plottable in _plot.PlottableList)
        {
            plottable.Axes.XAxis ??= Bottom;

            plottable.Axes.YAxis ??= Left;
        }
    }

    /// <summary>Automatically scales all axes to fit the data in all plottables.</summary>
    /// <param name="invertX">Whether horizontal autoscaling should be inverted, or null to retain the current setting.</param>
    /// <param name="invertY">Whether vertical autoscaling should be inverted, or null to retain the current setting.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional inversion arguments are part of the established public API; removing them would break source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "InvertX and invertY are established public named-argument identifiers.")]
    public void AutoScale(bool? invertX = null, bool? invertY = null)
    {
        ReplaceNullAxesWithDefaults();
        AutoScaler.InvertedX = invertX ?? AutoScaler.InvertedX;
        AutoScaler.InvertedY = invertY ?? AutoScaler.InvertedY;
        AutoScaler.AutoScaleAll(_plot.PlottableList);
    }

    /// <summary>Autoscales the specified axes to accommodate their plottable data.</summary>
    /// <param name="xAxis">The horizontal axis to scale.</param>
    /// <param name="yAxis">The vertical axis to scale.</param>
    /// <param name="horizontal">Whether to scale the horizontal axis.</param>
    /// <param name="vertical">Whether to scale the vertical axis.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional horizontal and vertical switches are part of the established public API; removing them would break source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "Horizontal and vertical are established public named-argument identifiers.")]
    public void AutoScale(IXAxis xAxis, IYAxis yAxis, bool horizontal = true, bool vertical = true)
    {
        ReplaceNullAxesWithDefaults();

        AxisLimits limits = AutoScaler.GetAxisLimits(_plot, xAxis, yAxis);

        if (horizontal)
        {
            SetLimitsX(limits.Left, limits.Right, xAxis);
        }

        if (!vertical)
        {
            return;
        }

        SetLimitsY(limits.Bottom, limits.Top, yAxis);
    }

    /// <summary>Autoscale the default (left and bottom) axis limits to fit the data of the supplied plottables.</summary>
    /// <param name="plottables">The plottables value.</param>
    public void AutoScale(IEnumerable<IPlottable> plottables)
    {
        if (HasNoPlottables(plottables))
        {
            return;
        }

        ReplaceNullAxesWithDefaults();

        AxisLimits limits = new(GetPlottablesInPlot(plottables));
        SetLimits(limits);
    }

    /// <summary>Automatically expand the default axes to fit the data in all plottables.</summary>
    public void AutoScaleExpand() => AutoScaleExpand(Bottom, Left);

    /// <summary>Automatically expands the specified axes to fit the data in all plottables.</summary>
    /// <param name="xAxis">The horizontal axis to expand.</param>
    /// <param name="yAxis">The vertical axis to expand.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "xAxis and yAxis are established public named-argument identifiers.")]
    public void AutoScaleExpand(IXAxis xAxis, IYAxis yAxis)
    {
        ReplaceNullAxesWithDefaults();
        AxisLimits suggestedLimits = AutoScaler.GetAxisLimits(_plot, xAxis, yAxis);
        AxisLimits currentLimits = GetLimits();

        ExpandingAxisLimits limits = new(currentLimits);
        limits.Expand(suggestedLimits);
        SetLimits(limits.AxisLimits);
    }

    /// <summary>Automatically expands the specified horizontal axis to fit data from its plottables.</summary>
    /// <param name="xAxis">The horizontal axis to expand.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "xAxis is an established public named-argument identifier.")]
    public void AutoScaleExpandX(IXAxis xAxis)
    {
        ReplaceNullAxesWithDefaults();
        AxisLimits suggestedLimits = AutoScaler.GetAxisLimits(_plot, xAxis, Left);
        AxisLimits currentLimits = GetLimits();

        ExpandingAxisLimits limits = new(currentLimits);
        limits.Expand(suggestedLimits);
        SetLimitsX(limits.AxisLimits);
    }

    /// <summary>Automatically expand the default horizontal axis to fit the data in all plottables.</summary>
    public void AutoScaleExpandX() => AutoScaleExpandX(Bottom);

    /// <summary>Automatically expands the specified vertical axis to fit data from its plottables.</summary>
    /// <param name="yAxis">The vertical axis to expand.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "yAxis is an established public named-argument identifier.")]
    public void AutoScaleExpandY(IYAxis yAxis)
    {
        ReplaceNullAxesWithDefaults();
        AxisLimits suggestedLimits = AutoScaler.GetAxisLimits(_plot, Bottom, yAxis);
        AxisLimits currentLimits = GetLimits();

        ExpandingAxisLimits limits = new(currentLimits);
        limits.Expand(suggestedLimits);
        SetLimitsY(limits.AxisLimits);
    }

    /// <summary>Automatically expand the default vertical axis to fit the data in all plottables.</summary>
    public void AutoScaleExpandY() => AutoScaleExpandY(Left);

    /// <summary>Autoscale the bottom horizontal axis limits to fit the data of all plotted objects.</summary>
    public void AutoScaleX() => AutoScaleX(Bottom);

    /// <summary>Autoscales the supplied horizontal axis to fit the data of all plotted objects.</summary>
    /// <param name="xAxis">The horizontal axis to scale.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "xAxis is an established public named-argument identifier.")]
    public void AutoScaleX(IXAxis xAxis)
    {
        ReplaceNullAxesWithDefaults();
        AxisLimits limits = AutoScaler.GetAxisLimits(_plot, xAxis, Left);
        SetLimitsX(limits.Left, limits.Right, xAxis);
    }

    /// <summary>Autoscale the default bottom horizontal axis limits to fit the data of the supplied plottables.</summary>
    /// <param name="plottables">The plottables value.</param>
    public void AutoScaleX(IEnumerable<IPlottable> plottables)
    {
        if (HasNoPlottables(plottables))
        {
            return;
        }

        ReplaceNullAxesWithDefaults();

        AxisLimits limits = new(GetPlottablesInPlot(plottables));
        SetLimitsX(limits);
    }

    /// <summary>Autoscale the left vertical axis limits to fit the data of all plotted objects.</summary>
    public void AutoScaleY() => AutoScaleY(Left);

    /// <summary>Autoscales the supplied vertical axis to fit the data of all plotted objects.</summary>
    /// <param name="yAxis">The vertical axis to scale.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "yAxis is an established public named-argument identifier.")]
    public void AutoScaleY(IYAxis yAxis)
    {
        ReplaceNullAxesWithDefaults();
        AxisLimits limits = AutoScaler.GetAxisLimits(_plot, Bottom, yAxis);
        SetLimitsY(limits.Bottom, limits.Top, yAxis);
    }

    /// <summary>Autoscale the default left vertical axis limits to fit the data of the supplied plottables.</summary>
    /// <param name="plottables">The plottables value.</param>
    public void AutoScaleY(IEnumerable<IPlottable> plottables)
    {
        if (HasNoPlottables(plottables))
        {
            return;
        }

        ReplaceNullAxesWithDefaults();

        AxisLimits limits = new(GetPlottablesInPlot(plottables));
        SetLimitsY(limits);
    }

    /// <summary>Adjusts all axis limits to pan by the pixel distance between two points.</summary>
    /// <param name="mouseDown">The starting pixel position.</param>
    /// <param name="mouseUp">The ending pixel position.</param>
    public void Pan(Pixel mouseDown, Pixel mouseUp)
    {
        // at least one render is required before pixel panning is possible
        if (_plot.RenderManager.LastRender.Count == 0)
        {
            return;
        }

        float mouseDeltaX = mouseUp.X - mouseDown.X;
        float mouseDeltaY = mouseUp.Y - mouseDown.Y;

        // note that the plot axis limits move in the direction OPPOSITE to the mouse.
        // this line also flips the vertical orientation.
        PixelOffset offset = new(-mouseDeltaX, mouseDeltaY);

        Pan(offset);
    }

    /// <summary>Adjust limits all axes to pan given pixel distance.</summary>
    /// <param name="offset">The offset value.</param>
    public void Pan(PixelOffset offset)
    {
        // at least one render is required before pixel panning is possible
        if (_plot.RenderManager.LastRender.Count == 0)
        {
            return;
        }

        XAxes.ForEach(ax => ax.Range.Pan(ax.GetCoordinateDistance(offset.X, _plot.RenderManager.LastRender.DataRect)));
        YAxes.ForEach(ax => ax.Range.Pan(ax.GetCoordinateDistance(offset.Y, _plot.RenderManager.LastRender.DataRect)));
    }

    /// <summary>Adjust limits all axes to pan by the given distance in coordinate space.</summary>
    /// <param name="distance">The distance value.</param>
    public void Pan(CoordinateOffset distance)
    {
        XAxes.ForEach(x => x.Range.Pan(distance.X));
        YAxes.ForEach(x => x.Range.Pan(distance.Y));
    }

    /// <summary>Modifies zoom for all axes as if the mouse were right-clicked and dragged.</summary>
    /// <param name="px1">The starting pixel position.</param>
    /// <param name="px2">The ending pixel position.</param>
    public void Zoom(Pixel px1, Pixel px2)
    {
        // at least one render is required before pixel panning is possible
        if (_plot.RenderManager.LastRender.Count == 0)
        {
            return;
        }

        float pixelDeltaX = px2.X - px1.X;
        float pixelDeltaY = -(px2.Y - px1.Y);

        XAxes.ForEach(horizontalAxis => horizontalAxis.Range.ZoomMouseDelta(pixelDeltaX, _plot.LastRender.DataRect.Width));
        YAxes.ForEach(verticalAxis => verticalAxis.Range.ZoomMouseDelta(pixelDeltaY, _plot.LastRender.DataRect.Height));
    }

    /// <summary>Zooms around the given point using horizontal and vertical scale fractions.</summary>
    /// <param name="px">The pixel position used as the zoom center.</param>
    /// <param name="fracX">The horizontal zoom fraction.</param>
    /// <param name="fracY">The vertical zoom fraction.</param>
    public void Zoom(Pixel px, double fracX, double fracY)
    {
        // at least one render is required before pixel panning is possible
        if (_plot.RenderManager.LastRender.Count == 0)
        {
            return;
        }

        XAxes.ForEach(horizontalAxis => horizontalAxis.Range.ZoomFrac(fracX, horizontalAxis.GetCoordinate(px.X, _plot.LastRender.DataRect)));
        YAxes.ForEach(verticalAxis => verticalAxis.Range.ZoomFrac(fracY, verticalAxis.GetCoordinate(px.Y, _plot.LastRender.DataRect)));
    }

    /// <summary>Modify limits of all axes to apply the given zoom. Fractional values greater than one zoom in and less than one zoom out.</summary>
    /// <param name="fracX">The horizontal zoom fraction.</param>
    /// <param name="fracY">The vertical zoom fraction.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional zoom defaults are part of the established public API; removing them would break source compatibility.")]
    public void Zoom(double fracX = DefaultZoom, double fracY = DefaultZoom)
    {
        if (fracX <= InvalidZoom || fracY <= InvalidZoom)
        {
            throw new ArgumentException(ZoomFractionErrorMessage);
        }

        XAxes.ForEach(horizontalAxis => horizontalAxis.Range.ZoomFrac(fracX));
        YAxes.ForEach(verticalAxis => verticalAxis.Range.ZoomFrac(fracY));
    }

    /// <summary>Modify limits of all axes to zoom in. Fractional values greater than one zoom in and less than one zoom out.</summary>
    /// <param name="fracX">The horizontal zoom fraction.</param>
    /// <param name="fracY">The vertical zoom fraction.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional zoom defaults are part of the established public API; removing them would break source compatibility.")]
    public void ZoomIn(double fracX = DefaultZoom, double fracY = DefaultZoom)
    {
        if (fracX <= InvalidZoom || fracY <= InvalidZoom)
        {
            throw new ArgumentException(ZoomFractionErrorMessage);
        }

        Zoom(fracX, fracY);
    }

    /// <summary>Zoom out so the new view is the given fraction of the original view.</summary>
    /// <param name="x">The horizontal zoom-out fraction.</param>
    /// <param name="y">The vertical zoom-out fraction.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional zoom-out defaults are part of the established public API; removing them would break source compatibility.")]
    public void ZoomOut(double x = DefaultZoom, double y = DefaultZoom)
    {
        if (x <= InvalidZoom || y <= InvalidZoom)
        {
            throw new ArgumentException(ZoomFractionErrorMessage);
        }

        XAxes.ForEach(horizontalAxis => horizontalAxis.Range.ZoomOut(x));
        YAxes.ForEach(verticalAxis => verticalAxis.Range.ZoomOut(y));
    }

    /// <summary>Zoom out so the new view is the given fraction of the original view.</summary>
    /// <param name="x">The horizontal zoom-out fraction.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional zoom-out defaults are part of the established public API; removing them would break source compatibility.")]
    public void ZoomOutX(double x = DefaultZoom)
    {
        if (x <= InvalidZoom)
        {
            throw new ArgumentException(ZoomFractionErrorMessage);
        }

        ZoomOut(x);
    }

    /// <summary>Zoom out so the new view is the given fraction of the original view.</summary>
    /// <param name="y">The vertical zoom-out fraction.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional zoom-out defaults are part of the established public API; removing them would break source compatibility.")]
    public void ZoomOutY(double y = DefaultZoom)
    {
        if (y <= InvalidZoom)
        {
            throw new ArgumentException(ZoomFractionErrorMessage);
        }

        ZoomOut(DefaultZoom, y);
    }

    /// <summary>Reset plot data margins to their default value.</summary>
    public void Margins()
    {
        AutoScaler = new AutoScalers.FractionalAutoScaler();
        AutoScale();
    }

    /// <summary>
    /// Defines the whitespace around the data area when calling <see cref="AutoScale(bool?, bool?)"/>.
    /// Values are fractions from 0 (tight fit) to 1 (substantial whitespace).
    /// </summary>
    /// <param name="horizontal">The horizontal margin fraction.</param>
    /// <param name="vertical">The vertical margin fraction.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional margin defaults are part of the established public API; removing them would break source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2319",
        Justification = "Callers may omit horizontal while specifying vertical by name; removing this default would break source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "The horizontal and vertical parameter names are established public named-argument identifiers.")]
    public void Margins(double horizontal = DefaultHorizontalMargin, double vertical = DefaultVerticalMargin)
    {
        AutoScaler = new AutoScalers.FractionalAutoScaler(horizontal, vertical);
        AutoScale();
    }

    /// <summary>
    /// Defines the whitespace around the data area when calling <see cref="AutoScale(bool?, bool?)"/>.
    /// Values are fractions from 0 (tight fit) to 1 (substantial whitespace).
    /// </summary>
    /// <param name="left">The left margin fraction.</param>
    /// <param name="right">The right margin fraction.</param>
    /// <param name="bottom">The bottom margin fraction.</param>
    /// <param name="top">The top margin fraction.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional margin defaults are part of the established public API; removing them would break source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2319",
        Justification = "Callers may omit left while specifying another margin by name; removing this default would break source compatibility.")]
    public void Margins(double left = DefaultLeftMargin, double right = DefaultRightMargin, double bottom = DefaultBottomMargin, double top = DefaultTopMargin)
    {
        AutoScaler = new AutoScalers.FractionalAutoScaler(left, right, bottom, top);
        AutoScale();
    }

    /// <summary>
    /// Defines the left and right whitespace around the data area when calling <see cref="AutoScale(bool?, bool?)"/>.
    /// Values are fractions from 0 (tight fit) to 1 (substantial whitespace).
    /// </summary>
    /// <param name="horizontal">The horizontal margin fraction.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional margin defaults are part of the established public API; removing them would break source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "The horizontal parameter name is an established public named-argument identifier.")]
    public void MarginsX(double horizontal = DefaultHorizontalMargin)
    {
        if (AutoScaler is AutoScalers.FractionalAutoScaler scaler)
        {
            scaler.SetMarginsX(horizontal);
            AutoScale();
        }
        else
        {
            Margins(horizontal: horizontal);
        }
    }

    /// <summary>
    /// Defines the bottom and top whitespace around the data area when calling <see cref="AutoScale(bool?, bool?)"/>.
    /// Values are fractions from 0 (tight fit) to 1 (substantial whitespace).
    /// </summary>
    /// <param name="vertical">The vertical margin fraction.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional margin defaults are part of the established public API; removing them would break source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "The vertical parameter name is an established public named-argument identifier.")]
    public void MarginsY(double vertical = DefaultVerticalMargin)
    {
        if (AutoScaler is AutoScalers.FractionalAutoScaler scaler)
        {
            scaler.SetMarginsY(vertical);
            AutoScale();
        }
        else
        {
            Margins(horizontal: DefaultHorizontalMargin, vertical: vertical);
        }
    }

    /// <summary>Auto-scale to tightly fit the data so there is no spacing between data points and the edge of the data area.</summary>
    public void TightMargins() => Margins(NoMargin, NoMargin);

    /// <summary>Force pixels to have a 1:1 scale ratio. This allows circles to always appear as circles and not stretched ellipses.</summary>
    public void SquareUnits()
    {
        IAxisRule rule = _plot.PlotControl is null
            ? new AxisRules.SquareZoomOut(Bottom, Left) // best for console apps
            : new AxisRules.SquarePreserveX(Bottom, Left); // best for interactive apps

        Rules.Add(rule);
    }

    /// <summary>
    /// If true, force pixels to have a 1:1 scale ratio.
    /// This allows circles to always appear as circles and not stretched ellipses.
    /// If false, disable axis rules related to square units and restore default behavior.
    /// </summary>
    /// <param name="enable">The enable value.</param>
    public void SquareUnits(bool enable)
    {
        if (enable)
        {
            SquareUnits();
            return;
        }

        List<IAxisRule> rulesToRemove = [];
        foreach (IAxisRule rule in Rules)
        {
            if (rule is AxisRules.SquareZoomOut or AxisRules.SquarePreserveX or AxisRules.SquarePreserveY)
            {
                rulesToRemove.Add(rule);
            }
        }

        foreach (IAxisRule rule in rulesToRemove)
        {
            _ = Rules.Remove(rule);
        }

        AutoScale();
    }

    /// <summary>
    /// Sets visibility for all axes and title panels. Hiding all panels lets the data area extend
    /// to the figure's edges and hides the title; call <c>Plot.Title()</c> to restore it.
    /// </summary>
    /// <param name="hideAllPanels">Whether to hide every axis and the title panel.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "The optional hide-all-panels default is part of the established public API; removing it would break source compatibility.")]
    public void Frameless(bool hideAllPanels = true)
    {
        XAxes.ForEach(x => x.IsVisible = !hideAllPanels);
        YAxes.ForEach(x => x.IsVisible = !hideAllPanels);
        Title.IsVisible = !hideAllPanels;
    }

    /// <summary>Set anti-aliasing for axis frames, tick marks, and grid lines.</summary>
    /// <param name="enable">The enable value.</param>
    public void AntiAlias(bool enable)
    {
        foreach (IAxis candidate in GetAxes())
        {
            if (candidate is not AxisBase axis)
            {
                continue;
            }

            axis.FrameLineStyle.AntiAlias = enable;
            axis.MajorTickStyle.AntiAlias = enable;
            axis.MinorTickStyle.AntiAlias = enable;
        }

        // grid lines
        _plot.Grid.XAxisStyle.MajorLineStyle.AntiAlias = enable;
        _plot.Grid.XAxisStyle.MinorLineStyle.AntiAlias = enable;
        _plot.Grid.YAxisStyle.MajorLineStyle.AntiAlias = enable;
        _plot.Grid.YAxisStyle.MinorLineStyle.AntiAlias = enable;
    }

    /// <summary>
    /// Hairline mode causes lines to always render exactly 1 pixel wide regardless of <see cref="Plot.ScaleFactor"/>.
    /// This function controls hairline behavior for all axis frames. tick marks, and grid lines.
    /// </summary>
    /// <param name="enable">The enable value.</param>
    public void Hairline(bool enable)
    {
        foreach (IAxis candidate in GetAxes())
        {
            if (candidate is not AxisBase axis)
            {
                continue;
            }

            axis.FrameLineStyle.Hairline = enable;
            axis.MajorTickStyle.Hairline = enable;
            axis.MinorTickStyle.Hairline = enable;
        }

        // grid lines
        _plot.Grid.XAxisStyle.MajorLineStyle.Hairline = enable;
        _plot.Grid.XAxisStyle.MinorLineStyle.Hairline = enable;
        _plot.Grid.YAxisStyle.MajorLineStyle.Hairline = enable;
        _plot.Grid.YAxisStyle.MinorLineStyle.Hairline = enable;
    }

    /// <summary>Adds a link to another plot's axis so its limits update when this plot's axis limits change.</summary>
    /// <param name="thisPlotAxis">The axis on this plot to link.</param>
    /// <param name="otherPlotAxis">The corresponding axis on the other plot.</param>
    /// <param name="otherPlot">The other plot.</param>
    public void Link(IAxis thisPlotAxis, IAxis otherPlotAxis, Plot otherPlot)
    {
        bool axisBelongsToPlot = false;
        foreach (IAxis axis in GetAxes())
        {
            if (axis == thisPlotAxis)
            {
                axisBelongsToPlot = true;
                break;
            }
        }

        if (!axisBelongsToPlot)
        {
            throw new InvalidOperationException($"{nameof(thisPlotAxis)} must be an axis from this plot");
        }

        LinkedAxisRule rule = new(thisPlotAxis, otherPlotAxis, otherPlot);
        _linkedAxisRules.Add(rule);
    }

    /// <summary>Adds a link to the target plot control so its axis limits update when this plot's axis limits change.</summary>
    /// <param name="target">The target plot control.</param>
    /// <param name="x">Whether to link the horizontal axes.</param>
    /// <param name="y">Whether to link the vertical axes.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional x/y link switches are part of the established public API; removing them would break source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "X and y are established public named-argument identifiers.")]
    public void Link(IPlotControl target, bool x = true, bool y = true) => Link(target.Plot, x, y);

    /// <summary>Adds a link to the target plot so its axis limits update when this plot's axis limits change.</summary>
    /// <param name="target">The target plot.</param>
    /// <param name="x">Whether to link the horizontal axes.</param>
    /// <param name="y">Whether to link the vertical axes.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2309",
        Justification = "Optional x/y link switches are part of the established public API; removing them would break source compatibility.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "SST1305", Justification = "X and y are established public named-argument identifiers.")]
    public void Link(Plot target, bool x = true, bool y = true)
    {
        if (x)
        {
            Link(_plot.Axes.Bottom, target.Axes.Bottom, target);
        }

        if (!y)
        {
            return;
        }

        Link(_plot.Axes.Left, target.Axes.Left, target);
    }

    /// <summary>Remove all linked axes rules.</summary>
    public void UnlinkAll() => _linkedAxisRules.Clear();

    /// <summary>Remove all linked axes rules involving the target plot control.</summary>
    /// <param name="target">The target value.</param>
    public void Unlink(IPlotControl target) => Unlink(target.Plot);

    /// <summary>Remove all linked axes rules involving the target plot.</summary>
    /// <param name="target">The target value.</param>
    public void Unlink(Plot target) => _ = _linkedAxisRules.RemoveAll(rule => rule.TargetPlot == target);

    /// <summary>Remove all linked axes rules involving the given axis.</summary>
    /// <param name="axis">The axis value.</param>
    public void Unlink(IAxis axis) => _ = _linkedAxisRules.RemoveAll(rule => rule.SourceAxis == axis || rule.TargetAxis == axis);

    /// <summary>This is called in the render system after <see cref="Rendering.RenderManager.AxisLimitsChanged"/> has been invoked.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1473",
        Justification = "Axis synchronization must skip only when both limits exactly match; a tolerance could suppress legitimate small changes.")]
    public void ApplyLinkedAxisRules()
    {
        HashSet<Plot> plotsNeedingUpdates = [];

        foreach (LinkedAxisRule rule in _linkedAxisRules)
        {
            if ((rule.TargetAxis.Min == rule.SourceAxis.Min) && (rule.TargetAxis.Max == rule.SourceAxis.Max))
            {
                continue;
            }

            rule.TargetAxis.Min = rule.SourceAxis.Min;
            rule.TargetAxis.Max = rule.SourceAxis.Max;
            rule.TargetPlot.RenderManager.DisableAxisLimitsChangedEventOnNextRender = true;
            _ = plotsNeedingUpdates.Add(rule.TargetPlot);
        }

        foreach (Plot plot in plotsNeedingUpdates)
        {
            if (plot.PlotControl is null)
            {
                plot.RenderInMemory();
            }
            else
            {
                plot.PlotControl.Refresh();
            }
        }
    }
}
