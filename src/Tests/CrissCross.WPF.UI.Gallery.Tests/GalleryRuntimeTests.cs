// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CrissCross.WPF.UI.Appearance;
using CrissCross.WPF.UI.Controls;
using CrissCross.WPF.UI.Gallery;
using CrissCross.WPF.UI.Gallery.Views;
using ReactiveUI.Reactive.Builder;
using ReactiveBuilder = ReactiveUI.Reactive.Builder.RxAppBuilder;
using WpfWindow = System.Windows.Window;

namespace CrissCross.WPF.UI.Gallery.Tests;

/// <summary>Exercises the WPF gallery through a real visual tree.</summary>
public sealed class GalleryRuntimeTests
{
    /// <summary>The minimum number of CrissCross controls expected in the catalog tree.</summary>
    private const int MinimumCatalogControlCount = 40;

    /// <summary>The minimum number of constructible gallery pages expected to render.</summary>
    private const int MinimumGalleryPageCount = 20;

    /// <summary>The minimum number of public WPF UI element types expected to construct.</summary>
    private const int MinimumPublicUiElementCount = 100;

    /// <summary>The gallery validation window height.</summary>
    private const double GalleryWindowHeight = 900D;

    /// <summary>The gallery validation window width.</summary>
    private const double GalleryWindowWidth = 1_600D;

    /// <summary>Verifies that the catalog loads, activates, and remains readable under both supported themes.</summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Test]
    public async Task ControlCatalog_WhenRendered_OperatesInDarkAndLightThemes()
    {
        var snapshot = await RunOnStaThreadAsync(RenderCatalog);

        await Assert.That(snapshot.CrissCrossControlCount).IsGreaterThanOrEqualTo(MinimumCatalogControlCount);
        await Assert.That(snapshot.ConstructedPublicUiElementCount).IsGreaterThanOrEqualTo(MinimumPublicUiElementCount);
        await Assert.That(snapshot.ConstructedReactiveUiElementCount).IsEqualTo(snapshot.ConstructedPublicUiElementCount);
        await Assert.That(snapshot.RoundTrippedPublicPropertyCount).IsGreaterThan(0);
        await Assert.That(snapshot.RoundTrippedReactivePropertyCount).IsEqualTo(snapshot.RoundTrippedPublicPropertyCount);
        await Assert.That(snapshot.DarkRenderedPageCount).IsGreaterThanOrEqualTo(MinimumGalleryPageCount);
        await Assert.That(snapshot.LightRenderedPageCount).IsEqualTo(snapshot.DarkRenderedPageCount);
        await Assert.That(snapshot.SnackbarIsShown).IsTrue();
        await Assert.That(snapshot.DarkTheme).IsEqualTo(ApplicationTheme.Dark);
        await Assert.That(snapshot.LightTheme).IsEqualTo(ApplicationTheme.Light);
    }

