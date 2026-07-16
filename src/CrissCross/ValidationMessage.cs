// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace CrissCross;

/// <summary>Immutable validation feedback for a named form field or form-level rule.</summary>
public sealed class ValidationMessage
{
    /// <inheritdoc />
    public ValidationMessage(string? fieldKey, string? fieldDisplayName, string message)
        : this(fieldKey, fieldDisplayName, message, ValidationSeverity.Error, null) { }

    /// <inheritdoc />
    public ValidationMessage(string? fieldKey, string? fieldDisplayName, string message, ValidationSeverity severity)
        : this(fieldKey, fieldDisplayName, message, severity, null) { }

    /// <summary>Initializes a new instance of the <see cref="ValidationMessage"/> class.</summary>
    /// <param name="fieldKey">The stable field key associated with the message.</param>
    /// <param name="fieldDisplayName">The human-readable field display name.</param>
    /// <param name="message">The validation message text.</param>
    /// <param name="severity">The message severity.</param>
    /// <param name="remediationCommand">An optional command that remediates or focuses the issue.</param>
    public ValidationMessage(
        string? fieldKey,
        string? fieldDisplayName,
        string message,
        ValidationSeverity severity,
        ICommand? remediationCommand)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(message, nameof(message));

        FieldKey = NormalizeOptionalText(fieldKey);
        FieldDisplayName = NormalizeOptionalText(fieldDisplayName);
        Message = message.Trim();
        Severity = severity;
        RemediationCommand = remediationCommand;
    }

    /// <summary>Gets the stable field key associated with the message.</summary>
    public string? FieldKey { get; }

    /// <summary>Gets the human-readable field display name associated with the message.</summary>
    public string? FieldDisplayName { get; }

    /// <summary>Gets the validation message text.</summary>
    public string Message { get; }

    /// <summary>Gets the message severity.</summary>
    public ValidationSeverity Severity { get; }

    /// <summary>Gets an optional command that remediates or focuses the validation issue.</summary>
    public ICommand? RemediationCommand { get; }

    /// <summary>Gets a value indicating whether the message is attached to a specific field.</summary>
    public bool HasField => !string.IsNullOrWhiteSpace(FieldKey);

    /// <summary>Gets a value indicating whether this message should block form submission.</summary>
    public bool IsBlocking => Severity == ValidationSeverity.Error;

    /// <summary>Gets a value indicating whether this message has an associated remediation command.</summary>
    public bool HasRemediation => RemediationCommand is not null;

    /// <summary>Gets the message text with field context when available.</summary>
    public string DisplayText =>
        string.IsNullOrWhiteSpace(FieldDisplayName) ? Message : $"{FieldDisplayName}: {Message}";

    /// <summary>Normalizes optional user-facing text.</summary>
    /// <param name="value">The optional text.</param>
    /// <returns>The normalized text, or <c>null</c> when blank.</returns>
    private static string? NormalizeOptionalText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value!.Trim();
}
