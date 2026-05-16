// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>
/// Displays a descriptor-driven filter editing surface.
/// </summary>
public class DataFilterPanel : ContentView
{
    /// <summary>
    /// Bindable property for <see cref="FilterPanelState"/>.
    /// </summary>
    public static readonly BindableProperty FilterPanelStateProperty = BindableProperty.Create(
        nameof(FilterPanelState),
        typeof(DataFilterPanelState),
        typeof(DataFilterPanel));

    /// <summary>
    /// Bindable property for <see cref="ApplyFiltersCommand"/>.
    /// </summary>
    public static readonly BindableProperty ApplyFiltersCommandProperty = BindableProperty.Create(
        nameof(ApplyFiltersCommand),
        typeof(ICommand),
        typeof(DataFilterPanel));

    /// <summary>
    /// Gets or sets the shared CrissCross state projected by this control.
    /// </summary>
    public DataFilterPanelState? FilterPanelState
    {
        get => (DataFilterPanelState?)GetValue(FilterPanelStateProperty);
        set => SetValue(FilterPanelStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked by the control surface.
    /// </summary>
    public ICommand? ApplyFiltersCommand
    {
        get => (ICommand?)GetValue(ApplyFiltersCommandProperty);
        set => SetValue(ApplyFiltersCommandProperty, value);
    }
}
