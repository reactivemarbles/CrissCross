// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot;

/// <summary>Provides the obsolete name for <see cref="LabelStyle"/>.</summary>
[Obsolete("Label has been renamed to LabelStyle", true)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("RoslynCommonAnalyzers", "SST2310", Justification = "Label remains available to compiled consumers of the obsolete public type.")]
public class Label : LabelStyle;
