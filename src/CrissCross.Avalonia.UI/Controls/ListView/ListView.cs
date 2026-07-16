// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>Represents the ListView type.</summary>
/// <remarks>ListView extends ListBox to provide additional functionality for switching between visual
/// representations of its items. The visual state can be controlled using the ViewState property. This control is
/// commonly used to present collections of data in either a standard list or a grid layout, depending on the
/// application's requirements.</remarks>
public class ListView : global::Avalonia.Controls.ListBox
{
    /// <summary>Dependency property backing <see cref="ViewState"/>.</summary>
    public static readonly StyledProperty<ListViewViewState> ViewStateProperty = AvaloniaProperty.Register<
        ListView,
        ListViewViewState
    >(nameof(ViewState), ListViewViewState.Default);

    /// <summary>Gets or sets the current visual state of the list (Default or GridView).</summary>
    public ListViewViewState ViewState
    {
        get => GetValue(ViewStateProperty);
        set => SetValue(ViewStateProperty, value);
    }
}
