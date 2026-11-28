// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot.Plottables;

/// <summary>
/// A radial gauge chart is a graphical method of displaying scalar data in the form of 
/// a chart made of circular gauges so that each scalar is represented by each gauge.
/// </summary>
public class RadialGaugePlot : IPlottable
{
    /// <summary>Describes labels drawn on each gauge.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "Retained as a public readonly field to preserve the existing binary API.")]
    public readonly FontStyle Font = new();

    /// <summary>Initializes a new instance of the <see cref="RadialGaugePlot"/> class.</summary>
    /// <param name="levels">The initial level for each gauge.</param>
    /// <param name="colors">The color for each gauge.</param>
    public RadialGaugePlot(double[] levels, Color[] colors)
    {
        Font.Color = CrissCross.Plot.Colors.White;
        Font.Bold = true;
        Update(levels, colors);
    }

    /// <summary>Gets or sets the axes used to position and render this plot.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the original levels used to calculate gauge positions during rendering.</summary>
    public double[] Levels { get; private set; } = [];

    /// <summary>Gets number of gauges.</summary>
    public int GaugeCount => Levels.Length;

    /// <summary>Gets or sets the maximum gauge size in degrees; a full circle is 360 degrees.</summary>
    public double MaximumAngle { get; set; } = Constants.FullTurnDegrees;

    /// <summary>Gets or sets controls whether the backgrounds of the gauges are full circles or stop at the maximum angle.</summary>
    public bool CircularBackground { get; set; } = true;

    /// <summary>Gets the legend labels for each gauge, or null when the gauges have no legend labels.</summary>
    public string[]? Labels { get; }

    /// <summary>Gets or sets the colors for each gauge.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2305", Justification = "Retained replaceable array property for source and binary compatibility.")]
    public Color[] Colors { get; set; } = [];

    /// <summary>
    /// Gets or sets describes how transparent the unfilled background of each gauge is (0 to 1).
    /// The larger the number the darker the background becomes.
    /// </summary>
    public double BackgroundTransparencyFraction { get; set; } = .15;

    /// <summary>
    /// Gets or sets indicates whether gauges fill clockwise as levels increase.
    /// If false, gauges will fill counter-clockwise (anti-clockwise).
    /// </summary>
    public bool Clockwise { get; set; } = true;

    /// <summary>
    /// Gets or sets determines whether the gauges are drawn stacked (dafault value), sequentially, or as a single gauge (ressembling a pie plot).
    /// </summary>
    public RadialGaugeMode GaugeMode { get; set; } = RadialGaugeMode.Stacked;

    /// <summary>Gets or sets controls whether gauges will be dwan inside-out (true) or outside-in (false).</summary>
    public bool OrderInsideOut { get; set; } = true;

    /// <summary>
    /// Gets or sets defines where the gauge label is written on the gage as a fraction of its length.
    /// Low values place the label near the base and high values place the label at its tip.
    /// </summary>
    public double LabelPositionFraction { get; set; } = 1;

    /// <summary>
    /// Gets or sets angle (degrees) at which the gauges start.
    /// 270° for North (default value), 0° for East, 90° for South, 180° for West, etc.
    /// Expected values in the range [0°-360°], otherwise unexpected side-effects might happen.
    /// </summary>
    public float StartingAngle { get; set; } = 270;

    /// <summary>Gets or sets the empty space between gauges as a fraction of the gauge width.</summary>
    public double SpaceFraction { get; set; } = .5F;

    /// <summary>Gets or sets size of the gague label text as a fraction of the gauge width.</summary>
    public double FontSizeFraction { get; set; } = .75;

    /// <summary>Gets or sets controls if value labels are shown inside the gauges.</summary>
    public bool ShowLevels { get; set; } = true;

    /// <summary>Gets or sets string formatter to use for converting gauge levels to text.</summary>
    public string LevelTextFormat { get; set; } = "0.##";

    /// <summary>Gets or sets style of the tip of the gauge.</summary>
    public SKStrokeCap EndCap { get; set; } = SKStrokeCap.Round;

    /// <summary>Gets or sets style of the base of the gauge.</summary>
    public SKStrokeCap StartCap { get; set; } = SKStrokeCap.Round;

    /// <summary>Gets or sets whether this plot is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the horizontal axis index used by this plot.</summary>
    public int XAxisIndex { get; set; }

    /// <summary>Gets or sets the vertical axis index used by this plot.</summary>
    public int YAxisIndex { get; set; }

