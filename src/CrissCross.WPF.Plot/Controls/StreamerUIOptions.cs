// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.Plot;
#else
namespace CrissCross.WPF.Plot;
#endif

/// <summary>Configures scaling and point retention for a <see cref="StreamerUI"/>.</summary>
public sealed class StreamerUIOptions
{
    /// <summary>Gets a value indicating whether automatic axis scaling is enabled.</summary>
    public bool AutoScale { get; init; } = true;

    /// <summary>Gets a value indicating whether manual axis scaling is enabled.</summary>
    public bool ManualScale { get; init; }

    /// <summary>Gets a value indicating whether the displayed point count is fixed.</summary>
    public bool FixedPoints { get; init; }
}
