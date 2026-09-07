// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

global using System.Windows.Input;
global using CrissCross.Maui.UI.Resources.Styles;
global using Microsoft.Maui.Controls;

namespace CrissCross.Maui.UI;

/// <summary>Provides registration helpers for CrissCross MAUI UI resources.</summary>
public static class MauiUiAppBuilderExtensions
{
    /// <summary>Provides extension members for MAUI application builders.</summary>
    /// <param name="builder">The builder value.</param>
    extension(MauiAppBuilder builder)
    {
        /// <summary>Provides the UseCrissCrossMauiUi member.</summary>
        /// <returns>The supplied builder for fluent composition.</returns>
        public MauiAppBuilder UseCrissCrossMauiUi()
        {
            ArgumentNullException.ThrowIfNull(builder);
            return builder.ConfigureFonts(static fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));
        }
    }

    /// <summary>Provides extension members for MAUI resource dictionaries.</summary>
    /// <param name="resources">The resources value.</param>
    extension(ResourceDictionary resources)
    {
        /// <summary>Provides the UseCrissCrossMauiUiResources member.</summary>
        /// <returns>The supplied resource dictionary.</returns>
        public ResourceDictionary UseCrissCrossMauiUiResources()
        {
            ArgumentNullException.ThrowIfNull(resources);
            EnsureBaseResources(resources);
            UseSystemThemeResources(resources, ThemeChoice.Light, useApplicationTheme: true);
            return resources;
        }

        /// <summary>Provides the UseCrissCrossMauiUiResources member for a concrete theme snapshot.</summary>
        /// <param name="themeState">The shared theme preference snapshot.</param>
        /// <returns>The supplied resource dictionary.</returns>
        public ResourceDictionary UseCrissCrossMauiUiResources(ThemePreferenceState? themeState)
        {
            if (themeState is null)
            {
                return resources.UseCrissCrossMauiUiResources();
            }

            if (themeState.SelectedChoice == ThemeChoice.System)
            {
                ArgumentNullException.ThrowIfNull(resources);
                EnsureBaseResources(resources);
                UseSystemThemeResources(resources, themeState.EffectiveChoice, useApplicationTheme: false);
                return resources;
            }

            return resources.UseCrissCrossMauiUiResources(themeState.EffectiveChoice);
        }

        /// <summary>Provides the UseCrissCrossMauiUiResources member for a concrete theme choice.</summary>
        /// <param name="themeChoice">The requested theme choice.</param>
        /// <returns>The supplied resource dictionary.</returns>
        public ResourceDictionary UseCrissCrossMauiUiResources(ThemeChoice themeChoice)
        {
            ArgumentNullException.ThrowIfNull(resources);
            EnsureBaseResources(resources);
            if (themeChoice == ThemeChoice.System)
            {
                UseSystemThemeResources(resources, ThemeChoice.Light, useApplicationTheme: true);
                return resources;
            }

            RemoveSystemThemeSubscription(resources);
            ApplyThemeResources(resources, themeChoice);
            return resources;
        }
    }

    /// <summary>Stores weak resource theme subscriptions without rooting resource dictionaries.</summary>
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<ResourceDictionary, MauiUiThemeSubscription> ThemeSubscriptions = new();

    /// <summary>Adds the base MAUI UI resource dictionary when it is not already present.</summary>
    /// <param name="resources">The target resources.</param>
    private static void EnsureBaseResources(ResourceDictionary resources)
    {
        foreach (var dictionary in resources.MergedDictionaries)
        {
            if (dictionary is CrissCrossMauiUi)
            {
                return;
            }
        }

        resources.MergedDictionaries.Add(new CrissCrossMauiUi());
    }

    /// <summary>Applies system-following resources and subscribes weakly to requested theme changes when an application exists.</summary>
    /// <param name="resources">The target resources.</param>
    /// <param name="fallbackChoice">The fallback concrete choice used when no application is available.</param>
    /// <param name="useApplicationTheme">A value indicating whether to use the current application requested theme immediately.</param>
    private static void UseSystemThemeResources(ResourceDictionary resources, ThemeChoice fallbackChoice, bool useApplicationTheme)
    {
        RemoveSystemThemeSubscription(resources);
        if (Application.Current is { } application)
        {
            var themeChoice = useApplicationTheme ? ResolveThemeChoice(application.RequestedTheme) : fallbackChoice;
            ApplyThemeResources(resources, themeChoice);
            ThemeSubscriptions.Add(resources, new(resources, application));
            return;
        }

        ApplyThemeResources(resources, fallbackChoice);
    }

    /// <summary>Applies the active semantic-token dictionary for the requested concrete theme.</summary>
    /// <param name="resources">The target resources.</param>
    /// <param name="themeChoice">The concrete theme choice.</param>
    private static void ApplyThemeResources(ResourceDictionary resources, ThemeChoice themeChoice)
    {
        RemoveThemeOverrideResources(resources);
        ResourceDictionary themeResources = themeChoice switch
        {
            ThemeChoice.Dark => new CrissCrossMauiDarkTheme(),
            ThemeChoice.HighContrast => new CrissCrossMauiHighContrastTheme(),
            _ => new CrissCrossMauiLightTheme(),
        };
        resources.MergedDictionaries.Add(themeResources);
    }

    /// <summary>Removes the system theme subscription for a resource dictionary.</summary>
    /// <param name="resources">The target resources.</param>
    private static void RemoveSystemThemeSubscription(ResourceDictionary resources)
    {
        if (!ThemeSubscriptions.TryGetValue(resources, out var subscription))
        {
            return;
        }

        subscription.Dispose();
        _ = ThemeSubscriptions.Remove(resources);
    }

    /// <summary>Removes explicit theme override dictionaries before the next choice is applied.</summary>
    /// <param name="resources">The target resources.</param>
    private static void RemoveThemeOverrideResources(ResourceDictionary resources)
    {
        var dictionaries = new List<ResourceDictionary>();
        foreach (var dictionary in resources.MergedDictionaries)
        {
            if (IsThemeOverrideDictionary(dictionary))
            {
                dictionaries.Add(dictionary);
            }
        }

        foreach (var dictionary in dictionaries)
        {
            _ = resources.MergedDictionaries.Remove(dictionary);
        }
    }

    /// <summary>Resolves a MAUI app theme to the shared concrete theme choice.</summary>
    /// <param name="theme">The MAUI app theme.</param>
    /// <returns>The shared concrete theme choice.</returns>
    private static ThemeChoice ResolveThemeChoice(AppTheme theme) => theme == AppTheme.Dark ? ThemeChoice.Dark : ThemeChoice.Light;

    /// <summary>Gets whether the dictionary is a theme override dictionary controlled by CrissCross.</summary>
    /// <param name="dictionary">The candidate resource dictionary.</param>
    /// <returns><c>true</c> when the dictionary is a CrissCross theme override dictionary.</returns>
    private static bool IsThemeOverrideDictionary(ResourceDictionary dictionary) =>
        dictionary is CrissCrossMauiLightTheme or CrissCrossMauiDarkTheme or CrissCrossMauiHighContrastTheme;

    /// <summary>Tracks a system-theme registration without keeping the target resource dictionary alive.</summary>
    private sealed class MauiUiThemeSubscription : IDisposable
    {
        /// <summary>Stores the subscribed resource dictionary without rooting it.</summary>
        private readonly WeakReference<ResourceDictionary> _resources;

        /// <summary>Stores the subscribed application without creating a cycle back to the event source.</summary>
        private readonly WeakReference<Application> _application;

        /// <summary>Initializes a new instance of the <see cref="MauiUiThemeSubscription"/> class.</summary>
        /// <param name="resources">The subscribed resources.</param>
        /// <param name="application">The application that raises requested-theme changes.</param>
        public MauiUiThemeSubscription(ResourceDictionary resources, Application application)
        {
            _resources = new(resources);
            _application = new(application);
            application.RequestedThemeChanged += HandleRequestedThemeChanged;
        }

        /// <summary>Unsubscribes from the requested-theme event.</summary>
        public void Dispose()
        {
            if (!_application.TryGetTarget(out var application))
            {
                return;
            }

            application.RequestedThemeChanged -= HandleRequestedThemeChanged;
        }

        /// <summary>Updates the active theme dictionary when the operating system requested theme changes.</summary>
        /// <param name="sender">The event source.</param>
        /// <param name="args">The requested-theme change arguments.</param>
        private void HandleRequestedThemeChanged(object? sender, AppThemeChangedEventArgs args)
        {
            _ = args;
            if (!_resources.TryGetTarget(out var resources))
            {
                Dispose();
                return;
            }

            var theme = sender is Application application ? application.RequestedTheme : AppTheme.Unspecified;
            ApplyThemeResources(resources, ResolveThemeChoice(theme));
        }
    }
}
