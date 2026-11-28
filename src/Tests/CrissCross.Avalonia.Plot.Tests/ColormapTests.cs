// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Plot;

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies the built-in colormap registry.</summary>
public sealed class ColormapTests
{
    /// <summary>Checks that every built-in colormap is available without reflection-based discovery.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetColormaps_ReturnsEveryBuiltInColormap()
    {
        IColormap[] colormaps = Colormap.GetColormaps();
        string[] names = new string[colormaps.Length];
        for (int i = 0; i < colormaps.Length; i++)
        {
            names[i] = colormaps[i].GetType().Name;
        }

        await Assert.That(names).IsEquivalentTo([
            "Algae",
            "Amp",
            "Balance",
            "Blues",
            "Curl",
            "Deep",
            "Delta",
            "Dense",
            "Diff",
            "Grayscale",
            "GrayscaleReversed",
            "Greens",
            "Haline",
            "Ice",
            "Inferno",
            "Jet",
            "Magma",
            "Matter",
            "MellowRainbow",
            "Oxy",
            "Phase",
            "Plasma",
            "Rain",
            "Solar",
            "Speed",
            "Tarn",
            "Tempo",
            "Thermal",
            "Topo",
            "Turbid",
            "Turbo",
            "Viridis",
        ]);
    }
}
