// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.iOS;
using Avalonia.ReactiveUI;

using Foundation;

namespace CrissCross.Avalonia.Test.IOS;

/// <summary>
/// The UIApplicationDelegate for the application. This class is responsible for launching the
/// User Interface of the application, as well as listening (and optionally responding) to
/// application events from iOS.
/// AppDelegate.
/// </summary>
/// <seealso cref="AvaloniaAppDelegate&lt;App&gt;" />
[Register("AppDelegate")]
public partial class AppDelegate : AvaloniaAppDelegate<App>
{
    /// <summary>
    /// Customizes the application builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>A App Builder.</returns>
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) =>
        base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .UseReactiveUI();
}
