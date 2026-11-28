// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.LegendLayouts;

/// <summary>Arranges legend items in rows or columns, wrapping to fit the available area.</summary>
public class Wrapping : ILegendLayout
{
    /// <summary>Calculates the item and label rectangles for a wrapping legend layout.</summary>
    /// <param name="legend">The legend settings.</param>
    /// <param name="items">The legend items to arrange.</param>
    /// <param name="maxSize">The maximum available size.</param>
    /// <param name="paint">The paint object used to measure labels.</param>
    /// <returns>The calculated legend layout.</returns>
    public LegendLayout GetLayout(Legend legend, LegendItem[] items, PixelSize maxSize, Paint paint)
    {
        (PixelSize[] labelSizes, float maxLabelWidth, float maxLabelHeight, float maxMarkerSize) = MeasureItems(items, paint);
        float maxItemWidth = Math.Max(legend.SymbolWidth, maxMarkerSize) + legend.SymbolPadding + maxLabelWidth;
        float maxItemHeight = Math.Max(maxLabelHeight, Math.Max(legend.SymbolHeight, maxMarkerSize));
        PixelSize maxSizeAfterPadding = maxSize.Contracted(legend.Padding);
        PixelRect maxRectAfterPadding = new(0, maxSizeAfterPadding.Width, maxSizeAfterPadding.Height, 0);
        (PixelRect[] labelRects, PixelRect[] symbolRects) = CreateItemRectangles(
            legend,
            items,
            labelSizes,
            maxItemWidth,
            maxItemHeight,
            maxSizeAfterPadding,
            maxRectAfterPadding);

        float tightWidth = 0;
        float tightHeight = 0;
        for (int i = 0; i < labelRects.Length; i++)
        {
            tightWidth = Math.Max(tightWidth, labelRects[i].Right);
            tightHeight = Math.Max(tightHeight, labelRects[i].Bottom);
        }

        tightWidth = Math.Min(tightWidth, maxSizeAfterPadding.Width);
        tightHeight = Math.Min(tightHeight, maxSizeAfterPadding.Height);
        PixelRect legendRect = new(0, tightWidth + legend.Padding.Horizontal, tightHeight + legend.Padding.Vertical, 0);
        PixelOffset paddingOffset = new(legend.Padding.Left, legend.Padding.Top);
        for (int i = 0; i < labelRects.Length; i++)
        {
            labelRects[i] = labelRects[i].WithOffset(paddingOffset);
            symbolRects[i] = symbolRects[i].WithOffset(paddingOffset);
        }

        return new LegendLayout { LegendItems = items, LegendRect = legendRect, LabelRects = labelRects, SymbolRects = symbolRects };
    }

    /// <summary>Measures legend labels and finds the maximum label and marker dimensions.</summary>
    /// <param name="items">The legend items to measure.</param>
    /// <param name="paint">The paint object used to measure labels.</param>
    /// <returns>The label sizes and the maximum measured dimensions.</returns>
    private static (PixelSize[] LabelSizes, float MaxLabelWidth, float MaxLabelHeight, float MaxMarkerSize) MeasureItems(LegendItem[] items, Paint paint)
    {
        PixelSize[] labelSizes = new PixelSize[items.Length];
        float maxLabelWidth = 0;
        float maxLabelHeight = 0;
        float maxMarkerSize = 0;
        for (int i = 0; i < items.Length; i++)
        {
            PixelSize labelSize = items[i].LabelStyle.Measure(items[i].LabelText, paint).Size;
            labelSizes[i] = labelSize;
            maxLabelWidth = Math.Max(maxLabelWidth, labelSize.Width);
            maxLabelHeight = Math.Max(maxLabelHeight, labelSize.Height);
            maxMarkerSize = Math.Max(maxMarkerSize, items[i].MarkerStyle.Size);
        }

        return (labelSizes, maxLabelWidth, maxLabelHeight, maxMarkerSize);
    }

    /// <summary>Creates symbol and label rectangles for the supplied legend items.</summary>
    /// <param name="legend">The legend settings.</param>
    /// <param name="items">The items to arrange.</param>
    /// <param name="labelSizes">The previously measured label sizes.</param>
    /// <param name="maxItemWidth">The maximum item width.</param>
    /// <param name="maxItemHeight">The maximum item height.</param>
    /// <param name="maxSizeAfterPadding">The available size after padding.</param>
    /// <param name="maxRectAfterPadding">The available rectangle after padding.</param>
    /// <returns>The label rectangles and symbol rectangles.</returns>
    private static (PixelRect[] LabelRects, PixelRect[] SymbolRects) CreateItemRectangles(
        Legend legend,
        LegendItem[] items,
        PixelSize[] labelSizes,
        float maxItemWidth,
        float maxItemHeight,
        PixelSize maxSizeAfterPadding,
        PixelRect maxRectAfterPadding)
    {
        PixelRect[] labelRects = new PixelRect[items.Length];
        PixelRect[] symbolRects = new PixelRect[items.Length];
        Pixel nextPixel = new(0, 0);
        for (int i = 0; i < items.Length; i++)
        {
            float itemWidth = legend.TightHorizontalWrapping
                ? legend.SymbolWidth + legend.SymbolPadding + labelSizes[i].Width
                : maxItemWidth;

            if (legend.Orientation == Orientation.Horizontal)
            {
                if (nextPixel.X + itemWidth > maxSizeAfterPadding.Width)
                {
                    nextPixel = new(0, nextPixel.Y + maxItemHeight + legend.InterItemPadding.Bottom);
                }
            }
            else if (nextPixel.Y + maxItemHeight > maxSizeAfterPadding.Height)
            {
                nextPixel = new(nextPixel.X + itemWidth + legend.InterItemPadding.Right, 0);
            }

            PixelRect itemRect = new PixelRect(nextPixel, new PixelSize(itemWidth, maxItemHeight)).Intersect(maxRectAfterPadding);
            float symbolWidth = Math.Max(legend.SymbolWidth, items[i].MarkerStyle.Size);
            symbolRects[i] = new(itemRect.Left, itemRect.Left + symbolWidth, itemRect.Bottom, itemRect.Top);
            labelRects[i] = new(
                left: itemRect.Left + symbolWidth + legend.SymbolPadding,
                right: itemRect.Right,
                bottom: itemRect.Bottom,
                top: itemRect.Top);
            nextPixel = legend.Orientation == Orientation.Horizontal
                ? new(nextPixel.X + itemWidth + legend.InterItemPadding.Right, nextPixel.Y)
                : new(nextPixel.X, nextPixel.Y + maxItemHeight + legend.InterItemPadding.Bottom);
        }

        return (labelRects, symbolRects);
    }
}
