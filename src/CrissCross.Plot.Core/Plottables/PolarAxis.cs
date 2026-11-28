// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders a polar coordinate system using configurable spokes and circles.</summary>
public class PolarAxis : IPlottable, IManagesAxisLimits, IHasFill
{
    /// <summary>Gets or sets a value indicating whether the axis is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to transform coordinates.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items associated with this axis.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets the spokes extending outward from the origin.</summary>
    public List<PolarAxisSpoke> Spokes { get; } = [];

    /// <summary>Gets radial positions describing concentric circles centered on the origin.</summary>
    public List<PolarAxisCircle> Circles { get; } = [];

    /// <summary>Gets or sets the rotation applied to all angles; zero points right.</summary>
    public Angle Rotation { get; set; } = Angle.FromDegrees(0);

    /// <summary>Gets or sets a value indicating whether angles increase clockwise.</summary>
    public bool Clockwise { get; set; }

    /// <summary>Gets or sets a value indicating whether circles are drawn as lines between spokes.</summary>
    public bool StraightLines { get; set; }

    /// <summary>Gets or sets a value indicating whether axis limits are adjusted to keep circles round.</summary>
    public bool ManageAxisLimits { get; set; } = true;

    /// <summary>Gets or sets the background fill style.</summary>
    public FillStyle FillStyle { get; set; } = new();

    /// <summary>Gets or sets the background fill color.</summary>
    public Color FillColor { get => FillStyle.Color; set => FillStyle.Color = value; }

    /// <summary>Gets or sets the background hatch color.</summary>
    public Color FillHatchColor { get => FillStyle.HatchColor; set => FillStyle.HatchColor = value; }

    /// <summary>Gets or sets the background hatch pattern.</summary>
    public IHatch? FillHatch { get => FillStyle.Hatch; set => FillStyle.Hatch = value; }

    /// <summary>Replaces circles with evenly spaced ticks from zero to the maximum radius.</summary>
    /// <param name="maximumRadius">The outermost circle radius.</param>
    /// <param name="count">The number of circles.</param>
    public void SetCircles(double maximumRadius, int count)
    {
        double[] positions = new double[count];
        for (int i = 0; i < count; i++)
        {
            positions[i] = maximumRadius * (i + 1) / count;
        }

        SetCircles(positions);
    }

    /// <summary>Replaces circles with unlabeled circles at the specified radii.</summary>
    /// <param name="positions">The circle radii.</param>
    public void SetCircles(double[] positions)
    {
        string[] labels = new string[positions.Length];
        SetCircles(positions, labels);
    }

    /// <summary>Replaces circles with labeled circles at the specified radii.</summary>
    /// <param name="positions">The circle radii.</param>
    /// <param name="labels">The corresponding circle labels.</param>
    /// <exception cref="ArgumentException">The arrays have different lengths.</exception>
    public void SetCircles(double[] positions, string[] labels)
    {
        if (positions.Length != labels.Length)
        {
            throw new ArgumentException($"{nameof(positions)} and {nameof(labels)} must have equal length");
        }

        Circles.Clear();
        for (int i = 0; i < positions.Length; i++)
        {
            PolarAxisCircle circle = new(positions[i]) { Origin = Coordinates.Origin, LabelText = labels[i] };
            Circles.Add(circle);
        }
    }

    /// <summary>Replaces spokes with a set evenly spaced around the circle using degree labels.</summary>
    /// <param name="count">The number of spokes.</param>
    /// <param name="length">The length of each spoke.</param>
    public void SetSpokes(int count, double length) => SetSpokes(count, length, true);

    /// <summary>Replaces spokes with a set evenly spaced around the circle.</summary>
    /// <param name="count">The number of spokes.</param>
    /// <param name="length">The length of each spoke.</param>
    /// <param name="degreeLabels">Whether to label spokes with their angles in degrees.</param>
    public void SetSpokes(int count, double length, bool degreeLabels)
    {
        Angle[] angles = new Angle[count];
        double delta = Constants.FullTurnDegrees / count;

        for (int i = 0; i < count; i++)
        {
            angles[i] = Angle.FromDegrees(delta * i);
        }

        string[] labels = new string[count];
        if (degreeLabels)
        {
            for (int i = 0; i < angles.Length; i++)
            {
                labels[i] = angles[i].Degrees.ToString();
            }
        }

        SetSpokes(angles, length, labels);
    }

