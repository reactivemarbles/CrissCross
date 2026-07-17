// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using System.Windows.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Custom <see cref="ScrollViewer"/> with events depending on actions taken by the user.</summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(DynamicScrollViewer), "DynamicScrollViewer.bmp")]
[DefaultEvent("ScrollChangedEvent")]
public class DynamicScrollViewer : PassiveScrollViewer
{
    /// <summary>Property for <see cref="IsScrollingVertically"/>.</summary>
    public static readonly DependencyProperty IsScrollingVerticallyProperty = DependencyProperty.Register(
        nameof(IsScrollingVertically),
        typeof(bool),
        typeof(DynamicScrollViewer),
        new PropertyMetadata(false, IsScrollingVerticallyProperty_OnChanged));

    /// <summary>Property for <see cref="IsScrollingHorizontally"/>.</summary>
    public static readonly DependencyProperty IsScrollingHorizontallyProperty = DependencyProperty.Register(
        nameof(IsScrollingHorizontally),
        typeof(bool),
        typeof(DynamicScrollViewer),
        new PropertyMetadata(false, IsScrollingHorizontally_OnChanged));

    /// <summary>Property for <see cref="MinimalChange"/>.</summary>
    public static readonly DependencyProperty MinimalChangeProperty = DependencyProperty.Register(
        nameof(MinimalChange),
        typeof(double),
        typeof(DynamicScrollViewer),
        new PropertyMetadata(40D, MinimalChangeProperty_OnChanged));

    /// <summary>Property for <see cref="Timeout"/>.</summary>
    public static readonly DependencyProperty TimeoutProperty = DependencyProperty.Register(
        nameof(Timeout),
        typeof(int),
        typeof(DynamicScrollViewer),
        new PropertyMetadata(1200, TimeoutProperty_OnChanged));

    /// <summary>The maximum requested timeout that is used without clamping.</summary>
    private const int MaximumRequestedDelayMilliseconds = 10_000;

    /// <summary>The timeout used when the requested delay is too large.</summary>
    private const int MaximumScrollDelayMilliseconds = 1000;

    /// <summary>Stores the _verticalIdentifier value.</summary>
    private readonly EventIdentifier _verticalIdentifier = new();

    /// <summary>Stores the _horizontalIdentifier value.</summary>
    private readonly EventIdentifier _horizontalIdentifier = new();

    /// <summary>Due to the large number of triggered events, we limit the complex logic of DependencyProperty</summary>
    private bool _scrollingVertically;

    /// <summary>Stores the _scrollingHorizontally value.</summary>
    private bool _scrollingHorizontally;

    /// <summary>Stores the _timeout value.</summary>
    private int _timeout = 1200;

    /// <summary>Stores the _minimalChange value.</summary>
    private double _minimalChange = 40D;

    /// <summary>Gets or sets a value indicating whether gets or sets information whether the user was scrolling
    /// vertically for the last few seconds.</summary>
    public bool IsScrollingVertically
    {
        get => (bool)GetValue(IsScrollingVerticallyProperty);
        set => SetValue(IsScrollingVerticallyProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether gets or sets information whether the user was scrolling
    /// horizontally for the last few seconds.</summary>
    public bool IsScrollingHorizontally
    {
        get => (bool)GetValue(IsScrollingHorizontallyProperty);
        set => SetValue(IsScrollingHorizontallyProperty, value);
    }

    /// <summary>Gets or sets the value required for the scroll to show automatically.</summary>
    public double MinimalChange
    {
        get => (double)GetValue(MinimalChangeProperty);
        set => SetValue(MinimalChangeProperty, value);
    }

    /// <summary>Gets or sets time after which the scroll is to be hidden.</summary>
    public int Timeout
    {
        get => (int)GetValue(TimeoutProperty);
        set => SetValue(TimeoutProperty, value);
    }

    /// <summary>Provides the OnScrollChanged member.</summary>
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
            _ = UpdateHorizontalScrollingStateAsync();
        }

        if ((e is null || e.VerticalChange <= _minimalChange) && (e is null || e.VerticalChange >= -_minimalChange))
        {
            return;
        }

        _ = UpdateVerticalScrollingStateAsync();
    }

    /// <summary>Provides the IsScrollingVerticallyProperty_OnChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void IsScrollingVerticallyProperty_OnChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        _ = e;
        if (d is not DynamicScrollViewer scroll)
        {
            return;
        }

        scroll._scrollingVertically = scroll.IsScrollingVertically;
    }

    /// <summary>Provides the IsScrollingHorizontally_OnChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void IsScrollingHorizontally_OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        _ = e;
        if (d is not DynamicScrollViewer scroll)
        {
            return;
        }

        scroll._scrollingHorizontally = scroll.IsScrollingHorizontally;
    }

    /// <summary>Provides the MinimalChangeProperty_OnChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void MinimalChangeProperty_OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        _ = e;
        if (d is not DynamicScrollViewer scroll)
        {
            return;
        }

        scroll._minimalChange = scroll.MinimalChange;
    }

    /// <summary>Provides the TimeoutProperty_OnChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void TimeoutProperty_OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        _ = e;
        if (d is not DynamicScrollViewer scroll)
        {
            return;
        }

        scroll._timeout = scroll.Timeout;
    }

    /// <summary>Updates vertical scrolling state after the configured inactivity delay.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task UpdateVerticalScrollingStateAsync()
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
            await Task.Delay(_timeout < MaximumRequestedDelayMilliseconds ? _timeout : MaximumScrollDelayMilliseconds);
        }

        if (!_verticalIdentifier.IsEqual(currentEvent) || !_scrollingVertically)
        {
            return;
        }

        IsScrollingVertically = false;
    }

    /// <summary>Updates horizontal scrolling state after the configured inactivity delay.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task UpdateHorizontalScrollingStateAsync()
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

        await Task.Delay(Timeout < MaximumRequestedDelayMilliseconds ? Timeout : MaximumScrollDelayMilliseconds);

        if (!_horizontalIdentifier.IsEqual(currentEvent) || !_scrollingHorizontally)
        {
            return;
        }

        IsScrollingHorizontally = false;
    }
}