    /// <summary>Gets the legend items for the gauges.</summary>
    public IEnumerable<LegendItem> LegendItems
    {
        get
        {
            if (Labels is null)
            {
                return LegendItem.None;
            }

            List<LegendItem> legendItems = [];
            for (int i = 0; i < Labels.Length; i++)
            {
                var item = new LegendItem() { LabelText = Labels[i], LineColor = Colors[i], LineWidth = Constants.LegendLineWidth, MarkerStyle = MarkerStyle.None };
                legendItems.Add(item);
            }

            return legendItems;
        }
    }

    /// <summary>Returns a description of this plot and its gauge count.</summary>
    /// <returns>A description of this radial gauge plot.</returns>
    public override string ToString() => $"RadialGaugePlot with {GaugeCount} gauges.";

    /// <summary>Replaces the gauge levels while retaining the current colors.</summary>
    /// <param name="levels">The new level for each gauge.</param>
    public void Update(double[] levels) => Update(levels, null);

    /// <summary>Replaces the gauge levels and optionally assigns new colors.</summary>
    /// <param name="levels">The new level for each gauge.</param>
    /// <param name="colors">The colors to assign when the gauge count changes.</param>
    public void Update(double[] levels, Color[]? colors)
    {
        if (levels is null || levels.Length == 0)
        {
            throw new ArgumentException("Values must not be null or empty");
        }

        bool numberOfGroupsChanged = (Levels is null) || (levels.Length != Levels.Length);
        if (numberOfGroupsChanged)
        {
            if (colors is null || colors.Length != levels.Length)
            {
                throw new ArgumentException("When changing the number of values a new colors array must be provided");
            }

            Colors = new Color[colors.Length];
            Array.Copy(colors, 0, Colors, 0, colors.Length);
        }

        Levels = levels;
    }

    /// <summary>Validates the current gauge data.</summary>
    public void ValidateData() => ValidateData(false);

    /// <summary>Validates the current gauge data.</summary>
    /// <param name="deep">Whether to perform additional deep validation.</param>
    public void ValidateData(bool deep)
    {
        if (Colors.Length != GaugeCount)
        {
            throw new InvalidOperationException($"{nameof(Colors)} must be an array with length equal to number of values");
        }

        if (Labels is not null && Labels.Length != GaugeCount)
        {
            throw new InvalidOperationException($"If {nameof(Labels)} is not null, it must be the same length as the number of values");
        }

        if (MaximumAngle < 0 || MaximumAngle > Constants.FullTurnDegrees)
        {
            throw new InvalidOperationException($"{nameof(MaximumAngle)} must be [0-360]");
        }

        if (LabelPositionFraction < 0 || LabelPositionFraction > 1)
        {
            throw new InvalidOperationException($"{nameof(LabelPositionFraction)} must be a value from 0 to 1");
        }

        if (SpaceFraction >= 0 && SpaceFraction <= 1)
        {
            return;
        }

        throw new InvalidOperationException($"{nameof(SpaceFraction)} must be from 0 to 1");
    }

    /// <summary>Gets the axis limits occupied by the gauge plot.</summary>
    /// <returns>The axis limits of the gauge plot.</returns>
    public AxisLimits GetAxisLimits()
    {
        double radius = 1 + (Constants.HalfGaugeRadius / GaugeCount) - (SpaceFraction / GaugeCount / Constants.GaugeSpacingDivisor);
        return new(-radius, radius, -radius, radius);
    }

    /// <summary>Renders the gauges and their labels.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        ValidateData();

        (double[] startAngles, double[] sweepAngles, double StartingAngleBackGauges) = GetGaugeAngles(
            values: Levels,
            angleStart: StartingAngle,
            angleRange: MaximumAngle,
            clockwise: Clockwise,
            mode: GaugeMode);

        // Copied from Pie plottable to get the maximum radius centered on the plot area
        Pixel origin = Axes.GetPixel(Coordinates.Origin);
        float minX = Math.Abs(Axes.GetPixelX(1) - origin.X);
        float minY = Math.Abs(Axes.GetPixelY(1) - origin.Y);
        float radius = Math.Min(minX, minY) / GaugeCount;

        float gaugeWidth = radius / ((float)SpaceFraction + 1);

        byte backgroundAlpha = (byte)(Constants.MaximumAlpha * BackgroundTransparencyFraction);

