// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using BenchmarkDotNet.Attributes;
using ReactiveUI;

namespace CrissCross.WPF.Benchmarks;

/// <summary>Benchmarks the platform-neutral bidirectional navigation registry.</summary>
/// <remarks>This is the default benchmark when the project is run without command-line arguments.</remarks>
[MemoryDiagnoser]
public class NavigationRegistryBenchmark
{
    /// <summary>Provides the number of contract registrations used to simulate an application navigation map.</summary>
    private const int RegistrationCount = 32;

    /// <summary>Stores the configured navigation registry.</summary>
    private NavigationRegistry? _registry;

    /// <summary>Initializes the navigation registry used by the benchmarks.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _registry = new();

        for (var index = 0; index < RegistrationCount; index++)
        {
            _ = _registry.Register(
                static _ => new BenchmarkViewModel(),
                static _ => new BenchmarkView(),
                index.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
    }

    /// <summary>Benchmarks creation of an immutable navigator snapshot.</summary>
    /// <returns>The navigator created from the registry.</returns>
    [Benchmark]
    public IBidirectionalNavigator CreateNavigator() => _registry!.CreateNavigator();

    /// <summary>Provides a view model for registry resolution benchmarks.</summary>
    private sealed class BenchmarkViewModel : RxObject;

    /// <summary>Provides a view for registry resolution benchmarks.</summary>
    private sealed class BenchmarkView : IViewFor<BenchmarkViewModel>
    {
        /// <summary>Gets or sets the view model.</summary>
        public BenchmarkViewModel? ViewModel { get; set; }

        /// <summary>Gets or sets the untyped view model.</summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (BenchmarkViewModel?)value;
        }
    }
}
