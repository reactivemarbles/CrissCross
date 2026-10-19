// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;
using Splat;
using TUnit.Core.Executors;
using TUnit.Core.Interfaces;
#if REACTIVELIST_REACTIVE
using ReactiveUI.Reactive;
using ReactiveUI.Reactive.Builder;
#else
using ReactiveUI;
using ReactiveUI.Builder;
#endif

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Scopes chart services and scheduler state to a dedicated STA test thread.</summary>
public sealed class WpfHoverTestExecutor : ITestExecutor
{
    /// <inheritdoc/>
    public ValueTask ExecuteTest(TestContext context, Func<ValueTask> action) =>
        new STAThreadExecutor().ExecuteTest(context, async () =>
        {
            _ = Dispatcher.CurrentDispatcher;
            using var resolver = new ModernDependencyResolver();
            using var scope = resolver.WithResolver();
            var previousScheduler = RxSchedulers.MainThreadScheduler;
            try
            {
                _ = resolver.CreateReactiveUIBuilder().WithWpf().WithMainThreadScheduler(ImmediateScheduler.Instance).BuildApp();
                RxSchedulers.MainThreadScheduler = ImmediateScheduler.Instance;
                await action();
            }
            finally
            {
                RxSchedulers.MainThreadScheduler = previousScheduler;
            }
        });
}
