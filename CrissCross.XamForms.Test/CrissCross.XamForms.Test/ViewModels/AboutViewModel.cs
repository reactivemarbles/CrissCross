using ReactiveUI;
using System;
using System.Windows.Input;
using Xamarin.Essentials;

namespace CrissCross.XamForms.Test.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            this.WhenSetup().Subscribe(_ =>
            {
                OpenWebCommand = ReactiveCommand.CreateFromTask(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
                NavigateBack = ReactiveCommand.Create(() => this.NavigateBack(), this.CanNavigateBack());
            });
        }

        public ICommand NavigateBack { get; private set; }
        public ICommand OpenWebCommand { get; private set; }
    }
}