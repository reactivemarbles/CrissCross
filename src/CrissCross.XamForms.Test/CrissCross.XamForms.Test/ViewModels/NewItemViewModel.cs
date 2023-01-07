// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Linq;
using CrissCross;
using CrissCross.XamForms.Test.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CrissCross.XamForms.Test.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        public NewItemViewModel()
        {
            SaveCommand = ReactiveCommand.Create(OnSave, ValidateSave());
            CancelCommand = ReactiveCommand.Create(OnCancel);
        }

        [Reactive]
        public string Text { get; set; }

        [Reactive]
        public string Description { get; set; }

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        private void OnCancel()
        {
            this.NavigateBack();
        }

        private IObservable<bool> ValidateSave()
        {
            return this.WhenAnyValue(vm => vm.Text, vm => vm.Description).Select(x => !String.IsNullOrWhiteSpace(x.Item1) && !String.IsNullOrWhiteSpace(x.Item2));
        }

        private async void OnSave()
        {
            Item newItem = new Item()
            {
                Id = Guid.NewGuid().ToString(),
                Text = Text,
                Description = Description
            };

            await DataStore.AddItemAsync(newItem);
            this.NavigateBack();
        }
    }
}