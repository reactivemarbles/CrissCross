// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace CrissCross.Tests;

/// <summary>Coverage tests for gallery/example projects that double as manual QA documentation.</summary>
public class GalleryExampleCoverageTests
{
    /// <summary>Provides the SourceRoot member.</summary>
    private static readonly string SourceRoot = LocateSourceRoot();

    /// <summary>Provides the WpfGallery_IncludesCompleteReactiveFeaturePlayground member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task WpfGallery_IncludesCompleteReactiveFeaturePlayground()
    {
        var viewModel = ReadSource("CrissCross.WPF.UI.Gallery", "ViewModels", "FeaturePlaygroundViewModel.cs");
        var view = ReadSource("CrissCross.WPF.UI.Gallery", "Views", "FeaturePlaygroundView.xaml");
        var navigation = ReadSource("CrissCross.WPF.UI.Gallery", "ViewModels", "MainWindowViewModel.cs");

        await Assert.That(viewModel).Contains("ReactiveCommand.CreateFromTask");
        await Assert.That(viewModel).Contains("ObservableAsPropertyHelper");
        await Assert.That(view).Contains("ui:CommandButton");
        await Assert.That(view).Contains("ui:BusyOverlay");
        await Assert.That(view).Contains("ui:SearchBox");
        await Assert.That(view).Contains("ui:ThemeSwitcher");
        await Assert.That(view).Contains("ui:DataPager");
        await Assert.That(view).Contains("ui:DateTimeRangePicker");
        await Assert.That(view).Contains("ui:SegmentedControl");
        await Assert.That(view).Contains("ui:Stepper");
        await Assert.That(navigation).Contains("FeaturePlaygroundViewModel");
    }

    /// <summary>Provides the AvaloniaGallery_IncludesCompleteReactiveFeaturePlayground member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task AvaloniaGallery_IncludesCompleteReactiveFeaturePlayground()
    {
        var viewModel = ReadSource("CrissCross.Avalonia.UI.Gallery", "ViewModels", "FeaturePlaygroundPageViewModel.cs");
        var view = ReadSource("CrissCross.Avalonia.UI.Gallery", "Views", "Pages", "FeaturePlaygroundPageView.axaml");
        var navigation = ReadSource("CrissCross.Avalonia.UI.Gallery", "ViewModels", "MainViewModel.cs");

        await Assert.That(viewModel).Contains("ReactiveCommand.CreateFromTask");
        await Assert.That(viewModel).Contains("ObservableAsPropertyHelper");
        await Assert.That(view).Contains("controls:CommandButton");
        await Assert.That(view).Contains("controls:BusyOverlay");
        await Assert.That(view).Contains("controls:SearchBox");
        await Assert.That(view).Contains("controls:ThemeSwitcher");
        await Assert.That(view).Contains("controls:DataPager");
        await Assert.That(view).Contains("controls:DateTimeRangePicker");
        await Assert.That(view).Contains("controls:SegmentedControl");
        await Assert.That(view).Contains("controls:Stepper");
        await Assert.That(navigation).Contains("GotoFeaturePlayground");
    }

    /// <summary>Provides the AvaloniaGallery_RichTextBoxDemo_ShowcasesFullControlSurface member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task AvaloniaGallery_RichTextBoxDemo_ShowcasesFullControlSurface()
    {
        var view = ReadSource("CrissCross.Avalonia.UI.Gallery", "Views", "Pages", "InputPageView.axaml");
        var codeBehind = ReadSource("CrissCross.Avalonia.UI.Gallery", "Views", "Pages", "InputPageView.axaml.cs");
        var documentation = ReadSource("..", "docs", "gallery-examples.md");

        await Assert.That(view).Contains("Formatting toolbar and commands");
        await Assert.That(view).Contains("Context menu");
        await Assert.That(view).Contains("Drag and drop text or images");
        await Assert.That(view).Contains("Copy/paste workflow");
        await Assert.That(view).Contains("Serialization/import/export samples");
        await Assert.That(view).Contains("EditMode=\"Display\"");
        await Assert.That(view).Contains("IsRichClipboardEnabled=\"True\"");
        await Assert.That(view).Contains("IsImagePasteEnabled=\"True\"");
        await Assert.That(view).Contains("IsImageDropEnabled=\"True\"");
        await Assert.That(view).Contains("Toggle Display Mode");
        await Assert.That(view).Contains("Copy Selection");
        await Assert.That(view).Contains("Paste Clipboard Sample");
        await Assert.That(view).Contains("Export HTML");
        await Assert.That(view).Contains("Export Markdown");
        await Assert.That(view).Contains("Save/Load Stream Sample");
        await Assert.That(view).Contains("Undo");
        await Assert.That(view).Contains("Redo");
        await Assert.That(view).Contains("Font Family");
        await Assert.That(view).Contains("Font Size");
        await Assert.That(view).Contains("Foreground");
        await Assert.That(view).Contains("Highlight");
        await Assert.That(codeBehind).Contains("LoadRepresentativeRichTextContent");
        await Assert.That(codeBehind).Contains("SetSelectionFontFamily");
        await Assert.That(codeBehind).Contains("SetSelectionFontSize");
        await Assert.That(codeBehind).Contains("SetSelectionForeground");
        await Assert.That(codeBehind).Contains("SetSelectionHighlight");
        await Assert.That(codeBehind).Contains("Save(memoryStream, RichTextDataFormat.Html");
        await Assert.That(codeBehind).Contains("Load(memoryStream, RichTextDataFormat.Html");
        await Assert.That(codeBehind).Contains("ClipboardAdapter");
        await Assert.That(documentation).Contains("RichTextBox gallery");
        await Assert.That(documentation).Contains("formatting toolbar/actions");
        await Assert.That(documentation).Contains("serialization/import/export");
    }

