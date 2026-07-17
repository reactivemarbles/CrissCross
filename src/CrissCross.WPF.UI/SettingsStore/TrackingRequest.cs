// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI;

/// <summary>Identifies the target type for a tracking configuration.</summary>
/// <typeparam name="T">The tracked type.</typeparam>
public sealed class TrackingRequest<T>
{
    /// <summary>Gets the tracked runtime type.</summary>
    public Type TargetType => typeof(T);
}
