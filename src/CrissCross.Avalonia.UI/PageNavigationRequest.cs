// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Avalonia.UI;
#else
namespace CrissCross.Avalonia.UI;
#endif

/// <summary>Identifies a typed Avalonia page navigation target and its data context.</summary>
/// <typeparam name="TPage">The page type.</typeparam>
public sealed record PageNavigationRequest<TPage>
    where TPage : class
{
    /// <summary>Gets the runtime page type.</summary>
    public Type PageType => typeof(TPage);

    /// <summary>Gets the optional data context assigned during navigation.</summary>
    public object? DataContext { get; init; }
}
