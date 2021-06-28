using CrissCross;
using CrissCross.XamForms.Test.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace CrissCross.XamForms.Test.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        public NewItemViewModel()
        {
            SaveCommand = ReactiveCommand.Create(OnSave, ValidateSave());
            CancelCommand = ReactiveCommand.Create(OnCancel);
        }

        private IObservable<bool> ValidateSave()
        {
            return this.WhenAnyValue(vm => vm.Text, vm => vm.Description).Select(x => !String.IsNullOrWhiteSpace(x.Item1) && !String.IsNullOrWhiteSpace(x.Item2));
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