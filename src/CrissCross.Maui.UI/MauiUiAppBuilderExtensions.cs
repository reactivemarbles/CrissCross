// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Maui.UI.Resources.Styles;

namespace CrissCross.Maui.UI;

/// <summary>
/// Provides registration helpers for CrissCross MAUI UI resources.
/// </summary>
public static class MauiUiAppBuilderExtensions
{
    /// <summary>
    /// Adds the CrissCross MAUI UI resource dictionary to the current application when one is available.
    /// </summary>
    /// <param name="builder">The MAUI application builder.</param>
    /// <returns>The supplied builder for fluent composition.</returns>
    public static MauiAppBuilder UseCrissCrossMauiUi(this MauiAppBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));
        return builder;
    }

    /// <summary>
    /// Adds CrissCross MAUI UI resources to the supplied resource dictionary if they are not already present.
    /// </summary>
    /// <param name="resources">The target resource dictionary.</param>
    /// <returns>The supplied resource dictionary.</returns>
    public static ResourceDictionary UseCrissCrossMauiUiResources(this ResourceDictionary resources)
    {
        ArgumentNullException.ThrowIfNull(resources);

        foreach (var dictionary in resources.MergedDictionaries)
        {
            if (dictionary is CrissCrossMauiUi)
            {
                return resources;
            }
        }

        resources.MergedDictionaries.Add(new CrissCrossMauiUi());
        return resources;
    }
}
