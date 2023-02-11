// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Android.App;
using Android.Content.PM;

namespace CrissCross.MAUI.Test
{
    /// <summary>
    /// MainActivity.
    /// </summary>
    /// <seealso cref="Microsoft.Maui.MauiAppCompatActivity" />
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}
