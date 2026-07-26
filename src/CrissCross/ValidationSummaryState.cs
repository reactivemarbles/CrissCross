// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Immutable aggregate validation state for form summary controls and submit gating.</summary>
public sealed class ValidationSummaryState
{
    /// <summary>The maximum number of categories included in the summary text.</summary>
    private const int SummaryCategoryCapacity = 3;

    /// <summary>Initializes a new instance of the <see cref="ValidationSummaryState"/> class.</summary>
    /// <param name="messages">The validation messages to summarize.</param>
    public ValidationSummaryState(IEnumerable<ValidationMessage>? messages)
    {
        var messageList = new List<ValidationMessage>();
        var errorCount = 0;
        var warningCount = 0;
        var pendingCount = 0;
        if (messages is not null)
        {
            foreach (var message in messages)
            {
                messageList.Add(message);
                switch (message.Severity)
                {
                    case ValidationSeverity.Error:
                        {
                            errorCount++;
                            break;
                        }

                    case ValidationSeverity.Warning:
                        {
                            warningCount++;
                            break;
                        }

                    case ValidationSeverity.Pending:
                        {
                            pendingCount++;
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }
        }

        Messages = new ReadOnlyCollection<ValidationMessage>(messageList);
        ErrorCount = errorCount;
        WarningCount = warningCount;
        PendingCount = pendingCount;
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
    public ValidationMessage? FirstError => FindFirstError();

    /// <summary>Gets a compact summary string suitable for validation summary headers.</summary>
    public string SummaryText
    {
        get
        {
            var parts = new List<string>(SummaryCategoryCapacity);
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
        var matchingMessages = new List<ValidationMessage>();
        foreach (var message in Messages)
        {
            if (string.Equals(message.FieldKey, normalized, StringComparison.OrdinalIgnoreCase))
            {
                matchingMessages.Add(message);
            }
        }

        return matchingMessages;
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

        var label = count == 1 ? singular : $"{singular}s";
        parts.Add($"{count} {label}");
    }

    /// <summary>Finds the first blocking validation error.</summary>
    /// <returns>The first error, or <c>null</c> when none exists.</returns>
    private ValidationMessage? FindFirstError()
    {
        foreach (var message in Messages)
        {
            if (message.Severity == ValidationSeverity.Error)
            {
                return message;
            }
        }

        return null;
    }
}
