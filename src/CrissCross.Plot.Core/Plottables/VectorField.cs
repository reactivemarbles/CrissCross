// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders a field of rooted vectors as arrows.</summary>
/// <param name="source">The vector field data source.</param>
public class VectorField(IVectorFieldSource source) : IPlottable, IHasArrow, IHasLegendText
{
    /// <summary>Gets or sets whether this vector field is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position and render this vector field.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the obsolete legend label.</summary>
    [Obsolete("use LegendText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained for public API compatibility; callers should use LegendText.")]
    public string Label { get => LegendText; set => LegendText = value; }

    /// <summary>Gets or sets the text displayed for this vector field in the legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets the style applied to vector arrows.</summary>
    public ArrowStyle ArrowStyle { get; set; } = new() { LineWidth = Constants.DefaultArrowLineWidth, LineColor = Colors.Black };

    /// <summary>Gets or sets the arrow outline color.</summary>
    public Color ArrowLineColor { get => ArrowStyle.LineStyle.Color; set => ArrowStyle.LineStyle.Color = value; }

    /// <summary>Gets or sets the arrow outline width.</summary>
    public float ArrowLineWidth { get => ArrowStyle.LineStyle.Width; set => ArrowStyle.LineStyle.Width = value; }

    /// <summary>Gets or sets the arrow fill color.</summary>
    public Color ArrowFillColor { get => ArrowStyle.FillStyle.Color; set => ArrowStyle.FillStyle.Color = value; }

    /// <summary>Gets or sets the minimum arrow length in pixels.</summary>
    public float ArrowMinimumLength { get => ArrowStyle.MinimumLength; set => ArrowStyle.MinimumLength = value; }

    /// <summary>Gets or sets the maximum arrow length in pixels.</summary>
    public float ArrowMaximumLength { get => ArrowStyle.MaximumLength; set => ArrowStyle.MaximumLength = value; }

    /// <summary>Gets or sets the arrow offset.</summary>
    public float ArrowOffset { get => ArrowStyle.Offset; set => ArrowStyle.Offset = value; }

    /// <summary>Gets or sets the arrow anchor position.</summary>
    public ArrowAnchor ArrowAnchor { get => ArrowStyle.Anchor; set => ArrowStyle.Anchor = value; }

    /// <summary>Gets or sets the arrow width.</summary>
    public float ArrowWidth { get => ArrowStyle.ArrowWidth; set => ArrowStyle.ArrowWidth = value; }

    /// <summary>Gets or sets the arrowhead axis length.</summary>
    public float ArrowheadAxisLength { get => ArrowStyle.ArrowheadAxisLength; set => ArrowStyle.ArrowheadAxisLength = value; }

    /// <summary>Gets or sets the arrowhead length.</summary>
    public float ArrowheadLength { get => ArrowStyle.ArrowheadLength; set => ArrowStyle.ArrowheadLength = value; }

    /// <summary>Gets or sets the arrowhead width.</summary>
    public float ArrowheadWidth { get => ArrowStyle.ArrowheadWidth; set => ArrowStyle.ArrowheadWidth = value; }

    /// <summary>Gets or sets the colormap used to color arrows by magnitude.</summary>
    public IColormap? Colormap { get; set; }

    /// <summary>Gets the legend item for this vector field.</summary>
    public IEnumerable<LegendItem> LegendItems => [new LegendItem { LabelText = LegendText, ArrowStyle = ArrowStyle }];

    /// <summary>Gets or sets the maximum arrow length in pixels.</summary>
    public float MaximumArrowLength { get; set; } = Constants.DefaultMaximumArrowLength;

    /// <summary>Gets the source of the vector data.</summary>
    private IVectorFieldSource Source { get; } = source;

    /// <summary>Gets the axis limits occupied by this vector field.</summary>
    /// <returns>The axis limits of the vector source.</returns>
    public AxisLimits GetAxisLimits() => Source.GetLimits();

    /// <summary>Renders this vector field.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        List<RootedPixelVector> vectors = GetPixelVectors();
        if (vectors.Count == 0)
        {
            return;
        }

        Range magnitudeRange = Helpers.GetMagnitudeRange(vectors);
        Range pixelMagnitudeRange = Helpers.ScaleVectors(vectors, magnitudeRange, MaximumArrowLength);
        ArrowStyle.LineStyle.ApplyToPaint(rp.Paint);
        rp.Paint.SKPaintStyle = SKPaintStyle.StrokeAndFill;

        IColormap? colormap = Colormap;
        if (colormap is null)
        {
            Helpers.RenderVectors(rp.Paint, rp.Canvas, vectors, ArrowStyle);
            return;
        }

