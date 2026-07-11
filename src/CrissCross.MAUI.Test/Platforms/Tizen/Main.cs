// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
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
