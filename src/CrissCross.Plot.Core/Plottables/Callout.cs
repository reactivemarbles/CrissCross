// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Displays a text label connected to a coordinate by an arrow.</summary>
public class Callout : LabelStyleProperties, IPlottable, IHasArrow
{
    /// <summary>The padding applied to the label in pixels.</summary>
    private const float DefaultLabelPadding = 5;

    /// <summary>The divisor used to calculate half of a dimension.</summary>
    private const double HalfDivisor = 2;

    /// <summary>Gets the text plottable used to render the callout label.</summary>
    public Text LabelPlottable { get; } = new() { LabelPadding = DefaultLabelPadding };

    /// <summary>Gets the arrow plottable used to render the callout arrow.</summary>
    public Arrow ArrowPlottable { get; } = new();

    /// <summary>Gets or sets the label style used to render the callout text.</summary>
    public sealed override LabelStyle LabelStyle { get => LabelPlottable.LabelStyle; set => LabelPlottable.LabelStyle = value; }

    /// <summary>Gets or sets the callout text.</summary>
    public string Text { get => LabelStyle.Text; set => LabelStyle.Text = value; }

    /// <summary>Gets or sets the style of the callout arrow.</summary>
    public ArrowStyle ArrowStyle { get => ArrowPlottable.ArrowStyle; set => ArrowPlottable.ArrowStyle = value; }

    /// <summary>Gets or sets the arrow line color.</summary>
    public Color ArrowLineColor { get => ArrowStyle.LineStyle.Color; set => ArrowStyle.LineStyle.Color = value; }

    /// <summary>Gets or sets the arrow line width.</summary>
    public float ArrowLineWidth { get => ArrowStyle.LineStyle.Width; set => ArrowStyle.LineStyle.Width = value; }

    /// <summary>Gets or sets the arrow fill color.</summary>
    public Color ArrowFillColor { get => ArrowStyle.FillStyle.Color; set => ArrowStyle.FillStyle.Color = value; }

    /// <summary>Gets or sets the minimum arrow length.</summary>
    public float ArrowMinimumLength { get => ArrowStyle.MinimumLength; set => ArrowStyle.MinimumLength = value; }

    /// <summary>Gets or sets the maximum arrow length.</summary>
    public float ArrowMaximumLength { get => ArrowStyle.MaximumLength; set => ArrowStyle.MaximumLength = value; }

    /// <summary>Gets or sets the offset applied to the arrow.</summary>
    public float ArrowOffset { get => ArrowStyle.Offset; set => ArrowStyle.Offset = value; }

    /// <summary>Gets or sets the anchor point for the arrow.</summary>
    public ArrowAnchor ArrowAnchor { get => ArrowStyle.Anchor; set => ArrowStyle.Anchor = value; }

    /// <summary>Gets or sets the width of the arrow body.</summary>
    public float ArrowWidth { get => ArrowStyle.ArrowWidth; set => ArrowStyle.ArrowWidth = value; }

    /// <summary>Gets or sets the length of the arrowhead axis.</summary>
    public float ArrowheadAxisLength { get => ArrowStyle.ArrowheadAxisLength; set => ArrowStyle.ArrowheadAxisLength = value; }

    /// <summary>Gets or sets the arrowhead length.</summary>
    public float ArrowheadLength { get => ArrowStyle.ArrowheadLength; set => ArrowStyle.ArrowheadLength = value; }

    /// <summary>Gets or sets the arrowhead width.</summary>
    public float ArrowheadWidth { get => ArrowStyle.ArrowheadWidth; set => ArrowStyle.ArrowheadWidth = value; }

