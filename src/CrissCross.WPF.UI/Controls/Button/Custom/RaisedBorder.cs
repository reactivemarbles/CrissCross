// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// AICSBorder.
/// </summary>
/// <seealso cref="System.Windows.Controls.ContentControl" />
public class RaisedBorder : ContentControl
{
    /// <summary>
    /// The corner radius1 property.
    /// </summary>
    public static readonly DependencyProperty CornerRadius1Property = DependencyProperty.Register("CornerRadius1", typeof(CornerRadius), typeof(RaisedBorder), new PropertyMetadata(new CornerRadius(10.0)));

    /// <summary>
    /// The corner radius2 property.
    /// </summary>
    public static readonly DependencyProperty CornerRadius2Property = DependencyProperty.Register("CornerRadius2", typeof(CornerRadius), typeof(RaisedBorder), new PropertyMetadata(new CornerRadius(20.0)));

    /// <summary>
    /// The glare brush property.
    /// </summary>
    public static readonly DependencyProperty GlareBrushProperty = DependencyProperty.Register("GlareBrush", typeof(Brush), typeof(RaisedBorder), new PropertyMetadata(null));

    /// <summary>
    /// The glare opacity mask property.
    /// </summary>
    public static readonly DependencyProperty GlareOpacityMaskProperty = DependencyProperty.Register("GlareOpacityMask", typeof(Brush), typeof(RaisedBorder), new PropertyMetadata(null));

    /// <summary>
    /// The minor border brush1 property.
    /// </summary>
    public static readonly DependencyProperty MinorBorderBrush1Property = DependencyProperty.Register("MinorBorderBrush1", typeof(Brush), typeof(RaisedBorder), new PropertyMetadata(null));

    /// <summary>
    /// The minor border thickness1 property.
    /// </summary>
    public static readonly DependencyProperty MinorBorderThickness1Property = DependencyProperty.Register("MinorBorderThickness1", typeof(Thickness), typeof(RaisedBorder), new PropertyMetadata(new Thickness(0.0)));

    /// <summary>
    /// Initializes a new instance of the <see cref="RaisedBorder"/> class.
    /// </summary>
    public RaisedBorder()
    {
        CreateControlTemplate();
        ClipToBounds = true;
    }

    /// <summary>
    /// Gets or sets the corner radius1.
    /// </summary>
    /// <value>
    /// The corner radius1.
    /// </value>
    public CornerRadius CornerRadius1
    {
        get => (CornerRadius)GetValue(CornerRadius1Property);
        set => SetValue(CornerRadius1Property, value);
    }

    /// <summary>
    /// Gets or sets the corner radius2.
    /// </summary>
    /// <value>
    /// The corner radius2.
    /// </value>
    public CornerRadius CornerRadius2
    {
        get => (CornerRadius)GetValue(CornerRadius2Property);
        set => SetValue(CornerRadius2Property, value);
    }

    /// <summary>
    /// Gets or sets the glare brush.
    /// </summary>
    /// <value>
    /// The glare brush.
    /// </value>
    public Brush GlareBrush
    {
        get => (Brush)GetValue(GlareBrushProperty);
        set => SetValue(GlareBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the glare opacity mask.
    /// </summary>
    /// <value>
    /// The glare opacity mask.
    /// </value>
    public Brush GlareOpacityMask
    {
        get => (Brush)GetValue(GlareOpacityMaskProperty);
        set => SetValue(GlareOpacityMaskProperty, value);
    }

    /// <summary>
    /// Gets or sets the minor border brush1.
    /// </summary>
    /// <value>
    /// The minor border brush1.
    /// </value>
    public Brush MinorBorderBrush1
    {
        get => (Brush)GetValue(MinorBorderBrush1Property);
        set => SetValue(MinorBorderBrush1Property, value);
    }

    /// <summary>
    /// Gets or sets the minor border thickness1.
    /// </summary>
    /// <value>
    /// The minor border thickness1.
    /// </value>
    public Thickness MinorBorderThickness1
    {
        get => (Thickness)GetValue(MinorBorderThickness1Property);
        set => SetValue(MinorBorderThickness1Property, value);
    }

    /// <summary>
    /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement" /> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)" />.
    /// </summary>
    /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        InvalidateVisual();
    }

    /// <summary>
    /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
    /// </summary>
    /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
    protected override void OnRender(DrawingContext drawingContext)
    {
        if (drawingContext == null)
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

        var pen = new Pen(BorderBrush, 0.5);
        var group = new GeometryGroup();
        group.Children.Add(geometry);
        group.Children.Add(geometry2);
        drawingContext.DrawGeometry(BorderBrush, pen, group);

        var pen2 = new Pen(MinorBorderBrush1, 0.5);
        group = new GeometryGroup();
        group.Children.Add(geometry2);
        group.Children.Add(geometry3);
        drawingContext.DrawGeometry(MinorBorderBrush1, pen2, group);

        var border = Template.FindName("PART_MainBorder", this) as Border;
        if (border != null)
        {
            border.Padding = new Thickness(
                borderThickness.Left + Padding.Left,
                borderThickness.Top + Padding.Top,
                borderThickness.Right + Padding.Right,
                borderThickness.Bottom + Padding.Bottom);
            border.Clip = geometry3;
        }

        base.OnRender(drawingContext);
    }

    /// <summary>
    /// Returns a value that indicates whether serialization processes should serialize the value for the provided dependency property.
    /// </summary>
    /// <param name="dp">The identifier for the dependency property that should be serialized.</param>
    /// <returns>
    /// true if the dependency property that is supplied should be value-serialized; otherwise, false.
    /// </returns>
    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        if (dp == StyleProperty)
        {
            return false;
        }

        return base.ShouldSerializeProperty(dp);
    }

