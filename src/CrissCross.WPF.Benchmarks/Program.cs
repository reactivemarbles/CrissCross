// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using BenchmarkDotNet.Running;

namespace CrissCross.WPF.Benchmarks;

/// <summary>Entry point for running WPF benchmarks.</summary>
public static class Program
{
    /// <summary>Main entry point for BenchmarkDotNet.</summary>
    /// <param name="args">Command-line arguments.</param>
    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            _ = BenchmarkRunner.Run<NavigationRegistryBenchmark>();
            return;
        }

        _ = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
