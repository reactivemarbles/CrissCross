using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using System.Reactive.Disposables;
using CrissCross;

namespace CrissCross.XamForms.Test.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private string itemId;

        [Reactive]
        public string Id { get; set; }

        [Reactive]
        public string Text { get; set; }

        [Reactive]
        public string Description { get; set; }

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref itemId, value);
                LoadItemId(value);
            }
        }

        public async void LoadItemId(string itemId)
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

        public override void WhenNavigatedTo(IViewModelNavigationEventArgs e, CompositeDisposable disposables)
        {
            ItemId = (string)e.NavigationParameter;
        }
    }
}