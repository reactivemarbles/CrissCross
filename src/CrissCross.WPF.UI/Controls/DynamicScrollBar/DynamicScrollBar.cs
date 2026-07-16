// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Drawing;
using System.Windows.Input;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Custom ScrollBar with events depending on actions taken by the user.</summary>
[ToolboxItem(true)]
[ToolboxBitmap(typeof(DynamicScrollBar), "DynamicScrollBar.bmp")]
public class DynamicScrollBar : System.Windows.Controls.Primitives.ScrollBar
{
    /// <summary>Property for <see cref="IsScrolling"/>.</summary>
    public static readonly DependencyProperty IsScrollingProperty = DependencyProperty.Register(
        nameof(IsScrolling),
        typeof(bool),
        typeof(DynamicScrollBar),
        new PropertyMetadata(false, IsScrollingProperty_OnChange));

    /// <summary>Property for <see cref="IsInteracted"/>.</summary>
    public static readonly DependencyProperty IsInteractedProperty = DependencyProperty.Register(
        nameof(IsInteracted),
        typeof(bool),
        typeof(DynamicScrollBar),
        new PropertyMetadata(false, IsInteractedProperty_OnChange));

    /// <summary>Property for <see cref="Timeout"/>.</summary>
    public static readonly DependencyProperty TimeoutProperty = DependencyProperty.Register(
        nameof(Timeout),
        typeof(int),
        typeof(DynamicScrollBar),
        new PropertyMetadata(1000));

    /// <summary>Stores the _interactiveIdentifier value.</summary>
    private readonly EventIdentifier _interactiveIdentifier = new();

    /// <summary>Stores the _isScrolling value.</summary>
    private bool _isScrolling;

    /// <summary>Stores the _isInteracted value.</summary>
    private bool _isInteracted;

    /// <summary>Gets or sets a value indicating whether gets or sets information whether the user was scrolling for the
    /// last few seconds.</summary>
    public bool IsScrolling
    {
        get => (bool)GetValue(IsScrollingProperty);
        set => SetValue(IsScrollingProperty, value);
    }

    /// <summary>Gets or sets whether informs whether the user has taken an action related to scrolling.</summary>
    public bool IsInteracted
    {
        get => (bool)GetValue(IsInteractedProperty);
        set
        {
            if ((bool)GetValue(IsInteractedProperty) == value)
            {
                return;
            }

            SetValue(IsInteractedProperty, value);
        }
    }

    /// <summary>Gets or sets additional delay after which the DynamicScrollBar should be hidden.</summary>
    public int Timeout
    {
        get => (int)GetValue(TimeoutProperty);
        set => SetValue(TimeoutProperty, value);
    }

    /// <summary>Method reporting the mouse entered this element.</summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);

        _ = UpdateScroll().GetAwaiter();
    }

    /// <summary>Method reporting the mouse leaved this element.</summary>
    /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);

        _ = UpdateScroll().GetAwaiter();
    }

    /// <summary>Provides the IsScrollingProperty_OnChange member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void IsScrollingProperty_OnChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        _ = e;
        if (d is not DynamicScrollBar bar)
        {
            return;
        }

        if (bar._isScrolling == bar.IsScrolling)
        {
            return;
        }

        bar._isScrolling = !bar._isScrolling;

        _ = bar.UpdateScroll().GetAwaiter();
    }

    /// <summary>Provides the IsInteractedProperty_OnChange member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void IsInteractedProperty_OnChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        _ = e;
        if (d is not DynamicScrollBar bar)
        {
            return;
        }

        if (bar._isInteracted == bar.IsInteracted)
        {
            return;
        }

        bar._isInteracted = !bar._isInteracted;

        _ = bar.UpdateScroll().GetAwaiter();
    }

    /// <summary>Provides the UpdateScroll member.</summary>
    /// <returns>The result.</returns>
    private async Task UpdateScroll()
    {
        var currentEvent = _interactiveIdentifier.GetNext();
        var shouldScroll = IsMouseOver || _isScrolling;

        if (shouldScroll == _isInteracted)
        {
            return;
        }

        if (!shouldScroll)
        {
            await Task.Delay(Timeout);
        }

        if (!_interactiveIdentifier.IsEqual(currentEvent))
        {
            return;
        }

        IsInteracted = shouldScroll;
    }
}
