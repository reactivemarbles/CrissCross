using CrissCross;
using CrissCross.XamForms.Test.Models;
using CrissCross.XamForms.Test.Services;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;

namespace CrissCross.XamForms.Test.ViewModels
{
    public class BaseViewModel : RxObject
    {
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();

        [Reactive]
        public bool IsBusy { get; set; }

        [Reactive]
        public string Title { get; set; }
    }
}