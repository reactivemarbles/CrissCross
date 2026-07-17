// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>A custom ScrollViewer that allows certain mouse events to bubble through when it's inactive.</summary>
public class PassiveScrollViewer : ScrollViewer
{
    /// <summary>Identifies the <see cref="IsScrollSpillEnabled"/> dependency property.</summary>
    public static readonly DependencyProperty IsScrollSpillEnabledProperty = DependencyProperty.Register(
        nameof(IsScrollSpillEnabled),
        typeof(bool),
        typeof(PassiveScrollViewer),
        new PropertyMetadata(true));

    /// <summary>Gets or sets a value indicating whether blocked inner scrolling should be propagated forward.</summary>
    public bool IsScrollSpillEnabled
    {
        get => (bool)GetValue(IsScrollSpillEnabledProperty);
        set => SetValue(IsScrollSpillEnabledProperty, value);
    }

    /// <summary>Gets the IsVerticalScrollingDisabled value.</summary>
    private bool IsVerticalScrollingDisabled => VerticalScrollBarVisibility == ScrollBarVisibility.Disabled;

    /// <summary>Gets the IsContentSmallerThanViewport value.</summary>
    private bool IsContentSmallerThanViewport => ScrollableHeight <= 0;

    /// <summary>Responds to a click of the mouse wheel.</summary>
    /// <param name="e">Required arguments that describe this event.</param>
    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        if (e is null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        if (
            IsVerticalScrollingDisabled
            || IsContentSmallerThanViewport
            || (IsScrollSpillEnabled && HasReachedEndOfScrolling(e)))
        {
            return;
        }

        base.OnMouseWheel(e);
    }

    /// <summary>Provides the HasReachedEndOfScrolling member.</summary>
    /// <param name="e">The event arguments.</param>
    /// <returns>The result.</returns>
    private bool HasReachedEndOfScrolling(MouseWheelEventArgs e)
    {
        var isScrollingUp = e.Delta > 0;
        var isScrollingDown = e.Delta < 0;
        var isTopOfViewport = VerticalOffset == 0;
        var isBottomOfViewport = VerticalOffset >= ScrollableHeight;

        return (isScrollingUp && isTopOfViewport) || (isScrollingDown && isBottomOfViewport);
    }
}
