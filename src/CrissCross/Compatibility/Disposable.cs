// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace CrissCross.Compatibility;

/// <summary>Provides compatibility helpers for disposable factory call sites migrated from Rx.</summary>
public static class Disposable
{
    /// <summary>Creates a disposable that invokes the specified action once.</summary>
    /// <param name="dispose">The action to invoke when the disposable is disposed.</param>
    /// <returns>A disposable wrapper around <paramref name="dispose"/>.</returns>
    public static IDisposable Create(Action dispose) => new ActionDisposable(dispose);
}
