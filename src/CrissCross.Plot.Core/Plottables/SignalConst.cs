// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
using CrissCross.Plot.DataSources;

namespace CrissCross.Plot.Plottables;

/// <summary>Compatibility type for constant-sample signal plots.</summary>
/// <typeparam name="T">The numeric sample type.</typeparam>
/// <param name="data">The constant-sample signal data source.</param>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "RoslynCommonAnalyzers",
    "SST2310",
    Justification = "This obsolete public compatibility type is retained to avoid breaking existing binaries.")]
[Obsolete(
    "SignalConst has been deprecated, "
    + "but its functionality may be achieved by creating a Signal plot with a SignalConstSource data source. "
    + "See the Add.SignalConst() method for reference.",
    true)]
public class SignalConst<T>(SignalConstSource<T> data) : Signal(data)
    where T : struct, IComparable
{
}
