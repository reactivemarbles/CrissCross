// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace CrissCross.Avalonia.Benchmarks
{
    /// <summary>
    /// Entry point for running Avalonia benchmarks.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry point for BenchmarkDotNet.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args)
        {
            var config = ManualConfig.CreateEmpty();
            BenchmarkRunner.Run<ViewModelRoutedViewHostBenchmark>(config);
        }
    }
}
