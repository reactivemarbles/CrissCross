﻿// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Base abstract class for creating virtualized panels.
/// <para>Based on <see href="https://github.com/sbaeumlisberger/VirtualizingWrapPanel"/>.</para>
/// </summary>
public abstract class VirtualizingPanelBase : VirtualizingPanel, IScrollInfo
{
    /// <summary>
    /// Property for <see cref="ScrollLineDelta"/>.
    /// </summary>
    public static readonly DependencyProperty ScrollLineDeltaProperty = DependencyProperty.Register(
        nameof(ScrollLineDelta),
        typeof(double),
        typeof(VirtualizingPanelBase),
        new FrameworkPropertyMetadata(16.0));

    /// <summary>
    /// Property for <see cref="MouseWheelDelta"/>.
    /// </summary>
    public static readonly DependencyProperty MouseWheelDeltaProperty = DependencyProperty.Register(
        nameof(MouseWheelDelta),
        typeof(double),
        typeof(VirtualizingPanelBase),
        new FrameworkPropertyMetadata(48.0));

    /// <summary>
    /// Property for <see cref="ScrollLineDeltaItem"/>.
    /// </summary>
    public static readonly DependencyProperty ScrollLineDeltaItemProperty = DependencyProperty.Register(
        nameof(ScrollLineDeltaItem),
        typeof(int),
        typeof(VirtualizingPanelBase),
        new FrameworkPropertyMetadata(1));

    /// <summary>
    /// Property for <see cref="MouseWheelDeltaItem"/>.
    /// </summary>
    public static readonly DependencyProperty MouseWheelDeltaItemProperty = DependencyProperty.Register(
        nameof(MouseWheelDeltaItem),
        typeof(int),
        typeof(VirtualizingPanelBase),
        new FrameworkPropertyMetadata(3));

    private DependencyObject? _itemsOwner;
    private IRecyclingItemContainerGenerator? _itemContainerGenerator;
    private Visibility _previousVerticalScrollBarVisibility = Visibility.Collapsed;
    private Visibility _previousHorizontalScrollBarVisibility = Visibility.Collapsed;

