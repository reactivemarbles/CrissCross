// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Foundation;

namespace CrissCross.MAUI.Test
{
    /// <summary>AppDelegate member.</summary>
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        /// <summary>Creates the maui application.</summary>
        /// <returns>A MauiApp.</returns>
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
