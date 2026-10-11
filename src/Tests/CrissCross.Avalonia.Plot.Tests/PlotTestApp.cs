// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media.Imaging;
#if REACTIVE_SHIM
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Themes.Fluent;
using ReactiveUI.Avalonia.Reactive;
using ReactiveUI.Reactive;
#else
using ReactiveUI;
using ReactiveUI.Avalonia;
#endif

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Provides an isolated Avalonia UI dispatcher for plot integration tests.</summary>
public static class PlotTestApp
{
    /// <summary>Saves a rendered window for native control visual verification.</summary>
    /// <param name="window">The displayed window.</param>
    /// <param name="name">The snapshot filename.</param>
    public static void SaveSnapshot(Window window, string name)
    {
        using var bitmap = new RenderTargetBitmap(new((int)window.Bounds.Width, (int)window.Bounds.Height));
        bitmap.Render(window);
        var directory = Path.Combine(AppContext.BaseDirectory, "artifacts", "visuals");
        _ = Directory.CreateDirectory(directory);
        bitmap.Save(Path.Combine(directory, name), new PngBitmapEncoderOptions());
    }

    /// <summary>Creates the headless application used by the test session.</summary>
    /// <returns>The configured application builder.</returns>
    public static AppBuilder BuildAvaloniaApp() =>
#if REACTIVELIST_REACTIVE
        AppBuilder.Configure<Application>()
        .AfterSetup(static builder =>
        {
            var application = builder.Instance!;
            application.Styles.Add(new FluentTheme());
            Uri themeUri = new("avares://CrissCross.Avalonia.UI.Reactive/Themes/Index.axaml");
            application.Styles.Add(new StyleInclude(themeUri) { Source = themeUri });
        })
#else
        AppBuilder.Configure<Example.App>()
#endif
        .UseHeadless(new() { UseHeadlessDrawing = false })
        .UseSkia()
        .UseReactiveUI(static _ => { });

    /// <summary>Runs chart interactions on a dedicated Avalonia UI dispatcher.</summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="action">The chart interaction.</param>
    /// <returns>The interaction result.</returns>
    internal static async Task<T> Run<T>(Func<T> action)
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(PlotTestApp));
        return await session.Dispatch(
            () =>
        {
            var previous = RxSchedulers.MainThreadScheduler;
            var previousTaskpool = RxSchedulers.TaskpoolScheduler;
            RxSchedulers.MainThreadScheduler = ImmediateScheduler.Instance;
            RxSchedulers.TaskpoolScheduler = ImmediateScheduler.Instance;
            try
            {
                return action();
            }
            finally
            {
                RxSchedulers.MainThreadScheduler = previous;
                RxSchedulers.TaskpoolScheduler = previousTaskpool;
            }
            },
            CancellationToken.None);
    }

    /// <summary>Runs asynchronous UI work with real dispatcher notification scheduling.</summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="action">The asynchronous chart interaction.</param>
    /// <returns>The interaction result.</returns>
    internal static async Task<T> RunAsync<T>(Func<Task<T>> action)
    {
        await using var session = HeadlessUnitTestSession.StartNew(typeof(PlotTestApp));
        return await session.Dispatch(
            async () =>
            {
                var previous = RxSchedulers.MainThreadScheduler;
#if REACTIVE_SHIM
                RxSchedulers.MainThreadScheduler = new SynchronizationContextScheduler(SynchronizationContext.Current!);
#else
                RxSchedulers.MainThreadScheduler = new SynchronizationContextSequencer(SynchronizationContext.Current!);
#endif
                try
                {
                    return await action();
                }
                finally
                {
                    RxSchedulers.MainThreadScheduler = previous;
                }
            },
            CancellationToken.None);
    }
}