    /// <summary>Renders the real catalog and captures the observable theme/control state.</summary>
    /// <returns>The rendered catalog snapshot.</returns>
    private static GallerySnapshot RenderCatalog()
    {
        var application = new App();
        application.InitializeComponent();

        var window = new MainWindow { Height = GalleryWindowHeight, ShowInTaskbar = false, Width = GalleryWindowWidth, WindowStartupLocation = WindowStartupLocation.Manual, };

        try
        {
            window.Show();
            var catalog = new ControlCatalogView();
            window.SetCurrentValue(System.Windows.Controls.ContentControl.ContentProperty, catalog);
            DrainDispatcher();
            window.UpdateLayout();

            var controlCount = 0;
            var snackbarIsShown = false;
            foreach (var descendant in EnumerateVisualDescendants(catalog))
            {
                if (descendant.GetType().Namespace?.StartsWith("CrissCross.WPF.UI", StringComparison.Ordinal) == true)
                {
                    controlCount++;
                }

                if (descendant is Snackbar { IsShown: true })
                {
                    snackbarIsShown = true;
                }
            }

            var publicSurface = ExerciseEveryPublicUiElement(typeof(Snackbar).Assembly);
            _ = ReactiveBuilder.CreateReactiveUIBuilder().WithWpf().BuildApp();
            var reactiveControls = new CrissCross.Reactive.WPF.UI.Markup.ControlsDictionary();
            application.Resources.MergedDictionaries.Add(reactiveControls);
            var reactiveSurface = ExerciseEveryPublicUiElement(
                typeof(CrissCross.Reactive.WPF.UI.Controls.Snackbar).Assembly);
            _ = application.Resources.MergedDictionaries.Remove(reactiveControls);

            ApplicationThemeManager.Apply(ApplicationTheme.Dark, WindowBackdropType.None, false);
            DrainDispatcher();
            var darkTheme = ApplicationThemeManager.GetAppTheme();
            var darkRenderedPageCount = RenderEveryGalleryPage(window);

            ApplicationThemeManager.Apply(ApplicationTheme.Light, WindowBackdropType.None, false);
            DrainDispatcher();
            var lightTheme = ApplicationThemeManager.GetAppTheme();
            var lightRenderedPageCount = RenderEveryGalleryPage(window);

            return new(
                controlCount,
                publicSurface.ElementCount,
                reactiveSurface.ElementCount,
                publicSurface.RoundTrippedPropertyCount,
                reactiveSurface.RoundTrippedPropertyCount,
                darkRenderedPageCount,
                lightRenderedPageCount,
                snackbarIsShown,
                darkTheme,
                lightTheme);
        }
        finally
        {
            window.Close();
            application.Shutdown();
        }
    }

    /// <summary>Runs queued layout, activation, and template work to application-idle priority.</summary>
    private static void DrainDispatcher() =>
        Dispatcher.CurrentDispatcher.Invoke(static () => { }, DispatcherPriority.ApplicationIdle);

