// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Represents a labeled, colored slice in a pie chart.</summary>
public class PieSlice : LabelStyleProperties, IHasLegendText, IHasLabel
{
    /// <summary>Initializes a new instance of the <see cref="PieSlice"/> class with default values.</summary>
    public PieSlice()
        : this(default, default, string.Empty)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PieSlice"/> class with a value and color.</summary>
    /// <param name="value">The slice value.</param>
    /// <param name="color">The slice fill color.</param>
    public PieSlice(double value, Color color)
        : this(value, color, string.Empty)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PieSlice"/> class with a value, color, and label.</summary>
    /// <param name="value">The slice value.</param>
    /// <param name="color">The slice fill color.</param>
    /// <param name="label">The slice label.</param>
    public PieSlice(double value, Color color, string label)
    {
        Value = value;
        Fill.Color = color;
        Label = label;
    }

    /// <summary>Gets or sets the slice label.</summary>
    public string Label { get => LabelStyle.Text; set => LabelStyle.Text = value; }

    /// <summary>Gets or sets the text shown in a legend.</summary>
    public string LegendText { get; set; } = string.Empty;

    /// <summary>Gets or sets the slice value.</summary>
    public double Value { get; set; }

    /// <summary>Gets or sets the slice fill style.</summary>
    public FillStyle Fill { get; set; } = new();

    /// <summary>Gets or sets the label style.</summary>
    public sealed override LabelStyle LabelStyle { get; set; } = new() { Alignment = Alignment.MiddleCenter };

    /// <summary>Gets or sets the slice fill color.</summary>
    public Color FillColor { get => Fill.Color; set => Fill.Color = value; }
}
