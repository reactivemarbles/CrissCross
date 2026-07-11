// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CrissCross;

/// <summary>Immutable aggregate validation state for form summary controls and submit gating.</summary>
public sealed class ValidationSummaryState
{
    /// <summary>Initializes a new instance of the <see cref="ValidationSummaryState"/> class.</summary>
    /// <param name="messages">The validation messages to summarize.</param>
    public ValidationSummaryState(IEnumerable<ValidationMessage>? messages)
    {
        Messages = new ReadOnlyCollection<ValidationMessage>((messages ?? []).ToList());
        ErrorCount = Messages.Count(message => message.Severity == ValidationSeverity.Error);
        WarningCount = Messages.Count(message => message.Severity == ValidationSeverity.Warning);
        PendingCount = Messages.Count(message => message.Severity == ValidationSeverity.Pending);
    }

    /// <summary>Gets the summarized validation messages.</summary>
    public IReadOnlyList<ValidationMessage> Messages { get; }

    /// <summary>Gets the number of blocking validation errors.</summary>
    public int ErrorCount { get; }

    /// <summary>Gets the number of non-blocking warnings.</summary>
    public int WarningCount { get; }

    /// <summary>Gets the number of pending async validation checks.</summary>
    public int PendingCount { get; }

    /// <summary>Gets the number of messages that block form submission.</summary>
    public int BlockingCount => ErrorCount;

    /// <summary>Gets a value indicating whether the summary has blocking validation errors.</summary>
    public bool HasErrors => ErrorCount > 0;

    /// <summary>Gets a value indicating whether the summary has validation warnings.</summary>
    public bool HasWarnings => WarningCount > 0;

    /// <summary>Gets a value indicating whether the summary has validation work in progress.</summary>
    public bool IsPending => PendingCount > 0;

    /// <summary>Gets a value indicating whether the summary is valid and has no pending validation work.</summary>
    public bool IsValid => !HasErrors && !IsPending;

    /// <summary>Gets the first blocking error, if one exists.</summary>
    public ValidationMessage? FirstError => Messages.FirstOrDefault(message => message.Severity == ValidationSeverity.Error);

    /// <summary>Gets a compact summary string suitable for validation summary headers.</summary>
    public string SummaryText
    {
        get
        {
            var parts = new List<string>(3);
            AddCount(parts, ErrorCount, "error");
            AddCount(parts, WarningCount, "warning");
            AddCount(parts, PendingCount, "pending");
            return parts.Count == 0 ? "No validation messages" : string.Join(", ", parts);
        }
    }

    /// <summary>Gets validation messages associated with a stable field key.</summary>
    /// <param name="fieldKey">The field key to match.</param>
    /// <returns>The messages associated with the field.</returns>
    public IReadOnlyList<ValidationMessage> GetMessagesForField(string fieldKey)
    {
        if (string.IsNullOrWhiteSpace(fieldKey))
        {
            return [];
        }

        var normalized = fieldKey.Trim();
        return Messages
            .Where(message => string.Equals(message.FieldKey, normalized, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>Adds a labeled count to the summary parts.</summary>
    /// <param name="parts">The summary parts.</param>
    /// <param name="count">The count to add.</param>
    /// <param name="singular">The singular label.</param>
    private static void AddCount(List<string> parts, int count, string singular)
    {
        if (count == 0)
        {
            return;
        }

        var label = count == 1 ? singular : singular + "s";
        parts.Add($"{count} {label}");
    }
}