        RenderColoredVectors(rp, vectors, pixelMagnitudeRange, colormap);
    }

    /// <summary>Converts source vectors to pixel-space vectors.</summary>
    /// <returns>The vectors positioned in pixel coordinates.</returns>
    private List<RootedPixelVector> GetPixelVectors()
    {
        List<RootedPixelVector> vectors = [];
        foreach (RootedCoordinateVector vector in Source.GetRootedVectors())
        {
            vectors.Add(new(Axes.GetPixel(vector.Point), vector.Vector));
        }

        return vectors;
    }

    /// <summary>Renders vectors grouped by their colormap color while preserving input order.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="vectors">The scaled vectors to render.</param>
    /// <param name="pixelMagnitudeRange">The range of scaled vector magnitudes.</param>
    /// <param name="colormap">The colormap used to choose each vector color.</param>
    private void RenderColoredVectors(RenderPack rp, List<RootedPixelVector> vectors, Range pixelMagnitudeRange, IColormap colormap)
    {
        Dictionary<Color, List<RootedPixelVector>> groupedVectors = [];
        List<KeyValuePair<Color, List<RootedPixelVector>>> orderedGroups = [];
        foreach (RootedPixelVector vector in vectors)
        {
            Color color = colormap.GetColor(vector.Magnitude, pixelMagnitudeRange);
            if (!groupedVectors.TryGetValue(color, out List<RootedPixelVector>? group))
            {
                group = [];
                groupedVectors.Add(color, group);
                orderedGroups.Add(new(color, group));
            }

            group.Add(vector);
        }

        Color originalColor = ArrowStyle.LineColor;
        try
        {
            foreach (KeyValuePair<Color, List<RootedPixelVector>> group in orderedGroups)
            {
                ArrowStyle.LineColor = group.Key;
                Helpers.RenderVectors(rp.Paint, rp.Canvas, group.Value, ArrowStyle);
            }
        }
        finally
        {
            ArrowStyle.LineColor = originalColor;
        }
    }

    /// <summary>Contains constants used by vector field plots.</summary>
    internal static class Constants
    {
        /// <summary>The default width of an arrow line.</summary>
        internal const float DefaultArrowLineWidth = 2;

        /// <summary>The default maximum arrow length in pixels.</summary>
        internal const float DefaultMaximumArrowLength = 25;

        /// <summary>The tolerance for detecting equal vector magnitude bounds.</summary>
        internal const double MagnitudeTolerance = 1e-10;
    }

    /// <summary>Contains static calculations and rendering helpers for vector fields.</summary>
    private static class Helpers
    {
        /// <summary>Gets the magnitude range of the source vectors.</summary>
        /// <param name="vectors">The vectors to measure.</param>
        /// <returns>The range used to normalize vector magnitudes.</returns>
        internal static Range GetMagnitudeRange(List<RootedPixelVector> vectors)
        {
            double minimumMagnitudeSquared = double.PositiveInfinity;
            double maximumMagnitudeSquared = double.NegativeInfinity;
            foreach (RootedPixelVector vector in vectors)
            {
                minimumMagnitudeSquared = Math.Min(minimumMagnitudeSquared, vector.MagnitudeSquared);
                maximumMagnitudeSquared = Math.Max(maximumMagnitudeSquared, vector.MagnitudeSquared);
            }

            double minimumMagnitude = Math.Sqrt(minimumMagnitudeSquared);
            double maximumMagnitude = Math.Sqrt(maximumMagnitudeSquared);
            if (Math.Abs(minimumMagnitude - maximumMagnitude) <= Constants.MagnitudeTolerance)
            {
                return maximumMagnitude <= Constants.MagnitudeTolerance
                    ? new(0, Constants.MagnitudeTolerance)
                    : new(0, maximumMagnitude);
            }

            return new(minimumMagnitude, maximumMagnitude);
        }

        /// <summary>Scales vector lengths and returns the resulting pixel magnitude range.</summary>
        /// <param name="vectors">The vectors to scale.</param>
        /// <param name="magnitudeRange">The original magnitude range.</param>
        /// <param name="maximumLength">The maximum rendered arrow length.</param>
        /// <returns>The range of scaled pixel magnitudes.</returns>
        internal static Range ScaleVectors(List<RootedPixelVector> vectors, Range magnitudeRange, float maximumLength)
        {
            double minimumMagnitudeSquared = double.PositiveInfinity;
            double maximumMagnitudeSquared = double.NegativeInfinity;
            for (int i = 0; i < vectors.Count; i++)
            {
                RootedPixelVector vector = vectors[i];
                double newMagnitude = magnitudeRange.Normalize(vector.Magnitude) * maximumLength;
                double direction = vector.Angle;
                vector.Vector = new((float)(Math.Cos(direction) * newMagnitude), (float)(Math.Sin(direction) * newMagnitude));
                vectors[i] = vector;
                minimumMagnitudeSquared = Math.Min(minimumMagnitudeSquared, vector.MagnitudeSquared);
                maximumMagnitudeSquared = Math.Max(maximumMagnitudeSquared, vector.MagnitudeSquared);
            }

            return new(Math.Sqrt(minimumMagnitudeSquared), Math.Sqrt(maximumMagnitudeSquared));
        }

        /// <summary>Renders a collection of arrow vectors using the provided style.</summary>
        /// <param name="paint">The paint used for drawing.</param>
        /// <param name="canvas">The canvas to draw on.</param>
        /// <param name="vectors">The vectors to render.</param>
        /// <param name="arrowStyle">The arrow appearance.</param>
        internal static void RenderVectors(Paint paint, SKCanvas canvas, IEnumerable<RootedPixelVector> vectors, ArrowStyle arrowStyle)
        {
            using SKPath path = PathStrategies.Arrows.GetPath(vectors, arrowStyle);
            Drawing.DrawArrow(canvas, paint, path, arrowStyle);
        }
    }
}
