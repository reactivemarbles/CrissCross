// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>Identifies a typed WPF page navigation target.</summary>
/// <typeparam name="TPage">The page type.</typeparam>
public sealed class PageNavigationRequest<TPage>
    where TPage : class
{
    /// <summary>Gets the runtime page type.</summary>
    public Type PageType => typeof(TPage);
}
