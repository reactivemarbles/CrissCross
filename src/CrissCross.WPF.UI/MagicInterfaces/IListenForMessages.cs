// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI;
#else
namespace CrissCross.WPF.UI;
#endif

/// <summary>Enable handling of messages on ModernWindow.</summary>
public interface IListenForMessages
{
    /// <summary>Gets or sets the listener name.</summary>
    string Name { get; set; }
}
