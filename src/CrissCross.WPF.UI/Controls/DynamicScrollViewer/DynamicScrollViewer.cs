// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using System.Windows.Controls;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Custom <see cref="ScrollViewer"/> with events depending on actions taken by the user.
/// </summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(DynamicScrollViewer), "DynamicScrollViewer.bmp")]
[DefaultEvent("ScrollChangedEvent")]
public class DynamicScrollViewer : PassiveScrollViewer
{
    /// <summary>
    /// Property for <see cref="IsScrollingVertically"/>.
    /// </summary>
    public static readonly DependencyProperty IsScrollingVerticallyProperty = DependencyProperty.Register(
        nameof(IsScrollingVertically),
        typeof(bool),
        typeof(DynamicScrollViewer),
        new PropertyMetadata(false, IsScrollingVerticallyProperty_OnChanged));

    /// <summary>
    /// Property for <see cref="IsScrollingHorizontally"/>.
    /// </summary>
    public static readonly DependencyProperty IsScrollingHorizontallyProperty = DependencyProperty.Register(
        nameof(IsScrollingHorizontally),
        typeof(bool),
        typeof(DynamicScrollViewer),
        new PropertyMetadata(false, IsScrollingHorizontally_OnChanged));

    /// <summary>
    /// Property for <see cref="MinimalChange"/>.
    /// </summary>
    public static readonly DependencyProperty MinimalChangeProperty = DependencyProperty.Register(
        nameof(MinimalChange),
        typeof(double),
        typeof(DynamicScrollViewer),
        new PropertyMetadata(40d, MinimalChangeProperty_OnChanged));

    /// <summary>
    /// Property for <see cref="Timeout"/>.
    /// </summary>
    public static readonly DependencyProperty TimeoutProperty = DependencyProperty.Register(
        nameof(Timeout),
        typeof(int),
        typeof(DynamicScrollViewer),
        new PropertyMetadata(1200, TimeoutProperty_OnChanged));

    private readonly EventIdentifier _verticalIdentifier = new();

    private readonly EventIdentifier _horizontalIdentifier = new();

    // Due to the large number of triggered events, we limit the complex logic of DependencyProperty
    private bool _scrollingVertically;
    private bool _scrollingHorizontally;
    private int _timeout = 1200;
    private double _minimalChange = 40d;

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets information whether the user was scrolling vertically for the last few seconds.
    /// </summary>
    public bool IsScrollingVertically
    {
        get => (bool)GetValue(IsScrollingVerticallyProperty);
        set => SetValue(IsScrollingVerticallyProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets information whether the user was scrolling horizontally for the last few seconds.
    /// </summary>
    public bool IsScrollingHorizontally
    {
        get => (bool)GetValue(IsScrollingHorizontallyProperty);
        set => SetValue(IsScrollingHorizontallyProperty, value);
    }

    /// <summary>
    /// Gets or sets the value required for the scroll to show automatically.
    /// </summary>
    public double MinimalChange
    {
        get => (double)GetValue(MinimalChangeProperty);
        set => SetValue(MinimalChangeProperty, value);
    }

    /// <summary>
    /// Gets or sets time after which the scroll is to be hidden.
    /// </summary>
    public int Timeout
    {
        get => (int)GetValue(TimeoutProperty);
        set => SetValue(TimeoutProperty, value);
    }

    /// <summary>
    /// OnScrollChanged is an override called whenever scrolling state changes on this <see cref="DynamicScrollViewer"/>.
    /// </summary>
    /// <remarks>
    /// OnScrollChanged fires the ScrollChangedEvent. Overriders of this method should call
    /// base.OnScrollChanged(args) if they want the event to be fired.
    /// </remarks>
    /// <param name="e">ScrollChangedEventArgs containing information about the change in scrolling state.</param>
    protected override void OnScrollChanged(ScrollChangedEventArgs e)
    {
        base.OnScrollChanged(e);

        if (e?.HorizontalChange > _minimalChange || e?.HorizontalChange < -_minimalChange)
        {
            UpdateHorizontalScrollingState();
        }

        if (e?.VerticalChange > _minimalChange || e?.VerticalChange < -_minimalChange)
        {
            UpdateVerticalScrollingState();
        }
    }

    private static void IsScrollingVerticallyProperty_OnChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not DynamicScrollViewer scroll)
        {
            return;
        }

        scroll._scrollingVertically = scroll.IsScrollingVertically;
    }

    private static void IsScrollingHorizontally_OnChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not DynamicScrollViewer scroll)
        {
            return;
        }

        scroll._scrollingHorizontally = scroll.IsScrollingHorizontally;
    }

    private static void MinimalChangeProperty_OnChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not DynamicScrollViewer scroll)
        {
            return;
        }

        scroll._minimalChange = scroll.MinimalChange;
    }

    private static void TimeoutProperty_OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not DynamicScrollViewer scroll)
        {
            return;
        }

        scroll._timeout = scroll.Timeout;
    }

    private async void UpdateVerticalScrollingState()
    {
        // TODO: Optimize
        // My main assumption here is that each scroll causes a new "event / thread" to be assigned.
        // If more than Timeout has passed since the last event, there is no interaction.
        // We pass this value to the ScrollBar and link it to IsMouseOver.
        // This way we have a dynamic scrollbar that responds to scroll / mouse over.
        var currentEvent = _verticalIdentifier.GetNext();

        if (!_scrollingVertically)
        {
            IsScrollingVertically = true;
        }

        if (_timeout > -1)
        {
            await Task.Delay(_timeout < 10000 ? _timeout : 1000);
        }

        if (_verticalIdentifier.IsEqual(currentEvent) && _scrollingVertically)
        {
            IsScrollingVertically = false;
        }
    }

    private async void UpdateHorizontalScrollingState()
    {
        // TODO: Optimize
        // My main assumption here is that each scroll causes a new "event / thread" to be assigned.
        // If more than Timeout has passed since the last event, there is no interaction.
        // We pass this value to the ScrollBar and link it to IsMouseOver.
        // This way we have a dynamic scrollbar that responds to scroll / mouse over.
        var currentEvent = _horizontalIdentifier.GetNext();

        if (!_scrollingHorizontally)
        {
            IsScrollingHorizontally = true;
        }

        await Task.Delay(Timeout < 10000 ? Timeout : 1000);

        if (_horizontalIdentifier.IsEqual(currentEvent) && _scrollingHorizontally)
        {
            IsScrollingHorizontally = false;
        }
    }
}
