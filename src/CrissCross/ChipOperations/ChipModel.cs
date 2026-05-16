// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace CrissCross;

/// <summary>
/// Represents a compact selectable or removable chip/tag item.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ChipModel"/> class.
/// </remarks>
/// <param name="key">The stable chip key used for selection and reconciliation.</param>
/// <param name="text">The user-facing chip text.</param>
/// <param name="isSelected">A value indicating whether the chip is selected.</param>
/// <param name="isRemovable">A value indicating whether the chip exposes a remove action.</param>
/// <param name="isEnabled">A value indicating whether the chip can be interacted with.</param>
/// <param name="icon">Optional icon content or icon key used by platform templates.</param>
/// <param name="removeCommand">Optional command invoked to remove the chip.</param>
/// <param name="selectCommand">Optional command invoked to select or toggle the chip.</param>
public sealed class ChipModel(
    string key,
    string text,
    bool isSelected = false,
    bool isRemovable = false,
    bool isEnabled = true,
    object? icon = null,
    ICommand? removeCommand = null,
    ICommand? selectCommand = null)
{
    /// <summary>
    /// Gets the stable chip key used for selection and reconciliation.
    /// </summary>
    public string Key { get; } = key;

    /// <summary>
    /// Gets the user-facing chip text.
    /// </summary>
    public string Text { get; } = text;

    /// <summary>
    /// Gets a value indicating whether the chip is selected.
    /// </summary>
    public bool IsSelected { get; } = isSelected;

    /// <summary>
    /// Gets a value indicating whether the chip exposes a remove action.
    /// </summary>
    public bool IsRemovable { get; } = isRemovable;

    /// <summary>
    /// Gets a value indicating whether the chip can be interacted with.
    /// </summary>
    public bool IsEnabled { get; } = isEnabled;

    /// <summary>
    /// Gets optional icon content or icon key used by platform templates.
    /// </summary>
    public object? Icon { get; } = icon;

    /// <summary>
    /// Gets optional command invoked to remove the chip.
    /// </summary>
    public ICommand? RemoveCommand { get; } = removeCommand;

    /// <summary>
    /// Gets optional command invoked to select or toggle the chip.
    /// </summary>
    public ICommand? SelectCommand { get; } = selectCommand;

    /// <summary>
    /// Gets a value indicating whether the chip has icon content.
    /// </summary>
    public bool HasIcon => Icon is not null;

    /// <summary>
    /// Gets a value indicating whether the chip can be selected or removed.
    /// </summary>
    public bool IsInteractive => IsEnabled && (SelectCommand is not null || CanRemove || IsSelected);

    /// <summary>
    /// Gets a value indicating whether the chip can be removed by the user.
    /// </summary>
    public bool CanRemove => IsEnabled && IsRemovable && RemoveCommand is not null;
}
