using ReactiveUI;
using System.Windows.Input;
using Xamarin.Essentials;

namespace CrissCross.XamForms.Test.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = ReactiveCommand.CreateFromTask(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
        }

        public ICommand OpenWebCommand { get; }
    }
}