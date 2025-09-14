// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Gallery.ViewModels;

/// <summary>
/// Represents a single control sample entry in the gallery.
/// </summary>
public sealed class ControlItem
{
    /// <summary>Gets the name of the control.</summary>
    public required string Name { get; init; }

    /// <summary>Gets icon path (pack relative) for the control.</summary>
    public required string Icon { get; init; }

    /// <summary>Gets command executed to navigate to the sample.</summary>
    public required System.Windows.Input.ICommand Command { get; init; }

    /// <summary>Gets short description.</summary>
    public string? Description { get; init; }
}
