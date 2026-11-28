// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Controls the appearance and rendering of plot grid lines.</summary>
public class GridStyle
{
    /// <summary>The opacity of major grid lines by default.</summary>
    private const double MajorLineOpacity = 0.1;

    /// <summary>The opacity of minor grid lines by default.</summary>
    private const double MinorLineOpacity = 0.05;

    /// <summary>The number of major tick positions used to determine alternating fills.</summary>
    private const int MaximumMajorTicksForFill = 2;

    /// <summary>The number of boundary positions surrounding the grid ticks.</summary>
    private const int BoundaryPositionCount = 2;

    /// <summary>Gets or sets a value indicating whether grid lines are rendered.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the style of major grid lines.</summary>
    public LineStyle MajorLineStyle { get; set; } = new() { Width = 1, Color = Colors.Black.WithOpacity(MajorLineOpacity), AntiAlias = false, };

    /// <summary>Gets or sets the style of minor grid lines.</summary>
    public LineStyle MinorLineStyle { get; set; } = new() { Width = 0, IsVisible = true, Color = Colors.Black.WithOpacity(MinorLineOpacity), AntiAlias = false, };

    /// <summary>Gets or sets the maximum number of major or minor grid lines rendered.</summary>
    public int MaximumNumberOfGridLines { get; set; } = 1_000;

    /// <summary>Gets or sets a value indicating whether the grid is rendered beneath plottables.</summary>
    public bool IsBeneathPlottables { get; set; } = true;

    /// <summary>Gets or sets the first alternating fill color between major grid lines.</summary>
    public Color FillColor1 { get; set; } = Colors.Transparent;

    /// <summary>Gets or sets the second alternating fill color between major grid lines.</summary>
    public Color FillColor2 { get; set; } = Colors.Transparent;

    /// <summary>Gets a value indicating whether either alternating fill color is enabled.</summary>
    private bool FillColorEnabled => FillColor1 != Colors.Transparent || FillColor2 != Colors.Transparent;

