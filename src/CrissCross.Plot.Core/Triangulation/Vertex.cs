// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Triangulation;

/// <summary>Represents a grid vertex using its integer coordinates.</summary>
/// <param name="i">The first grid coordinate.</param>
/// <param name="j">The second grid coordinate.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1800", Justification = "The record remains inheritable for compatibility with existing consumers.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleSharp", "SST1801", Justification = "The positional parameter names are retained for public API compatibility.")]
public record Vertex(int i, int j);