    /// <summary>Provides the MauiExample_IncludesUiGalleryWithSharedStylesAndPlatformNotes member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task MauiExample_IncludesUiGalleryWithSharedStylesAndPlatformNotes()
    {
        var viewModel = ReadSource("CrissCross.MAUI.Test", "ViewModels", "ControlsGalleryViewModel.cs");
        var view = ReadSource("CrissCross.MAUI.Test", "Views", "ControlsGalleryView.xaml");
        var app = ReadSource("CrissCross.MAUI.Test", "App.xaml.cs");
        var project = ReadSource("CrissCross.MAUI.Test", "CrissCross.MAUI.Example.csproj");

        await Assert.That(viewModel).Contains("ReactiveCommand.CreateFromTask");
        await Assert.That(viewModel).Contains("ObservableAsPropertyHelper");
        await Assert.That(viewModel).Contains("OperatingSystem.IsAndroid");
        await Assert.That(view).Contains("mauiui:CommandButton");
        await Assert.That(view).Contains("mauiui:BusyOverlay");
        await Assert.That(view).Contains("mauiui:SearchBox");
        await Assert.That(view).Contains("mauiui:ThemeSwitcher");
        await Assert.That(view).Contains("mauiui:DataPager");
        await Assert.That(view).Contains("mauiui:DateTimeRangePicker");
        await Assert.That(view).Contains("mauiui:SegmentedControl");
        await Assert.That(view).Contains("mauiui:ChipGroup");
        await Assert.That(view).Contains("mauiui:Stepper");
        await Assert.That(app).Contains("UseCrissCrossMauiUiResources");
        await Assert.That(project).Contains("CrissCross.Maui.UI.csproj");
    }

    /// <summary>Provides the GalleryDocumentation_ExplainsRunCommandsAndManualQaCoverage member.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task GalleryDocumentation_ExplainsRunCommandsAndManualQaCoverage()
    {
        var documentation = ReadSource("..", "docs", "gallery-examples.md");

        await Assert.That(documentation).Contains("CrissCross.WPF.UI.Gallery");
        await Assert.That(documentation).Contains("CrissCross.Avalonia.UI.Gallery");
        await Assert.That(documentation).Contains("CrissCross.MAUI.Test");
        await Assert.That(documentation).Contains("ViewModel-based navigation");
        await Assert.That(documentation).Contains("View-based navigation");
        await Assert.That(documentation).Contains("async reactive commands");
        await Assert.That(documentation).Contains("activation/disposal");
        await Assert.That(documentation).Contains("/mnt/c/Program Files/dotnet/dotnet.exe");
    }

    /// <summary>Provides the ReadSource member.</summary>
    /// <param name="relativeSegments">The relativeSegments value.</param>
    /// <returns>The result.</returns>
    private static string ReadSource(params string[] relativeSegments)
    {
        var path = Path.Combine(new[] { SourceRoot }.Concat(relativeSegments).ToArray());
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "Expected gallery source file was not found: {0}", path), path);
        }

        return File.ReadAllText(path);
    }

    /// <summary>Provides the LocateSourceRoot member.</summary>
    /// <returns>The result.</returns>
    private static string LocateSourceRoot()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "CrissCross.slnx");
            if (File.Exists(candidate))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate CrissCross.slnx from the current test working directory.");
    }
}
