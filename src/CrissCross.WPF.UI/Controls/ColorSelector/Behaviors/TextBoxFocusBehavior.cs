// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace CrissCross.WPF.UI.Behaviors;

/// <summary>Provides the TextBoxFocusBehavior member.</summary>
internal sealed class TextBoxFocusBehavior : Behavior<TextBox>
{
    /// <summary>Provides the SelectOnMouseClickProperty member.</summary>
    public static readonly DependencyProperty SelectOnMouseClickProperty = DependencyProperty.Register(
        nameof(SelectOnMouseClick),
        typeof(bool),
        typeof(TextBoxFocusBehavior),
        new PropertyMetadata(true));

    /// <summary>Provides the ConfirmOnEnterProperty member.</summary>
    public static readonly DependencyProperty ConfirmOnEnterProperty = DependencyProperty.Register(
        nameof(ConfirmOnEnter),
        typeof(bool),
        typeof(TextBoxFocusBehavior),
        new PropertyMetadata(true));

    /// <summary>Provides the DeselectOnFocusLossProperty member.</summary>
    public static readonly DependencyProperty DeselectOnFocusLossProperty = DependencyProperty.Register(
        nameof(DeselectOnFocusLoss),
        typeof(bool),
        typeof(TextBoxFocusBehavior),
        new PropertyMetadata(true));

    /// <summary>Gets or sets SelectOnMouseClick.</summary>
    public bool SelectOnMouseClick
    {
        get => (bool)GetValue(SelectOnMouseClickProperty);
        set => SetValue(SelectOnMouseClickProperty, value);
    }

    /// <summary>Gets or sets ConfirmOnEnter.</summary>
    public bool ConfirmOnEnter
    {
        get => (bool)GetValue(ConfirmOnEnterProperty);
        set => SetValue(ConfirmOnEnterProperty, value);
    }

    /// <summary>Gets or sets DeselectOnFocusLoss.</summary>
    public bool DeselectOnFocusLoss
    {
        get => (bool)GetValue(DeselectOnFocusLossProperty);
        set => SetValue(DeselectOnFocusLossProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.GotKeyboardFocus += AssociatedObjectGotKeyboardFocus;
        AssociatedObject.GotMouseCapture += AssociatedObjectGotMouseCapture;
        AssociatedObject.LostFocus += AssociatedObject_LostFocus;
        AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObjectPreviewMouseLeftButtonDown;
        AssociatedObject.KeyUp += AssociatedObject_KeyUp;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.GotKeyboardFocus -= AssociatedObjectGotKeyboardFocus;
        AssociatedObject.GotMouseCapture -= AssociatedObjectGotMouseCapture;
        AssociatedObject.LostFocus -= AssociatedObject_LostFocus;
        AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObjectPreviewMouseLeftButtonDown;
        AssociatedObject.KeyUp -= AssociatedObject_KeyUp;
    }

    /// <summary>Converts number to proper format if enter is clicked and moves focus to next object.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void AssociatedObject_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter || !ConfirmOnEnter)
        {
            return;
        }

        RemoveFocus();
    }

    /// <summary>Provides the RemoveFocus member.</summary>
    private void RemoveFocus()
    {
        var scope = FocusManager.GetFocusScope(AssociatedObject);
        var parent = (FrameworkElement)AssociatedObject.Parent;

        while (parent is not null && parent is IInputElement element && !element.Focusable)
        {
            parent = (FrameworkElement)parent.Parent;
        }

        FocusManager.SetFocusedElement(scope, parent);
        Keyboard.ClearFocus();
    }

    /// <summary>Provides the AssociatedObjectGotKeyboardFocus member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void AssociatedObjectGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (!SelectOnMouseClick && !e.KeyboardDevice.IsKeyDown(Key.Tab))
        {
            return;
        }

        AssociatedObject.SelectAll();
    }

    /// <summary>Provides the AssociatedObjectGotMouseCapture member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void AssociatedObjectGotMouseCapture(object sender, MouseEventArgs e)
    {
        if (!SelectOnMouseClick)
        {
            return;
        }

        AssociatedObject.SelectAll();
    }

    /// <summary>Provides the AssociatedObject_LostFocus member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void AssociatedObject_LostFocus(object sender, RoutedEventArgs e)
    {
        if (!DeselectOnFocusLoss)
        {
            return;
        }

        AssociatedObject.Select(0, 0);
    }

    /// <summary>Provides the AssociatedObjectPreviewMouseLeftButtonDown member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void AssociatedObjectPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!SelectOnMouseClick)
        {
            return;
        }

        if (AssociatedObject.IsKeyboardFocusWithin)
        {
            return;
        }

        _ = AssociatedObject.Focus();
        e.Handled = true;
    }
}
