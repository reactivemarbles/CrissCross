// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.App;
using Android.Runtime;

namespace CrissCross.MAUI.Test;

/// <summary>
/// MainApplication.
/// </summary>
/// <seealso cref="MauiApplication" />
[Application]
public class MainApplication : MauiApplication
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainApplication"/> class.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="ownership">The ownership.</param>
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    /// <summary>
    /// Creates the maui application.
    /// </summary>
    /// <returns>A MauiApp.</returns>
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
