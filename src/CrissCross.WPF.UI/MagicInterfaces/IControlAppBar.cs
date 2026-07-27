// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Enables control of AppBar on modernWindow.</summary>
public interface IControlAppBar
{
    /// <summary>Gets a value indicating whether the app bar is sticky.</summary>
    bool AppBarIsSticky { get; }
}
