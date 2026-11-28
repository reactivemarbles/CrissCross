// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using System.ComponentModel;

namespace System.Runtime.CompilerServices;

/// <summary>Enables init-only setters when targeting older .NET versions.</summary>
[EditorBrowsable(EditorBrowsableState.Never)]
[Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "SST1436:Empty types should not be declared", Justification = "Provides support for init-only setters in older .NET versions.")]
internal static class IsExternalInit;
