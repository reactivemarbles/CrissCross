// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
#if NETFRAMEWORK
namespace System.Diagnostics.CodeAnalysis;

/// <summary>Indicates that a member requires code that may be removed by trimming.</summary>
/// <param name="message">The message describing the trimming requirement.</param>
[AttributeUsage(
    AttributeTargets.Method
    | AttributeTargets.Constructor
    | AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Interface
    | AttributeTargets.Event
    | AttributeTargets.Property,
    Inherited = false)]
public sealed class RequiresUnreferencedCodeAttribute(string message) : Attribute
{
    /// <summary>Gets the message describing the trimming requirement.</summary>
    public string Message { get; } = message;
}
#endif