        int index;
        int position;
        for (int i = 0; i < GaugeCount; i++)
        {
            if (GaugeMode == RadialGaugeMode.SingleGauge)
            {
                index = GaugeCount - i - 1;
                position = GaugeCount;
            }
            else
            {
                index = i;
                position = OrderInsideOut ? i + 1 : (GaugeCount - i);
            }

            RadialGauge gauge = new()
            {
                MaximumSizeAngle = MaximumAngle,
                StartAngle = startAngles[index],
                SweepAngle = sweepAngles[index],
                Color = Colors[index],
                BackgroundColor = new(Colors[index].R, Colors[index].G, Colors[index].B, backgroundAlpha),
                Width = gaugeWidth,
                CircularBackground = CircularBackground,
                Clockwise = Clockwise,
                BackStartAngle = StartingAngleBackGauges,
                StartCap = StartCap,
                EndCap = EndCap,
                Mode = GaugeMode,
                Font = Font,
                FontSizeFraction = FontSizeFraction,
                Label = Levels[index].ToString(LevelTextFormat),
                LabelPositionFraction = LabelPositionFraction,
                ShowLabels = ShowLevels,
            };

            float radiusPx = position * radius;
            gauge.Render(rp, radiusPx);
        }
    }

    /// <summary>Calculates the rotational angles for each gauge from the original level values.</summary>
    /// <param name="values">The gauge levels.</param>
    /// <param name="angleStart">The starting angle in degrees.</param>
    /// <param name="angleRange">The total available angle in degrees.</param>
    /// <param name="clockwise">Whether the gauges sweep clockwise.</param>
    /// <param name="mode">The gauge arrangement mode.</param>
    /// <returns>Starting angles, sweep angles, and the background start angle.</returns>
    private static (double[] startAngles, double[] sweepAngles, double backStartAngle) GetGaugeAngles(
        double[] values,
        double angleStart,
        double angleRange,
        bool clockwise,
        RadialGaugeMode mode)
    {
        if (values.Length == 0)
        {
            throw new InvalidOperationException("Sequence contains no elements");
        }

        (double scaleMin, double scaleRange) = GetGaugeScale(values, mode);
        int gaugeCount = values.Length;
        double[] startAngles = new double[gaugeCount];
        double[] sweepAngles = new double[gaugeCount];
        angleStart = RadialGauge.ReduceAngle(angleStart);
        double angleSum = angleStart;
        for (int i = 0; i < gaugeCount; i++)
        {
            double angleSwept = scaleRange > 0 ? angleRange * values[i] / scaleRange : 0;
            if (!clockwise)
            {
                angleSwept *= -1;
            }

            double initialAngle = mode == RadialGaugeMode.Stacked ? angleStart : angleSum;
            angleSum += angleSwept;
            startAngles[i] = initialAngle;
            sweepAngles[i] = angleSwept;
        }

        double backOffset = scaleRange > 0 ? angleRange * scaleMin / scaleRange : 0;
        if (!clockwise)
        {
            backOffset *= -1;
        }

        return (startAngles, sweepAngles, angleStart + backOffset);
    }

    /// <summary>Gets the minimum and range used to scale the gauge level angles.</summary>
    /// <param name="values">The gauge levels.</param>
    /// <param name="mode">The gauge arrangement mode.</param>
    /// <returns>The minimum scale value and total scale range.</returns>
    private static (double scaleMinimum, double scaleRange) GetGaugeScale(double[] values, RadialGaugeMode mode)
    {
        double minimumValue = values[0];
        double maximumAbsoluteValue = Math.Abs(values[0]);
        double absoluteValueSum = maximumAbsoluteValue;
        for (int i = 1; i < values.Length; i++)
        {
            minimumValue = Math.Min(minimumValue, values[i]);
            double absoluteValue = Math.Abs(values[i]);
            maximumAbsoluteValue = Math.Max(maximumAbsoluteValue, absoluteValue);
            absoluteValueSum += absoluteValue;
        }

        if (mode is RadialGaugeMode.Sequential or RadialGaugeMode.SingleGauge)
        {
            return (0, absoluteValueSum);
        }

        double scaleMinimum = Math.Min(0, minimumValue);
        return (scaleMinimum, maximumAbsoluteValue - scaleMinimum);
    }

    /// <summary>Contains constants used by radial gauge plots.</summary>
    internal static class Constants
    {
        /// <summary>The angle of a full turn in degrees.</summary>
        internal const double FullTurnDegrees = 360;

        /// <summary>The fractional radius of an individual gauge.</summary>
        internal const double HalfGaugeRadius = 0.5;

        /// <summary>Divisor used when positioning space between gauges.</summary>
        internal const double GaugeSpacingDivisor = 4;

        /// <summary>The maximum channel value for an opaque color.</summary>
        internal const int MaximumAlpha = 255;

        /// <summary>The legend line width in pixels.</summary>
        internal const float LegendLineWidth = 10;
    }
}
