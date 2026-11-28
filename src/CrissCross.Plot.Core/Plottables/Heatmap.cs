// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Plottables;

/// <summary>Displays a two-dimensional array of values as a color-mapped heatmap.</summary>
/// <param name="intensities">The intensity values to display.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage("PerformanceSharp", "PSH1020", Justification = "The multidimensional data-array constructor is an established public API.")]
public class Heatmap(double[, ] intensities) : IPlottable, IHasColorAxis
{
    /// <summary>The divisor used to calculate half-cell offsets.</summary>
    private const double HalfCellDivisor = 2;

    /// <summary>The generated color for each intensity value, refreshed by <see cref="Update"/>.</summary>
    private uint[] _cellColors = null!;

    /// <summary>The bitmap generated during rendering and disposed when the heatmap is updated.</summary>
    private SKBitmap? _bitmap;

    /// <summary>Finalizes an instance of the <see cref="Heatmap"/> class.</summary>
    ~Heatmap()
    {
        _bitmap?.Dispose();
    }

    /// <summary>Gets the data values displayed by the heatmap. Call <see cref="Update"/> after editing the array.</summary>
    public double[, ] Intensities { get; } = intensities;

    /// <summary>Gets or sets whether this plottable is visible.</summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>Gets or sets the axes used to position and render this heatmap.</summary>
    public IAxes Axes { get; set; } = new Axes();

    /// <summary>Gets or sets the colormap used to convert intensity values to colors.</summary>
    public IColormap Colormap
    {
        get => field;
        set
        {
            field = value;
            Update();
        }
    } = new Colormaps.Viridis();

    /// <summary>Gets or sets the width of one heatmap cell in coordinate units.</summary>
    public double CellWidth
    {
        get => GetExtentOrDefault().Width / Intensities.GetLength(1);
        set
        {
            var rect = GetExtentOrDefault();
            double left = rect.Left;
            double right = rect.Left + (value * Intensities.GetLength(1));
            double bottom = rect.Bottom;
            double top = rect.Top;
            Extent = new(left, right, bottom, top);
        }
    }

    /// <summary>Gets or sets the alignment of each cell relative to its data coordinate.</summary>
    /// <remarks>An upper-right alignment places the cell to the lower-left of its coordinate.</remarks>
    public Alignment CellAlignment
    {
        get => field;
        set
        {
            field = value;
            Update();
        }
    } = Alignment.MiddleCenter;

    /// <summary>Gets or sets the coordinate extent used to position heatmap cell centers.</summary>
    public CoordinateRect? Extent
    {
        get => field;
        set
        {
            field = value;
            Update();
        }
    }

