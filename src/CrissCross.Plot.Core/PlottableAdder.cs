// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Plot;

/// <summary>Helper methods to create plottable objects and add them to the plot.</summary>
/// <param name="plot">The plot to manage.</param>
public partial class PlottableAdder(Plot plot)
{
    /// <summary>Opacity of the default annotation background.</summary>
    private const double AnnotationBackgroundAlpha = 0.75;

    /// <summary>Padding used around annotation labels.</summary>
    private const int AnnotationLabelPadding = 5;

    /// <summary>Opacity of the default arrow fill.</summary>
    private const double ArrowFillAlpha = 0.3;

    /// <summary>Opacity of default background text.</summary>
    private const double BackgroundTextAlpha = 0.3;

    /// <summary>Number of rows used by the two-line background-text layout.</summary>
    private const int BackgroundTextRowCount = 2;

    /// <summary>Default callout border width.</summary>
    private const int CalloutBorderWidth = 2;

    /// <summary>Default callout font size.</summary>
    private const int CalloutFontSize = 14;

    /// <summary>Opacity of span fills.</summary>
    private const double SpanFillAlpha = 0.2;

    /// <summary>Opacity of span borders.</summary>
    private const double SpanBorderAlpha = 0.5;

    /// <summary>Opacity of interactive area fills.</summary>
    private const double InteractiveFillOpacity = 0.2;

    /// <summary>Opacity of phasor arrows.</summary>
    private const double PhasorAlpha = 0.7;

    /// <summary>Opacity used for default plottable fills.</summary>
    private const double PlottableFillAlpha = 0.5;

    /// <summary>Lightness used for population box fills.</summary>
    private const float PopulationBoxLightness = 0.8F;

    /// <summary>Number of circles in the default radar axis.</summary>
    private const int RadarCircleCount = 4;

    /// <summary>Scale margin applied to radar-axis spokes.</summary>
    private const double RadarSpokeMargin = 1.1;

    /// <summary>Outer positive tick position on the Smith chart.</summary>
    private const double SmithChartThirty = 30;

    /// <summary>Smith-chart tick position with magnitude five.</summary>
    private const double SmithChartFive = 5;

    /// <summary>Smith-chart tick position with magnitude two.</summary>
    private const double SmithChartTwo = 2;

    /// <summary>Smith-chart tick position with magnitude one.</summary>
    private const double SmithChartOne = 1;

    /// <summary>Smith-chart tick position with magnitude one half.</summary>
    private const double SmithChartHalf = 0.5;

    /// <summary>Smith-chart tick position with magnitude one fifth.</summary>
    private const double SmithChartFifth = 0.2;

    /// <summary>Zero tick position on the Smith chart.</summary>
    private const double SmithChartZero = 0;

    /// <summary>Default line width for vector-field arrows.</summary>
    private const float VectorFieldLineWidth = 2;

    /// <summary>Index of the next color to retrieve from the palette.</summary>
    private int _nextColorIndex;

    /// <summary>Gets the plot managed by this helper.</summary>
    public Plot Plot { get; } = plot;

    /// <summary>Gets or sets color set used for adding new plottables.</summary>
    public IPalette Palette { get; set; } = new Palettes.Category10();

    /// <summary>Return the next color of the <see cref="Palette"/>. Colors reset if <see cref="Plot.PlottableList"/> is cleared.</summary>
    /// <param name="incrementCounter">true to advance the palette index; otherwise, false.</param>
    /// <returns>The next color from the palette.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2309", Justification = "Retained optional defaults preserve existing source compatibility.")]
    public Color GetNextColor(bool incrementCounter = true)
    {
        if (Plot.PlottableList.Count == 0)
        {
            _nextColorIndex = 0;
        }

        Color color = Palette.GetColor(_nextColorIndex);

        if (incrementCounter)
        {
            _nextColorIndex++;
        }

        return color;
    }
}
