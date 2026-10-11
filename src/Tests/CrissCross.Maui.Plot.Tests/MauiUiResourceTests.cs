// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Graphics;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Maui.UI.Resources.Styles;
#else
using CrissCross.Maui.UI.Resources.Styles;
#endif

namespace CrissCross.Maui.Plot.Tests;

/// <summary>Verifies the plot dependency supplies working MAUI UI resources in both package families.</summary>
[NotInParallel]
public sealed class MauiUiResourceTests
{
    /// <summary>Verifies linked theme markup resolves its merged dictionaries and semantic palette.</summary>
    /// <returns>The asynchronous test.</returns>
    [Test]
    public async Task UiResources_LoadMergedMarkupAndPalette()
    {
        var available = await PlotTestApp.Run(static () =>
        {
            var resources = new CrissCrossMauiUi();
            return resources.TryGetValue("CrissCrossAccentColorLight", out var accent)
                && accent is Color;
        });

        await Assert.That(available).IsTrue();
    }
}
