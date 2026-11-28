// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CrissCross.Plot;

/// <summary>Represents a single bar in a bar chart.</summary>
public class Bar : IHasFill, IHasLine
{
    /// <summary>
    /// Labels for bars less than this value will be displayed beneath the bar.
    /// Set this value to negative infinity to force labels to be always on top of the bar.
    /// </summary>
    [SuppressMessage("Style", "SST1401", Justification = "Retained as a public field for binary and source compatibility with the existing API.")]
    public double LabelInvertWhenValueBelow;

    /// <summary>Divisor used when calculating a midpoint.</summary>
    private const float HalfDivisor = 2F;

    /// <summary>
    /// Gets or sets the position (not the value) of the bar.
    /// Increment the position of successive bars so they do not overlap.
    /// For simple bar plots, the positions of successive bars are 1, 2, 3, etc. on the horizontal axis.
    /// </summary>
    public double Position { get; set; }

    /// <summary>Gets or sets the value this bar represents, which is the top of the bar for positive values.</summary>
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the position where the bar begins.
    /// This is the bottom of the bar for positive values.
    /// Typically this is zero, so bars extend from zero toward their value.
    /// </summary>
    public double ValueBase { get; set; }

    /// <summary>
    /// Gets or sets the thickness of the bar in the same units as <see cref="Position"/>.
    /// Typically the size of each bar is equal to or less than the difference in <see cref="Position"/> between adjacent bars.
    /// </summary>
    public double Size { get; set; } = 0.8;

    /// <summary>Gets or sets the size of the error bar extending from <see cref="Value"/>.</summary>
    public double Error { get; set; }

    /// <summary>Gets or sets the width of the error bar whiskers in axis units (the same units as <see cref="Position"/>).</summary>
    public double ErrorSize { get; set; } = 0.2;

    /// <summary>Gets or sets a value indicating whether this bar is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the fill style of the bar.</summary>
    public FillStyle FillStyle { get; set; } = new() { Color = Colors.Gray, IsVisible = true };

    /// <summary>Gets or sets the hatch color of the bar fill.</summary>
    public Color FillHatchColor { get => FillStyle.HatchColor; set => FillStyle.HatchColor = value; }

    /// <summary>Gets or sets the hatch pattern of the bar fill.</summary>
    public IHatch? FillHatch { get => FillStyle.Hatch; set => FillStyle.Hatch = value; }

    /// <summary>Gets or sets the color of the bar fill.</summary>
    public Color FillColor { get => FillStyle.Color; set => FillStyle.Color = value; }

    /// <summary>Gets or sets the line style of the bar outline.</summary>
    public LineStyle LineStyle { get; set; } = new() { Color = Colors.Black, Width = 1, IsVisible = true };

    /// <summary>Gets or sets the width of the bar outline.</summary>
    public float LineWidth { get => LineStyle.Width; set => LineStyle.Width = value; }

    /// <summary>Gets or sets the pattern of the bar outline.</summary>
    public LinePattern LinePattern { get => LineStyle.Pattern; set => LineStyle.Pattern = value; }

    /// <summary>Gets or sets the color of the bar outline.</summary>
    public Color LineColor { get => LineStyle.Color; set => LineStyle.Color = value; }

    /// <summary>Gets or sets the obsolete border color; use <see cref="LineColor"/> instead.</summary>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained as an obsolete public member for binary and source compatibility with the existing API.")]
    [Obsolete("use LineColor", true)]
    public Color BorderColor { get; set; }

    /// <summary>Gets or sets the obsolete border line width; use <see cref="LineWidth"/> instead.</summary>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained as an obsolete public member for binary and source compatibility with the existing API.")]
    [Obsolete("use LineWidth", true)]
    public float BorderLineWidth { get; set; } = 1;

