// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CrissCross;

/// <summary>Describes a field that can participate in descriptor-driven data filtering.</summary>
public sealed class FilterDescriptor
{
    /// <summary>Initializes a new instance of the <see cref="FilterDescriptor"/> class.</summary>
    /// <param name="key">The stable field key.</param>
    /// <param name="displayName">The user-facing field name.</param>
    /// <param name="editorKind">The editor kind used by platform presenters.</param>
    /// <param name="supportedOperators">The supported comparison operators.</param>
    /// <param name="choices">The optional explicit choices for enum-like fields.</param>
    /// <param name="defaultValue">The optional default value.</param>
    /// <param name="isRequired">A value indicating whether the filter requires a value before apply.</param>
    public FilterDescriptor(
        string key,
        string displayName,
        FilterEditorKind editorKind,
        IReadOnlyList<FilterOperator>? supportedOperators = null,
        IReadOnlyList<object?>? choices = null,
        object? defaultValue = null,
        bool isRequired = false)
    {
        Key = key;
        DisplayName = displayName;
        EditorKind = editorKind;
        SupportedOperators = supportedOperators is { Count: > 0 } ? supportedOperators : GetDefaultOperators(editorKind);
        Choices = choices;
        DefaultValue = defaultValue;
        IsRequired = isRequired;
    }

    /// <summary>Gets the stable field key.</summary>
    public string Key { get; }

    /// <summary>Gets the user-facing field name.</summary>
    public string DisplayName { get; }

    /// <summary>Gets the editor kind used by platform presenters.</summary>
    public FilterEditorKind EditorKind { get; }

    /// <summary>Gets the supported comparison operators.</summary>
    public IReadOnlyList<FilterOperator> SupportedOperators { get; }

    /// <summary>Gets the optional explicit choices for enum-like fields.</summary>
    public IReadOnlyList<object?>? Choices { get; }

    /// <summary>Gets the optional default value.</summary>
    public object? DefaultValue { get; }

    /// <summary>Gets a value indicating whether the filter requires a value before apply.</summary>
    public bool IsRequired { get; }

    /// <summary>Gets the first supported operator used when callers do not specify one.</summary>
    public FilterOperator DefaultOperator => SupportedOperators[0];

    /// <summary>Gets a value indicating whether explicit choices are available.</summary>
    public bool HasChoices => Choices is { Count: > 0 };

    /// <summary>Determines whether the descriptor supports the specified operator.</summary>
    /// <param name="operator">The operator to test.</param>
    /// <returns><c>true</c> when the operator is supported; otherwise, <c>false</c>.</returns>
    public bool SupportsOperator(FilterOperator @operator) => SupportedOperators.Contains(@operator);

    /// <summary>Creates a filter token for this descriptor.</summary>
    /// <param name="value">The filter value.</param>
    /// <param name="operator">The optional operator; the default operator is used when omitted.</param>
    /// <param name="isRemovable">A value indicating whether the token may be removed.</param>
    /// <returns>The filter token projected from this descriptor.</returns>
    public FilterToken CreateToken(object? value, FilterOperator? @operator = null, bool isRemovable = true)
    {
        var resolvedOperator = @operator ?? DefaultOperator;
        return new FilterToken(Key, resolvedOperator, value, CreateDisplayText(resolvedOperator, value), isRemovable);
    }

    /// <summary>Creates display text for a filter value and operator.</summary>
    /// <param name="operator">The comparison operator.</param>
    /// <param name="value">The filter value.</param>
    /// <returns>The user-facing display text.</returns>
    public string CreateDisplayText(FilterOperator @operator, object? value) => string.Format(
            CultureInfo.InvariantCulture,
            "{0} {1} {2}",
            DisplayName,
            GetOperatorDisplayText(@operator),
            FormatValue(value));

    /// <summary>Gets the default operators for an editor kind.</summary>
    /// <param name="editorKind">The editor kind.</param>
    /// <returns>The default operators.</returns>
    private static IReadOnlyList<FilterOperator> GetDefaultOperators(FilterEditorKind editorKind) => editorKind switch
    {
        FilterEditorKind.Text => [FilterOperator.Contains, FilterOperator.Equals, FilterOperator.StartsWith, FilterOperator.EndsWith],
        FilterEditorKind.Number => [FilterOperator.Equals, FilterOperator.GreaterThan, FilterOperator.GreaterThanOrEqual, FilterOperator.LessThan, FilterOperator.LessThanOrEqual],
        FilterEditorKind.Date or FilterEditorKind.DateTime => [FilterOperator.Equals, FilterOperator.GreaterThanOrEqual, FilterOperator.LessThanOrEqual],
        FilterEditorKind.DateRange => [FilterOperator.Between],
        _ => [FilterOperator.Equals]
    };

    /// <summary>Gets display text for an operator.</summary>
    /// <param name="operator">The operator.</param>
    /// <returns>The display text.</returns>
    private static string GetOperatorDisplayText(FilterOperator @operator) => @operator switch
    {
        FilterOperator.Equals => "equals",
        FilterOperator.NotEquals => "does not equal",
        FilterOperator.Contains => "contains",
        FilterOperator.StartsWith => "starts with",
        FilterOperator.EndsWith => "ends with",
        FilterOperator.GreaterThan => "is greater than",
        FilterOperator.GreaterThanOrEqual => "is greater than or equal to",
        FilterOperator.LessThan => "is less than",
        FilterOperator.LessThanOrEqual => "is less than or equal to",
        FilterOperator.Between => "between",
        _ => @operator.ToString()
    };

    /// <summary>Formats a filter value for display.</summary>
    /// <param name="value">The filter value.</param>
    /// <returns>The formatted value.</returns>
    private static string FormatValue(object? value) => value switch
    {
        null => string.Empty,
        DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
        DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
    };
}
