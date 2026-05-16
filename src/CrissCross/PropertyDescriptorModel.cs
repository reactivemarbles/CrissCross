// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace CrissCross;

/// <summary>
/// Describes a single AOT-friendly property inspector field without reflection-based discovery.
/// </summary>
public sealed class PropertyDescriptorModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyDescriptorModel"/> class.
    /// </summary>
    /// <param name="key">The stable property key.</param>
    /// <param name="displayName">The user-facing property name.</param>
    /// <param name="category">The category used for grouping.</param>
    /// <param name="editorKind">The editor kind used by platform presenters.</param>
    /// <param name="value">The current property value snapshot.</param>
    /// <param name="originalValue">The original value snapshot used for modified-state calculation.</param>
    /// <param name="isReadOnly">A value indicating whether the property is read-only.</param>
    /// <param name="choices">Optional explicit choices for enum-like editors.</param>
    /// <param name="setValueCommand">An optional command used to set a new value.</param>
    /// <param name="resetCommand">An optional command used to reset the property.</param>
    /// <param name="validationMessages">Optional validation messages for the property.</param>
    /// <param name="templateKey">An optional platform template key for custom editors.</param>
    public PropertyDescriptorModel(
        string key,
        string displayName,
        string? category = null,
        PropertyEditorKind editorKind = PropertyEditorKind.Text,
        object? value = null,
        object? originalValue = null,
        bool isReadOnly = false,
        IReadOnlyList<object?>? choices = null,
        ICommand? setValueCommand = null,
        ICommand? resetCommand = null,
        IReadOnlyList<ValidationMessage>? validationMessages = null,
        string? templateKey = null)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Property key cannot be empty.", nameof(key));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Property display name cannot be empty.", nameof(displayName));
        }

        Key = key.Trim();
        DisplayName = displayName.Trim();
        Category = string.IsNullOrWhiteSpace(category) ? "General" : category!.Trim();
        EditorKind = editorKind;
        Value = value;
        OriginalValue = originalValue;
        IsReadOnly = isReadOnly;
        Choices = choices ?? [];
        SetValueCommand = setValueCommand;
        ResetCommand = resetCommand;
        ValidationMessages = validationMessages ?? [];
        TemplateKey = string.IsNullOrWhiteSpace(templateKey) ? null : templateKey!.Trim();
    }

    /// <summary>
    /// Gets the stable property key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the user-facing property name.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the category used for grouping.
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// Gets the editor kind used by platform presenters.
    /// </summary>
    public PropertyEditorKind EditorKind { get; }

    /// <summary>
    /// Gets the current property value snapshot.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Gets the original value snapshot used for modified-state calculation.
    /// </summary>
    public object? OriginalValue { get; }

    /// <summary>
    /// Gets a value indicating whether the property is read-only.
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    /// Gets the explicit choices for enum-like editors.
    /// </summary>
    public IReadOnlyList<object?> Choices { get; }

    /// <summary>
    /// Gets an optional command used to set a new value.
    /// </summary>
    public ICommand? SetValueCommand { get; }

    /// <summary>
    /// Gets an optional command used to reset the property.
    /// </summary>
    public ICommand? ResetCommand { get; }

    /// <summary>
    /// Gets validation messages for the property.
    /// </summary>
    public IReadOnlyList<ValidationMessage> ValidationMessages { get; }

    /// <summary>
    /// Gets an optional platform template key for custom editors.
    /// </summary>
    public string? TemplateKey { get; }

    /// <summary>
    /// Gets a value indicating whether the descriptor has a value.
    /// </summary>
    public bool HasValue => Value is not null && (Value is not string text || text.Trim().Length > 0);

    /// <summary>
    /// Gets a value indicating whether explicit choices are available.
    /// </summary>
    public bool HasChoices => Choices.Count > 0;

    /// <summary>
    /// Gets a value indicating whether validation messages are present.
    /// </summary>
    public bool HasValidationMessages => ValidationMessages.Count > 0;

    /// <summary>
    /// Gets a value indicating whether any validation message blocks commit.
    /// </summary>
    public bool IsInvalid => ValidationMessages.Any(static message => message.IsBlocking);

    /// <summary>
    /// Gets a value indicating whether any validation message is pending.
    /// </summary>
    public bool IsPending => ValidationMessages.Any(static message => message.Severity == ValidationSeverity.Pending);

    /// <summary>
    /// Gets a value indicating whether the value has changed from the original snapshot.
    /// </summary>
    public bool IsModified => !ValuesEqual(Value, OriginalValue);

    /// <summary>
    /// Gets a value indicating whether the property can be edited.
    /// </summary>
    public bool CanEdit => !IsReadOnly;

    /// <summary>
    /// Gets a value indicating whether reset should be offered.
    /// </summary>
    public bool CanReset => !IsReadOnly && IsModified && ResetCommand is not null;

    /// <summary>
    /// Gets a culture-invariant display string for the current value.
    /// </summary>
    public string ValueDisplayText => FormatValue(Value);

    /// <summary>
    /// Gets a stable key that combines category and property key.
    /// </summary>
    public string CategoryKey => string.Format(CultureInfo.InvariantCulture, "{0}:{1}", Category, Key);

    /// <summary>
    /// Creates a descriptor snapshot with an updated value.
    /// </summary>
    /// <param name="value">The updated value.</param>
    /// <returns>The descriptor snapshot.</returns>
    public PropertyDescriptorModel WithValue(object? value) => new(
            Key,
            DisplayName,
            Category,
            EditorKind,
            value,
            OriginalValue,
            IsReadOnly,
            Choices,
            SetValueCommand,
            ResetCommand,
            ValidationMessages,
            TemplateKey);

    private static bool ValuesEqual(object? left, object? right) => Equals(left, right);

    private static string FormatValue(object? value) => value switch
    {
        null => string.Empty,
        DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
        DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
        bool boolean => boolean ? "True" : "False",
        _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
    };
}
