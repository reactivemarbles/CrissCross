// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>Enable handling of messages on ModernWindow.</summary>
public interface IListenForMessages
{
    /// <summary>Gets or sets the listener name.</summary>
    string Name { get; set; }
}
