// Copyright (c) 2016-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Storage;

/// <summary>Interface for storing and retrieving data.</summary>
public interface IStore
{
    /// <summary>Lists the ids of all stored data.</summary>
    /// <returns>A collection of storage ids.</returns>
    IEnumerable<string> ListIds();

    /// <summary>Sets the data for the specified id.</summary>
    /// <param name="id">The storage id.</param>
    /// <param name="values">The values.</param>
    void SetData(string id, IDictionary<string, object?> values);

    /// <summary>Gets the data for the specified id.</summary>
    /// <param name="id">The storage id.</param>
    /// <returns>The stored values.</returns>
    IDictionary<string, object?> GetData(string id);

    /// <summary>Clears the data for the specified id.</summary>
    /// <param name="id">The storage id.</param>
    void ClearData(string id);

    /// <summary>Clears all stored data.</summary>
    void ClearAll();
}