    /// <summary>Sets the rectangle within whose edges the heatmap is rendered.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST1421",
        Justification = "This established write-only property configures the heatmap extent; adding a getter would require defining new inverse semantics.")]
    public CoordinateRect? Rectangle
    {
        set
        {
            if (value is null)
            {
                Extent = null;
                return;
            }

            CoordinateRect rect = value.Value;
            double cellWidth = rect.Width / Intensities.GetLength(1);
            double cellHeight = rect.Height / Intensities.GetLength(0);
            Extent = new(
                rect.Left + (cellWidth / HalfCellDivisor),
                rect.Right - (cellWidth / HalfCellDivisor),
                rect.Bottom + (cellHeight / HalfCellDivisor),
                rect.Top - (cellHeight / HalfCellDivisor));
        }
    }

    /// <summary>Gets or sets the axis boundaries of the heatmap data.</summary>
    /// <remarks>The rendered heatmap area extends one cell beyond this rectangle.</remarks>
    public CoordinateRect? Position { get => Extent; set => Extent = value; }

    /// <summary>Gets or sets whether the rows are flipped vertically.</summary>
    public bool FlipVertically
    {
        get => field;
        set
        {
            field = value;
            Update();
        }
    }

    /// <summary>Gets or sets whether the columns are flipped horizontally.</summary>
    public bool FlipHorizontally
    {
        get => field;
        set
        {
            field = value;
            Update();
        }
    }

    /// <summary>Gets or sets whether the columns are flipped horizontally.</summary>
    public bool FlipColumns { get => FlipHorizontally; set => FlipHorizontally = value; }

    /// <summary>Gets or sets whether the rows are flipped vertically.</summary>
    public bool FlipRows { get => FlipVertically; set => FlipVertically = value; }

    /// <summary>Gets or sets whether the rendered image is interpolated for a smooth appearance.</summary>
    /// <remarks>When false, cells retain sharp edges.</remarks>
    public bool Smooth
    {
        get => field;
        set
        {
            field = value;
            Update();
        }
    }

    /// <summary>Gets or sets the color used to fill cells containing NaN.</summary>
    public Color NaNCellColor
    {
        get => field;
        set
        {
            field = value;
            Update();
        }
    } = Colors.Transparent;

    /// <summary>Gets or sets the transparency of each heatmap cell when specified.</summary>
    /// <remarks>Alpha values range from 0 (transparent) through 255 (opaque).</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleSharp",
        "SST2305",
        Justification = "Callers rely on replacing the complete alpha map through this established public setter.")]
    public byte[, ]? AlphaMap { get; set; }

    /// <summary>Gets or sets the opacity of the entire heatmap from 0 (transparent) to 1 (opaque).</summary>
    public double Opacity
    {
        get => field;
        set
        {
            field = NumericConversion.Clamp(value, 0, 1);
            Update();
        }
    } = 1;

    /// <summary>Gets or sets the strategy used to render heatmap values.</summary>
    public IRenderStrategy RenderStrategy { get; set; } = new RenderStrategies.Bitmap();

    /// <summary>Gets the range of values spanned by the data when it was last updated.</summary>
    public Range DataRange { get; private set; }

    /// <summary>Gets or sets the range spanned by the colormap, or null to use the data range.</summary>
    public Range? ManualRange
    {
        get => field;
        set
        {
            field = value;
            Update();
        }
    }

    /// <summary>Gets the legend items associated with this heatmap.</summary>
    public IEnumerable<LegendItem> LegendItems => [];

    /// <summary>Gets or sets the height of one heatmap cell in coordinate units.</summary>
    public double CellHeight
    {
        get => GetExtentOrDefault().Height / Intensities.GetLength(0);
        set
        {
            var rect = GetExtentOrDefault();
            double left = rect.Left;
            double right = rect.Right;
            double bottom = rect.Bottom;
            double top = rect.Bottom + (value * Intensities.GetLength(0));
            Extent = new(left, right, bottom, top);
        }
    }

    /// <summary>Gets the value range used to map intensity values to colors.</summary>
    /// <returns>The manual range when set, otherwise the most recently calculated data range.</returns>
    public Range GetRange() => ManualRange ?? DataRange;

    /// <summary>Gets the actual extent of the heatmap after cell alignment has been applied.</summary>
    /// <returns>The aligned extent in data coordinates.</returns>
    public CoordinateRect GetAlignedExtent()
    {
        double horizontalOffset = Math.Abs(CellWidth) * CellAlignment.HorizontalFraction();
        double verticalOffset = Math.Abs(CellHeight) * CellAlignment.VerticalFraction();
        Coordinates cellOffset = new(-horizontalOffset, -verticalOffset);
        return GetExtentOrDefault().WithTranslation(cellOffset);
    }

    /// <summary>Regenerates the image using the current settings and data in <see cref="Intensities"/>.</summary>
    public void Update()
    {
        DataRange = Range.GetRange(Intensities);
        _cellColors = GetArgbValues();
        _bitmap?.Dispose();
        _bitmap = null;
    }

    /// <summary>Gets the indexes of the cell beneath the specified coordinate.</summary>
    /// <param name="coordinates">The coordinate to locate in the heatmap.</param>
    /// <returns>The column and row indexes of the corresponding cell.</returns>
    public (int x, int y) GetIndexes(Coordinates coordinates)
    {
        CoordinateRect rect = GetAlignedExtent();

        double distanceFromLeft = coordinates.X - rect.Left;
        int columnIndex = (int)(distanceFromLeft / CellWidth);

        double distanceFromTop = rect.Top - coordinates.Y;
        int rowIndex = (int)(distanceFromTop / CellHeight);

        return (columnIndex, rowIndex);
    }

    /// <summary>Gets the axis limits occupied by the heatmap.</summary>
    /// <returns>The axis limits of the aligned heatmap extent.</returns>
    public AxisLimits GetAxisLimits() => new(GetAlignedExtent());

    /// <summary>Gets the value beneath a coordinate, or NaN if the coordinate is outside the heatmap.</summary>
    /// <param name="coordinates">The coordinate to query.</param>
    /// <returns>The intensity at the corresponding cell, or NaN when outside the heatmap.</returns>
    public double GetValue(Coordinates coordinates)
    {
        CoordinateRect rect = GetAlignedExtent();

        if (!rect.Contains(coordinates))
        {
            return double.NaN;
        }

        (int columnIndex, int rowIndex) = GetIndexes(coordinates);

        return Intensities[rowIndex, columnIndex];
    }

    /// <summary>Renders the heatmap using its configured render strategy.</summary>
    /// <param name="rp">The render pack containing the canvas and paint.</param>
    public virtual void Render(RenderPack rp)
    {
        if (_cellColors is null)
        {
            // Updating is required before rendering to update colors, generate bitmap, etc.
            // If not called manually before the first render it gets called automatically.
            Update();
        }

        RenderStrategy.Render(rp, this);
    }

    /// <summary>Gets the render extent, using a unit-cell extent centered at the origin when none is specified.</summary>
    /// <returns>The extent used to render the heatmap.</returns>
    private CoordinateRect GetExtentOrDefault()
    {
        if (Extent.HasValue)
        {
            var extent = Extent.Value;

            // The user-supplied extent describes data boundaries, so include one cell beyond each boundary.
            double cellWidth = extent.Width / (Intensities.GetLength(1) - 1);
            double cellHeight = extent.Height / (Intensities.GetLength(0) - 1);
            if (extent.Left < extent.Right)
            {
                extent.Right += cellWidth;
            }

            if (extent.Left > extent.Right)
            {
                extent.Left -= cellWidth; // cellWidth will be negative if the extent is flipped
            }

            if (extent.Bottom < extent.Top)
            {
                extent.Top += cellHeight;
            }

            if (extent.Bottom > extent.Top)
            {
                extent.Bottom -= cellHeight; // cellHeight will be negative if the extent is inverted
            }

            return extent;
        }

        return new(
            left: 0,
            right: Intensities.GetLength(1),
            bottom: 0,
            top: Intensities.GetLength(0));
    }

    /// <summary>Gets the heatmap cell colors as premultiplied ARGB values in bitmap order.</summary>
    /// <returns>The color value for each heatmap cell.</returns>
    private uint[] GetArgbValues()
    {
        Range range = GetRange();
        uint[] argb = new uint[Intensities.Length];

        // The XOR disables flipping when the flip property and the extent are inverted.
        CoordinateRect rect = GetExtentOrDefault();
        bool flipY = FlipVertically ^ rect.IsInvertedY;
        bool flipX = FlipHorizontally ^ rect.IsInvertedX;
        int height = Intensities.GetLength(0);
        int width = Intensities.GetLength(1);

        for (int rowIndex = 0; rowIndex < height; rowIndex++)
        {
            int destinationRowIndex = flipY ? height - 1 - rowIndex : rowIndex;
            for (int columnIndex = 0; columnIndex < width; columnIndex++)
            {
                int sourceColumnIndex = flipX ? width - 1 - columnIndex : columnIndex;
                int destinationIndex = (destinationRowIndex * width) + columnIndex;
                argb[destinationIndex] = GetCellArgb(rowIndex, sourceColumnIndex, range);
            }
        }

        return argb;
    }

    /// <summary>Gets the premultiplied ARGB color for one heatmap cell.</summary>
    /// <param name="rowIndex">The source row index.</param>
    /// <param name="columnIndex">The source column index.</param>
    /// <param name="range">The intensity range used by the colormap.</param>
    /// <returns>The cell color, including alpha-map and opacity settings.</returns>
    private uint GetCellArgb(int rowIndex, int columnIndex, Range range)
    {
        double intensity = Intensities[rowIndex, columnIndex];
        if (double.IsNaN(intensity))
        {
            return NaNCellColor.PremultipliedARGB;
        }

        Color cellColor = Colormap.GetColor(intensity, range);
        if (AlphaMap is not null)
        {
            cellColor = cellColor.WithAlpha(AlphaMap[rowIndex, columnIndex]);
        }

        if (BitConverter.DoubleToInt64Bits(Opacity) != BitConverter.DoubleToInt64Bits(1D))
        {
            cellColor = cellColor.WithAlpha((byte)(cellColor.Alpha * Opacity));
        }

        return cellColor.PremultipliedARGB;
    }

    /// <summary>Defines a strategy for rendering heatmap data.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "RoslynCommonAnalyzers",
        "SST1201:Members should be ordered by kind",
        Justification = "Keep this public interface nested to preserve the established Heatmap API; the analyzer reports it regardless of supported nested-type ordering.")]
    public interface IRenderStrategy
    {
        /// <summary>Renders the supplied heatmap.</summary>
        /// <param name="rp">The render pack containing the canvas and paint.</param>
        /// <param name="hm">The heatmap to render.</param>
        void Render(RenderPack rp, Heatmap hm);
    }

    /// <summary>Provides built-in heatmap rendering strategies.</summary>
    public static class RenderStrategies
    {
        /// <summary>Renders the heatmap as a bitmap, allowing interpolation when smoothing is enabled.</summary>
        public class Bitmap() : IRenderStrategy
        {
            /// <inheritdoc/>
            public void Render(RenderPack rp, Heatmap hm)
            {
                hm._bitmap ??= Drawing.BitmapFromArgbs(
                    hm._cellColors,
                    hm.Intensities.GetLength(1),
                    hm.Intensities.GetLength(0));

                // disable anti-aliasing for crisp edges
                rp.Paint.IsAntialias = hm.Smooth;

                // filter controls resize anti-aliasing
                rp.Paint.ResizeFilter = hm.Smooth ? ResizeFilter.Bicubic : ResizeFilter.NearestNeighbor;

                PixelRect rect = hm.Axes.GetPixelRect(hm.GetAlignedExtent());
                Drawing.DrawImage(rp.Canvas, hm._bitmap, rect, rp.Paint);
            }
        }

        /// <summary>
        /// Render cells as a distinct rectangles instead of pixels on a single bitmap.
        /// This is very inefficient, but may be required for some platforms (like SVG)
        /// where low-pixel bitmaps are rendered with smoothing by default.
        /// </summary>
        public class Rectangles : IRenderStrategy
        {
            /// <summary>The reusable fill style used to draw each cell.</summary>
            private readonly FillStyle _fillStyle = new() { AntiAlias = false };

            /// <inheritdoc/>
            public void Render(RenderPack rp, Heatmap hm)
            {
                CoordinateRect rect = hm.GetAlignedExtent();
                int height = hm.Intensities.GetLength(0);
                int width = hm.Intensities.GetLength(1);
                for (int h = 0; h < height; h++)
                {
                    var offsetY = (height - 1 - h) * hm.CellHeight;
                    for (int w = 0; w < width; w++)
                    {
                        double offsetX = w * hm.CellWidth;
                        PixelRect cellRect = hm.Axes.GetPixelRect(new(
                            rect.Left + offsetX,
                            rect.Left + offsetX + hm.CellWidth,
                            rect.Bottom + offsetY,
                            rect.Bottom + offsetY + hm.CellHeight));
                        _fillStyle.Color = Color.FromARGB(hm._cellColors[(h * width) + w]);
                        Drawing.FillRectangle(rp.Canvas, cellRect, rp.Paint, _fillStyle);
                    }
                }
            }
        }
    }
}
