// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Describes a reusable empty, error, offline, or no-results presentation surface.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmptyStateModel"/> class.
/// </remarks>
/// <param name="title">The primary heading for the empty state.</param>
/// <param name="message">Optional explanatory text.</param>
/// <param name="variant">The empty-state variant.</param>
/// <param name="primaryActionText">Optional primary action label.</param>
/// <param name="primaryActionCommand">Optional primary action command.</param>
/// <param name="secondaryActionText">Optional secondary action label.</param>
/// <param name="secondaryActionCommand">Optional secondary action command.</param>
public sealed class EmptyStateModel(
    string title,
    string? message = null,
    EmptyStateVariant variant = EmptyStateVariant.NoData,
    string? primaryActionText = null,
    ICommand? primaryActionCommand = null,
    string? secondaryActionText = null,
    ICommand? secondaryActionCommand = null)
{
    /// <summary>Gets the primary heading for the empty state.</summary>
    public string Title { get; } = title;

    /// <summary>Gets optional explanatory text.</summary>
    public string? Message { get; } = message;

    /// <summary>Gets the empty-state variant.</summary>
    public EmptyStateVariant Variant { get; } = variant;

    /// <summary>Gets optional primary action label.</summary>
    public string? PrimaryActionText { get; } = primaryActionText;

    /// <summary>Gets optional primary action command.</summary>
    public ICommand? PrimaryActionCommand { get; } = primaryActionCommand;

    /// <summary>Gets optional secondary action label.</summary>
    public string? SecondaryActionText { get; } = secondaryActionText;

    /// <summary>Gets optional secondary action command.</summary>
    public ICommand? SecondaryActionCommand { get; } = secondaryActionCommand;

    /// <summary>Gets a value indicating whether the model exposes a primary action.</summary>
    public bool HasPrimaryAction => PrimaryActionCommand is not null && !string.IsNullOrWhiteSpace(PrimaryActionText);

    /// <summary>Gets a value indicating whether the model exposes a secondary action.</summary>
    public bool HasSecondaryAction =>
        SecondaryActionCommand is not null && !string.IsNullOrWhiteSpace(SecondaryActionText);
}
