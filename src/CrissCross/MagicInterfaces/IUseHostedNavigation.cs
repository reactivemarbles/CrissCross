// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross;

/// <summary>Enables Navigation commands specified by host name.</summary>
public interface IUseHostedNavigation
{
    /// <summary>Gets the navigation host or component name.</summary>
    string? Name { get; }
}
