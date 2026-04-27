// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using ReactiveUI.Avalonia;

namespace CrissCross.Avalonia.Test.Android;

/// <summary>
/// Android application entry point for Avalonia.
/// </summary>
[Application]
public class AndroidApp : AvaloniaAndroidApplication<App>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidApp"/> class.
    /// </summary>
    /// <param name="javaReference">The Java reference.</param>
    /// <param name="transfer">The JNI handle ownership.</param>
    protected AndroidApp(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
    }

    /// <summary>
    /// Customizes the application builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns>App Builder.</returns>
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder) =>
        base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .UseReactiveUI(b => { });
}
