// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Represents a square-shaped 2D slider control for selecting X/Y values.
/// </summary>
public class SquareSlider : TemplatedControl
{
    /// <summary>
    /// Property for <see cref="ValueX"/>.
    /// </summary>
    public static readonly StyledProperty<double> ValueXProperty =
        AvaloniaProperty.Register<SquareSlider, double>(nameof(ValueX), 0.5);

    /// <summary>
    /// Property for <see cref="ValueY"/>.
    /// </summary>
    public static readonly StyledProperty<double> ValueYProperty =
        AvaloniaProperty.Register<SquareSlider, double>(nameof(ValueY), 0.5);

    /// <summary>
    /// Property for <see cref="MinimumX"/>.
    /// </summary>
    public static readonly StyledProperty<double> MinimumXProperty =
        AvaloniaProperty.Register<SquareSlider, double>(nameof(MinimumX), 0.0);

    /// <summary>
    /// Property for <see cref="MaximumX"/>.
    /// </summary>
    public static readonly StyledProperty<double> MaximumXProperty =
        AvaloniaProperty.Register<SquareSlider, double>(nameof(MaximumX), 1.0);

    /// <summary>
    /// Property for <see cref="MinimumY"/>.
    /// </summary>
    public static readonly StyledProperty<double> MinimumYProperty =
        AvaloniaProperty.Register<SquareSlider, double>(nameof(MinimumY), 0.0);

    /// <summary>
    /// Property for <see cref="MaximumY"/>.
    /// </summary>
    public static readonly StyledProperty<double> MaximumYProperty =
        AvaloniaProperty.Register<SquareSlider, double>(nameof(MaximumY), 1.0);

    /// <summary>
    /// Property for <see cref="ThumbSize"/>.
    /// </summary>
    public static readonly StyledProperty<double> ThumbSizeProperty =
        AvaloniaProperty.Register<SquareSlider, double>(nameof(ThumbSize), 16.0);

    /// <summary>
    /// Property for <see cref="ThumbBrush"/>.
    /// </summary>
    public static readonly StyledProperty<IBrush> ThumbBrushProperty =
        AvaloniaProperty.Register<SquareSlider, IBrush>(nameof(ThumbBrush), Brushes.White);

    private Canvas? _canvas;
    private global::Avalonia.Controls.Border? _thumb;
    private bool _isDragging;

    /// <summary>
    /// Gets or sets the X value (0.0 to 1.0 by default).
    /// </summary>
    public double ValueX
    {
        get => GetValue(ValueXProperty);
        set => SetValue(ValueXProperty, Math.Clamp(value, MinimumX, MaximumX));
    }

    /// <summary>
    /// Gets or sets the Y value (0.0 to 1.0 by default).
    /// </summary>
    public double ValueY
    {
        get => GetValue(ValueYProperty);
        set => SetValue(ValueYProperty, Math.Clamp(value, MinimumY, MaximumY));
    }

    /// <summary>
    /// Gets or sets the minimum X value.
    /// </summary>
    public double MinimumX
    {
        get => GetValue(MinimumXProperty);
        set => SetValue(MinimumXProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum X value.
    /// </summary>
    public double MaximumX
    {
        get => GetValue(MaximumXProperty);
        set => SetValue(MaximumXProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum Y value.
    /// </summary>
    public double MinimumY
    {
        get => GetValue(MinimumYProperty);
        set => SetValue(MinimumYProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum Y value.
    /// </summary>
    public double MaximumY
    {
        get => GetValue(MaximumYProperty);
        set => SetValue(MaximumYProperty, value);
    }

    /// <summary>
    /// Gets or sets the thumb size.
    /// </summary>
    public double ThumbSize
    {
        get => GetValue(ThumbSizeProperty);
        set => SetValue(ThumbSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the thumb brush.
    /// </summary>
    public IBrush ThumbBrush
    {
        get => GetValue(ThumbBrushProperty);
        set => SetValue(ThumbBrushProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnApplyTemplate(e);

        _canvas = e.NameScope.Find<Canvas>("PART_Canvas");
        _thumb = e.NameScope.Find<global::Avalonia.Controls.Border>("PART_Thumb");

        if (_canvas != null)
        {
            _canvas.PointerPressed += OnCanvasPointerPressed;
            _canvas.PointerMoved += OnCanvasPointerMoved;
            _canvas.PointerReleased += OnCanvasPointerReleased;
        }

        UpdateThumbPosition();
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ArgumentNullException.ThrowIfNull(change);

        base.OnPropertyChanged(change);

        if (change.Property == ValueXProperty || change.Property == ValueYProperty ||
            change.Property == BoundsProperty)
        {
            UpdateThumbPosition();
        }
    }

    private void OnCanvasPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_canvas == null)
        {
            return;
        }

        _isDragging = true;
        e.Pointer.Capture(_canvas);
        UpdateValueFromPointer(e.GetPosition(_canvas));
    }

    private void OnCanvasPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging || _canvas == null)
        {
            return;
        }

        UpdateValueFromPointer(e.GetPosition(_canvas));
    }

    private void OnCanvasPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        e.Pointer.Capture(null);
    }

    private void UpdateValueFromPointer(Point position)
    {
        if (_canvas == null || _canvas.Bounds.Width == 0 || _canvas.Bounds.Height == 0)
        {
            return;
        }

        var normalizedX = Math.Clamp(position.X / _canvas.Bounds.Width, 0, 1);
        var normalizedY = Math.Clamp(1 - (position.Y / _canvas.Bounds.Height), 0, 1);

        ValueX = MinimumX + (normalizedX * (MaximumX - MinimumX));
        ValueY = MinimumY + (normalizedY * (MaximumY - MinimumY));
    }

    private void UpdateThumbPosition()
    {
        if (_canvas == null || _thumb == null || _canvas.Bounds.Width == 0 || _canvas.Bounds.Height == 0)
        {
            return;
        }

        var rangeX = MaximumX - MinimumX;
        var rangeY = MaximumY - MinimumY;

        var normalizedX = rangeX > 0 ? (ValueX - MinimumX) / rangeX : 0;
        var normalizedY = rangeY > 0 ? (ValueY - MinimumY) / rangeY : 0;

        var thumbHalfSize = ThumbSize / 2;
        var x = (normalizedX * _canvas.Bounds.Width) - thumbHalfSize;
        var y = ((1 - normalizedY) * _canvas.Bounds.Height) - thumbHalfSize;

        Canvas.SetLeft(_thumb, x);
        Canvas.SetTop(_thumb, y);
    }
}
