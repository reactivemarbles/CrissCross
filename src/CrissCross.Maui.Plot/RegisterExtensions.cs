// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using SkiaSharp.Views.Maui.Controls.Hosting;

#if REACTIVELIST_REACTIVE
namespace CrissCross.Reactive.Maui.Plot;
#else
namespace CrissCross.Maui.Plot;
#endif

/// <summary>Provides MAUI service registration extensions for plot controls.</summary>
public static class RegisterExtensions
{
    /// <summary>Provides extension methods for configuring MAUI plot services.</summary>
    /// <param name="builder">The MAUI application builder.</param>
    extension(MauiAppBuilder builder)
    {
        /// <summary>Registers the SkiaSharp services required by the MAUI plot controls.</summary>
        /// <returns>The configured application builder.</returns>
        public MauiAppBuilder UseCrissCrossPlot()
        {
            ArgumentNullException.ThrowIfNull(builder);
            _ = builder.UseSkiaSharp();
            return builder;
        }
    }
}
