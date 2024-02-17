// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

using System.Collections.ObjectModel;

// ReSharper disable once CheckNamespace
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
        // Headers changed
    }

    /// <summary>
    /// Called when [content changed].
    /// </summary>
    protected virtual void OnContentChanged()
    {
        // Content changed
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
