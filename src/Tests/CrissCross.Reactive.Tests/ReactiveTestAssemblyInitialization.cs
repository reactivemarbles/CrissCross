// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Reactive.Builder;

namespace CrissCross.Reactive.Tests;

/// <summary>Initializes ReactiveUI before reactive-shim tests create property observables.</summary>
public static class ReactiveTestAssemblyInitialization
{
    /// <summary>Stores the scheduler configured before the test assembly starts.</summary>
    private static IScheduler? _previousMainThreadScheduler;

    /// <summary>Builds the ReactiveUI service graph once for the reactive test assembly.</summary>
    [Before(HookType.Assembly)]
    public static void Initialize()
    {
        _ = RxAppBuilder.CreateReactiveUIBuilder().WithCoreServices().BuildApp();
        _previousMainThreadScheduler = RxSchedulers.MainThreadScheduler;
        RxSchedulers.MainThreadScheduler = ImmediateScheduler.Instance;
    }

    /// <summary>Restores the scheduler configured before the test assembly started.</summary>
    [After(HookType.Assembly)]
    public static void RestoreScheduler()
    {
        if (_previousMainThreadScheduler is null)
        {
            return;
        }

        RxSchedulers.MainThreadScheduler = _previousMainThreadScheduler;
    }
}
