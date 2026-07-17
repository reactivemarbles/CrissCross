// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Configuration;

/// <summary>Event args for a tracking operation. Enables the handler to cancel the operation and modify the data that
/// will be persisted/applied.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PropertyOperationData"/> class.
/// Creates a new instance of PropertyData.
/// </remarks>
/// <param name="Property">The property that is being persisted or applied to.</param>
/// <param name="Value">The value that is being persited or applied.</param>
public sealed record PropertyOperationData(string Property, object? Value)
{
    /// <summary>Gets the value.</summary>
    /// <value>
    ///   <c>true</c> if cancel; otherwise, <c>false</c>.
    /// </value>
    public bool Cancel { get; init; }
}
