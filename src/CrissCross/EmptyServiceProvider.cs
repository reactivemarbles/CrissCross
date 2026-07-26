// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive;
#else
namespace CrissCross;
#endif

/// <summary>Provides a null-object service provider for factory delegates that do not need services.</summary>
internal sealed class EmptyServiceProvider : IServiceProvider
{
    /// <summary>Gets the singleton empty service provider instance.</summary>
    internal static readonly EmptyServiceProvider Instance = new();

    /// <summary>Initializes a new instance of the <see cref="EmptyServiceProvider"/> class.</summary>
    private EmptyServiceProvider()
    {
    }

    /// <inheritdoc/>
    public object? GetService(Type serviceType) => null;
}