    /// <summary>Replaces spokes with labeled spokes at the specified angles.</summary>
    /// <param name="angles">The angles of the spokes.</param>
    /// <param name="length">The length of each spoke.</param>
    /// <param name="labels">The corresponding spoke labels.</param>
    /// <exception cref="ArgumentException">The angle and label arrays have different lengths.</exception>
    public void SetSpokes(Angle[] angles, double length, string[] labels)
    {
        if (angles.Length != labels.Length)
        {
            throw new ArgumentException($"{nameof(angles)} and {nameof(labels)} must have equal length");
        }

        Spokes.Clear();
        for (int i = 0; i < angles.Length; i++)
        {
            PolarAxisSpoke spoke = new(angles[i], length) { LabelText = labels[i] };
            Spokes.Add(spoke);
        }
    }

    /// <summary>Retains the obsolete clockwise spoke configuration overload.</summary>
    /// <param name="labels">The spoke labels.</param>
    /// <param name="length">The length of each spoke.</param>
    /// <param name="clockwise">Whether angles increase clockwise.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete overload to preserve the existing public API.")]
    [Obsolete("Set the Clockwise property, then call the other SetSpokes() overload", true)]
    public void SetSpokes(string[] labels, double length, bool clockwise) { }

    /// <summary>Replaces spokes with evenly spaced spokes carrying the specified labels.</summary>
    /// <param name="labels">The labels of the spokes.</param>
    /// <param name="length">The length of each spoke.</param>
    public void SetSpokes(string[] labels, double length)
    {
        Spokes.Clear();

        Angle[] angles = new Angle[labels.Length];
        double delta = Constants.FullTurnDegrees / labels.Length;
        for (int i = 0; i < labels.Length; i++)
        {
            angles[i] = Angle.FromDegrees(delta * i);
        }

        SetSpokes(angles, length, labels);
    }

    /// <summary>Converts a polar point to Cartesian coordinates.</summary>
    /// <param name="radius">The distance from the origin.</param>
    /// <param name="degrees">The angle in degrees.</param>
    /// <returns>The rotated Cartesian coordinates.</returns>
    public Coordinates GetCoordinates(double radius, double degrees) => GetCoordinates(radius, Angle.FromDegrees(degrees));

    /// <summary>Converts a polar point to Cartesian coordinates.</summary>
    /// <param name="radius">The distance from the origin.</param>
    /// <param name="angle">The angle from the origin.</param>
    /// <returns>The rotated Cartesian coordinates.</returns>
    public Coordinates GetCoordinates(double radius, Angle angle) => GetCoordinates(new PolarCoordinates(radius, angle));

    /// <summary>Converts a polar point to Cartesian coordinates.</summary>
    /// <param name="point">The point in polar coordinates.</param>
    /// <returns>The rotated Cartesian coordinates.</returns>
    public Coordinates GetCoordinates(PolarCoordinates point)
    {
        Angle angleWithRotation = point.Angle + Rotation;

        if (Clockwise)
        {
            angleWithRotation = angleWithRotation.Inverted;
        }

        return point.WithAngle(angleWithRotation).ToCartesian();
    }

    /// <summary>Sets the direction and converts one radius per spoke to Cartesian coordinates.</summary>
    /// <param name="values">The radii corresponding to the spokes.</param>
    /// <param name="clockwise">Whether angles increase clockwise.</param>
    /// <returns>The rotated Cartesian coordinates.</returns>
    /// <exception cref="ArgumentException">The number of radii differs from the number of spokes.</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete overload to preserve the existing public API.")]
    [Obsolete("Set the Clockwise property then call the GetCoordinates() overload that does not pass it in")]
    public Coordinates[] GetCoordinates(IReadOnlyList<double> values, bool clockwise)
    {
        Clockwise = clockwise;
        return GetCoordinates(values);
    }

