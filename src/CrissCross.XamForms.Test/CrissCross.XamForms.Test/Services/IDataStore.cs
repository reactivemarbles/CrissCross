// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrissCross.XamForms.Test.Services;

/// <summary>
/// IDataStore.
/// </summary>
/// <typeparam name="T">The Type.</typeparam>
public interface IDataStore<T>
{
    /// <summary>
    /// Adds the item asynchronous.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>A Bool.</returns>
    Task<bool> AddItemAsync(T item);

    /// <summary>
    /// Updates the item asynchronous.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>A Bool.</returns>
    Task<bool> UpdateItemAsync(T item);

    /// <summary>
    /// Deletes the item asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>A Bool.</returns>
    Task<bool> DeleteItemAsync(string? id);

    /// <summary>
    /// Gets the item asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>A Value of T.</returns>
    Task<T> GetItemAsync(string? id);

    /// <summary>
    /// Gets the items asynchronous.
    /// </summary>
    /// <param name="forceRefresh">if set to <c>true</c> [force refresh].</param>
    /// <returns>An Enumerable.</returns>
    Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
}
