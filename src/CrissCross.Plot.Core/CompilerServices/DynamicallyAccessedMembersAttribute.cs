// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
#if NETFRAMEWORK
namespace System.Diagnostics.CodeAnalysis;

/// <summary>Indicates that specified members of a type are accessed dynamically.</summary>
/// <param name="memberTypes">The members that must be preserved.</param>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, Inherited = false)]
public sealed class DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes) : Attribute
{
    /// <summary>Gets the members that must be preserved.</summary>
    public DynamicallyAccessedMemberTypes MemberTypes { get; } = memberTypes;
}
#endif
