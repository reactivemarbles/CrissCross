// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Maui.UI;

namespace CrissCross.Maui.UI.Gallery;

/// <summary>Builds the MAUI gallery application.</summary>
public static class MauiProgram
{
    /// <summary>Creates the MAUI application.</summary>
    /// <returns>The configured MAUI application.</returns>
    public static MauiApp CreateMauiApp() => MauiApp.CreateBuilder().UseMauiApp<App>().UseCrissCrossMauiUi().Build();
}
