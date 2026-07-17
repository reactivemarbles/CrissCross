// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
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
