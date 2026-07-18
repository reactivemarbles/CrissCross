// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents AICSBorder.</summary>
/// <seealso cref="System.Windows.Controls.ContentControl" />
public class RaisedBorder : ContentControl
{
    /// <summary>The corner radius1 property.</summary>
    public static readonly DependencyProperty CornerRadius1Property = DependencyProperty.Register(
        nameof(CornerRadius1),
        typeof(CornerRadius),
        typeof(RaisedBorder),
        new PropertyMetadata(new CornerRadius(10.0)));

    /// <summary>The corner radius2 property.</summary>
    public static readonly DependencyProperty CornerRadius2Property = DependencyProperty.Register(
        nameof(CornerRadius2),
        typeof(CornerRadius),
        typeof(RaisedBorder),
        new PropertyMetadata(new CornerRadius(20.0)));

    /// <summary>The glare brush property.</summary>
    public static readonly DependencyProperty GlareBrushProperty = DependencyProperty.Register(
        nameof(GlareBrush),
        typeof(Brush),
        typeof(RaisedBorder),
        new PropertyMetadata(null));

    /// <summary>The glare opacity mask property.</summary>
    public static readonly DependencyProperty GlareOpacityMaskProperty = DependencyProperty.Register(
        nameof(GlareOpacityMask),
        typeof(Brush),
        typeof(RaisedBorder),
        new PropertyMetadata(null));

    /// <summary>The minor border brush1 property.</summary>
    public static readonly DependencyProperty MinorBorderBrush1Property = DependencyProperty.Register(
        nameof(MinorBorderBrush1),
        typeof(Brush),
        typeof(RaisedBorder),
        new PropertyMetadata(null));

    /// <summary>The minor border thickness1 property.</summary>
    public static readonly DependencyProperty MinorBorderThickness1Property = DependencyProperty.Register(
        nameof(MinorBorderThickness1),
        typeof(Thickness),
        typeof(RaisedBorder),
        new PropertyMetadata(new Thickness(0.0)));

    /// <summary>The pen thickness used to draw raised border outlines.</summary>
    private const double BorderPenThickness = 0.5D;

    /// <summary>Initializes a new instance of the <see cref="RaisedBorder"/> class.</summary>
    public RaisedBorder()
    {
        CreateControlTemplate();
        ClipToBounds = true;
    }

    /// <summary>Gets or sets the corner radius1.</summary>
    /// <value>
    /// The corner radius1.
    /// </value>
    public CornerRadius CornerRadius1
    {
        get => (CornerRadius)GetValue(CornerRadius1Property);
        set => SetValue(CornerRadius1Property, value);
    }

    /// <summary>Gets or sets the corner radius2.</summary>
    /// <value>
    /// The corner radius2.
    /// </value>
    public CornerRadius CornerRadius2
    {
        get => (CornerRadius)GetValue(CornerRadius2Property);
        set => SetValue(CornerRadius2Property, value);
    }

    /// <summary>Gets or sets the glare brush.</summary>
    /// <value>
    /// The glare brush.
    /// </value>
    public Brush GlareBrush
    {
        get => (Brush)GetValue(GlareBrushProperty);
        set => SetValue(GlareBrushProperty, value);
    }

    /// <summary>Gets or sets the glare opacity mask.</summary>
    /// <value>
    /// The glare opacity mask.
    /// </value>
    public Brush GlareOpacityMask
    {
        get => (Brush)GetValue(GlareOpacityMaskProperty);
        set => SetValue(GlareOpacityMaskProperty, value);
    }

    /// <summary>Gets or sets the minor border brush1.</summary>
    /// <value>
    /// The minor border brush1.
    /// </value>
    public Brush MinorBorderBrush1
    {
        get => (Brush)GetValue(MinorBorderBrush1Property);
        set => SetValue(MinorBorderBrush1Property, value);
    }

    /// <summary>Gets or sets the minor border thickness1.</summary>
    /// <value>
    /// The minor border thickness1.
    /// </value>
    public Thickness MinorBorderThickness1
    {
        get => (Thickness)GetValue(MinorBorderThickness1Property);
        set => SetValue(MinorBorderThickness1Property, value);
    }