    /// <summary>Converts one radius per spoke to Cartesian coordinates.</summary>
    /// <param name="radiusValues">The radii corresponding to the spokes.</param>
    /// <returns>The rotated Cartesian coordinates.</returns>
    /// <exception cref="ArgumentException">The number of radii differs from the number of spokes.</exception>
    public Coordinates[] GetCoordinates(IReadOnlyList<double> radiusValues)
    {
        if (radiusValues.Count != Spokes.Count)
        {
            throw new ArgumentException($"{nameof(radiusValues)} must have a length equal to the number of spokes");
        }

        Coordinates[] coordinates = new Coordinates[Spokes.Count];
        for (int i = 0; i < Spokes.Count; i++)
        {
            coordinates[i] = GetCoordinates(radiusValues[i], Spokes[i].Angle);
        }

        return coordinates;
    }

    /// <summary>Gets the square limits enclosing circle radii and spoke labels.</summary>
    /// <returns>The limits, or no limits when the maximum radius is zero.</returns>
    public AxisLimits GetAxisLimits()
    {
        double maxCircleRadius = 0;
        foreach (PolarAxisCircle circle in Circles)
        {
            maxCircleRadius = Math.Max(maxCircleRadius, circle.Radius);
        }

        double maxSpokeLabelRadius = 0;
        foreach (PolarAxisSpoke spoke in Spokes)
        {
            maxSpokeLabelRadius = Math.Max(maxSpokeLabelRadius, spoke.LabelLength);
        }

        double radius = Math.Max(maxCircleRadius, maxSpokeLabelRadius);
        return radius == 0
            ? AxisLimits.NoLimits
            : new AxisLimits(-radius, radius, -radius, radius);
    }

    /// <summary>Adds a square zoom-out rule if the plot has none.</summary>
    /// <param name="plot">The plot whose axis rules are updated.</param>
    public virtual void UpdateAxisLimits(Plot plot)
    {
        foreach (IAxisRule rule in plot.Axes.Rules)
        {
            if (rule is AxisRules.SquareZoomOut)
            {
                return;
            }
        }

        AxisRules.SquareZoomOut squareRule = new(Axes.XAxis, Axes.YAxis);
        plot.Axes.Rules.Add(squareRule);
    }

    /// <summary>Renders the background, spokes, circles, and circle labels.</summary>
    /// <param name="rp">The rendering context.</param>
    public virtual void Render(RenderPack rp)
    {
        RenderBackgroundColor(rp, rp.Paint);
        RenderSpokes(rp, rp.Paint);

        if (StraightLines)
        {
            RenderStraightLines(rp, rp.Paint);
        }
        else
        {
            RenderCircles(rp, rp.Paint);
        }

        RenderCircleLabels(rp, rp.Paint);
    }

    /// <summary>Retains the obsolete circle regeneration overload.</summary>
    /// <param name="count">The number of circles.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete overload to preserve the existing public API.")]
    [Obsolete("use SetCircles()", true)]
    public void RegenerateCircles(int count) { }

    /// <summary>Retains the obsolete default circle regeneration overload.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete overload to preserve the existing public API.")]
    [Obsolete("use SetCircles()", true)]
    public void RegenerateCircles() { }

    /// <summary>Retains the obsolete spoke regeneration overload.</summary>
    /// <param name="count">The number of spokes.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete overload to preserve the existing public API.")]
    [Obsolete("use SetSpokes()", true)]
    public void RegenerateSpokes(int count) { }

    /// <summary>Retains the obsolete default spoke regeneration overload.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete overload to preserve the existing public API.")]
    [Obsolete("use SetSpokes()", true)]
    public void RegenerateSpokes() { }

    /// <summary>Retains the obsolete labeled spoke regeneration overload.</summary>
    /// <param name="labels">The spoke labels.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST2310", Justification = "Retained obsolete overload to preserve the existing public API.")]
    [Obsolete("use SetSpokes()", true)]
    public void RegenerateSpokes(string[] labels) { }

