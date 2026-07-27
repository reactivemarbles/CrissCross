// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using System.Windows.Input;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>A custom ScrollViewer that allows certain mouse events to bubble through when it's inactive.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class PassiveScrollViewer : ScrollViewer
{
    /// <summary>Identifies the <see cref="IsScrollSpillEnabled"/> dependency property.</summary>
    public static readonly DependencyProperty IsScrollSpillEnabledProperty = DependencyProperty.Register(
        nameof(IsScrollSpillEnabled),
        typeof(bool),
        typeof(PassiveScrollViewer),
        new(true));

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

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Responds to a click of the mouse wheel.</summary>
    /// <param name="e">Required arguments that describe this event.</param>
    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        ThrowHelper.ThrowIfNull(e, nameof(e));

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
