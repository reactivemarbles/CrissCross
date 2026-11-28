// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace CrissCross.Plot.Testing;

/// <summary>Detects duplicate identifiers and reports the associated items.</summary>
/// <typeparam name="T">The type of items associated with each identifier.</typeparam>
/// <param name="title">The title used in duplicate error messages.</param>
public class DuplicateIdentifier<T>(string title)
{
    /// <summary>The identifiers and items to validate.</summary>
    private readonly List<KeyValuePair<string, T>> _things = [];

    /// <summary>The title used in duplicate error messages.</summary>
    private readonly string _title = title;

    /// <summary>Adds an item with its identifier.</summary>
    /// <param name="id">The identifier to check for duplicates.</param>
    /// <param name="thing">The item associated with the identifier.</param>
    public void Add(string id, T thing) => _things.Add(new(id, thing));

    /// <summary>Throws an exception if any identifier has been added more than once.</summary>
    public void ShouldHaveNoDuplicates()
    {
        HashSet<string> duplicateIDs = new();
        HashSet<string> seenIDs = new();
        for (int i = 0; i < _things.Count; i++)
        {
            string id = _things[i].Key;
            if (seenIDs.Contains(id))
            {
                _ = duplicateIDs.Add(id);
            }

            _ = seenIDs.Add(id);
        }

        if (duplicateIDs.Count == 0)
        {
            return;
        }

        StringBuilder sb = new();
        foreach (string id in duplicateIDs)
        {
            List<T> duplicateThings = [];
            foreach (KeyValuePair<string, T> thing in _things)
            {
                if (thing.Key == id)
                {
                    duplicateThings.Add(thing.Value);
                }
            }

            string duplicateThingsString = string.Join(", ", duplicateThings);
            _ = sb.AppendLine($"The {_title} \"{id}\" is not unique as it is shared by: {duplicateThingsString}");
        }

        throw new InvalidOperationException(sb.ToString());
    }
}
