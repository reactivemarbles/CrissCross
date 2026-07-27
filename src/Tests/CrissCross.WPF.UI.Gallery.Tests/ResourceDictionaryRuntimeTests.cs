// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Loads every WPF resource dictionary and theme option in both package variants.</summary>
public sealed class ResourceDictionaryRuntimeTests
{
    /// <summary>The standard controls dictionary pack URI component.</summary>
    private const string StandardControlsAssembly = "CrissCross.WPF.UI;component";

    /// <summary>The reactive controls dictionary pack URI component.</summary>
    private const string ReactiveControlsAssembly = "CrissCross.WPF.UI.Reactive;component";

    /// <summary>The dark-theme resource dictionary suffix.</summary>
    private const string DarkThemeSuffix = "Dark.xaml";

    /// <summary>The light-theme resource dictionary suffix.</summary>
    private const string LightThemeSuffix = "Light.xaml";

    /// <summary>The default high-contrast theme resource dictionary suffix.</summary>
    private const string HighContrastThemeSuffix = "HCBlack.xaml";

    /// <summary>Verifies standard and reactive resource dictionaries load all supported theme variants.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ResourceDictionaries_WhenThemeChanges_LoadStandardAndReactiveResources()
    {
        ResourceSnapshot snapshot = await RunOnStaThreadAsync(LoadResources);
        string[] expectedThemeSuffixes = [DarkThemeSuffix, LightThemeSuffix, HighContrastThemeSuffix];

        await Assert.That(snapshot.StandardControlsSource).Contains(StandardControlsAssembly);
        await Assert.That(snapshot.ReactiveControlsSource).Contains(ReactiveControlsAssembly);
        foreach (string themeSuffix in expectedThemeSuffixes)
        {
            await Assert.That(ContainsSuffix(snapshot.StandardThemeSources, themeSuffix)).IsTrue();
            await Assert.That(ContainsSuffix(snapshot.ReactiveThemeSources, themeSuffix)).IsTrue();
        }
    }

    /// <summary>Loads resources and returns the selected pack URIs.</summary>
    /// <returns>The resource URI snapshot.</returns>
    private static ResourceSnapshot LoadResources()
    {
        CrissCross.WPF.UI.Markup.ControlsDictionary standardControls = new();
        CrissCross.Reactive.WPF.UI.Markup.ControlsDictionary reactiveControls = new();
        CrissCross.WPF.UI.Markup.ThemesDictionary standardThemes = new();
        CrissCross.Reactive.WPF.UI.Markup.ThemesDictionary reactiveThemes = new();
        List<string> standardThemeSources = [];
        List<string> reactiveThemeSources = [];

        foreach (CrissCross.WPF.UI.Appearance.ApplicationTheme theme in Enum.GetValues<CrissCross.WPF.UI.Appearance.ApplicationTheme>())
        {
            standardThemes.Theme = theme;
            standardThemeSources.Add(GetRequiredUriText(standardThemes.Source));
        }

        foreach (CrissCross.Reactive.WPF.UI.Appearance.ApplicationTheme theme in Enum.GetValues<CrissCross.Reactive.WPF.UI.Appearance.ApplicationTheme>())
        {
            reactiveThemes.Theme = theme;
            reactiveThemeSources.Add(GetRequiredUriText(reactiveThemes.Source));
        }

        return new(
            GetRequiredUriText(standardControls.Source),
            GetRequiredUriText(reactiveControls.Source),
            standardThemeSources,
            reactiveThemeSources);
    }

    /// <summary>Determines whether a source collection contains a URI with the requested suffix.</summary>
    /// <param name="sources">The source URI collection.</param>
    /// <param name="suffix">The required URI suffix.</param>
    /// <returns><c>true</c> when a matching source exists.</returns>
    private static bool ContainsSuffix(IEnumerable<string> sources, string suffix)
    {
        foreach (string source in sources)
        {
            if (source.EndsWith(suffix, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Converts a required dictionary source URI to its string representation.</summary>
    /// <param name="source">The dictionary source URI.</param>
    /// <returns>The non-null URI text.</returns>
    private static string GetRequiredUriText(Uri? source) => source?.ToString() ?? string.Empty;

    /// <summary>Runs resource loading on a WPF-compatible STA thread.</summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="action">The resource-loading action.</param>
    /// <returns>A task that completes with the result.</returns>
    private static Task<TResult> RunOnStaThreadAsync<TResult>(Func<TResult> action)
    {
        TaskCompletionSource<TResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Thread thread = new(
            () =>
            {
                try
                {
                    completion.SetResult(action());
                }
                catch (Exception exception)
                {
                    completion.SetException(exception);
                }
            });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return completion.Task;
    }

    /// <summary>Captures resource pack URIs for both package variants.</summary>
    /// <param name="StandardControlsSource">The standard control dictionary URI.</param>
    /// <param name="ReactiveControlsSource">The reactive control dictionary URI.</param>
    /// <param name="StandardThemeSources">The standard theme dictionary URIs.</param>
    /// <param name="ReactiveThemeSources">The reactive theme dictionary URIs.</param>
    private sealed record ResourceSnapshot(
        string StandardControlsSource,
        string ReactiveControlsSource,
        IReadOnlyCollection<string> StandardThemeSources,
        IReadOnlyCollection<string> ReactiveThemeSources);
}
