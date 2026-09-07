// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Maui.UI.Gallery;
using Microsoft.Maui.Controls;

namespace CrissCross.Tests;

/// <summary>Verifies the actual gallery accepts payloads emitted by its composed controls.</summary>
public sealed class MauiGalleryCommandTests
{
    /// <summary>The explicit and system theme transitions exercised by the gallery.</summary>
    private static readonly ThemeChoice[] ThemeChoices = [ThemeChoice.HighContrast, ThemeChoice.Light, ThemeChoice.Dark, ThemeChoice.System];

    /// <summary>Verifies explicit theme transitions update the gallery state and resources.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task ThemeCommand_CyclesBetweenHighContrastLightAndDark()
    {
        var previousApplication = Application.Current;
        Application.SetCurrentApplication(new());
        try
        {
            using var gallery = new GalleryViewModel();
            foreach (var choice in ThemeChoices)
            {
                gallery.SetThemeCommand.Execute(choice);
                await Assert.That(gallery.ThemeState.SelectedChoice).IsEqualTo(choice);
                await Assert.That(gallery.ThemeDescription).IsEqualTo(gallery.ThemeState.DisplayText);
            }
        }
        finally
        {
            Application.Current = previousApplication;
        }
    }

    /// <summary>Verifies step selection consumes a descriptor and updates the selected key.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task StepCommand_AcceptsTheControlDescriptorPayload()
    {
        using var gallery = new GalleryViewModel();
        var step = gallery.StepperState.Steps[0];
        gallery.SelectStepCommand.Execute(step);
        await Assert.That(gallery.StepperState.CurrentStep?.Key).IsEqualTo(step.Key);
    }

    /// <summary>Verifies submitted filters become the gallery search state.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task ApplyFilters_AcceptsTheSubmittedQuery()
    {
        using var gallery = new GalleryViewModel();
        var query = new SearchQueryState("pump");
        gallery.ApplyFiltersCommand.Execute(query);
        await Assert.That(gallery.SearchState).IsSameReferenceAs(query);
        await Assert.That(gallery.FilterPanelState.Descriptors.Count).IsGreaterThan(0);
    }

    /// <summary>Verifies committing an edited descriptor establishes a new unmodified baseline.</summary>
    /// <returns>The asynchronous test operation.</returns>
    [Test]
    public async Task CommitProperties_PreservesTheEditedValueAsTheBaseline()
    {
        using var gallery = new GalleryViewModel();
        var edited = new PropertyDescriptorModel("tag", "Equipment tag", new() { Value = "PUMP-102", OriginalValue = "PUMP-101" });
        gallery.UpdatePropertyCommand.Execute(new PropertyGridState([edited]));
        var committed = gallery.PropertyGridState.Descriptors[0];
        await Assert.That(committed.Value).IsEqualTo(edited.Value);
        await Assert.That(committed.OriginalValue).IsEqualTo(edited.Value);
        await Assert.That(committed.IsModified).IsFalse();
    }
}
