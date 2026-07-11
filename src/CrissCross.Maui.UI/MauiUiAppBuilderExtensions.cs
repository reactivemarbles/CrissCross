// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.Maui.UI.Resources.Styles;

namespace CrissCross.Maui.UI;

/// <summary>Provides registration helpers for CrissCross MAUI UI resources.</summary>
public static class MauiUiAppBuilderExtensions
{
    /// <summary>Provides extension members for MAUI application builders.</summary>
    /// <param name="builder">The builder value.</param>
    extension(MauiAppBuilder builder)
    {
        /// <summary>Adds the CrissCross MAUI UI resource dictionary to the current application when one is available.</summary>
        /// <returns>The supplied builder for fluent composition.</returns>
        public MauiAppBuilder UseCrissCrossMauiUi()
        {
            ArgumentNullException.ThrowIfNull(builder);
            return builder.ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));
        }
    }

    /// <summary>Provides extension members for MAUI resource dictionaries.</summary>
    /// <param name="resources">The resources value.</param>
    extension(ResourceDictionary resources)
    {
        /// <summary>Adds CrissCross MAUI UI resources to the supplied resource dictionary if they are not already present.</summary>
        /// <returns>The supplied resource dictionary.</returns>
        public ResourceDictionary UseCrissCrossMauiUiResources()
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
}
