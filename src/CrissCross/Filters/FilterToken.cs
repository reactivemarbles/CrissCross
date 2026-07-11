// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>Represents a removable or fixed filter chip/token used by search and filter controls.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FilterToken"/> class.
/// </remarks>
/// <param name="field">The stable field key represented by the token.</param>
/// <param name="operator">The comparison operator represented by the token.</param>
/// <param name="value">The field value represented by the token.</param>
/// <param name="displayText">The user-facing text shown for the token.</param>
/// <param name="isRemovable">A value indicating whether the user may remove the token.</param>
/// <param name="icon">Optional icon content or key used by a platform template.</param>
public sealed class FilterToken(
    string field,
    FilterOperator @operator,
    object? value,
    string displayText,
    bool isRemovable = true,
    object? icon = null)
{
    /// <summary>Gets the stable field key represented by the token.</summary>
    public string Field { get; } = field;

    /// <summary>Gets the comparison operator represented by the token.</summary>
    public FilterOperator Operator { get; } = @operator;

    /// <summary>Gets the field value represented by the token.</summary>
    public object? Value { get; } = value;

    /// <summary>Gets the user-facing text shown for the token.</summary>
    public string DisplayText { get; } = displayText;

    /// <summary>Gets a value indicating whether the user may remove the token.</summary>
    public bool IsRemovable { get; } = isRemovable;

    /// <summary>Gets optional icon content or key used by a platform template.</summary>
    public object? Icon { get; } = icon;

    /// <summary>Gets a stable key for equality-aware UI reconciliation.</summary>
    public string Key => string.Format(
        System.Globalization.CultureInfo.InvariantCulture,
        "{0}:{1}:{2}",
        Field,
        Operator,
        Value);
}