    /// <summary>Renders minor and major grid lines and the alternating fill regions.</summary>
    /// <param name="rp">The render pack containing the canvas and plot rectangle.</param>
    /// <param name="axis">The axis used to convert tick positions to pixels.</param>
    /// <param name="ticks">The ticks used to place the grid lines.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Style",
        "SST1473",
        Justification = "Exact equality detects duplicate tick positions and prevents division by zero; a tolerance would alter fill parity for distinct nearby ticks.")]
    public void Render(RenderPack rp, IAxis axis, IEnumerable<Tick> ticks)
    {
        if (!IsVisible)
        {
            return;
        }

        int maximumGridLines = Math.Max(0, MaximumNumberOfGridLines);
        List<Tick> tickValues = maximumGridLines > 0 ? new(ticks) : [];
        int maximumMajorTicks = Math.Min(MaximumMajorTicksForFill, maximumGridLines);
        List<float> minorTickPixels = [];
        List<float> majorTickPixels = [];
        List<Tick> majorTicks = [];

        foreach (Tick tick in tickValues)
        {
            if (tick.IsMajor)
            {
                if (majorTickPixels.Count < maximumGridLines)
                {
                    majorTickPixels.Add(axis.GetPixel(tick.Position, rp.DataRect));
                }

                if (majorTicks.Count < maximumMajorTicks)
                {
                    majorTicks.Add(tick);
                }
            }
            else if (minorTickPixels.Count < maximumGridLines)
            {
                minorTickPixels.Add(axis.GetPixel(tick.Position, rp.DataRect));
            }
        }

        int oddTick = 0; // 0 for even, 1 for odd.
        if (majorTicks.Count >= MaximumMajorTicksForFill && majorTicks[1].Position != majorTicks[0].Position)
        {
            oddTick = (int)(majorTicks[0].Position / (majorTicks[1].Position - majorTicks[0].Position)) % MaximumMajorTicksForFill;
        }

        float[] minorPixels = minorTickPixels.ToArray();
        float[] majorPixels = majorTickPixels.ToArray();
        RenderGridLines(rp, minorPixels, axis.Edge, MinorLineStyle);
        RenderAlternatingFill(rp, majorPixels, axis.Edge, oddTick);
        RenderGridLines(rp, majorPixels, axis.Edge, MajorLineStyle);
    }

    /// <summary>Gets the pixel coordinate used for a boundary or tick position.</summary>
    /// <param name="index">The position index, including the two outer boundaries.</param>
    /// <param name="positions">The tick positions between the outer boundaries.</param>
    /// <param name="edge">The edge that determines the grid orientation.</param>
    /// <param name="dataRect">The plot rectangle.</param>
    /// <returns>The pixel coordinate at the specified index.</returns>
    private static float GetFillPosition(int index, float[] positions, Edge edge, PixelRect dataRect)
    {
        if (index == 0)
        {
            return edge.IsHorizontal() ? dataRect.Left : dataRect.Bottom;
        }

        if (index == positions.Length + 1)
        {
            return edge.IsHorizontal() ? dataRect.Right : dataRect.Top;
        }

        return positions[index - 1];
    }

    /// <summary>Gets the start and end pixels for a grid position.</summary>
    /// <param name="position">The grid position in pixels.</param>
    /// <param name="edge">The edge that determines the grid orientation.</param>
    /// <param name="dataRect">The plot rectangle.</param>
    /// <returns>The start and end pixels spanning the plot rectangle.</returns>
    private static (Pixel Start, Pixel End) GetGridLinePixels(float position, Edge edge, PixelRect dataRect) =>
        edge.IsHorizontal()
            ? (new Pixel(position, dataRect.Bottom), new Pixel(position, dataRect.Top))
            : (new Pixel(dataRect.Left, position), new Pixel(dataRect.Right, position));

    /// <summary>Renders grid lines at the specified pixel positions.</summary>
    /// <param name="rp">The render pack containing the canvas and plot rectangle.</param>
    /// <param name="positions">The pixel positions of the grid lines.</param>
    /// <param name="edge">The edge that determines the grid line orientation.</param>
    /// <param name="lineStyle">The style used to render the lines.</param>
    private static void RenderGridLines(RenderPack rp, float[] positions, Edge edge, LineStyle lineStyle)
    {
        if (!lineStyle.CanBeRendered)
        {
            return;
        }

        var starts = new Pixel[positions.Length];
        var ends = new Pixel[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            float px = positions[i];

            starts[i] = edge.IsHorizontal()
                ? new Pixel(px, rp.DataRect.Bottom)
                : new Pixel(rp.DataRect.Left, px);

            ends[i] = edge.IsHorizontal()
                ? new Pixel(px, rp.DataRect.Top)
                : new Pixel(rp.DataRect.Right, px);
        }

        lineStyle.Render(rp.Canvas, starts, ends, rp.Paint);
    }

    /// <summary>Renders alternating fill regions between major grid lines.</summary>
    /// <param name="rp">The render pack containing the canvas and plot rectangle.</param>
    /// <param name="positions">The pixel positions of the major grid lines.</param>
    /// <param name="edge">The edge that determines the fill orientation.</param>
    /// <param name="oddTick">The initial alternating-fill parity.</param>
    private void RenderAlternatingFill(RenderPack rp, float[] positions, Edge edge, int oddTick)
    {
        if (!FillColorEnabled)
        {
            return;
        }

        Pixel[] starts = new Pixel[positions.Length + BoundaryPositionCount];
        Pixel[] ends = new Pixel[positions.Length + BoundaryPositionCount];
        for (int i = 0; i < starts.Length; i++)
        {
            float position = GetFillPosition(i, positions, edge, rp.DataRect);
            (starts[i], ends[i]) = GetGridLinePixels(position, edge, rp.DataRect);
        }

        for (int i = 1; i < starts.Length; i++)
        {
            Color fillColor = (i + oddTick) % MaximumMajorTicksForFill == 0 ? FillColor1 : FillColor2;
            FillStyle fillStyle = new() { Color = fillColor };
            PixelRect rect = new(starts[i - 1], ends[i]);
            Drawing.FillRectangle(rp.Canvas, rect, rp.Paint, fillStyle);
        }
    }
}
