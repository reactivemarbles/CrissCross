// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Hosting;
#if REACTIVELIST_REACTIVE
using CrissCross.Reactive.Maui.Plot;
using PlotRegistration = CrissCross.Reactive.Maui.Plot.RegisterExtensions;
#else
using CrissCross.Maui.Plot;
using PlotRegistration = CrissCross.Maui.Plot.RegisterExtensions;
#endif

namespace CrissCross.Maui.Plot.Tests;

/// <summary>Verifies plot handler registration is available in the selected package family.</summary>
public sealed class RegisterExtensionsTests
{
    /// <summary>Verifies registration is exposed in the selected namespace and returns the original builder.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task UseCrissCrossPlot_RegistersServices_ReturnsBuilder()
    {
        var builder = MauiApp.CreateBuilder();
        var result = builder.UseCrissCrossPlot();

        await Assert.That(result).IsSameReferenceAs(builder);
#if REACTIVELIST_REACTIVE
        await Assert.That(typeof(PlotRegistration).Namespace).IsEqualTo("CrissCross.Reactive.Maui.Plot");
        await Assert.That(typeof(PlotRegistration).Assembly.GetName().Name).IsEqualTo("CrissCross.Maui.Plot.Reactive");
#else
        await Assert.That(typeof(PlotRegistration).Namespace).IsEqualTo("CrissCross.Maui.Plot");
        await Assert.That(typeof(PlotRegistration).Assembly.GetName().Name).IsEqualTo("CrissCross.Maui.Plot");
#endif
    }

    /// <summary>Verifies the MAUI plot control and menu belong to the selected package family.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task PlotControls_AreAvailableInSelectedPackage()
    {
#if REACTIVELIST_REACTIVE
        const string expectedNamespace = "CrissCross.Reactive.Maui.Plot";
        const string expectedAssembly = "CrissCross.Maui.Plot.Reactive";
#else
        const string expectedNamespace = "CrissCross.Maui.Plot";
        const string expectedAssembly = "CrissCross.Maui.Plot";
#endif
        foreach (var type in new[] { typeof(MauiPlot), typeof(MauiPlotMenu) })
        {
            await Assert.That(type.Namespace).IsEqualTo(expectedNamespace);
            await Assert.That(type.Assembly.GetName().Name).IsEqualTo(expectedAssembly);
        }
    }
}
