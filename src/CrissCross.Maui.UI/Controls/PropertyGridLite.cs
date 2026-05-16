// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Maui.UI.Controls;

/// <summary>
/// Displays categorized editable property descriptors without reflection-heavy discovery.
/// </summary>
public class PropertyGridLite : ContentView
{
    /// <summary>
    /// Bindable property for <see cref="PropertyGridState"/>.
    /// </summary>
    public static readonly BindableProperty PropertyGridStateProperty = BindableProperty.Create(
        nameof(PropertyGridState),
        typeof(PropertyGridState),
        typeof(PropertyGridLite));

    /// <summary>
    /// Bindable property for <see cref="UpdatePropertyCommand"/>.
    /// </summary>
    public static readonly BindableProperty UpdatePropertyCommandProperty = BindableProperty.Create(
        nameof(UpdatePropertyCommand),
        typeof(ICommand),
        typeof(PropertyGridLite));

    /// <summary>
    /// Gets or sets the shared CrissCross state projected by this control.
    /// </summary>
    public PropertyGridState? PropertyGridState
    {
        get => (PropertyGridState?)GetValue(PropertyGridStateProperty);
        set => SetValue(PropertyGridStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the command invoked by the control surface.
    /// </summary>
    public ICommand? UpdatePropertyCommand
    {
        get => (ICommand?)GetValue(UpdatePropertyCommandProperty);
        set => SetValue(UpdatePropertyCommandProperty, value);
    }
}
