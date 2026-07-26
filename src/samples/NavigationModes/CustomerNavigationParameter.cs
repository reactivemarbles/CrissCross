// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.NavigationModes.Example;

/// <summary>Provides a strongly typed navigation parameter.</summary>
/// <param name="CustomerId">The selected customer identifier.</param>
public sealed record CustomerNavigationParameter(string CustomerId);
