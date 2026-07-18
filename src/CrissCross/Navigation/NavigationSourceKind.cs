// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Identifies the caller-facing side of a bidirectional navigation request.</summary>
public enum NavigationSourceKind
{
    /// <summary>The request is keyed by a view model type or instance.</summary>
    ViewModel,

    /// <summary>The request is keyed by a view type or instance.</summary>
    View,
}
