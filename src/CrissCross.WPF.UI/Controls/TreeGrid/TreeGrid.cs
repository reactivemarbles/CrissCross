// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Work in progress.
/// </summary>
public class TreeGrid : System.Windows.Controls.Primitives.Selector
{
    /// <summary>
    /// Property for <see cref="Headers"/>.
    /// </summary>
    public static readonly DependencyProperty HeadersProperty = DependencyProperty.Register(
        nameof(Headers),
        typeof(ObservableCollection<TreeGridHeader>),
        typeof(TreeGrid),
        new PropertyMetadata(new ObservableCollection<TreeGridHeader>(), OnHeadersChanged));

    /// <summary>
    /// Initializes a new instance of the <see cref="TreeGrid"/> class.
    /// </summary>
    public TreeGrid()
    {
    }

    /// <summary>
    /// Gets or sets content is the data used to generate the child elements of this control.
    /// </summary>
    [Bindable(true)]
    public ObservableCollection<TreeGridHeader> Headers
    {
        get => (GetValue(HeadersProperty) as ObservableCollection<TreeGridHeader>)!;
        set => SetValue(HeadersProperty, value);
    }

    /// <summary>
    /// Called when [headers changed].
    /// </summary>
    protected virtual void OnHeadersChanged()
    {
    }

    /// <summary>
    /// Called when [content changed].
    /// </summary>
    protected virtual void OnContentChanged()
    {
    }

    private static void OnHeadersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TreeGrid treeGrid)
        {
            return;
        }

        treeGrid.OnHeadersChanged();
    }

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TreeGrid treeGrid)
        {
            return;
        }

        treeGrid.OnContentChanged();
    }
}
