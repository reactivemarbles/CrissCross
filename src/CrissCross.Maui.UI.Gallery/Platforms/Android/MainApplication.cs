// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.App;
using Android.Runtime;

namespace CrissCross.Maui.UI.Gallery;

/// <summary>Provides the Android MAUI application entry point.</summary>
/// <param name="handle">The native Java object handle.</param>
/// <param name="ownership">The ownership policy for the native handle.</param>
[Application]
public sealed class MainApplication(IntPtr handle, JniHandleOwnership ownership) : MauiApplication(handle, ownership)
{
    /// <inheritdoc />
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
