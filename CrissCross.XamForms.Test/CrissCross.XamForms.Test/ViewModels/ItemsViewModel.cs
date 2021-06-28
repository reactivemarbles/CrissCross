using CrissCross;
using CrissCross.XamForms.Test.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;

namespace CrissCross.XamForms.Test.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; }
        public ReactiveCommand<Unit, Unit> LoadItemsCommand { get; }
        public ReactiveCommand<object, Unit> AddItemCommand { get; }
        public ReactiveCommand<Item, Unit> ItemTapped { get; }

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = ReactiveCommand.CreateFromTask(async () => await ExecuteLoadItemsCommand());

            ItemTapped = ReactiveCommand.Create<Item>(OnItemSelected);

            AddItemCommand = ReactiveCommand.Create<object>(OnAddItem);
            LoadItemsCommand.Execute();
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

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        [Reactive]
        public Item SelectedItem { get; set; }

        private void OnAddItem(object obj)
        {
            this.NavigateToView<NewItemViewModel>();
        }

        private void OnItemSelected(Item item)
        {
            if (item == null)
            {
                return;
            }

            this.NavigateToView<ItemDetailViewModel>(parameter: item.Id);
        }
    }
}