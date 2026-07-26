// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.WPF.UI.Configuration;
#else
namespace CrissCross.WPF.UI.Configuration;
#endif

/// <summary>Represents ITrackingAware.</summary>
public interface ITrackingAware
{
    /// <summary>Allows an object to configure its tracking.</summary>
    /// <param name="configuration">The configuration value.</param>
    void ConfigureTracking(TrackingConfiguration configuration);
}
