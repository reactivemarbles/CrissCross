// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.Primitives;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI.Controls;
#else
namespace CrissCross.Avalonia.UI.Controls;
#endif

/// <summary>Work in progress.</summary>
public class TreeGrid : TemplatedControl
{
    /// <summary>Property for <see cref="Headers"/>.</summary>
    public static readonly StyledProperty<ObservableCollection<object>> HeadersProperty = AvaloniaProperty.Register<
        TreeGrid,
        ObservableCollection<object>
    >(nameof(Headers));

    /// <summary>Initializes a new instance of the <see cref="TreeGrid"/> class.</summary>
    public TreeGrid()
    {
        _ = SetValue(HeadersProperty, new ObservableCollection<object>());
    }

    /// <summary>Gets content used to generate the child elements of this control.</summary>
    public ObservableCollection<object> Headers
    {
        get => GetValue(HeadersProperty);
    }

    /// <summary>Called when [headers changed].</summary>
    protected virtual void OnHeadersChanged() { }

    /// <summary>Called when [content changed].</summary>
    protected virtual void OnContentChanged() { }
}
