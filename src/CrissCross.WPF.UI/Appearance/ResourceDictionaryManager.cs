// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace CrissCross.WPF.UI.Appearance;

/// <summary>Allows managing application dictionaries.</summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ResourceDictionaryManager"/> class.
/// </remarks>
/// <param name="searchNamespace">The search namespace.</param>
internal sealed class ResourceDictionaryManager(string searchNamespace)
{
    /// <summary>Gets the namespace, e.g. the library the resource is being searched for.</summary>
    public string SearchNamespace { get; } = searchNamespace;

    /// <summary>Shows whether the application contains the <see cref="ResourceDictionary"/>.</summary>
    /// <param name="resourceLookup">Any part of the resource name.</param>
    /// <returns><see langword="false"/> if it doesn't exist.</returns>
    public bool HasDictionary(string resourceLookup) => GetDictionary(resourceLookup) is not null;

    /// <summary>Gets the <see cref="ResourceDictionary"/> if exists.</summary>
    /// <param name="resourceLookup">Any part of the resource name.</param>
    /// <returns><see cref="ResourceDictionary"/>, <see langword="null"/> if it doesn't exist.</returns>
    public ResourceDictionary? GetDictionary(string resourceLookup)
    {
        var applicationDictionaries = GetApplicationMergedDictionaries();

        if (applicationDictionaries.Count == 0)
        {
            return null;
        }

        resourceLookup = resourceLookup.ToLower().Trim();

        foreach (var dictionary in applicationDictionaries)
        {
            if (IsMatchingDictionary(dictionary, resourceLookup))
            {
                return dictionary;
            }

            var mergedDictionary = dictionary.MergedDictionaries.FirstOrDefault(child => IsMatchingDictionary(child, resourceLookup));
            if (mergedDictionary is not null)
            {
                return mergedDictionary;
            }
        }

        return null;
    }

    /// <summary>Shows whether the application contains the <see cref="ResourceDictionary"/>.</summary>
    /// <param name="resourceLookup">Any part of the resource name.</param>
    /// <param name="newResourceUri">A valid <see cref="Uri"/> for the replaced resource.</param>
    /// <returns><see langword="true"/> if the dictionary <see cref="Uri"/> was updated. <see langword="false"/> otherwise.</returns>
    public bool UpdateDictionary(string resourceLookup, Uri? newResourceUri)
    {
        var applicationDictionaries = UiApplication
            .Current
            .Resources
            .MergedDictionaries;

        if (applicationDictionaries.Count == 0 || newResourceUri is null)
        {
            return false;
        }

        resourceLookup = resourceLookup.ToLower().Trim();

        return TryUpdateDictionary(applicationDictionaries, resourceLookup, newResourceUri);
    }

    /// <summary>Provides the GetApplicationMergedDictionaries member.</summary>
    /// <returns>The result.</returns>
    private static Collection<ResourceDictionary> GetApplicationMergedDictionaries() => UiApplication.Current.Resources.MergedDictionaries;

    /// <summary>Determines whether the dictionary source matches the lookup.</summary>
    /// <param name="dictionary">The resource dictionary.</param>
    /// <param name="resourceLookup">The resource lookup text.</param>
    /// <returns><c>true</c> if the dictionary matches; otherwise, <c>false</c>.</returns>
    private bool IsMatchingDictionary(ResourceDictionary? dictionary, string resourceLookup)
    {
        var sourceUri = dictionary?.Source?.ToString().ToLower().Trim();
        return sourceUri?.Contains(SearchNamespace) == true && sourceUri.Contains(resourceLookup);
    }

    /// <summary>Updates the first matching dictionary in the collection.</summary>
    /// <param name="dictionaries">The dictionaries to inspect.</param>
    /// <param name="resourceLookup">The resource lookup text.</param>
    /// <param name="newResourceUri">The replacement resource URI.</param>
    /// <returns><c>true</c> if a dictionary was updated; otherwise, <c>false</c>.</returns>
    private bool TryUpdateDictionary(Collection<ResourceDictionary> dictionaries, string resourceLookup, Uri newResourceUri)
    {
        for (var i = 0; i < dictionaries.Count; i++)
        {
            if (IsMatchingDictionary(dictionaries[i], resourceLookup))
            {
                dictionaries[i] = new() { Source = newResourceUri };
                return true;
            }

            if (TryUpdateDictionary(dictionaries[i].MergedDictionaries, resourceLookup, newResourceUri))
            {
                return true;
            }
        }

        return false;
    }
}
