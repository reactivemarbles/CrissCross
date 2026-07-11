// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Versioning;
using ReactiveUI.Builder;

namespace CrissCross.WinForms.Test;

/// <summary>Provides the WinForms application entry point.</summary>
internal static class Program
{
    /// <summary>The main entry point for the application.</summary>
    [STAThread]
    [RequiresPreviewFeatures]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        _ = RxAppBuilder.CreateReactiveUIBuilder().WithWinForms().BuildApp();
        Application.Run(new MainForm());
    }
}
