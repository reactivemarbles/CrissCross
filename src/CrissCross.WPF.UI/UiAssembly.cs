// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Allows to get the assembly through <see cref="Assembly"/>.</summary>
public static class UiAssembly
{
    /// <summary>Gets the assembly.</summary>
    public static Assembly Assembly => typeof(UiAssembly).Assembly;
}
