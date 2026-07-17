// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Specifies how an area series chooses its fill baseline.</summary>
public enum PlotBaselineMode
{
    /// <summary>Do not render a baseline fill.</summary>
    None,

    /// <summary>Fill to zero.</summary>
    Zero,

    /// <summary>Fill to <see cref="ReactivePlotSeriesStyle.Baseline"/>.</summary>
    Custom,
}
