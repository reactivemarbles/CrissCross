// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
