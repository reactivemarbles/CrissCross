// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Interactivity.UserActions;

/// <summary>Represents a user action that has no recognized input type.</summary>
/// <param name="device">The input device identifier.</param>
/// <param name="description">An optional description of the action.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "RoslynCommonAnalyzers",
    "SST1801",
    Justification = "The public positional parameter names are retained to preserve the existing record API and named-argument callers.")]
public record struct Unknown(string device, string? description) : IUserAction
{
    /// <summary>Gets the input device identifier.</summary>
    public readonly string Device => device;

    /// <summary>Gets the action description, or an empty string when no description is available.</summary>
    public readonly string Description => description ?? string.Empty;

    /// <summary>Gets or sets the UTC time when this action was recorded.</summary>
    public DateTime DateTime { get; set; } = UserActionTimestamp.UtcNow;
}
