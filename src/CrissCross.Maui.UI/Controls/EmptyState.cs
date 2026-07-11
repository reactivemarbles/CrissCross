// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>Displays an empty, no-results, or error state with optional action commands.</summary>
public class EmptyState : ContentView
{
    /// <summary>Bindable property for <see cref="Model"/>.</summary>
    public static readonly BindableProperty ModelProperty = BindableProperty.Create(
        nameof(Model),
        typeof(EmptyStateModel),
        typeof(EmptyState));

    /// <summary>Bindable property for <see cref="PrimaryCommand"/>.</summary>
    public static readonly BindableProperty PrimaryCommandProperty = BindableProperty.Create(
        nameof(PrimaryCommand),
        typeof(ICommand),
        typeof(EmptyState));

    /// <summary>Gets or sets the shared CrissCross state projected by this control.</summary>
    public EmptyStateModel? Model
    {
        get => (EmptyStateModel?)GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }

    /// <summary>Gets or sets the command invoked by the control surface.</summary>
    public ICommand? PrimaryCommand
    {
        get => (ICommand?)GetValue(PrimaryCommandProperty);
        set => SetValue(PrimaryCommandProperty, value);
    }
}
