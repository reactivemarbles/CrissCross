// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.Avalonia.UI.Storage;

/// <summary>
/// Interface for storing and retrieving data.
/// </summary>
public interface IStore
{
    /// <summary>
    /// Lists the ids of all stored data.
    /// </summary>
    /// <returns>A collection of storage ids.</returns>
    IEnumerable<string> ListIds();

    /// <summary>
    /// Sets the data for the specified id.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="values">The values to store.</param>
    void SetData(string id, IDictionary<string, object?> values);

    /// <summary>
    /// Gets the data for the specified id.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The stored values.</returns>
    IDictionary<string, object?> GetData(string id);

    /// <summary>
    /// Clears the data for the specified id.
    /// </summary>
    /// <param name="id">The identifier.</param>
    void ClearData(string id);

    /// <summary>
    /// Clears all stored data.
    /// </summary>
    void ClearAll();
}
