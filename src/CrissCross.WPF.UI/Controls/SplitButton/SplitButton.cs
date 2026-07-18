// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents a button with two parts that can be invoked separately. One part behaves like a standard button
/// and the other part invokes a flyout.</summary>
[TemplatePart(Name = TemplateElementToggleButton, Type = typeof(ToggleButton))]
public class SplitButton : Button, IDisposable
{
    /// <summary>Property for <see cref="Flyout"/>.</summary>
    public static readonly DependencyProperty FlyoutProperty = DependencyProperty.Register(
        nameof(Flyout),
        typeof(object),
        typeof(SplitButton),
        new PropertyMetadata(null, OnFlyoutChangedCallback));

    /// <summary>Property for <see cref="IsDropDownOpen"/>.</summary>
    public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
        nameof(IsDropDownOpen),
        typeof(bool),
        typeof(SplitButton),
        new PropertyMetadata(false, OnIsDropDownOpenChanged));

    /// <summary>Template element represented by the <c>ToggleButton</c> name.</summary>
    private const string TemplateElementToggleButton = "ToggleButton";

    /// <summary>Stores the _contextMenu value.</summary>
    private ContextMenu? _contextMenu;

    /// <summary>Stores the _disposedvalue.</summary>
    private bool _disposedValue;

    /// <summary>Initializes a new instance of the <see cref="SplitButton"/> class.</summary>
    public SplitButton() => Unloaded += OnUnloaded;

    /// <summary>Gets or sets the flyout associated with this button.</summary>
    [Bindable(true)]
    public object? Flyout
    {
        get => GetValue(FlyoutProperty);
        set => SetValue(FlyoutProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether the drop-down for a button is currently open.</summary>
    /// <returns>
    /// <see langword="true" /> if the drop-down is open; otherwise, <see langword="false" />. The default is <see
    /// langword="false" />.</returns>
    [Bindable(true)]
    [Browsable(false)]
    [Category("Appearance")]
    public bool IsDropDownOpen
    {
        get => (bool)GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    /// <summary>Gets or sets the control responsible for toggling the drop-down button.</summary>
    protected ToggleButton SplitButtonToggleButton { get; set; } = null!;

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild(TemplateElementToggleButton) is ToggleButton toggleButton)
        {
            SplitButtonToggleButton = toggleButton;

            SplitButtonToggleButton.Click -= OnSplitButtonToggleButtonOnClick;
            SplitButtonToggleButton.Click += OnSplitButtonToggleButtonOnClick;
        }
        else
        {
            throw new InvalidOperationException(
                $"Element {nameof(TemplateElementToggleButton)} of type {typeof(ToggleButton)} "
                    + $"not found in {typeof(SplitButton)}");
        }
    }

    /// <summary>Provides the Dispose member.</summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Called when [context menu closed].</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnContextMenuClosed(object sender, RoutedEventArgs e) =>
        SetCurrentValue(IsDropDownOpenProperty, false);

    /// <summary>Called when [context menu opened].</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    protected virtual void OnContextMenuOpened(object sender, RoutedEventArgs e) =>
        SetCurrentValue(IsDropDownOpenProperty, true);

    /// <summary>Called when [is drop down open changed].</summary>
    /// <param name="currentValue">if set to <c>true</c> [current value].</param>
    protected virtual void OnIsDropDownOpenChanged(bool currentValue) { }

    /// <summary>Triggered when the control is unloaded. Releases resource bindings.</summary>
    protected virtual void ReleaseTemplateResources() =>
        SplitButtonToggleButton.Click -= OnSplitButtonToggleButtonOnClick;

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
    /// only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            Unloaded -= OnUnloaded;
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        _disposedValue = true;
    }

    /// <summary>Called when [flyout changed callback].</summary>
    /// <param name="value">The value.</param>
    protected virtual void OnFlyoutChangedCallback(object value)
    {
        if (value is not ContextMenu contextMenu)
        {
            return;
        }

        _contextMenu = contextMenu;
        _contextMenu.Opened += OnContextMenuOpened;
        _contextMenu.Closed += OnContextMenuClosed;
    }

    /// <summary>Provides the OnIsDropDownOpenChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not SplitButton dropDownButton)
        {
            return;
        }

        dropDownButton.OnIsDropDownOpenChanged(e.NewValue is bool value && value);
    }

    /// <summary>Provides the OnFlyoutChangedCallback member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void OnFlyoutChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not SplitButton dropDownButton)
        {
            return;
        }

        dropDownButton.OnFlyoutChangedCallback(e.NewValue);
    }

    /// <summary>Provides the OnSplitButtonToggleButtonOnClick member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnSplitButtonToggleButtonOnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton || _contextMenu is null)
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

    /// <summary>Provides the OnUnloaded member.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnUnloaded(object sender, RoutedEventArgs e) => ReleaseTemplateResources();
}
