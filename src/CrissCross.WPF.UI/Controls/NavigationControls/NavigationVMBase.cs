// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Controls;
#else
namespace CrissCross.WPF.UI.Controls;
#endif

/// <summary>Represents NavigationVMBase.</summary>
/// <seealso cref="System.Windows.Controls.Control" />
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class NavigationVMBase : System.Windows.Controls.Control
{
    /// <summary>The items source property.</summary>
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(IEnumerable<NavigationModel>),
        typeof(NavigationVMBase),
        new(null, ItemsSourceChanged));

    /// <summary>The is expanded property.</summary>
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded),
        typeof(bool),
        typeof(NavigationVMBase),
        new(true));

    /// <summary>The filter property.</summary>
    public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
        nameof(Filter),
        typeof(string),
        typeof(NavigationVMBase),
        new(string.Empty, FilterChanged));

    /// <summary>Gets or sets the items source.</summary>
    /// <value>
    /// The items source.
    /// </value>
    [Bindable(true)]
    [Category("Content")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IEnumerable<NavigationModel> ItemsSource
    {
        get => (IEnumerable<NavigationModel>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether this instance is expanded.</summary>
    /// <value>
    ///   <c>true</c> if this instance is expanded; otherwise, <c>false</c>.
    /// </value>
    [Bindable(true)]
    [Category("Control")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>Gets or sets the filter.</summary>
    /// <value>
    /// The filter.
    /// </value>
    [Bindable(true)]
    [Category("Control")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Filter
    {
        get => (string?)GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    /// <summary>Gets a debugger-friendly textual representation of this instance.</summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => ToString() ?? GetType().Name;

    /// <summary>Provides the FilterChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void FilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NavigationVMBase navigation || navigation.ItemsSource is null || e.NewValue is not string filter)
        {
            return;
        }

        // Get items from the ItemsSource that have a Name that contains text from the filter.
        // Reset visibility.
        foreach (var item in navigation.ItemsSource)
        {
            item.Visibility = !string.IsNullOrEmpty(item.Name) && !item.Name.Contains(filter)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }

    /// <summary>Provides the ItemsSourceChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="e">The event arguments.</param>
    private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NavigationVMBase navigation || e.NewValue is not IEnumerable<NavigationModel>)
        {
            return;
        }

        if (string.IsNullOrEmpty(navigation.Filter))
        {
            return;
        }

        // Get items from the ItemsSource that have a Name that contains text from the filter.
        // Reset visibility.
        foreach (var item in navigation.ItemsSource)
        {
            item.Visibility = !string.IsNullOrEmpty(item.Name) && !item.Name.Contains(navigation.Filter)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}
