// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.Plot;

/// <summary>
/// Describes how X-axis values should be interpreted.
/// </summary>
public enum PlotXAxisKind
{
    /// <summary>
    /// Numeric X-axis values.
    /// </summary>
    Numeric,

    /// <summary>
    /// OLE Automation date values.
    /// </summary>
    OADate,

    /// <summary>
    /// .NET tick values.
    /// </summary>
    Ticks,
}
