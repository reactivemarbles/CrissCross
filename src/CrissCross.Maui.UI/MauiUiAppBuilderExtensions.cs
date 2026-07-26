// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

global using System.Windows.Input;
global using CrissCross.Maui.UI.Resources.Styles;
global using Microsoft.Maui.Controls;

namespace CrissCross.Maui.UI;

/// <summary>Provides registration helpers for CrissCross MAUI UI resources.</summary>
public static class MauiUiAppBuilderExtensions
{
    /// <summary>Provides extension members for MAUI application builders.</summary>
    /// <param name="builder">The builder value.</param>
    extension(MauiAppBuilder builder)
    {
        /// <summary>Provides the UseCrissCrossMauiUi member.</summary>
        /// <returns>The supplied builder for fluent composition.</returns>
        public MauiAppBuilder UseCrissCrossMauiUi()
        {
            ArgumentNullException.ThrowIfNull(builder);
            return builder.ConfigureFonts(static fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));
        }
    }

    /// <summary>Provides extension members for MAUI resource dictionaries.</summary>
    /// <param name="resources">The resources value.</param>
    extension(ResourceDictionary resources)
    {
        /// <summary>Provides the UseCrissCrossMauiUiResources member.</summary>
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
