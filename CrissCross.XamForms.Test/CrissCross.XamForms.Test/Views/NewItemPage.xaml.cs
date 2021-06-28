using CrissCross.XamForms.Test.ViewModels;
using CrissCross.XamForms.Test.Models;
using Splat;

namespace CrissCross.XamForms.Test.Views
{
    public partial class NewItemPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = ViewModel = Locator.Current.GetService<NewItemViewModel>();
        }
    }
}