    /// <summary>
    /// Gets or sets the scroll owner.
    /// </summary>
    public ScrollViewer? ScrollOwner { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets a value that indicates whether the content can be vertically scrolled.
    /// </summary>
    public bool CanVerticallyScroll { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets a value that indicates whether the content can be horizontally scrolled.
    /// </summary>
    public bool CanHorizontallyScroll { get; set; }

    /// <summary>
    /// Gets or sets scroll line delta for pixel based scrolling. The default value is 16 dp.
    /// </summary>
    public double ScrollLineDelta
    {
        get => (double)GetValue(ScrollLineDeltaProperty);
        set => SetValue(ScrollLineDeltaProperty, value);
    }

    /// <summary>
    /// Gets or sets mouse wheel delta for pixel based scrolling. The default value is 48 dp.
    /// </summary>
    public double MouseWheelDelta
    {
        get => (double)GetValue(MouseWheelDeltaProperty);
        set => SetValue(MouseWheelDeltaProperty, value);
    }

    /// <summary>
    /// Gets or sets scroll line delta for item based scrolling. The default value is 1 item.
    /// </summary>
    public double ScrollLineDeltaItem
    {
        get => (int)GetValue(ScrollLineDeltaItemProperty);
        set => SetValue(ScrollLineDeltaItemProperty, value);
    }

    /// <summary>
    /// Gets or sets mouse wheel delta for item based scrolling. The default value is 3 items.
    /// </summary>
    public int MouseWheelDeltaItem
    {
        get => (int)GetValue(MouseWheelDeltaItemProperty);
        set => SetValue(MouseWheelDeltaItemProperty, value);
    }

    /// <summary>
    /// Gets width of the <see cref="Extent"/>.
    /// </summary>
    public double ExtentWidth => Extent.Width;

    /// <summary>
    /// Gets height of the <see cref="Extent"/>.
    /// </summary>
    public double ExtentHeight => Extent.Height;

    /// <summary>
    /// Gets the horizontal offset.
    /// </summary>
    public double HorizontalOffset => Offset.X;

    /// <summary>
    /// Gets the vertical offset.
    /// </summary>
    public double VerticalOffset => Offset.Y;

    /// <summary>
    /// Gets the <see cref="Viewport"/> width.
    /// </summary>
    public double ViewportWidth => Viewport.Width;

    /// <summary>
    /// Gets the <see cref="Viewport"/> height.
    /// </summary>
    public double ViewportHeight => Viewport.Height;

    /// <inheritdoc />
    protected override bool CanHierarchicallyScrollAndVirtualizeCore => true;

    /// <summary>
    /// Gets the scroll unit.
    /// </summary>
    protected ScrollUnit ScrollUnit => GetScrollUnit(ItemsControl);

    /// <summary>
    /// Gets or sets the direction in which the panel scrolls when user turns the mouse wheel.
    /// </summary>
    protected ScrollDirection MouseWheelScrollDirection { get; set; } = ScrollDirection.Vertical;

    /// <summary>
    /// Gets a value indicating whether gets a value that inidicates whether the virtualizing is enabled.
    /// </summary>
    protected bool IsVirtualizing => GetIsVirtualizing(ItemsControl);

    /// <summary>
    /// Gets the virtualization mode.
    /// </summary>
    protected VirtualizationMode VirtualizationMode => GetVirtualizationMode(ItemsControl);

    /// <summary>
    /// Gets a value indicating whether returns true if the panel is in VirtualizationMode.Recycling, otherwise false.
    /// </summary>
    protected bool IsRecycling => VirtualizationMode == VirtualizationMode.Recycling;

    /// <summary>
    /// Gets the cache length before and after the viewport.
    /// </summary>
    protected VirtualizationCacheLength CacheLength { get; private set; }

    /// <summary>
    /// Gets the Unit of the cache length. Can be Pixel, Item or Page.
    /// When the ItemsOwner is a group item it can only be pixel or item.
    /// </summary>
    protected VirtualizationCacheLengthUnit CacheLengthUnit { get; private set; }

    /// <summary>
    /// Gets the ItemsControl (e.g. ListView).
    /// </summary>
    protected ItemsControl ItemsControl => ItemsControl.GetItemsOwner(this);

    /// <summary>
    /// Gets the ItemsControl (e.g. ListView) or if the ItemsControl is grouping a GroupItem.
    /// </summary>
    protected DependencyObject ItemsOwner
    {
        get
        {
            if (_itemsOwner is not null)
            {
                return _itemsOwner;
            }

            /* Use reflection to access internal method because the public
                 * GetItemsOwner method does always return the itmes control instead
                 * of the real items owner for example the group item when grouping */
            var getItemsOwnerInternalMethod = typeof(ItemsControl).GetMethod(
                "GetItemsOwnerInternal",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                [typeof(DependencyObject)],
                null)!;

            _itemsOwner = (DependencyObject)getItemsOwnerInternalMethod.Invoke(null, [this])!;

            return _itemsOwner;
        }
    }

    /// <summary>
    /// Gets items collection.
    /// </summary>
    protected ReadOnlyCollection<object> Items => ((ItemContainerGenerator)ItemContainerGenerator).Items;

    /// <summary>
    /// Gets the offset.
    /// </summary>
    protected Point Offset { get; private set; } = new(0, 0);

    /// <summary>
    /// Gets items container.
    /// </summary>
    protected new IRecyclingItemContainerGenerator ItemContainerGenerator
    {
        get
        {
            if (_itemContainerGenerator is not null)
            {
                return _itemContainerGenerator;
            }

            /* Because of a bug in the framework the ItemContainerGenerator
               is null until InternalChildren accessed at least one time. */
            _ = InternalChildren;
            _itemContainerGenerator = (IRecyclingItemContainerGenerator)base.ItemContainerGenerator;

            return _itemContainerGenerator;
        }
    }

    /// <summary>
    /// Gets or sets the range of items that a realized in <see cref="Viewport"/> or cache.
    /// </summary>
    protected ItemRange ItemRange { get; set; }

    /// <summary>
    /// Gets the <see cref="Extent"/>.
    /// </summary>
    protected Size Extent { get; private set; } = new Size(0, 0);

    /// <summary>
    /// Gets the viewport.
    /// </summary>
    protected Size Viewport { get; private set; } = new Size(0, 0);

    /// <inheritdoc />
    public virtual Rect MakeVisible(Visual visual, Rect rectangle)
    {
        if (visual == null)
        {
            return default;
        }

        var pos = visual.TransformToAncestor(this).Transform(Offset);

        var scrollAmountX = 0d;
        var scrollAmountY = 0d;

        if (pos.X < Offset.X)
        {
            scrollAmountX = -(Offset.X - pos.X);
        }
        else if ((pos.X + rectangle.Width) > (Offset.X + Viewport.Width))
        {
            var notVisibleX = (pos.X + rectangle.Width) - (Offset.X + Viewport.Width);
            var maxScrollX = pos.X - Offset.X; // keep left of the visual visible
            scrollAmountX = Math.Min(notVisibleX, maxScrollX);
        }

        if (pos.Y < Offset.Y)
        {
            scrollAmountY = -(Offset.Y - pos.Y);
        }
        else if ((pos.Y + rectangle.Height) > (Offset.Y + Viewport.Height))
        {
            var notVisibleY = (pos.Y + rectangle.Height) - (Offset.Y + Viewport.Height);
            var maxScrollY = pos.Y - Offset.Y; // keep top of the visual visible
            scrollAmountY = Math.Min(notVisibleY, maxScrollY);
        }

        SetHorizontalOffset(Offset.X + scrollAmountX);
        SetVerticalOffset(Offset.Y + scrollAmountY);

        var visibleRectWidth = Math.Min(rectangle.Width, Viewport.Width);
        var visibleRectHeight = Math.Min(rectangle.Height, Viewport.Height);

        return new Rect(scrollAmountX, scrollAmountY, visibleRectWidth, visibleRectHeight);
    }

    /// <summary>
    /// Sets the vertical offset.
    /// </summary>
    /// <param name="offset">The offset.</param>
    public void SetVerticalOffset(double offset)
    {
        if (offset < 0 || Viewport.Height >= Extent.Height)
        {
            offset = 0;
        }
        else if (offset + Viewport.Height >= Extent.Height)
        {
            offset = Extent.Height - Viewport.Height;
        }

        Offset = new Point(Offset.X, offset);
        ScrollOwner?.InvalidateScrollInfo();

        InvalidateMeasure();
    }

    /// <summary>
    /// Sets the horizontal offset.
    /// </summary>
    /// <param name="offset">The offset.</param>
    public void SetHorizontalOffset(double offset)
    {
        if (offset < 0 || Viewport.Width >= Extent.Width)
        {
            offset = 0;
        }
        else if (offset + Viewport.Width >= Extent.Width)
        {
            offset = Extent.Width - Viewport.Width;
        }

        Offset = new Point(offset, Offset.Y);
        ScrollOwner?.InvalidateScrollInfo();
        InvalidateMeasure();
    }

    /// <inheritdoc />
    public void LineUp() =>
        ScrollVertical(ScrollUnit == ScrollUnit.Pixel ? -ScrollLineDelta : GetLineUpScrollAmount());

    /// <inheritdoc />
    public void LineDown() =>
        ScrollVertical(ScrollUnit == ScrollUnit.Pixel ? ScrollLineDelta : GetLineDownScrollAmount());

    /// <inheritdoc />
    public void LineLeft() =>
        ScrollHorizontal(ScrollUnit == ScrollUnit.Pixel ? -ScrollLineDelta : GetLineLeftScrollAmount());

    /// <inheritdoc />
    public void LineRight() =>
        ScrollHorizontal(ScrollUnit == ScrollUnit.Pixel ? ScrollLineDelta : GetLineRightScrollAmount());

    /// <inheritdoc />
    public void MouseWheelUp()
    {
        if (MouseWheelScrollDirection == ScrollDirection.Vertical)
        {
            ScrollVertical(ScrollUnit == ScrollUnit.Pixel ? -MouseWheelDelta : GetMouseWheelUpScrollAmount());
        }
        else
        {
            MouseWheelLeft();
        }
    }

    /// <inheritdoc />
    public void MouseWheelDown()
    {
        if (MouseWheelScrollDirection == ScrollDirection.Vertical)
        {
            ScrollVertical(ScrollUnit == ScrollUnit.Pixel ? MouseWheelDelta : GetMouseWheelDownScrollAmount());
        }
        else
        {
            MouseWheelRight();
        }
    }

    /// <inheritdoc />
    public void MouseWheelLeft() =>
        ScrollHorizontal(ScrollUnit == ScrollUnit.Pixel ? -MouseWheelDelta : GetMouseWheelLeftScrollAmount());

    /// <inheritdoc />
    public void MouseWheelRight() =>
        ScrollHorizontal(ScrollUnit == ScrollUnit.Pixel ? MouseWheelDelta : GetMouseWheelRightScrollAmount());

    /// <inheritdoc />
    public void PageUp() =>
        ScrollVertical(ScrollUnit == ScrollUnit.Pixel ? -ViewportHeight : GetPageUpScrollAmount());

    /// <inheritdoc />
    public void PageDown() =>
        ScrollVertical(ScrollUnit == ScrollUnit.Pixel ? ViewportHeight : GetPageDownScrollAmount());

    /// <inheritdoc />
    public void PageLeft() =>
        ScrollHorizontal(ScrollUnit == ScrollUnit.Pixel ? -ViewportHeight : GetPageLeftScrollAmount());

    /// <inheritdoc />
    public void PageRight() =>
        ScrollHorizontal(ScrollUnit == ScrollUnit.Pixel ? ViewportHeight : GetPageRightScrollAmount());

    /// <inheritdoc />
    protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
    {
        switch (args?.Action)
        {
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Replace:
                RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                break;
            case NotifyCollectionChangedAction.Move:
                RemoveInternalChildRange(args.OldPosition.Index, args.ItemUICount);
                break;
        }
    }

    /// <summary>
    /// Updates scroll offset, extent and viewport.
    /// </summary>
    /// <param name="availableSize">Size of the available.</param>
    /// <param name="extent">The extent.</param>
    protected virtual void UpdateScrollInfo(Size availableSize, Size extent)
    {
        var invalidateScrollInfo = false;

        if (extent != Extent)
        {
            Extent = extent;
            invalidateScrollInfo = true;
        }

        if (availableSize != Viewport)
        {
            Viewport = availableSize;
            invalidateScrollInfo = true;
        }

        if (ViewportHeight != 0 && VerticalOffset != 0 && VerticalOffset + ViewportHeight + 1 >= ExtentHeight)
        {
            Offset = new Point(Offset.X, extent.Height - availableSize.Height);
            invalidateScrollInfo = true;
        }

        if (
            ViewportWidth != 0
            && HorizontalOffset != 0
            && HorizontalOffset + ViewportWidth + 1 >= ExtentWidth)
        {
            Offset = new Point(extent.Width - availableSize.Width, Offset.Y);
            invalidateScrollInfo = true;
        }

        if (invalidateScrollInfo)
        {
            ScrollOwner?.InvalidateScrollInfo();
        }
    }

    /// <summary>
    /// Gets item index from the generator.
    /// </summary>
    /// <param name="childIndex">Index of the child.</param>
    /// <returns>An Int.</returns>
    protected int GetItemIndexFromChildIndex(int childIndex)
    {
        var generatorPosition = GetGeneratorPositionFromChildIndex(childIndex);
        return ItemContainerGenerator.IndexFromGeneratorPosition(generatorPosition);
    }

    /// <summary>
    /// Gets the position of children from the generator.
    /// </summary>
    /// <param name="childIndex">Index of the child.</param>
    /// <returns>
    /// A GeneratorPosition.
    /// </returns>
    protected virtual GeneratorPosition GetGeneratorPositionFromChildIndex(int childIndex) =>
        new(childIndex, 0);

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        /* Sometimes when scrolling the scrollbar gets hidden without any reason. In this case the "IsMeasureValid"
         * property of the ScrollOwner is false. To prevent a infinite circle the mesasure call is ignored. */
        if (ScrollOwner != null)
        {
            var verticalScrollBarGotHidden =
                ScrollOwner.VerticalScrollBarVisibility == ScrollBarVisibility.Auto
                && ScrollOwner.ComputedVerticalScrollBarVisibility != Visibility.Visible
                && ScrollOwner.ComputedVerticalScrollBarVisibility != _previousVerticalScrollBarVisibility;

            var horizontalScrollBarGotHidden =
                ScrollOwner.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto
                && ScrollOwner.ComputedHorizontalScrollBarVisibility != Visibility.Visible
                && ScrollOwner.ComputedHorizontalScrollBarVisibility
                    != _previousHorizontalScrollBarVisibility;

            _previousVerticalScrollBarVisibility = ScrollOwner.ComputedVerticalScrollBarVisibility;
            _previousHorizontalScrollBarVisibility = ScrollOwner.ComputedHorizontalScrollBarVisibility;

            if ((!ScrollOwner.IsMeasureValid && verticalScrollBarGotHidden) || horizontalScrollBarGotHidden)
            {
                return availableSize;
            }
        }

        Size extent;
        Size desiredSize;

        if (ItemsOwner is IHierarchicalVirtualizationAndScrollInfo groupItem)
        {
            /* If the ItemsOwner is a group item the availableSize is ifinity.
             * Therfore the vieport size provided by the group item is used. */
            var viewportSize = groupItem.Constraints.Viewport.Size;
            var headerSize = groupItem.HeaderDesiredSizes.PixelSize;
            var availableWidth = Math.Max(viewportSize.Width - 5, 0); // left margin of 5 dp
            var availableHeight = Math.Max(viewportSize.Height - headerSize.Height, 0);
            availableSize = new Size(availableWidth, availableHeight);

            extent = CalculateExtent(availableSize);

            desiredSize = new Size(extent.Width, extent.Height);

            Extent = extent;
            Offset = groupItem.Constraints.Viewport.Location;
            Viewport = groupItem.Constraints.Viewport.Size;
            CacheLength = groupItem.Constraints.CacheLength;
            CacheLengthUnit = groupItem.Constraints.CacheLengthUnit; // can be Item or Pixel
        }
        else
        {
            extent = CalculateExtent(availableSize);
            var desiredWidth = Math.Min(availableSize.Width, extent.Width);
            var desiredHeight = Math.Min(availableSize.Height, extent.Height);
            desiredSize = new Size(desiredWidth, desiredHeight);

            UpdateScrollInfo(desiredSize, extent);
            CacheLength = GetCacheLength(ItemsOwner);
            CacheLengthUnit = GetCacheLengthUnit(ItemsOwner); // can be Page, Item or Pixel
        }

        ItemRange = UpdateItemRange();

        RealizeItems();
        VirtualizeItems();

        return desiredSize;
    }

    /// <summary>
    /// Realizes visible and cached items.
    /// </summary>
    protected virtual void RealizeItems()
    {
        var startPosition = ItemContainerGenerator.GeneratorPositionFromIndex(ItemRange.StartIndex);
        var childIndex = startPosition.Offset == 0 ? startPosition.Index : startPosition.Index + 1;

        using var at = ItemContainerGenerator.StartAt(
            startPosition,
            GeneratorDirection.Forward,
            true);

        for (var i = ItemRange.StartIndex; i <= ItemRange.EndIndex; i++, childIndex++)
        {
            var child = (UIElement)ItemContainerGenerator.GenerateNext(out var isNewlyRealized);

            if (
                isNewlyRealized
                || /*recycled*/
                !InternalChildren.Contains(child))
            {
                if (childIndex >= InternalChildren.Count)
                {
                    AddInternalChild(child);
                }
                else
                {
                    InsertInternalChild(childIndex, child);
                }

                ItemContainerGenerator.PrepareItemContainer(child);

                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            if (child is not IHierarchicalVirtualizationAndScrollInfo groupItem)
            {
                continue;
            }

            groupItem.Constraints = new HierarchicalVirtualizationConstraints(
                new VirtualizationCacheLength(0),
                VirtualizationCacheLengthUnit.Item,
                new Rect(0, 0, ViewportWidth, ViewportHeight));

            child.Measure(new Size(ViewportWidth, ViewportHeight));
        }
    }

    /// <summary>
    /// Virtualizes (cleanups) no longer visible or cached items.
    /// </summary>
    protected virtual void VirtualizeItems()
    {
        for (var childIndex = InternalChildren.Count - 1; childIndex >= 0; childIndex--)
        {
            var generatorPosition = GetGeneratorPositionFromChildIndex(childIndex);

            var itemIndex = ItemContainerGenerator.IndexFromGeneratorPosition(generatorPosition);

            if (itemIndex == -1 || ItemRange.Contains(itemIndex))
            {
                continue;
            }

            if (VirtualizationMode == VirtualizationMode.Recycling)
            {
                ItemContainerGenerator.Recycle(generatorPosition, 1);
            }
            else
            {
                ItemContainerGenerator.Remove(generatorPosition, 1);
            }

            RemoveInternalChildRange(childIndex, 1);
        }
    }

    /// <summary>
    /// Sets vertical scroll offset by given amount.
    /// </summary>
    /// <param name="amount">The value by which the offset is to be increased.</param>
    protected void ScrollVertical(double amount) => SetVerticalOffset(VerticalOffset + amount);

    /// <summary>
    /// Sets horizontal scroll offset by given amount.
    /// </summary>
    /// <param name="amount">The value by which the offset is to be increased.</param>
    protected void ScrollHorizontal(double amount) => SetHorizontalOffset(HorizontalOffset + amount);

    /// <summary>
    /// Calculates the extent that would be needed to show all items.
    /// </summary>
    /// <param name="availableSize">Size of the available.</param>
    /// <returns>A Size.</returns>
    protected abstract Size CalculateExtent(Size availableSize);

    /// <summary>
    /// Calculates the item range that is visible in the viewport or cached.
    /// </summary>
    /// <returns>ItemRange.</returns>
    protected abstract ItemRange UpdateItemRange();

    /// <summary>
    /// Gets line up scroll amount.
    /// </summary>
    /// <returns>A double.</returns>
    protected abstract double GetLineUpScrollAmount();

    /// <summary>
    /// Gets line down scroll amount.
    /// </summary>
    /// <returns>A double.</returns>
    protected abstract double GetLineDownScrollAmount();

    /// <summary>
    /// Gets line left scroll amount.
    /// </summary>
    /// <returns>A double.</returns>
    protected abstract double GetLineLeftScrollAmount();

    /// <summary>
    /// Gets line right scroll amount.
    /// </summary>
    /// <returns>A double.</returns>
    protected abstract double GetLineRightScrollAmount();

    /// <summary>
    /// Gets mouse wheel up scroll amount.
    /// </summary>
    /// <returns>A double.</returns>
    protected abstract double GetMouseWheelUpScrollAmount();

    /// <summary>
    /// Gets mouse wheel down scroll amount.
    /// </summary>
    /// <returns>A double.</returns>
    protected abstract double GetMouseWheelDownScrollAmount();

    /// <summary>
    /// Gets mouse wheel left scroll amount.
    /// </summary>
    /// <returns>A double.</returns>
    protected abstract double GetMouseWheelLeftScrollAmount();

    /// <summary>
    /// Gets mouse wheel right scroll amount.
    /// </summary>
    /// <returns>A double.</returns>
    protected abstract double GetMouseWheelRightScrollAmount();

    /// <summary>
    /// Gets page up scroll amount.
    /// </summary>
    /// <returns>A double.</returns>
    protected abstract double GetPageUpScrollAmount();

    /// <summary>
    /// Gets page down scroll amount.
    /// </summary>
    /// <returns>A double.</returns>
    protected abstract double GetPageDownScrollAmount();

    /// <summary>
    /// Gets page left scroll amount.
    /// </summary>
    /// <returns>A double.</returns>
    protected abstract double GetPageLeftScrollAmount();

    /// <summary>
    /// Gets page right scroll amount.
    /// </summary>
    /// <returns>A double.</returns>
    protected abstract double GetPageRightScrollAmount();
}
