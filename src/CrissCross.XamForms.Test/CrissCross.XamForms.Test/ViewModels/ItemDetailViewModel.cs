// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;

namespace CrissCross.XamForms.Test.ViewModels;

/// <summary>
/// ItemDetailViewModel.
/// </summary>
/// <seealso cref="BaseViewModel" />
[QueryProperty(nameof(ItemId), nameof(ItemId))]
public class ItemDetailViewModel : BaseViewModel
{
    private string? _itemId;

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    [Reactive]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    [Reactive]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    [Reactive]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the item identifier.
    /// </summary>
    /// <value>
    /// The item identifier.
    /// </value>
    public string? ItemId
    {
        get => _itemId;

        set
        {
            this.RaiseAndSetIfChanged(ref _itemId, value);
            LoadItemId(value);
        }
    }

    /// <summary>
    /// Loads the item identifier.
    /// </summary>
    /// <param name="itemId">The item identifier.</param>
    public async void LoadItemId(string? itemId)
    {
        try
        {
            var item = await DataStore.GetItemAsync(itemId);
            Id = item.Id;
            Text = item.Text;
            Description = item.Description;
        }
        catch (Exception)
        {
            Debug.WriteLine("Failed to Load Item");
        }
    }

    /// <summary>
    /// WhenNavigatedTo.
    /// </summary>
    /// <param name="e"></param>
    /// <param name="disposables"></param>
    /// <inheritdoc />
    public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables)
    {
        if (e == null)
        {
            throw new ArgumentNullException(nameof(e));
        }

        ItemId = (string?)e.NavigationParameter;
    }
}
