// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// NavigationVMBase.
/// </summary>
/// <seealso cref="System.Windows.Controls.Control" />
public class NavigationVMBase : System.Windows.Controls.Control
{
    /// <summary>
    /// The items source property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(IEnumerable<NavigationModel>),
            typeof(NavigationVMBase),
            new PropertyMetadata(null, ItemsSourceChanged));

    /// <summary>
    /// The is expanded property.
    /// </summary>
    public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register(
            nameof(IsExpanded),
            typeof(bool),
            typeof(NavigationVMBase),
            new PropertyMetadata(true));

    /// <summary>
    /// The filter property.
    /// </summary>
    public static readonly DependencyProperty FilterProperty =
        DependencyProperty.Register(
            nameof(Filter),
            typeof(string),
            typeof(NavigationVMBase),
            new PropertyMetadata(string.Empty, FilterChanged));

    /// <summary>
    /// Gets or sets the items source.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether this instance is expanded.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the filter.
    /// </summary>
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

    private static void FilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationVMBase navigation && navigation.ItemsSource != null && e.NewValue is string filter)
        {
            // Get items from the ItemsSource that have a Name that contains text from the filter.
            var items = navigation.ItemsSource.Where(x => !string.IsNullOrEmpty(x.Name) && !x.Name.Contains(filter));

            // Reset visibility.
            foreach (var item in navigation.ItemsSource)
            {
                item.Visibility = Visibility.Visible;
            }

            // Hide items that do not match the filter.
            foreach (var item in items)
            {
                item.Visibility = Visibility.Collapsed;
            }
        }
    }

    private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NavigationVMBase navigation && e.NewValue is IEnumerable<NavigationModel> itemsSource)
        {
            if (string.IsNullOrEmpty(navigation.Filter))
            {
                return;
            }

            // Get items from the ItemsSource that have a Name that contains text from the filter.
            var items = itemsSource.Where(x => !string.IsNullOrEmpty(x.Name) && !x.Name.Contains(navigation.Filter));

            // Reset visibility.
            foreach (var item in navigation.ItemsSource)
            {
                item.Visibility = Visibility.Visible;
            }

            // Hide items that do not match the filter.
            foreach (var item in items)
            {
                item.Visibility = Visibility.Collapsed;
            }
        }
    }
}
