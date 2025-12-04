// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Extends <see cref="Avalonia.Controls.ListBox"/>.
/// </summary>
public class ListView : global::Avalonia.Controls.ListBox
{
    /// <summary>
    /// Dependency property backing <see cref="ViewState"/>.
    /// </summary>
    public static readonly StyledProperty<ListViewViewState> ViewStateProperty = AvaloniaProperty.Register<ListView, ListViewViewState>(
        nameof(ViewState), ListViewViewState.Default);

    /// <summary>
    /// Gets or sets the current visual state of the list (Default or GridView).
    /// </summary>
    public ListViewViewState ViewState
    {
        get => GetValue(ViewStateProperty);
        set => SetValue(ViewStateProperty, value);
    }
}
