// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// A control that drop downs a flyout of choices from which one can be chosen.
/// </summary>
public class DropDownButton : Button
{
    /// <summary>
    /// Property for <see cref="Flyout"/>.
    /// </summary>
    public static readonly DependencyProperty FlyoutProperty = DependencyProperty.Register(
        nameof(Flyout),
        typeof(object),
        typeof(DropDownButton),
        new PropertyMetadata(null, OnFlyoutChangedCallback));

    /// <summary>
    /// Property for <see cref="IsDropDownOpen"/>.
    /// </summary>
    public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
        nameof(IsDropDownOpen),
        typeof(bool),
        typeof(DropDownButton),
        new PropertyMetadata(false));

    private ContextMenu? _contextMenu;

    /// <summary>
    /// Gets or sets the flyout associated with this button.
    /// </summary>
    [Bindable(true)]
    public object? Flyout
    {
        get => GetValue(FlyoutProperty);
        set => SetValue(FlyoutProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the drop-down for a button is currently open.
    /// </summary>
    /// <returns>
    /// <see langword="true" /> if the drop-down is open; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
    [Bindable(true)]
    [Browsable(false)]
    [Category("Appearance")]
    public bool IsDropDownOpen
    {
        get => (bool)GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    /// <summary>
    /// Called when [flyout changed callback].
    /// </summary>
    /// <param name="value">The value.</param>
    protected virtual void OnFlyoutChangedCallback(object value)
    {
        if (value is ContextMenu contextMenu)
        {
            _contextMenu = contextMenu;
            _contextMenu.Opened += OnContextMenuOpened;
            _contextMenu.Closed += OnContextMenuClosed;
        }
    }

    /// <summary>
    /// Called when [context menu closed].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnContextMenuClosed(object sender, RoutedEventArgs e) =>
        SetCurrentValue(IsDropDownOpenProperty, false);

    /// <summary>
    /// Called when [context menu opened].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnContextMenuOpened(object sender, RoutedEventArgs e) =>
        SetCurrentValue(IsDropDownOpenProperty, true);

    /// <summary>
    /// Called when [is drop down open changed].
    /// </summary>
    /// <param name="currentValue">if set to <c>true</c> [current value].</param>
    protected virtual void OnIsDropDownOpenChanged(bool currentValue)
    {
    }

    /// <summary>
    /// Called when a <see cref="T:System.Windows.Controls.Button" /> is clicked.
    /// </summary>
    protected override void OnClick()
    {
        base.OnClick();

        if (_contextMenu is null)
        {
            return;
        }

        _contextMenu.SetCurrentValue(MinWidthProperty, ActualWidth);
        _contextMenu.SetCurrentValue(ContextMenu.PlacementTargetProperty, this);
        _contextMenu.SetCurrentValue(
            ContextMenu.PlacementProperty,
            System.Windows.Controls.Primitives.PlacementMode.Bottom);
        _contextMenu.SetCurrentValue(ContextMenu.IsOpenProperty, true);
    }

    private static void OnFlyoutChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DropDownButton dropDownButton)
        {
            dropDownButton.OnFlyoutChangedCallback(e.NewValue);
        }
    }
}
