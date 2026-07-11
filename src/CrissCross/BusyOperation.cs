// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace CrissCross;

/// <summary>Describes the operation projected by a regional busy overlay.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BusyOperation"/> class.
/// </remarks>
/// <param name="title">The operation title displayed by the overlay.</param>
/// <param name="message">Optional operation details.</param>
/// <param name="progress">Optional normalized progress from 0.0 to 1.0.</param>
/// <param name="cancelCommand">Optional command used to request operation cancellation.</param>
public sealed class BusyOperation(string title, string? message = null, double? progress = null, ICommand? cancelCommand = null)
{
    /// <summary>Gets the operation title displayed by the overlay.</summary>
    public string Title { get; } = title;

    /// <summary>Gets optional operation details.</summary>
    public string? Message { get; } = message;

    /// <summary>Gets optional normalized progress from 0.0 to 1.0.</summary>
    public double? Progress { get; } = progress;

    /// <summary>Gets optional command used to request operation cancellation.</summary>
    public ICommand? CancelCommand { get; } = cancelCommand;

    /// <summary>Gets a value indicating whether this operation should be displayed by an overlay.</summary>
    public bool IsActive => !string.IsNullOrWhiteSpace(Title);

    /// <summary>Gets a value indicating whether the operation has determinate progress.</summary>
    public bool IsDeterminate => Progress is >= 0d and <= 1d;

    /// <summary>Gets a value indicating whether the operation can be cancelled by the user.</summary>
    public bool IsCancellable => CancelCommand is not null;
}
