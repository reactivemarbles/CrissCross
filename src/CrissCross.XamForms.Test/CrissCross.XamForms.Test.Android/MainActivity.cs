// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace CrissCross.XamForms.Test.Droid
{
    /// <summary>
    /// MainActivity.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Platform.Android.FormsAppCompatActivity" />
    [Activity(Label = "CrissCross.XamForms.Test", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        /// <summary>
        /// Callback for the result from requesting permissions.
        /// </summary>
        /// <param name="requestCode">The request code passed in <c>#requestPermissions(String[], int)</c>.</param>
        /// <param name="permissions">The requested permissions. Never null.</param>
        /// <param name="grantResults">The grant results for the corresponding permissions
        /// which is either <c>android.content.pm.PackageManager#PERMISSION_GRANTED</c>
        /// or <c>android.content.pm.PackageManager#PERMISSION_DENIED</c>. Never null.</param>
        /// <remarks>
        /// <para>
        ///   <format type="text/html">
        ///     <a href="https://developer.android.com/reference/android/app/Activity#onRequestPermissionsResult(int,%20java.lang.String[],%20int[])" title="Reference documentation">Java documentation for <code>android.app.Activity.onRequestPermissionsResult(int, java.lang.String[], int[])</code>.</a>
        ///   </format>
        /// </para>
        /// <para>
        /// Portions of this page are modifications based on work created and shared by the
        /// <format type="text/html"><a href="https://developers.google.com/terms/site-policies" title="Android Open Source Project">Android Open Source Project</a></format>
        /// and used according to terms described in the.
        /// <format type="text/html"><a href="https://creativecommons.org/licenses/by/2.5/" title="Creative Commons 2.5 Attribution License">Creative Commons 2.5 Attribution License.</a></format></para>
        /// </remarks>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /// <summary>
        /// Called when [create].
        /// </summary>
        /// <param name="savedInstanceState">State of the saved instance.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
    }
}
