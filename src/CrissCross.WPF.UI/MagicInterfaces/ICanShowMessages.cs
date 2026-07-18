// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Enables showing messages on ModernWindow.</summary>
public interface ICanShowMessages
{
    /// <summary>Gets the owner.</summary>
    /// <value>
    /// The owner.
    /// </value>
    string Owner { get; }
}
