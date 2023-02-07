// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CrissCross.XamForms.Test.Services;
using CrissCross.XamForms.Test.ViewModels;
using CrissCross.XamForms.Test.Views;
using ReactiveUI;
using Splat;
using Xamarin.Forms;

namespace CrissCross.XamForms.Test
{
    /// <summary>
    /// App.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Application" />
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
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

        /// <summary>
        /// Application developers override this method to perform actions when the application starts.
        /// </summary>
        /// <remarks>
        /// To be added.
        /// </remarks>
        protected override void OnStart()
        {
        }

        /// <summary>
        /// Application developers override this method to perform actions when the application enters the sleeping state.
        /// </summary>
        /// <remarks>
        /// To be added.
        /// </remarks>
        protected override void OnSleep()
        {
        }

        /// <summary>
        /// Application developers override this method to perform actions when the application resumes from a sleeping state.
        /// </summary>
        /// <remarks>
        /// To be added.
        /// </remarks>
        protected override void OnResume()
        {
        }
    }
}
