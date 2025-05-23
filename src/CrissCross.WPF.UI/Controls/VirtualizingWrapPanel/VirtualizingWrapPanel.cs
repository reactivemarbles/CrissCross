﻿// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Extended base class for <see cref="VirtualizingPanel"/>.
/// <para>Based on <see href="https://github.com/sbaeumlisberger/VirtualizingWrapPanel"/>.</para>
/// </summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(VirtualizingWrapPanel), "VirtualizingWrapPanel.bmp")]
public class VirtualizingWrapPanel : VirtualizingPanelBase
{
    /// <summary>
    /// Property for <see cref="SpacingMode"/>.
    /// </summary>
    public static readonly DependencyProperty SpacingModeProperty = DependencyProperty.Register(
        nameof(SpacingMode),
        typeof(SpacingMode),
        typeof(VirtualizingWrapPanel),
        new FrameworkPropertyMetadata(SpacingMode.Uniform, FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// Property for <see cref="Orientation"/>.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(VirtualizingWrapPanel),
        new FrameworkPropertyMetadata(
            Orientation.Vertical,
            FrameworkPropertyMetadataOptions.AffectsMeasure,
            OnOrientationChanged));

    /// <summary>
    /// Property for <see cref="ItemSize"/>.
    /// </summary>
    public static readonly DependencyProperty ItemSizeProperty = DependencyProperty.Register(
        nameof(ItemSize),
        typeof(Size),
        typeof(VirtualizingWrapPanel),
        new FrameworkPropertyMetadata(Size.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// Property for <see cref="StretchItems"/>.
    /// </summary>
    public static readonly DependencyProperty StretchItemsProperty = DependencyProperty.Register(
        nameof(StretchItems),
        typeof(bool),
        typeof(VirtualizingWrapPanel),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Size of the single child element.
    /// </summary>
#pragma warning disable SA1401 // Fields should be private
    protected Size ChildSize;

    /// <summary>
    /// Amount of the displayed rows.
    /// </summary>
    protected int RowCount;

    /// <summary>
    /// Amount of displayed items per row.
    /// </summary>
    protected int ItemsPerRowCount;
#pragma warning restore SA1401 // Fields should be private

    /// <summary>
    /// Gets or sets the spacing mode used when arranging the items. The default value is <see cref="SpacingMode.Uniform"/>.
    /// </summary>
    public SpacingMode SpacingMode
    {
        get => (SpacingMode)GetValue(SpacingModeProperty);
        set => SetValue(SpacingModeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that specifies the orientation in which items are arranged. The default value is <see cref="Orientation.Vertical"/>.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that specifies the size of the items. The default value is <see cref="Size.Empty"/>.
    /// If the value is <see cref="Size.Empty"/> the size of the items gots measured by the first realized item.
    /// </summary>
    public Size ItemSize
    {
        get => (Size)GetValue(ItemSizeProperty);
        set => SetValue(ItemSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets a value that specifies if the items get stretched to fill up remaining space. The default value is false.
    /// </summary>
    /// <remarks>
    /// The MaxWidth and MaxHeight properties of the ItemContainerStyle can be used to limit the stretching.
    /// In this case the use of the remaining space will be determined by the SpacingMode property.
    /// </remarks>
    public bool StretchItems
    {
        get => (bool)GetValue(StretchItemsProperty);
        set => SetValue(StretchItemsProperty, value);
    }

    /// <summary>
    /// This virtual method is called when <see cref="Orientation"/> is changed.
    /// </summary>
    protected virtual void OnOrientationChanged() => MouseWheelScrollDirection =
            Orientation == Orientation.Vertical ? ScrollDirection.Vertical : ScrollDirection.Horizontal;

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        UpdateChildSize(availableSize);

        return base.MeasureOverride(availableSize);
    }

    /// <inheritdoc />
    protected override Size CalculateExtent(Size availableSize)
    {
        var extentWidth =
            SpacingMode != SpacingMode.None && !double.IsInfinity(GetWidth(availableSize))
                ? GetWidth(availableSize)
                : GetWidth(ChildSize) * ItemsPerRowCount;

        if (ItemsOwner is IHierarchicalVirtualizationAndScrollInfo groupItem)
        {
            extentWidth =
                Orientation == Orientation.Vertical
                    ? Math.Max(extentWidth - (Margin.Left + Margin.Right), 0)
                    : Math.Max(extentWidth - (Margin.Top + Margin.Bottom), 0);
        }

        var extentHeight = GetHeight(ChildSize) * RowCount;

        return CreateSize(extentWidth, extentHeight);
    }

    /// <summary>
    /// Calculates desired spacing between items.
    /// </summary>
    /// <param name="finalSize">The final size.</param>
    /// <param name="innerSpacing">The inner spacing.</param>
    /// <param name="outerSpacing">The outer spacing.</param>
    protected void CalculateSpacing(Size finalSize, out double innerSpacing, out double outerSpacing)
    {
        var childSize = CalculateChildArrangeSize(finalSize);

        var finalWidth = GetWidth(finalSize);

        var totalItemsWidth = Math.Min(GetWidth(childSize) * ItemsPerRowCount, finalWidth);
        var unusedWidth = finalWidth - totalItemsWidth;

        var spacingMode = SpacingMode;

        switch (spacingMode)
        {
            case SpacingMode.Uniform:
                innerSpacing = outerSpacing = unusedWidth / (ItemsPerRowCount + 1);
                break;

            case SpacingMode.BetweenItemsOnly:
                innerSpacing = unusedWidth / Math.Max(ItemsPerRowCount - 1, 1);
                outerSpacing = 0;
                break;

            case SpacingMode.StartAndEndOnly:
                innerSpacing = 0;
                outerSpacing = unusedWidth / 2;
                break;
            default:
                innerSpacing = 0;
                outerSpacing = 0;
                break;
        }
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        var offsetX = GetX(Offset);
        var offsetY = GetY(Offset);

        /* When the items owner is a group item offset is handled by the parent panel. */
        if (ItemsOwner is IHierarchicalVirtualizationAndScrollInfo groupItem)
        {
            offsetY = 0;
        }

        var childSize = CalculateChildArrangeSize(finalSize);

        CalculateSpacing(finalSize, out var innerSpacing, out var outerSpacing);

        for (var childIndex = 0; childIndex < InternalChildren.Count; childIndex++)
        {
            var child = InternalChildren[childIndex];

            var itemIndex = GetItemIndexFromChildIndex(childIndex);

            var columnIndex = itemIndex % ItemsPerRowCount;
            var rowIndex = itemIndex / ItemsPerRowCount;

            var x = outerSpacing + (columnIndex * (GetWidth(childSize) + innerSpacing));
            var y = rowIndex * GetHeight(childSize);

            if (GetHeight(finalSize) == 0.0)
            {
                /* When the parent panel is grouping and a cached group item is not
                 * in the viewport it has no valid arrangement. That means that the
                 * height/width is 0. Therefore the items should not be visible so
                 * that they are not falsely displayed. */
                child.Arrange(new Rect(0, 0, 0, 0));
            }
            else
            {
                child.Arrange(CreateRect(x - offsetX, y - offsetY, childSize.Width, childSize.Height));
            }
        }

        return finalSize;
    }

    /// <summary>
    /// Calculates desired child arrange size.
    /// </summary>
    /// <param name="finalSize">The final size.</param>
    /// <returns>A Size.</returns>
    protected Size CalculateChildArrangeSize(Size finalSize)
    {
        if (!StretchItems)
        {
            return ChildSize;
        }

        if (Orientation == Orientation.Vertical)
        {
            var childMaxWidth = ReadItemContainerStyle(MaxWidthProperty, double.PositiveInfinity);
            var maxPossibleChildWith = finalSize.Width / ItemsPerRowCount;
            var childWidth = Math.Min(maxPossibleChildWith, childMaxWidth);

            return new Size(childWidth, ChildSize.Height);
        }

        var childMaxHeight = ReadItemContainerStyle(MaxHeightProperty, double.PositiveInfinity);
        var maxPossibleChildHeight = finalSize.Height / ItemsPerRowCount;
        var childHeight = Math.Min(maxPossibleChildHeight, childMaxHeight);

        return new Size(ChildSize.Width, childHeight);
    }

    /// <inheritdoc />
    protected override ItemRange UpdateItemRange()
    {
        if (!IsVirtualizing)
        {
            return new ItemRange(0, Items.Count - 1);
        }

        int startIndex;
        int endIndex;

        if (ItemsOwner is IHierarchicalVirtualizationAndScrollInfo groupItem)
        {
            if (!VirtualizingPanel.GetIsVirtualizingWhenGrouping(ItemsControl))
            {
                return new ItemRange(0, Items.Count - 1);
            }

            var offset = new Point(Offset.X, groupItem.Constraints.Viewport.Location.Y);

            int offsetRowIndex;
            double offsetInPixel;

            int rowCountInViewport;

            if (ScrollUnit == ScrollUnit.Item)
            {
                offsetRowIndex = GetY(offset) >= 1 ? (int)GetY(offset) - 1 : 0; // ignore header
                offsetInPixel = offsetRowIndex * GetHeight(ChildSize);
            }
            else
            {
                offsetInPixel = Math.Min(
                    Math.Max(GetY(offset) - GetHeight(groupItem.HeaderDesiredSizes.PixelSize), 0),
                    GetHeight(Extent));
                offsetRowIndex = GetRowIndex(offsetInPixel);
            }

            var viewportHeight = Math.Min(
                GetHeight(Viewport),
                Math.Max(GetHeight(Extent) - offsetInPixel, 0));

            rowCountInViewport =
                (int)Math.Ceiling((offsetInPixel + viewportHeight) / GetHeight(ChildSize))
                - (int)Math.Floor(offsetInPixel / GetHeight(ChildSize));

            startIndex = offsetRowIndex * ItemsPerRowCount;
            endIndex = Math.Min(
                ((offsetRowIndex + rowCountInViewport) * ItemsPerRowCount) - 1,
                Items.Count - 1);

            if (CacheLengthUnit == VirtualizationCacheLengthUnit.Pixel)
            {
                var cacheBeforeInPixel = Math.Min(CacheLength.CacheBeforeViewport, offsetInPixel);
                var cacheAfterInPixel = Math.Min(
                    CacheLength.CacheAfterViewport,
                    GetHeight(Extent) - viewportHeight - offsetInPixel);

                var rowCountInCacheBefore = (int)(cacheBeforeInPixel / GetHeight(ChildSize));
                var rowCountInCacheAfter = ((int)Math.Ceiling((offsetInPixel + viewportHeight + cacheAfterInPixel) / GetHeight(ChildSize))) - (int)Math.Ceiling((offsetInPixel + viewportHeight) / GetHeight(ChildSize));

                startIndex = Math.Max(startIndex - (rowCountInCacheBefore * ItemsPerRowCount), 0);
                endIndex = Math.Min(endIndex + (rowCountInCacheAfter * ItemsPerRowCount), Items.Count - 1);
            }
            else if (CacheLengthUnit == VirtualizationCacheLengthUnit.Item)
            {
                startIndex = Math.Max(startIndex - (int)CacheLength.CacheBeforeViewport, 0);
                endIndex = Math.Min(endIndex + (int)CacheLength.CacheAfterViewport, Items.Count - 1);
            }
        }
        else
        {
            var viewportSartPos = GetY(Offset);
            var viewportEndPos = GetY(Offset) + GetHeight(Viewport);

            if (CacheLengthUnit == VirtualizationCacheLengthUnit.Pixel)
            {
                viewportSartPos = Math.Max(viewportSartPos - CacheLength.CacheBeforeViewport, 0);
                viewportEndPos = Math.Min(viewportEndPos + CacheLength.CacheAfterViewport, GetHeight(Extent));
            }

            var startRowIndex = GetRowIndex(viewportSartPos);
            startIndex = startRowIndex * ItemsPerRowCount;

            var endRowIndex = GetRowIndex(viewportEndPos);
            endIndex = Math.Min((endRowIndex * ItemsPerRowCount) + (ItemsPerRowCount - 1), Items.Count - 1);

            if (CacheLengthUnit == VirtualizationCacheLengthUnit.Page)
            {
                var itemsPerPage = endIndex - startIndex + 1;
                startIndex = Math.Max(startIndex - ((int)CacheLength.CacheBeforeViewport * itemsPerPage), 0);
                endIndex = Math.Min(
                    endIndex + ((int)CacheLength.CacheAfterViewport * itemsPerPage),
                    Items.Count - 1);
            }
            else if (CacheLengthUnit == VirtualizationCacheLengthUnit.Item)
            {
                startIndex = Math.Max(startIndex - (int)CacheLength.CacheBeforeViewport, 0);
                endIndex = Math.Min(endIndex + (int)CacheLength.CacheAfterViewport, Items.Count - 1);
            }
        }

        return new ItemRange(startIndex, endIndex);
    }

    /// <inheritdoc />
    protected override void BringIndexIntoView(int index)
    {
        if (index < 0 || index >= Items.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index),
                $"The argument {nameof(index)} must be >= 0 and < the number of items.");
        }

        if (ItemsPerRowCount == 0)
        {
            throw new InvalidOperationException();
        }

        var offset = (index / ItemsPerRowCount) * GetHeight(ChildSize);

        if (Orientation == Orientation.Horizontal)
        {
            SetHorizontalOffset(offset);
        }
        else
        {
            SetVerticalOffset(offset);
        }
    }

    /// <inheritdoc />
    protected override double GetLineUpScrollAmount() =>
        -Math.Min(ChildSize.Height * ScrollLineDeltaItem, Viewport.Height);

    /// <inheritdoc />
    protected override double GetLineDownScrollAmount() =>
        Math.Min(ChildSize.Height * ScrollLineDeltaItem, Viewport.Height);

    /// <inheritdoc />
    protected override double GetLineLeftScrollAmount() =>
        -Math.Min(ChildSize.Width * ScrollLineDeltaItem, Viewport.Width);

    /// <inheritdoc />
    protected override double GetLineRightScrollAmount() =>
        Math.Min(ChildSize.Width * ScrollLineDeltaItem, Viewport.Width);

    /// <inheritdoc />
    protected override double GetMouseWheelUpScrollAmount() =>
        -Math.Min(ChildSize.Height * MouseWheelDeltaItem, Viewport.Height);

    /// <inheritdoc />
    protected override double GetMouseWheelDownScrollAmount() =>
        Math.Min(ChildSize.Height * MouseWheelDeltaItem, Viewport.Height);

    /// <inheritdoc />
    protected override double GetMouseWheelLeftScrollAmount() =>
        -Math.Min(ChildSize.Width * MouseWheelDeltaItem, Viewport.Width);

    /// <inheritdoc />
    protected override double GetMouseWheelRightScrollAmount() =>
        Math.Min(ChildSize.Width * MouseWheelDeltaItem, Viewport.Width);

    /// <inheritdoc />
    protected override double GetPageUpScrollAmount() => -Viewport.Height;

    /// <inheritdoc />
    protected override double GetPageDownScrollAmount() => Viewport.Height;

    /// <inheritdoc />
    protected override double GetPageLeftScrollAmount() => -Viewport.Width;

    /// <inheritdoc />
    protected override double GetPageRightScrollAmount() => Viewport.Width;

    /// <summary>
    /// Gets X panel orientation.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>A double.</returns>
    protected double GetX(Point point) => Orientation == Orientation.Vertical ? point.X : point.Y;

    /// <summary>
    /// Gets Y panel orientation.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>A double.</returns>
    protected double GetY(Point point) => Orientation == Orientation.Vertical ? point.Y : point.X;

    /// <summary>
    /// Gets panel width.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>A double.</returns>
    protected double GetWidth(Size size) => Orientation == Orientation.Vertical ? size.Width : size.Height;

    /// <summary>
    /// Gets panel height.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>A double.</returns>
    protected double GetHeight(Size size) => Orientation == Orientation.Vertical ? size.Height : size.Width;

    /// <summary>
    /// Defines panel size.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns>A size.</returns>
    protected Size CreateSize(double width, double height) =>
        Orientation == Orientation.Vertical ? new Size(width, height) : new Size(height, width);

    /// <summary>
    /// Defines panel coordinates and size.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns>A rect.</returns>
    protected Rect CreateRect(double x, double y, double width, double height) =>
        Orientation == Orientation.Vertical ? new Rect(x, y, width, height) : new Rect(y, x, width, height);

    /// <summary>
    /// Private callback for <see cref="OrientationProperty"/>.
    /// </summary>
    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not VirtualizingWrapPanel panel)
        {
            return;
        }

        panel.OnOrientationChanged();
    }

    /// <summary>
    /// Gets container style of the <see cref="ItemsControl"/>.
    /// </summary>
    private T ReadItemContainerStyle<T>(DependencyProperty property, T fallbackValue)
        where T : notnull
    {
        var value = ItemsControl
            .ItemContainerStyle?.Setters.OfType<Setter>()
            .FirstOrDefault(setter => setter.Property == property)
            ?.Value;
        return (T)(value ?? fallbackValue);
    }

    /// <summary>
    /// Gets item row index.
    /// </summary>
    private int GetRowIndex(double location)
    {
        var calculatedRowIndex = (int)Math.Floor(location / GetHeight(ChildSize));
        var maxRowIndex = (int)Math.Ceiling((double)Items.Count / (double)ItemsPerRowCount);

        return Math.Max(Math.Min(calculatedRowIndex, maxRowIndex), 0);
    }

    /// <summary>
    /// Updates child size of <see cref="ItemSize"/>.
    /// </summary>
    private void UpdateChildSize(Size availableSize)
    {
        if (
            ItemsOwner is IHierarchicalVirtualizationAndScrollInfo groupItem
            && VirtualizingPanel.GetIsVirtualizingWhenGrouping(ItemsControl))
        {
            if (Orientation == Orientation.Vertical)
            {
                availableSize.Width = groupItem.Constraints.Viewport.Size.Width;
                availableSize.Width = Math.Max(availableSize.Width - (Margin.Left + Margin.Right), 0);
            }
            else
            {
                availableSize.Height = groupItem.Constraints.Viewport.Size.Height;
                availableSize.Height = Math.Max(availableSize.Height - (Margin.Top + Margin.Bottom), 0);
            }
        }

        if (ItemSize != Size.Empty)
        {
            ChildSize = ItemSize;
        }
        else if (InternalChildren.Count != 0)
        {
            ChildSize = InternalChildren[0].DesiredSize;
        }
        else
        {
            ChildSize = CalculateChildSize(availableSize);
        }

        ItemsPerRowCount = double.IsInfinity(GetWidth(availableSize))
            ? Items.Count
            : Math.Max(1, (int)Math.Floor(GetWidth(availableSize) / GetWidth(ChildSize)));

        RowCount = (int)Math.Ceiling((double)Items.Count / ItemsPerRowCount);
    }

    /// <summary>
    /// Calculates child size.
    /// </summary>
    private Size CalculateChildSize(Size availableSize)
    {
        if (Items.Count == 0)
        {
            return new Size(0, 0);
        }

        var startPosition = ItemContainerGenerator.GeneratorPositionFromIndex(0);

        using var at = ItemContainerGenerator.StartAt(
            startPosition,
            GeneratorDirection.Forward,
            true);

        var child = (UIElement)ItemContainerGenerator.GenerateNext();
        AddInternalChild(child);
        ItemContainerGenerator.PrepareItemContainer(child);
        child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        return child.DesiredSize;
    }
}
