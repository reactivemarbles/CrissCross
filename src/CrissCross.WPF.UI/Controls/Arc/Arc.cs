// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Control that draws a symmetrical arc with rounded edges.
/// </summary>
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
        new PropertyMetadata(0.0d, PropertyChangedCallback));

    /// <summary>Identifies the <see cref="EndAngle"/> dependency property.</summary>
    public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register(
        nameof(EndAngle),
        typeof(double),
        typeof(Arc),
        new PropertyMetadata(0.0d, PropertyChangedCallback));

    /// <summary>Identifies the <see cref="SweepDirection"/> dependency property.</summary>
    public static readonly DependencyProperty SweepDirectionProperty = DependencyProperty.Register(
        nameof(SweepDirection),
        typeof(SweepDirection),
        typeof(Arc),
        new PropertyMetadata(SweepDirection.Clockwise, PropertyChangedCallback));

    private System.Windows.Controls.Viewbox? _rootLayout;

    /// <summary>
    /// Initializes static members of the <see cref="Arc"/> class.
    /// Modify the metadata of the StrokeStartLineCap dependency property.
    /// </summary>
    static Arc() => StrokeStartLineCapProperty.OverrideMetadata(
            typeof(Arc),
            new FrameworkPropertyMetadata(PenLineCap.Round, PropertyChangedCallback));

    /// <summary>
    /// Gets or sets the initial angle from which the arc will be drawn.
    /// </summary>
    public double StartAngle
    {
        get => (double)GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }

    /// <summary>
    /// Gets or sets the final angle from which the arc will be drawn.
    /// </summary>
    public double EndAngle
    {
        get => (double)GetValue(EndAngleProperty);
        set => SetValue(EndAngleProperty, value);
    }

    /// <summary>
    /// Gets or sets the direction to where the arc will be drawn.
    /// </summary>
    public SweepDirection SweepDirection
    {
        get => (SweepDirection)GetValue(SweepDirectionProperty);
        set => SetValue(SweepDirectionProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether one of the two larger arc sweeps is chosen; otherwise, if is <see langword="false"/>, one of the smaller arc sweeps is chosen.
    /// </summary>
    public bool IsLargeArc { get; internal set; }

    /// <inheritdoc />
    protected override Geometry DefiningGeometry => DefinedGeometry();

    /// <summary>
    /// Event triggered when one of the key parameters is changed. Forces the geometry to be redrawn.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    protected static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Arc control)
        {
            return;
        }

        control.IsLargeArc = Math.Abs(control.EndAngle - control.StartAngle) > 180;
        control.InvalidateVisual();
    }

    /// <summary>
    /// Get the geometry that defines this shape.
    /// <para><see href="https://stackoverflow.com/a/36756365/13224348">Based on Mark Feldman implementation.</see></para>
    /// </summary>
    /// <returns>A Geometry.</returns>
    protected Geometry DefinedGeometry()
    {
        var geometryStream = new StreamGeometry();
        var arcSize = new Size(
            Math.Max(0, (RenderSize.Width - StrokeThickness) / 2),
            Math.Max(0, (RenderSize.Height - StrokeThickness) / 2));

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

        geometryStream.Transform = new TranslateTransform(StrokeThickness / 2, StrokeThickness / 2);

        return geometryStream;
    }

    /// <summary>
    /// Draws a point on the coordinates of the given angle.
    /// <para><see href="https://stackoverflow.com/a/36756365/13224348">Based on Mark Feldman implementation.</see></para>
    /// </summary>
    /// <param name="angle">The angle at which to create the point.</param>
    /// <returns>A Point.</returns>
    protected Point PointAtAngle(double angle)
    {
        if (SweepDirection == SweepDirection.Counterclockwise)
        {
            angle += 90;
            angle %= 360;
            if (angle < 0)
            {
                angle += 360;
            }

            var radAngle = angle * (Math.PI / 180);
            var xRadius = (RenderSize.Width - StrokeThickness) / 2;
            var yRadius = (RenderSize.Height - StrokeThickness) / 2;

            return new Point(
                xRadius + (xRadius * Math.Cos(radAngle)),
                yRadius - (yRadius * Math.Sin(radAngle)));
        }
        else
        {
            angle -= 90;
            angle %= 360;
            if (angle < 0)
            {
                angle += 360;
            }

            var radAngle = angle * (Math.PI / 180);
            var xRadius = (RenderSize.Width - StrokeThickness) / 2;
            var yRadius = (RenderSize.Height - StrokeThickness) / 2;

            return new Point(
                xRadius + (xRadius * Math.Cos(-radAngle)),
                yRadius - (yRadius * Math.Sin(-radAngle)));
        }
    }

    /// <summary>
    /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)" />, and returns a child at the specified index from a collection of child elements.
    /// </summary>
    /// <param name="index">The zero-based index of the requested child element in the collection.</param>
    /// <returns>
    /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">index - Arc should have only 1 child.</exception>
    protected override Visual? GetVisualChild(int index)
    {
        if (index != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Arc should have only 1 child");
        }

        EnsureRootLayout();

        return _rootLayout;
    }

    /// <summary>
    /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class.
    /// </summary>
    /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
    /// <returns>
    /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
    /// </returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        EnsureRootLayout();

        _rootLayout!.Measure(availableSize);
        return _rootLayout.DesiredSize;
    }

    /// <summary>
    /// Arranges a <see cref="T:System.Windows.Shapes.Shape" /> by evaluating its <see cref="P:System.Windows.Shapes.Shape.RenderedGeometry" /> and <see cref="P:System.Windows.Shapes.Shape.Stretch" /> properties.
    /// </summary>
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
    /// <param name="drawingContext">A <see cref="DrawingContext" /> object that is drawn during the rendering pass of this <see cref="System.Windows.Shapes.Shape" />.</param>
    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (drawingContext == null)
        {
            throw new ArgumentNullException(nameof(drawingContext));
        }

        Pen pen =
            new(Stroke, StrokeThickness)
            {
                StartLineCap = StrokeStartLineCap,
                EndLineCap = StrokeStartLineCap
            };

        drawingContext.DrawGeometry(Stroke, pen, DefinedGeometry());
    }

    private void EnsureRootLayout()
    {
        if (_rootLayout != null)
        {
            return;
        }

        _rootLayout = new System.Windows.Controls.Viewbox { SnapsToDevicePixels = true };
        AddVisualChild(_rootLayout);
    }
}
