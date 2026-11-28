// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.TickGenerators;

/// <summary>Generates numeric ticks with automatically selected spacing.</summary>
public class NumericAutomatic : ITickGenerator
{
    /// <summary>Gets the calculator used to choose decimal tick spacing.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public DecimalTickSpacingCalculator TickSpacingCalculator = new();

    /// <summary>Gets or sets the requested tick count, when specified.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1401", Justification = "This public field is retained for binary API compatibility.")]
    public int? TargetTickCount;

    /// <summary>Gets or sets the generated ticks.</summary>
    public Tick[] Ticks { get; set; } = [];

    /// <summary>Gets or sets the maximum number of ticks.</summary>
    public int MaxTickCount
    {
        get => TickSpacingCalculator.MaximumTickCount;
        set => TickSpacingCalculator.MaximumTickCount = value;
    }

    /// <summary>Gets or sets whether to retain only integer positions.</summary>
    public bool IntegerTicksOnly { get; set; }

    /// <summary>Gets or sets the formatter used for major tick labels.</summary>
    public Func<double, string> LabelFormatter { get; set; } = LabelFormatters.Numeric;

    /// <summary>Gets or sets the generator used to create minor ticks.</summary>
    public IMinorTickGenerator MinorTickGenerator { get; set; } = new EvenlySpacedMinorTickGenerator(Defaults.MinorTickCount);

    /// <summary>Gets or sets the minimum spacing between tick labels in pixels.</summary>
    public float MinimumTickSpacing { get; set; }

    /// <summary>Gets or sets the density multiplier used when estimating label width.</summary>
    public double TickDensity { get; set; } = 1.0; // TODO: consider adding logic to make this a fraction of the width in pixels

    /// <summary>Regenerates ticks for the specified range and axis presentation.</summary>
    /// <param name="range">The visible coordinate range.</param>
    /// <param name="edge">The axis edge.</param>
    /// <param name="size">The axis size.</param>
    /// <param name="paint">The paint settings.</param>
    /// <param name="labelStyle">The tick-label style.</param>
    public void Regenerate(CoordinateRange range, Edge edge, PixelLength size, Paint paint, LabelStyle labelStyle)
    {
        const int EstimatedTickLabelHeight = 12;

        Tick[] generatedTicks = GenerateTicks(range, edge, size, new(EstimatedTickLabelHeight), paint, labelStyle);
        List<Tick> visibleTicks = [];
        foreach (Tick tick in generatedTicks)
        {
            if (range.Contains(tick.Position))
            {
                visibleTicks.Add(tick);
            }
        }

        Ticks = [.. visibleTicks];
    }

    /// <summary>Generates major tick positions and labels, retrying if labels exceed their estimated width.</summary>
    /// <param name="range">The visible coordinate range.</param>
    /// <param name="edge">The axis edge.</param>
    /// <param name="axisLength">The axis length in pixels.</param>
    /// <param name="maxLabelLength">The estimated maximum label length.</param>
    /// <param name="paint">The paint settings.</param>
    /// <param name="labelStyle">The tick-label style.</param>
    /// <param name="depth">The current recalculation depth.</param>
    /// <returns>The generated major and minor ticks.</returns>
    private Tick[] GenerateTicks(in CoordinateRange range, Edge edge, PixelLength axisLength, PixelLength maxLabelLength, Paint paint, LabelStyle labelStyle, int depth = 0)
    {
        const int MaximumRecalculationDepth = 3;

        if (depth > MaximumRecalculationDepth)
        {
            Debug.WriteLine($"Warning: Tick recursion depth = {depth}");
        }

        float labelWidth = Math.Max(MinimumTickSpacing, maxLabelLength.Length * (1 / (float)TickDensity));

        if (TargetTickCount.HasValue)
        {
            labelWidth = axisLength.Length / (TargetTickCount.Value + 1);
        }

        double[] majorTickPositions = TickSpacingCalculator.GenerateTickPositions(range, axisLength, labelWidth);
        string[] majorTickLabels = new string[majorTickPositions.Length];
        for (int i = 0; i < majorTickPositions.Length; i++)
        {
            majorTickLabels[i] = LabelFormatter(majorTickPositions[i]);
        }

        (string largestText, PixelLength actualMaxLength) = edge.IsVertical()
            ? labelStyle.MeasureHighestString(majorTickLabels, paint)
            : labelStyle.MeasureWidestString(majorTickLabels, paint);

        return actualMaxLength.Length > maxLabelLength.Length
            ? GenerateTicks(range, edge, axisLength, actualMaxLength, paint, labelStyle, depth + 1)
            : GenerateFinalTicks(majorTickPositions, majorTickLabels, range);
    }

    /// <summary>Creates major and minor ticks from their positions and labels.</summary>
    /// <param name="positions">The major tick positions.</param>
    /// <param name="labels">The corresponding major tick labels.</param>
    /// <param name="visibleRange">The visible coordinate range.</param>
    /// <returns>The ordered major and minor ticks.</returns>
    private Tick[] GenerateFinalTicks(double[] positions, string[] labels, in CoordinateRange visibleRange)
    {
        const double IntegerPositionTolerance = 0.01;

        List<double> retainedPositions = [with(capacity: positions.Length)];
        List<Tick> majorTicks = [with(capacity: positions.Length)];
        for (int i = 0; i < positions.Length; i++)
        {
            double position = positions[i];
            if (IntegerTicksOnly && Math.Abs(position - (int)position) >= IntegerPositionTolerance)
            {
                continue;
            }

            retainedPositions.Add(position);
            majorTicks.Add(Tick.Major(position, labels[i]));
        }

        double[] minorSourcePositions = [.. retainedPositions];
        List<Tick> ticks = majorTicks;
        foreach (double minorPosition in MinorTickGenerator.GetMinorTicks(minorSourcePositions, visibleRange))
        {
            ticks.Add(Tick.Minor(minorPosition));
        }

        return [.. ticks];
    }

    /// <summary>Stores defaults used by this tick generator.</summary>
    private static class Defaults
    {
        /// <summary>The default number of minor divisions between major ticks.</summary>
        public const int MinorTickCount = 5;
    }
}
