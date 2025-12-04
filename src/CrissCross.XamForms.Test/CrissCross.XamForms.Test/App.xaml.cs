// Copyright (c) 2019-2025 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CrissCross.XamForms.Test.Services;
using CrissCross.XamForms.Test.ViewModels;
using CrissCross.XamForms.Test.Views;
using ReactiveUI;
using Splat;
using Xamarin.Forms;

namespace CrissCross.XamForms.Test;

/// <summary>
/// App.
/// </summary>
/// <seealso cref="Application" />
public partial class App : Application
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        InitializeComponent();

        DependencyService.Register<MockDataStore>();
        AppLocator.CurrentMutable.RegisterConstant<AboutViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<ItemDetailViewModel>(new());
        AppLocator.CurrentMutable.RegisterConstant<ItemsViewModel>(new());
        AppLocator.CurrentMutable.Register<LoginViewModel>(() => new());
        AppLocator.CurrentMutable.Register<NewItemViewModel>(() => new());

        AppLocator.CurrentMutable.Register<IViewFor<AboutViewModel>>(() => new AboutPage());
        AppLocator.CurrentMutable.Register<IViewFor<ItemDetailViewModel>>(() => new ItemDetailPage());
        AppLocator.CurrentMutable.Register<IViewFor<ItemsViewModel>>(() => new ItemsPage());
        AppLocator.CurrentMutable.Register<IViewFor<LoginViewModel>>(() => new LoginPage());
        AppLocator.CurrentMutable.Register<IViewFor<NewItemViewModel>>(() => new NewItemPage());

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
