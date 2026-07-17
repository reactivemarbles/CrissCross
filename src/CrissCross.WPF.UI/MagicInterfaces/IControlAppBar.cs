// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
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
