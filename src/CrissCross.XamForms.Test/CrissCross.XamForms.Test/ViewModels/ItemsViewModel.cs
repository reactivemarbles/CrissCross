// Copyright (c) 2019-2024 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using CrissCross.XamForms.Test.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrissCross.XamForms.Test.ViewModels;

/// <summary>
/// ItemsViewModel.
/// </summary>
/// <seealso cref="BaseViewModel" />
public class ItemsViewModel : BaseViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsViewModel"/> class.
    /// </summary>
    public ItemsViewModel()
    {
        Title = "Browse";
        Items = new ObservableCollection<Item>();
        LoadItemsCommand = ReactiveCommand.CreateFromTask(async () => await ExecuteLoadItemsCommand());

        ItemTapped = ReactiveCommand.Create<Item>(OnItemSelected);

        AddItemCommand = ReactiveCommand.Create<object>(OnAddItem);
        LoadItemsCommand.Execute();
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>
    /// The items.
    /// </value>
    public ObservableCollection<Item> Items { get; }

    /// <summary>
    /// Gets the load items command.
    /// </summary>
    /// <value>
    /// The load items command.
    /// </value>
    public ReactiveCommand<Unit, Unit> LoadItemsCommand { get; }

    /// <summary>
    /// Gets the add item command.
    /// </summary>
    /// <value>
    /// The add item command.
    /// </value>
    public ReactiveCommand<object, Unit> AddItemCommand { get; }

    /// <summary>
    /// Gets the item tapped.
    /// </summary>
    /// <value>
    /// The item tapped.
    /// </value>
    public ReactiveCommand<Item, Unit> ItemTapped { get; }

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    /// <value>
    /// The selected item.
    /// </value>
    [Reactive]
    public Item? SelectedItem { get; set; }

    /// <summary>
    /// Called when [appearing].
    /// </summary>
    public void OnAppearing()
    {
        IsBusy = true;
        SelectedItem = null;
    }

    private async Task ExecuteLoadItemsCommand()
    {
        IsBusy = true;

        try
        {
            Items.Clear();
            var items = await DataStore.GetItemsAsync(true);
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnAddItem(object obj) => this.NavigateToView<NewItemViewModel>();

    private void OnItemSelected(Item item)
    {
        if (item == null)
        {
            return;
        }

        this.NavigateToView<ItemDetailViewModel>(parameter: item.Id);
    }
}
