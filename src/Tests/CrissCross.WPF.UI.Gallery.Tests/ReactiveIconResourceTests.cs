// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Verifies reactive icon assets remain in the main assembly when names resemble cultures.</summary>
public sealed class ReactiveIconResourceTests
{
    /// <summary>Verifies culture-shaped icon suffixes are embedded in the reactive UI assembly.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task ReactiveIconResources_WhenSuffixResemblesCulture_RemainInMainAssembly()
    {
        Assembly assembly = typeof(CrissCross.Reactive.WPF.UI.Markup.ControlsDictionary).Assembly;
        string[] manifestNames = assembly.GetManifestResourceNames();
        string[] expectedNames =
        [
            "CrissCross.Reactive.WPF.UI.Controls.AppBarButton.Assets.AppBar.appbar.futurama.fry.xaml",
            "CrissCross.Reactive.WPF.UI.Controls.AppBarButton.Assets.AppBar.appbar.man.suitcase.run.xaml",
            "CrissCross.Reactive.WPF.UI.Controls.AppBarButton.Assets.AppBar.appbar.weather.sun.xaml",
            "CrissCross.Reactive.WPF.UI.Controls.AppBarButton.Assets.svg.run.svg",
        ];

        foreach (string expectedName in expectedNames)
        {
            await Assert.That(manifestNames).Contains(expectedName);
            await using var stream = assembly.GetManifestResourceStream(expectedName);
            await Assert.That(stream).IsNotNull();
        }
    }
}
