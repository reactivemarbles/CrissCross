using CrissCross;
using ReactiveUI;
using System.Reactive;

namespace CrissCross.XamForms.Test.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public ReactiveCommand<object, Unit> LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = ReactiveCommand.Create<object>(o => OnLoginClicked(o));
        }

        private void OnLoginClicked(object obj)
        {
            this.NavigateToView<AboutViewModel>();
        }
    }
}