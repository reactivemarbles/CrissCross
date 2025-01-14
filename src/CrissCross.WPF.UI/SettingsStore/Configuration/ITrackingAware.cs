// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Configuration;

/// <summary>
/// ITrackingAware.
/// </summary>
public interface ITrackingAware
{
    /// <summary>
    /// Allows an object to configure its tracking.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    void ConfigureTracking(TrackingConfiguration configuration);
}
