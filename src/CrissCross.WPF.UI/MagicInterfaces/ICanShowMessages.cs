// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>
/// Enables showing messages on ModernWindow.
/// </summary>
public interface ICanShowMessages
{
    /// <summary>
    /// Gets the owner.
    /// </summary>
    /// <value>
    /// The owner.
    /// </value>
    string Owner { get; }
}
