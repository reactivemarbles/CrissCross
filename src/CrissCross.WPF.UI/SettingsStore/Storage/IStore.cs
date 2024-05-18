// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace CrissCross.WPF.UI.Storage;

/// <summary>
/// IStore.
/// </summary>
public interface IStore
{
    /// <summary>
    /// Lists the ids.
    /// </summary>
    /// <returns>A string array.</returns>
    IEnumerable<string> ListIds();

    /// <summary>
    /// Sets the data.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="values">The values.</param>
    void SetData(string id, IDictionary<string, object?> values);

    /// <summary>
    /// Gets the data.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>A string array.</returns>
    IDictionary<string, object?> GetData(string id);

    /// <summary>
    /// Clears the data.
    /// </summary>
    /// <param name="id">The identifier.</param>
    void ClearData(string id);

    /// <summary>
    /// Clears all.
    /// </summary>
    void ClearAll();
}
