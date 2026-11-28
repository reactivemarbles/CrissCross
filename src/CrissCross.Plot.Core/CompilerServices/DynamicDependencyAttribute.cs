// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
#if NETFRAMEWORK
namespace System.Diagnostics.CodeAnalysis;

/// <summary>Specifies a dependency on members that must be preserved when trimming is enabled.</summary>
/// <param name="memberTypes">The members that must be preserved.</param>
/// <param name="type">The type whose members must be preserved.</param>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public sealed class DynamicDependencyAttribute(DynamicallyAccessedMemberTypes memberTypes, Type type) : Attribute
{
    /// <summary>Gets the members that must be preserved.</summary>
    public DynamicallyAccessedMemberTypes MemberTypes { get; } = memberTypes;

    /// <summary>Gets the type whose members must be preserved.</summary>
    public Type Type { get; } = type;
}
#endif