    /// <summary>Fills the background within the outermost circle or spoke.</summary>
    /// <param name="rp">The rendering context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    protected virtual void RenderBackgroundColor(RenderPack rp, Paint paint)
    {
        double maxCircleRadius = 0;
        foreach (PolarAxisCircle circle in Circles)
        {
            maxCircleRadius = Math.Max(maxCircleRadius, circle.Radius);
        }

        double maxSpokeRadius = 0;
        foreach (PolarAxisSpoke spoke in Spokes)
        {
            maxSpokeRadius = Math.Max(maxSpokeRadius, spoke.Length);
        }

        double radius = Math.Max(maxCircleRadius, maxSpokeRadius);
        double pixelsPerUnit = rp.DataRect.Width / Axes.XAxis.Width;
        float radiusPx = (float)(pixelsPerUnit * radius);
        Pixel originPx = Axes.GetPixel(Coordinates.Origin);
        PixelRect rect = new(
            originPx.X - radiusPx,
            originPx.X + radiusPx,
            originPx.Y + radiusPx,
            originPx.Y - radiusPx);
        Drawing.FillOval(rp.Canvas, paint, FillStyle, rect);
    }

    /// <summary>Renders the configured axis spokes and their labels.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    private void RenderSpokes(RenderPack rp, Paint paint)
    {
        using SKAutoCanvasRestore _ = new(rp.Canvas);
        Pixel origin = Axes.GetPixel(Coordinates.Origin);
        rp.Canvas.Translate(origin.X, origin.Y);

        foreach (var spoke in Spokes)
        {
            Angle angle = spoke.Angle + Rotation;
            if (Clockwise)
            {
                angle = angle.Inverted;
            }

            PolarCoordinates tipPoint = new(spoke.Length, angle);
            Pixel tipPixel = Axes.GetPixel(tipPoint.ToCartesian()) - Axes.GetPixel(Coordinates.Origin);
            Drawing.DrawLine(rp.Canvas, paint, new(0, 0), tipPixel, spoke.LineStyle);

            spoke.LabelStyle.Text = spoke.LabelText ?? string.Empty;
            spoke.LabelStyle.Alignment = Alignment.MiddleCenter;

            PolarCoordinates labelPoint = new(spoke.LabelLength, tipPoint.Angle);
            Pixel labelPixel = Axes.GetPixel(labelPoint.ToCartesian()) - Axes.GetPixel(Coordinates.Origin);
            spoke.LabelStyle.Render(rp.Canvas, labelPixel.WithOffset(0, 0), paint);
        }
    }

    /// <summary>Renders the labels for the configured circles.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    private void RenderCircleLabels(RenderPack rp, Paint paint)
    {
        foreach (var circle in Circles)
        {
            Coordinates c = GetCoordinates(radius: circle.Radius, angle: circle.LabelAngle);
            Pixel px = Axes.GetPixel(c);
            circle.LabelStyle.Render(rp.Canvas, px, paint, circle.LabelText);
        }
    }

    /// <summary>Renders the configured concentric circles.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    private void RenderCircles(RenderPack rp, Paint paint)
    {
        double pixelsPerUnit = rp.DataRect.Width / Axes.XAxis.Width;

        for (int i = 0; i < Circles.Count; i++)
        {
            float radiusPx = (float)(pixelsPerUnit * Circles[i].Radius);
            Pixel originPx = Axes.GetPixel(Circles[i].Origin);
            PixelRect rect = new(
                originPx.X - radiusPx,
                originPx.X + radiusPx,
                originPx.Y + radiusPx,
                originPx.Y - radiusPx);
            Drawing.DrawArc(
                rp.Canvas,
                paint,
                Circles[i].LineStyle,
                rect,
                (float)Circles[i].StartAngle.Degrees,
                (float)Circles[i].SweepAngle.Degrees);
        }
    }

    /// <summary>Renders straight segments connecting the spokes at each circle radius.</summary>
    /// <param name="rp">The render context.</param>
    /// <param name="paint">The paint used for drawing.</param>
    private void RenderStraightLines(RenderPack rp, Paint paint)
    {
        for (int i = 0; i < Circles.Count; i++)
        {
            Pixel[] pixels = new Pixel[Spokes.Count];
            for (int j = 0; j < Spokes.Count; j++)
            {
                Coordinates coordinate = GetCoordinates(Circles[i].Radius, Spokes[j].Angle);
                pixels[j] = Axes.GetPixel(coordinate);
            }

            Drawing.DrawPath(rp.Canvas, paint, pixels, Circles[i].LineStyle, true);
        }
    }

    /// <summary>Contains internal constants used by the polar axis.</summary>
    internal static class Constants
    {
        /// <summary>The number of degrees in a full turn.</summary>
        internal const double FullTurnDegrees = 360.0;
    }
}
