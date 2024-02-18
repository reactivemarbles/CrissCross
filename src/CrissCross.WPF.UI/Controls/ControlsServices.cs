// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////   This file has been borrowed from Wpf-UI.

//// This Source Code Form is subject to the terms of the MIT License.
//// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
//// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
//// All Rights Reserved.

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
