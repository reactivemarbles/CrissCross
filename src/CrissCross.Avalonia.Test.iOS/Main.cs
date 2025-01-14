// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

namespace CrissCross.Avalonia.Test.IOS;

/// <summary>
/// Application.
/// </summary>
#pragma warning disable SA1649 // File name should match first type name
public static class Application
#pragma warning restore SA1649 // File name should match first type name
{
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    /// <param name="args">The arguments.</param>
    private static void Main(string[] args)
    {
        // if you want to use a different Application Delegate class from "AppDelegate"
        // you can specify it here.
        UIApplication.Main(args, null, typeof(AppDelegate));
    }
}
