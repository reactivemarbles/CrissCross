// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Extended TreeViewItem with SymbolRegular properties.</summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class TreeViewItem : System.Windows.Controls.TreeViewItem
{
    /// <summary>Property for <see cref="Icon"/>.</summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(TreeViewItem),
        new(null, IconPropertyChanged));

    /// <summary>
    /// Identifies the IconVisibility dependency property, which determines whether the icon associated with the
    /// TreeViewItem is visible.
    /// </summary>
    /// <remarks>The IconVisibilityProperty is used to control the display state of the icon within a
    /// TreeViewItem. By default, the icon is collapsed. Setting this property to <see cref="Visibility.Visible"/> will
    /// display the icon, while <see cref="Visibility.Collapsed"/> or <see cref="Visibility.Hidden"/> will hide it. This
    /// property can be used in styles, templates, or code to customize the appearance of TreeViewItem
    /// controls.</remarks>
    public static readonly DependencyProperty IconVisibilityProperty = DependencyProperty.Register(
        nameof(IconVisibility),
        typeof(Visibility),
        typeof(TreeViewItem),
        new(Visibility.Collapsed));

    /// <summary>Gets or sets displayed <see cref="IconElement"/>.</summary>
    [Bindable(true)]
    [Category("Appearance")]
    public IconElement? Icon
    {
        get => (IconElement?)GetValue(IconProperty);
        set
        {
            SetValue(IconVisibilityProperty, value is null ? Visibility.Collapsed : Visibility.Visible);
            SetValue(IconProperty, value);
        }
    }

    /// <summary>Gets or sets the visibility of the icon associated with the control.</summary>
    /// <remarks>A value of <see cref="Visibility.Visible"/> displays the icon, while <see
    /// cref="Visibility.Collapsed"/> hides it. Changing this property may affect the layout of the control.</remarks>
    public Visibility IconVisibility
    {
        get => (Visibility)GetValue(IconVisibilityProperty);
        set => SetValue(IconVisibilityProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <inheritdoc/>
    protected override DependencyObject GetContainerForItemOverride() => new TreeViewItem();

    /// <inheritdoc/>
    protected override bool IsItemItsOwnContainerOverride(object item) => item is TreeViewItem;

    /// <summary>Provides the IconPropertyChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void IconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TreeViewItem item)
        {
            return;
        }

        item.IconVisibility = e.NewValue is null ? Visibility.Collapsed : Visibility.Visible;
    }
}
