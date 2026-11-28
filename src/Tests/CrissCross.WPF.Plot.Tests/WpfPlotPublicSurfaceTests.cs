// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.WPF.Plot;
#endif

namespace CrissCross.WPF.Plot.Tests;

/// <summary>Verifies the WPF plot controls are exposed by each package family.</summary>
public sealed class WpfPlotPublicSurfaceTests
{
    /// <summary>Verifies the software and GL controls, menu, and viewer belong to the selected assembly and namespace.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task PlotControls_AreAvailableInSelectedPackage()
    {
#if REACTIVELIST_REACTIVE
        const string expectedNamespace = "CrissCross.Reactive.WPF.Plot";
        const string expectedAssembly = "CrissCross.WPF.Plot.Reactive";
#else
        const string expectedNamespace = "CrissCross.WPF.Plot";
        const string expectedAssembly = "CrissCross.WPF.Plot";
#endif
        foreach (var type in new[] { typeof(WpfPlot), typeof(WpfPlotBase), typeof(WpfPlotGL), typeof(WpfPlotMenu), typeof(WpfPlotViewer) })
        {
            await Assert.That(type.Namespace).IsEqualTo(expectedNamespace);
            await Assert.That(type.Assembly.GetName().Name).IsEqualTo(expectedAssembly);
        }
    }
}
