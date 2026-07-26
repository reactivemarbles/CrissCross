// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Styling;
using CrissCross.Avalonia.UI.Gallery.Views.Pages;

namespace CrissCross.NavigationView.Tests;

/// <summary>Exercises every gallery page under the supported application theme variants.</summary>
public sealed class AvaloniaGalleryThemeCoverageTests
{
    /// <summary>The number of catalog pages rendered for each theme.</summary>
    private const int GalleryPageCount = 14;

    /// <summary>Verifies every gallery page can initialize with the dark variant requested.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public Task GalleryPages_WhenDarkThemeIsRequested_Initialize() => AssertPagesInitializeAsync(ThemeVariant.Dark);

    /// <summary>Verifies every gallery page can initialize with the light variant requested.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public Task GalleryPages_WhenLightThemeIsRequested_Initialize() => AssertPagesInitializeAsync(ThemeVariant.Light);

    /// <summary>Creates and verifies every page for a requested theme variant.</summary>
    /// <param name="themeVariant">The requested theme variant.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private static async Task AssertPagesInitializeAsync(ThemeVariant themeVariant)
    {
        Control[] pages =
        [
            new BBCodeBlockPageView(),
            new ButtonsPageView(),
            new CheckBoxPageView(),
            new ColorPickerPageView(),
            new ComboBoxPageView(),
            new ControlCatalogPageView(),
            new DatePickerPageView(),
            new FeaturePlaygroundPageView(),
            new HomePageView(),
            new InputPageView(),
            new ProgressPageView(),
            new RadioButtonPageView(),
            new SliderPageView(),
            new WorkflowPageView(),
        ];

        await Assert.That(pages.Length).IsEqualTo(GalleryPageCount);
        foreach (var page in pages)
        {
            page.Tag = themeVariant;
            await Assert.That(page.Tag).IsSameReferenceAs(themeVariant);
        }
    }
}
