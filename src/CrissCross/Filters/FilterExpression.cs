// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Represents a concrete field/operator/value filter expression.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FilterExpression"/> class.
/// </remarks>
/// <param name="fieldKey">The stable field key.</param>
/// <param name="operator">The comparison operator.</param>
/// <param name="value">The comparison value.</param>
/// <param name="displayName">The optional field display name override.</param>
/// <param name="isEnabled">A value indicating whether the expression participates in filtering.</param>
public sealed class FilterExpression(
    string fieldKey,
    FilterOperator @operator,
    object? value,
    string? displayName = null,
    bool isEnabled = true)
{
    /// <summary>Formats a stable filter expression key.</summary>
#if NET8_0_OR_GREATER
    private static readonly System.Text.CompositeFormat KeyFormat = System.Text.CompositeFormat.Parse("{0}:{1}:{2}");
#else
    private const string KeyFormat = "{0}:{1}:{2}";
#endif

    /// <summary>Formats filter expression display text.</summary>
#if NET8_0_OR_GREATER
    private static readonly System.Text.CompositeFormat DisplayTextFormat = System.Text.CompositeFormat.Parse("{0} {1} {2}");
#else
    private const string DisplayTextFormat = "{0} {1} {2}";
#endif

    /// <summary>Gets the stable field key.</summary>
    public string FieldKey { get; } = fieldKey;

    /// <summary>Gets the comparison operator.</summary>
    public FilterOperator Operator { get; } = @operator;

    /// <summary>Gets the comparison value.</summary>
    public object? Value { get; } = value;

    /// <summary>Gets the optional field display name override.</summary>
    public string? DisplayName { get; } = displayName;

    /// <summary>Gets a value indicating whether the expression participates in filtering.</summary>
    public bool IsEnabled { get; } = isEnabled;

    /// <summary>Gets a value indicating whether this expression has a meaningful filter value.</summary>
    public bool IsActive => IsEnabled && Value switch
    {
        null => false,
        string text => text.Trim().Length > 0,
        _ => true
    };

    /// <summary>Gets a stable expression key for reconciliation and saved-filter persistence.</summary>
    public string Key => string.Format(CultureInfo.InvariantCulture, KeyFormat, FieldKey, Operator, Value);

    /// <summary>Creates a token for this expression without descriptor metadata.</summary>
    /// <returns>The projected filter token.</returns>
    public FilterToken ToToken() => ToToken(null);

    /// <summary>Creates a token for this expression using the supplied descriptor when available.</summary>
    /// <param name="descriptor">The optional descriptor for display metadata.</param>
    /// <returns>The projected filter token.</returns>
    public FilterToken ToToken(FilterDescriptor? descriptor)
    {
        if (descriptor is not null)
        {
            return descriptor.CreateToken(Value, Operator);
        }

        var displayName = string.IsNullOrWhiteSpace(DisplayName) ? FieldKey : DisplayName;
        var displayText = string.Format(CultureInfo.InvariantCulture, DisplayTextFormat, displayName, Operator, Value);
        return new(FieldKey, Operator, Value, displayText);
    }
}
