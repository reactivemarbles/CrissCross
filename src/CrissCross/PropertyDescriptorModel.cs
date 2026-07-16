// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CrissCross;

/// <summary>Describes a single AOT-friendly property inspector field without reflection-based discovery.</summary>
public sealed class PropertyDescriptorModel
{
    /// <inheritdoc />
    public PropertyDescriptorModel(string key, string displayName)
        : this(key, displayName, new PropertyDescriptorOptions())
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PropertyDescriptorModel"/> class.</summary>
    /// <param name="key">The stable property key.</param>
    /// <param name="displayName">The user-facing property name.</param>
    /// <param name="options">The editing, value, command, and validation options.</param>
    public PropertyDescriptorModel(
        string key,
        string displayName,
        PropertyDescriptorOptions options)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(key, nameof(key));
        ThrowHelper.ThrowIfNullOrWhiteSpace(displayName, nameof(displayName));
        ThrowHelper.ThrowIfNull(options, nameof(options));

        Key = key.Trim();
        DisplayName = displayName.Trim();
        Category = string.IsNullOrWhiteSpace(options.Category) ? "General" : options.Category!.Trim();
        EditorKind = options.EditorKind;
        Value = options.Value;
        OriginalValue = options.OriginalValue;
        IsReadOnly = options.IsReadOnly;
        Choices = options.Choices ?? [];
        SetValueCommand = options.SetValueCommand;
        ResetCommand = options.ResetCommand;
        ValidationMessages = options.ValidationMessages ?? [];
        TemplateKey = string.IsNullOrWhiteSpace(options.TemplateKey) ? null : options.TemplateKey!.Trim();
    }

    /// <summary>Gets the stable property key.</summary>
    public string Key { get; }

    /// <summary>Gets the user-facing property name.</summary>
    public string DisplayName { get; }

    /// <summary>Gets the category used for grouping.</summary>
    public string Category { get; }

    /// <summary>Gets the editor kind used by platform presenters.</summary>
    public PropertyEditorKind EditorKind { get; }

    /// <summary>Gets the current property value snapshot.</summary>
    public object? Value { get; }

    /// <summary>Gets the original value snapshot used for modified-state calculation.</summary>
    public object? OriginalValue { get; }

    /// <summary>Gets a value indicating whether the property is read-only.</summary>
    public bool IsReadOnly { get; }

    /// <summary>Gets the explicit choices for enum-like editors.</summary>
    public IReadOnlyList<object?> Choices { get; }

    /// <summary>Gets an optional command used to set a new value.</summary>
    public System.Windows.Input.ICommand? SetValueCommand { get; }

    /// <summary>Gets an optional command used to reset the property.</summary>
    public System.Windows.Input.ICommand? ResetCommand { get; }

    /// <summary>Gets validation messages for the property.</summary>
    public IReadOnlyList<ValidationMessage> ValidationMessages { get; }

    /// <summary>Gets an optional platform template key for custom editors.</summary>
    public string? TemplateKey { get; }

    /// <summary>Gets a value indicating whether the descriptor has a value.</summary>
    public bool HasValue => Value is not null && (Value is not string text || text.Trim().Length > 0);

    /// <summary>Gets a value indicating whether explicit choices are available.</summary>
    public bool HasChoices => Choices.Count > 0;

    /// <summary>Gets a value indicating whether validation messages are present.</summary>
    public bool HasValidationMessages => ValidationMessages.Count > 0;

    /// <summary>Gets a value indicating whether any validation message blocks commit.</summary>
    public bool IsInvalid => ValidationMessages.Any(static message => message.IsBlocking);

    /// <summary>Gets a value indicating whether any validation message is pending.</summary>
    public bool IsPending => ValidationMessages.Any(static message => message.Severity == ValidationSeverity.Pending);

    /// <summary>Gets a value indicating whether the value has changed from the original snapshot.</summary>
    public bool IsModified => !ValuesEqual(Value, OriginalValue);

    /// <summary>Gets a value indicating whether the property can be edited.</summary>
    public bool CanEdit => !IsReadOnly;

    /// <summary>Gets a value indicating whether reset should be offered.</summary>
    public bool CanReset => !IsReadOnly && IsModified && ResetCommand is not null;

    /// <summary>Gets a culture-invariant display string for the current value.</summary>
    public string ValueDisplayText => FormatValue(Value);

    /// <summary>Gets a stable key that combines category and property key.</summary>
    public string CategoryKey => string.Format(CultureInfo.InvariantCulture, "{0}:{1}", Category, Key);

    /// <summary>Creates a descriptor snapshot with an updated value.</summary>
    /// <param name="value">The updated value.</param>
    /// <returns>The descriptor snapshot.</returns>
    public PropertyDescriptorModel WithValue(object? value)
    {
        var currentCategory = Category;
        var currentEditorKind = EditorKind;
        var currentOriginalValue = OriginalValue;
        var currentIsReadOnly = IsReadOnly;
        var currentChoices = Choices;
        var currentSetValueCommand = SetValueCommand;
        var currentResetCommand = ResetCommand;
        var currentValidationMessages = ValidationMessages;
        var currentTemplateKey = TemplateKey;

        return new(
            Key,
            DisplayName,
            new PropertyDescriptorOptions
            {
                Category = currentCategory,
                EditorKind = currentEditorKind,
                Value = value,
                OriginalValue = currentOriginalValue,
                IsReadOnly = currentIsReadOnly,
                Choices = currentChoices,
                SetValueCommand = currentSetValueCommand,
                ResetCommand = currentResetCommand,
                ValidationMessages = currentValidationMessages,
                TemplateKey = currentTemplateKey
            });
    }

    /// <summary>Compares two property values for equality.</summary>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    /// <returns><c>true</c> when the values are equal; otherwise, <c>false</c>.</returns>
    private static bool ValuesEqual(object? left, object? right) => Equals(left, right);

    /// <summary>Formats a property value for display.</summary>
    /// <param name="value">The property value.</param>
    /// <returns>The formatted value.</returns>
    private static string FormatValue(object? value) => value switch
    {
        null => string.Empty,
        DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
        DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
        bool boolean => boolean ? "True" : "False",
        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
    };
}
