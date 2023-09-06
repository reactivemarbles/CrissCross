// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Linq;
using CrissCross.XamForms.Test.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrissCross.XamForms.Test.ViewModels;

/// <summary>
/// NewItemViewModel.
/// </summary>
/// <seealso cref="BaseViewModel" />
public class NewItemViewModel : BaseViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NewItemViewModel"/> class.
    /// </summary>
    public NewItemViewModel()
    {
        SaveCommand = ReactiveCommand.Create(OnSave, ValidateSave());
        CancelCommand = ReactiveCommand.Create(OnCancel);
    }

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
    /// Gets the save command.
    /// </summary>
    /// <value>
    /// The save command.
    /// </value>
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    /// <summary>
    /// Gets the cancel command.
    /// </summary>
    /// <value>
    /// The cancel command.
    /// </value>
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    private void OnCancel()
    {
        this.NavigateBack();
    }

    private IObservable<bool> ValidateSave()
    {
        return this.WhenAnyValue(vm => vm.Text, vm => vm.Description).Select(x => !string.IsNullOrWhiteSpace(x.Item1) && !string.IsNullOrWhiteSpace(x.Item2));
    }

    private async void OnSave()
    {
        var newItem = new Item()
        {
            Id = Guid.NewGuid().ToString(),
            Text = Text!,
            Description = Description!
        };

        await DataStore.AddItemAsync(newItem);
        this.NavigateBack();
    }
}
