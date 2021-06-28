using CrissCross.XamForms.Test.ViewModels;
using ReactiveUI;
using Splat;

namespace CrissCross.XamForms.Test.Views
{
    public partial class ItemsPage
    {
        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = Locator.Current.GetService<ItemsViewModel>();
            this.WhenActivated(d =>
            {
                ViewModel.OnAppearing();
            });
        }
    }
}