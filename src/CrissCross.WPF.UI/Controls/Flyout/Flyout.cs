// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using System.Windows.Controls.Primitives;
using ReactiveMarbles.ObservableEvents;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Represents a control that creates a pop-up window that displays information for an element in the interface.
/// </summary>
[TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
public class Flyout : System.Windows.Controls.ContentControl
{
    /// <summary>
    /// Property for <see cref="IsOpen"/>.
    /// </summary>
    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
        nameof(IsOpen),
        typeof(bool),
        typeof(Flyout),
        new PropertyMetadata(false));

    /// <summary>
    /// Property for <see cref="Placement"/>.
    /// </summary>
    public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(
        nameof(Placement),
        typeof(PlacementMode),
        typeof(Flyout),
        new PropertyMetadata(PlacementMode.Top));

    /// <summary>
    /// Routed event for <see cref="Opened"/>.
    /// </summary>
    public static readonly RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent(
        nameof(Opened),
        RoutingStrategy.Bubble,
        typeof(TypedEventHandler<Flyout, RoutedEventArgs>),
        typeof(Flyout));

    /// <summary>
    /// Routed event for <see cref="Closed"/>.
    /// </summary>
    public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent(
        nameof(Closed),
        RoutingStrategy.Bubble,
        typeof(TypedEventHandler<Flyout, RoutedEventArgs>),
        typeof(Flyout));

    private const string ElementPopup = "PART_Popup";
    private Popup? _popup;
    private CompositeDisposable? _disposables = [];

    /// <summary>
    /// Event triggered when <see cref="Flyout" /> is opened.
    /// </summary>
    public event TypedEventHandler<Flyout, RoutedEventArgs> Opened
    {
        add => AddHandler(OpenedEvent, value);
        remove => RemoveHandler(OpenedEvent, value);
    }

    /// <summary>
    /// Event triggered when <see cref="Flyout" /> is opened.
    /// </summary>
    public event TypedEventHandler<Flyout, RoutedEventArgs> Closed
    {
        add => AddHandler(ClosedEvent, value);
        remove => RemoveHandler(ClosedEvent, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether a <see cref="Flyout" /> is visible.
    /// </summary>
    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the orientation of the <see cref="Flyout" /> control when the control opens,
    /// and specifies the behavior of the <see cref="T:System.Windows.Controls.Primitives.Popup" />
    /// control when it overlaps screen boundaries.
    /// </summary>
    [Bindable(true)]
    [Category("Layout")]
    public PlacementMode Placement
    {
        get => (PlacementMode)GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    /// <summary>
    /// Invoked whenever application code or an internal process,
    /// such as a rebuilding layout pass, calls the ApplyTemplate method.
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _popup = GetTemplateChild(ElementPopup) as Popup;

        if (_popup is null)
        {
            return;
        }

        _disposables?.Dispose();
        _disposables = [];
        _disposables.Add(_popup.Events().Opened.Subscribe(OnPopupOpened));
        _disposables.Add(_popup.Events().Closed.Subscribe(OnPopupClosed));
    }

    /// <summary>
    /// Shows this instance.
    /// </summary>
    public void Show()
    {
        if (!IsOpen)
        {
            SetCurrentValue(IsOpenProperty, true);
        }
    }

    /// <summary>
    /// Hides this instance.
    /// </summary>
    public void Hide()
    {
        if (IsOpen)
        {
            SetCurrentValue(IsOpenProperty, false);
        }
    }

    /// <summary>
    /// Called when [popup opened].
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected virtual void OnPopupOpened(EventArgs e) =>
        RaiseEvent(new RoutedEventArgs(OpenedEvent, this));

    /// <summary>
    /// Called when [popup closed].
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected virtual void OnPopupClosed(EventArgs e)
    {
        Hide();
        RaiseEvent(new RoutedEventArgs(ClosedEvent, this));
    }
}
