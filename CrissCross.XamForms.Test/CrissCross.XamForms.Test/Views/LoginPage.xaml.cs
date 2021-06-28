using CrissCross.XamForms.Test.ViewModels;
using Splat;
using Xamarin.Forms.Xaml;

namespace CrissCross.XamForms.Test.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = ViewModel = Locator.Current.GetService<LoginViewModel>();
        }
    }
}