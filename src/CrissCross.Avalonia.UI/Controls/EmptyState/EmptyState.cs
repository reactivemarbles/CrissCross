// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using CrissCross;

namespace CrissCross.Avalonia.UI.Controls;

/// <summary>
/// Presents a consistent no-data, no-results, error, offline, or permission-required state.
/// </summary>
public class EmptyState : ContentControl
{
    /// <summary>
    /// Property for <see cref="Model"/>.
    /// </summary>
    public static readonly StyledProperty<EmptyStateModel?> ModelProperty = AvaloniaProperty.Register<EmptyState, EmptyStateModel?>(
        nameof(Model));

    /// <summary>
    /// Gets or sets the empty-state model.
    /// </summary>
    public EmptyStateModel? Model
    {
        get => GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }
}