    /// <summary>Gets or sets the obsolete error line width; use <see cref="LineWidth"/> instead.</summary>
    [SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Retained as an obsolete public member for binary and source compatibility with the existing API.")]
    [Obsolete("use LineWidth", true)]
    public float ErrorLineWidth { get; set; }

    // TODO: something like ErrorInDirectionOfValue?
    // Maybe ErrorPosition should be an enum containing: None, Upward, Downward, Both, or Extend.
    /// <summary>Gets or sets whether to draw the positive-direction error portion.</summary>
    public bool ErrorPositive { get; set; } = true;

    /// <summary>Gets or sets whether to draw the negative-direction error portion.</summary>
    public bool ErrorNegative { get; set; } = true;

    /// <summary>Gets or sets the text to display on, above, or below the bar.</summary>
    /// <remarks>Typically this is the string representation of the value of the bar.</remarks>
    public string Label { get; set; } = string.Empty;

    /// <summary>Gets or sets whether labels are rendered last, after all other plottables.</summary>
    public bool LabelOnTop { get; set; }

    /// <summary>Gets or sets the value label text.</summary>
    /// <remarks>Typically this is the string representation of the value of the bar.</remarks>
    public string ValueLabel { get => Label; set => Label = value; }

    /// <summary>Gets or sets whether the value label is rendered in the center of the bar.</summary>
    public bool CenterLabel { get; set; }

    /// <summary>Gets or sets the distance in pixels between the value label and the edge of the bar.</summary>
    public float LabelOffset { get; set; } = 5;

    /// <summary>Gets or sets the orientation of the bar.</summary>
    public Orientation Orientation { get; set; } = Orientation.Vertical;

    /// <summary>Gets the coordinate rectangle occupied by the bar.</summary>
    public CoordinateRect Rect =>
        Orientation == Orientation.Vertical
            ? new CoordinateRect(
                left: Position - (Size / HalfDivisor),
                right: Position + (Size / HalfDivisor),
                bottom: ValueBase,
                top: Value)
            : new CoordinateRect(
                left: ValueBase,
                right: Value,
                bottom: Position - (Size / HalfDivisor),
                top: Position + (Size / HalfDivisor));

    /// <summary>Gets the error bar lines.</summary>
    public IEnumerable<CoordinateLine> ErrorLines
    {
        get
        {
            CoordinateLine center;
            CoordinateLine top;
            CoordinateLine bottom;
            if (Orientation == Orientation.Vertical)
            {
                center = new(Position, Value - Error, Position, Value + Error);
                top = new(Position - (ErrorSize / HalfDivisor), Value + Error, Position + (ErrorSize / HalfDivisor), Value + Error);
                bottom = new(Position - (ErrorSize / HalfDivisor), Value - Error, Position + (ErrorSize / HalfDivisor), Value - Error);
            }
            else
            {
                center = new(Value - Error, Position, Value + Error, Position);
                top = new(Value + Error, Position - (ErrorSize / HalfDivisor), Value + Error, Position + (ErrorSize / HalfDivisor));
                bottom = new(Value - Error, Position - (ErrorSize / HalfDivisor), Value - Error, Position + (ErrorSize / HalfDivisor));
            }

            return [center, top, bottom];
        }
    }

    /// <summary>Gets the axis limits occupied by the bar, including its error bar.</summary>
    public AxisLimits AxisLimits =>
        Orientation == Orientation.Vertical
            ? new AxisLimits(
                left: Position - (Size / HalfDivisor),
                right: Position + (Size / HalfDivisor),
                bottom: Math.Min(ValueBase, Value - Error),
                top: Value + Error)
            : new AxisLimits(
                left: Math.Min(ValueBase, Value - Error),
                right: Value + Error,
                bottom: Position - (Size / HalfDivisor),
                top: Position + (Size / HalfDivisor));

    /// <summary>Renders the bar and its label.</summary>
    /// <param name="rp">The render pack to use.</param>
    /// <param name="axes">The axes used to transform coordinates.</param>
    /// <param name="labelStyle">The style used to render the label.</param>
    public void Render(RenderPack rp, IAxes axes, LabelStyle labelStyle)
    {
        RenderBody(rp, axes);
        RenderText(rp, axes, labelStyle);
    }

    /// <summary>Renders the bar body and error bars.</summary>
    /// <param name="rp">The render pack to use.</param>
    /// <param name="axes">The axes used to transform coordinates.</param>
    public void RenderBody(RenderPack rp, IAxes axes)
    {
        PixelRect rect = axes.GetPixelRect(Rect);

        Drawing.FillRectangle(rp.Canvas, rect, rp.Paint, FillStyle);
        Drawing.DrawRectangle(rp.Canvas, rect, rp.Paint, LineStyle);

        if (Error == 0)
        {
            return;
        }

        foreach (CoordinateLine line in ErrorLines)
        {
            Pixel pt1 = axes.GetPixel(line.Start);
            Pixel pt2 = axes.GetPixel(line.End);
            PixelLine pixelLine = new(pt1, pt2);
            Drawing.DrawLine(rp.Canvas, rp.Paint, pixelLine, LineStyle);
        }
    }

    /// <summary>Renders the bar label.</summary>
    /// <param name="rp">The render pack to use.</param>
    /// <param name="axes">The axes used to transform coordinates.</param>
    /// <param name="labelStyle">The style used to render the label.</param>
    public void RenderText(RenderPack rp, IAxes axes, LabelStyle labelStyle)
    {
        PixelRect rect = axes.GetPixelRect(Rect);
        Paint paint = rp.Paint;

        if (CenterLabel)
        {
            labelStyle.Alignment = Alignment.MiddleCenter;
            labelStyle.Render(rp.Canvas, rect.Center, paint);
            return;
        }

        bool labelAboveBarTop = Value >= LabelInvertWhenValueBelow;

        if (Orientation == Orientation.Vertical)
        {
            float horizontalPosition = rect.HorizontalCenter;
            float verticalPosition = labelAboveBarTop ? rect.Top : rect.Bottom;
            labelStyle.Alignment = labelAboveBarTop ? Alignment.LowerCenter : Alignment.UpperCenter;
            Pixel labelPixel = labelAboveBarTop ? new(horizontalPosition, verticalPosition - LabelOffset) : new(horizontalPosition, verticalPosition + LabelOffset);
            labelStyle.Render(rp.Canvas, labelPixel, paint);
        }
        else
        {
            MeasuredText measured = labelStyle.Measure(labelStyle.Text, paint);

            if (Value < 0)
            {
                float horizontalPosition = rect.LeftCenter.X - (LabelOffset + (measured.Width / HalfDivisor));
                float verticalPosition = rect.LeftCenter.Y + (measured.Height / HalfDivisor);
                Pixel labelPixel = new(horizontalPosition, verticalPosition);
                labelStyle.Render(rp.Canvas, labelPixel, paint);
            }
            else
            {
                float horizontalPosition = rect.RightCenter.X + (LabelOffset + (measured.Width / HalfDivisor));
                float verticalPosition = rect.RightCenter.Y + (measured.Height / HalfDivisor);
                Pixel labelPixel = new(horizontalPosition, verticalPosition);
                labelStyle.Render(rp.Canvas, labelPixel, paint);
            }
        }
    }
}
