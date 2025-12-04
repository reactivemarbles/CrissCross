// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.App;
using Android.Content.PM;

using Avalonia;
using Avalonia.Android;
using ReactiveUI.Avalonia;

namespace CrissCross.Avalonia.Test.Android;

#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
/// <summary>
/// MainActivity.
/// </summary>
[Activity(
#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute
    Label = "CrissCross.Avalonia.Test.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    /// <summary>
    /// Customizes the application builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>App Builder.</returns>
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) =>
        base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .UseReactiveUI();
}
