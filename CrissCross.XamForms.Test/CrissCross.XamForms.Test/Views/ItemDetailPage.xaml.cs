using CrissCross.XamForms.Test.ViewModels;
using Splat;

namespace CrissCross.XamForms.Test.Views
{
    public partial class ItemDetailPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = ViewModel = Locator.Current.GetService<ItemDetailViewModel>();
        }
    }
}