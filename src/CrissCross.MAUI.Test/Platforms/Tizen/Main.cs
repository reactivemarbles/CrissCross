// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace CrissCross.MAUI.Test
{
    /// <summary>Program member.</summary>
    internal class Program : MauiApplication
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        /// <summary>Defines the entry point of the application.</summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
