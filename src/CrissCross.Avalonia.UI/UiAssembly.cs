// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI;
#else
namespace CrissCross.Avalonia.UI;
#endif

/// <summary>Allows to get the assembly through <see cref="Assembly"/>.</summary>
public static class UiAssembly
{
    /// <summary>Gets the assembly.</summary>
    public static Assembly Assembly => Assembly.GetExecutingAssembly();
}
