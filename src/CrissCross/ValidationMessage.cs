// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows.Input;

namespace CrissCross;

/// <summary>
/// Immutable validation feedback for a named form field or form-level rule.
/// </summary>
public sealed class ValidationMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationMessage"/> class.
    /// </summary>
    /// <param name="fieldKey">The stable field key associated with the message.</param>
    /// <param name="fieldDisplayName">The human-readable field display name.</param>
    /// <param name="message">The validation message text.</param>
    /// <param name="severity">The message severity.</param>
    /// <param name="remediationCommand">An optional command that remediates or focuses the issue.</param>
    public ValidationMessage(
        string? fieldKey,
        string? fieldDisplayName,
        string message,
        ValidationSeverity severity = ValidationSeverity.Error,
        ICommand? remediationCommand = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Validation message text cannot be empty.", nameof(message));
        }

        FieldKey = NormalizeOptionalText(fieldKey);
        FieldDisplayName = NormalizeOptionalText(fieldDisplayName);
        Message = message.Trim();
        Severity = severity;
        RemediationCommand = remediationCommand;
    }

    /// <summary>
    /// Gets the stable field key associated with the message.
    /// </summary>
    public string? FieldKey { get; }

    /// <summary>
    /// Gets the human-readable field display name associated with the message.
    /// </summary>
    public string? FieldDisplayName { get; }

    /// <summary>
    /// Gets the validation message text.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the message severity.
    /// </summary>
    public ValidationSeverity Severity { get; }

    /// <summary>
    /// Gets an optional command that remediates or focuses the validation issue.
    /// </summary>
    public ICommand? RemediationCommand { get; }

    /// <summary>
    /// Gets a value indicating whether the message is attached to a specific field.
    /// </summary>
    public bool HasField => !string.IsNullOrWhiteSpace(FieldKey);

    /// <summary>
    /// Gets a value indicating whether this message should block form submission.
    /// </summary>
    public bool IsBlocking => Severity == ValidationSeverity.Error;

    /// <summary>
    /// Gets a value indicating whether this message has an associated remediation command.
    /// </summary>
    public bool HasRemediation => RemediationCommand is not null;

    /// <summary>
    /// Gets the message text with field context when available.
    /// </summary>
    public string DisplayText => string.IsNullOrWhiteSpace(FieldDisplayName) ? Message : $"{FieldDisplayName}: {Message}";

    private static string? NormalizeOptionalText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value!.Trim();
    }
}
