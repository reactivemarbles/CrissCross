// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Control that draws a symmetrical arc with rounded edges.</summary>
/// <example>
/// <code lang="xml">
/// &lt;ui:Arc
///     EndAngle="359"
///     StartAngle="0"
///     Stroke="{ui:ThemeResource SystemAccentColorSecondaryBrush}"
///     StrokeThickness="2"
///     Visibility="Visible" /&gt;
/// </code>
/// </example>
public class Arc : System.Windows.Shapes.Shape
{
    /// <summary>Identifies the <see cref="StartAngle"/> dependency property.</summary>
    public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
        nameof(StartAngle),
        typeof(double),
        typeof(Arc),
        new PropertyMetadata(0.0D, PropertyChangedCallback));

    /// <summary>Identifies the <see cref="EndAngle"/> dependency property.</summary>
    public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register(
        nameof(EndAngle),
        typeof(double),
        typeof(Arc),
        new PropertyMetadata(0.0D, PropertyChangedCallback));

    /// <summary>Identifies the <see cref="SweepDirection"/> dependency property.</summary>
    public static readonly DependencyProperty SweepDirectionProperty = DependencyProperty.Register(
        nameof(SweepDirection),
        typeof(SweepDirection),
        typeof(Arc),
        new PropertyMetadata(SweepDirection.Clockwise, PropertyChangedCallback));

    /// <summary>The number of degrees in a full circle.</summary>
    private const double DegreesInFullCircle = 360D;

    /// <summary>The number of degrees in a half circle.</summary>
    private const double DegreesInHalfCircle = 180D;

    /// <summary>The number of degrees in a right angle.</summary>
    private const double DegreesInRightAngle = 90D;

    /// <summary>The divisor used to convert a diameter to a radius.</summary>
    private const double RadiusDivisor = 2D;

    /// <summary>Stores the _rootLayout value.</summary>
    private System.Windows.Controls.Viewbox? _rootLayout;

    /// <summary>Initializes static members of the Arc class.</summary>
    static Arc() =>
        StrokeStartLineCapProperty.OverrideMetadata(
            typeof(Arc),
            new FrameworkPropertyMetadata(PenLineCap.Round, PropertyChangedCallback));

    /// <summary>Gets or sets the initial angle from which the arc will be drawn.</summary>
    public double StartAngle
    {
        get => (double)GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }

    /// <summary>Gets or sets the final angle from which the arc will be drawn.</summary>
    public double EndAngle
    {
        get => (double)GetValue(EndAngleProperty);
        set => SetValue(EndAngleProperty, value);
    }

    /// <summary>Gets or sets the direction to where the arc will be drawn.</summary>
    public SweepDirection SweepDirection
    {
        get => (SweepDirection)GetValue(SweepDirectionProperty);
        set => SetValue(SweepDirectionProperty, value);
    }

    /// <summary>Gets a value indicating whether one of the two larger arc sweeps is chosen; otherwise, if is <see
    /// langword="false"/>, one of the smaller arc sweeps is chosen.</summary>
    public bool IsLargeArc { get; internal set; }

    /// <inheritdoc />
    protected override Geometry DefiningGeometry => DefinedGeometry();

    /// <summary>Event triggered when one of the key parameters is changed. Forces the geometry to be redrawn.</summary>
    /// <param name="d">The d.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    protected static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Arc control)
        {
            return;
        }

        control.IsLargeArc = Math.Abs(control.EndAngle - control.StartAngle) > DegreesInHalfCircle;
        control.InvalidateVisual();
    }

    /// <summary>
    /// Get the geometry that defines this shape.
    /// <para><see href="https://stackoverflow.com/a/36756365/13224348">Based on Mark Feldman
    /// implementation.</see></para>
    /// </summary>
    /// <returns>A Geometry.</returns>
    protected Geometry DefinedGeometry()
    {
        var geometryStream = new StreamGeometry();
        var arcSize = new Size(
            Math.Max(0, (RenderSize.Width - StrokeThickness) / RadiusDivisor),
            Math.Max(0, (RenderSize.Height - StrokeThickness) / RadiusDivisor));

        using var context = geometryStream.Open();
        context.BeginFigure(PointAtAngle(Math.Min(StartAngle, EndAngle)), false, false);

        context.ArcTo(
            PointAtAngle(Math.Max(StartAngle, EndAngle)),
            arcSize,
            0,
            IsLargeArc,
            SweepDirection,
            true,
            false);

        geometryStream.Transform = new TranslateTransform(
            StrokeThickness / RadiusDivisor,
            StrokeThickness / RadiusDivisor);

        return geometryStream;
    }

    /// <summary>
    /// Draws a point on the coordinates of the given angle.
    /// <para><see href="https://stackoverflow.com/a/36756365/13224348">Based on Mark Feldman
    /// implementation.</see></para>
    /// </summary>
    /// <param name="angle">The angle at which to create the point.</param>
    /// <returns>A Point.</returns>
    protected Point PointAtAngle(double angle)
    {
        if (SweepDirection == SweepDirection.Counterclockwise)
        {
            angle += DegreesInRightAngle;
            angle %= DegreesInFullCircle;
            if (angle < 0)
            {
                angle += DegreesInFullCircle;
            }

            var radAngle = angle * (Math.PI / DegreesInHalfCircle);
            var horizontalRadius = (RenderSize.Width - StrokeThickness) / RadiusDivisor;
            var verticalRadius = (RenderSize.Height - StrokeThickness) / RadiusDivisor;

            return new Point(
                horizontalRadius + (horizontalRadius * Math.Cos(radAngle)),
                verticalRadius - (verticalRadius * Math.Sin(radAngle)));
        }

        angle -= DegreesInRightAngle;
        angle %= DegreesInFullCircle;
        if (angle < 0)
        {
            angle += DegreesInFullCircle;
        }

        var clockwiseRadAngle = angle * (Math.PI / DegreesInHalfCircle);
        var clockwiseHorizontalRadius = (RenderSize.Width - StrokeThickness) / RadiusDivisor;
        var clockwiseVerticalRadius = (RenderSize.Height - StrokeThickness) / RadiusDivisor;

        return new Point(
            clockwiseHorizontalRadius + (clockwiseHorizontalRadius * Math.Cos(-clockwiseRadAngle)),
            clockwiseVerticalRadius - (clockwiseVerticalRadius * Math.Sin(-clockwiseRadAngle)));
    }

    /// <summary>Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)" />, and returns a
    /// child at the specified index from a collection of child elements.</summary>
    /// <exception cref="System.ArgumentOutOfRangeException">index - Arc should have only 1 child.</exception>
    /// <param name="index">The zero-based index of the requested child element in the collection.</param>
    /// <returns>
    /// The requested child element. This should not return null; if the provided index is out of range, an exception is
    /// thrown.
    /// </returns>
    protected override Visual? GetVisualChild(int index)
    {
        if (index != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Arc should have only 1 child");
        }

        EnsureRootLayout();

        return _rootLayout;
    }

    /// <summary>When overridden in a derived class, measures the size in layout required for child elements and
    /// determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class.</summary>
    /// <param name="constraint">The available size that this element can give to child elements. Infinity can be
    /// specified as a value to indicate that the element will size to whatever content is available.</param>
    /// <returns>
    /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
        EnsureRootLayout();

        _rootLayout!.Measure(constraint);
        return _rootLayout.DesiredSize;
    }

    /// <summary>Arranges a <see cref="T:System.Windows.Shapes.Shape" /> by evaluating its <see
    /// cref="P:System.Windows.Shapes.Shape.RenderedGeometry" /> and <see cref="P:System.Windows.Shapes.Shape.Stretch"
    /// /> properties.</summary>
    /// <param name="finalSize">The final evaluated size of the <see cref="T:System.Windows.Shapes.Shape" />.</param>
    /// <returns>
    /// The final size of the arranged <see cref="T:System.Windows.Shapes.Shape" /> element.
    /// </returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        EnsureRootLayout();

        _rootLayout!.Arrange(new Rect(default, finalSize));
        return finalSize;
    }

    /// <summary>Overrides the default OnRender method to draw the <see cref="Arc" /> element.</summary>
    /// <param name="drawingContext">A <see cref="DrawingContext" /> object that is drawn during the rendering pass of
    /// this <see cref="System.Windows.Shapes.Shape" />.</param>
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (drawingContext is null)
        {
            throw new ArgumentNullException(nameof(drawingContext));
        }

        Pen pen = new(Stroke, StrokeThickness) { StartLineCap = StrokeStartLineCap, EndLineCap = StrokeStartLineCap };

        drawingContext.DrawGeometry(Stroke, pen, DefinedGeometry());
    }

    /// <summary>Provides the EnsureRootLayout member.</summary>
    private void EnsureRootLayout()
    {
        if (_rootLayout is not null)
        {
            return;
        }

        _rootLayout = new System.Windows.Controls.Viewbox { SnapsToDevicePixels = true };
        AddVisualChild(_rootLayout);
    }
}