    private static PathGeometry CreateGeometryForPath(double width, double height, CornerRadius cornerRadius) => CreateGeometryForPath(width, height, cornerRadius, new Thickness(0));

    private static PathGeometry CreateGeometryForPath(double actualWidth, double actualHeight, CornerRadius cornerRadius, Thickness borderThickness)
    {
        var geometry = new PathGeometry();
        var figure = new PathFigure();
        double x = 0;
        var num2 = actualWidth - borderThickness.Right - borderThickness.Left;
        double y = 0;
        var num4 = actualHeight - borderThickness.Bottom - borderThickness.Top;
        var num5 = num2 - x;
        var num6 = num4 - y;
        Point point;
        var num7 = cornerRadius.TopLeft + cornerRadius.TopRight;
        var point2 = new Point((cornerRadius.TopLeft / num7) * num5, y);
        if (double.IsNaN(point2.X))
        {
            point2.X = 0;
        }

        if (point2.X > cornerRadius.TopLeft)
        {
            point2 = new Point(cornerRadius.TopLeft, y);
        }

        var point3 = new Point(point2.X, y);
        point = new Point(point3.X + borderThickness.Left, point3.Y + borderThickness.Top);
        figure.StartPoint = point;
        point3 = new Point((cornerRadius.TopRight / num7) * num5, y);
        if (double.IsNaN(point3.X))
        {
            point3.X = 0;
        }

        point3.X = num5 - point3.X;
        if (point3.X <= x + num5 - cornerRadius.TopRight)
        {
            point3 = new Point(num5 - cornerRadius.TopRight, y);
            point = new Point(point3.X + borderThickness.Left, point3.Y + borderThickness.Top);
            var segment = new LineSegment(point, false);
            figure.Segments.Add(segment);
        }

        var size = default(Size);
        var num8 = cornerRadius.BottomRight + cornerRadius.TopRight;
        if (cornerRadius.TopRight != 0)
        {
            size.Width = Math.Min(cornerRadius.TopRight, Math.Max(0, num5 - point3.X));
            point3 = new Point(num2, (cornerRadius.TopRight / num8) * num6);
            if (double.IsNaN(point3.Y))
            {
                point3.Y = 0;
            }

            if (point3.Y > cornerRadius.TopRight)
            {
                point3 = new Point(num2, cornerRadius.TopRight);
                size.Height = cornerRadius.TopRight;
            }
            else
            {
                point3.Y = point3.Y;
                size.Height = Math.Max(0, point3.Y);
            }

            point = new Point(point3.X + borderThickness.Left, point3.Y + borderThickness.Top);
            var segment2 = new ArcSegment(point, size, 0, false, SweepDirection.Clockwise, false);
            figure.Segments.Add(segment2);
        }

        if (point3.Y < num6 - cornerRadius.BottomRight)
        {
            point3 = new Point(num2, num6 - cornerRadius.BottomRight);
            point = new Point(point3.X + borderThickness.Left, point3.Y + borderThickness.Top);
            var segment3 = new LineSegment(point, false);
            figure.Segments.Add(segment3);
        }

        var num9 = cornerRadius.BottomRight + cornerRadius.BottomLeft;
        if (cornerRadius.BottomRight != 0)
        {
            size.Height = Math.Min(cornerRadius.BottomRight, Math.Max(0, num6 - point3.Y));
            point3 = new Point((cornerRadius.BottomRight / num9) * num5, num4);
            if (double.IsNaN(point3.X))
            {
                point3.X = 0;
            }

            if (point3.X > cornerRadius.BottomRight)
            {
                point3 = new Point(num5 - cornerRadius.BottomRight, num4);
                size.Width = cornerRadius.BottomRight;
            }
            else
            {
                point3 = new Point(num5 - point3.X, num4);
                size.Width = Math.Max(0, num5 - point3.X);
            }

            point = new Point(point3.X + borderThickness.Left, point3.Y + borderThickness.Top);
            var segment4 = new ArcSegment(point, size, 0, false, SweepDirection.Clockwise, false);
            figure.Segments.Add(segment4);
        }

        if (point3.X > cornerRadius.BottomLeft)
        {
            point3 = new Point(x + cornerRadius.BottomLeft, num4);
            point = new Point(point3.X + borderThickness.Left, point3.Y + borderThickness.Top);
            var segment5 = new LineSegment(point, false);
            figure.Segments.Add(segment5);
        }

        var num10 = cornerRadius.BottomLeft + cornerRadius.TopLeft;
        if (cornerRadius.BottomLeft != 0)
        {
            size.Width = Math.Min(cornerRadius.BottomLeft, Math.Max(0, point3.X));
            point3 = new Point(x, (cornerRadius.BottomLeft / num10) * num6);
            if (double.IsNaN(point3.Y))
            {
                point3.Y = 0;
            }

            if (point3.Y > cornerRadius.BottomLeft)
            {
                point3 = new Point(x, num6 - cornerRadius.BottomLeft);
                size.Height = cornerRadius.BottomLeft;
            }
            else
            {
                point3 = new Point(x, num6 - point3.Y);
                size.Height = Math.Max(0, num6 - point3.Y);
            }

            point = new Point(point3.X + borderThickness.Left, point3.Y + borderThickness.Top);
            var segment6 = new ArcSegment(point, size, 0, false, SweepDirection.Clockwise, false);
            figure.Segments.Add(segment6);
        }

        if (point3.Y > cornerRadius.TopLeft)
        {
            point3 = new Point(x, y + cornerRadius.TopLeft);
            point = new Point(point3.X + borderThickness.Left, point3.Y + borderThickness.Top);
            var segment7 = new LineSegment(point, false);
            figure.Segments.Add(segment7);
        }

        if (cornerRadius.TopLeft != 0)
        {
            size.Height = Math.Min(cornerRadius.TopLeft, Math.Max(0, point3.Y));
            point3 = new Point((cornerRadius.TopLeft / num7) * num5, y);
            if (double.IsNaN(point3.X))
            {
                point3.X = 0;
            }

            if (point3.X > cornerRadius.TopLeft)
            {
                point3 = new Point(cornerRadius.TopLeft, y);
                size.Width = cornerRadius.TopLeft;
            }
            else
            {
                point3 = new Point(point3.X, y);
                size.Width = Math.Max(0, point3.X);
            }

            point = new Point(point3.X + borderThickness.Left, point3.Y + borderThickness.Top);
            var segment8 = new ArcSegment(point, size, 0, false, SweepDirection.Clockwise, false);
            figure.Segments.Add(segment8);
        }

        figure.IsClosed = true;
        geometry.Figures.Add(figure);
        return geometry;
    }

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
        extension = new TemplateBindingExtension(HorizontalContentAlignmentProperty);
        factory3.SetValue(HorizontalAlignmentProperty, extension);
        extension = new TemplateBindingExtension(VerticalContentAlignmentProperty);
        factory3.SetValue(VerticalAlignmentProperty, extension);
        extension = new TemplateBindingExtension(ContentProperty);
        factory3.SetValue(ContentPresenter.ContentProperty, extension);
        child.AppendChild(factory3);

        var factory4 = new FrameworkElementFactory(typeof(Border), "PART_GlareBorder");
        extension = new TemplateBindingExtension(GlareBrushProperty);
        factory4.SetValue(Border.BackgroundProperty, extension);
        extension = new TemplateBindingExtension(GlareOpacityMaskProperty);
        factory4.SetValue(OpacityMaskProperty, extension);
        factory4.SetValue(IsHitTestVisibleProperty, false);
        child.AppendChild(factory4);

        template.VisualTree = factory;
        Template = template;
    }
}
