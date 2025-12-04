// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Work in progress.
/// </summary>
public class TreeGrid : Control
{
    /// <summary>
    /// Property for <see cref="Headers"/>.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<object>> HeadersProperty = AvaloniaProperty.Register<TreeGrid, ObservableCollection<object>>(
        nameof(Headers));

    /// <summary>
    /// Initializes a new instance of the <see cref="TreeGrid"/> class.
    /// </summary>
    public TreeGrid()
    {
    }

    /// <summary>
    /// Gets or sets content is the data used to generate the child elements of this control.
    /// </summary>
    public ObservableCollection<object> Headers
    {
        get => GetValue(HeadersProperty);
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
}
