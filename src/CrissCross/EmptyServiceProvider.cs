// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace CrissCross;

/// <summary>Provides a null-object service provider for factory delegates that do not need services.</summary>
internal sealed class EmptyServiceProvider : IServiceProvider
{
    /// <summary>Gets the singleton empty service provider instance.</summary>
    public static readonly EmptyServiceProvider Instance = new();

    /// <summary>Initializes a new instance of the <see cref="EmptyServiceProvider"/> class.</summary>
    private EmptyServiceProvider()
    {
    }

    /// <inheritdoc/>
    public object? GetService(Type serviceType) => null;
}
