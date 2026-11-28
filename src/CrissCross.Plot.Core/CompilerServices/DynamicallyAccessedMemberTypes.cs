// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
#if NETFRAMEWORK
namespace System.Diagnostics.CodeAnalysis;

/// <summary>Specifies which members of a type are preserved when trimming is enabled.</summary>
[Flags]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "RoslynCommonAnalyzers",
    "SST2303",
    Justification = "The enum values intentionally match the framework trimming API; PublicConstructors is a composite flags value.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "RoslynCommonAnalyzers",
    "SST2461",
    Justification = "The enum values intentionally match the framework trimming API, including its composite constructor and all-members values.")]
public enum DynamicallyAccessedMemberTypes
{
    /// <summary>No members are preserved.</summary>
    None = 0,

    /// <summary>The public parameterless constructor is preserved.</summary>
    PublicParameterlessConstructor = 1,

    /// <summary>All public constructors are preserved.</summary>
    PublicConstructors = 3,

    /// <summary>All non-public constructors are preserved.</summary>
    NonPublicConstructors = 4,

    /// <summary>All public methods are preserved.</summary>
    PublicMethods = 8,

    /// <summary>All non-public methods are preserved.</summary>
    NonPublicMethods = 16,

    /// <summary>All public fields are preserved.</summary>
    PublicFields = 32,

    /// <summary>All non-public fields are preserved.</summary>
    NonPublicFields = 64,

    /// <summary>All public nested types are preserved.</summary>
    PublicNestedTypes = 128,

    /// <summary>All non-public nested types are preserved.</summary>
    NonPublicNestedTypes = 256,

    /// <summary>All public properties are preserved.</summary>
    PublicProperties = 512,

    /// <summary>All non-public properties are preserved.</summary>
    NonPublicProperties = 1024,

    /// <summary>All interfaces implemented by the type are preserved.</summary>
    Interfaces = 2048,

    /// <summary>All public events are preserved.</summary>
    PublicEvents = 4096,

    /// <summary>All non-public events are preserved.</summary>
    NonPublicEvents = 8192,

    /// <summary>All members are preserved.</summary>
    All = -1,
}
#endif
