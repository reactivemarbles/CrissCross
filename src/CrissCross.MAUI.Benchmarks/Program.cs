// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace CrissCross.MAUI.Benchmarks;

/// <summary>Entry point for running MAUI benchmarks.</summary>
public static class Program
{
    /// <summary>Main entry point for BenchmarkDotNet.</summary>
    /// <param name="args">Command-line arguments.</param>
    public static void Main(string[] args)
    {
        var config = ManualConfig.CreateEmpty();
        _ = BenchmarkRunner.Run<NavigationShellBenchmark>(config);
    }
}
