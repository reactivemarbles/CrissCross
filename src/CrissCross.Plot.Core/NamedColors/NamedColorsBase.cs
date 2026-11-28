// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.NamedColors;

/// <summary>Provides enumeration of named colors exposed by derived color tables.</summary>
public class NamedColorsBase : INamedColors
{
    /// <summary>Gets every color exposed by the concrete derived color table.</summary>
    /// <returns>An array containing the colors declared by the derived type.</returns>
    [System.Diagnostics.CodeAnalysis.DynamicDependency(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods, typeof(XkcdColors))]
    [System.Diagnostics.CodeAnalysis.DynamicDependency(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods, typeof(WebColors))]
    [System.Diagnostics.CodeAnalysis.DynamicDependency(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods, typeof(WindowsVistaColors))]
    [System.Diagnostics.CodeAnalysis.DynamicDependency(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods, typeof(WindowsStandardColors))]
    [System.Diagnostics.CodeAnalysis.DynamicDependency(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicMethods, typeof(WindowsClassicColors))]
    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage(
        "Trimming",
        "IL2075",
        Justification = "NamedColorsBase is extensible, so its runtime type cannot be statically annotated; built-in color table methods are preserved with DynamicDependency attributes.")]
    public Color[] GetAllColors()
    {
        List<Color> colors = [];
        foreach (var method in GetType().GetMethods())
        {
            if (method.ReturnType == typeof(Color))
            {
                colors.Add((Color)method.Invoke(null, null)!);
            }
        }

        return colors.ToArray();
    }
}