    /// <summary>Gets or sets the obsolete combined arrow color.</summary>
    [Obsolete("use ArrowLineColor and ArrowFillColor", true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310:Deprecated public API", Justification = "Retained as an obsolete error to preserve the established public API.")]
    public Color ArrowColor { get; set; }

    /// <summary>Gets or sets the callout text color.</summary>
    public Color TextColor
    {
        get => LabelPlottable.LabelFontColor;
        set => LabelPlottable.LabelFontColor = value;
    }

    /// <summary>Gets or sets whether the callout text is bold.</summary>
    public bool Bold
    {
        get => LabelPlottable.LabelBold;
        set => LabelPlottable.LabelBold = value;
    }

    /// <summary>Gets or sets the callout font size.</summary>
    public float FontSize
    {
        get => LabelPlottable.LabelFontSize;
        set => LabelPlottable.LabelFontSize = value;
    }

    /// <summary>Gets or sets the callout text border color.</summary>
    public Color TextBorderColor
    {
        get => LabelPlottable.LabelBorderColor;
        set => LabelPlottable.LabelBorderColor = value;
    }

    /// <summary>Gets or sets the callout text border width.</summary>
    public float TextBorderWidth
    {
        get => LabelPlottable.LabelBorderWidth;
        set => LabelPlottable.LabelBorderWidth = value;
    }

    /// <summary>Gets or sets the callout text background color.</summary>
    public Color TextBackgroundColor
    {
        get => LabelPlottable.LabelBackgroundColor;
        set => LabelPlottable.LabelBackgroundColor = value;
    }

    /// <summary>Gets or sets the data coordinate of the callout text.</summary>
    public Coordinates TextCoordinates { get; set; }

    /// <summary>Gets the pixel coordinate of the text from the last render.</summary>
    public Pixel TextPixel { get; private set; }

    /// <summary>Gets or sets the data coordinate indicated by the arrow tip.</summary>
    public Coordinates TipCoordinates { get; set; }

    /// <summary>Gets the pixel coordinate of the arrow tip from the last render.</summary>
    public Pixel TipPixel { get; private set; }

    /// <summary>Gets the pixel rectangle occupied by the text during the last render.</summary>
    public PixelRect LastRenderRect => LabelPlottable.LabelStyle.LastRenderPixelRect;

    /// <summary>Gets or sets whether the callout is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position and render the callout.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets the legend items associated with this callout.</summary>
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    /// <summary>Gets the axis limits occupied by the arrow tip and label.</summary>
    /// <returns>The limits required to display both the tip and text coordinates.</returns>
    public AxisLimits GetAxisLimits()
    {
        ExpandingAxisLimits expandingLimits = new();
        expandingLimits.Expand(TipCoordinates);
        expandingLimits.Expand(TextCoordinates);
        return expandingLimits.AxisLimits;
    }

    /// <summary>Calculates the label attachment point closest to the specified location.</summary>
    /// <param name="paint">The paint used to measure the label.</param>
    /// <returns>The coordinates of the attachment point on the label.</returns>
    public Coordinates CalculateClosestAttachPoint(Paint paint)
    {
        PixelSize labelSize = LabelStyle.Measure(LabelStyle.Text, paint).Size;

        Pixel[] attachPoints = [
            new(TextPixel.X - LabelStyle.PixelPadding.Left, TextPixel.Y + (labelSize.Height / HalfDivisor)), // west
            new(TextPixel.X + labelSize.Width + LabelStyle.PixelPadding.Right, TextPixel.Y + (labelSize.Height / HalfDivisor)), // east
            new(TextPixel.X + (labelSize.Width / HalfDivisor), TextPixel.Y - LabelStyle.PixelPadding.Top), // north
            new(TextPixel.X + (labelSize.Width / HalfDivisor), TextPixel.Y + labelSize.Height + LabelStyle.PixelPadding.Bottom), // south
            new(TextPixel.X - LabelStyle.PixelPadding.Left, TextPixel.Y - LabelStyle.PixelPadding.Top), // northWest
            new(TextPixel.X + labelSize.Width + LabelStyle.PixelPadding.Right, TextPixel.Y - LabelStyle.PixelPadding.Top), // northEast
            new(TextPixel.X - LabelStyle.PixelPadding.Left, TextPixel.Y + labelSize.Height + LabelStyle.PixelPadding.Bottom), // southWest
            new(TextPixel.X + labelSize.Width + LabelStyle.PixelPadding.Right, TextPixel.Y + labelSize.Height + LabelStyle.PixelPadding.Bottom) // southEast
        ];

        Pixel closestAttachPoint = attachPoints[0];
        float distanceMin = TipPixel.DistanceFrom(attachPoints[0]);

        for (int i = 1; i < attachPoints.Length; i++)
        {
            float distance = TipPixel.DistanceFrom(attachPoints[i]);
            if (distance < distanceMin)
            {
                closestAttachPoint = attachPoints[i];
                distanceMin = distance;
            }
        }

        return Axes.GetCoordinates(closestAttachPoint);
    }

    /// <summary>Renders the callout label and arrow.</summary>
    /// <param name="rp">The render pack containing the canvas and paint.</param>
    public virtual void Render(RenderPack rp)
    {
        TextPixel = Axes.GetPixel(TextCoordinates);

        TipPixel = Axes.GetPixel(TipCoordinates);

        LabelPlottable.LabelText = LabelStyle.Text;
        LabelPlottable.Axes = Axes;
        LabelPlottable.Location = TextCoordinates;
        LabelPlottable.Render(rp);

        ArrowPlottable.Axes = Axes;
        ArrowPlottable.Base = CalculateClosestAttachPoint(rp.Paint);
        ArrowPlottable.Tip = TipCoordinates;
        ArrowPlottable.Render(rp);
    }
}
