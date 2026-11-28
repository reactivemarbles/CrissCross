// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Avalonia.Plot;
#endif

namespace CrissCross.Avalonia.Plot.Tests;

/// <summary>Verifies the Avalonia plot controls are exposed by each package family.</summary>
public sealed class AvaPlotPublicSurfaceTests
{
    /// <summary>Verifies the control and menu belong to the selected assembly and namespace.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task PlotControls_AreAvailableInSelectedPackage()
    {
#if REACTIVELIST_REACTIVE
        const string expectedNamespace = "CrissCross.Reactive.Avalonia.Plot";
        const string expectedAssembly = "CrissCross.Avalonia.Plot.Reactive";
#else
        const string expectedNamespace = "CrissCross.Avalonia.Plot";
        const string expectedAssembly = "CrissCross.Avalonia.Plot";
#endif
        foreach (var type in new[] { typeof(AvaPlot), typeof(AvaPlotMenu) })
        {
            await Assert.That(type.Namespace).IsEqualTo(expectedNamespace);
            await Assert.That(type.Assembly.GetName().Name).IsEqualTo(expectedAssembly);
        }
    }
}