    /// <summary>Invoked whenever the effective value of any dependency property on this <see
    /// cref="T:System.Windows.FrameworkElement" /> has been updated. The specific dependency property that changed is
    /// reported in the arguments parameter. Overrides <see
    /// cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"
    /// />.</summary>
    /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        InvalidateVisual();
    }

    /// <summary>When overridden in a derived class, participates in rendering operations that are directed by the
    /// layout system. The rendering instructions for this element are not used directly when this method is invoked,
    /// and are instead preserved for later asynchronous use by layout and drawing.</summary>
    /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the
    /// layout system.</param>
    protected override void OnRender(DrawingContext drawingContext)
    {
        if (drawingContext is null)
        {
            throw new ArgumentNullException(nameof(drawingContext));
        }

        var borderThickness = new Thickness(
            Math.Max(0, BorderThickness.Left + MinorBorderThickness1.Left),
            Math.Max(0, BorderThickness.Top + MinorBorderThickness1.Top),
            Math.Max(0, BorderThickness.Right + MinorBorderThickness1.Right),
            Math.Max(0, BorderThickness.Bottom + MinorBorderThickness1.Bottom));

        var geometry = RaisedBorder.CreateGeometryForPath(ActualWidth, ActualHeight, CornerRadius1);
        var geometry2 = RaisedBorder.CreateGeometryForPath(ActualWidth, ActualHeight, CornerRadius2, BorderThickness);
        var geometry3 = RaisedBorder.CreateGeometryForPath(ActualWidth, ActualHeight, CornerRadius2, borderThickness);

        var pen = new Pen(BorderBrush, BorderPenThickness);
        var group = new GeometryGroup();
        group.Children.Add(geometry);
        group.Children.Add(geometry2);
        drawingContext.DrawGeometry(BorderBrush, pen, group);

        var pen2 = new Pen(MinorBorderBrush1, BorderPenThickness);
        group = new();
        group.Children.Add(geometry2);
        group.Children.Add(geometry3);
        drawingContext.DrawGeometry(MinorBorderBrush1, pen2, group);

        var border = Template.FindName("PART_MainBorder", this) as Border;
        if (border is not null)
        {
            border.Padding = new(
                borderThickness.Left + Padding.Left,
                borderThickness.Top + Padding.Top,
                borderThickness.Right + Padding.Right,
                borderThickness.Bottom + Padding.Bottom);
            border.Clip = geometry3;
        }

        base.OnRender(drawingContext);
    }

    /// <summary>Returns a value that indicates whether serialization processes should serialize the value for the
    /// provided dependency property.</summary>
    /// <param name="dp">The identifier for the dependency property that should be serialized.</param>
    /// <returns>
    /// true if the dependency property that is supplied should be value-serialized; otherwise, false.
    /// </returns>
    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        return dp == StyleProperty ? false : base.ShouldSerializeProperty(dp);
    }

    /// <summary>Provides the CreateGeometryForPath member.</summary>
    /// <param name="width">The width value.</param>
    /// <param name="height">The height value.</param>
    /// <param name="cornerRadius">The cornerRadius value.</param>
    /// <returns>The result.</returns>
    private static PathGeometry CreateGeometryForPath(double width, double height, CornerRadius cornerRadius) =>
        CreateGeometryForPath(width, height, cornerRadius, new Thickness(0));

    /// <summary>Provides the CreateGeometryForPath member.</summary>
    /// <param name="actualWidth">The actualWidth value.</param>
    /// <param name="actualHeight">The actualHeight value.</param>
    /// <param name="cornerRadius">The cornerRadius value.</param>
    /// <param name="borderThickness">The borderThickness value.</param>
    /// <returns>The result.</returns>
    private static PathGeometry CreateGeometryForPath(
        double actualWidth,
        double actualHeight,
        CornerRadius cornerRadius,
        Thickness borderThickness)
    {
        var bounds = new Rect(
            borderThickness.Left,
            borderThickness.Top,
            Math.Max(0, actualWidth - borderThickness.Left - borderThickness.Right),
            Math.Max(0, actualHeight - borderThickness.Top - borderThickness.Bottom));
        var radius = NormalizeCornerRadius(cornerRadius, bounds.Width, bounds.Height);
        var figure = new PathFigure { StartPoint = new(bounds.Left + radius.TopLeft, bounds.Top), IsClosed = true };

        AddLine(figure, new(bounds.Right - radius.TopRight, bounds.Top));
        AddArc(figure, new(bounds.Right, bounds.Top + radius.TopRight), radius.TopRight);
        AddLine(figure, new(bounds.Right, bounds.Bottom - radius.BottomRight));
        AddArc(figure, new(bounds.Right - radius.BottomRight, bounds.Bottom), radius.BottomRight);
        AddLine(figure, new(bounds.Left + radius.BottomLeft, bounds.Bottom));
        AddArc(figure, new(bounds.Left, bounds.Bottom - radius.BottomLeft), radius.BottomLeft);
        AddLine(figure, new(bounds.Left, bounds.Top + radius.TopLeft));
        AddArc(figure, new(bounds.Left + radius.TopLeft, bounds.Top), radius.TopLeft);

        var geometry = new PathGeometry();
        geometry.Figures.Add(figure);
        return geometry;
    }

    /// <summary>Adds an arc segment when radius is greater than zero.</summary>
    /// <param name="figure">The path figure.</param>
    /// <param name="point">The end point.</param>
    /// <param name="radius">The radius.</param>
    private static void AddArc(PathFigure figure, Point point, double radius)
    {
        if (radius <= 0)
        {
            AddLine(figure, point);
            return;
        }

        figure.Segments.Add(new ArcSegment(point, new(radius, radius), 0, false, SweepDirection.Clockwise, false));
    }

    /// <summary>Adds a line segment.</summary>
    /// <param name="figure">The path figure.</param>
    /// <param name="point">The end point.</param>
    private static void AddLine(PathFigure figure, Point point) => figure.Segments.Add(new LineSegment(point, false));

    /// <summary>Calculates a scale factor for adjacent corner radii.</summary>
    /// <param name="length">The available length.</param>
    /// <param name="radiusSum">The combined radii.</param>
    /// <returns>The scale factor.</returns>
    private static double GetRadiusScale(double length, double radiusSum) =>
        radiusSum > 0 && radiusSum > length ? length / radiusSum : 1;

    /// <summary>Normalizes corner radii so adjacent corners fit the bounds.</summary>
    /// <param name="cornerRadius">The requested corner radii.</param>
    /// <param name="width">The available width.</param>
    /// <param name="height">The available height.</param>
    /// <returns>The normalized corner radii.</returns>
    private static CornerRadius NormalizeCornerRadius(CornerRadius cornerRadius, double width, double height)
    {
        var scale = Math.Min(
            Math.Min(
                GetRadiusScale(width, cornerRadius.TopLeft + cornerRadius.TopRight),
                GetRadiusScale(width, cornerRadius.BottomLeft + cornerRadius.BottomRight)),
            Math.Min(
                GetRadiusScale(height, cornerRadius.TopLeft + cornerRadius.BottomLeft),
                GetRadiusScale(height, cornerRadius.TopRight + cornerRadius.BottomRight)));

        return new(
            Math.Max(0, cornerRadius.TopLeft * scale),
            Math.Max(0, cornerRadius.TopRight * scale),
            Math.Max(0, cornerRadius.BottomRight * scale),
            Math.Max(0, cornerRadius.BottomLeft * scale));
    }

    /// <summary>Provides the CreateControlTemplate member.</summary>
    private void CreateControlTemplate()
    {
        var template = new ControlTemplate(GetType());
        var factory = new FrameworkElementFactory(typeof(Border), "PART_MainBorder");
        var extension = new TemplateBindingExtension(BackgroundProperty);
        factory.SetValue(Border.BackgroundProperty, extension);
        factory.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        factory.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);
        factory.SetValue(IsHitTestVisibleProperty, true);

        var child = new FrameworkElementFactory(typeof(Grid));
        child.SetValue(MarginProperty, new Thickness(-1));
        child.SetValue(IsHitTestVisibleProperty, true);
        factory.AppendChild(child);

        var factory3 = new FrameworkElementFactory(typeof(ContentPresenter));
        extension = new(HorizontalContentAlignmentProperty);
        factory3.SetValue(HorizontalAlignmentProperty, extension);
        extension = new(VerticalContentAlignmentProperty);
        factory3.SetValue(VerticalAlignmentProperty, extension);
        extension = new(ContentProperty);
        factory3.SetValue(ContentPresenter.ContentProperty, extension);
        child.AppendChild(factory3);

        var factory4 = new FrameworkElementFactory(typeof(Border), "PART_GlareBorder");
        extension = new(GlareBrushProperty);
        factory4.SetValue(Border.BackgroundProperty, extension);
        extension = new(GlareOpacityMaskProperty);
        factory4.SetValue(OpacityMaskProperty, extension);
        factory4.SetValue(IsHitTestVisibleProperty, false);
        child.AppendChild(factory4);

        template.VisualTree = factory;
        Template = template;
    }
}
