// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace CrissCross.WPF.UI.Controls;

/// <summary>Work in progress.</summary>
public class TreeGrid : System.Windows.Controls.Primitives.Selector
{
    /// <summary>Property for <see cref="Headers"/>.</summary>
    public static readonly DependencyProperty HeadersProperty = DependencyProperty.Register(
        nameof(Headers),
        typeof(ObservableCollection<TreeGridHeader>),
        typeof(TreeGrid),
        new PropertyMetadata(new ObservableCollection<TreeGridHeader>(), OnHeadersChanged));

    /// <summary>Gets or sets content is the data used to generate the child elements of this control.</summary>
    [Bindable(true)]
    public ObservableCollection<TreeGridHeader> Headers
    {
        get => (GetValue(HeadersProperty) as ObservableCollection<TreeGridHeader>)!;
        set => SetValue(HeadersProperty, value);
    }

    /// <summary>Called when [headers changed].</summary>
    protected virtual void OnHeadersChanged()
    {
    }

    /// <summary>Called when [content changed].</summary>
    protected virtual void OnContentChanged()
    {
    }

    /// <summary>Provides the OnHeadersChanged member.</summary>
    /// <param name="d">The d value.</param>
    /// <param name="_">Unused event arguments required by the dependency property callback.</param>
    private static void OnHeadersChanged(DependencyObject d, DependencyPropertyChangedEventArgs _)
    {
        if (d is not TreeGrid treeGrid)
        {
            return;
        }

        treeGrid.OnHeadersChanged();
    }
}
