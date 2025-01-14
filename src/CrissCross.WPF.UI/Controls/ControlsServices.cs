// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Controls;

/// <summary>
/// Used to initialize the library controls with static values.
/// </summary>
public static class ControlsServices
{
#if NET48_OR_GREATER || NETCOREAPP3_0_OR_GREATER
    internal static IServiceProvider? ControlsServiceProvider { get; private set; }

    /// <summary>
    /// Accepts a ServiceProvider for configuring dependency injection.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public static void Initialize(IServiceProvider? serviceProvider)
    {
        if (serviceProvider == null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        ControlsServiceProvider = serviceProvider;
    }
#endif
}
