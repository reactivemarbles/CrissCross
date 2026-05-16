// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace CrissCross;

/// <summary>
/// Provides a null-object service provider for factory delegates that do not need services.
/// </summary>
internal sealed class EmptyServiceProvider : IServiceProvider
{
    public static readonly EmptyServiceProvider Instance = new();

    private EmptyServiceProvider()
    {
    }

    public object? GetService(Type serviceType) => null;
}