    /// <summary>Constructs every exported, concrete, parameterless WPF UI element type.</summary>
    /// <returns>The number of constructed types.</returns>
    /// <param name="assembly">The UI assembly to exercise.</param>
    private static PublicUiSurfaceSnapshot ExerciseEveryPublicUiElement(Assembly assembly)
    {
        var elements = new List<FrameworkElement>();
        var roundTrippedPropertyCount = 0;

        foreach (var type in assembly.GetExportedTypes())
        {
            if (!IsConstructiblePublicUiElement(type))
            {
                continue;
            }

            var element = (FrameworkElement)Activator.CreateInstance(type)!;
            elements.Add(element);
            roundTrippedPropertyCount += RoundTripPublicCrissCrossProperties(type, element);
        }

        foreach (var element in elements)
        {
            if (element is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        return new(elements.Count, roundTrippedPropertyCount);
    }

    /// <summary>Determines whether an exported type can participate in the public UI surface test.</summary>
    /// <param name="type">The exported UI type.</param>
    /// <returns><c>true</c> when the type can be constructed safely by the test.</returns>
    private static bool IsConstructiblePublicUiElement(Type type) =>
        type is { IsAbstract: false, IsPublic: true }
        && !type.ContainsGenericParameters
        && typeof(FrameworkElement).IsAssignableFrom(type)
        && !typeof(WpfWindow).IsAssignableFrom(type)
        && type.GetConstructor(Type.EmptyTypes) is not null;

    /// <summary>Reads and writes public CrissCross properties to exercise their dependency-property surface.</summary>
    /// <param name="type">The concrete UI element type.</param>
    /// <param name="element">The constructed element.</param>
    /// <returns>The number of properties exercised.</returns>
    private static int RoundTripPublicCrissCrossProperties(Type type, FrameworkElement element)
    {
        var roundTrippedPropertyCount = 0;
        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (
                property is not { CanRead: true, SetMethod.IsPublic: true }
                || property.GetIndexParameters().Length != 0
                || property.DeclaringType?.Namespace?.StartsWith("CrissCross.", StringComparison.Ordinal) != true)
            {
                continue;
            }

            var value = property.GetValue(element);
            property.SetValue(element, value);
            roundTrippedPropertyCount++;
        }

        return roundTrippedPropertyCount;
    }

    /// <summary>Constructs and renders every public gallery page with a parameterless constructor.</summary>
    /// <param name="window">The gallery host window.</param>
    /// <returns>The number of rendered pages.</returns>
    private static int RenderEveryGalleryPage(MainWindow window)
    {
        var renderedPageCount = 0;

        foreach (var pageType in typeof(ControlCatalogView).Assembly.GetTypes())
        {
            if (
                pageType is not { IsAbstract: false, IsPublic: true }
                || pageType.Namespace?.StartsWith(
                    "CrissCross.WPF.UI.Gallery.Views",
                    StringComparison.Ordinal) != true
                || !typeof(FrameworkElement).IsAssignableFrom(pageType)
                || pageType.GetConstructor(Type.EmptyTypes) is null)
            {
                continue;
            }

            var page = (FrameworkElement)Activator.CreateInstance(pageType)!;
            window.SetCurrentValue(System.Windows.Controls.ContentControl.ContentProperty, page);
            DrainDispatcher();
            window.UpdateLayout();
            renderedPageCount++;
        }

        return renderedPageCount;
    }

    /// <summary>Enumerates the complete rendered visual subtree.</summary>
    /// <param name="root">The subtree root.</param>
    /// <returns>The descendants in depth-first order.</returns>
    private static IEnumerable<DependencyObject> EnumerateVisualDescendants(DependencyObject root)
    {
        var childCount = VisualTreeHelper.GetChildrenCount(root);
        for (var index = 0; index < childCount; index++)
        {
            var child = VisualTreeHelper.GetChild(root, index);
            yield return child;

            foreach (var descendant in EnumerateVisualDescendants(child))
            {
                yield return descendant;
            }
        }
    }

    /// <summary>Runs work on an STA thread suitable for WPF controls.</summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="action">The WPF action.</param>
    /// <returns>A task that completes with the action result.</returns>
    private static Task<TResult> RunOnStaThreadAsync<TResult>(Func<TResult> action)
    {
        var completion = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(
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

    /// <summary>Captures observable state from a rendered catalog.</summary>
    /// <param name="CrissCrossControlCount">The number of rendered CrissCross controls.</param>
    /// <param name="ConstructedPublicUiElementCount">The number of constructed public WPF UI element types.</param>
    /// <param name="ConstructedReactiveUiElementCount">The number of constructed reactive WPF UI element types.</param>
    /// <param name="RoundTrippedPublicPropertyCount">The number of standard public properties exercised.</param>
    /// <param name="RoundTrippedReactivePropertyCount">The number of reactive public properties exercised.</param>
    /// <param name="DarkRenderedPageCount">The number of gallery pages rendered with the Dark theme.</param>
    /// <param name="LightRenderedPageCount">The number of gallery pages rendered with the Light theme.</param>
    /// <param name="SnackbarIsShown">Whether the composed Snackbar is visible.</param>
    /// <param name="DarkTheme">The theme observed after applying Dark.</param>
    /// <param name="LightTheme">The theme observed after applying Light.</param>
    private sealed record GallerySnapshot(
        int CrissCrossControlCount,
        int ConstructedPublicUiElementCount,
        int ConstructedReactiveUiElementCount,
        int RoundTrippedPublicPropertyCount,
        int RoundTrippedReactivePropertyCount,
        int DarkRenderedPageCount,
        int LightRenderedPageCount,
        bool SnackbarIsShown,
        ApplicationTheme DarkTheme,
        ApplicationTheme LightTheme);

    /// <summary>Captures the exercised public WPF UI surface.</summary>
    /// <param name="ElementCount">The number of constructed element types.</param>
    /// <param name="RoundTrippedPropertyCount">The number of public properties read and written.</param>
    private sealed record PublicUiSurfaceSnapshot(int ElementCount, int RoundTrippedPropertyCount);
}
