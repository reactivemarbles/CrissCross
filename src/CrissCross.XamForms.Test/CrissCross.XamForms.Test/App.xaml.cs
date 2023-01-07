// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CrissCross.XamForms.Test.ViewModels;
using CrissCross.XamForms.Test.Views;
using CrissCross.XamForms.Test.Services;
using ReactiveUI;
using Splat;
using Xamarin.Forms;

namespace CrissCross.XamForms.Test
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            Locator.CurrentMutable.RegisterConstant<AboutViewModel>(new());
            Locator.CurrentMutable.RegisterConstant<ItemDetailViewModel>(new());
            Locator.CurrentMutable.RegisterConstant<ItemsViewModel>(new());
            Locator.CurrentMutable.Register<LoginViewModel>(() => new());
            Locator.CurrentMutable.Register<NewItemViewModel>(() => new());

            Locator.CurrentMutable.Register<IViewFor<AboutViewModel>>(() => new AboutPage());
            Locator.CurrentMutable.Register<IViewFor<ItemDetailViewModel>>(() => new ItemDetailPage());
            Locator.CurrentMutable.Register<IViewFor<ItemsViewModel>>(() => new ItemsPage());
            Locator.CurrentMutable.Register<IViewFor<LoginViewModel>>(() => new LoginPage());
            Locator.CurrentMutable.Register<IViewFor<NewItemViewModel>>(() => new NewItemPage());

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}