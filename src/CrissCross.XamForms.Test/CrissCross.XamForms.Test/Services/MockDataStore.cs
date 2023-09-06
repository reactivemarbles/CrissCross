// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrissCross.XamForms.Test.Models;

namespace CrissCross.XamForms.Test.Services;

/// <summary>
/// MockDataStore.
/// </summary>
public class MockDataStore : IDataStore<Item>
{
    private readonly List<Item> _items;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockDataStore"/> class.
    /// </summary>
    public MockDataStore()
    {
        _items = new List<Item>()
        {
            new Item { Id = Guid.NewGuid().ToString(), Text = "First item", Description = "This is an item description." },
            new Item { Id = Guid.NewGuid().ToString(), Text = "Second item", Description = "This is an item description." },
            new Item { Id = Guid.NewGuid().ToString(), Text = "Third item", Description = "This is an item description." },
            new Item { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description = "This is an item description." },
            new Item { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description = "This is an item description." },
            new Item { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description = "This is an item description." }
        };
    }

    /// <summary>
    /// Adds the item asynchronous.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>A Bool.</returns>
    public async Task<bool> AddItemAsync(Item item)
    {
        _items.Add(item);

        return await Task.FromResult(true);
    }

    /// <summary>
    /// Updates the item asynchronous.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>A Bool.</returns>
    public async Task<bool> UpdateItemAsync(Item item)
    {
        var oldItem = _items.Find((Item arg) => arg.Id == item.Id);
        _items.Remove(oldItem);
        _items.Add(item);

        return await Task.FromResult(true);
    }

    /// <summary>
    /// Deletes the item asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>A Bool.</returns>
    public async Task<bool> DeleteItemAsync(string? id)
    {
        var oldItem = _items.Find((Item arg) => arg.Id == id);
        _items.Remove(oldItem);

        return await Task.FromResult(true);
    }

    /// <summary>
    /// Gets the item asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>A Item.</returns>
    public async Task<Item> GetItemAsync(string? id) =>
        await Task.FromResult(_items.Find(s => s.Id == id));

    /// <summary>
    /// Gets the items asynchronous.
    /// </summary>
    /// <param name="forceRefresh">if set to <c>true</c> [force refresh].</param>
    /// <returns>An IEnum.</returns>
    public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false) =>
        await Task.FromResult(_items);
}
