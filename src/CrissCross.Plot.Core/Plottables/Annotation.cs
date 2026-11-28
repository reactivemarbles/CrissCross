// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Renders text at a position within the plot data area.</summary>
public class Annotation : LabelStyleProperties, IPlottable, IHasLabel
{
    /// <summary>The default alpha of the annotation shadow.</summary>
    private const float DefaultShadowAlpha = 0.2F;

    /// <summary>The default pixel offset applied to the annotation.</summary>
    private const float DefaultPixelOffset = 10;

    /// <summary>Gets or sets whether the annotation is rendered.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used by this plottable.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items for this annotation. Annotations do not produce legend items.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets or sets the style used to render the annotation label.</summary>
    public sealed override LabelStyle LabelStyle { get; set; } = new() { ShadowColor = Colors.Black.WithAlpha(DefaultShadowAlpha) };

    /// <summary>Gets or sets the label style.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST2310",
        Justification = "The obsolete public property is retained for compatibility with existing annotation consumers.")]
    [Obsolete("Interact properties in this class (e.g., LabelFontColor) or properties of LabelStyle")]
    public LabelStyle Label { get => LabelStyle; set => LabelStyle = value; }

    /// <summary>Gets or sets the annotation text.</summary>
    public string Text { get => LabelText; set => LabelText = value; }

    /// <summary>Gets or sets the annotation's alignment within the data area.</summary>
    public Alignment Alignment { get; set; } = Alignment.UpperLeft;

    /// <summary>Gets or sets the annotation's pixel offset from its aligned position.</summary>
    public PixelOffset PixelOffset { get; set; } = new(DefaultPixelOffset, DefaultPixelOffset);

    /// <summary>Gets or sets the horizontal pixel offset.</summary>
    public float OffsetX
    {
        get => PixelOffset.X;
        set => PixelOffset = PixelOffset.WithX(value);
    }

    /// <summary>Gets or sets the vertical pixel offset.</summary>
    public float OffsetY
    {
        get => PixelOffset.Y;
        set => PixelOffset = PixelOffset.WithY(value);
    }

    /// <summary>Gets or sets the fractional area of the data rectangle used to position the annotation.</summary>
    public FractionRect FractionRect { get; set; } = FractionRect.Full;

    /// <summary>Gets the axis limits required by this annotation. Annotations do not impose limits.</summary>
    /// <returns>No axis limits.</returns>
    public AxisLimits GetAxisLimits() => AxisLimits.NoLimits;

    /// <summary>Renders the annotation when it is visible.</summary>
    /// <param name="rp">The render context.</param>
    public virtual void Render(RenderPack rp)
    {
        if (!IsVisible)
        {
            return;
        }

        PixelRect rect = rp.DataRect.Fraction(FractionRect);
        Pixel px = LabelStyle.GetRenderLocation(rect, Alignment, OffsetX, OffsetY, rp.Paint);
        LabelStyle.Render(rp.Canvas, px, rp.Paint);
    }
}
