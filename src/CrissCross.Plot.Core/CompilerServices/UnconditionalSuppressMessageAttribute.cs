// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
#if NETFRAMEWORK
namespace System.Diagnostics.CodeAnalysis;

/// <summary>Suppresses a trimming warning when the affected code has a documented justification.</summary>
/// <param name="category">The category of the warning.</param>
/// <param name="checkId">The identifier of the warning.</param>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public sealed class UnconditionalSuppressMessageAttribute(string category, string checkId) : Attribute
{
    /// <summary>Gets the warning category.</summary>
    public string Category { get; } = category;

    /// <summary>Gets the warning identifier.</summary>
    public string CheckId { get; } = checkId;

    /// <summary>Gets or sets the reason for suppressing the warning.</summary>
    public string? Justification { get; set; }
}
#endif
