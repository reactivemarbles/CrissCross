// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Android.App;
using Android.Content.PM;

using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;

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
