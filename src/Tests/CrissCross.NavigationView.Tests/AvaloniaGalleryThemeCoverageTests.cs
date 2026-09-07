// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Styling;
using CrissCross.Avalonia.UI.Appearance;
using CrissCross.Avalonia.UI.Gallery.Views.Pages;

namespace CrissCross.NavigationView.Tests;

/// <summary>Exercises every gallery page under the supported application theme variants.</summary>
[TUnit.Core.Executors.TestExecutor<AvaloniaUiTestExecutor>]
public sealed class AvaloniaGalleryThemeCoverageTests
{
    /// <summary>The number of catalog pages rendered for each theme.</summary>
    private const int GalleryPageCount = 15;

    /// <summary>Verifies every gallery page can initialize with the dark variant requested.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public Task GalleryPages_WhenDarkThemeIsRequested_Initialize() => AssertPagesInitializeAsync(ThemeVariant.Dark);

    /// <summary>Verifies every gallery page can initialize with the light variant requested.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public Task GalleryPages_WhenLightThemeIsRequested_Initialize() => AssertPagesInitializeAsync(ThemeVariant.Light);

    /// <summary>Verifies every gallery page inherits the custom high contrast variant.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public Task GalleryPages_WhenHighContrastIsRequested_Initialize() => AssertPagesInitializeAsync(ApplicationThemeManager.HighContrastThemeVariant);

    /// <summary>Creates and verifies every page for a requested theme variant.</summary>
    /// <param name="themeVariant">The requested theme variant.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private static Task AssertPagesInitializeAsync(ThemeVariant themeVariant) => AvaloniaTestUiThread.RunAsync(async () =>
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
            new IndustrialPageView(),
            new InputPageView(),
            new ProgressPageView(),
            new RadioButtonPageView(),
            new SliderPageView(),
            new WorkflowPageView(),
        ];

        await Assert.That(pages.Length).IsEqualTo(GalleryPageCount);
        foreach (var page in pages)
        {
            var scope = new ThemeVariantScope { RequestedThemeVariant = themeVariant, Child = page };
            await Assert.That(page.ActualThemeVariant).IsSameReferenceAs(themeVariant);
            scope.Child = null;
        }
    });
}
